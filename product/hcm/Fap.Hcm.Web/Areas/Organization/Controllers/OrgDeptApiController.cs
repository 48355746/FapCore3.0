using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Fap.Core.Extensions;
using Fap.Core.Rbac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Dapper;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Fap.Core.Rbac.Model;
using Fap.AspNetCore.Controls;
using Fap.Core.Infrastructure.Metadata;
using System.Net.Mime;
using Fap.Hcm.Service.Organization;

namespace Fap.Hcm.Web.Areas.Organization.Controllers
{
    [Area("Organization")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[area]/Api/OrgDept")]
    public class OrgDeptApiController : FapController
    {
        private readonly ILogger<OrgDeptApiController> _logger;
        private readonly IOrganizationService _organizationService;
        public OrgDeptApiController(IServiceProvider serviceProvider, IOrganizationService organizationService) : base(serviceProvider)
        {
            _logger = _loggerFactory.CreateLogger<OrgDeptApiController>();
            _organizationService = organizationService;
        }
        /// <summary>
        /// 历史部门
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [Route("History/{date}")]
        // POST: api/Common
        public JsonResult GetHistoryOrgDepts(string date)
        {
            _dbContext.HistoryDateTime = date;
            IEnumerable<OrgDept> powerDepts = _dbContext.QueryAll<OrgDept>();
            _dbContext.HistoryDateTime = string.Empty;
            //将List<dynamic>转换成List<TreeDataView>
            List<TreeDataView> treeList = new List<TreeDataView>();
            foreach (var data in powerDepts)
            {
                treeList.Add(new TreeDataView() { Id = data.Fid, Text = data.DeptName, Pid = data.Pid, Data = new { Code = data.DeptCode, selectable = !data.HasPartPower }, Icon = "icon-folder  ace-icon fa fa-folder orange" });
            }
            List<TreeDataView> tree = new List<TreeDataView>();
            string parentID = "0";
            string _rootText = "";
            var pt = powerDepts.FirstOrDefault<OrgDept>(t => t.Pid == "0" || t.Pid.IsMissing() || t.Pid == "#" || t.Pid == "~");
            if (pt != null)
            {
                parentID = pt.Fid;
                _rootText = pt.DeptName;
            }
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = parentID,
                Text = _rootText,
                State = new NodeState { Opened = true },
                Icon = "icon-folder purple ace-icon fa fa-sitemap",

            };
            if (parentID == "0")
            {
                treeRoot.Data = new { Code = "", selectable = false };
            }
            else
            {
                treeRoot.Data = new { Code = pt.DeptCode, selectable = !pt.HasPartPower };
            }
            TreeViewHelper.MakeTree(treeRoot.Children, treeList, treeRoot.Id);
            tree.Add(treeRoot);

            return JsonIgnoreNull(tree);
        }
        //[HttpGet]
        //[Route("~/api/orgdept/validedeptpermission/{fid=''}")]
        //public bool ValideDeptPermission(string fid)
        //{
        //    IEnumerable<OrgDept> powerDepts = _rbacService.GetDeptInfoAuthority(_applicationContext.CurrentRoleUid);
        //    if (powerDepts == null || powerDepts.Count() < 1)
        //    {
        //        return false;
        //    }
        //    powerDepts = powerDepts.Where(d => d.HasPartPower == false);
        //    if (powerDepts != null && powerDepts.Any() && powerDepts.FirstOrDefault(d => d.Fid == fid) != null)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
        /// <summary>
        /// 合并部门
        /// </summary>
        /// <param name="formObj"></param>
        /// <returns></returns>
        [HttpPost("Merge")]
        // POST: api/Common
        public JsonResult PostMergeDept(MergeDeptModel mergeDept)
        {
            var rv = _organizationService.MergeDepartment(mergeDept);
            return Json(rv);
        }
        /// <summary>
        /// 移动部门
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="id"></param>
        /// <param name="parent"></param>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        [HttpPost("MoveDept")]
        public JsonResult GetMoveDept(TreePostData postData)
        {
            var rv = _organizationService.MoveDepartment(postData);
            return Json(rv);
        }     
        /// <summary>
        /// GoJs 组织结构图
        /// </summary>
        /// <param name="fid">选中部门</param>
        /// <returns></returns>
        [HttpGet("OrgChartGojs/{fid}")]
        public JsonResult GetOrgChart2(string fid)
        {
            //人员类别设置
            string sql = "select count(0) as Num,DeptUid from Employee where 1=1 ";
            DynamicParameters parameters = new DynamicParameters();
            string empCategorys = _configService.GetSysParamValue("system.stat.empcategory");
            if (empCategorys.IsPresent())
            {
                sql += " and EmpCategory in @EmpCategorys ";
                parameters.Add("EmpCategorys", new List<string>(empCategorys.Split(',')));
            }

            //List<OrgDept> deptList = _dbContext.QueryEntity<OrgDept>(true);
            //查找有权限部门
            IEnumerable<OrgDept> deptList = _rbacService.GetDeptInfoAuthority(_applicationContext.CurrentRoleUid);
            IEnumerable<string> powerdepts = deptList.Where(d => d.HasPartPower == false).Select(d => d.Fid);
            if (powerdepts != null && powerdepts.Any())
            {
                sql += " and  DeptUid in @PowerDepts";
                parameters.Add("PowerDepts", powerdepts);
            }
            sql += " group by DeptUid";
            IEnumerable<DeptStat> list = _dbContext.Query<DeptStat>(sql, parameters);

            //获取选中部门
            OrgDept parent = deptList.FirstOrDefault<OrgDept>(d => d.Fid == fid);
            //过滤子部门 根据DeptCode
            IEnumerable<OrgDept> depts = deptList.Where(d => d.DeptCode.StartsWith(parent.DeptCode,StringComparison.OrdinalIgnoreCase)).OrderBy(d => d.DeptOrder);
            var employees = _dbContext.QueryAll<Fap.Core.Rbac.Model.Employee>();
            //人数计算
            depts.ToList().ForEach((d) =>
            {
                d.EmpNum = (list.FirstOrDefault(e => e.DeptUid == d.Fid)?.Num.ToString() ?? "0").ToInt();
            });
            depts.ToList().ForEach((d) =>
            {
                d.EmpNum = depts.Where(dd => dd.DeptCode.StartsWith(d.DeptCode,StringComparison.OrdinalIgnoreCase)).Sum(dd => dd.EmpNum);
                if (d.DeptManager.IsPresent())
                {
                    d.DeptManagerMC = employees.FirstOrDefault(e => e.Fid == d.DeptManager)?.EmpName;
                }
                if (d.Director.IsPresent())
                {
                    d.DirectorMC = employees.FirstOrDefault(e => e.Fid == d.Director)?.EmpName;
                }
            });
            IEnumerable<OrgChartNode2> nodeList = depts
            .Select<OrgDept, OrgChartNode2>(d => new OrgChartNode2 { Key = d.Fid, Parent = d.Pid, DeptName = d.DeptName, DeptCode = d.DeptCode, ManagerUid = d.DeptManager, ManagerName = d.DeptManagerMC, DirectorUid = d.Director, DirectorName = d.DirectorMC, DeptNum = d.EmpNum.ToString() });
            //IEnumerable<OrgChartNode> nodeList = deptList.Select<OrgDept, OrgChartNode>(d => new OrgChartNode { Id = d.Id, Parent = (deptList.FirstOrDefault(t => t.Fid == d.Pid) == null ? 0 : deptList.FirstOrDefault(t => t.Fid == d.Pid).Id), Text = d.DeptName, ExtendData=d.DeptCode });
            return Json(nodeList);
        }

        [HttpGet("OrgJobGroup")]
        public JsonResult OrgJobGroupTree()
        {
            var tree = _organizationService.GetOrgJobGroupTree();
            return Json(tree);
        }
        /// <summary>
        /// 岗位图
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        //[HttpGet]
        //[Route("~/api/orgdept/positionchartgojs/{fid=''}")]
        //public JsonResult GetJobChart2(string fid)
        //{
        //    string histroyDate = "";
        //    if (Request.Query.ContainsKey("date"))
        //    {
        //        histroyDate = Request.Query["date"].ToString();
        //    }

        //    //List<OrgDept> deptList = _dbContext.QueryEntity<OrgDept>(true);
        //    //查找有权限部门
        //    IEnumerable<OrgDept> deptList = _rbacService.GetDeptInfoAuthority(_applicationContext.CurrentRoleUid);
        //    string sql = $"select count(0) as  Num,DeptUid from Employee  ";
        //    DynamicParameters parameters = new DynamicParameters();
        //    string empCategorys = _configService.GetSysParamValue("system.stat.empcategory");
        //    if (empCategorys.IsPresent())
        //    {
        //        sql += " where EmpCategory in (@EmpCategorys) ";
        //        parameters.Add("EmpCategorys", empCategorys.Split(','));
        //    }
        //    sql += "   group by DeptUid";
        //    IEnumerable<DeptStat> list = null;

        //    _dbContext.HistoryDateTime = histroyDate;
        //    list = _dbContext.Query<DeptStat>(sql, parameters);


        //    //获取选中部门
        //    OrgDept parent = deptList.FirstOrDefault<OrgDept>(d => d.Fid == fid);


        //    var selDepts = deptList.Where(d => d.DeptCode.StartsWith(parent.DeptCode));
        //    selDepts.ToList().ForEach((d) =>
        //    {
        //        d.EmpNum = Convert.ToInt32(list.FirstOrDefault(e => e.DeptUid == d.Fid) == null ? "0" : list.FirstOrDefault(e => e.DeptUid == d.Fid).Num.ToString());
        //    });
        //    selDepts.ToList().ForEach((d) =>
        //    {
        //        d.EmpNum = selDepts.Where(dd => dd.DeptCode.StartsWith(d.DeptCode)).Sum(dd => dd.EmpNum);
        //    });

        //    //过滤子部门 根据DeptCode
        //    IEnumerable<OrgChartNode2> nodeList = selDepts.Select<OrgDept, OrgChartNode2>(d => new OrgChartNode2 { Key = d.Fid, Parent = d.Pid, OrgType = "dept", DeptName = d.DeptName, DeptCode = d.DeptCode, ManagerUid = d.DeptManager, ManagerName = d.DeptManagerMC, DeptNum = d.EmpNum.ToString() });

        //    //获取所有的职位
        //    IEnumerable<OrgPosition> positionList = null;

        //    _dbContext.HistoryDateTime = histroyDate;
        //    positionList = _dbContext.QueryWhere<OrgPosition>("");

        //    foreach (var dept in selDepts)
        //    {
        //        var ps = positionList?.Where(p => p.DeptUid == dept.Fid);
        //        if (ps != null && ps.Any())
        //        {
        //            var pNodes = ps.Select(d => new OrgChartNode2 { Key = d.Fid, Parent = d.DeptUid, OrgType = "position", DeptName = d.PstName, DeptCode = d.PstCode, ManagerUid = "", ManagerName = "", DeptNum = d.Actual.ToString() });
        //            nodeList = nodeList.Concat(pNodes);
        //        }
        //    }

        //    //IEnumerable<OrgChartNode> nodeList = deptList.Select<OrgDept, OrgChartNode>(d => new OrgChartNode { Id = d.Id, Parent = (deptList.FirstOrDefault(t => t.Fid == d.Pid) == null ? 0 : deptList.FirstOrDefault(t => t.Fid == d.Pid).Id), Text = d.DeptName, ExtendData=d.DeptCode });
        //    return Json(nodeList);
        //}
 
    }
}