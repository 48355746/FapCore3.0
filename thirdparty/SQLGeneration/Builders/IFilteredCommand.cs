using System;
using System.Collections.Generic;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a command that can be filtered.
    /// </summary>
    public interface IFilteredCommand : ICommand
    {
        /// <summary>
        /// Gets the filters in the where clause.
        /// </summary>
        IEnumerable<IFilter> Where
        {
            get;
        }

        /// <summary>
        /// Gets the filter group used to build the command's where clause.
        /// </summary>
        FilterGroup WhereFilterGroup
        {
            get;
        }

        /// <summary>
        /// Adds the filter to the where clause.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        void AddWhere(IFilter filter);

        /// <summary>
        /// Removes the filter from the where clause.
        /// </summary>
        /// <param name="filter">The filter to remove.</param>
        /// <returns>True if the filter was removed; otherwise, false.</returns>
        bool RemoveWhere(IFilter filter);
    }
}
