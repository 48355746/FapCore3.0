using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{

    /// <summary>
    /// The "Foreign Key" Constraint builder.
    /// </summary>
    public class ForeignKeyConstraint : Constraint
    {
        /// <summary>
        /// Initializes a new instance of an ForeignKeyConstraint.
        /// </summary>    
        public ForeignKeyConstraint()
            : this(null)
        {

        }

        /// <summary>
        /// Initializes a new instance of an ForeignKeyConstraint.
        /// </summary> 
        public ForeignKeyConstraint(string constraintName)
        {
            this.ConstraintName = constraintName;
        }

        /// <summary>
        /// Gets or sets the table that the foreign key is referencing.
        /// </summary>
        public Table ReferencedTable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the column name the foreign key is referencing.
        /// </summary>
        public string ReferencedColumn
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the on delete action of the foreign key constraint.
        /// </summary>
        public ForeignKeyAction OnDeleteAction { get; set; }

        /// <summary>
        /// Gets or sets the on update action of the foreign key constraint.
        /// </summary>
        public ForeignKeyAction OnUpdateAction { get; set; }

        /// <summary>
        /// whether this auto increment should be excluded from replicated databases.
        /// </summary>
        public NotForReplicationColumnProperty NotForReplication { get; set; }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        public override void Accept(BuilderVisitor visitor)
        {
            visitor.VisitForeignKeyConstraint(this);
        }

    }   

}
