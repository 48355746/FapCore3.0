using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Fap.Core.Extensions;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using System;
using Fap.Core.Rbac.Model;
using Fap.Core.Infrastructure.Query;
using Fap.Core.Infrastructure.Domain;

namespace Fap.Hcm.Web.Areas.Organization.Controllers
{
    /// <summary>
    /// 组织部门
    /// </summary>
    [Area("Organization")]
    public class OrgDeptController : FapController
    {
        public OrgDeptController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }


        //组织部门首页
        // GET: /Organization/OrgDept/
        public ActionResult DeptIndex()
        {
            JqGridViewModel model = this.GetJqGridModel("OrgDept");
            return View(model);
        }
        //合并部门
        //Get:/Organization/OrgDept/MergeDept
        public PartialViewResult MergeDept(string id)
        {           
            //父级部门
            OrgDept pOrgDept = _dbContext.Get<OrgDept>(id);
            DynamicParameters param = new DynamicParameters();
            param.Add("Pid", id);
            var maxCodeStr = _dbContext.ExecuteScalar<string>("select max(deptcode) from OrgDept where pid=@Pid", param);
            int maxLength = 0;
            int maxOrder = 1;
            string deptCode = "";
            if (maxCodeStr.IsPresent())
            {
                maxLength = maxCodeStr.Length;
                maxOrder = maxCodeStr.Substring(maxLength - 2).ToInt() + 1;
                int maxCode = maxCodeStr.ToInt() + 1;
                deptCode = maxCode.ToString().PadLeft(maxLength, '0');
            }
            else
            {
                deptCode = pOrgDept.DeptCode + "01";
            }
            FormViewModel fd = new FormViewModel();
            QuerySet  sq = new QuerySet();
            sq.TableName = "OrgDept";
            sq.InitWhere = "Id=@Id";
            sq.Parameters.Add(new Parameter("Id", -1));
            fd.QueryOption = sq;
            fd.TableName = "OrgDept";
            Dictionary<string, string> defaultData = new Dictionary<string, string>();
            defaultData.Add("DeptCode", deptCode);
            defaultData.Add("Pid", id);
            defaultData.Add("PidMC", pOrgDept.DeptName);
            defaultData.Add("PCode", pOrgDept.DeptCode);
            defaultData.Add("DeptType", pOrgDept.DeptType);
            defaultData.Add("TreeLevel", (pOrgDept.TreeLevel + 1).ToString());
            defaultData.Add("DeptOrder", maxOrder.ToString());
            fd.DefaultData = defaultData;
            return PartialView(fd);
        }
        //移动部门
        //Get:/Organization/OrgDept/MoveDept
        public PartialViewResult MoveDept()
        {
            return PartialView();
        }
        /// <summary>
        /// 组织机构图
        /// </summary>
        /// <returns></returns>
        public ViewResult OrgChart(string id, string type)
        {
            //
            //List<OrgDept> orgDepts = acc.QueryEntityByWhere<OrgDept>();
            //OrgDept pOrgDept = orgDepts.FirstOrDefault(d => d.Pid.IsNullOrEmpty() || d.Pid == "~");

            //var cOrgDept = orgDepts.Where(d => d.Pid == pOrgDept.Fid);

            //BiuldOrgDept(cOrgDept, orgDepts, pOrgDept);

            //OrgDeptViewModel model = new OrgDeptViewModel();
            //model.RootOrgDept = pOrgDept;
            //return View(model);
            ViewBag.Fid = id;
            if (string.IsNullOrWhiteSpace(type))
            {
                type = "managerName";
            }
            ViewBag.Type = type;
            return View();
        }
        public ViewResult OrgChart2()
        {
            return View();
        }
        /// <summary>
        /// 岗位图
        /// </summary>
        /// <returns></returns>
        public ViewResult PositionChart(string id)
        {
            //部门Uid
            ViewBag.DeptUid = id;
            string historyDate = "";
            if (Request.Query.ContainsKey("date"))
            {
                historyDate = Request.Query["date"].ToString();
            }
            ViewBag.HistoryDate = historyDate;
            return View();
        }
        private void BiuldOrgDept(IEnumerable<OrgDept> cOrgDepts, IEnumerable<OrgDept> allOrgDept, OrgDept pOrgDept)
        {
            if (cOrgDepts != null && cOrgDepts.Any())
            {
                pOrgDept.Children = cOrgDepts;
                foreach (OrgDept dept in cOrgDepts)
                {
                    var tempDepts = allOrgDept.Where(d => d.Pid == dept.Fid);
                    BiuldOrgDept(tempDepts, allOrgDept, dept);
                }
            }

        }
        /// <summary>
        /// 职务岗位
        /// </summary>
        /// <returns></returns>
        public ActionResult OrgJob()
        {
            MultiJqGridViewModel model = new MultiJqGridViewModel();
            model.JqGridViewModels.Add("orgjob", this.GetJqGridModel("OrgJob"));
            model.JqGridViewModels.Add("orgposition", this.GetJqGridModel("OrgPosition", (qs) =>
            {
                qs.GlobalWhere = "DeptUid in(" + FapPlatformConstants.DepartmentAuthority + ")";
                //增加统计
                qs.AddStatSet(StatSymbolEnum.Description, "'合计:' as PstName");
                qs.AddStatSet(StatSymbolEnum.SUM, "Preparation");
                qs.AddStatSet(StatSymbolEnum.SUM, "Actual");
            }, true));
            return View(model);
        }
        /// <summary>
        /// 历史架构
        /// </summary>
        /// <returns></returns>
        public ActionResult OrgHistroy()
        {
            JqGridViewModel model = GetJqGridModel("OrgPosition", (qs) =>
            {
                qs.InitWhere = "1=2";
                //增加统计
                qs.AddStatSet(StatSymbolEnum.SUM, "Preparation");
                qs.AddStatSet(StatSymbolEnum.SUM, "Actual");
            });
            return View(model);
        }
    }
}