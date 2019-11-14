using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using SQLGeneration.SQLGeneration;

namespace SQLGeneration.Parsing
{
    /// <summary>
    /// Generates a series of tokens.
    /// </summary>
    public abstract class TokenRegistry : ITokenRegistry
    {
        private readonly List<string> tokenNames;
        private readonly Dictionary<string, TokenDefinition> definitionLookup;
        private ConcurrentDictionary<string, Regex> checks = new ConcurrentDictionary<string, Regex>();

        /// <summary>
        /// Initializes a new instance of a TokenRegistry.
        /// </summary>
        protected TokenRegistry()
        {
            tokenNames = new List<string>();
            definitionLookup = new Dictionary<string, TokenDefinition>();
        }

        /// <summary>
        /// Associates the given token name to the regular expression that
        /// tokens of that type are expected to match.
        /// </summary>
        /// <param name="tokenName">The type of the token to associate the regular expression with.</param>
        /// <param name="regex">The regular expression that the token is expected match.</param>
        /// <param name="ignoreCase">Specifies whether the regex should be case-sensitive.</param>
        /// <remarks>
        /// Multiple regular expressions can be registered to the same token name.
        /// They will be tried in the order that they are defined.
        /// </remarks>
        public void Define(string tokenName, string regex, bool ignoreCase = false)
        {
            if (definitionLookup.ContainsKey(tokenName))
            {
                string message = String.Format("An attempt was made to define two tokens with the same name: {0}.", tokenName);
                throw new SQLGenerationException(message);
            }
            tokenNames.Add(tokenName);
            TokenDefinition definition = new TokenDefinition()
            {
                Type = tokenName,
                Regex = regex,
                IgnoreCase = ignoreCase,
            };
            definitionLookup.Add(tokenName, definition);
        }

        /// <summary>
        /// Gets whether a token with the given name has been registered.
        /// </summary>
        /// <param name="tokenName">The name of the token to search for.</param>
        /// <returns>True if the token has been registered; otherwise, false.</returns>
        public bool IsRegistered(string tokenName)
        {
            return definitionLookup.ContainsKey(tokenName);
        }

        /// <summary>
        /// Extracts the next token from the given input string, starting at the given index.
        /// </summary>
        /// <param name="input">The input string to get the next token from.</param>
        /// <param name="index">The index into the string to start searching for a token.</param>
        /// <returns>The extracted token -or- null if no token is found.</returns>
        internal TokenResult ExtractToken(string input, ref int index)
        {
            ConcurrentDictionary<string, Regex> checks = getRegex();
            foreach (string tokenName in tokenNames)
            {
                Regex regex = checks[tokenName];
                Match match = regex.Match(input, index);
                if (match.Success)
                {
                    index = match.Index + match.Length;
                    string value = match.Groups["Token"].Value;
                    return new TokenResult(tokenName, value);
                }
            }
            return null;
        }

        private string GetTokenType(string token)
        {
            ConcurrentDictionary<string, Regex> checks = getRegex();
            foreach (string name in tokenNames)
            {
                Regex regex = checks[name];
                Match match = regex.Match(token, 0);
                if (match.Success)
                {
                    return name;
                }
            }
            return null;
        }
        private object lockObj = new object();
        private ConcurrentDictionary<string, Regex> getRegex()
        {
            lock (lockObj)
            {
                if (checks.Count < 1)
                {
                    foreach (string tokenName in definitionLookup.Keys)
                    {
                        string pattern = getTokenRegex(definitionLookup[tokenName]);
                        Regex regex = new Regex(pattern, RegexOptionsHelper.DefaultOptions | RegexOptions.ExplicitCapture);
                        checks.TryAdd(tokenName, regex);
                    }
                }

                return checks;


            }
        }

        private string getTokenRegex(TokenDefinition definition)
        {
            StringBuilder regexBuilder = new StringBuilder();
            regexBuilder.Append(@"\G\s*(?<Token>");
            if (definition.IgnoreCase)
            {
                regexBuilder.Append("(?i)");
            }
            regexBuilder.Append(definition.Regex);
            if (definition.IgnoreCase)
            {
                regexBuilder.Append("(?-i)");
            }
            regexBuilder.Append(")");
            return regexBuilder.ToString();
        }

        private sealed class TokenDefinition
        {
            public string Type { get; set; }

            public string Regex { get; set; }

            public bool IgnoreCase { get; set; }
        }

        /// <summary>
        /// Creates a stream of tokens by tokenizing the given string,
        /// verifying the tokens against the token definitions.
        /// </summary>
        /// <param name="commandText">The input stream containing the tokens.</param>
        /// <returns>The new token source.</returns>
        public ITokenSource CreateTokenSource(string commandText)
        {
            return new Tokenizer(this, commandText);
        }

        private sealed class Tokenizer : TokenSource
        {
            private readonly TokenRegistry registry;
            private readonly string commandText;
            private int index;

            public Tokenizer(TokenRegistry registry, string commandText)
            {
                this.registry = registry;
                this.commandText = commandText;
            }

            protected override TokenResult GetNextToken()
            {
                if (index >= commandText.Length)
                {
                    return null;
                }
                TokenResult result = registry.ExtractToken(commandText, ref index);
                if (result == null && !String.IsNullOrWhiteSpace(commandText.Substring(index)))
                {
                    throw new SQLGenerationException("Encountered an unknown token type.");
                }
                return result;
            }
        }
    }
}
