using System;
using System.Collections.Generic;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a join that is filtered with an ON expression.
    /// </summary>
    public abstract class FilteredJoin : BinaryJoin
    {
        private readonly FilterGroup on;

        /// <summary>
        /// Initializes a new instance of a FilteredJoin.
        /// </summary>
        /// <param name="left">The left hand item in the join.</param>
        /// <param name="right">The right hand item in the join.</param>
        protected FilteredJoin(Join left, AliasedSource right)
            : base(left, right)
        {
            on = new FilterGroup(Conjunction.And);
        }

        /// <summary>
        /// Sets the condition on which the source is joined with the other tables.
        /// </summary>
        /// <param name="filterGenerator">A function that creates the join.</param>
        /// <returns>The current join.</returns>
        public Join On(Func<Join, IFilter> filterGenerator)
        {
            if (filterGenerator == null)
            {
                throw new ArgumentNullException("filterGenerator");
            }
            on.Clear();
            IFilter filter = filterGenerator(this);
            on.AddFilter(filter);
            return this;
        }

        /// <summary>
        /// Gets the filters by which the left and right hand items are joined.
        /// </summary>
        public IEnumerable<IFilter> OnFilters
        {
            get { return on.Filters; }
        }

        /// <summary>
        /// Gets the filter group.
        /// </summary>
        public FilterGroup OnFilterGroup
        {
            get { return on; }
        }

        /// <summary>
        /// Adds the filter to the group.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        public void AddOnFilter(IFilter filter)
        {
            on.AddFilter(filter);
        }

        /// <summary>
        /// Removes the filter from the group.
        /// </summary>
        /// <param name="filter">The filter to remove.</param>
        /// <returns>True if the filter was removed; otherwise, false.</returns>
        public bool RemoveOnFilter(IFilter filter)
        {
            return on.RemoveFilter(filter);
        }
    }
}
