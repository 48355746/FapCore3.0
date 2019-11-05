using System;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// An item that can be visited by a builder visitor.
    /// </summary>
    public interface IVisitableBuilder
    {
        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        void Accept(BuilderVisitor visitor);
    }
}
