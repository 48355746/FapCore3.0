using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// The constraint base class.
    /// </summary> 
    public abstract class Constraint : IDatabaseObject
    {

        /// <summary>
        /// The name of this constraint.
        /// </summary>
        public string ConstraintName { get; set; }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        public abstract void Accept(BuilderVisitor visitor);

    }
}
