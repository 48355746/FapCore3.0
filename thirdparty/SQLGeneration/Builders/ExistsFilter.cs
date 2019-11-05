using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a test for the presence of a record in a sub-query.
    /// </summary>
    public class ExistsFilter : Filter
    {
        /// <summary>
        /// Initializes a new instance of an ExistsFilter.
        /// </summary>
        /// <param name="select"></param>
        public ExistsFilter(ISelectBuilder select)
        {
            if (select == null)
            {
                throw new ArgumentNullException("select");
            }
            Select = select;
        }

        /// <summary>
        /// Gets the select builder used to test for the existance of a record.
        /// </summary>
        public ISelectBuilder Select
        {
            get;
            private set;
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        protected override void OnAccept(BuilderVisitor visitor)
        {
            visitor.VisitExistsFilter(this);
        }
    }
}
