using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Metadata
{
    public interface IMetaDataService
    {
        FapTable GetTableByName(string tableName);
        IEnumerable<FapTable> GetTablesByCategory(string tableCategory);

        FapColumn GetFapColumnByName(string colName);
        IEnumerable<FapColumn> GetFapColumnsByTable(string tableName);

    }
}
