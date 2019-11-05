using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Logically negates another filter.
    /// </summary>
    public class NotFilter : Filter
    {
        private readonly IFilter filter;

        /// <summary>
        /// Initializes a new instance of a NotFilter.
        /// </summary>
        /// <param name="filter">The filter to negate.</param>
        public NotFilter(IFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }
            this.filter = filter;
        }

        /// <summary>
        /// Gets the filter that will be negated.
        /// </summary>
        public IFilter Filter 
        { 
            get { return filter; } 
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        protected override void OnAccept(BuilderVisitor visitor)
        {
            visitor.VisitNotFilter(this);
        }
    }
}
