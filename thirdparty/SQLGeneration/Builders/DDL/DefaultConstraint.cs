using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a "Default" Constraint.
    /// </summary>
    public class DefaultConstraint : Constraint
    {
        /// <summary>
        /// Initializes a new instance of an DefaultConstraint.
        /// </summary>    
        public DefaultConstraint()
            : this(null)
        {

        }

        /// <summary>
        /// Initializes a new instance of an DefaultConstraint.
        /// </summary> 
        public DefaultConstraint(string constraintName)
        {
            this.ConstraintName = constraintName;
        }

        /// <summary>
        /// The default value expressed as a single literal.
        /// </summary>
        public Literal Value { get; set; }

        /// <summary>
        /// The default value function.
        /// </summary>
        public Function Function { get; set; }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        public override void Accept(BuilderVisitor visitor)
        {
            visitor.VisitDefaultConstraint(this);
        }     
    
    }
}
