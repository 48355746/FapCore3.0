using System;
using System.Collections.Generic;
using System.Text;

namespace SQLGeneration.Builders
{
    public class ParameterLiteral : Literal
    {
        /// <summary>
        /// Initializes a new instance of a StringLiteral.
        /// </summary>
        public ParameterLiteral()
        {
            Value = String.Empty;
        }

        /// <summary>
        /// Initializes a new instance of a StringLiteral.
        /// </summary>
        /// <param name="value">The string value.</param>
        public ParameterLiteral(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets or sets the value of the string literal.
        /// </summary>
        public string Value
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
            visitor.VisitParameterLiteral(this);
        }
    }
}
