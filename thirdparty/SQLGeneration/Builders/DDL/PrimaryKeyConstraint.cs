using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{

    /// <summary>
    /// A Primary Key Constraint.
    /// </summary>
    public class PrimaryKeyConstraint : Constraint
    {
        /// <summary>
        /// Initializes a new instance of an PrimaryKeyConstraint.
        /// </summary>    
        public PrimaryKeyConstraint()
            : this(null)
        {

        }

        /// <summary>
        /// Initializes a new instance of an DefaultConstraint.
        /// </summary> 
        public PrimaryKeyConstraint(string constraintName)
        {
            this.ConstraintName = constraintName;
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        public override void Accept(BuilderVisitor visitor)
        {
            visitor.VisitPrimaryKeyConstraint(this);
        }

    }
}
