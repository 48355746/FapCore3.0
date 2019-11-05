using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// A Unique Constraint.
    /// </summary>
    public class UniqueConstraint : Constraint
    {
        /// <summary>
        /// Initializes a new instance of an UniqueConstraint.
        /// </summary>    
        public UniqueConstraint()
            : this(null)
        {

        }

        /// <summary>
        /// Initializes a new instance of an UniqueConstraint.
        /// </summary> 
        public UniqueConstraint(string constraintName)
        {
            this.ConstraintName = constraintName;
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        public override void Accept(BuilderVisitor visitor)
        {
            visitor.VisitUniqueConstraint(this);
        }

    }
}
