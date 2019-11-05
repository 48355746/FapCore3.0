using System;
using SQLGeneration.Parsing;


namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents how a value will be compared to a set of values.
    /// </summary>
    public enum Quantifier
    {
        /// <summary>
        /// Specifies that a predicate must be satisfied for all values in a set.
        /// </summary>
        All,
        /// <summary>
        /// Specifies that a predicate must be satisfied for at least one value in a set.
        /// </summary>
        Any,
        /// <summary>
        /// Specifies that a predicate must be satisfied for at least one value in a set.
        /// </summary>
        Some,
    }

    /// <summary>
    /// Converts between representation of quantifiers.
    /// </summary>
    internal sealed class QuantifierConverter
    {
        /// <summary>
        /// Initializes a new instance of a QuantifierConverter.
        /// </summary>
        public QuantifierConverter()
        {
        }

        /// <summary>
        /// Converts the given quantifier to its string equivalent.
        /// </summary>
        /// <param name="quantifier">The valeu to convert to a string.</param>
        /// <returns>The token representing the quantifier.</returns>
        public string ToString(Quantifier quantifier)
        {
            switch (quantifier)
            {
                case Quantifier.All:
                    return "ALL";
                case Quantifier.Any:
                    return "ANY";
                case Quantifier.Some:
                    return "SOME";
                default:
                    throw new ArgumentException("Encountered an unknown quantifier.");
            }
        }
    }
}
