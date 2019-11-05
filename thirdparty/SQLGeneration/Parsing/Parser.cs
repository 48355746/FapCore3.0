using System;
using System.Collections.Generic;


namespace SQLGeneration.Parsing
{
    /// <summary>
    /// Parses a sequence of tokens using a grammar, applying actions to matching sequences.
    /// </summary>
    public sealed class Parser
    {
        private readonly Grammar grammar;

        /// <summary>
        /// Initializes a new instance of a Parser.
        /// </summary>
        /// <param name="grammar">The grammar to use.</param>
        public Parser(Grammar grammar)
        {
            if (grammar == null)
            {
                throw new ArgumentNullException("grammar");
            }
            this.grammar = grammar;
        }

        /// <summary>
        /// Parses the given token source using the specified grammar, starting with
        /// expression with the given name.
        /// </summary>
        /// <param name="expressionType">The type of the expression to start parsing.</param>
        /// <param name="tokenSource">The source of tokens.</param>
        public MatchResult Parse(string expressionType, ITokenSource tokenSource)
        {
            if (tokenSource == null)
            {
                throw new ArgumentNullException("tokenSource");
            }

            Expression expression = grammar.Expression(expressionType);
            ParseAttempt attempt = new ParseAttempt(this, tokenSource);
            MatchResult result = expression.Match(attempt, String.Empty);
            return result;
        }

        private sealed class ParseAttempt : IParseAttempt
        {
            private readonly Parser parser;
            private readonly ITokenSource tokenSource;
            private readonly List<TokenResult> tokens;

            /// <summary>
            /// Initializes a new instance of a ParseAttempt.
            /// </summary>
            /// <param name="parser">The parser containing</param>
            /// <param name="tokenSource">An object to retrieve the sequence of tokens from.</param>
            public ParseAttempt(Parser parser, ITokenSource tokenSource)
            {
                this.parser = parser;
                this.tokenSource = tokenSource;
                this.tokens = new List<TokenResult>();
            }

            /// <summary>
            /// Gets the tokens collected during the attempt.
            /// </summary>
            public List<TokenResult> Tokens
            {
                get { return tokens; }
            }

            /// <summary>
            /// Attempts to get a token of the given type.
            /// </summary>
            /// <returns>The result of the search.</returns>
            public TokenResult GetToken()
            {
                TokenResult result = tokenSource.GetToken();
                if (result != null)
                {
                    tokens.Add(result);
                }
                return result;
            }

            /// <summary>
            /// Creates an attempt to parse a child expression.
            /// </summary>
            /// <returns>A new attempt object.</returns>
            public IParseAttempt Attempt()
            {
                return new ParseAttempt(parser, tokenSource);
            }

            /// <summary>
            /// Accepts the attempt as a successful parse, joining the given attempt's tokens
            /// with the current attempt's.
            /// </summary>
            /// <param name="attempt">The child attempt to accept.</param>
            public void Accept(IParseAttempt attempt)
            {
                tokens.AddRange(attempt.Tokens);
            }

            /// <summary>
            /// Rejects the attempt as a failed parse, returning the attempt's token
            /// to the token stream.
            /// </summary>
            public void Reject()
            {
                int index = tokens.Count;
                while (index != 0)
                {
                    --index;
                    tokenSource.PutBack(tokens[index]);
                }
            }
        }
    }
}
