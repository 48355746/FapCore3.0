using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Defines the limits of a function window whose frame ranges before and after the current row.
    /// </summary>
    public class BetweenWindowFrame : WindowFrame
    {
        /// <summary>
        /// Initializes a new instance of a BetweenWindowFrame.
        /// </summary>
        /// <param name="precedingFrame">The object describing the preceding frame.</param>
        /// <param name="followingFrame">The object describing the following frame.</param>
        public BetweenWindowFrame(IPrecedingFrame precedingFrame, IFollowingFrame followingFrame)
        {
            if (precedingFrame == null)
            {
                throw new ArgumentNullException("precedingFrame");
            }
            if (followingFrame == null)
            {
                throw new ArgumentNullException("followingFrame");
            }
            PrecedingFrame = precedingFrame;
            FollowingFrame = followingFrame;
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
        /// Gets the following window frame.
        /// </summary>
        public IFollowingFrame FollowingFrame
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
            visitor.VisitBetweenWindowFrame(this);
        }
    }
}
