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
            qs.GlobalWhere = "";
            qs.QueryCols = "*";
            qs.OrderByList = new List<OrderBy>();
            //事件处理
            handler?.Invoke(qs);
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
            JqGridViewModel model = new JqGridViewModel()
            {
                JqgridId = tableName
            };
            QuerySet qs = new QuerySet()
            {
                TableName = tableName,
                InitWhere = "",
                GlobalWhere = "",
                QueryCols = "*"
            };
            //事件处理
            handler?.Invoke(qs);
            var allCols = _dbContext.Columns(tableName);
            //默认排序
            var sorts = allCols.Where(c => c.SortDirection.IsPresent());
            foreach (var sort in sorts)
            {
                qs.AddOrderBy(sort.ColName, sort.SortDirection);
            }
            //当排序没有设置的时候，设置创建时间倒序排列
            if (!qs.OrderByList.Any())
            {
                qs.AddOrderBy("CreateDate", "desc");
            }
            qs.InitWhere = qs.InitWhere.IsPresent() ? ReplaceSafeSql(qs.InitWhere) : "";
            qs.GlobalWhere = qs.GlobalWhere.IsPresent() ? ReplaceSafeSql(qs.GlobalWhere) : "";
            model.QuerySet = qs;            
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