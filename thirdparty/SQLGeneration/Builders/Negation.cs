using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Negates an arithmetic expression.
    /// </summary>
    public class Negation : IProjectionItem, IFilterItem, IGroupByItem
    {
        /// <summary>
        /// Initializes a new instance of a Negation.
        /// </summary>
        /// <param name="item">The item to negate.</param>
        public Negation(IProjectionItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            Item = item;
        }

        /// <summary>
        /// Gets the item that will be negated.
        /// </summary>
        public IProjectionItem Item 
        { 
            get; 
            private set; 
        }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitNegation(this);
        }
    }
}
