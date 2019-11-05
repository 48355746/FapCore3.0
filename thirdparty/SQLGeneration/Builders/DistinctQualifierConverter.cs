using System;
using SQLGeneration.Parsing;


namespace SQLGeneration.Builders
{
    /// <summary>
    /// Specifies how duplicate records will be treated in a SELECT statement.
    /// </summary>
    public enum DistinctQualifier
    {
        /// <summary>
        /// Specifies that the select statement should use the default rules when handling duplicates.
        /// </summary>
        Default,
        /// <summary>
        /// Specifies that the select statement should return duplicate records.
        /// </summary>
        All,
        /// <summary>
        /// Specifies that the select statement should remove duplicate records.
        /// </summary>
        Distinct,
    }

    /// <summary>
    /// Converts between the DistinctQualifier enumeration and strings.
    /// </summary>
    internal class DistinctQualifierConverter
    {
        /// <summary>
        /// Initializes a new instance of a DistinctQualifierConverter.
        /// </summary>
        public DistinctQualifierConverter()
        {
        }

        /// <summary>
        /// Gets a string representation of the given distinct qualifier.
        /// </summary>
        /// <param name="qualifier">The qualifier to convert to a string.</param>
        /// <returns>The string representation.</returns>
        public string ToString(DistinctQualifier qualifier)
        {
            switch (qualifier)
            {
                case DistinctQualifier.All:
                case DistinctQualifier.Default:
                    return "ALL";
                case DistinctQualifier.Distinct:
                    return "DISTINCT";
                default:
                    throw new ArgumentException("Encountered an unknown distinct qualifier.");
            }
        }
    }
}
