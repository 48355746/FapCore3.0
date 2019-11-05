using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents the first source in a series of joins.
    /// </summary>
    internal class JoinStart : Join
    {
        private readonly AliasedSource source;

        /// <summary>
        /// Initializes a new instance of a JoinStart.
        /// </summary>
        /// <param name="source">The first source in a series of joins.</param>
        public JoinStart(AliasedSource source)
            : base(source)
        {
            this.source = source;
        }

        /// <summary>
        /// Gets the source.
        /// </summary>
        internal AliasedSource Source
        {
            get { return source; }
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        protected override void OnAccept(BuilderVisitor visitor)
        {
            IJoinItem joinItem = source;
            joinItem.Accept(visitor);
        }
    }
}
