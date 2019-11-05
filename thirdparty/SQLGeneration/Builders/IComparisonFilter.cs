using System;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Adds a filter comparison two items together.
    /// </summary>
    public interface IComparisonFilter : IFilter
    {
        /// <summary>
        /// Gets the left hand operand of the filter.
        /// </summary>
        IFilterItem LeftHand
        {
            get;
        }

        /// <summary>
        /// Gets the right hand operand of the comparison.
        /// </summary>
        IFilterItem RightHand
        {
            get;
        }
    }
}
