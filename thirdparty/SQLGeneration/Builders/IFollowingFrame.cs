using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Specifies where a windowed function's frame should stop.
    /// </summary>
    public interface IFollowingFrame : IVisitableBuilder
    {
    }
}
