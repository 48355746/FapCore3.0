using System;
using System.Collections.Generic;


namespace SQLGeneration.Parsing
{
    /// <summary>
    /// Specifies the expressions making up a syntax.
    /// </summary>
    public class Grammar
    {
        private readonly TokenRegistry tokenRegistry;
        private readonly Dictionary<string, ExpressionDefinition> expressionLookup;

        /// <summary>
        /// Initializes a new instance of a Grammar, initially empty.
        /// </summary>
        /// <param name="tokenRegistry">The token registry to use to verify referenced tokens types are defined.</param>
        public Grammar(TokenRegistry tokenRegistry)
        {
            this.tokenRegistry = tokenRegistry;
            this.expressionLookup = new Dictionary<string, ExpressionDefinition>();
        }

        /// <summary>
        /// Gets the token registery being used.
        /// </summary>
        public TokenRegistry TokenRegistry
        {
            get { return tokenRegistry; }
        }

        /// <summary>
        /// Gets the definitions comprising the grammar.
        /// </summary>
        public IEnumerable<ExpressionDefinition> Definitions
        {
            get { return expressionLookup.Values; }
        }

        /// <summary>
        /// Creates a sub-expression definition.
        /// </summary>
        /// <returns>The expression definition to allow for configuration.</returns>
        public ExpressionDefinition Define()
        {
            ExpressionDefinition definition = new ExpressionDefinition(null);
            return definition;
        }

        /// <summary>
        /// Creates or retrieves the expression definition associated with the given name.
        /// </summary>
        /// <param name="type">The identifier to use to refer to the expression type later.</param>
        /// <returns>The expression definition to allow for configuration.</returns>
        public ExpressionDefinition Define(string type)
        {
            if (String.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentException("Encountered a null or blank expression type.");
            }
            ExpressionDefinition definition;
            if (!expressionLookup.TryGetValue(type, out definition))
            {
                definition = new ExpressionDefinition(type);
                expressionLookup.Add(type, definition);
            }
            return definition;
        }

        /// <summary>
        /// Gets a placeholder to indicate that a token is expected next in an expression.
        /// </summary>
        /// <param name="tokenName">The type of the token that is expected.</param>
        /// <returns>The token placeholder.</returns>
        public Token Token(string tokenName)
        {
            if (String.IsNullOrWhiteSpace(tokenName) || !tokenRegistry.IsRegistered(tokenName))
            {
                throw new ArgumentException("Encountered an unknown token type.");
            }
            return new Token(tokenName);
        }

        /// <summary>
        /// Gets a placeholder to indicate that a token is expected next in an expression.
        /// </summary>
        /// <param name="tokenName">The type of the token that is expected.</param>
        /// <param name="expectedValue">The expected value of the token.</param>
        /// <returns>The token placeholder.</returns>
        public Token Token(string tokenName, string expectedValue)
        {
            if (String.IsNullOrWhiteSpace(tokenName) || !tokenRegistry.IsRegistered(tokenName))
            {
                throw new ArgumentException("Encountered an unknown token type.");
            }
            if (String.IsNullOrWhiteSpace(expectedValue))
            {
                throw new ArgumentException("Encountered a null or blank expected token value.");
            }
            return new Token(tokenName, expectedValue);
        }

        /// <summary>
        /// Gets a placeholder to indicate that one of many expressions is next in an expression.
        /// </summary>
        /// <returns>The options placeholder.</returns>
        public Options Options()
        {
            return new Options();
        }

        /// <summary>
        /// Gets a placeholder to indicate that one of many expressions is next in an expression.
        /// </summary>
        /// <param name="type">The type of the expression for future reference.</param>
        /// <returns>The options placeholder.</returns>
        public Expression Expression(string type)
        {
            ExpressionDefinition definition = Define(type);
            return new Expression(definition);
        }
    }
}
