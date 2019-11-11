using Dapper;
using Fap.Core.Extensions;
using Fap.Core.Platform;
using Fap.Model;
using Fap.Model.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Rbac;
using Fap.Core.Infrastructure.Interface;
using Fap.Core.Infrastructure.Thirdparty;
using Fap.Core.Platform.Domain;
using Microsoft.Extensions.Logging;
using Fap.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Fap.Core.DataAccess.BaseAccess;
using Fap.Core.Utility;
using Fap.Model.MetaData;

namespace Fap.Core.Infrastructure.Support.Interceptor
{
    public class OrgDeptDataInterceptor : DataInterceptorBase
    {
        ILogger _logger;
        IEventBus _eventBus;

        public OrgDeptDataInterceptor(IServiceProvider provider, IDbContext dataAccessor, DbSession dbSession) : base(provider, dataAccessor, dbSession)
        {
            _logger =_loggerFactory.CreateLogger<OrgDeptDataInterceptor>();
            _eventBus = provider.GetService<IEventBus>();
        }

        public override void BeforeDynamicObjectInsert(dynamic fapDynamicData)
        {
            dynamic dynamicData = fapDynamicData;
            string pid = dynamicData.Get("Pid");
            if (pid.IsNotNullOrEmpty())
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("Fid", pid);
                string sql = "select * from OrgDept where Fid=@Fid or Pid=@Fid";
                
                IEnumerable<OrgDept> list =_dbContext.Query<OrgDept>(sql,param,false,_dbSession);
                OrgDept pDept = list.First<OrgDept>(d => d.Fid == pid);
                dynamicData.PCode = pDept.DeptCode;
                dynamicData.TreeLevel = pDept.TreeLevel + 1;
                if (dynamicData.ContainsKey("DeptOrder") && dynamicData.DeptOrder == 0)
                {
                    dynamicData.DeptOrder = list.Count();

                }
                dynamicData.FullName = pDept.FullName + "/" + dynamicData.DeptName;
                var childs = list.Where(d => d.Pid == pid);
                if (childs == null || childs.Count() < 1)
                {
                    dynamicData.DeptCode = pDept.DeptCode + "01";
                }
                else
                {
                    string maxCode = childs.Max(d => d.DeptCode);
                    dynamicData.DeptCode = (maxCode.ToInt32() + 1).ToString();
                }
            }
            else
            {
                if (dynamicData.Get("DeptCode") == "")
                {
                    dynamicData.DeptCode = "1";
                    dynamicData.FullName = dynamicData.DeptName;
                    dynamicData.TreeLevel = 1;
                    dynamicData.DeptOrder = 1;
                }
            }

        }


        /// <summary>
        /// 更新前
        /// </summary>
        public override void BeforeDynamicObjectUpdate(dynamic fapDynamicData)
        {
            dynamic dynamicData = fapDynamicData;
            string fid = dynamicData.Fid;
            string sql = $"select * from OrgDept where Fid='{fid}'";
            OrgDept oriDept =_dbContext.QueryFirst<OrgDept>(sql,null,false,_dbSession);

            string pid = dynamicData.Pid;
            //父部门没变化
            if (pid == oriDept.Pid&&pid.IsNotNullOrEmpty())
            {
                OrgDept pDept = _dbContext.Get<OrgDept>(pid,false,_dbSession);
                dynamicData.FullName = pDept.FullName + "/" + dynamicData.DeptName;
            }
            else
            {
                //父部门不为空
                if (pid.IsNotNullOrEmpty())
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("Fid", pid);
                    IEnumerable<OrgDept> list =_dbContext.QueryWhere<OrgDept>(" Fid=@Fid or Pid=@Fid", param,false,_dbSession);
                    OrgDept pDept = list.First<OrgDept>(d => d.Fid == pid);
                    dynamicData.PCode = pDept.DeptCode;
                    dynamicData.TreeLevel = pDept.TreeLevel + 1;
                    if (dynamicData.ContainsKey("DeptOrder") && dynamicData.DeptOrder == 0)
                    {
                        dynamicData.DeptOrder = list.Count();

                    }
                    dynamicData.FullName = pDept.FullName + "/" + dynamicData.DeptName;
                    var childs = list.Where(d => d.Pid == pid);
                    if (childs == null || childs.Count() < 1)
                    {
                        dynamicData.DeptCode = pDept.DeptCode + "01";
                    }
                    else
                    {
                        string maxCode = childs.Max(d => d.DeptCode);
                        dynamicData.DeptCode = (maxCode.ToInt32() + 1).ToString();
                    }
                }
                else
                {
                    dynamicData.DeptCode = "1";
                    dynamicData.FullName = dynamicData.DeptName;
                    dynamicData.TreeLevel = 1;
                    dynamicData.DeptOrder = 1;
                }

            }
        }
        /// <summary>
        /// 更新后
        /// </summary>
        public override void AfterDynamicObjectUpdate(dynamic fapDynamicData)
        {
            dynamic dynamicData = fapDynamicData;
            string fid = dynamicData.Fid;
            //检查是否有子
            DynamicParameters param = new DynamicParameters();
            param.Add("Fid", fid);
            IEnumerable<OrgDept> childs = _dbContext.QueryWhere<OrgDept>("Pid=@Fid", param,false,_dbSession);
            if (childs != null && childs.Any())
            {
                //如果code不是以父code开头 则 祖父部门变了
                if (!childs.First().DeptCode.StartsWith(dynamicData.DeptCode))
                {
                    childs.ToList().ForEach((d) =>
                    {
                        d.DeptCode = dynamicData.DeptCode + d.DeptCode.Substring(d.DeptCode.Length - 2);
                        d.FullName = dynamicData.FullName + "/" + d.DeptName;
                        d.PCode = dynamicData.DeptCode;
                        d.TreeLevel = dynamicData.TreeLevel + 1;
                        _dbContext.Update<OrgDept>(d,_dbSession);
                    });
                }
            }
            this.DataSynchDynamicObject(dynamicData, RealtimeData.OPER_UPDATE);

            _appDomain.OrgDeptSet.Refresh();

        }
        #region 动态对象
        /// <summary>
        /// 新增后
        /// </summary>
        public override void AfterDynamicObjectInsert(dynamic fapDynamicData)
        {
            dynamic dynamicData = fapDynamicData;
            //同步其他系統，放入隊列
            this.DataSynchDynamicObject(dynamicData, RealtimeData.OPER_ADD);

            #region 新增的加到權限中
            try
            {
                if (_appDomain != null && _httpSession.AcSession != null && _httpSession.AcSession.OnlineUser != null)
                {
                    string roleid = _httpSession.AcSession.Role.Fid;

                    FapRoleDept model = new FapRoleDept() { DeptUid = dynamicData.Fid, RoleUid = roleid,Dr=0,CreateBy=_httpSession.EmpUid,CreateName=_httpSession.EmpName, CreateDate=PublicUtils.CurrentDateTimeStr };
                    _dbContext.Insert<FapRoleDept>(model,_dbSession);
                    _appDomain.OrgDeptSet.Refresh();
                    _appDomain.RoleDeptSet.Refresh();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"表注入事件inserted{ex.Message}");
            }
            #endregion
        }

        /// <summary>
        /// 更新后
        /// </summary>
        //public  void AfterDynamicObjectUpdate(dynamic dynamicData)
        //{
        //    this.DataSynchDynamicObject(dynamicData, RealtimeData.OPER_UPDATE);
        //}

        /// <summary>
        /// 删除后
        /// </summary>
        public override void AfterDynamicObjectDelete(dynamic fapDynamicData)
        {
            dynamic dynamicData = fapDynamicData;
            //同步其他系统 放入队列
            this.DataSynchDynamicObject(dynamicData, RealtimeData.OPER_DELETE);
            #region 删除權限中的部门
            try
            {
                if (_appDomain != null && _httpSession.AcSession != null && _httpSession.AcSession.OnlineUser != null)
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("Fid", dynamicData.Fid);
                    _dbContext.Execute("delete from FapRoleDept where DeptUid=@Fid",param,_dbSession);
                    _appDomain.OrgDeptSet.Refresh();
                    _appDomain.RoleDeptSet.Refresh();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"表注入事件deleted{ex.Message}");
            }
            #endregion
        }

        private void DataSynchDynamicObject(dynamic dynamicData, string oper)
        {
            if (dynamicData == null) { return; }

            RealtimeData data = new RealtimeData();
            data.Oper = oper;
            data.Type = "dept";
            List<dynamic> orgDepts = new List<dynamic>();
            //orgDepts.Add(BaseModel.ToEntity<OrgDept>(dynamicData));
            orgDepts.Add(dynamicData);
            data.Data = orgDepts;
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

            _appDomain.OrgDeptSet.Refresh();

        }

        /// <summary>
        /// 新增实体对象后
        /// </summary>
        public override void AfterEntityInsert(object entity)
        {
            DataSynchEntity(entity, RealtimeData.OPER_ADD);

            _appDomain.OrgDeptSet.Refresh();

        }

        /// <summary>
        /// 删除实体对象后
        /// </summary>
        public override void AfterEntityDelete(object entity)
        {
            DataSynchEntity(entity, RealtimeData.OPER_DELETE);

            _appDomain.OrgDeptSet.Refresh();
        }

        private void DataSynchEntity(object entity, string oper)
        {
            if (entity == null) { return; }
            RealtimeData data = new RealtimeData();
            data.Oper = oper;
            data.Type = "dept";
            if (entity is OrgDept)
            {
                OrgDept orgDept = entity as OrgDept;
                List<OrgDept> orgDepts = new List<OrgDept>();
                orgDepts.Add(orgDept);
                data.Data = orgDepts;
            }
            else if (entity is List<OrgDept>)
            {
                List<OrgDept> orgDepts = entity as List<OrgDept>;
                data.Data = orgDepts;
            }

            RealtimeSynchServiceFactory.GetInstance().EnqueueQueue(data);
        }

        #endregion 实体对象
    }
}
