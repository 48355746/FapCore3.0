using System;

namespace SQLGeneration.Parsing
{
    /// <summary>
    /// Retrieves tokens and provides the ability to return them if they can't be used.
    /// </summary>
    public interface ITokenSource
    {
        /// <summary>
        /// Attempts to retrieve a token matching the definition associated
        /// with the given name.
        /// </summary>
        /// <returns>
        /// A result object describing the token that was found -or- null if no more tokens are found.
        /// </returns>
        TokenResult GetToken();

        /// <summary>
        /// Restores the given token to the front of the token stream.
        /// </summary>
        /// <param name="result">The token to restore.</param>
        void PutBack(TokenResult result);
    }
}
