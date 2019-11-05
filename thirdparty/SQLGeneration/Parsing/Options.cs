using System;
using System.Collections.Generic;

namespace SQLGeneration.Parsing
{
    /// <summary>
    /// Represents a list of possible expressions or tokens that the parser should try.
    /// </summary>
    public sealed class Options : IExpressionItem
    {
        private readonly List<ExpressionItem> options;

        /// <summary>
        /// Initializes a new instance of an Options.
        /// </summary>
        internal Options()
        {
            options = new List<ExpressionItem>();
        }

        /// <summary>
        /// Gets the option items.
        /// </summary>
        internal IEnumerable<ExpressionItem> Items
        {
            get { return options; }
        }

        /// <summary>
        /// Indicates that the given item is the next expected, giving it a
        /// name and specifying whether it is required.
        /// </summary>
        /// <param name="itemName">The name that the token will be identified with in the outer expression.</param>
        /// <param name="item">The expression item to add to the sequence.</param>
        /// <returns>The updated expression.</returns>
        public Options Add(string itemName, IExpressionItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            options.Add(new ExpressionItem(itemName, false, item));
            return this;
        }

        /// <summary>
        /// Indicates that the given sub-expression is the next expected, giving it a
        /// name and specifying whether it is required.
        /// </summary>
        /// <param name="itemName">The name that the token will be identified with in the outer expression.</param>
        /// <param name="definition">The definition for the sub-expression.</param>
        /// <returns>The updated expression.</returns>
        public Options Add(string itemName, ExpressionDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }
            IExpressionItem item = new Expression(definition);
            options.Add(new ExpressionItem(itemName, false, item));
            return this;
        }

        /// <summary>
        /// Attempts to match the expression item with the values returned by the parser.
        /// </summary>
        /// <param name="attempt">The parser currently iterating over the token source.</param>
        /// <param name="itemName">This value will be empty for an options list.</param>
        /// <returns>The results of the match.</returns>
        public MatchResult Match(IParseAttempt attempt, string itemName)
        {
            foreach (ExpressionItem option in options)
            {
                IParseAttempt nextAttempt = attempt.Attempt();
                MatchResult innerResult = option.Item.Match(nextAttempt, option.ItemName);
                if (innerResult.IsMatch)
                {
                    attempt.Accept(nextAttempt);
                    return innerResult;
                }
                nextAttempt.Reject();
            }
            return new MatchResult() { IsMatch = false };
        }
    }
}
