using Fap.AspNetCore.Controls.JqGrid;
using Fap.AspNetCore.Controls.JqGrid.Enums;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Query;
using Fap.Core.MultiLanguage;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Rbac;
using System;
using Fap.Core.Rbac.Model;
using Fap.AspNetCore.ViewModel;

namespace Fap.AspNetCore.Controls.TagHelpers
{
    public class FapGridTagHelper : TagHelper
    {
        private IDbContext _dbContext;
        //private IOptions<FapOption> _fapOption;
        private ILogger<FapGridTagHelper> _logger;
        private ILoggerFactory _loggerFactory;
        private IFapApplicationContext _applicationContext;
        private IMultiLangService _multiLang;
        private readonly IRbacService _rbacService;
        public FapGridTagHelper(IDbContext dataAccessor, ILoggerFactory logger, IFapApplicationContext applicationContext, IMultiLangService multiLang, IRbacService rbacService)
        {
            _dbContext = dataAccessor;
            //_fapOption = fapOption;
            _loggerFactory = logger;
            _logger = logger.CreateLogger<FapGridTagHelper>();
            _applicationContext = applicationContext;
            _multiLang = multiLang;
            _rbacService = rbacService;
        }
        /// <summary>
        /// 控件ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 添加多列
        /// </summary>
        public IEnumerable<Column> AttachColumns { get; set; }
        /// <summary>
        /// 添加一列
        /// </summary>
        public Column AttachColumn { get; set; }
        /// <summary>
        /// 配置jqgrid模型
        /// </summary>
        public JqGridViewModel GridModel { get; set; }
        /// <summary>
        /// 自动宽度
        /// </summary>
        public bool AutoWidth { get; set; } = true;
        /// <summary>
        /// 获取数据的url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Post Url
        /// </summary>
        public string EditUrl { get; set; }
        /// <summary>
        /// 数据源类型(因为前端不能以 data-开头，所以改名)
        /// </summary>
        public DataType SourceType { get; set; } = DataType.Json;

        /// <summary>
        /// 包裹容器class(不带.),
        /// 例外tab:"TabControlId,TabItemId"
        /// </summary>
        public string Wrapper { get; set; }
        /// <summary>
        /// 自适应宽度(default:true)
        /// </summary>
        public bool ShrinkFit { get; set; } = true;
        /// <summary>
        /// 显示总条数
        /// </summary>
        public bool ViewRecords { get; set; } = true;
        /// <summary>
        /// 是否多选
        /// </summary>
        public bool MultiSelect { get; set; }
        /// <summary>
        /// 仅仅点复选框的时候多选
        /// </summary>
        public bool MultiBoxOnly { get; set; }
        /// <summary>
        /// 启用行内编辑(表单中的子表)
        /// </summary>
        public bool InlineFormEnabled { get; set; } = false;
        /// <summary>
        /// 启用行内编辑（单独grid）
        /// </summary>
        public bool InlineGridEnabled { get; set; } = false;
        /// <summary>
        /// 扩展自定义数据
        /// </summary>
        public Dictionary<string, string> ExtraData { get; set; }
        /// <summary>
        /// grid数据完成事件
        /// </summary>
        public string OnGridComplete { get; set; }
        /// <summary>
        /// 新增表单初始化事件
        /// </summary>
        public string OnFormInitAdd { get; set; }
        /// <summary>
        /// 列头分组设置
        /// </summary>
        public IEnumerable<GroupHeader> GroupHeaders { get; set; }
        /// <summary>
        /// 编辑表单初始化事件
        /// </summary>
        public string OnFormInitEdit { get; set; }
        /// <summary>
        /// 行选中事件
        /// </summary>
        public string OnSelectRow { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Caption { get; set; }
        /// <summary>
        /// 显示搜索工具栏
        /// </summary>
        public bool SearchToolbar { get; set; }
        public int Height { get; set; }

        /// <summary>
        /// 显示行数
        /// </summary>
        public int RowNum { get; set; } = 10;
        /// <summary>
        /// 是否显示列菜单
        /// </summary>
        public bool ColMenu { get; set; } = true;
        /// <summary>
        /// 是否为树表
        /// </summary>
        public bool IsTreeGrid { get; set; }
        /// <summary>
        /// 树展开列
        /// </summary>
        public string ExpandColumn { get; set; }
        /// <summary>
        /// 显示行数选择
        /// </summary>
        public bool RowNumbers { get; set; } = true;
        /// <summary>
        /// 分页工具条上方，用于统计行
        /// </summary>
        public bool FooterRow { get; set; } = false;
        /// <summary>
        ///  当为true时把userData放到底部，
        ///  用法：如果userData的值与colModel的值相同，
        ///  那么此列就显示正确的值，如果不等那么此列就为空
        /// </summary>
        public bool UserdataFooter { get; set; } = false;
        /// <summary>
        /// 显示新增按钮
        /// </summary>
        public bool OperAdd { get; set; }
        /// <summary>
        /// 显示修改按钮
        /// </summary>
        public bool OperUpdate { get; set; }
        /// <summary>
        /// 显示批量更新按钮
        /// </summary>
        public bool OperBatchUpdate { get; set; }
        /// <summary>
        /// 导出（word，excel）
        /// </summary>
        public bool OperExport { get; set; }
        /// <summary>
        /// 显示导出Excel按钮
        /// </summary>
        public bool OperExportExcel { get; set; }
        /// <summary>
        /// 导出word
        /// </summary>
        public bool OperExportWord { get; set; }
        /// <summary>
        /// 显示导入按钮
        /// </summary>
        public bool OperImport { get; set; }
        /// <summary>
        /// 增删改组合
        /// </summary>
        public bool OperCud { get; set; }
        /// <summary>
        /// 显示删除按钮
        /// </summary>
        public bool OperDelete { get; set; }
        /// <summary>
        /// 显示查询按钮
        /// </summary>
        public bool OperSearch { get; set; }
        /// <summary>
        /// 显示刷新按钮
        /// </summary>
        public bool OperRefresh { get; set; }
        /// <summary>
        /// 显示查询方案
        /// </summary>
        public bool OperQueryprogram { get; set; }
        /// <summary>
        /// 是否注册表格权限
        /// </summary>
        public bool RegisterAuthority { get; set; } = true;
        /// <summary>
        /// 是否为子表格
        /// </summary>
        public bool IsSubgrid { get; set; }
        /// <summary>
        /// subGrid设置展开内容
        /// function showChildGrid(parentRowID, parentRowKey) {
        /// $('#' + parentRowID).load($.randomUrl("@Url.Content("~/Workflow/Template/HistoryVersion/")"+parentRowKey));
        /// }
        /// </summary>
        public string SubgridRowexpanded { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            QuerySet querySet = GridModel.QuerySet;
            output.TagName = "div";
            output.Content.Clear();
            string id =GridModel.JqgridId;
            if (Id.IsPresent())
            {
                id = Id;
            }
            this.Id = $"grid-{id}";
            string pager = $"pager-{id}";
            Grid grid = new Grid(_dbContext, _rbacService, _applicationContext, _multiLang, this.Id);

            if (Url.IsPresent())
            {
                grid.SetUrl(Url);
            }
            else
            {
                if (IsTreeGrid)
                {
                    grid.SetUrl($"{ _applicationContext.BaseUrl }/Api/Core/TreeDataList");
                }
                else
                {
                    grid.SetUrl($"{ _applicationContext.BaseUrl }/Api/Core/DataList");
                }
            }
            if (EditUrl.IsPresent())
            {
                grid.SetEditUrl(EditUrl);
            }
            else
            {
                if (InlineFormEnabled)
                {
                    grid.SetEditUrl("clientArray");
                }
                else
                {
                    grid.SetEditUrl($"{_applicationContext.BaseUrl}/Api/Core/Persistence?from=grid");
                }
            }
            if (Caption.IsPresent())
            {
                grid.SetCaption(Caption);
            }
            if (AttachColumn != null)
            {
                grid.AddColumn(AttachColumn);
            }
            if (ExtraData != null)
            {
                grid.SetExtraData(ExtraData);
            }
            if (AttachColumns != null && AttachColumns.Any())
            {
                grid.AddColumns(AttachColumns);
            }

            //鉴权列
            string cols = GetColumnPermission(querySet);
            if (cols.IsPresent())
            {
                querySet.QueryCols = cols;
            }
            grid.SetQueryOption(querySet);
            //获取数据时ajax post数据
            grid.SetPostData(new { QuerySet = querySet });
            //鉴权操作
            string authorize = AuthenticationButton(querySet);
            //设置操作
            SetGirdOper(grid, authorize);

            grid.SetAutoWidth(AutoWidth);
            grid.SetDataType(SourceType);
            grid.SetViewRecords(ViewRecords);
            grid.SetShrinkToFit(ShrinkFit);
            grid.SetColMenu(ColMenu);

            if (Height > 0)
            {
                grid.SetHeight(Height);
            }
            if (Wrapper.IsPresent())
            {
                string[] wrapper = Wrapper.Split(',');
                if (wrapper.Length > 1)
                {
                    grid.SetInsideTabControl($"{wrapper[0]}", $"{wrapper[1]}");
                }
                else
                {
                    grid.SetInsideElement(Wrapper);
                }
            }
            else
            {
                grid.SetInsideWidget();
            }
            if (MultiSelect)
            {
                grid.SetMultiSelect(MultiSelect);
            }
            if (MultiBoxOnly)
            {
                grid.SetMultiBoxOnly(MultiBoxOnly);
            }
            if (RowNum != 10)
            {
                grid.SetRowNum(RowNum);
            }
            grid.SetRowNumbers(RowNumbers);
            if (InlineFormEnabled || InlineGridEnabled)
            {
                grid.SetEditRowModel(EditRowModel.Inline);
            }
            grid.SetFooterRow(FooterRow);
            grid.SetUserDataOnFooter(UserdataFooter);
            if (IsTreeGrid)
            {
                if (ExpandColumn.IsPresent())
                {
                    grid.SetExpandColumn(ExpandColumn);
                }
                grid.EnableTreeGrid();
            }
            if (OnGridComplete.IsPresent())
            {
                if (OnGridComplete.Contains("("))
                {
                    grid.OnGridComplete($"{OnGridComplete};");
                }
                else
                {
                    grid.OnGridComplete($"{OnGridComplete}();");
                }
            }
            if (OnFormInitAdd.IsPresent())
            {
                if (OnFormInitAdd.Contains("("))
                {
                    grid.OnAddAfterInitDataForm($"{OnFormInitAdd};");
                }
                else
                {
                    grid.OnAddAfterInitDataForm($"{OnFormInitAdd}();");
                }
            }
            if (OnFormInitEdit.IsPresent())
            {
                if (OnFormInitEdit.Contains("("))
                {
                    grid.OnEditAfterInitDataForm($"{OnFormInitEdit};");
                }
                else
                {
                    grid.OnEditAfterInitDataForm($"{OnFormInitEdit}();");
                }
            }
            if (OnSelectRow.IsPresent())
            {
                if (OnSelectRow.Contains("("))
                {
                    grid.OnSelectRow($"{OnSelectRow};");
                }
                else
                {
                    grid.OnSelectRow($"{OnSelectRow}(rowid, status);");
                }
            }

            if (SearchToolbar)
            {
                grid.SetSearchToolbar(SearchToolbar);
            }
            if (SubgridRowexpanded.IsPresent())
            {
                grid.SetSubGridRowExpanded(SubgridRowexpanded);
            }
            if (GroupHeaders != null && GroupHeaders.Any())
            {
                grid.SetGroupHeaders(GroupHeaders);
            }
            grid.SetPager(pager);

            output.Content.AppendHtml(grid.ToString());

        }
        private void SetGirdOper(Grid grid, string authorize)
        {
            if (authorize.IsMissing())
            {
                return;
            }
            var power = authorize.SplitComma().Select(v => v.ToInt());
            int formType = 0;
            foreach (int p in power)
            {
                if (IsTreeGrid && p == 128)
                {
                    //树表格不应用批量编辑
                    continue;
                }
                formType |= p;
            }
            grid.SetFormType((OperEnum)formType);
        }
        private string AuthenticationButton(QuerySet querySet)
        {
            if (RegisterAuthority)
            {
                string btnId = Id;
                if (IsSubgrid)
                {
                    btnId = Id.Substring(0, Id.LastIndexOf("_"));
                }
                FapMenuButton menuButton = new FapMenuButton()
                {
                    ButtonID = btnId,
                    ButtonName = "表格按钮",
                    ButtonType = FapMenuButtonType.Grid,
                    Description = _dbContext.Table(querySet.TableName).TableComment
                };
                //注册按钮
                return _rbacService.GetMenuButtonAuthority(_applicationContext.CurrentRoleUid, menuButton);
            }
            return string.Empty;
        }
        private string GetColumnPermission(QuerySet querySet)
        {
            if (RegisterAuthority)
            {
                string btnId = Id;
                if (IsSubgrid)
                {
                    btnId = Id.Substring(0, Id.LastIndexOf("_"));
                }
                FapMenuColumn menuColumn = new FapMenuColumn()
                {
                    GridId = btnId,
                    TableName = querySet.TableName,
                    GridColumn = querySet.QueryCols,
                    Enabled = 1,
                    Description = _dbContext.Table(querySet.TableName).TableComment
                };

                //获取列权限
                return _rbacService.GetMenuColumnAuthority(_applicationContext.CurrentRoleUid, menuColumn);
            }
            return string.Empty;
        }

    }
}
