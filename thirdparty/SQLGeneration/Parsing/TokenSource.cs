using System;
using System.Collections.Generic;

namespace SQLGeneration.Parsing
{
    /// <summary>
    /// Retrieves tokens and provides the ability to return them if they can't be used.
    /// </summary>
    internal abstract class TokenSource : ITokenSource
    {
        private readonly Stack<TokenResult> undo;

        /// <summary>
        /// Initializes a new instance of a TokenSource.
        /// </summary>
        protected TokenSource()
        {
            undo = new Stack<TokenResult>();
        }

        /// <summary>
        /// Attempts to retrieve a token matching the definition associated
        /// with the given name.
        /// </summary>
        /// <returns>
        /// A result object describing the token that was found -or- null if no more tokens are found.
        /// </returns>
        public TokenResult GetToken()
        {
            if (undo.Count == 0)
            {
                return GetNextToken();
            }
            return undo.Pop();
        }

        /// <summary>
        /// Retrieves the next token from the source if the undo buffer is empty.
        /// </summary>
        /// <returns>The next token -or- null if there are no more tokens.</returns>
        protected abstract TokenResult GetNextToken();

        /// <summary>
        /// Restores the given token to the front of the token stream.
        /// </summary>
        /// <param name="result">The token to restore.</param>
        public void PutBack(TokenResult result)
        {
            undo.Push(result);
        }
    }
}
