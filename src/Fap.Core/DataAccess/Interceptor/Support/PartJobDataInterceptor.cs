/* ==============================================================================
 * 功能描述：兼职信息  
 * 创 建 者：wyf
 * 创建日期：2017-03-30 17:01:39
 * ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Rbac;
using Fap.Core.Platform.Domain;
using Microsoft.Extensions.Logging;
using Fap.Core.Events;
using Fap.Core.DataAccess.BaseAccess;
using Fap.Model.MetaData;

namespace Fap.Core.Infrastructure.Support.Interceptor
{
    public class PartJobDataInterceptor : DataInterceptorBase
    {
        private ILogger _logger;

        public PartJobDataInterceptor(IServiceProvider provider, IDbContext dataAccessor, DbSession dbSession) : base(provider, dataAccessor, dbSession)
        {
            _logger = _loggerFactory.CreateLogger<PartJobDataInterceptor>();
        }




        #region 动态对象
        /// <summary>
        /// 新增兼职信息后
        /// </summary>
        public override void AfterDynamicObjectInsert(dynamic fapDynamicData)
        {
            dynamic dynamicData = fapDynamicData;
            //向employee中添加兼职信息
            string empCode = dynamicData.EmpCode;
            IEnumerable<dynamic> dataEmployees =_dbContext.QueryWhere("Employee", "EmpCode='" + empCode + "' and IsMainJob=1",null,false,_dbSession);
            if (dataEmployees != null && dataEmployees.Any())
            {
                dynamic employee = dataEmployees.First();
                employee.Fid = "";
                employee.Id = -1;
                employee.DeptUid = dynamicData.DeptUid;
                employee.DeptCode = dynamicData.DeptCode;
                employee.EmpPosition = dynamicData.PositionUid;
                employee.EmpJob = dynamicData.JobUid;
                employee.Leadership = dynamicData.Leadership;
                //职位类型改为兼职
                employee.IsMainJob = 0;
                employee.TableName = "Employee";
                _dbContext.InsertDynamicData(employee,_dbSession);
            }

        }

        /// <summary>
        /// 更新后
        /// </summary>
        public override void AfterDynamicObjectUpdate(dynamic dynamicData)
        {
            
        }

        /// <summary>
        /// 删除后
        /// </summary>
        public override void AfterDynamicObjectDelete(dynamic fapDynamicData)
        {
            dynamic dynamicData = fapDynamicData;
            //删除人员信息中的兼职信息
            string empCode = dynamicData.EmpCode;
            string deptUid = dynamicData.DeptUid;
            string positionUid = dynamicData.PositionUid;
            IEnumerable<dynamic> dataEmployees = _dbContext.QueryWhere("Employee", "EmpCode='" + empCode + "' and DeptUid='" + deptUid + "' and EmpPosition='" + positionUid + "' and IsMainJob=0",null,false,_dbSession);
            if (dataEmployees != null && dataEmployees.Any())
            {
                dynamic employee= dataEmployees.First();
                employee.TableName = "Employee";
                _dbContext.DeleteDynamicData(employee,_dbSession);
            }
        }



        #endregion 动态对象

     
    }
}
