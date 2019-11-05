using System;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a branch in a match case expression.
    /// </summary>
    public class MatchCaseBranch
    {
        /// <summary>
        /// The value that must match for the value to be chosen.
        /// </summary>
        public IProjectionItem Option { get; internal set; }

        /// <summary>
        /// The value that will be returned if the option matches.
        /// </summary>
        public IProjectionItem Value { get; internal set; }
    }
}
