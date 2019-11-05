using System;
using System.Collections.Generic;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Describes the window that a function is applied to.
    /// </summary>
    public class FunctionWindow : IVisitableBuilder
    {
        private readonly List<AliasedProjection> partitionItems;
        private readonly List<OrderBy> orderByItems;
        
        /// <summary>
        /// Initializes a new instance of a FunctionWindow.
        /// </summary>
        public FunctionWindow()
        {
            this.partitionItems = new List<AliasedProjection>();
            this.orderByItems = new List<OrderBy>();
        }

        /// <summary>
        /// Gets the items making up the partitioning.
        /// </summary>
        public IEnumerable<AliasedProjection> Partition
        {
            get { return partitionItems; }
        }

        /// <summary>
        /// Adds the item as a partitioner.
        /// </summary>
        /// <param name="item">The item to partition the records on.</param>
        /// <returns>An aliased projection wrapping the given item.</returns>
        public AliasedProjection AddPartition(IProjectionItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            AliasedProjection projection = new AliasedProjection(item, null);
            partitionItems.Add(projection);
            return projection;
        }

        /// <summary>
        /// Adds the item as a partitioner.
        /// </summary>
        /// <param name="item">The aliased projection to add.</param>
        public void AddPartition(AliasedProjection item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            partitionItems.Add(item);
        }

        /// <summary>
        /// Removes the item from the partition.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was removed; otherwise, false.</returns>
        public bool RemovePartition(AliasedProjection item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            return partitionItems.Remove(item);
        }

        /// <summary>
        /// Gets the order by items.
        /// </summary>
        public IEnumerable<OrderBy> OrderBy
        {
            get { return orderByItems; }
        }

        /// <summary>
        /// Gets the order by items.
        /// </summary>
        internal List<OrderBy> OrderByList
        {
            get { return orderByItems; }
        }

        /// <summary>
        /// Adds the item as a sort condition to the window.
        /// </summary>
        /// <param name="orderBy">The order by to add.</param>
        public void AddOrderBy(OrderBy orderBy)
        {
            if (orderBy == null)
            {
                throw new ArgumentNullException("orderBy");
            }
            orderByItems.Add(orderBy);
        }

        /// <summary>
        /// Removes the item as a sort condition to the window.
        /// </summary>
        /// <param name="orderBy">The order by to remove.</param>
        /// <returns>True if the order by was removed; otherwise, false.</returns>
        public bool RemoveOrderBy(OrderBy orderBy)
        {
            if (orderBy == null)
            {
                throw new ArgumentNullException("orderBy");
            }
            return orderByItems.Remove(orderBy);
        }

        /// <summary>
        /// Gets or sets the window framing.
        /// </summary>
        public WindowFrame Frame
        {
            get;
            set;
        }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitFunctionWindow(this);
        }
    }
}
