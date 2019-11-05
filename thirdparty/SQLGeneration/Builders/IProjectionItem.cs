using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents an item that can appear in the projection-clause of a select statement.
    /// </summary>
    public interface IProjectionItem : IVisitableBuilder
    {
    }
}
