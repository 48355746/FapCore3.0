using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// The base class for a foreign key action.
    /// </summary> 
    public abstract class ForeignKeyAction : IVisitableBuilder
    {
        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        public abstract void Accept(BuilderVisitor visitor);
    }
}
