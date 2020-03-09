using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;

namespace Fap.Core.Rbac.AC
{
    /// <summary>
    /// 表集合
    /// </summary>
    public interface ITableSet:IEnumerable<FapTable>
    {
        void Refresh();
        bool TryGetValue(string fid, out FapTable fapTable);
        bool TryGetValueByName(string tableName, out FapTable fapTable);
        bool TryGetValueByCategory(string category, out IEnumerable<FapTable> tables);
        IEnumerable<FapTable> TryGetValue(Func<FapTable, bool> predicate);
    }
}
