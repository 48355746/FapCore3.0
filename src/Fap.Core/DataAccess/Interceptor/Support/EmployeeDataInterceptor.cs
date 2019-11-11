using Dapper;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Interface;
using Fap.Model;
using Fap.Model.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Infrastructure.Thirdparty;
using Fap.Core.Rbac;
using Fap.Core.Platform.Domain;
using Microsoft.Extensions.Logging;
using Fap.Core.DataAccess.BaseAccess;
using Fap.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Fap.Core.DataAccess.Database.DBUtils;
using Fap.Model.MetaData;
using Fap.Core.DataAccess.Interceptor;

namespace Fap.Core.Infrastructure.Support.Interceptor
{
    /// <summary>
    /// 人员数据拦截器
    /// </summary>
    public class EmployeeDataInterceptor : DataInterceptorBase
    {
        ILogger _logger;
        IEventBus _eventBus;

        public EmployeeDataInterceptor(IServiceProvider provider, IDbContext dbContext) : base(provider, dbContext, dbSession)
        {
            _logger = _loggerFactory.CreateLogger<EmployeeDataInterceptor>();
            _eventBus = provider.GetService<IEventBus>();
        }

        

        #region 动态对象
        /// <summary>
        /// 新增后
        /// </summary>
        public override void AfterDynamicObjectInsert(dynamic dynamicData)
        {
            this.DataSynchDynamicObject(dynamicData, RealtimeData.OPER_ADD);
            AddUser(dynamicData);
        }

        private void AddUser(dynamic dynamicData)
        {
            if (dynamicData.ContainsKey("LoginName"))
            {
                //登录名
                string loginName = dynamicData.LoginName;
                if (loginName.IsNotNullOrEmpty())
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("UserName", loginName);
                    string sql = "select * from FapUser where UserName=@UserName";
                    FapUser user = _dbContext.QueryFirstOrDefault<FapUser>(sql, param,false,_dbSession);
                    if (user == null)
                    {
                        user = new FapUser();
                        user.UserCode = user.UserName = loginName;
                        user.UserIdentity = dynamicData.Fid;
                        string pwd = GetDefaultPassword();
                        //默认密码111111
                        user.UserPassword = pwd == "" ? "111111" : pwd;
                        user.EnableState = 1;
                        user.IsLocked = 0;
                        _dbContext.Insert<FapUser>(user,_dbSession);
                        //_dataAccessor.InsertEntity<FapUser>(user);
                    }
                }
            }
        }
        private string GetDefaultPassword()
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("ParamKey", "employee.user.password");
            string sql = "select * from FapConfig where ParamKey=@ParamKey";
            FapConfig config = _dbSession.QueryFirst<FapConfig>(sql, param);
            if (config != null)
            {
                return config.ParamValue;
            }
            return "";
        }
        /// <summary>
        /// 更新后
        /// </summary>
        public override void AfterDynamicObjectUpdate(dynamic dynamicData)
        {
            //同步进队列
            this.DataSynchDynamicObject(dynamicData, RealtimeData.OPER_UPDATE);
            //更新兼职
            UpdatePartJob(dynamicData);
            //登录名变化的时候 新增用户
            AddUser(dynamicData);
        }

        private void UpdatePartJob(dynamic dynamicData)
        {
            if (dynamicData.ContainsKey("EmpCode") && dynamicData.ContainsKey("IsMainJob"))
            {
                int pt = Convert.ToInt32(dynamicData.IsMainJob);
                if (pt == 1)
                {
                    string empCode = dynamicData.EmpCode;
                    UpdatePartJob(dynamicData, empCode);
                }
            }
        }

        /// <summary>
        /// 删除后
        /// </summary>
        public override void AfterDynamicObjectDelete(dynamic dynamicData)
        {
            this.DataSynchDynamicObject(dynamicData, RealtimeData.OPER_DELETE);
            //删除主职的同时删除兼职
            DeletePartJob(dynamicData);
            //删除用户
            DeleteUser(dynamicData);
        }

        private void DeleteUser(dynamic dynamicData)
        {
            if (dynamicData.ContainsKey("LoginName") && dynamicData.ContainsKey("IsMainJob"))
            {
                //登录名
                string loginName = dynamicData.LoginName;
                int pt = Convert.ToInt32(dynamicData.IsMainJob);
                if (loginName.IsNotNullOrEmpty() && pt == 1)
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("Fid", dynamicData.Fid);
                    var users = _dbContext.QueryWhere<FapUser>("UserIdentity=@Fid", param,false,_dbSession);
                    if (users != null && users.Any())
                    {
                        _dbContext.DeleteBatch<FapUser>(users,_dbSession);
                    }
                }
            }
        }

        private void DeletePartJob(dynamic dynamicData)
        {
            if (dynamicData.ContainsKey("EmpCode") && dynamicData.ContainsKey("IsMainJob"))
            {
                int pt = Convert.ToInt32(dynamicData.IsMainJob);
                if (pt == 1)
                {
                    string empCode = dynamicData.EmpCode;
                    IEnumerable<dynamic> employees = _dbContext.QueryWhere("Employee", "IsMainJob=0 and EmpCode='" + empCode + "'",null,false,_dbSession);
                    if (employees.Count() > 0)
                    {
                        var dynEmps= employees.ToFapDynamicObjectList("Employee");

                        _dbContext.DeleteDynamicDataBatch(dynEmps,_dbSession);
                    }
                }
            }
        }
        /// <summary>
        /// 更新兼职
        /// </summary>
        /// <param name="dynamicData"></param>
        /// <param name="empCode"></param>
        private void UpdatePartJob(dynamic dynamicData, string empCode)
        {
            var employees = _dbContext.QueryWhere("Employee", "IsMainJob=0 and EmpCode='" + empCode + "'",null,false,_dbSession);
            if (employees.Count() > 0)
            {
                //List<string> defaultCols = FapDbUtils.GetDefaultFieldNameList();
                string[] partJobCols = { "DeptUid", "DeptCode", "EmpPosition", "EmpJob", "IsMainJob", "JobGrade" };
                //List<string> DataAccessor. GetDefaultFieldNameList();
                foreach (var emp in employees)
                {
                    IDictionary<string, object> dataDict = dynamicData as IDictionary<string,object>;
                    //foreach (var dy in dataDict)
                    //{
                    //    if (!defaultCols.Contains(dy.Key) && !partJobCols.Contains(dy.Key))
                    //    {
                    //        emp.Add(dy.Key, dy.Value);
                    //    }
                    //}
                    //emp.TableName = "Employee";
                    var dynEmployee = dataDict.ToFapDynamicObject("Employee");
                    _dbContext.UpdateDynamicData(dynEmployee,_dbSession);
                }
            }
        }

        /// <summary>
        /// 同步
        /// </summary>
        /// <param name="dynamicData"></param>
        /// <param name="oper"></param>
        private void DataSynchDynamicObject(dynamic dynamicData, string oper)
        {
            if (dynamicData == null) { return; }

            RealtimeData data = new RealtimeData();
            data.Oper = oper;
            data.Type = "employee";
            //List<Employee> employees = new List<Employee>();
            //employees.Add(BaseModel.ToEntity<Employee>(dynamicData));
            List<dynamic> employees = new List<dynamic>();
            employees.Add(dynamicData);
            data.Data = employees;
            this._eventBus.PublishAsync(new RealtimeSynEvent(data));
            //RealtimeSynchServiceFactory.GetInstance().EnqueueQueue(data);
        }

        #endregion 动态对象

        #region 实体对象
        /// <summary>
        /// 更新实体对象后
        /// </summary>
        public override void AfterEntityUpdate(object entity)
        {
            DataSynchEntity(entity, RealtimeData.OPER_UPDATE);
        }

        /// <summary>
        /// 新增实体对象后
        /// </summary>
        public override void AfterEntityInsert(object entity)
        {
            DataSynchEntity(entity, RealtimeData.OPER_ADD);
        }

        /// <summary>
        /// 删除实体对象后
        /// </summary>
        public override void AfterEntityDelete(object entity)
        {
            DataSynchEntity(entity, RealtimeData.OPER_DELETE);
        }

        private void DataSynchEntity(object entity, string oper)
        {
            if (entity == null) { return; }
            RealtimeData data = new RealtimeData();
            data.Oper = oper;
            data.Type = "employee";
            if (entity is Employee)
            {
                Employee employee = entity as Employee;
                List<Employee> employees = new List<Employee>();
                employees.Add(employee);
                data.Data = employees;
            }
            else if (entity is List<Employee>)
            {
                List<Employee> employees = entity as List<Employee>;
                data.Data = employees;
            }

            RealtimeSynchServiceFactory.GetInstance().EnqueueQueue(data);
        }


        #endregion 实体对象
    }
}
