using Fap.Core.DataAccess;
using Fap.Core.DataAccess.Interceptor;
using Fap.Core.DI;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Interceptor
{
    /// <summary>
    /// 兼职
    /// </summary>
    [Service]
    public class PartJobDataInterceptor : DataInterceptorBase
    {
        public PartJobDataInterceptor(IServiceProvider provider, IDbContext dbContext) : base(provider, dbContext)
        {
        }
        public override void AfterDynamicObjectInsert(FapDynamicObject fapDynamicData)
        {
            //向employee中添加兼职信息
            string empUid = fapDynamicData.Get("EmpUid").ToString();
            dynamic dynEmployee = _dbContext.QueryFirstOrDefault("select * from Employee where Fid='" + empUid + "' and IsMainJob=1");
            if (dynEmployee != null)
            {
                dynEmployee.Fid = "";
                dynEmployee.Id = "-1";
                dynEmployee.DeptUid = fapDynamicData.Get("DeptUid");
                dynEmployee.DeptCode = fapDynamicData.Get("DeptCode");

                dynEmployee.Leadership = fapDynamicData.Get("Leadership");
                //职位类型改为兼职
                dynEmployee.IsMainJob = "0";
                var dicEmp = dynEmployee as IDictionary<string, object>;
                var demp = dicEmp.ToFapDynamicObject(_dbContext.Columns("Employee"));
                _dbContext.InsertDynamicData(demp);
                //更新映射
                _dbContext.Execute($"update EmpPartJob set EmpMapUid='{demp.Get("Fid")}' where Fid='{fapDynamicData.Get("Fid")}'");
            }

        }
        public override void AfterDynamicObjectDelete(FapDynamicObject fapDynamicData)
        {
            //删除人员信息中的兼职信息
            string empUid = fapDynamicData.Get("EmpMapUid").ToString();
            FapDynamicObject demp = new FapDynamicObject(_dbContext.Columns("Employee"));
            demp.SetValue("Fid", empUid);

            _dbContext.DeleteDynamicData(demp);

        }
    }
}
