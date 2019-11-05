using System;
using System.IO;
using System.Text;
using SQLGeneration.Builders;
using SQLGeneration.Parsing;

namespace SQLGeneration.Generators
{
    /// <summary>
    /// Generates simple SQL from a token source.
    /// </summary>
    public sealed class Formatter
    {
        /// <summary>
        /// Initializes a new instance of a Formatter.
        /// </summary>
        public Formatter()
        {
        }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <returns>The command text.</returns>
        public string GetCommandText(ICommand command, CommandOptions options = null)
        {
            if (options == null)
            {
                options = new CommandOptions();
            }
            StringWriter writer = new StringWriter();
            FormattingVisitor visitor = new FormattingVisitor(writer, options);
            visitor.Visit(command);
            return writer.ToString();
        }
    }
}
