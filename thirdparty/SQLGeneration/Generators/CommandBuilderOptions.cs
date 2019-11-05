using System;

namespace SQLGeneration.Generators
{
    /// <summary>
    /// Provides options for customizing the behavior of the command builder.
    /// </summary>
    public class CommandBuilderOptions
    {
        /// <summary>
        /// Specifies a prefix that indicates an identifier is a placeholder.
        /// </summary>
        public string PlaceholderPrefix { get; set; }
    }
}
