using System;
using System.Collections.Generic;

namespace SQLGeneration.Parsing
{
    /// <summary>
    /// Represents an attempt to parse an expression.
    /// </summary>
    public interface IParseAttempt
    {
        /// <summary>
        /// Gets the tokens that were collected during the attempt.
        /// </summary>
        List<TokenResult> Tokens { get; }

        /// <summary>
        /// Attempts to get a token of the given type.
        /// </summary>
        /// <returns>The result of the search.</returns>
        TokenResult GetToken();

        /// <summary>
        /// Creates an attempt to parse a child expression.
        /// </summary>
        /// <returns>A new attempt object.</returns>
        IParseAttempt Attempt();

        /// <summary>
        /// Accepts the attempt as a successful parse, joining the given attempt's tokens
        /// with the current attempt's.
        /// </summary>
        /// <param name="attempt">The child attempt to accept.</param>
        void Accept(IParseAttempt attempt);

        /// <summary>
        /// Rejects the attempt as a failed parse, returning the attempt's token
        /// to the token stream.
        /// </summary>
        void Reject();
    }
}
