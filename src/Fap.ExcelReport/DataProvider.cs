using Fap.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.ExcelReport
{
    public class DataProvider
    {
        private readonly IDbContext _dbContext;
        public DataProvider(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //public IEnumerable<IDictionary<string, object>> 
    }
}
