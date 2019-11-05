using System;
using SQLGeneration.Parsing;


namespace SQLGeneration.Builders
{
    /// <summary>
    /// Affects the logic of how two filters will be applied.
    /// </summary>
    public enum Conjunction
    {
        /// <summary>
        /// Joins two filters with an 'and'.
        /// </summary>
        And,
        /// <summary>
        /// Joins two filters with an 'or'.
        /// </summary>
        Or
    }

    /// <summary>
    /// Converts conjunctions to their string representations.
    /// </summary>
    internal class ConjunctionConverter
    {
        /// <summary>
        /// Initializes a new instance of a ConjunctionConverter.
        /// </summary>
        public ConjunctionConverter()
        {
        }

        /// <summary>
        /// Gets a string representation of the given conjunction.
        /// </summary>
        /// <param name="conjunction">The conjunction to convert to a string.</param>
        /// <returns>The string representation.</returns>
        public string ToString(Conjunction conjunction)
        {
            switch (conjunction)
            {
                case Conjunction.And:
                    return "AND";
                case Conjunction.Or:
                    return "OR";
                default:
                    throw new ArgumentException("Encountered an unknown conjunction type.");
            }
        }
    }
}
