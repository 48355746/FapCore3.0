using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a portion of a series of joins.
    /// </summary>
    public abstract class Join : IJoinItem
    {
        private readonly SourceCollection sources;

        /// <summary>
        /// Initializes a new instance of a Join.
        /// </summary>
        /// <param name="source">The source for the current </param>
        protected Join(AliasedSource source)
            : this(new SourceCollection(), source)
        {
        }

        /// <summary>
        /// Initializes a new instance of a Join.
        /// </summary>
        /// <param name="other">The previous join in the sequence.</param>
        /// <param name="source">The source for the current </param>
        protected Join(Join other, AliasedSource source)
            : this(new SourceCollection(other.sources), source)
        {
        }

        private Join(SourceCollection sourceCollection, AliasedSource source)
        {
            this.sources = sourceCollection;
            string newSourceName = source.GetSourceName();
            if (newSourceName != null)
            {
                this.sources.AddSource(newSourceName, source);
            }
        }

        /// <summary>
        /// Starts creating a BinaryJoin.
        /// </summary>
        /// <param name="source">The table or select statement to start the join series with.</param>
        /// <param name="alias">The alias to give the item.</param>
        /// <returns>The first join item.</returns>
        public static Join From(IRightJoinItem source, string alias = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            AliasedSource start = new AliasedSource(source, alias);
            return new JoinStart(start);
        }

        /// <summary>
        /// Gets or sets whether the join should be wrapped in parentheses.
        /// </summary>
        public bool? WrapInParentheses
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a collection of the table and SELECT statements within the join.
        /// </summary>
        public SourceCollection Sources
        {
            get { return sources; }
        }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            OnAccept(visitor);
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        protected abstract void OnAccept(BuilderVisitor visitor);

        /// <summary>
        /// Creates a new join where the given item is inner joined with the existing join items.
        /// </summary>
        /// <param name="item">The item to join with.</param>
        /// <param name="alias">The alias to give the item.</param>
        /// <returns>The new join.</returns>
        public FilteredJoin InnerJoin(IRightJoinItem item, string alias = null)
        {
            AliasedSource source = new AliasedSource(item, alias);
            return new InnerJoin(this, source);
        }

        /// <summary>
        /// Creates a new join where the given item is left outer joined with the existing join items.
        /// </summary>
        /// <param name="item">The item to join with.</param>
        /// <param name="alias">The alias to give the item.</param>
        /// <returns>The new join.</returns>
        public FilteredJoin LeftOuterJoin(IRightJoinItem item, string alias = null)
        {
            AliasedSource source = new AliasedSource(item, alias);
            return new LeftOuterJoin(this, source);
        }

        /// <summary>
        /// Creates a new join where the given item is right outer joined with the existing join items.
        /// </summary>
        /// <param name="item">The item to join with.</param>
        /// <param name="alias">The alias to give the item.</param>
        /// <returns>The new join.</returns>
        public FilteredJoin RightOuterJoin(IRightJoinItem item, string alias = null)
        {
            AliasedSource source = new AliasedSource(item, alias);
            return new RightOuterJoin(this, source);
        }

        /// <summary>
        /// Creates a new join where the given item is full outer joined with the existing join items.
        /// </summary>
        /// <param name="item">The item to join with.</param>
        /// <param name="alias">The alias to give the item.</param>
        /// <returns>The new join.</returns>
        public FilteredJoin FullOuterJoin(IRightJoinItem item, string alias = null)
        {
            AliasedSource source = new AliasedSource(item, alias);
            return new FullOuterJoin(this, source);
        }

        /// <summary>
        /// Creates a new join where the given item is cross joined with the existing join items.
        /// </summary>
        /// <param name="item">The item to join with.</param>
        /// <param name="alias">The alias to give the item.</param>
        /// <returns>The new join.</returns>
        public Join CrossJoin(IRightJoinItem item, string alias = null)
        {
            AliasedSource source = new AliasedSource(item, alias);
            return new CrossJoin(this, source);
        }
    }
}
