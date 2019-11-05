using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a comparison that checks that an item is between two values.
    /// </summary>
    public class BetweenFilter : Filter
    {
        private readonly IFilterItem _value;
        private readonly IFilterItem _lowerBound;
        private readonly IFilterItem _upperBound;

        /// <summary>
        /// Initializes a new instance of a NullFilter.
        /// </summary>
        /// <param name="value">The item to check whether is within a range.</param>
        /// <param name="lowerBound">The smallest value the item can be.</param>
        /// <param name="upperBound">The largest value the item can be.</param>
        public BetweenFilter(IFilterItem value, IFilterItem lowerBound, IFilterItem upperBound)
        {
            if (value == null)
            {
                throw new ArgumentNullException("item");
            }
            if (lowerBound == null)
            {
                throw new ArgumentNullException("lowerBound");
            }
            if (upperBound == null)
            {
                throw new ArgumentNullException("upperBound");
            }
            _value = value;
            _lowerBound = lowerBound;
            _upperBound = upperBound;
        }

        /// <summary>
        /// Gets the value being checked.
        /// </summary>
        public IFilterItem Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Gets the least the value can be.
        /// </summary>
        public IFilterItem LowerBound
        {
            get { return _lowerBound; }
        }

        /// <summary>
        /// Gets the most the value can be.
        /// </summary>
        public IFilterItem UpperBound
        {
            get { return _upperBound; }
        }

        /// <summary>
        /// Gets or sets whether to negate the results of the filter.
        /// </summary>
        public bool Not
        {
            get;
            set;
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        protected override void OnAccept(BuilderVisitor visitor)
        {
            visitor.VisitBetweenFilter(this);
        }
    }
}
