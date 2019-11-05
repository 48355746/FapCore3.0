using System;
using SQLGeneration.Parsing;


namespace SQLGeneration.Builders
{
    /// <summary>
    /// Describes a window frame that is limited to a specific number of rows in one direction.
    /// </summary>
    public abstract class BoundFrame : IVisitableBuilder
    {
        /// <summary>
        /// Initializes a new instance of a BoundFrame.
        /// </summary>
        /// <param name="rowCount">The limit to the number of rows to include in the frame.</param>
        protected BoundFrame(int rowCount)
        {
            if (rowCount < 0)
            {
                throw new ArgumentOutOfRangeException("rowCount", rowCount, "The row count cannot be negative when defining a bound function window frame.");
            }
            RowCount = rowCount;
        }

        /// <summary>
        /// Gets the number of rows to include in the frame in one direction.
        /// </summary>
        public int RowCount
        {
            get;
            private set;
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
