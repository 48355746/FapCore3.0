using System;
using System.Collections.Generic;
using SQLGeneration.Parsing;


namespace SQLGeneration.Builders
{
    /// <summary>
    /// Builds a string of a select statement.
    /// </summary>
    public class SelectBuilder : ISelectBuilder, IFilteredCommand
    {
        private readonly List<IJoinItem> _from;
        private readonly List<AliasedProjection> _projection;
        private readonly FilterGroup _where;
        private readonly List<OrderBy> _orderBy;
        private readonly List<IGroupByItem> _groupBy;
        private readonly FilterGroup _having;
        private readonly SourceCollection sources;
        private bool _hasTerminator = false;

        /// <summary>
        /// Initializes a new instance of a SelectBuilder.
        /// </summary>
        public SelectBuilder()
        {
            _from = new List<IJoinItem>();
            _projection = new List<AliasedProjection>();
            _where = new FilterGroup();
            _orderBy = new List<OrderBy>();
            _groupBy = new List<IGroupByItem>();
            _having = new FilterGroup();
            sources = new SourceCollection();
        }

        /// <summary>
        /// Gets or sets how the database will handle duplicate records.
        /// </summary>
        public DistinctQualifier Distinct
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the TOP clause.
        /// </summary>
        public Top Top
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the items that are part of the projection.
        /// </summary>
        public IEnumerable<AliasedProjection> Projection
        {
            get { return _projection; }
        }

        /// <summary>
        /// Adds a projection item to the projection.
        /// </summary>
        /// <param name="item">The projection item to add.</param>
        /// <param name="alias">The alias to refer to the item with.</param>
        /// <returns>The item that was added.</returns>
        public AliasedProjection AddProjection(IProjectionItem item, string alias = null)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            AliasedProjection projection = new AliasedProjection(item, alias);
            _projection.Add(projection);
            return projection;
        }

        /// <summary>
        /// Removes the projection item from the projection.
        /// </summary>
        /// <param name="projection">The projection item to remove.</param>
        /// <returns>True if the item was removed; otherwise, false.</returns>
        public bool RemoveProjection(AliasedProjection projection)
        {
            if (projection == null)
            {
                throw new ArgumentNullException("projection");
            }
            return _projection.Remove(projection);
        }

        /// <summary>
        /// Gets the tables, joins or sub-queries that are projected from.
        /// </summary>
        public IEnumerable<IJoinItem> From
        {
            get { return _from; }
        }

        /// <summary>
        /// Gets the sources that have been added to the builder.
        /// </summary>
        public SourceCollection Sources
        {
            get { return sources; }
        }

        /// <summary>
        /// Adds the given table to the FROM clause.
        /// </summary>
        /// <param name="table">The table to add.</param>
        /// <param name="alias">The optional alias to give the table within the SELECT statement.</param>
        /// <returns>An object to support aliasing the table and defining columns.</returns>
        public AliasedSource AddTable(Table table, string alias = null)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }
            AliasedSource source = new AliasedSource(table, alias);
            sources.AddSource(source.GetSourceName(), source);
            _from.Add(source);
            return source;
        }

        /// <summary>
        /// Adds the given function to the FROM clause.
        /// </summary>
        /// <param name="function">The function to add.</param>
        /// <param name="alias">The optional alias to give the function within the SELECT statement.</param>
        /// <returns>An object to support aliasing the function and defining column.</returns>
        public AliasedSource AddFunction(Function function, string alias = null)
        {
            if (function == null)
            {
                throw new ArgumentNullException("function");
            }
            AliasedSource source = new AliasedSource(function, alias);
            sources.AddSource(source.GetSourceName(), source);
            _from.Add(source);
            return source;
        }

        /// <summary>
        /// Adds the given SELECT statement to the FROM clause.
        /// </summary>
        /// <param name="builder">The SELECT statement to add.</param>
        /// <param name="alias">The optional alias to give the SELECT statement within the SELECT statement.</param>
        /// <returns>An object to support aliasing the SELECT statement and defining columns.</returns>
        public AliasedSource AddSelect(ISelectBuilder builder, string alias = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            AliasedSource source = new AliasedSource(builder, alias);
            sources.AddSource(source.GetSourceName(), source);
            _from.Add(source);
            return source;
        }

        /// <summary>
        /// Adds the given join to the FROM clause.
        /// </summary>
        /// <param name="join">The join to add.</param>
        public void AddJoin(Join join)
        {
            if (join == null)
            {
                throw new ArgumentNullException("join");
            }
            sources.AddSources(join.Sources);
            _from.Add(join);
        }

        /// <summary>
        /// Removes the given table or SELECT statement from the FROM clause.
        /// </summary>
        /// <param name="source">The table or SELECT statement to remove.</param>
        /// <returns>True if the table or SELECT statement was found and removed; otherwise, false.</returns>
        public bool RemoveSource(AliasedSource source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("joinItem");
            }
            string sourceName = source.GetSourceName();
            if (sourceName == null)
            {
                return _from.Remove(source.Source);
            }
            if (sources.Exists(sourceName) &&_from.Remove(source))//wyf添加的时候是source，所以要移除source.下面这个错误： _from.Remove(source.Source))
            {
                sources.Remove(sourceName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the given join from the FROM clause.
        /// </summary>
        /// <param name="join">The join to remove.</param>
        /// <returns>True if the item was found and removed; otherwise, false.</returns>
        public bool RemoveJoin(Join join)
        {
            if (join == null)
            {
                throw new ArgumentNullException("joinItem");
            }
            return _from.Remove(join);
        }

        /// <summary>
        /// Gets the items used to sort the results.
        /// </summary>
        public IEnumerable<OrderBy> OrderBy
        {
            get { return _orderBy; }
        }

        List<OrderBy> ISelectBuilder.OrderByList
        {
            get { return _orderBy; }
        }

        /// <summary>
        /// Adds a sort criteria to the query.
        /// </summary>
        /// <param name="item">The sort criteria to add.</param>
        public void AddOrderBy(OrderBy item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            _orderBy.Add(item);
        }

        /// <summary>
        /// Removes the sort criteria from the query.
        /// </summary>
        /// <param name="item">The order by item to remove.</param>
        /// <returns>True if the item was removed; otherwise, false.</returns>
        public bool RemoveOrderBy(OrderBy item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            return _orderBy.Remove(item);
        }

        /// <summary>
        /// Gets the items that the query is grouped by.
        /// </summary>
        public IEnumerable<IGroupByItem> GroupBy
        {
            get { return _groupBy; }
        }

        /// <summary>
        /// Adds the item to the group by clause of the query.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddGroupBy(IGroupByItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            _groupBy.Add(item);
        }

        /// <summary>
        /// Removes the item from the group by clause of the query.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was removed; otherwise, false.</returns>
        public bool RemoveGroupBy(IGroupByItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            return _groupBy.Remove(item);
        }

        /// <summary>
        /// Gets the filters in the filter group.
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
        /// Gets the filters in the having clause.
        /// </summary>
        public IEnumerable<IFilter> Having
        {
            get { return _having.Filters; }
        }

        /// <summary>
        /// Gets the filter group used to building the having clause.
        /// </summary>
        public FilterGroup HavingFilterGroup
        {
            get { return _having; }
        }

        /// <summary>
        /// Adds the filter to the having clause.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        public void AddHaving(IFilter filter)
        {
            _having.AddFilter(filter);
        }

        /// <summary>
        /// Removes the filter from the having clause.
        /// </summary>
        /// <param name="filter">The filter to remove.</param>
        /// <returns>True if the filter was removed; otherwise, false.</returns>
        public bool RemoveHaving(IFilter filter)
        {
            return _having.RemoveFilter(filter);
        }

        string IRightJoinItem.GetSourceName()
        {
            return null;
        }

        bool IRightJoinItem.IsAliasRequired
        {
            get { return true; }
        }

        bool IValueProvider.IsValueList
        {
            get { return false; }
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
            visitor.VisitSelect(this);
        }
    }
}
