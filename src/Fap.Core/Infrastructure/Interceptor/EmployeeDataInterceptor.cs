using Dapper;
using Fap.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Fap.Core.DataAccess.Interceptor;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Tracker;
using Fap.Core.Rbac.Model;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.DI;

namespace Fap.Core.Infrastructure.Interceptor
{
    /// <summary>
    /// 人员数据拦截器
    /// </summary>
    [Service]
    public class EmployeeDataInterceptor : DataInterceptorBase
    {
        private readonly ILogger _logger;
        private static string TableName = "Employee";
        public EmployeeDataInterceptor(IServiceProvider provider, IDbContext dataAccessor) : base(provider, dataAccessor)
        {
            _logger = _loggerFactory.CreateLogger<FapUserDataInterceptor>();
        }


        #region 动态对象
        /// <summary>
        /// 新增后
        /// </summary>
        public override void AfterDynamicObjectInsert(FapDynamicObject dynamicData)
        {
            if (dynamicData.ContainsKey("LoginName"))
            {
                string loginName = dynamicData.Get("LoginName").ToString();
                string fid = dynamicData.Get("Fid").ToString();
                AddUser(loginName, fid);
            }
            DataSynchEntity(dynamicData, DataChangeTypeEnum.ADD);
        }
        public override void AfterEntityInsert(object entity)
        {
            Employee employee = entity as Employee;
            if (employee.LoginName.IsPresent())
            {
                AddUser(employee.LoginName, employee.Fid);
            }
            DataSynchEntity(employee, DataChangeTypeEnum.ADD);
        }

        private void AddUser(string loginName, string fid)
        {
            //登录名
            if (loginName.IsPresent())
            {
                if (!_appDomain.UserSet.TryGetValueByUserName(loginName, out FapUser user))
                {
                    user = new FapUser();
                    user.UserCode = user.UserName = loginName;
                    user.UserIdentity = fid;
                    user.EnableState = 1;
                    user.IsLocked = 0;
                    _dbContext.Insert<FapUser>(user);
                }
            }

        }
        /// <summary>
        /// 更新后
        /// </summary>
        public override void AfterDynamicObjectUpdate(FapDynamicObject dynamicData)
        {
            //同步进队列
            DataSynchEntity(dynamicData, DataChangeTypeEnum.UPDATE);
            //更新兼职
            //UpdatePartJob(dynamicData);
            //登录名变化的时候 新增用户
            //AddUser(dynamicData);
        }
        public override void AfterEntityUpdate(object entity)
        {
            DataSynchEntity(entity, DataChangeTypeEnum.UPDATE);
        }
        //private void UpdatePartJob(dynamic dynamicData)
        //{
        //    if (dynamicData.ContainsKey("EmpCode") && dynamicData.ContainsKey("IsMainJob"))
        //    {
        //        int pt = Convert.ToInt32(dynamicData.IsMainJob);
        //        if (pt == 1)
        //        {
        //            string empCode = dynamicData.EmpCode;
        //            UpdatePartJob(dynamicData, empCode);
        //        }
        //    }
        //}

        /// <summary>
        /// 删除后
        /// </summary>
        public override void AfterDynamicObjectDelete(FapDynamicObject dynamicData)
        {
            DataSynchEntity(dynamicData, DataChangeTypeEnum.DELETE);
            //删除主职的同时删除兼职
            //DeletePartJob(dynamicData);
            //删除用户

            string fid = dynamicData.Get("Fid").ToString();
            DeleteUser(fid);

        }
        public override void AfterEntityDelete(object entity)
        {
            Employee employee = entity as Employee;
            DataSynchEntity(entity, DataChangeTypeEnum.DELETE);
            DeleteUser(employee.Fid);
        }
        private void DeleteUser(string empUid)
        {
            if (empUid.IsPresent())
            {
                var users = _appDomain.UserSet.Where(u => u.UserIdentity == empUid);
                foreach (var user in users)
                {
                    _dbContext.Delete(user);
                }
            }
        }

        //private void DeletePartJob(dynamic dynamicData)
        //{
        //    if (dynamicData.ContainsKey("EmpCode") && dynamicData.ContainsKey("IsMainJob"))
        //    {
        //        int pt = Convert.ToInt32(dynamicData.IsMainJob);
        //        if (pt == 1)
        //        {
        //            string empCode = dynamicData.EmpCode;
        //            IEnumerable<dynamic> employees = _dbContext.QueryWhere("Employee", "IsMainJob=0 and EmpCode='" + empCode + "'", null, false, _dbSession);
        //            if (employees.Count() > 0)
        //            {
        //                var dynEmps = employees.ToFapDynamicObjectList("Employee");

        //                _dbContext.DeleteDynamicDataBatch(dynEmps, _dbSession);
        //            }
        //        }
        //    }
        //}
        /// <summary>
        /// 更新兼职
        /// </summary>
        /// <param name="dynamicData"></param>
        /// <param name="empCode"></param>
        //private void UpdatePartJob(dynamic dynamicData, string empCode)
        //{
        //    var employees = _dbContext.QueryWhere("Employee", "IsMainJob=0 and EmpCode='" + empCode + "'", null, false, _dbSession);
        //    if (employees.Count() > 0)
        //    {
        //        string[] partJobCols = { "DeptUid", "DeptCode", "EmpPosition", "EmpJob", "IsMainJob", "JobGrade" };
        //        foreach (var emp in employees)
        //        {
        //            IDictionary<string, object> dataDict = dynamicData as IDictionary<string, object>;

        //            var dynEmployee = dataDict.ToFapDynamicObject("Employee");
        //            _dbContext.UpdateDynamicData(dynEmployee, _dbSession);
        //        }
        //    }
        //}

        /// <summary>
        /// 同步
        /// </summary>
        /// <param name="dynamicData"></param>
        /// <param name="oper"></param>
        private void DataSynchEntity(List<object> list, DataChangeTypeEnum oper)
        {
            EventDataTracker tracker = _provider.GetService<EventDataTracker>();
            if (tracker != null)
            {
                EventData data = new EventData();
                data.ChangeDataType = oper.ToString();
                data.EntityName = TableName;
                data.ChangeData = list;
                tracker.TrackEventData(data);
            }
        }
        private void DataSynchEntity(object obj, DataChangeTypeEnum oper)
        {
            DataSynchEntity(new List<object> { obj }, oper);
        }

        #endregion 动态对象


    }
}
