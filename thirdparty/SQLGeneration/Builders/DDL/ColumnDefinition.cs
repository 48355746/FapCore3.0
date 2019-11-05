using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// The column definition.
    /// </summary>
    public class ColumnDefinition : IDatabaseObject
    {

        private readonly ConstraintList _constraints;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        public ColumnDefinition(string name)
        {
            this.Name = name;
            _constraints = new ConstraintList();
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
        /// The autoincrement of the column.
        /// </summary>
        public AutoIncrement AutoIncrement { get; set; }

        /// <summary>
        /// The default value of the column.
        /// </summary>
        public DefaultConstraint Default { get; set; }

        /// <summary>
        /// Returns the constraints for the column.
        /// </summary>
        public ConstraintList Constraints
        {
            get
            {
                return _constraints;
            }
        }    

        /// <summary>
        /// Whether the column can hold null values.
        /// </summary>
        public bool? IsNullable { get; set; }

        /// <summary>
        /// Whether the column is a row guid column.
        /// </summary>
        public bool IsRowGuid { get; set; }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitColumnDefinition(this);
        }
    }
}
