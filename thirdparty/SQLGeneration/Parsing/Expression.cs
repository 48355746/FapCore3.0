using System;

namespace SQLGeneration.Parsing
{
    /// <summary>
    /// Represents a sub-expression made up of tokens and sub-expressions.
    /// </summary>
    public sealed class Expression : IExpressionItem
    {
        private readonly ExpressionDefinition expression;

        /// <summary>
        /// Initializes a new instance of an Expression.
        /// </summary>
        /// <param name="expression">The sequence of tokens and sub-expressions expected to appear.</param>
        internal Expression(ExpressionDefinition expression)
        {
            this.expression = expression;
        }

        /// <summary>
        /// Attempts to match the expression item with the values returned by the parser.
        /// </summary>
        /// <param name="attempt">The parser currently iterating over the token source.</param>
        /// <param name="itemName">The name of the item in the outer expression.</param>
        /// <returns>The results of the match.</returns>
        public MatchResult Match(IParseAttempt attempt, string itemName)
        {
            MatchResult result = new MatchResult() { ItemName = itemName, IsMatch = true };
            foreach (ExpressionItem detail in expression.Items)
            {
                IParseAttempt nextAttempt = attempt.Attempt();
                MatchResult innerResult = detail.Item.Match(nextAttempt, detail.ItemName);
                if (innerResult.IsMatch)
                {
                    attempt.Accept(nextAttempt);
                    result.Matches.Add(innerResult);
                }
                else
                {
                    nextAttempt.Reject();
                    if (detail.IsRequired)
                    {
                        result.IsMatch = false;
                        return result;
                    }
                }
            }
            return result;
        }
    }
}
