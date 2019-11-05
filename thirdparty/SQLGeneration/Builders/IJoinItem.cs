using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents an item that can appear in a join statement.
    /// </summary>
    public interface IJoinItem : IVisitableBuilder
    {
    }
}
