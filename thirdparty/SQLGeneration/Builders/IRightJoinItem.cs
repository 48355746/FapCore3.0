using System;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents the right hand side of a join.
    /// </summary>
    public interface IRightJoinItem : IJoinItem
    {
        /// <summary>
        /// Gets whether the join item is a SELECT statement.
        /// </summary>
        bool IsAliasRequired
        {
            get;
        }

        /// <summary>
        /// Gets the name of the table -or- null if the item is a SELECT statement.
        /// </summary>
        /// <returns></returns>
        string GetSourceName();
    }
}
