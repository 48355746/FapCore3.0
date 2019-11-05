using System;

namespace SQLGeneration.Parsing
{
    /// <summary>
    /// Represents an item in a grammar expression.
    /// </summary>
    public interface IExpressionItem
    {
        /// <summary>
        /// Attempts to match the expression item with the values returned by the parser.
        /// </summary>
        /// <param name="parser">The parser currently iterating over the token source.</param>
        /// <param name="itemName">The name of the item in the outer expression.</param>
        /// <returns>The results of the match.</returns>
        MatchResult Match(IParseAttempt parser, string itemName);
    }
}
