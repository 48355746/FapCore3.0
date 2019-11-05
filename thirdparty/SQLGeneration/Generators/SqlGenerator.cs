using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Generators
{
    /// <summary>
    /// Provides the methods that must be overridden in order to properly process SQL expressions.
    /// </summary>
    public abstract class SqlGenerator
    {
        private readonly SqlGrammar grammar;

        /// <summary>
        /// Initializes a new instance of a SqlResponder.
        /// </summary>
        /// <param name="grammar">The grammar to use.</param>
        protected SqlGenerator(SqlGrammar grammar)
        {
            if (grammar == null)
            {
                grammar = SqlGrammar.Default;
            }
            this.grammar = grammar;
        }

        /// <summary>
        /// Gets the grammar.
        /// </summary>
        protected SqlGrammar Grammar
        {
            get { return grammar; }
        }

        /// <summary>
        /// Extracts expressions from the token stream and calls the corresponding handler.
        /// </summary>
        /// <param name="tokenSource">The source of SQL tokens.</param>
        /// <returns>The results of the parse.</returns>
        protected MatchResult GetResult(ITokenSource tokenSource)
        {
            Parser parser = new Parser(grammar);
            var matchedStatement = parser.Parse(SqlGrammar.Start.Name, tokenSource);
            return matchedStatement;
        }
    }
}
