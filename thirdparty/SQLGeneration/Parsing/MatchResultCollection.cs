using System;
using System.Collections.Generic;

namespace SQLGeneration.Parsing
{
    /// <summary>
    /// Holds the match results of the items found under an expression item.
    /// </summary>
    public sealed class MatchResultCollection
    {
        private readonly Dictionary<string, MatchResult> resultLookup;

        /// <summary>
        /// Initializes a new instance of a MatchResultCollection.
        /// </summary>
        internal MatchResultCollection()
        {
            resultLookup = new Dictionary<string, MatchResult>();
        }

        /// <summary>
        /// Associates the given result to its name.
        /// </summary>
        /// <param name="result">The match result of an item.</param>
        internal void Add(MatchResult result)
        {
            resultLookup.Add(result.ItemName, result);
        }

        /// <summary>
        /// Gets the match result for the item with the given name.
        /// </summary>
        /// <param name="itemName">The name of the item to get the results for.</param>
        /// <returns>The match result for the item with the given name -or- an empty MatchResult.</returns>
        public MatchResult this[string itemName]
        {
            get
            {
                MatchResult result;
                if (!resultLookup.TryGetValue(itemName, out result))
                {
                    result = new MatchResult() { ItemName = itemName, IsMatch = false };
                }
                return result;
            }
        }
    }
}
