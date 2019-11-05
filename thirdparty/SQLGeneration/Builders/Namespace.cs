using System;
using System.Collections.Generic;
using SQLGeneration.Parsing;


namespace SQLGeneration.Builders
{
    /// <summary>
    /// Qualifies an object with one or more identifiers.
    /// </summary>
    public class Namespace
    {
        private readonly List<string> qualifiers;

        /// <summary>
        /// Initializes a new instance of a Namespace.
        /// </summary>
        /// <param name="qualifiers">The qualifiers to include, in the order they will appear in the output.</param>
        public Namespace(params string[] qualifiers)
        {
            if (qualifiers == null)
            {
                throw new ArgumentNullException("qualifiers");
            }
            this.qualifiers = new List<string>();
            foreach (string qualifier in qualifiers)
            {
                AddQualifier(qualifier);
            }
        }

        /// <summary>
        /// Adds the given qualifier.
        /// </summary>
        /// <param name="qualifier">The qualifier to add.</param>
        public void AddQualifier(string qualifier)
        {
            if (String.IsNullOrWhiteSpace(qualifier))
            {
                throw new ArgumentException("Encountered a null or blank schema name.");
            }
            qualifiers.Add(qualifier);
        }

        /// <summary>
        /// Gets the qualifiers making up the namespace.
        /// </summary>
        public IEnumerable<string> Qualifiers
        {
            get { return qualifiers; }
        }
    }
}
