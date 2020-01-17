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
            //表格权限，不在这里设置，需要取权限--数据权限哪里设置
            JqGridViewModel model = this.GetJqGridModel("OrgDept");
            return View(model);
        }
        //合并部门
        //Get:/Organization/OrgDept/MergeDept
        public PartialViewResult MergeDept()
        {   
            FormViewModel fd = new FormViewModel();
            QuerySet  sq = new QuerySet();
            sq.TableName = "OrgDept";
            sq.QueryCols = "*";
            sq.InitWhere = "Id=@Id";
            sq.Parameters.Add(new Parameter("Id", -1));
            fd.QueryOption = sq;
            fd.TableName = "OrgDept";           
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
        public ViewResult OrgChart(string fid, string type)
        {
            ViewBag.Fid = fid;
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