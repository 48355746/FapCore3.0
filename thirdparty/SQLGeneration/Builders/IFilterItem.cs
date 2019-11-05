using System;
using SQLGeneration.Parsing;

namespace SQLGeneration.Builders
{
    /// <summary>
    /// Represents an item that can appear in a filter.
    /// </summary>
    public interface IFilterItem : IVisitableBuilder
    {
    }
}
