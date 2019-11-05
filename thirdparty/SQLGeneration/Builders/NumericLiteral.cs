using System;
using System.Globalization;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a numeric literal.
    /// </summary>
    public class NumericLiteral : Literal
    {
        /// <summary>
        /// Initializes a new instance of a NumericLiteral.
        /// </summary>
        public NumericLiteral()
        {
        }

        /// <summary>
        /// Initializes a new instance of a NumericLiteral.
        /// </summary>
        /// <param name="value">The value to make the literal.</param>
        public NumericLiteral(double value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets or sets the numeric value of the literal.
        /// </summary>
        public double Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the format to represent the value with.
        /// </summary>
        public string Format
        {
            get;
            set;
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        protected override void OnAccept(BuilderVisitor visitor)
        {
            visitor.VisitNumericLiteral(this);
        }
    }
}
