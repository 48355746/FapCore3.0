using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a filter that performs a univeral or existential comparison.
    /// </summary>
    public abstract class QuantifierFilter : Filter
    {
        /// <summary>
        /// Initializes a new insstance of a QuantifierFilter.
        /// </summary>
        /// <param name="leftHand">The value being compared to the set of values.</param>
        /// <param name="quantifier">The quantifier to use to compare the value to the set of values.</param>
        /// <param name="valueProvider">The source of values.</param>
        protected QuantifierFilter(IFilterItem leftHand, Quantifier quantifier, IValueProvider valueProvider)
        {
            if (leftHand == null)
            {
                throw new ArgumentNullException("leftHand");
            }
            if (valueProvider == null)
            {
                throw new ArgumentNullException("valueProvider");
            }
            LeftHand = leftHand;
            Quantifier = quantifier;
            ValueProvider = valueProvider;
        }

        /// <summary>
        /// Gets the value being compared to the set of values.
        /// </summary>
        public IFilterItem LeftHand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the quantifier being used to compare the items.
        /// </summary>
        public Quantifier Quantifier
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the source of the values.
        /// </summary>
        public IValueProvider ValueProvider
        {
            get;
            private set;
        }
    }
}
