using System;
using System.Collections.Generic;

namespace SQLGeneration.Parsing
{
    /// <summary>
    /// Represents a sequence of tokens and sub-expressions.
    /// </summary>
    public sealed class ExpressionDefinition
    {
        private readonly List<ExpressionItem> items;

        /// <summary>
        /// Initializes a new instance of an ExpressionDefinition.
        /// </summary>
        /// <param name="expressionType">An optional identifier for the expression's type.</param>
        internal ExpressionDefinition(string expressionType)
        {
            ExpressionType = expressionType;
            items = new List<ExpressionItem>();
        }

        /// <summary>
        /// Gets the type of the expression.
        /// </summary>
        public string ExpressionType
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates that the given expression is expected next, giving it a
        /// name and specifying whether it is required.
        /// </summary>
        /// <param name="name">The name that the expression will be identified with in the outer expression.</param>
        /// <param name="isRequired">Indicates whether the expression is required in order for the outer expression to be a match.</param>
        /// <param name="expression">The expression to add to the outer expressions.</param>
        /// <returns>The updated definition.</returns>
        public ExpressionDefinition Add(string name, bool isRequired, Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            items.Add(new ExpressionItem(name, isRequired, expression));
            return this;
        }

        /// <summary>
        /// Indicates that the given options list is the next expected, giving it a
        /// name and specifying whether it is required.
        /// </summary>
        /// <param name="isRequired">Indicates whether at least one of the options is required in order for the outer expression to be a match.</param>
        /// <param name="options">The options to add to the outer expression.</param>
        /// <returns>The updated definition.</returns>
        public ExpressionDefinition Add(bool isRequired, Options options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            items.Add(new ExpressionItem(String.Empty, isRequired, options));
            return this;
        }

        /// <summary>
        /// Indicates that the given token is the next expected, giving it a
        /// name and specifying whether it is required.
        /// </summary>
        /// <param name="itemName">The name that the token will be identified with in the outer expression.</param>
        /// <param name="isRequired">Indicates whether the token is required in order for the outer expression to be a match.</param>
        /// <param name="token">The token to add to the outer expression.</param>
        /// <returns>The updated definition.</returns>
        public ExpressionDefinition Add(string itemName, bool isRequired, Token token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }
            items.Add(new ExpressionItem(itemName, isRequired, token));
            return this;
        }

        /// <summary>
        /// Indicates that the given sub-expression is the next expected, giving it a
        /// name and specifying whether it is required.
        /// </summary>
        /// <param name="itemName">The name that the sub-expression will be identified with in the outer expression.</param>
        /// <param name="isRequired">Indicates whether the sub-expression is required in order for the expression to match.</param>
        /// <param name="definition">The definition for the sub-expression.</param>
        /// <returns>The updated definition.</returns>
        public ExpressionDefinition Add(string itemName, bool isRequired, ExpressionDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }
            IExpressionItem item = new Expression(definition);
            items.Add(new ExpressionItem(itemName, isRequired, item));
            return this;
        }

        /// <summary>
        /// Gets the items making up the expression.
        /// </summary>
        internal IEnumerable<ExpressionItem> Items
        {
            get { return items; }
        }
    }
}
