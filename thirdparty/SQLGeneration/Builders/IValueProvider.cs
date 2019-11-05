using System;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a source of values in an insert statement.
    /// </summary>
    public interface IValueProvider : IFilterItem
    {
        /// <summary>
        /// Gets or sets whether the value provider gets its values from a value list.
        /// </summary>
        bool IsValueList
        {
            get;
        }
    }
}
