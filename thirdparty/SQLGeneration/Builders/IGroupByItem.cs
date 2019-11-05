using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents an item that can appear in a group by clause in a select statement.
    /// </summary>
    public interface IGroupByItem : IVisitableBuilder
    {
    }
}
