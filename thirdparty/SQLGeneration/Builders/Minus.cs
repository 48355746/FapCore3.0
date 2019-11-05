using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Removes the items returned by the second query from the first query.
    /// </summary>
    public class Minus : SelectCombiner
    {
        /// <summary>
        /// Initializes a new instance of a Minus.
        /// </summary>
        /// <param name="leftHand">The left hand SELECT command.</param>
        /// <param name="rightHand">The right hand SELECT command.</param>
        public Minus(ISelectBuilder leftHand, ISelectBuilder rightHand)
            : base(leftHand, rightHand)
        {
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        protected override void OnAccept(BuilderVisitor visitor)
        {
            visitor.VisitMinus(this);
        }
    }
}
