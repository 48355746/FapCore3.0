using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Allows a column to be optionally referred to using an alias.
    /// </summary>
    public class AliasedProjection
    {
        /// <summary>
        /// Initializes a new instance of an AliasedProjection.
        /// </summary>
        /// <param name="item">The projection item.</param>
        /// <param name="alias">The alias to refer to the item with.</param>
        internal AliasedProjection(IProjectionItem item, string alias = null)
        {
            ProjectionItem = item;
            Alias = alias;
        }

        /// <summary>
        /// Gets the projection item.
        /// </summary>
        public IProjectionItem ProjectionItem
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Gets or sets the alias for the projection item.
        /// </summary>
        public string Alias
        {
            get;
            private set;
        }
    }
}
