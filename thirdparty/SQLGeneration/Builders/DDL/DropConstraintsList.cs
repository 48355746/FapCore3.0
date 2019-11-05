using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// List of items to drop from a table.
    /// </summary>
    public class DropConstraintsList : IDropTableItem
    {
        private readonly List<DropConstraint> _dropItems;

        /// <summary>
        /// Constructor.
        /// </summary>     
        public DropConstraintsList()
        {
            _dropItems = new List<DropConstraint>();
        }

        /// <summary>
        /// Returns the constraints to be dropped.
        /// </summary>
        public List<DropConstraint> Items
        {
            get
            {
                return _dropItems;
            }
        }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitDropConstraintsList(this);
        }

    }
}
