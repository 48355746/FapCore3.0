using System;
using System.Collections.Generic;
using SQLGeneration.Parsing;


namespace SQLGeneration.Builders
{
    /// <summary>
    /// Builds a string of an update statement.
    /// </summary>
    public class UpdateBuilder : IFilteredCommand, IOutputCommand
    {
        private readonly AliasedSource _table;
        private readonly IList<Setter> _setters;
        private readonly FilterGroup _where;
        private bool _hasTerminator = false;
        private readonly List<AliasedProjection> _outputProjection;

        /// <summary>
        /// Initializes a new instance of a UpdateBuilder.
        /// </summary>
        /// <param name="table">The table being updated.</param>
        /// <param name="alias">The alias to use to refer to the table.</param>
        public UpdateBuilder(Table table, string alias = null)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            _table = new AliasedSource(table, alias);
            _setters = new List<Setter>();
            _where = new FilterGroup();
            _outputProjection = new List<AliasedProjection>();
        }

        /// <summary>
        /// Gets the table that is being updated.
        /// </summary>
        public AliasedSource Table
        {
            get { return _table; }
        }

        /// <summary>
        /// Gets the columns that are being set.
        /// </summary>
        public IEnumerable<Setter> Setters
        {
            get { return _setters; }
        }

        /// <summary>
        /// Adds the setter to the update statement.
        /// </summary>
        /// <param name="setter">The setter to add.</param>
        public void AddSetter(Setter setter)
        {
            if (setter == null)
            {
                throw new ArgumentNullException("setter");
            }
            _setters.Add(setter);
        }

        /// <summary>
        /// Removes the setter from the update statement.
        /// </summary>
        /// <param name="setter">The setter to remove.</param>
        /// <returns>True if the setter is removed; otherwise, false.</returns>
        public bool RemoveSetter(Setter setter)
        {
            if (setter == null)
            {
                throw new ArgumentNullException("setter");
            }
            return _setters.Remove(setter);
        }

        /// <summary>
        /// Gets the filters in the where clause.
        /// </summary>
        public IEnumerable<IFilter> Where
        {
            get { return _where.Filters; }
        }

        /// <summary>
        /// Gets the filter group used to build the where clause.
        /// </summary>
        public FilterGroup WhereFilterGroup 
        {
            get { return _where; }
        }

        /// <summary>
        /// Adds the filter to the where clause.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        public void AddWhere(IFilter filter)
        {
            _where.AddFilter(filter);
        }

        /// <summary>
        /// Removes the filter from the where clause.
        /// </summary>
        /// <param name="filter">The filter to remove.</param>
        /// <returns>True if the filter was removed; otherwise, false.</returns>
        public bool RemoveWhere(IFilter filter)
        {
            return _where.RemoveFilter(filter);
        }

        /// <summary>
        /// Columns that are put of an Output Clause.
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
            visitor.VisitUpdate(this);
        }
    }
}
