using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Defines the limits of a function window whose frame ranges before the current row.
    /// </summary>
    public class PrecedingOnlyWindowFrame : WindowFrame
    {
        /// <summary>
        /// Initializes a new instance of a PrecedingOnlyWindowFrame.
        /// </summary>
        /// <param name="precedingFrame">The object describing the preceding frame.</param>
        public PrecedingOnlyWindowFrame(IPrecedingFrame precedingFrame)
        {
            if (precedingFrame == null)
            {
                throw new ArgumentNullException("precedingFrame");
            }
            PrecedingFrame = precedingFrame;
        }

        /// <summary>
        /// Gets the preceding window frame.
        /// </summary>
        public IPrecedingFrame PrecedingFrame
        {
            get;
            private set;
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        protected override void OnAccept(BuilderVisitor visitor)
        {
            visitor.VisitPrecedingOnlyWindowFrame(this);
        }
    }
}
