using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Acts as a placeholder anywhere within the options of a SQL statement.
    /// </summary>
    public class Placeholder : IProjectionItem, IFilterItem, IGroupByItem
    {
        /// <summary>
        /// Initializes a new instance of a Placeholder.
        /// </summary>
        /// <param name="value">The value of the placeholder.</param>
        public Placeholder(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the value of the placeholder.
        /// </summary>
        public string Value
        {
            get;
            private set;
        }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitPlaceholder(this);
        }
    }
}
