using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Describes a window frame that is unbounded in one direction.
    /// </summary>
    public class FollowingUnboundFrame : UnboundFrame, IFollowingFrame
    {
        /// <summary>
        /// Initializes a new instance of an FollowingUnboundFrame.
        /// </summary>
        public FollowingUnboundFrame()
        {
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        protected override void OnAccept(BuilderVisitor visitor)
        {
            visitor.VisitFollowingUnboundFrame(this);
        }
    }
}
