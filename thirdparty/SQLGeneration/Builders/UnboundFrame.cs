using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Describes a window frame that is unbounded in one direction.
    /// </summary>
    public abstract class UnboundFrame : IVisitableBuilder
    {
        /// <summary>
        /// Initializes a new instance of an UnboundFrame.
        /// </summary>
        protected UnboundFrame()
        {
        }

        void IVisitableBuilder.Accept(BuilderVisitor visitor)
        {
            OnAccept(visitor);
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        protected abstract void OnAccept(BuilderVisitor visitor);
    }
}
