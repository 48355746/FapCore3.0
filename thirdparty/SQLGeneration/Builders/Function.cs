using System;
using System.Collections.Generic;
using SQLGeneration.Parsing;


namespace SQLGeneration.Builders
{
    /// <summary>
    /// Adds a function call to a command.
    /// </summary>
    public class Function : Filter, IProjectionItem, IRightJoinItem, IFilterItem, IGroupByItem, IValueProvider
    {
        private readonly Namespace qualifier;
        private readonly List<IProjectionItem> arguments;

        /// <summary>
        /// Initializes a new instance of a Function.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        public Function(string name)
            : this(null, name, new IProjectionItem[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of a Function.
        /// </summary>
        /// <param name="qualifier">The schema the function exists in.</param>
        /// <param name="name">The name of the function.</param>
        public Function(Namespace qualifier, string name)
            : this(qualifier, name, new IProjectionItem[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of a Function.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="arguments">The arguments being passed to the function.</param>
        public Function(string name, params IProjectionItem[] arguments)
            : this(null, name, arguments)
        {
        }

        /// <summary>
        /// Initializes a new instance of a Function.
        /// </summary>
        /// <param name="qualifier">The schema the function exists in.</param>
        /// <param name="name">The name of the function.</param>
        /// <param name="arguments">The arguments being passed to the function.</param>
        public Function(Namespace qualifier, string name, params IProjectionItem[] arguments)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Encountered a null or blank function name.");
            }
            this.qualifier = qualifier;
            Name = name;
            this.arguments = new List<IProjectionItem>(arguments);
        }

        /// <summary>
        /// Gets or sets the schema the functions belongs to.
        /// </summary>
        public Namespace Qualifier
        {
            get { return qualifier; }
        }

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a list of the arguments being passed to the function.
        /// </summary>
        public IEnumerable<IProjectionItem> Arguments
        {
            get { return arguments; }
        }

        /// <summary>
        /// Adds the given projection item to the arguments list.
        /// </summary>
        /// <param name="item">The value to add.</param>
        public void AddArgument(IProjectionItem item)
        {
            arguments.Add(item);
        }

        /// <summary>
        /// Removes the given projection item from the arguments list.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was removed; otherwise, false.</returns>
        public bool RemoveArgument(IProjectionItem item)
        {
            return arguments.Remove(item);
        }

        /// <summary>
        /// Gets or sets the window to apply the function over.
        /// </summary>
        public FunctionWindow FunctionWindow
        {
            get;
            set;
        }

        bool IRightJoinItem.IsAliasRequired
        {
            get { return true; }
        }

        string IRightJoinItem.GetSourceName()
        {
            return null;
        }

        bool IValueProvider.IsValueList
        {
            get { return false; }
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        protected override void OnAccept(BuilderVisitor visitor)
        {
            visitor.VisitFunction(this);
        }
    }
}
