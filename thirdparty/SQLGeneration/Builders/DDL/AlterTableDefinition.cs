
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// The alter table definition builder.
    /// </summary>
    public class AlterTableDefinition : IDatabaseObject
    {       

        /// <summary>
        /// Initializes a new instance of a Table.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        public AlterTableDefinition(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of a Table.
        /// </summary>
        /// <param name="qualifier">The schema the table belongs to.</param>
        /// <param name="name">The name of the table.</param>
        public AlterTableDefinition(Namespace qualifier, string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Encountered a null or blank table name.");
            }
            Qualifier = qualifier;
            Name = name;
         
        }

        /// <summary>
        /// Gets or sets the schema the table belongs to.
        /// </summary>
        public Namespace Qualifier
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }    

        /// <summary>
        /// The alteration to be made to this table.
        /// </summary>
        public ITableAlteration Alteration { get; set; }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitAlterTableDefinition(this);
        }

    }

}
