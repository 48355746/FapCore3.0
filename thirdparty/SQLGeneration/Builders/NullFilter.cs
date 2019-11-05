using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a comparison between a value and null.
    /// </summary>
    public class NullFilter : Filter
    {
        private readonly IFilterItem _item;

        /// <summary>
        /// Initializes a new instance of a NullFilter.
        /// </summary>
        /// <param name="item">The item to check whether or not is null.</param>
        public NullFilter(IFilterItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            _item = item;
        }

        /// <summary>
        /// Gets the item being compared to null.
        /// </summary>
        public IFilterItem LeftHand
        {
            get { return _item; }
        }

        /// <summary>
        /// Gets or sets whether to negate the comparison.
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
            visitor.VisitNullFilter(this);
        }
    }
}
