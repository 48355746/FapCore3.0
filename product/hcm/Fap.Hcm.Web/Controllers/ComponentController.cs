using System;
using System.Collections.Generic;
using System.Linq;
using Fap.Core.Extensions;
using Fap.AspNetCore.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Fap.Core.Annex;
using Fap.Core.Infrastructure.Model;
using System.IO;
using Fap.Core.Infrastructure.Metadata;
using System.Text.RegularExpressions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.DataAccess;
using Newtonsoft.Json.Linq;
using Fap.AspNetCore.ViewModel;
using Fap.AspNetCore.Controls;
using Fap.Core.Rbac.Model;
using Fap.Core.Infrastructure.Query;
using Dapper;
using System.Web;
using Fap.AspNetCore.Controls.DataForm;

namespace Fap.Hcm.Web.Controllers
{
    public class ComponentController : FapController
    {
        private readonly IFapFileService _fapFileService;
        public ComponentController(IServiceProvider serviceProvider, IFapFileService fapFileService) : base(serviceProvider)
        {
            _fapFileService = fapFileService;
        }
        public FileResult Photo(string fid)
        {
            if (fid.IsMissing())
            {
                return File("~/Content/avatars/profile-pic.jpg", "image/png");
            }
            FapAttachment attachment = _fapFileService.DownloadOneFileByBid(fid, out Stream stream);
            if (attachment == null || stream == null)
            {
                return File("~/Content/avatars/profile-pic.jpg", "image/png");
            }
            else
            {
                return File(stream, attachment.FileType);
            }

        }


        //同时表单数据也会传递过来，条件设置参数同Dapper
        //例如：TableName=@Entity,Entity为form传过来的参数
        // GET: /Component/GridReference
        /// <summary>
        /// 表格参照
        /// </summary>
        /// <returns></returns>
        public IActionResult GridReference(string fid)
        {
            string formid = "frm";
            string ctrlid = "ctrl";
            if (Request.Query.ContainsKey("frmid"))
            {
                formid = Request.Query["frmid"].ToString();
            }
            if (Request.Query.ContainsKey("ctrlid"))
            {
                ctrlid = Request.Query["ctrlid"].ToString();
            }
            string refcondition = string.Empty;
            _platformDomain.ColumnSet.TryGetValue(fid, out FapColumn fc);
            //fc.RefCondition替换参数的值
            if (fc.RefCondition.IsPresent())
            {
                refcondition = fc.RefCondition;
                string fieldName = string.Empty;
                //
                Regex regex = new Regex(FapPlatformConstants.VariablePattern, RegexOptions.IgnoreCase);
                var mat = regex.Matches(fc.RefCondition);
                foreach (Match item in mat)
                {

                    fieldName = item.ToString().Substring(2, item.ToString().Length - 3);
                    if (!Request.Query.ContainsKey(fieldName))
                    {
                        fieldName = item.ToString();
                        if (fieldName.EqualsWithIgnoreCase(FapDbConstants.CurrentEmployee))
                        {
                            refcondition = refcondition.Replace(item.ToString(), _applicationContext.EmpUid);
                        }
                        else if (fieldName.EqualsWithIgnoreCase(FapDbConstants.CurrentUser))
                        {
                            refcondition = refcondition.Replace(item.ToString(), _applicationContext.UserName);
                        }
                        else if (fieldName.EqualsWithIgnoreCase(FapDbConstants.CurrentDept))
                        {
                            refcondition = refcondition.Replace(item.ToString(), _applicationContext.DeptUid);
                        }
                        else if (fieldName.EqualsWithIgnoreCase(FapDbConstants.CurrentDeptCode))
                        {
                            //OrgDept dept = da.Get<OrgDept>(_session.AcSession.Employee.DeptUid);
                            refcondition = refcondition.Replace(item.ToString(), _applicationContext.DeptCode);
                        }
                    }
                    else
                    {
                        refcondition = refcondition.Replace(item.ToString(), Request.Query[fieldName].ToString());
                    }
                }
            }
            List<string> refCols = new List<string>();
            List<string> frmCols = new List<string>();

            //参照名称
            List<string> refRefCols = new List<string>();
            List<string> frmRefCols = new List<string>();
            if (fc.RefReturnMapping.IsPresent())
            {
                JArray arrMapping = JArray.Parse(fc.RefReturnMapping);
                foreach (JObject item in arrMapping)
                {
                    string refCol = item.GetValue("RefCol").ToString();
                    refCols.Add(refCol);
                    string frmCol = item.GetValue("FrmCol").ToString();
                    frmCols.Add(frmCol);
                    string refTable = fc.RefTable;
                    //针对参照列的特殊处理
                    var refColumn = _dbContext.Column(refTable, refCol);
                    var frmColumn = _dbContext.Column(fc.TableName, frmCol);
                    if (refColumn != null && frmColumn != null)
                    {
                        if (refColumn.CtrlType == FapColumn.CTRL_TYPE_REFERENCE && frmColumn.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                        {
                            refRefCols.Add(refColumn.ColName + "MC");
                            frmRefCols.Add(frmColumn.TableName + frmColumn.ColName + "MC");
                        }
                    }
                }
            }
            string cols = "Id," + (!string.IsNullOrWhiteSpace(fc.RefID) ? fc.RefID + "," : "") + (!string.IsNullOrWhiteSpace(fc.RefCode) ? fc.RefCode + "," : "") + fc.RefName + "," + (!string.IsNullOrWhiteSpace(fc.RefCols) ? fc.RefCols : "");
            if (refCols.Count > 0)
            {
                cols = cols.TrimEnd(',') + "," + string.Join(",", refCols);
            }
            else
            {
                cols = cols.TrimEnd(',');
            }
            string[] colList = cols.Split(',');
            //注释掉人员权限，如果需要权限直接在条件中写
            //人员加权限
            if (fc.RefTable.EqualsWithIgnoreCase("employee") && !_applicationContext.IsAdministrator)
            {
                //包含权限设置
                if (refcondition.IsPresent() && !refcondition.Contains(FapDbConstants.EmployeeNoPower))
                {
                    string where = "DeptUid in(" + FapPlatformConstants.DepartmentAuthority + ")";
                    refcondition += " and " + where;
                }

            }
            JqGridViewModel model = this.GetJqGridModel(fc.RefTable, (q) =>
            {
                q.GlobalWhere = refcondition.Replace(FapDbConstants.EmployeeNoPower, "");
                q.QueryCols = string.Join(",", colList.Distinct());

            });
            model.JqgridId = $"ref{model.JqgridId}";
            if (refRefCols.Any())
            {
                refCols.AddRange(refRefCols);
                frmCols.AddRange(frmRefCols);
            }

            model.TempData.Add("refid", fc.RefID);
            model.TempData.Add("refcode", fc.RefCode);
            model.TempData.Add("refname", fc.RefName);
            model.TempData.Add("frmCols", string.Join(",", frmCols));
            model.TempData.Add("refCols", string.Join(",", refCols));
            return View(model);

        }
        /// <summary>
        /// 树表参照
        /// </summary>
        /// <returns></returns>
        public IActionResult TreeGridReference()
        {
            string formid = "frm";
            string ctrlid = "ctrl";
            if (Request.Query.ContainsKey("frmid"))
            {
                formid = Request.Query["frmid"].ToString();
            }
            if (Request.Query.ContainsKey("ctrlid"))
            {
                ctrlid = Request.Query["ctrlid"].ToString();
            }
            return View();
        }
        /// <summary>
        /// 树参照
        /// </summary>
        /// <returns></returns>
        public IActionResult TreeReference(string fid)
        {
            string formid = "frm";
            string ctrlid = "ctrl";
            if (Request.Query.ContainsKey("frmid"))
            {
                formid = Request.Query["frmid"].ToString();
            }
            if (Request.Query.ContainsKey("ctrlid"))
            {
                ctrlid = Request.Query["ctrlid"].ToString();
            }
            string refcondition = string.Empty;
            _platformDomain.ColumnSet.TryGetValue(fid, out FapColumn fc);
            //fc.RefCondition替换参数的值
            if (fc.RefCondition.IsPresent())
            {
                refcondition = fc.RefCondition;
                string fieldName = string.Empty;
                //
                Regex regex = new Regex(FapPlatformConstants.VariablePattern, RegexOptions.IgnoreCase);
                var mat = regex.Matches(fc.RefCondition);
                foreach (Match item in mat)
                {
                    fieldName = item.ToString().Substring(2, item.ToString().Length - 3);
                    refcondition = refcondition.Replace(item.ToString(), Request.Query[fieldName].ToString());
                }
            }

            string icon = "icon-folder  ace-icon fa fa-folder blue";
            if (!string.IsNullOrWhiteSpace(fc.Icon))
            {
                icon = fc.Icon;
            }
            List<string> refCols = new List<string>();
            List<string> frmCols = new List<string>();
            //参照名称
            List<string> refRefCols = new List<string>();
            List<string> frmRefCols = new List<string>();
            List<string> refMcSqls = new List<string>();
            if (fc.RefReturnMapping.IsPresent())
            {
                JArray arrMapping = JArray.Parse(fc.RefReturnMapping);
                foreach (JObject item in arrMapping)
                {
                    string refCol = item.GetStringValue("RefCol");
                    refCols.Add(refCol);
                    string frmCol = item.GetStringValue("FrmCol");
                    frmCols.Add(frmCol);
                    string refTable = fc.RefTable;
                    //针对参照列的特殊处理
                    var refColumn = _dbContext.Column(refTable, refCol);
                    var frmColumn = _dbContext.Column(fc.TableName, frmCol);
                    if (refColumn != null && frmColumn != null)
                    {
                        if (refColumn.CtrlType == FapColumn.CTRL_TYPE_REFERENCE && frmColumn.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                        {
                            refMcSqls.Add($"(select {refColumn.RefName} from {refColumn.RefTable} where {refColumn.RefTable}.{refColumn.RefID}={refColumn.TableName}.{refColumn.ColName} ) as {refColumn.ColName}MC");

                            refRefCols.Add(refColumn.ColName + "MC");
                            frmRefCols.Add(frmColumn.TableName + frmColumn.ColName + "MC");
                        }
                    }
                }
            }
            List<TreeDataView> treeList = new List<TreeDataView>();
            //非部门参照
            if (!fc.RefTable.EqualsWithIgnoreCase("orgdept"))
            {
                string sql = string.Format("select {0} as Id,{1} as Text,Pid,'{2}' as Icon from {3} ", fc.RefID, fc.RefName, icon, fc.RefTable);
                if (refCols.Any())
                {
                    sql = $"select {fc.RefID} as Id,{fc.RefName} as Text,Pid,{ string.Join(",", refCols)},";
                    if (refMcSqls.Any())
                    {
                        sql += $"{ string.Join(",", refMcSqls)},";
                    }
                    sql += $"'{icon}' as Icon from { fc.RefTable}";
                }
                if (!string.IsNullOrWhiteSpace(fc.RefCondition))
                {
                    sql += " where " + refcondition;
                }
                var dataList = _dbContext.Query(sql);

                //将List<dynamic>转换成List<TreeDataView>

                foreach (var data in dataList)
                {
                    TreeDataView tdv = new TreeDataView() { Id = data.Id, Text = data.Text, State = new NodeState { Opened = false }, Pid = data.Pid, Icon = data.Icon };
                    if (refCols.Count > 0)
                    {
                        IDictionary<string, object> d = data as IDictionary<string, object>;
                        List<string> refValues = new List<string>();
                        foreach (var col in refCols)
                        {
                            refValues.Add(d[col] + "");
                        }
                        //参照列
                        if (refRefCols.Any())
                        {
                            foreach (var col in refRefCols)
                            {
                                refValues.Add(d[col] + "");
                            }
                        }
                        tdv.Data = new { ext = string.Join("^-^", refValues), selectable = true };
                    }
                    else
                    {
                        tdv.Data = new { ext = "", selectable = true };
                    }
                    treeList.Add(tdv);
                }
            }
            else
            {
                IEnumerable<OrgDept> powerDepts = _rbacService.GetDeptInfoAuthority(_applicationContext.CurrentRoleUid);
                if (powerDepts != null && powerDepts.Any())
                {
                    foreach (var data in powerDepts)
                    {
                        TreeDataView tdv = new TreeDataView() { Id = data.Fid, Text = data.DeptName, State = new NodeState { Opened = false }, Pid = data.Pid, Icon = icon };
                        if (refCols.Count > 0)
                        {
                            List<string> refValues = new List<string>();
                            foreach (var col in refCols)
                            {
                                refValues.Add(data.GetType().GetProperty(col).GetValue(data, null) + "");
                            }
                            //参照列
                            if (refRefCols.Any())
                            {
                                foreach (var col in refRefCols)
                                {
                                    refValues.Add(data.GetType().GetProperty(col).GetValue(data, null) + "");
                                }
                            }

                            tdv.Data = new { ext = string.Join("^-^", refValues), selectable = !data.HasPartPower };
                            if (data.HasPartPower)
                            {
                                tdv.Icon = "icon-folder  ace-icon fa fa-ban orange";
                            }
                        }
                        else
                        {
                            tdv.Data = new { ext = "", selectable = !data.HasPartPower };
                        }
                        treeList.Add(tdv);
                    }
                }
            }

            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = "请选择",
                State = new NodeState { Opened = true },
                Icon = "icon-folder purple ace-icon fa 	fa-flag",
            };
            TreeViewHelper.MakeTree(treeRoot.Children, treeList, treeRoot.Id);
            tree.Add(treeRoot);

            string rej = tree.ToJsonIgnoreNullValue();
            //包含参照列
            if (refRefCols.Any())
            {
                refCols.AddRange(refRefCols);
                frmCols.AddRange(frmRefCols);
            }
            JsTreeViewModel treeModel = new JsTreeViewModel();
            treeModel.CtrlName = formid + ctrlid + fc.TableName + fc.ColName;
            treeModel.JsonData = rej;
            treeModel.TempData.Add("refid", fc.RefID);
            treeModel.TempData.Add("refcode", fc.RefCode);
            treeModel.TempData.Add("refname", fc.RefName);
            treeModel.TempData.Add("frmCols", string.Join(",", frmCols));
            treeModel.TempData.Add("refCols", string.Join(",", refCols));
            return View(treeModel);
        }
        public IActionResult DataGrid(string tableName)
        {
            JqGridViewModel model = GetJqGridModel(tableName);
            return View(model);
        }

        /// <summary>
        ///  表单
        /// </summary>
        /// <param name="fid">数据Fid</param>
        /// <param name="gid">jqgridId</param>
        /// <param name="menuid">菜单uid</param>
        /// <param name="fs">单据状态</param>
        /// <param name="qrycols">优先级高，指定的col</param>
        /// <returns></returns>
        public IActionResult DataForm(string fid, string gid, string menuid,int fs,string qrycols)
        {
            if (_platformDomain.MenuColumnSet.TryGetValue(menuid, out IEnumerable<FapMenuColumn> menuColumns))
            {
                var menuColumn = menuColumns.Where(r => r.GridId == gid).FirstOrDefault();
                if (menuColumn != null)
                {
                    //检查权限
                    var roleCols = _rbacService.GetRoleColumnList(_applicationContext.CurrentRoleUid).Where(r => r.MenuUid == menuid && r.GridId == gid);
                    if (roleCols.Any())
                    {
                        var allColumn = _dbContext.Columns(menuColumn.TableName);
                        string queryCols =string.Join(',', allColumn.Where(c => roleCols.Select(r => r.ColumnUid).Distinct().Contains(c.Fid)).Select(c => c.ColName));
                        string readonlyCols = string.Join(',', allColumn.Where(c => roleCols.Where(r => r.ViewAble == 1).Select(r => r.ColumnUid).Contains(c.Fid)).Select(c => c.ColName));
                        FormViewModel fd = this.GetFormViewModel(menuColumn.TableName, menuColumn.GridId, fid, qs =>
                         {
                             qs.QueryCols = queryCols;
                             qs.ReadOnlyCols = readonlyCols;

                         });
                        fd.FormStatus = (FormStatus)fs;
                        //高优先级
                        if (qrycols.IsPresent())
                        {
                            fd.QueryOption.QueryCols = qrycols;
                        }
                        return View(fd);
                    }
                    else
                    {
                        FormViewModel fd = this.GetFormViewModel(menuColumn.TableName, menuColumn.GridId, fid, qs =>
                        {
                            qs.QueryCols = menuColumn.GridColumn;
                        });
                        fd.FormStatus = (FormStatus)fs;
                        //高优先级
                        if (qrycols.IsPresent())
                        {
                            fd.QueryOption.QueryCols = qrycols;
                        }
                        return View(fd);
                    }

                }
            }
            return Content("未授权");
        }
        
        /// <summary>
        /// 自由表单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fid"></param>
        /// <param name="tn"></param>
        /// <param name="frm"></param>
        /// <returns></returns>
        public IActionResult FreeForm(string fid = "", string tn = "", string frm = "")
        {
            if (frm == "")
            {
                //jqgrid弹出窗口，统一设置为这个值，frmname即为frm-tablename
                frm = "jqgriddataform";
            }
            bool hasScroll = true;
            if (Request.Query.ContainsKey("noscroll"))
            {
                hasScroll = false;
            }
            //var tn = Request.Query["tn"];
            FormViewModel fd = new FormViewModel();
            fd.FormId = frm;
            QuerySet sq = new QuerySet();
            sq.TableName = tn;

            sq.InitWhere = "Fid=@Fid";
            sq.Parameters.Add(new Parameter("Fid", fid));


            fd.QueryOption = sq;
            fd.TableName = tn;
            ViewBag.Scroll = hasScroll;
            return View(fd);


        }
        /// <summary>
        /// 自由表单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="fid"></param>
        /// <param name="tn"></param>
        /// <param name="frm"></param>
        /// <returns></returns>
        public IActionResult FreeFormView(string fid, string tn = "", string frm = "")
        {
            if (frm == "")
            {
                //jqgrid弹出窗口，统一设置为这个值，frmname即为frm-tablename
                frm = "jqgriddataform";
            }
            bool hasScroll = true;
            if (Request.Query.ContainsKey("noscroll"))
            {
                hasScroll = false;
            }
            //var tn = Request.Query["tn"];
            FormViewModel fd = new FormViewModel();
            fd.FormId = frm;
            QuerySet sq = new QuerySet();
            sq.TableName = tn;

            sq.InitWhere = "Fid=@Fid";
            sq.Parameters.Add(new Parameter("Fid", fid));


            fd.QueryOption = sq;
            fd.TableName = tn;
            ViewBag.Scroll = hasScroll;
            return View(fd);
        }
        /// <summary>
        /// 表格组件
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="multi"></param>
        /// <returns></returns>
        public IActionResult GridComponent(string fid, string multi)
        {
            FapComponent fc = _dbContext.Get<FapComponent>(fid);

            JqGridViewModel model = this.GetJqGridModel(fc.GridTableName, (q) =>
            {
                q.QueryCols = fc.GridDisplayFields;
                q.InitWhere = fc.TableCondition;

            });
            model.JqgridId = fid;
            model.IsMulti = multi.ToBool();

            model.TempData.Add("returnfields", fc.ReturnFields);


            return View(model);

        }
        /// <summary>
        /// 树组件
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="multi"></param>
        /// <returns></returns>
        public IActionResult TreeComponent(string fid, string multi)
        {
            FapComponent fc = _dbContext.Get<FapComponent>(fid);
            string icon = "icon-folder  ace-icon fa fa-folder blue";

            //将List<dynamic>转换成List<TreeDataView>
            List<TreeDataView> treeList = new List<TreeDataView>();
            if (!string.IsNullOrWhiteSpace(fc.TreeNodeIcon))
            {
                icon = fc.TreeNodeIcon;
            }
            if (!fc.TreeTableName.EqualsWithIgnoreCase("orgdept"))
            {
                string sql = string.Format("select Fid as Id,{0} as Text,Pid,'{1}' as Icon from {2} ", fc.TreeDisplayField, icon, fc.TreeTableName);
                if (!string.IsNullOrWhiteSpace(fc.TreeCondition))
                {
                    sql += " where " + fc.TreeCondition;
                }
                if (!string.IsNullOrWhiteSpace(fc.TreeOrder))
                {
                    sql += " order by " + fc.TreeOrder;
                }
                var dataList = _dbContext.Query(sql);

                foreach (var data in dataList)
                {
                    treeList.Add(new TreeDataView() { Id = data.Id, Text = data.Text, Pid = data.Pid, Icon = data.Icon, Data = new { selectable = true } });
                }
            }
            else
            {
                IEnumerable<OrgDept> powerDepts = _rbacService.GetDeptInfoAuthority(_applicationContext.CurrentRoleUid);
                if (powerDepts != null && powerDepts.Any())
                {
                    foreach (var data in powerDepts)
                    {
                        TreeDataView tdv = new TreeDataView() { Id = data.Fid, Text = data.DeptName, State = new NodeState { Opened = true }, Pid = data.Pid, Icon = icon, Data = new { selectable = !data.HasPartPower } };
                        if (data.HasPartPower)
                        {
                            tdv.Icon = "icon-folder  ace-icon fa fa-ban orange";
                        }
                        treeList.Add(tdv);
                    }
                }
            }
            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = "请选择",
                State = new NodeState { Opened = true },
                Icon = "icon-folder purple ace-icon fa 	fa-flag",
            };
            TreeViewHelper.MakeTree(treeRoot.Children, treeList, treeRoot.Id);
            tree.Add(treeRoot);

            string rej = tree.ToJsonIgnoreNullValue();

            JsTreeViewModel treeModel = new JsTreeViewModel();
            treeModel.JsTreeId = fid;
            treeModel.IsMulti = multi.ToBool();
            treeModel.JsonData = rej;
            treeModel.TempData.Add("returnfields", fc.ReturnFields);
            return View(treeModel);
        }
        public IActionResult TreeGridComponent(string fid, string multi)
        {
            FapComponent fc = _dbContext.Get<FapComponent>(fid);
            TreeGridViewModel model = new TreeGridViewModel();
            model.TreeGridId = fid;
            model.IsMulti = multi.ToBool();
            model.TreeFilterCondition = fc.TreeFilterCondition;
            model.TreeTitle = fc.TreeTitle;
            model.GridTitle = fc.GridTitle;
            #region 表
            QuerySet qs = new QuerySet();
            qs.TableName = fc.GridTableName;
            qs.QueryCols = fc.GridDisplayFields;
            qs.InitWhere = fc.TableCondition;

            model.GridModel = new JqGridViewModel
            {
                QuerySet = qs
            };
            model.TempData.Add("returnfields", fc.ReturnFields);

            #endregion

            #region 树
            //将List<dynamic>转换成List<TreeDataView>
            List<TreeDataView> treeList = new List<TreeDataView>();
            string icon = "icon-folder  ace-icon fa fa-folder blue";
            if (!fc.TreeTableName.EqualsWithIgnoreCase("orgdept"))
            {
                if (!string.IsNullOrWhiteSpace(fc.TreeNodeIcon))
                {
                    icon = fc.TreeNodeIcon;
                }
                string sql = string.Format("select Fid as Id,{0} as Text,Pid,'{1}' as Icon from {2} ", fc.TreeDisplayField, icon, fc.TreeTableName);
                if (!string.IsNullOrWhiteSpace(fc.TreeCondition))
                {
                    sql += " where " + fc.TreeCondition;
                }
                if (!string.IsNullOrWhiteSpace(fc.TreeOrder))
                {
                    sql += " order by " + fc.TreeOrder;
                }
                var dataList = _dbContext.Query(sql);


                foreach (var data in dataList)
                {
                    treeList.Add(new TreeDataView() { Id = data.Id, Text = data.Text, Pid = data.Pid, Icon = data.Icon, Data = new { selectable = true } });
                }
            }
            else
            {
                IEnumerable<OrgDept> powerDepts = _rbacService.GetDeptInfoAuthority(_applicationContext.CurrentRoleUid);
                if (powerDepts != null && powerDepts.Any())
                {
                    foreach (var data in powerDepts)
                    {
                        TreeDataView tdv = new TreeDataView() { Id = data.Fid, Text = data.DeptName, State = new NodeState { Opened = false }, Pid = data.Pid, Icon = icon, Data = new { selectable = !data.HasPartPower } };
                        if (data.HasPartPower)
                        {
                            tdv.Icon = "icon-folder  ace-icon fa fa-ban orange";
                        }
                        treeList.Add(tdv);
                    }
                }
            }
            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = "请选择",
                State = new NodeState { Opened = true },
                Icon = "icon-folder purple ace-icon fa 	fa-flag",
            };
            TreeViewHelper.MakeTree(treeRoot.Children, treeList, treeRoot.Id);
            tree.Add(treeRoot);

            string rej = tree.ToJsonIgnoreNullValue();
            model.JsonData = rej;
            #endregion


            return View(model);
        }
        #region 附件相关
        public PartialViewResult AttachmentInfo(string fid)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("Bid", fid);
            IEnumerable<FapAttachment> atts = _dbContext.QueryWhere<FapAttachment>("Bid=@Bid", param);
            return PartialView(atts);
        }
        /// <summary>
        /// 查看附件图片
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileResult AttachmentImg(string fid)
        {
            FapAttachment attachment = _fapFileService.DownloadFileByFid(fid, out Stream strm);
            if (attachment == null || strm == null)
            {
                return File("~/Content/avatars/profile-pic.jpg", "image/png");
            }
            else
            {
                return new FileStreamResult(strm, attachment.FileType);
            }
        }

        /// <summary>
        /// 下载附件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DownloadFile(string fid)
        {
            FapAttachment attachment = _fapFileService.DownloadFileByFid(fid, out Stream strm);
            if (attachment == null || strm == null)
            {
                return Content("服务器未找到文件");
                //throw new FileNotFoundException("服务器未找到文件");
            }
            else
            {
                return File(strm, attachment.FileType, attachment.FileName);
            }

        }

        /// <summary>
        /// 下载Zip文件（多文件打包下载）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FileStreamResult DownloadZipFile(string fid)
        {
            FileStreamResult streamResult = null;
            _fapFileService.DownloadZip(fid, (zipStream) =>
            {
                if (zipStream == null)
                {
                    throw new FileNotFoundException("服务器未找到文件");
                }

                streamResult = File(zipStream, "application/x-zip-compressed", fid + ".zip");
            });

            return streamResult;
        }
        #endregion
    }
}