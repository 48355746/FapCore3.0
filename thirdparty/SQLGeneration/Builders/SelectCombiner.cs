using System;
using System.Collections.Generic;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Performs a set operation on the results of two queries.
    /// </summary>
    public abstract class SelectCombiner : ISelectBuilder
    {
        private readonly ISelectBuilder leftHand;
        private readonly ISelectBuilder rightHand;
        private readonly List<OrderBy> orderBy;
        private bool _hasTerminator = false;

        /// <summary>
        /// Initializes a new instance of a SelectCombiner.
        /// </summary>
        protected SelectCombiner(ISelectBuilder leftHand, ISelectBuilder rightHand)
        {
            if (leftHand == null)
            {
                throw new ArgumentNullException("leftHand");
            }
            if (rightHand == null)
            {
                throw new ArgumentNullException("rightHand");
            }
            this.leftHand = leftHand;
            this.rightHand = rightHand;
            this.orderBy = new List<OrderBy>();
        }

        /// <summary>
        /// Gets the SELECT command on the left side.
        /// </summary>
        public ISelectBuilder LeftHand
        {
            get { return leftHand; }
        }

        /// <summary>
        /// Gets the SELECT comman on the right side.
        /// </summary>
        public ISelectBuilder RightHand
        {
            get { return rightHand; }
        }

        /// <summary>
        /// Gets the distinct qualifier for the combiner.
        /// </summary>
        public DistinctQualifier Distinct
        {
            get;
            set;
        }

        SourceCollection ISelectBuilder.Sources
        { 
            get { return new SourceCollection(); } 
        }

        List<OrderBy> ISelectBuilder.OrderByList
        {
            get { return orderBy; }
        }

        /// <summary>
        /// Gets the items used to sort the results.
        /// </summary>
        public IEnumerable<OrderBy> OrderBy
        {
            get { return orderBy; }
        }

        /// <summary>
        /// Adds a sort criteria to the query.
        /// </summary>
        /// <param name="item">The sort criteria to add.</param>
        public void AddOrderBy(OrderBy item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            orderBy.Add(item);
        }

        /// <summary>
        /// Removes the sort criteria from the query.
        /// </summary>
        /// <param name="item">The order by item to remove.</param>
        /// <returns>True if the item was removed; otherwise, false.</returns>
        public bool RemoveOrderBy(OrderBy item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            return orderBy.Remove(item);
        }

        string IRightJoinItem.GetSourceName()
        {
            return null;
        }

        bool IRightJoinItem.IsAliasRequired
        {
            get { return true; }
        }

        bool IValueProvider.IsValueList
        {
            get { return false; }
        }

        /// <summary>
        /// Gets whether this command has a terminator.
        /// </summary>
        public bool HasTerminator
        {
            get
            {
                return _hasTerminator;
            }
            set
            {
                _hasTerminator = value;
            }
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
    }
}
