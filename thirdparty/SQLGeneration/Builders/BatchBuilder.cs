using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLGeneration.Builders
{

    /// <summary>
    /// Builds a string that contains many individual commands seprated by terminators.
    /// </summary>
    public class BatchBuilder : ICommand
    {
        private readonly IList<ICommand> _commands;
        private bool _hasTerminator = false;

        /// <summary>
        /// Initializes a new instance of a BatchBuilder.
        /// </summary>     
        /// <param name="commands">The commands to be in the batch.</param>
        public BatchBuilder(IList<ICommand> commands)
        {
            _commands = commands;
        }

        /// <summary>
        /// Initializes a new instance of a BatchBuilder.
        /// </summary>     
        public BatchBuilder()
        {
            _commands = new List<ICommand>();
        }

        /// <summary>
        /// Adds the command to the batch.
        /// </summary>
        /// <param name="command">The command to add.</param>
        public void AddCommand(ICommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            _commands.Add(command);
        }

        /// <summary>
        /// Removes the command from the batch.
        /// </summary>
        /// <param name="command">The command to remove.</param>
        /// <returns>True if the command was removed; otherwise, false.</returns>
        public bool RemoveCommand(ICommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }
            return _commands.Remove(command);
        }

        /// <summary>
        /// Gets the command at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ICommand GetCommand(int index)
        {
            return _commands[index];
        }

        /// <summary>
        /// Returns the commands as an IEnumerable.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICommand> Commands()
        {
            return _commands.AsEnumerable();
        }

        /// <summary>
        /// Returns whether there is a single command in the batch.
        /// </summary>
        /// <returns></returns>
        public bool IsSingleCommand()
        {
            return !(_commands.Count > 1);
        }

        /// <summary>
        /// Gets whether this command has a terminator.
        /// </summary>
        public bool HasTerminator
        {
            get
            {
                return _hasTerminator;
            }
            set
            {
                _hasTerminator = value;
            }
        }

        /// <summary>
        /// Accepts a visitor.
        /// </summary>
        /// <param name="visitor"></param>
        public void Accept(BuilderVisitor visitor)
        {
            visitor.VisitBatch(this);
        }
    }
}
