using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Builds a TOP clause that is found in a SELECT statement.
    /// </summary>
    public class Top : IVisitableBuilder
    {
        private readonly IProjectionItem _expression;

        /// <summary>
        /// Initializes a new instance of a Top.
        /// </summary>
        /// <param name="expression">The number or percent of items to return.</param>
        public Top(IProjectionItem expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            _expression = expression;
        }

        /// <summary>
        /// Gets the expression representing the number or percent of rows to return.
        /// </summary>
        public IProjectionItem Expression
        {
            get
            {
                return _expression;
            }
        }

        /// <summary>
        /// Gets whether or not the expression represents a percent.
        /// </summary>
        public bool IsPercent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether records matching the last item according to the order by
        /// clause shall be returned.
        /// </summary>
        public bool WithTies
        {
            get;
            set;
        }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitTop(this);
        }
    }
}
