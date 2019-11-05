using Fap.Core.DI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.MetaData
{
    [Service(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
    public class MetaDataService : IMetaDataService
    {
        public FapColumn GetFapColumnByName(string colName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FapColumn> GetFapColumnsByTable(string tableName)
        {
            throw new NotImplementedException();
        }

        public FapTable GetTableByName(string tableName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FapTable> GetTablesByCategory(string tableCategory)
        {
            throw new NotImplementedException();
        }
    }
}
