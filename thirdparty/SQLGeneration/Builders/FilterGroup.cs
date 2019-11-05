using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SQLGeneration.Parsing;


namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a grouping of filters.
    /// </summary>
    public class FilterGroup : Filter
    {
        private readonly Conjunction _conjunction;
        private readonly List<IFilter> _filters;

        /// <summary>
        /// Initializes a new instance of a FilterGroup.
        /// </summary>
        public FilterGroup(Conjunction conjunction = Conjunction.And, params IFilter[] filters)
        {
            if (!Enum.IsDefined(typeof(Conjunction), conjunction))
            {
                throw new ArgumentException("Invalid enum", "conjunction");
            }
            if (filters == null)
            {
                throw new ArgumentNullException("filters");
            }
            _conjunction = conjunction;
            _filters = new List<IFilter>();
            foreach (IFilter filter in filters)
            {
                AddFilter(filter);
            }
        }

        /// <summary>
        /// Gets the conjunction used to combine the filters within the group.
        /// </summary>
        public Conjunction Conjunction
        {
            get { return _conjunction; }
        }
        
        /// <summary>
        /// Gets the filters in the filter group.
        /// </summary>
        public IEnumerable<IFilter> Filters
        {
            get
            {
                return _filters;
            }
        }

        /// <summary>
        /// Adds the filter to the group.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        public void AddFilter(IFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            _filters.Add(filter);
        }

        /// <summary>
        /// Removes the filter from the group.
        /// </summary>
        /// <param name="filter">The filter to remove.</param>
        /// <returns>True if the filter was removed; otherwise, false.</returns>
        public bool RemoveFilter(IFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            return _filters.Remove(filter);
        }

        /// <summary>
        /// Clears the group of all filters.
        /// </summary>
        public void Clear()
        {
            _filters.Clear();
        }

        /// <summary>
        /// Gets whether there are any filters in the group.
        /// </summary>
        public bool HasFilters
        {
            get { return _filters.Count > 0; }
        }

        /// <summary>
        /// Gets the number of items in the filter.
        /// </summary>
        internal int Count
        {
            get { return _filters.Count; }
        }

        /// <summary>
        /// Compacts the filter group by combining it with child filter
        /// groups where doing so won't alter the semantics of the overall
        /// expression.
        /// </summary>
        /// <remarks>
        /// This is meant to be used by the command
        /// builders to reduce the overhead of deeply nested filters.
        /// </remarks>
        public void Optimize()
        {
            List<IFilter> updates = new List<IFilter>();
            bool wasOptimized = optimize(updates);
            if (wasOptimized)
            {
                // replace our filters
                _filters.Clear();
                _filters.AddRange(updates);
            }
        }

        private bool optimize(List<IFilter> updates)
        {
            bool wasOptimized = false;
            foreach (IFilter filter in _filters)
            {
                FilterGroup innerGroup = filter as FilterGroup;
                if (innerGroup == null)
                {
                    updates.Add(filter);
                }
                else
                {
                    // make sure to optimize the child filter group first
                    List<IFilter> filters = new List<IFilter>();
                    bool wereChildrenOptimized = innerGroup.optimize(filters);

                    // if the code explicitly requests parenthesis, keep them in place
                    bool requiresParenthesis = innerGroup.WrapInParentheses == true;
                    // if a filter group only has one item, its conjunction is ignored
                    bool hasUnusedConjunction = filters.Count == 1;
                    // if the filter group has the same conjunction, then we can safely compact
                    bool hasMatchingConjunction = innerGroup._conjunction == _conjunction;

                    bool canOptimize = !requiresParenthesis && (hasUnusedConjunction || hasMatchingConjunction);
                    if (canOptimize)
                    {
                        updates.AddRange(filters);
                        wasOptimized = true;
                    }
                    else if (wereChildrenOptimized)
                    {
                        FilterGroup newGroup = new FilterGroup(innerGroup._conjunction, filters.ToArray());
                        if (requiresParenthesis)
                        {
                            newGroup.WrapInParentheses = true;
                        }
                        updates.Add(newGroup);
                        wasOptimized = true;
                    }
                    else
                    {
                        updates.Add(innerGroup);
                    }
                }
            }
            return wasOptimized;
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        protected override void OnAccept(BuilderVisitor visitor)
        {
            visitor.VisitFilterGroup(this);
        }
    }
}
