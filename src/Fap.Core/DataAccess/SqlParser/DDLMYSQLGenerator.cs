using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.DataAccess.SqlParser
{
    class DDLMYSQLGenerator : IDDLSqlGenerator
    {
        public string CreateTable(FapTable table, IEnumerable<FapColumn> columns)
        {
            throw new NotImplementedException();
        }
    }
}
