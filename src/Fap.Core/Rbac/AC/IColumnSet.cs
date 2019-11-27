using Fap.Core.Infrastructure.Metadata;
using System.Collections.Generic;

namespace Fap.Core.Rbac.AC
{
    /// <summary>
    /// 列集合
    /// </summary>
    public interface IColumnSet:IEnumerable<FapColumn>
    {
        void Refresh();
        bool TryGetValue(string fid, out FapColumn fapColumn);
        bool TryGetValue(int id, out FapColumn fapColumn);

        bool TryGetValueByTable(string tableName, out List<FapColumn> fapColumns);
    }
}
