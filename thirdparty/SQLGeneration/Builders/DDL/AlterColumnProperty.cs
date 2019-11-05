using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// An alter column property. Used to Add or Drop a column property.
    /// </summary>
    public class AlterColumnProperty : ITableAlteration
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="alterType"></param>
        /// <param name="columnname">The column name to be altered.</param>
        public AlterColumnProperty(AlterAction alterType, string columnname)
        {
            this.Name = columnname;
            this.AlterType = alterType;
        }

        /// <summary>
        /// The name of the column.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The property.
        /// </summary>
        public IColumnProperty Property { get; set; }

        /// <summary>
        /// The action to be performed.
        /// </summary>
        public AlterAction AlterType { get; set; }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitAlterColumnProperty(this);
        }

    }


}
