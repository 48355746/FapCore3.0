using System;
using SQLGeneration.Parsing;


namespace SQLGeneration.Builders
{
    /// <summary>
    /// Provides a table name.
    /// </summary>
    public class Table : IRightJoinItem
    {
        /// <summary>
        /// Initializes a new instance of a Table.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        public Table(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of a Table.
        /// </summary>
        /// <param name="qualifier">The schema the table belongs to.</param>
        /// <param name="name">The name of the table.</param>
        public Table(Namespace qualifier, string name)
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

        string IRightJoinItem.GetSourceName()
        {
            return Name;
        }

        bool IRightJoinItem.IsAliasRequired
        {
            get { return false; }
        }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitTable(this);
        }
    }
}
