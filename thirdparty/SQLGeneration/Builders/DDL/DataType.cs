
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// The datatype builder.
    /// </summary>
    public class DataType : IDatabaseObject
    {
        private readonly Namespace qualifier;

        private readonly List<Literal> arguments;

        /// <summary>
        /// Initializes a new instance of a DataType.
        /// </summary>
        /// <param name="name">The name of the DataType.</param>
        public DataType(string name)
            : this(null, name, new Literal[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of a DataType.
        /// </summary>
        /// <param name="qualifier">The schema the DataType exists in.</param>
        /// <param name="name">The name of the DataType.</param>
        public DataType(Namespace qualifier, string name)
            : this(qualifier, name, new Literal[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of a DataType.
        /// </summary>
        /// <param name="name">The name of the DataType.</param>
        /// <param name="arguments">The arguments being passed to the DataType.</param>
        public DataType(string name, params Literal[] arguments)
            : this(null, name, arguments)
        {
        }

        /// <summary>
        /// Initializes a new instance of a DataType.
        /// </summary>
        /// <param name="qualifier">The schema the DataType exists in.</param>
        /// <param name="name">The name of the DataType.</param>
        /// <param name="arguments">The arguments being passed to the DataType.</param>
        public DataType(Namespace qualifier, string name, params Literal[] arguments)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Encountered a null or blank datatype name.");
            }
            this.qualifier = qualifier;
            Name = name;
            this.arguments = new List<Literal>(arguments);
        }

        /// <summary>
        /// Gets or sets the schema the datatype belongs to.
        /// </summary>
        public Namespace Qualifier
        {
            get { return qualifier; }
        }

        /// <summary>
        /// Gets the name of the datatype.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Whether this datatype size has "max" specified. 
        /// </summary>
        public bool HasMax { get; set; }

        /// <summary>
        /// Gets a list of the arguments being passed to the datatype.
        /// </summary>
        public IEnumerable<Literal> Arguments
        {
            get { return arguments; }
        }

        /// <summary>
        /// Adds the given literal item to the arguments list.
        /// </summary>
        /// <param name="item">The value to add.</param>
        public void AddArgument(Literal item)
        {
            arguments.Add(item);
        }

        /// <summary>
        /// Removes the given literal item from the arguments list.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was removed; otherwise, false.</returns>
        public bool RemoveArgument(Literal item)
        {
            return arguments.Remove(item);
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitDataType(this);
        }
    }

}
