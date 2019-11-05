using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Describes a window frame that is unbounded in one direction.
    /// </summary>
    public class PrecedingUnboundFrame : UnboundFrame, IPrecedingFrame
    {
        /// <summary>
        /// Initializes a new instance of an PrecedingUnboundFrame.
        /// </summary>
        public PrecedingUnboundFrame()
        {
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        protected override void OnAccept(BuilderVisitor visitor)
        {
            visitor.VisitPrecedingUnboundFrame(this);
        }
    }
}
