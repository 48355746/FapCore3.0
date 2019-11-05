using System;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a branch in a condition case expression.
    /// </summary>
    public class ConditionalCaseBranch
    {
        /// <summary>
        /// The condition that must evaluate to true for the value to be chosen.
        /// </summary>
        public IFilter Condition { get; internal set; }

        /// <summary>
        /// The value that will be returned if the condition evaluated to true.
        /// </summary>
        public IProjectionItem Value { get; internal set; }
    }
}
