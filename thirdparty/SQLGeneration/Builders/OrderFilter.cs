using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a filter that checks that a value is less than, greater than or equal to.
    /// </summary>
    public abstract class OrderFilter : Filter, IComparisonFilter
    {
        private readonly IFilterItem _leftHand;
        private readonly IFilterItem _rightHand;

        /// <summary>
        /// Initializes a new instance of a OrderFilter.
        /// </summary>
        /// <param name="leftHand">The left hand side of the comparison.</param>
        /// <param name="rightHand">The right hand side of the comparison.</param>
        protected OrderFilter(IFilterItem leftHand, IFilterItem rightHand)
        {
            if (leftHand == null)
            {
                throw new ArgumentNullException("leftHand");
            }
            if (rightHand == null)
            {
                throw new ArgumentNullException("rightHand");
            }
            _leftHand = leftHand;
            _rightHand = rightHand;
        }

        /// <summary>
        /// Gets the left hand operand of the filter.
        /// </summary>
        public IFilterItem LeftHand
        {
            get { return _leftHand; }
        }

        /// <summary>
        /// Gets the right hand operand of the comparison.
        /// </summary>
        public IFilterItem RightHand
        {
            get { return _rightHand; }
        }
    }
}
