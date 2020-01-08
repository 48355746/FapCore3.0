using Fap.Core.Extensions;
using Fap.AspNetCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using Fap.AspNetCore.Controls.JqGrid;
using Fap.Core.Infrastructure.Query;
using Fap.Core.Rbac.Model;
using Microsoft.AspNetCore.Authorization;
using Fap.AspNetCore.Model;
using Fap.Core.Utility;

namespace Fap.AspNetCore.Infrastructure
{
    /// <summary>
    /// 所有控制器必须继承此类
    /// </summary>
    [Authorize]
    public class FapController : BaseController
    {
        public FapController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    
        protected FormViewModel GetFormViewModel(string tableName, string fid, Action<QuerySet> handler = null)
        {
            FormViewModel model = new FormViewModel();
            model.FormId = $"frm-{tableName}";

            QuerySet qs = new QuerySet();
            qs.TableName = tableName;
            qs.InitWhere = "";
            qs.GlobalWhere = "";
            qs.QueryCols = "*";
            qs.OrderByList = new List<OrderBy>();
            //事件处理
            if (handler != null)
            {
                handler(qs);
            }
            if (fid.IsMissing())
            {
                fid = UUIDUtils.Fid;
            }
            qs.InitWhere = "Fid=@Fid";
            qs.Parameters.Add(new Parameter("Fid", fid));

            model.QueryOption = qs;
            model.TableName = tableName;
            return model;
        }
        /// <summary>
        /// 返回带有JqGrid的Model
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="handler">处理事件</param>
        /// <param name="hasOperCol">包含操作列</param>
        /// <param name="initCondition">初始化条件</param>
        /// <param name="isGlobalCondition">是否为全局条件</param>
        /// <returns></returns>
        protected JqGridViewModel GetJqGridModel(string tableName, Action<QuerySet> handler = null, bool hasOperCol = false)
        {
            JqGridViewModel model = new JqGridViewModel();
            model.JqgridId = tableName;
            QuerySet qs = new QuerySet();
            qs.TableName = tableName;
            qs.InitWhere = "";
            qs.GlobalWhere = "";
            qs.QueryCols = "*";
            qs.OrderByList = new List<OrderBy>();
            //事件处理
            if (handler != null)
            {
                handler(qs);
            }
            var allCols = _dbContext.Columns(tableName);
            //加权限
            if (qs.UsePermissions)
            {
                //IEnumerable<FapRoleColumn> rcs = _rbacService.GetRoleColumnList(_applicationContext.CurrentRoleUid).Where(f => f.TableUid == qs.TableName);
                //if (rcs != null && rcs.Any())
                //{
                //    //有权限的列
                //    var rCols = allCols.Where(tc => rcs.ToList().Exists(rc => rc.ColumnUid == tc.Fid) && tc.IsDefaultCol == 0 && tc.ShowAble == 1);
                //    if (qs.QueryCols == "*")
                //    {
                //        qs.QueryCols = "Id,Fid," + string.Join(",", rCols.Select(f => f.ColName).ToList());
                //    }
                //    else
                //    {
                //        char[] ch = { ',' };
                //        string[] qcols = qs.QueryCols.Replace(" ", "").Split(ch, StringSplitOptions.RemoveEmptyEntries);
                //        var notPowerCols = qcols.Where(f => !rCols.Select(r => r.ColName).ToList().Contains(f));
                //        if (notPowerCols != null && notPowerCols.Any())
                //        {
                //            //没在权限的列设置为隐藏
                //            qs.HiddenCols = string.Join(",", notPowerCols);
                //        }
                //        //var resultCols = rCols.Where(rc => qcols.Contains(rc.ColName, StringComparison.CurrentCultureIgnoreCase));
                //        //qs.QueryCols = "Id,Fid," + string.Join(",", resultCols.Select(f => f.ColName).ToList());

                //    }
                //}
            }
            else
            {
                //无权限设置
                if (qs.QueryCols == "*")
                {
                    qs.QueryCols = "Id,Fid," + string.Join(",", allCols.Where(f => f.IsDefaultCol == 0 && f.ShowAble == 1).Select(f => f.ColName).ToList());
                }
                //else
                //{
                //    char[] ch = { ',' };
                //    string[] qcols = qs.QueryCols.Replace(" ", "").Split(ch, StringSplitOptions.RemoveEmptyEntries);
                //    var resultCols = allCols.Where(f => qcols.Contains(f.ColName, StringComparison.CurrentCultureIgnoreCase) && f.IsDefaultCol == 0 && f.ShowAble == 1);
                //    qs.QueryCols = "Id,Fid," + string.Join(",", resultCols.Select(f => f.ColName).ToList());
                //}
            }
            //默认排序
            var sort = allCols.Where(c => c.SortDirection.IsPresent());
            if (sort != null && sort.Any())
            {
                sort.ToList().ForEach((c) =>
                {
                    qs.AddOrderBy(c.ColName, c.SortDirection);
                });
            }
            //当排序没有设置的时候，设置创建时间倒序排列
            if (qs.OrderByList.Count < 1)
            {
                qs.AddOrderBy("CreateDate", "desc");
            }
            qs.InitWhere = qs.InitWhere.IsPresent() ? ReplaceSafeSql(qs.InitWhere) : "";
            qs.GlobalWhere = qs.GlobalWhere.IsPresent() ? ReplaceSafeSql(qs.GlobalWhere) : "";
            model.QueryOption = qs;
            model.PostData = new PostData { QuerySet = qs, HasOperCol = hasOperCol };

            return model;
        }
        /// <summary>
        /// 替换安全sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private string ReplaceSafeSql(string sql)
        {
            return sql.ReplaceIgnoreCase("select ", "query ").ReplaceIgnoreCase("delete ", "").ReplaceIgnoreCase("update ", "").ReplaceIgnoreCase("insert ", "").ReplaceIgnoreCase("drop ", "").ReplaceIgnoreCase("alter ", "");
        }
        protected List<Column> CreateOperColumn(List<OperButton> operBtns, int width = 100)
        {
            List<Column> operColumns = new List<Column>();
            Column colOper = new Column("oper");
            colOper.SetLabel("操作");
            colOper.SetWidth(width);
            colOper.SetSortable(false);
            //colOper.SetCustomFormatter("function (value, grid, rows, state){   return \"<button class=\\\"btn btn-xs btn-warning\\\" onclick=\\\"EditFlow('\" + rows[1] + \"');\\\"><i class=\\\"ace-icon fa fa-flag bigger-110\\\"></i>编辑	</button>\"  }");
            operColumns.Add(colOper);

            colOper.SetOperButtons(operBtns);

            return operColumns;
        }
    }
}