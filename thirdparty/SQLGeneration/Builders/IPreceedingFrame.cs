using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Specifies where a windowed function's frame should start.
    /// </summary>
    public interface IPrecedingFrame : IVisitableBuilder
    {
    }
}
