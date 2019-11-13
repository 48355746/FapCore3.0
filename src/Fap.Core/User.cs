using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core
{
    [Service(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
    public class User : IUser
    {
        private IDbContext _dbContext;
        public User(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [Transactional]
        public bool ModifyEmployee(string pinyin)
        {
            var emp= _dbContext.Get("Employee", "3534239003521843200");
            dynamic keyValuePairs = new FapDynamicObject();
            keyValuePairs.TableName = "Employee";
            keyValuePairs.Id = emp.Id;
            keyValuePairs.EmpPinYin = pinyin;
            keyValuePairs.Fid = emp.Fid;
            var b= _dbContext.UpdateDynamicData(keyValuePairs);
            return b;
        }

    }
}
