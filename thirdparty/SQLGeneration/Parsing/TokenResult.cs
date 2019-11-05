using System;

namespace SQLGeneration.Parsing
{
    /// <summary>
    /// Holds the results of trying to extract a token from a token source.
    /// </summary>
    public sealed class TokenResult
    {
        /// <summary>
        /// Initializes a new instance of a TokenResult.
        /// </summary>
        internal TokenResult(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets the name of the token type that was requested.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the value that was found, whether it was the requested
        /// type or not -or- null if no more tokens were available from
        /// the token source.
        /// </summary>
        public string Value
        {
            get;
            private set;
        }
    }
}
