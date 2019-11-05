using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Describes a window frame that is limited to a specific number of rows preceding in one direction.
    /// </summary>
    public class FollowingBoundFrame : BoundFrame, IFollowingFrame
    {
        /// <summary>
        /// Initializes a new instance of a FollowingBoundFrame.
        /// </summary>
        /// <param name="rowCount">The limit to the number of rows to include in the frame.</param>
        public FollowingBoundFrame(int rowCount)
            : base(rowCount)
        {
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        protected override void OnAccept(BuilderVisitor visitor)
        {
            visitor.VisitFollowingBoundFrame(this);
        }
    }
}
