using Fap.Core.MetaData;
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
    }
}
