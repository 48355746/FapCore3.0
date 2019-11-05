using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents an AutoIncrement.
    /// </summary>
    public class AutoIncrement : IDatabaseObject
    {
        private readonly List<NumericLiteral> arguments;

        /// <summary>
        /// Initializes a new instance of an AutoIncrement.
        /// </summary>    
        public AutoIncrement()
            : this(new NumericLiteral[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of a DataType.
        /// </summary>        
        /// <param name="arguments">The arguments being passed to the AutoIncrement.</param>
        public AutoIncrement(params NumericLiteral[] arguments)
        {
            this.arguments = new List<NumericLiteral>(arguments);
        }

        /// <summary>
        /// Gets a list of the arguments being passed to the datatype.
        /// </summary>
        public IEnumerable<NumericLiteral> Arguments
        {
            get { return arguments; }
        }

        /// <summary>
        /// whether this auto increment should be excluded from replicated databases.
        /// </summary>
        public NotForReplicationColumnProperty NotForReplication { get; set; }

        /// <summary>
        /// Adds the given literal item to the arguments list.
        /// </summary>
        /// <param name="item">The value to add.</param>
        public void AddArgument(NumericLiteral item)
        {
            arguments.Add(item);
        }

        /// <summary>
        /// Removes the given literal item from the arguments list.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was removed; otherwise, false.</returns>
        public bool RemoveArgument(NumericLiteral item)
        {
            return arguments.Remove(item);
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitAutoIncrement(this);
        }
    }
}
