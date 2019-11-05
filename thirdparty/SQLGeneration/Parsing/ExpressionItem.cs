using System;

namespace SQLGeneration.Parsing
{
    /// <summary>
    /// Holds information descibing an expression or token's role in an outer expression.
    /// </summary>
    internal sealed class ExpressionItem
    {
        /// <summary>
        /// Initializes a new instance of an ExpressionItem.
        /// </summary>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="isRequired">Specifies whether the item missing results in the outer expression not matching.</param>
        /// <param name="item">The actual item that is expected.</param>
        public ExpressionItem(string itemName, bool isRequired, IExpressionItem item)
        {
            ItemName = itemName;
            IsRequired = isRequired;
            Item = item;
        }

        /// <summary>
        /// Gets the name that the outer expression refers to the item with.
        /// </summary>
        public string ItemName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets whether the item is required in order for the outer expression to match.
        /// </summary>
        public bool IsRequired
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        public IExpressionItem Item
        {
            get;
            private set;
        }
    }
}
