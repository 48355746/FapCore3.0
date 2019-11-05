using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Generates the intersection among all of the queries.
    /// </summary>
    public class Intersect : SelectCombiner
    {
        /// <summary>
        /// Initializes a new instance of a Intersect.
        /// </summary>
        /// <param name="leftHand">The left hand SELECT command.</param>
        /// <param name="rightHand">The right hand SELECT command.</param>
        public Intersect(ISelectBuilder leftHand, ISelectBuilder rightHand)
            : base(leftHand, rightHand)
        {
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        protected override void OnAccept(BuilderVisitor visitor)
        {
            visitor.VisitIntersect(this);
        }
    }
}
