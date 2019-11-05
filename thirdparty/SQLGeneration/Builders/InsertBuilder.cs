using System;
using System.Collections.Generic;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Builds a string of an insert statement.
    /// </summary>
    public class InsertBuilder : IOutputCommand
    {
        private readonly AliasedSource _table;
        private readonly List<Column> _columns;

        private readonly List<AliasedProjection> _outputProjection;

      //  private readonly List<Column> _outputColumns;
        private readonly IValueProvider _values;
        private bool _hasTerminator = false;

        /// <summary>
        /// Initializes a new instance of a InsertBuilder.
        /// </summary>
        /// <param name="table">The table being inserted into.</param>
        /// <param name="values">The values to insert into the table.</param>
        /// <param name="alias">The alias to use to refer to the table.</param>
        public InsertBuilder(Table table, IValueProvider values, string alias = null)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            _table = new AliasedSource(table, alias);
            _columns = new List<Column>();
            _outputProjection = new List<AliasedProjection>();
          //  _outputColumns = new List<Column>();
            _values = values;
        }

        /// <summary>
        /// Gets the table that is being inserted into.
        /// </summary>
        public AliasedSource Table
        {
            get { return _table; }
        }

        /// <summary>
        /// Gets the columns being inserted into.
        /// </summary>
        public IEnumerable<Column> Columns
        {
            get { return _columns; }
        }

      

        /// <summary>
        /// Adds the column to the insert statement.
        /// </summary>
        /// <param name="column">The column to add.</param>
        public void AddColumn(Column column)
        {
            if (column == null)
            {
                throw new ArgumentNullException("column");
            }
            _columns.Add(column);
        }      

        /// <summary>
        /// Removes the column from the insert statement.
        /// </summary>
        /// <param name="column">The column to remove.</param>
        /// <returns>True if the column was removed; otherwise, false.</returns>
        public bool RemoveColumn(Column column)
        {
            if (column == null)
            {
                throw new ArgumentNullException("column");
            }
            return _columns.Remove(column);
        }

        /// <summary>
        /// Gets the output projection items.
        /// </summary>
        public IEnumerable<AliasedProjection> Output
        {
            get { return _outputProjection; }
        }

        /// <summary>
        /// Adds a projection item to the output projection.
        /// </summary>
        /// <param name="item">The projection item to add.</param>
        /// <param name="alias">The alias to refer to the item with.</param>
        /// <returns>The item that was added.</returns>
        public AliasedProjection AddOutputProjection(IProjectionItem item, string alias = null)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            AliasedProjection projection = new AliasedProjection(item, alias);
            _outputProjection.Add(projection);
            return projection;
        }        

        /// <summary>
        /// Removes the projection item from the output projection.
        /// </summary>
        /// <param name="projection">The projection item to remove.</param>
        /// <returns>True if the item was removed; otherwise, false.</returns>
        public bool RemoveOutputProjection(AliasedProjection projection)
        {
            if (projection == null)
            {
                throw new ArgumentNullException("projection");
            }
            return _outputProjection.Remove(projection);
        }

        ///// <summary>
        ///// Adds the column as an output column.
        ///// </summary>
        ///// <param name="column">The column to add.</param>
        //public void AddOutputColumn(Column column)
        //{
        //    if (column == null)
        //    {
        //        throw new ArgumentNullException("column");
        //    }
        //    _outputColumns.Add(column);
        //}

        ///// <summary>
        ///// Removes the column from the output columns.
        ///// </summary>
        ///// <param name="column">The column to remove.</param>
        ///// <returns>True if the column was removed; otherwise, false.</returns>
        //public bool RemoveOutputColumn(Column column)
        //{
        //    if (column == null)
        //    {
        //        throw new ArgumentNullException("column");
        //    }
        //    return _outputColumns.Remove(column);
        //}

        /// <summary>
        /// Gets the list of values or select statement that populates the insert.
        /// </summary>
        public IValueProvider Values
        {
            get { return _values; }
        }

        /// <summary>
        /// Gets whether this command has a terminator.
        /// </summary>
        public bool HasTerminator
        {
            get
            {
                return _hasTerminator;
            }
            set
            {
                _hasTerminator = value;
            }
        }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitInsert(this);
        }


    }
}
