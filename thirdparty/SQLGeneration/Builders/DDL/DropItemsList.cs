using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// List of items to drop from a table.
    /// </summary>
    public class DropItemsList : ITableAlteration
    {
        private readonly List<IDropTableItem> _dropItems;

        /// <summary>
        /// Constructor.
        /// </summary>     
        public DropItemsList()
        {
            _dropItems = new List<IDropTableItem>();
        }

        /// <summary>
        /// Returns the columns to be added.
        /// </summary>
        public List<IDropTableItem> Items
        {
            get
            {
                return _dropItems;
            }
        }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitDropItemsList(this);
        }

    }
}
