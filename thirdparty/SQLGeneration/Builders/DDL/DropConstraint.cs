using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{   

    /// <summary>
    /// A drop constraint.
    /// </summary>
    public class DropConstraint : IVisitableBuilder
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the constraint to drop.</param>
        public DropConstraint(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// The name of the column.
        /// </summary>
        public string Name { get; set; }


        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitDropConstraint(this);
        }
    }
}
