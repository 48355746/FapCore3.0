using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// List of items to drop from a table.
    /// </summary>
    public class DropColumnsList : IDropTableItem
    {
        private readonly List<DropColumn> _dropItems;

        /// <summary>
        /// Constructor.
        /// </summary>   
        public DropColumnsList()
        {
            _dropItems = new List<DropColumn>();
        }

        /// <summary>
        /// Returns the columns to be dropped.
        /// </summary>
        public List<DropColumn> Items
        {
            get
            {
                return _dropItems;
            }
        }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitDropColumnsList(this);
        }

    }
}
