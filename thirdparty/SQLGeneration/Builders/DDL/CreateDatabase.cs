
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// The create database builder.
    /// </summary>
    public class CreateDatabase : IDatabaseObject
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        public CreateDatabase(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// The name of the database.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The collation of the database.
        /// </summary>
        public string Collation { get; set; }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitDatabase(this);
        }
    }


}
