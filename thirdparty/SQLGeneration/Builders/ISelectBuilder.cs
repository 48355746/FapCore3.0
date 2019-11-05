using System;
using System.Collections.Generic;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Builds SELECT statements.
    /// </summary>
    public interface ISelectBuilder : ICommand, IRightJoinItem, IProjectionItem, IValueProvider
    {
        /// <summary>
        /// Gets the source collection for the builder.
        /// </summary>
        SourceCollection Sources { get; }

        /// <summary>
        /// Gets the items used to sort the results.
        /// </summary>
        IEnumerable<OrderBy> OrderBy
        {
            get;
        }

        /// <summary>
        /// Gets the order by items.
        /// </summary>
        List<OrderBy> OrderByList
        {
            get;
        }

        /// <summary>
        /// Adds a sort criteria to the query.
        /// </summary>
        /// <param name="item">The sort criteria to add.</param>
        void AddOrderBy(OrderBy item);

        /// <summary>
        /// Removes the sort criteria from the query.
        /// </summary>
        /// <param name="item">The order by item to remove.</param>
        /// <returns>True if the item was removed; otherwise, false.</returns>
        bool RemoveOrderBy(OrderBy item);
    }
}
