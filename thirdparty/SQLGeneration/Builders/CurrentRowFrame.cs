using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Describes a window frame that is limited to the current row in one direction.
    /// </summary>
    public class CurrentRowFrame : IPrecedingFrame, IFollowingFrame
    {
        /// <summary>
        /// Initializes a new instance of a CurrentRowFrame.
        /// </summary>
        public CurrentRowFrame()
        {
        }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            visitor.VisitCurrentRowFrame(this);
        }
    }
}
