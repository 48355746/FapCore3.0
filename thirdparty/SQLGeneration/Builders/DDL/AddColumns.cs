using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Add columns.
    /// </summary>
    public class AddColumns : ITableAlteration
    {

        private readonly ColumnDefinitionList _addColumnsList;

        /// <summary>
        /// Constructor.
        /// </summary>     
        public AddColumns()
        {
            _addColumnsList = new ColumnDefinitionList();
        }

        /// <summary>
        /// Returns the columns to be added.
        /// </summary>
        public ColumnDefinitionList Columns
        {
            get
            {
                return _addColumnsList;
            }
        }


        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitAddColumns(this);
        }

    }  
}
