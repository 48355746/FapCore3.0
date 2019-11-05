using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a database column.
    /// </summary>
    public class Column : IProjectionItem, IGroupByItem, IFilterItem
    {
        /// <summary>
        /// Initializes a new instance of a Column that is not associated
        /// with a specific table.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        public Column(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of a Column.
        /// </summary>
        /// <param name="source">The column source that the column belongs to.</param>
        /// <param name="name">The name of the column.</param>
        internal Column(AliasedSource source, string name)
        {
            Source = source;
            Name = name;
        }

        /// <summary>
        /// Gets the table that the column belongs to.
        /// </summary>
        public AliasedSource Source
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets whether the column should be qualified with the source.
        /// </summary>
        public bool? Qualify
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitColumn(this);
        }
    }
  
}
