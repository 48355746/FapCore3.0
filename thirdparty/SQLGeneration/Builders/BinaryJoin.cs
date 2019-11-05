using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents a join between two tables or sub-queries.
    /// </summary>
    public abstract class BinaryJoin : Join
    {
        /// <summary>
        /// Initializes a new instance of a BinaryJoin.
        /// </summary>
        /// <param name="leftHand">The left hand item or join.</param>
        /// <param name="rightHand">The right hand item in the join.</param>
        protected BinaryJoin(Join leftHand, AliasedSource rightHand)
            : base(leftHand, rightHand)
        {
            if (leftHand == null)
            {
                throw new ArgumentNullException("leftHand");
            }
            if (rightHand == null)
            {
                throw new ArgumentNullException("rightHand");
            }
            LeftHand = leftHand;
            RightHand = rightHand;
        }

        /// <summary>
        /// Gets the item on the left hand side of the join.
        /// </summary>
        public Join LeftHand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the table on the right hand side of the join.
        /// </summary>
        public AliasedSource RightHand
        {
            get;
            private set;
        }
    }
}
