using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Unions the items from the first query to the second.
    /// </summary>
    public class Union : SelectCombiner
    {
        /// <summary>
        /// Initializes a new instance of a Union.
        /// </summary>
        /// <param name="leftHand">The left hand SELECT command.</param>
        /// <param name="rightHand">The right hand SELECT command.</param>
        public Union(ISelectBuilder leftHand, ISelectBuilder rightHand)
            : base(leftHand, rightHand)
        {
        }

        /// <summary>
        /// Provides information to the given visitor about the current builder.
        /// </summary>
        /// <param name="visitor">The visitor requesting information.</param>
        protected override void OnAccept(BuilderVisitor visitor)
        {
            visitor.VisitUnion(this);
        }
    }
}
