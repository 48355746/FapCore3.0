using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Fap.Core.DataAccess.Interceptor;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Tracker;
using Fap.Core.Extensions;
using Fap.Core.Rbac.Model;
using Fap.Core.Infrastructure.Enums;

namespace Fap.Core.Infrastructure.Interceptor
{
    public class OrgDeptDataInterceptor : DataInterceptorBase
    {
        ILogger _logger;
        private static string TableName = "OrgDept";

        public OrgDeptDataInterceptor(IServiceProvider provider, IDbContext dbContext) : base(provider, dbContext)
        {
            _logger = _loggerFactory.CreateLogger<OrgDeptDataInterceptor>();
        }

        public override void BeforeDynamicObjectInsert(FapDynamicObject fapDynamicData)
        {
            HandlerOrgdept(fapDynamicData);
        }

        private void HandlerOrgdept(FapDynamicObject fapDynamicData)
        {
            string pid = fapDynamicData.Get("Pid").ToString();
            if (pid.IsPresent())
            {
                _appDomain.OrgDeptSet.TryGetValue(pid, out OrgDept parentDept);
                _appDomain.OrgDeptSet.TryGetValueByPid(pid, out IEnumerable<OrgDept> childDepts);
                if (parentDept.IsFinal == 1)
                {
                    //更新父部门是否末级标记
                    parentDept.IsFinal = 0;
                    _dbContext.Update(parentDept);
                }
                fapDynamicData.SetValue("PCode", parentDept.DeptCode);
                fapDynamicData.SetValue("TreeLevel", parentDept.TreeLevel + 1);
                if (fapDynamicData.ContainsKey("DeptOrder") && fapDynamicData.Get("DeptOrder").ToString() == "0")
                {
                    fapDynamicData.SetValue("DeptOrder", childDepts.Any() ? childDepts.Count() : 1);

                }
                fapDynamicData.SetValue("FullName", parentDept.FullName + "/" + fapDynamicData.Get("DeptName"));
                fapDynamicData.SetValue("DeptCode", childDepts.Any() ? (childDepts.Max(d => d.DeptCode).ToInt() + 1).ToString() : $"{parentDept.DeptCode}01");

            }
            else
            {
                int c= _appDomain.OrgDeptSet.Count(d => d.Pid.IsMissing())+1;
                if (fapDynamicData.Get("DeptCode").ToString().IsMissing())
                {
                    fapDynamicData.SetValue("DeptCode", c);
                    fapDynamicData.SetValue("FullName", fapDynamicData.Get("DeptName"));
                    fapDynamicData.SetValue("TreeLevel", 0);
                    fapDynamicData.SetValue("DeptOrder", c);
                }
            }
        }


        /// <summary>
        /// 更新前
        /// </summary>
        public override void BeforeDynamicObjectUpdate(FapDynamicObject fapDynamicData)
        {
            string fid = fapDynamicData.Get("Fid").ToString();
            _appDomain.OrgDeptSet.TryGetValue(fid, out OrgDept orgDept);

            string pid = fapDynamicData.Get("Pid").ToString();
            //父部门没变化
            if (pid != orgDept.Pid)
            {
                HandlerOrgdept(fapDynamicData);
            }
        }
        /// <summary>
        /// 更新后
        /// </summary>
        public override void AfterDynamicObjectUpdate(FapDynamicObject fapDynamicData)
        {
            string fid = fapDynamicData.Get("Fid").ToString();
            //检查是否有子
            _appDomain.OrgDeptSet.TryGetValueByPid(fid, out IEnumerable<OrgDept> childs);
            if (childs.Any())
            {
                string deptCode = fapDynamicData.Get("DeptCode").ToString();
                string fullName = fapDynamicData.Get("FullName").ToString();
                int level = fapDynamicData.Get("TreeLevel").ToInt();
                HandlerChildsEdit(deptCode, fullName, level, childs);
            }
            //同步其他系統，放入隊列
            DataSynchDynamicObject(fapDynamicData, DataChangeTypeEnum.UPDATE);
            _appDomain.OrgDeptSet.Refresh();

        }

        private void HandlerChildsEdit(string deptCode, string fullName, int level, IEnumerable<OrgDept> childs)
        {
            //如果code不是以父code开头 则 祖父部门变了
            if (!childs.First().DeptCode.StartsWith(deptCode))
            {
                List<OrgDept> list = new List<OrgDept>();
                childs.ToList().ForEach((d) =>
                {
                    d.DeptCode = deptCode + d.DeptCode.Substring(d.DeptCode.Length - 2);
                    d.FullName = fullName + "/" + d.DeptName;
                    d.PCode = deptCode;
                    d.TreeLevel = level + 1;
                    _dbContext.Update<OrgDept>(d);
                    list.Add(d);
                });
                DataSynchEntity(list, DataChangeTypeEnum.UPDATE);
            }
        }
        #region 动态对象
        /// <summary>
        /// 新增后
        /// </summary>
        public override void AfterDynamicObjectInsert(FapDynamicObject fapDynamicData)
        {
            //同步其他系統，放入隊列
            DataSynchDynamicObject(fapDynamicData, DataChangeTypeEnum.ADD);
            _appDomain.OrgDeptSet.Refresh();

        }


        /// <summary>
        /// 删除后
        /// </summary>
        public override void AfterDynamicObjectDelete(FapDynamicObject fapDynamicData)
        {
            string fid = fapDynamicData.Get("Fid").ToString();
            string pid = fapDynamicData.Get("Pid").ToString();
            if (pid.IsPresent())
            {
                _appDomain.OrgDeptSet.TryGetValueByPid(pid, out IEnumerable<OrgDept> pchilds);
                if (pchilds.Count() < 1)
                {
                    //如果父部门只有一个部门，修改父部门末级标记
                    _appDomain.OrgDeptSet.TryGetValue(pid, out OrgDept parentOrgDept);
                    parentOrgDept.IsFinal = 1;
                    _dbContext.Update(parentOrgDept);
                }
            }
            var rd = _appDomain.RoleDeptSet.Where(d => d.DeptUid == fid);
            if (rd.Any())
            {
                //删除角色部门中的数据
                DynamicParameters param = new DynamicParameters();
                param.Add("Fid", fid);
                _dbContext.DeleteExec(nameof(FapRoleDept), "DeptUid=@Fid", param);
            }
            _appDomain.OrgDeptSet.TryGetValueByPid(fid, out IEnumerable<OrgDept> childs);
            if (childs.Any())
            {
                HandlerChildsDelete(childs);
            }
            _appDomain.OrgDeptSet.Refresh();
            _appDomain.RoleDeptSet.Refresh();
            //同步其他系统 放入队列
            DataSynchDynamicObject(fapDynamicData, DataChangeTypeEnum.DELETE);
        }

        private void HandlerChildsDelete(IEnumerable<OrgDept> childs)
        {
            foreach (var orgdept in childs)
            {
                _dbContext.Delete(orgdept);
            }
            DataSynchEntity(childs.ToList(), DataChangeTypeEnum.DELETE);
        }

        #endregion 动态对象
        public override void AfterEntityUpdate(object entity)
        {
            OrgDept orgDept = entity as OrgDept;
            string fid = orgDept.Fid;
            //检查是否有子
            _appDomain.OrgDeptSet.TryGetValueByPid(fid, out IEnumerable<OrgDept> childs);
            if (childs.Any())
            {
                string deptCode = orgDept.DeptCode;
                string fullName = orgDept.FullName;
                int level = orgDept.TreeLevel;
                HandlerChildsEdit(deptCode, fullName, level, childs);
            }
        }
        public override void AfterEntityDelete(object entity)
        {
            OrgDept orgDept = entity as OrgDept;
            string fid = orgDept.Fid;
            //检查是否有子
            _appDomain.OrgDeptSet.TryGetValueByPid(fid, out IEnumerable<OrgDept> childs);
            if (childs.Any())
            {
                HandlerChildsDelete(childs);
            }
        }

        private void DataSynchDynamicObject(FapDynamicObject orgDept, DataChangeTypeEnum oper)
        {
            EventDataTracker tracker = _provider.GetService<EventDataTracker>();
            if (tracker != null)
            {
                EventData data = new EventData();
                data.ChangeDataType = oper.ToString();
                data.EntityName = TableName;
                List<dynamic> users = new List<dynamic>();
                users.Add(orgDept);
                data.ChangeData = users;
                tracker.TrackEventData(data);
            }
        }
        private void DataSynchEntity(List<OrgDept> list, DataChangeTypeEnum oper)
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
    }
}
