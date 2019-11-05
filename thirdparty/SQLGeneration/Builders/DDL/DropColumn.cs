using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{   

    /// <summary>
    /// The drop column.
    /// </summary>
    public class DropColumn : IVisitableBuilder
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the column to drop.</param>
        public DropColumn(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// The name of the column.
        /// </summary>
        public string Name { get; set; }


        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitDropColumn(this);
        }
    }
}
