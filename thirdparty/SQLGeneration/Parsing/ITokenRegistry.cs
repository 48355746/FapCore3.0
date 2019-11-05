using System;

namespace SQLGeneration.Parsing
{
    /// <summary>
    /// Indicates whether a token has been registered.
    /// </summary>
    public interface ITokenRegistry
    {
        /// <summary>
        /// Gets whether a token with the given name has been registered.
        /// </summary>
        /// <param name="tokenName">The name of the token to search for.</param>
        /// <returns>True if the token has been registered; otherwise, false.</returns>
        bool IsRegistered(string tokenName);
    }
}
