using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Adds a condition to a where clause.
    /// </summary>
    public interface IFilter : IVisitableBuilder
    {
        /// <summary>
        /// Gets or sets whether to wrap the filter in parentheses.
        /// </summary>
        bool? WrapInParentheses
        {
            get;
            set;
        }
    }
}
