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

        [Route("~/api/orgdept/orgdepts/{pid=''}")]
        // POST: api/Common
        public JsonResult GetOrgDepts(string pid)
        {

            IEnumerable<OrgDept> listDepts = null;
            OrgDept pDept = null;
            if (pid.IsMissing())
            {
                pDept = _dbContext.QueryFirstOrDefaultWhere<OrgDept>("(Pid='' or Pid='~' or Pid='0')");
                var listDept = _dbContext.QueryAll<OrgDept>().AsList();
                listDept.Remove(pDept);
                listDepts = listDept;
            }
            else
            {
                pDept = _dbContext.Get<OrgDept>(pid);
                listDepts = _dbContext.QueryWhere<OrgDept>("DeptCode like '" + pDept.DeptCode + "%'");
            }

            List<TreeDataView> oriList = listDepts.Select(t => new TreeDataView { Id = t.Fid.ToString(), Pid = t.Pid.ToString(), Data = new { deptCode = t.DeptCode }, Text = t.DeptName, Icon = "icon-folder orange ace-icon fa fa-folder" }).ToList<TreeDataView>();

            List<TreeDataView> tree = new List<TreeDataView>();

            TreeDataView treeRoot = new TreeDataView()
            {
                Id = pDept.Fid,
                Text = pDept.DeptName,
                Data = new { deptCode = pDept.DeptCode },
                State = new NodeState { Opened = true },
                Icon = "icon-folder orange ace-icon fa fa-folder",
            };
            tree.Add(treeRoot);
            TreeViewHelper.MakeTree(treeRoot.Children, oriList, treeRoot.Id);
            return JsonIgnoreNull(tree);
        }
        /// <summary>
        /// 历史部门
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [Route("~/api/orgdept/historyorgdepts/{date=''}")]
        // POST: api/Common
        public JsonResult GetHistoryOrgDepts(string date)
        {
            //权限
            IEnumerable<OrgDept> powerDepts = _rbacService.GetDeptInfoAuthority(_applicationContext.CurrentRoleUid);// _dbContext.QueryEntityByWhere<OrgDept>(where, null, null, false, date);
            if (powerDepts != null && powerDepts.Any())
            {
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
            else
            {
                List<TreeDataView> tree = new List<TreeDataView>();
                TreeDataView treeRoot = new TreeDataView()
                {
                    Id = "0",
                    Text = "部门未建立或无权限",
                    Data = new { selectable = false },
                    State = new NodeState { Opened = true },
                    Icon = "icon-folder orange ace-icon fa  fa-ban",
                };
                tree.Add(treeRoot);
                return JsonIgnoreNull(tree);
            }
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
            try
            {
                var rv = _organizationService.MergeDepartment(mergeDept);
                return Json(rv);
            }
            catch (Exception ex)
            {
                return Json(ResponseViewModelUtils.Failure(ex.Message));
            }

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
        /// 移除部门
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/orgdept/removedept/{fid=''}")]
        public JsonResult GetRemoveDept(string fid)
        {
            bool success = false;
            string strResult = "";
            DynamicParameters param = new DynamicParameters();
            param.Add("Pid", fid);
            int count = _dbContext.Count("OrgDept", "Pid=@Pid", param);
            if (count > 0)
            {
                strResult = "此部门下还有部门，不能移除！请先移除其子部门。";
            }
            else
            {
                int ecount = _dbContext.Count("Employee", "EmpStatus='Current' and  DeptUid=@Pid", param);
                if (ecount > 0)
                {
                    strResult = "此部门下还有人员，不能移除。";
                }
            }
            if (strResult == "")
            {
                //移除操作
                dynamic fdo = new FapDynamicObject(_dbContext.Columns("OrgDept"));
                //fdo.TableName = "OrgDept";
                fdo.Fid = fid;//根据Fid进行更新操作
                _dbContext.DeleteDynamicData(fdo);
                success = true;
            }

            return Json(new { message = strResult, result = success });
        }
        /// <summary>
        /// 恢复已经移除的部门
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/orgdept/restoredept/{fid}/{historyDate}")]
        public JsonResult GetRestoreDept(string fid, string historyDate)
        {
            bool operResult = false;

            _dbContext.HistoryDateTime = historyDate;
            var dept = _dbContext.QueryFirstOrDefaultWhere("OrgDept", "Fid=@Fid and Dr=1", new DynamicParameters(new { Fid = fid }));
            if (dept != null && dept.Dr == 1)
            {
                dept.Dr = "0";
                //dept.EnableDate = PublicUtils.CurrentDateTimeStr;
                //dept.DisableDate = "9999-12-31 23:59:59";
                dept.Id = null;
                dynamic obj = new FapDynamicObject(_dbContext.Columns("OrgDept"));
                IDictionary<string, object> row = dept as IDictionary<string, object>;
                List<string> keyList = new List<string>(row.Keys);
                foreach (var key in keyList)
                {
                    obj.Add(key, row[key]);
                }
                obj.TableName = "OrgDept";
                _dbContext.InsertDynamicData(obj);
                operResult = true;
            }

            if (operResult)
            {

                return Json(new ResponseViewModel { msg = "已成功恢复部门" });
            }
            else
            {
                return Json(new ResponseViewModel { msg = "此部门没有被移除，不用恢复" });
            }
        }
        /// <summary>
        /// 精简组织结构图
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/orgdept/orgchart/")]
        public JsonResult GetOrgChart()
        {

            //List<OrgDept> deptList = _dbContext.QueryEntity<OrgDept>(true);
            //查找有权限部门
            IEnumerable<OrgDept> deptList = _rbacService.GetDeptInfoAuthority(_applicationContext.CurrentRoleUid);

            IEnumerable<OrgChartNode> nodeList = deptList.OrderBy(d => d.DeptOrder).Select<OrgDept, OrgChartNode>(d => new OrgChartNode { Id = d.Fid, Parent = d.Pid, Text = d.DeptName, ExtendData = d.DeptCode, Order = d.DeptOrder });
            nodeList = nodeList.OrderBy(n => n.Order);
            //IEnumerable<OrgChartNode> nodeList = deptList.Select<OrgDept, OrgChartNode>(d => new OrgChartNode { Id = d.Id, Parent = (deptList.FirstOrDefault(t => t.Fid == d.Pid) == null ? 0 : deptList.FirstOrDefault(t => t.Fid == d.Pid).Id), Text = d.DeptName, ExtendData=d.DeptCode });
            return Json(nodeList);
        }
        /// <summary>
        /// GoJs 组织结构图
        /// </summary>
        /// <param name="fid">选中部门</param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/orgdept/orgchartgojs/{fid=''}")]
        public JsonResult GetOrgChart2(string fid)
        {
            //人员类别设置
            string sql = "select count(0) as Num,DeptUid from employee where 1=1 ";
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
                sql += " and  DeptUid in (@PowerDepts)";
                parameters.Add("PowerDepts", powerdepts);
            }
            sql += " group by DeptUid";
            IEnumerable<DeptStat> list = _dbContext.Query<DeptStat>(sql, parameters);

            //获取选中部门
            OrgDept parent = deptList.FirstOrDefault<OrgDept>(d => d.Fid == fid);
            //过滤子部门 根据DeptCode
            IEnumerable<OrgDept> depts = deptList.Where(d => d.DeptCode.StartsWith(parent.DeptCode)).OrderBy(d => d.DeptOrder);
            var employees = _dbContext.QueryAll<Employee>();
            //人数计算
            depts.ToList().ForEach((d) =>
            {
                d.EmpNum = Convert.ToInt32(list.FirstOrDefault(e => e.DeptUid == d.Fid) == null ? "0" : list.FirstOrDefault(e => e.DeptUid == d.Fid).Num.ToString());
            });
            depts.ToList().ForEach((d) =>
            {
                d.EmpNum = depts.Where(dd => dd.DeptCode.StartsWith(d.DeptCode)).Sum(dd => dd.EmpNum);
                if (d.DeptManager.IsPresent())
                {
                    d.DeptManagerMC = employees.FirstOrDefault(e => e.Fid == d.DeptManager).EmpName;
                }
                if (d.Director.IsPresent())
                {
                    d.DirectorMC = employees.FirstOrDefault(e => e.Fid == d.Director).EmpName;
                }
            });
            IEnumerable<OrgChartNode2> nodeList = depts
            .Select<OrgDept, OrgChartNode2>(d => new OrgChartNode2 { Key = d.Fid, Parent = d.Pid, DeptName = d.DeptName, DeptCode = d.DeptCode, ManagerUid = d.DeptManager, ManagerName = d.DeptManagerMC, DirectorUid = d.Director, DirectorName = d.DirectorMC, DeptNum = d.EmpNum.ToString() });
            //IEnumerable<OrgChartNode> nodeList = deptList.Select<OrgDept, OrgChartNode>(d => new OrgChartNode { Id = d.Id, Parent = (deptList.FirstOrDefault(t => t.Fid == d.Pid) == null ? 0 : deptList.FirstOrDefault(t => t.Fid == d.Pid).Id), Text = d.DeptName, ExtendData=d.DeptCode });
            return Json(nodeList);
        }

        /// <summary>
        /// 岗位图
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/orgdept/positionchartgojs/{fid=''}")]
        public JsonResult GetJobChart2(string fid)
        {
            string histroyDate = "";
            if (Request.Query.ContainsKey("date"))
            {
                histroyDate = Request.Query["date"].ToString();
            }

            //List<OrgDept> deptList = _dbContext.QueryEntity<OrgDept>(true);
            //查找有权限部门
            IEnumerable<OrgDept> deptList = _rbacService.GetDeptInfoAuthority(_applicationContext.CurrentRoleUid);
            string sql = $"select count(0) as  Num,DeptUid from employee  ";
            DynamicParameters parameters = new DynamicParameters();
            string empCategorys = _configService.GetSysParamValue("system.stat.empcategory");
            if (empCategorys.IsPresent())
            {
                sql += " where EmpCategory in (@EmpCategorys) ";
                parameters.Add("EmpCategorys", empCategorys.Split(','));
            }
            sql += "   group by DeptUid";
            IEnumerable<DeptStat> list = null;

            _dbContext.HistoryDateTime = histroyDate;
            list = _dbContext.Query<DeptStat>(sql, parameters);


            //获取选中部门
            OrgDept parent = deptList.FirstOrDefault<OrgDept>(d => d.Fid == fid);


            var selDepts = deptList.Where(d => d.DeptCode.StartsWith(parent.DeptCode));
            selDepts.ToList().ForEach((d) =>
            {
                d.EmpNum = Convert.ToInt32(list.FirstOrDefault(e => e.DeptUid == d.Fid) == null ? "0" : list.FirstOrDefault(e => e.DeptUid == d.Fid).Num.ToString());
            });
            selDepts.ToList().ForEach((d) =>
            {
                d.EmpNum = selDepts.Where(dd => dd.DeptCode.StartsWith(d.DeptCode)).Sum(dd => dd.EmpNum);
            });

            //过滤子部门 根据DeptCode
            IEnumerable<OrgChartNode2> nodeList = selDepts.Select<OrgDept, OrgChartNode2>(d => new OrgChartNode2 { Key = d.Fid, Parent = d.Pid, OrgType = "dept", DeptName = d.DeptName, DeptCode = d.DeptCode, ManagerUid = d.DeptManager, ManagerName = d.DeptManagerMC, DeptNum = d.EmpNum.ToString() });

            //获取所有的职位
            IEnumerable<OrgPosition> positionList = null;

            _dbContext.HistoryDateTime = histroyDate;
            positionList = _dbContext.QueryWhere<OrgPosition>("");

            foreach (var dept in selDepts)
            {
                var ps = positionList?.Where(p => p.DeptUid == dept.Fid);
                if (ps != null && ps.Any())
                {
                    var pNodes = ps.Select(d => new OrgChartNode2 { Key = d.Fid, Parent = d.DeptUid, OrgType = "position", DeptName = d.PstName, DeptCode = d.PstCode, ManagerUid = "", ManagerName = "", DeptNum = d.Actual.ToString() });
                    nodeList = nodeList.Concat(pNodes);
                }
            }

            //IEnumerable<OrgChartNode> nodeList = deptList.Select<OrgDept, OrgChartNode>(d => new OrgChartNode { Id = d.Id, Parent = (deptList.FirstOrDefault(t => t.Fid == d.Pid) == null ? 0 : deptList.FirstOrDefault(t => t.Fid == d.Pid).Id), Text = d.DeptName, ExtendData=d.DeptCode });
            return Json(nodeList);
        }
        /// <summary>
        /// 同步在岗人数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/orgdept/synpositionnum")]
        public JsonResult GetSynPositionNum()
        {
            string histroyDate = "";
            if (Request.Query.ContainsKey("histroyDate"))
            {
                histroyDate = Request.Query["histroyDate"].ToString();
            }
            //统计人员类别设置
            DynamicParameters parameters = new DynamicParameters();
            string empCategoryWhere = "1=1";
            string empCategorys = _configService.GetSysParamValue("system.stat.empcategory");
            if (empCategorys.IsPresent())
            {
                empCategoryWhere = "  EmpCategory in (@EmpCategorys)";
                parameters.Add("EmpCategorys", empCategorys.Split(','));
            }

            _dbContext.HistoryDateTime = histroyDate;
            string sqlinit = " update OrgPosition  set Actual=0 ";
            _dbContext.Execute(sqlinit);
            string synSql = $" update OrgPosition set actual=a.Num from ( select empposition,count(0) Num from Employee where {empCategoryWhere}  group by EmpPosition) a where a.empposition=orgposition.fid  ";
            _dbContext.Execute(synSql, parameters);

            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpGet]
        [Route("~/api/orgdept/positionemps/{fid=''}")]
        public JsonResult GetPositionEmployees(string fid)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("PositionUid", fid);
            IEnumerable<Employee> list = _dbContext.QueryWhere<Employee>("EmpPosition=@PositionUid", param);

            return Json(list);
        }
        /// <summary>
        /// 是否存在说明书
        /// </summary>
        /// <param name="pstid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/orgdept/existpositionmanual/{pstid=''}")]
        public int GetExistPositionManual(string pstid)
        {

            DynamicParameters param = new DynamicParameters();
            param.Add("PositionUid", pstid);
            string sql = "select Id from OrgPositionManual where PositionUid=@PositionUid";
            IEnumerable<dynamic> list = _dbContext.Query(sql, param);
            if (list == null || list.Count() == 0)
            {
                return 0;
            }
            else
            {
                return list.First().Id;
            }

        }
        /// <summary>
        /// 查找说明书
        /// </summary>
        /// <param name="pstid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("~/api/orgdept/positionmanual/{pstid=''}")]
        public JsonResult GetPositionManual(string pstid)
        {

            DynamicParameters param = new DynamicParameters();
            param.Add("PositionUid", pstid);
            OrgPositionManual model = _dbContext.QueryFirstOrDefaultWhere<OrgPositionManual>("PositionUid=@PositionUid", param);
            if (model == null)
            {
                return Json(new OrgPositionManual { ManualContent = "" });
            }
            else
            {
                return Json(model);
            }
        }
        [Route("~/api/orgdept/depts/{power}")]
        // POST: api/Common
        public JsonResult GetOrgDeptsPower(string power)
        {
            IEnumerable<OrgDept> powerDepts = null;
            if (power.ToBool())
            {
                powerDepts = _rbacService.GetDeptInfoAuthority(_applicationContext.CurrentRoleUid);
            }
            else
            {
                powerDepts = _platformDomain.OrgDeptSet.OrderBy(d => d.DeptOrder);
            }
            //将List<dynamic>转换成List<TreeDataView>
            List<TreeDataView> treeList = new List<TreeDataView>();
            foreach (var data in powerDepts)
            {
                treeList.Add(new TreeDataView() { Id = data.Fid, Text = data.DeptName, Pid = data.Pid, Data = new { Code = data.DeptCode, Ext1 = data.HasPartPower, Ext2 = "" }, Icon = "icon-folder  ace-icon fa fa-folder orange" });
            }
            List<TreeDataView> tree = new List<TreeDataView>();
            string parentID = "0";
            var pt = powerDepts.FirstOrDefault<OrgDept>(t => t.Pid == "0" || t.Pid.IsMissing() || t.Pid == "#" || t.Pid == "~");
            string _rootText = "组织架构";
            if (_rootText.IsMissing())
            {
                if (pt != null)
                {
                    _rootText = pt.DeptName;
                    parentID = pt.Fid;
                }
                else
                {
                    _rootText = "无权限";
                }
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
                treeRoot.Data = new { Code = "", Ext1 = false, Ext2 = "" };
            }
            else
            {
                treeRoot.Data = new { Code = pt.DeptCode, Ext1 = pt.HasPartPower, Ext2 = "" };
            }
            tree.Add(treeRoot);
            TreeViewHelper.MakeTree(treeRoot.Children, treeList, treeRoot.Id);

            return Json(tree);
        }
    }
}