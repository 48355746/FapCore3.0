using System;

namespace SQLGeneration.Parsing
{
    /// <summary>
    /// Holds the result of trying to match an expression item.
    /// </summary>
    public sealed class MatchResult
    {
        private readonly MatchResultCollection matches;

        /// <summary>
        /// Initializes a new instance of a MatchResult.
        /// </summary>
        internal MatchResult()
        {
            matches = new MatchResultCollection();
        }

        /// <summary>
        /// Gets the name of the item as it will be referred to in the outer expression.
        /// </summary>
        public string ItemName
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets whether the expression item was matched against the parser.
        /// </summary>
        public bool IsMatch
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets any information to be passed along with the match results.
        /// </summary>
        public object Context
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the match result for sub-expressions or tokens within the current expression.
        /// This collection will be empty for tokens.
        /// </summary>
        public MatchResultCollection Matches
        {
            get { return matches; }
        }
    }
}
