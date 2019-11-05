using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// An alter column.
    /// </summary>
    public class AlterColumn : ITableAlteration
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        public AlterColumn(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// The name of the column.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The collation of the column.
        /// </summary>
        public string Collation { get; set; }

        /// <summary>
        /// The datatype of the column.
        /// </summary>
        public DataType DataType { get; set; }

        /// <summary>
        /// Whether the column can hold null values.
        /// </summary>
        public bool? IsNullable { get; set; }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitAlterColumn(this);
        }

    }
}
