using System;
using System.Collections.Generic;

namespace SQLGeneration.Parsing
{
    /// <summary>
    /// Provides convenience methods for building streams of tokens.
    /// </summary>
    public sealed class TokenStream
    {
        private List<TokenResult> tokens;

        /// <summary>
        /// Initializes a new instance of a TokenStream.
        /// </summary>
        public TokenStream()
        {
            tokens = new List<TokenResult>();
        }

        /// <summary>
        /// Adds a token to the stream.
        /// </summary>
        /// <param name="result">The token result to add.</param>
        /// <returns>The current token stream.</returns>
        public TokenStream Add(TokenResult result)
        {
            tokens.Add(result);
            return this;
        }

        /// <summary>
        /// Adds the given tokens to the stream.
        /// </summary>
        /// <param name="stream">The tokens to add to the stream.</param>
        /// <returns>The current token stream.</returns>
        public TokenStream AddRange(TokenStream stream)
        {
            tokens.AddRange(stream.tokens);
            return this;
        }

        /// <summary>
        /// Creates a token source from the stream.
        /// </summary>
        /// <returns>The token source.</returns>
        public ITokenSource CreateTokenSource()
        {
            return new StreamTokenSource(tokens);
        }

        private sealed class StreamTokenSource : TokenSource
        {
            private readonly List<TokenResult> tokens;
            private int index;

            public StreamTokenSource(List<TokenResult> tokens)
            {
                this.tokens = new List<TokenResult>(tokens);
            }

            protected override TokenResult GetNextToken()
            {
                if (index >= tokens.Count)
                {
                    return null;
                }
                TokenResult result = tokens[index];
                ++index;
                return result;
            }
        }
    }
}
