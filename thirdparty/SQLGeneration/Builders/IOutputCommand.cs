using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a SQL statement that has output columns.
    /// </summary>
    public interface IOutputCommand : ICommand
    {

        /// <summary>
        /// Adds the projection item to the output projection items.
        /// </summary>
        /// <param name="item">The column to add.</param>
        /// <param name="alias">The alias for the column.</param>
        AliasedProjection AddOutputProjection(IProjectionItem item, string alias = null);

        /// <summary>
        /// Removes the AliasedProjection from the output projection.
        /// </summary>
        /// <param name="projection">The AliasedProjection to remove.</param>
        /// <returns>True if the AliasedProjection was removed; otherwise, false.</returns>
        bool RemoveOutputProjection(AliasedProjection projection);

    }
}
