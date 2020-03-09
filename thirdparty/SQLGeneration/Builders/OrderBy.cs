using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents an item in the order by clause of a select statement.
    /// </summary>
    public class OrderBy : IVisitableBuilder
    {
        /// <summary>
        /// Initializes a new instance of a OrderBy.
        /// </summary>
        /// <param name="projection">The item to sort by.</param>
        /// <param name="order">The order in which to sort the items.</param>
        /// <param name="nullPlacement">The placement of nulls in the results.</param>
        public OrderBy(
            AliasedProjection projection, 
            Order order = Order.Default, 
            NullPlacement nullPlacement = NullPlacement.Default)
        {
            if (projection == null)
            {
                throw new ArgumentNullException("projection");
            }
            Projection = projection;
            Order = order;
            NullPlacement = nullPlacement;
        }

        /// <summary>
        /// Initializes a new instance of a OrderBy.
        /// </summary>
        /// <param name="projection">The item to sort by.</param>
        /// <param name="order">The order in which to sort the items.</param>
        /// <param name="nullPlacement">The placement of nulls in the results.</param>
        public OrderBy(
            IProjectionItem projection,
            Order order = Order.Default,
            NullPlacement nullPlacement = NullPlacement.Default)
        {
            if (projection == null)
            {
                throw new ArgumentNullException("projection");
            }
            Projection = new AliasedProjection(projection, null);
            Order = order;
            NullPlacement = nullPlacement;
        }

        /// <summary>
        /// Gets the item to order by.
        /// </summary>
        public AliasedProjection Projection
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the order to sort the results.
        /// </summary>
        public Order Order
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies where null values appear in the results.
        /// </summary>
        public NullPlacement NullPlacement
        {
            get;
            set;
        }
        /// <summary>
        /// 分页
        /// </summary>
        public string Pagination
        {
            get;set;
        }
       
        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitOrderBy(this);
        }
    }
}
