using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Metadata;
using Fap.Core.Rbac.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTestFapCore
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
            //var emp= _dbContext.Get("Employee", "3534239003521843200");
            //dynamic keyValuePairs = new FapDynamicObject("Employee",emp.Id);         
            //keyValuePairs.EmpPinYin = pinyin;
            //keyValuePairs.Fid = emp.Fid;
            //var b= _dbContext.UpdateDynamicData(keyValuePairs);
            var emp = _dbContext.Get<Employee>(73);

            emp.EmpPinYin = pinyin;
            
            var b = _dbContext.Update(emp);
            return b;
        }
        [Transactional]
        public bool ModifyUser(string pinyin)
        {
            //var emp= _dbContext.Get("Employee", "3534239003521843200");
            //dynamic keyValuePairs = new FapDynamicObject("Employee",emp.Id);         
            //keyValuePairs.EmpPinYin = pinyin;
            //keyValuePairs.Fid = emp.Fid;
            //var b= _dbContext.UpdateDynamicData(keyValuePairs);
            var emp = _dbContext.Get<FapUser>(31);

            emp.UserPhone = pinyin;

            var b = _dbContext.Update(emp);
            return b;
        }
    }
}
