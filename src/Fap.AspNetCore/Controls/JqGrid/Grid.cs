
using Dapper;
using Fap.AspNetCore.Controls.JqGrid.Enums;
//using Fap.Core.Extensions;
using Fap.AspNetCore.Controls.JqGrid.Extensions;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.Model;
using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Query;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.MultiLanguage;
using Fap.Core.Utility;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yahoo.Yui.Compressor;
using System.Text.Encodings.Web;
using System.Web;
using Fap.Core.Infrastructure.Enums;

namespace Fap.AspNetCore.Controls.JqGrid
{
    /// <summary>
    ///     Grid class, used to render JqGrid
    /// </summary>
    public class Grid : HtmlString
    {
        //grid所在的元素
        private string _insideElementClass = "page-content";
        private string _tabControlName;
        private string _tabItemName;
        private Dictionary<string, string> _tabExtraData; //表格的Data属性值
        private List<Column> _columns = new List<Column>();
        private IEnumerable<FapColumn> _fapColumns = new List<FapColumn>();
        //private readonly List<FapColumn> _fapColumns = new List<FapColumn>();
        private readonly string _id;
        private string _altClass;
        private bool? _altRows;
        private bool? _autoEncode;
        private bool? _autoWidth;
        private string _caption;
        //行编辑模式
        private EditRowModel _editRowModel = EditRowModel.Dialog;
        private DataType _dataType = DataType.Json;
        private string _emptyRecords = "暂无数据";
        private bool? _footerRow;
        private bool? _userDataOnFooter;
        private bool? _forceFit;
        private bool? _gridView;
        private bool? _headerTitles;
        private int? _height;
        private bool? _hiddenGrid;
        private bool? _hideGrid;
        private bool? _hoverRows;
        private bool? _loadOnce;
        private string _loadText;
        private LoadUi? _loadUi;
        private bool? _multiBoxOnly;
        private MultiKey? _multiKey;
        private bool? _multiSelect;
        private int? _multiSelectWidth;
        private string _onAfterInsertRow;
        //新增初始化表单后js事件
        private string _onAddAfterInitDataForm;
        //编辑初始化表单后js事件
        private string _onEditAfterInitDataForm;
        private string _onBeforeRequest;
        private string _onBeforeSelectRow;
        private string _onCellSelect;
        private string _onDblClickRow;
        private string _onGridComplete;
        private string _onHeaderClick;
        private string _onLoadBeforeSend;
        private string _onLoadComplete;
        private string _onLoadError;
        private string _onPaging;
        private string _onResizeStart;
        private string _onResizeStop;
        private string _onRightClickRow;
        private string _onSelectAll;
        private string _onSelectRow;
        private string _onSerializeGridData;
        private string _onSortCol;
        private int? _page;
        private string _pager;
        private PagerPos? _pagerPos;
        private bool? _pgButtons;
        private bool? _pgInput;
        private string _pgText;
        private RecordPos? _recordPos;
        private string _recordText;
        private RequestType? _requestType = RequestType.Post;
        private string _resizeClass;
        private int[] _rowList = new[] { 10, 20, 50, 10000 };
        private int? _rowNum = 10;
        private int? _rowNumWidth;
        private bool? _rowNumbers = true;
        private bool? _scroll;
        private int? _scrollInt;
        private int? _scrollOffset;
        private bool? _scrollRows;
        private int? _scrollTimeout;
        private bool? _searchClearButton;
        private bool? _searchOnEnter;
        private bool? _searchToolbar;
        private bool? _colMenu;
        private bool? _showAllSortIcons;
        private bool? _shrinkToFit;
        private Direction? _sortIconDirection;
        private string _sortName;
        private bool? _sortOnHeaderClick;
        private SortOrder? _sortOrder;
        private bool? _toolbar;
        private ToolbarPosition _toolbarPosition = ToolbarPosition.Top;
        private bool? _topPager;
        private string _url;//获取数据url
        private string _editUrl;//编辑url
        private bool? _viewRecords;
        private int? _width;
        private DataReaders.JsonReader _jsonReader;
        private bool? _searchToggleButton;
        private bool _enabledTreeGrid;
        //private int? _treeGridRootLevel;
        private TreeGridModel _treeGridModel;
        private bool? _asyncLoad;
        private bool _stringResult = true;
        private bool? _ignoreCase;
        private string _rowAttr;


        //列头分组设置
        private IEnumerable<GroupHeader> _groupHeaders;
        private string _expandColumn;//树展开点击的列
        //子面板
        private bool _subGrid = false;
        //设置子面板加载内容的js function
        private string _subGridRowExpanded;
        //subGrid: true, // set the subGrid property to true to show expand buttons for each row
        //subGridRowExpanded: showChildGrid, // javascript function that will take care of showing the child grid
        // the event handler on expanding parent row receives two parameters
        // the ID of the grid tow  and the primary key of the row
        //function showChildGrid(parentRowID, parentRowKey) {
        //    $.ajax({
        //        url: parentRowKey+".html",
        //        type: "GET",
        //        success: function (html) {
        //            $("#" + parentRowID).append(html);
        //        }
        //    });

        //}
        /// <summary>
        /// 根据元数据增删改
        /// </summary>
        //private bool? _crudByMetaEnable;
        /// <summary>
        /// 根据jqgrid默认的增删改
        /// </summary>
        // private bool? _crudDefaultEnable;
        /// <summary>
        /// 操作表单类型
        /// </summary>
        private OperEnum _dataFormType = OperEnum.Search;
        private string _postData;

        private IDbContext _dataAccessor;
        private ILoggerFactory _loggerFactory;
        private IFapApplicationContext _applicationContext;
        private IMultiLangService _multiLang;
        //分布式缓存
        //IDistributedCache
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name = "id">Id of grid</param>
        public Grid(IDbContext dataAccessor, ILoggerFactory logger, IFapApplicationContext applicationContext, IMultiLangService multiLang, string id) : base("")
        {
            if (id.IsMissing())
            {
                throw new ArgumentException("Id must contain a value to identify the grid");
            }
            _id = id;
            _dataAccessor = dataAccessor;
            _loggerFactory = logger;
            _applicationContext = applicationContext;
            _multiLang = multiLang;
        }
        private string TableName
        {
            get { return _fapColumns.First().TableName; }
        }
        /// <summary>
        ///    添加grid列,放在前面执行
        /// </summary>
        /// <param name = "column">Colomn object</param>
        public Grid AddColumn(Column column)
        {
            column.SetColmenu(false);
            _columns.Add(column);
            return this;
        }
        /// <summary>
        ///    添加多个grid列
        /// </summary>
        /// <param name = "columns">IEnumerable of Colomn objects to add to the grid</param>
        public Grid AddColumns(IEnumerable<Column> columns)
        {
            if (columns != null && columns.Any())
            {
                columns.ToList().ForEach((c) =>
                {
                    c.SetColmenu(false);
                });
            }
            _columns.AddRange(columns);
            return this;
        }
        /// <summary>
        /// 位于哪个元素里面.class，jqgrid会找相邻最近的class
        /// 用于计算宽度
        /// </summary>
        /// <param name="elementClass"></param>
        /// <returns></returns>
        public Grid SetInsideElement(string elementClass)
        {
            _insideElementClass = elementClass;
            return this;
        }
        public Grid SetInsideWidget()
        {
            _insideElementClass = "widget-main";
            return this;
        }

        public Grid SetExtraData(Dictionary<string, string> data)
        {
            _tabExtraData = data;
            return this;
        }
        /// <summary>
        /// 设置Grid处于TabControl下的TabControl的名字
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public Grid SetInsideTabControl(string tabControlId, string tabItemId)
        {
            _insideElementClass = "tab-pane";
            _tabControlName = tabControlId;
            _tabItemName = tabItemId;
            return this;
        }
        /// <summary>
        /// 自定义的json数据,提交到服务器端，会跟在url后面
        /// </summary>
        public Grid SetPostData(object postJsonData)
        {
            _postData = postJsonData.ToJsonIgnoreNullValue(false);
            return this;
        }
        private List<DefaultValue> _defaultValues = new List<DefaultValue>();
        private QuerySet _querySet;
        /// <summary>
        /// 设置简单查询设置，和SetFapColumns不同时使用
        /// </summary>
        /// <param name="queryOption"></param>
        /// <returns></returns>
        public Grid SetQueryOption(QuerySet queryset)
        {
            _querySet = queryset;
            //设置表单默认值
            if (queryset.DefaultValues != null && queryset.DefaultValues.Any())
            {
                _defaultValues = queryset.DefaultValues;
            }
            Pageable queryOption = new Pageable(_dataAccessor) { TableName = queryset.TableName, QueryCols = queryset.QueryCols };
            _fapColumns = _dataAccessor.Columns(queryset.TableName);
            if (!queryset.QueryCols.EqualsWithIgnoreCase("*"))
            {
                var queryColList = queryset.QueryCols.ToLower().SplitComma();
                _fapColumns = _fapColumns.Where(c => queryColList.Contains(c.ColName.ToLower()));
            }
            if (_fapColumns.Any())
            {
                List<string> disCols = new List<string>();
                List<string> hideCols = new List<string>(); ;
                if (queryset.DispalyCols.IsPresent())
                {
                    disCols = queryset.DispalyCols.ToLower().SplitComma();
                }
                if (queryset.HiddenCols.IsPresent())
                {
                    hideCols = queryset.HiddenCols.ToLower().SplitComma();
                }
                List<Column> grdColumns = _fapColumns.OrderBy(c => c.ColOrder).ToColumns(_loggerFactory, _dataAccessor, _multiLang, disCols, hideCols).ToList();

                _columns.AddRange(grdColumns);
            }
            return this;
        }
        /// <summary>
        /// 设置FapColumns，转化column
        /// </summary>
        /// <param name="fapColumns"></param>
        /// <returns></returns>
        //public Grid SetFapColumns(IEnumerable<FapColumn> fapColumns)
        //{
        //    if (fapColumns != null)
        //    {
        //        _fapColumns = fapColumns;
        //        _columns.AddRange(fapColumns.ToColumns(_dataAccessor,_multiLang));
        //    }
        //    return this;
        //}

        /// <summary>
        ///     用来指定行显示的css，可以编辑自己的css文件，只有当altRows设为 ture时起作用
        /// </summary>
        /// <param name = "altClass">Classname for alternate rows</param>
        public Grid SetAltClass(string altClass)
        {
            _altClass = altClass;
            return this;
        }

        /// <summary>
        ///     设置表格 斑马条纹 值 (default: false)
        /// </summary>
        /// <param name = "altRows">Boolean indicating if zebra-striped grid is used</param>
        public Grid SetAltRows(Boolean altRows)
        {
            _altRows = altRows;
            return this;
        }

        /// <summary>
        ///    对url进行编码(default: false)
        /// </summary>
        /// <param name = "autoEncode">Boolean indicating if autoencode is used</param>
        public Grid SetAutoEncode(bool autoEncode)
        {
            _autoEncode = autoEncode;
            return this;
        }

        /// <summary>
        ///     When set to true, the grid width is recalculated automatically to the width of the
        ///     parent element. This is done only initially when the grid is created. In order to
        ///     resize the grid when the parent element changes width you should apply custom code
        ///     and use a setGridWidth method for this purpose. (default: false)
        ///     自适应宽度，仅仅是创建的时候，其他时候需要自己写代码
        /// </summary>
        /// <param name = "autoWidth">Boolean indicating if autowidth is used</param>
        public Grid SetAutoWidth(bool autoWidth)
        {
            _autoWidth = autoWidth;
            return this;
        }

        /// <summary>
        ///     Defines the caption layer for the grid. This caption appears above the header layer.
        ///     If the string is empty the caption does not appear. (default: empty)
        ///     设置标题，不设置的时候不显示
        /// </summary>
        /// <param name = "caption">Caption of grid</param>
        public Grid SetCaption(string caption)
        {
            _caption = caption;
            return this;
        }

        /// <summary>
        ///     Defines what type of information to expect to represent data in the grid. Valid
        ///     options are json (default) and xml
        ///     从服务器端返回的数据类型，默认xml。可选类型：xml，local，json，jsonnp，script，xmlstring，jsonstring，clientside
        /// </summary>
        /// <param name = "dataType">Data type</param>
        public Grid SetDataType(DataType dataType)
        {
            _dataType = dataType;
            return this;
        }

        /// <summary>
        ///     Displayed when the returned (or the current) number of records is zero.
        ///     This option is valid only if viewrecords option is set to true. (default value is
        ///     set in language file)
        ///     当返回的数据行数为0时显示的信息。只有当属性 viewrecords 设置为ture时起作用
        /// </summary>
        /// <param name = "emptyRecords">Display string</param>
        public Grid SetEmptyRecords(string emptyRecords)
        {
            _emptyRecords = emptyRecords;
            return this;
        }

        /// <summary>
        ///     If set to true this will place a footer table with one row below the grid records
        ///     and above the pager. The number of columns equal to the number of columns in the colModel
        ///     (default: false)
        ///     分页工具条上方，用于统计行
        /// </summary>
        /// <param name = "footerRow">Boolean indicating whether footerrow is displayed</param>
        public Grid SetFooterRow(bool footerRow)
        {
            _footerRow = footerRow;
            return this;
        }

        /// <summary>
        ///     When set to true we directly place the user data array userData in the footer. 
        ///     The rules are as follows: If the userData array contains a name which matches any name defined in colModel, 
        ///     then the value is placed in that column. If there are no such values nothing is placed. 
        ///     Note that if this option is used we use the current formatter options (if available) for that column.
        ///     (default: false)
        ///     当为true时把userData放到底部，用法：如果userData的值与colModel的值相同，那么此列就显示正确的值，如果不等那么此列就为空
        /// </summary>
        /// <param name = "userDataOnFooter">Boolean indicating whether user data is set on footer row</param>
        public Grid SetUserDataOnFooter(bool userDataOnFooter)
        {
            _userDataOnFooter = userDataOnFooter;
            return this;
        }

        /// <summary>
        ///     If set to true, when resizing the width of a column, the adjacent column (to the right)
        ///     will resize so that the overall grid width is maintained (e.g., reducing the width of
        ///     column 2 by 30px will increase the size of column 3 by 30px).
        ///     In this case there is no horizontal scrolbar.
        ///     Warning: this option is not compatible with shrinkToFit option - i.e if
        ///     shrinkToFit is set to false, forceFit is ignored.
        ///     当为ture时，调整列宽度不会改变表格的宽度。当shrinkToFit 为false时，此属性会被忽略
        /// </summary>
        /// <param name = "forceFit">Boolean indicating if forcefit is enforced</param>
        public Grid SetForceFit(bool forceFit)
        {
            _forceFit = forceFit;
            return this;
        }

        /// <summary>
        ///     In the previous versions of jqGrid including 3.4.X,reading relatively big data sets
        ///     (Rows >=100 ) caused speed problems. The reason for this was that as every cell was
        ///     inserted into the grid we applied about 5-6 jQuery calls to it. Now this problem has
        ///     been resolved; we now insert the entry row at once with a jQuery append. The result is
        ///     impressive - about 3-5 times faster. What will be the result if we insert all the
        ///     data at once? Yes, this can be done with help of the gridview option. When set to true,
        ///     the result is a grid that is 5 to 10 times faster. Of course when this option is set
        ///     to true we have some limitations. If set to true we can not use treeGrid, subGrid,
        ///     or afterInsertRow event. If you do not use these three options in the grid you can
        ///     set this option to true and enjoy the speed. (default: false)
        ///     如果设置为true，我们不能用的TreeGrid，子网格，
        ///     或afterInsertRow事件。如果你没有在网格中使用这三个选项，你可以
        ///     将此选项设置为true，享受速度。 （缺省值：false）
        ///     构造一行数据后添加到grid中，如果设为true则是将整个表格的数据都构造完成后再添加到grid中，但treeGrid, subGrid, or afterInsertRow 不能用
        /// </summary>
        /// <param name = "gridView">Boolean indicating gridview is enabled</param>
        public Grid SetGridView(bool gridView)
        {
            _gridView = gridView;
            return this;
        }

        /// <summary>
        ///     If the option is set to true the title attribute is added to the column headers (default: false)
        ///     
        /// </summary>
        /// <param name = "headerTitles">Boolean indicating if headertitles are enabled</param>
        public Grid SetHeaderTitles(bool headerTitles)
        {
            _headerTitles = headerTitles;
            return this;
        }

        /// <summary>
        ///     The height of the grid in pixels (default: 100%, which is the only acceptable percentage for jqGrid)
        ///     表格高度，可以是数字，像素值或者百分比
        /// </summary>
        /// <param name = "height">Height in pixels</param>
        public Grid SetHeight(int height)
        {
            _height = height;
            return this;
        }

        /// <summary>
        ///     If set to true the grid initially is hidden. The data is not loaded (no request is sent) and only the
        ///     caption layer is shown. When the show/hide button is clicked for the first time to show the grid, the request
        ///     is sent to the server, the data is loaded, and the grid is shown. From this point on we have a regular grid.
        ///     This option has effect only if the caption property is not empty. (default: false)
        ///     当为ture时，表格不会被显示，只显示表格的标题。只有当点击显示表格的那个按钮时才会去初始化表格数据。
        /// </summary>
        /// <param name = "hiddenGrid">Boolean indicating if hiddengrid is enforced</param>
        public Grid SetHiddenGrid(bool hiddenGrid)
        {
            _hiddenGrid = hiddenGrid;
            return this;
        }

        /// <summary>
        ///     Enables or disables the show/hide grid button, which appears on the right side of the caption layer.
        ///     Takes effect only if the caption property is not an empty string. (default: true)
        ///     启用或者禁用控制表格显示、隐藏的按钮，只有当caption 属性不为空时起效
        /// </summary>
        /// <param name = "hideGrid">Boolean indicating if show/hide button is enabled</param>
        public Grid SetHideGrid(bool hideGrid)
        {
            _hideGrid = hideGrid;
            return this;
        }

        /// <summary>
        ///     When set to false the mouse hovering is disabled in the grid data rows. (default: true)
        ///     当为false时mouse hovering会被禁用
        /// </summary>
        /// <param name = "hoverRows">Indicates whether hoverrows is enabled</param>
        public Grid SetHoverRows(bool hoverRows)
        {
            _hoverRows = hoverRows;
            return this;
        }

        /// <summary>
        ///     If this flag is set to true, the grid loads the data from the server only once (using the
        ///     appropriate datatype). After the first request the datatype parameter is automatically
        ///     changed to local and all further manipulations are done on the client side. The functions
        ///     of the pager (if present) are disabled. (default: false)
        ///     只加载一次，设置为true，刷新将不起作用
        /// </summary>
        /// <param name = "loadOnce">Boolean indicating if loadonce is enforced</param>
        public Grid SetLoadOnce(bool loadOnce)
        {
            _loadOnce = loadOnce;
            return this;
        }

        /// <summary>
        ///     The text which appears when requesting and sorting data. This parameter override the value located
        ///     in the language file
        ///     加载时显示的信息
        /// </summary>
        /// <param name = "loadText">Loadtext</param>
        public Grid SetLoadText(string loadText)
        {
            _loadText = loadText;
            return this;
        }

        /// <summary>
        ///     This option controls what to do when an ajax operation is in progress.
        ///     'disable' - disables the jqGrid progress indicator. This way you can use your own indicator.
        ///     'enable' (default) - enables “Loading” message in the center of the grid.
        ///     'block' - enables the “Loading” message and blocks all actions in the grid until the ajax request
        ///     is finished. Be aware that this disables paging, sorting and all actions on toolbar, if any.
        /// </summary>
        /// <param name = "loadUi">Load ui mode</param>
        public Grid SetLoadUi(LoadUi loadUi)
        {
            _loadUi = loadUi;
            return this;
        }

        /// <summary>
        ///     This parameter makes sense only when multiselect option is set to true.
        ///     Defines the key which will be pressed
        ///     when we make a multiselection. The possible values are:
        ///     'shiftKey' - the user should press Shift Key
        ///     'altKey' - the user should press Alt Key
        ///     'ctrlKey' - the user should press Ctrl Key
        ///     设置多选快捷键
        /// </summary>
        /// <param name = "multiKey">Key to multiselect</param>
        public Grid SetMultiKey(MultiKey multiKey)
        {
            _multiKey = multiKey;
            return this;
        }

        /// <summary>
        ///     This option works only when multiselect = true. When multiselect is set to true, clicking anywhere
        ///     on a row selects that row; when multiboxonly is also set to true, the multiselection is done only
        ///     when the checkbox is clicked (Yahoo style). Clicking in any other row (suppose the checkbox is
        ///     not clicked) deselects all rows and the current row is selected. (default: false)
        ///     仅仅点击checkbox选中
        /// </summary>
        /// <param name = "multiBoxOnly">Boolean indicating if multiboxonly is enforced</param>
        public Grid SetMultiBoxOnly(bool multiBoxOnly)
        {
            _multiBoxOnly = multiBoxOnly;
            return this;
        }

        /// <summary>
        ///     If this flag is set to true a multi selection of rows is enabled. A new column
        ///     at the left side is added. Can be used with any datatype option. (default: false)
        ///     设置多选
        /// </summary>
        /// <param name = "multiSelect">Boolean indicating if multiselect is enabled</param>
        public Grid SetMultiSelect(bool multiSelect)
        {
            _multiSelect = multiSelect;
            return this;
        }
        /// <summary>
        /// 行编辑模式dialog，inline
        /// 仅仅用于简单类型字段
        /// </summary>
        /// <param name="editModel"></param>
        /// <returns></returns>
        public Grid SetEditRowModel(EditRowModel editModel)
        {
            _editRowModel = editModel;
            return this;
        }
        /// <summary>
        ///     Determines the width of the multiselect column if multiselect is set to true. (default: 20)
        ///     多选列的宽度，默认20
        /// </summary>
        /// <param name = "multiSelectWidth"></param>
        public Grid SetMultiSelectWidth(int multiSelectWidth)
        {
            _multiSelectWidth = multiSelectWidth;
            return this;
        }

        /// <summary>
        ///     Set the initial number of selected page when we make the request.This parameter is passed to the url
        ///     for use by the server routine retrieving the data (default: 1)
        ///     初始化的页数，默认初始化第一页
        /// </summary>
        /// <param name = "page">Number of page</param>
        public Grid SetPage(int page)
        {
            _page = page;
            return this;
        }

        /// <summary>
        ///     If pagername is specified a pagerelement is automatically added to the grid
        ///     设置分页div的id，显示分页
        /// </summary>
        /// <param name = "pager">Id/name of pager</param>
        public Grid SetPager(string pager)
        {
            _pager = pager;
            return this;
        }

        /// <summary>
        ///     Determines the position of the pager in the grid. By default the pager element
        ///     when created is divided in 3 parts (one part for pager, one part for navigator
        ///     buttons and one part for record information) (default: center)
        ///     分页的位置，默认是中间
        /// </summary>
        /// <param name = "pagerPos">Position of pager</param>
        public Grid SetPagerPos(PagerPos pagerPos)
        {
            _pagerPos = pagerPos;
            return this;
        }

        /// <summary>
        ///     Determines if the pager buttons should be displayed if pager is available. Valid
        ///     only if pager is set correctly. The buttons are placed in the pager bar. (default: true)
        ///     显示翻页按钮 默认true
        /// </summary>
        /// <param name = "pgButtons">Boolean indicating if pager buttons are displayed</param>
        public Grid SetPgButtons(bool pgButtons)
        {
            _pgButtons = pgButtons;
            return this;
        }

        /// <summary>
        ///     Determines if the input box, where the user can change the number of the requested page,
        ///     should be available. The input box appears in the pager bar. (default: true)
        ///     是否显示跳转页面的输入框
        /// </summary>
        /// <param name = "pgInput">Boolean indicating if pager input is available</param>
        public Grid SetPgInput(bool pgInput)
        {
            _pgInput = pgInput;
            return this;
        }

        /// <summary>
        ///     Show information about current page status. The first value is the current loaded page.
        ///     The second value is the total number of pages (default is set in language file)
        ///     Example: "Page {0} of {1}"
        ///     翻页文字描述Example: "Page {0} of {1}"
        /// </summary>
        /// <param name = "pgText">Current page status text</param>
        public Grid SetPgText(string pgText)
        {
            _pgText = pgText;
            return this;
        }

        /// <summary>
        ///     Determines the position of the record information in the pager. Can be left, center, right
        ///     (default: right)
        ///     Warning: When pagerpos en recordpos are equally set, pager is hidden.
        ///     记录数的位置
        /// </summary>
        /// <param name = "recordPos">Position of record information</param>
        public Grid SetRecordPos(RecordPos recordPos)
        {
            _recordPos = recordPos;
            return this;
        }

        /// <summary>
        ///     Represent information that can be shown in the pager. This option is valid if viewrecords
        ///     option is set to true. This text appears only if the total number of records is greater then
        ///     zero.
        ///     In order to show or hide information the items between {} mean the following: {0} the
        ///     start position of the records depending on page number and number of requested records;
        ///     {1} - the end position {2} - total records returned from the data (default defined in language file)
        ///     记录数的文字描述
        /// </summary>
        /// <param name = "recordText">Record Text</param>
        public Grid SetRecordText(string recordText)
        {
            _recordText = recordText;
            return this;
        }

        /// <summary>
        ///     Defines the type of request to make (“POST” or “GET”) (default: GET)
        ///     设置请求类型，默认post
        /// </summary>
        /// <param name = "requestType">Request type</param>
        public Grid SetRequestType(RequestType requestType)
        {
            _requestType = requestType;
            return this;
        }

        /// <summary>
        ///     Assigns a class to columns that are resizable so that we can show a resize
        ///     handle (default: empty string)
        ///     设置根据什么class变化大小
        /// </summary>
        /// <param name = "resizeClass"></param>
        /// <returns></returns>
        public Grid SetResizeClass(string resizeClass)
        {
            _resizeClass = resizeClass;
            return this;
        }

        /// <summary>
        ///     An array to construct a select box element in the pager in which we can change the number
        ///     of the visible rows. When changed during the execution, this parameter replaces the rowNum
        ///     parameter that is passed to the url. If the array is empty the element does not appear
        ///     in the pager. Typical you can set this like [10,20,30]. If the rowNum parameter is set to
        ///     30 then the selected value in the select box is 30.
        ///     设置按多少条记录分页
        /// </summary>
        /// <example>
        ///     setRowList(new int[]{10,20,50})
        /// </example>
        /// <param name = "rowList">List of rows per page</param>
        public Grid SetRowList(int[] rowList)
        {
            _rowList = rowList;
            return this;
        }

        /// <summary>
        ///     Sets how many records we want to view in the grid. This parameter is passed to the url
        ///     for use by the server routine retrieving the data. Be aware that if you set this parameter
        ///     to 10 (i.e. retrieve 10 records) and your server return 15 then only 10 records will be
        ///     loaded. Set this parameter to -1 (unlimited) to disable this checking. (default: 20)
        ///     设置每页多少行
        /// </summary>
        /// <param name = "rowNum">Number of rows per page</param>
        public Grid SetRowNum(int rowNum)
        {
            _rowNum = rowNum;
            return this;
        }

        /// <summary>
        ///     If this option is set to true, a new column at the leftside of the grid is added. The purpose of
        ///     this column is to count the number of available rows, beginning from 1. In this case
        ///     colModel is extended automatically with a new element with name - 'rn'. Also, be careful
        ///     not to use the name 'rn' in colModel
        ///     设置显示行号
        /// </summary>
        /// <param name = "rowNumbers">Boolean indicating if rownumbers are enabled</param>
        public Grid SetRowNumbers(bool rowNumbers)
        {
            _rowNumbers = rowNumbers;
            return this;
        }

        /// <summary>
        ///     Determines the width of the row number column if rownumbers option is set to true. (default: 25)
        /// </summary>
        /// <param name = "rowNumWidth">Width of rownumbers column</param>
        public Grid SetRowNumWidth(int rowNumWidth)
        {
            _rowNumWidth = rowNumWidth;
            return this;
        }
        /// <summary>
        /// 设置子面板加载js函数名
        /// 
        /// </summary>
        /// <param name="subGridRowExpanded"></param>
        /// <returns></returns>
        public Grid SetSubGridRowExpanded(string subGridRowExpanded)
        {
            //function showChildGrid(parentRowID, parentRowKey) {
            //    $.ajax({
            //        url: parentRowKey+".html",
            //        type: "GET",
            //        success: function (html) {
            //            $("#" + parentRowID).append(html);
            //        }
            //    });

            //}
            _subGrid = true;
            _subGridRowExpanded = subGridRowExpanded;
            _gridView = false;
            return this;
        }
        /// <summary>
        /// 设置列头分组
        /// </summary>
        /// <param name="groupHeaders"></param>
        /// <returns></returns>
        public Grid SetGroupHeaders(IEnumerable<GroupHeader> groupHeaders)
        {
            if (groupHeaders != null && groupHeaders.Any())
            {
                _groupHeaders = groupHeaders;
            }
            return this;
        }
        /// <summary>
        ///     Creates dynamic scrolling grids. When enabled, the pager elements are disabled and we can use the
        ///     vertical scrollbar to load data. When set to true the grid will always hold all the items from the
        ///     start through to the latest point ever visited.
        ///     When scroll is set to an integer value (eg 1), the grid will just hold the visible lines. This allow us to
        ///     load the data at portions whitout to care about the memory leaks. (default: false)
        /// </summary>
        /// <param name = "scroll">Boolean indicating if scroll is enforced</param>
        [Obsolete("Method is obsolete, use SetVirtualScroll instead")]
        public Grid SetScroll(bool scroll)
        {
            _scroll = scroll;
            return this;
        }

        /// <summary>
        ///     Creates dynamic scrolling grids. When enabled, the pager elements are disabled and we can use the
        ///     vertical scrollbar to load data. When set to true the grid will always hold all the items from the
        ///     start through to the latest point ever visited.
        ///     When scroll is set to an integer value (eg 1), the grid will just hold the visible lines. This allow us to
        ///     load the data at portions whitout to care about the memory leaks. (default: false)
        /// </summary>
        /// <param name = "scroll">When integer value is set (eg 1) scroll is enforced</param>
        [Obsolete("Method is obsolete, use SetVirtualScroll instead")]
        public Grid SetScroll(int scroll)
        {
            _scrollInt = scroll;
            return this;
        }

        /// <summary>
        ///     Determines how to post the data on which we perform searching. 
        ///     When the this option is false the posted data is in key:value pair, if the option is true, the posted data is equal on those as in searchGrid method.
        ///     See here: http://www.trirand.com/jqgridwiki/doku.php?id=wiki:advanced_searching#options
        ///     (default: true)
        /// </summary>
        /// <param name = "stringResult">Boolean indicating if</param>        
        public Grid SetStringResult(bool stringResult)
        {
            _stringResult = stringResult;
            return this;
        }

        /// <summary>
        /// Creates virtual scrolling grid. When enabled, the pager elements are disabled and we can use the vertical scrollbar to load data.
        /// </summary>
        /// <param name="virtualScroll">Enables virtual scrolling when set to true</param>
        /// <param name="justHoldVisibleLines">Boolean indicating if only the visible lines in the grid should be held in memory (prevents memory leaks)</param>
        /// <returns></returns>
        public Grid SetVirtualScroll(bool virtualScroll, bool justHoldVisibleLines = true)
        {
            if (virtualScroll && justHoldVisibleLines)
            {
                _scrollInt = 1;
            }
            else if (!virtualScroll)
            {
                _scroll = false;
            }
            else
            {
                _scroll = true;
            }

            return this;
        }

        /// <summary>
        ///     Determines the width of the vertical scrollbar. Since different browsers interpret this width
        ///     differently (and it is difficult to calculate it in all browsers) this can be changed. (default: 18)
        ///     设置垂直滚动条宽度
        /// </summary>
        /// <param name = "scrollOffset">Scroll offset</param>
        public Grid SetScrollOffset(int scrollOffset)
        {
            _scrollOffset = scrollOffset;
            return this;
        }

        /// <summary>
        ///     When enabled, selecting a row with setSelection scrolls the grid so that the selected row is visible.
        ///     This is especially useful when we have a verticall scrolling grid and we use form editing with
        ///     navigation buttons (next or previous row). On navigating to a hidden row, the grid scrolls so the
        ///     selected row becomes visible. (default: false)
        /// </summary>
        /// <param name = "scrollRows">Boolean indicating if scrollrows is enabled</param>
        public Grid SetScrollRows(bool scrollRows)
        {
            _scrollRows = scrollRows;
            return this;
        }

        /// <summary>
        ///     This controls the timeout handler when scroll is set to 1. (default: 200 milliseconds)
        /// </summary>
        /// <param name = "scrollTimeout">Scroll timeout in milliseconds</param>
        /// <returns></returns>
        public Grid SetScrollTimeout(int scrollTimeout)
        {
            _scrollTimeout = scrollTimeout;
            return this;
        }

        /// <summary>
        ///     This option describes the type of calculation of the initial width of each column
        ///     against the width of the grid. If the value is true and the value in width option
        ///     is set then: Every column width is scaled according to the defined option width.
        ///     Example: if we define two columns with a width of 80 and 120 pixels, but want the
        ///     grid to have a 300 pixels - then the columns are recalculated as follow:
        ///     1- column = 300(new width)/200(sum of all width)*80(column width) = 120 and 2 column = 300/200*120 = 180.
        ///     The grid width is 300px. If the value is false and the value in width option is set then:
        ///     The width of the grid is the width set in option.
        ///     The column width are not recalculated and have the values defined in colModel. (default: true)
        /// </summary>
        /// <param name = "shrinkToFit">Boolean indicating if shrink to fit is enforced</param>
        public Grid SetShrinkToFit(bool shrinkToFit)
        {
            _shrinkToFit = shrinkToFit;
            return this;
        }

        /// <summary>
        ///     Determines how the search should be applied. If this option is set to true search is started when
        ///     the user hits the enter key. If the option is false then the search is performed immediately after
        ///     the user presses some character. (default: true
        /// </summary>
        /// <param name = "searchOnEnter">Indicates if search is started on enter</param>
        public Grid SetSearchOnEnter(bool searchOnEnter)
        {
            _searchOnEnter = searchOnEnter;
            return this;
        }

        /// <summary>
        ///     Enables toolbar searching / filtering
        /// </summary>
        /// <param name = "searchToolbar">Indicates if toolbar searching is enabled</param>
        public Grid SetSearchToolbar(bool searchToolbar)
        {
            _searchToolbar = searchToolbar;
            return this;
        }

        /// <summary>
        ///     When set to true adds clear button to clear all search entries (default: false)
        /// </summary>
        /// <param name = "searchClearButton"></param>
        /// <returns></returns>
        public Grid SetSearchClearButton(bool searchClearButton)
        {
            _searchClearButton = searchClearButton;
            return this;
        }

        /// <summary>
        ///     When set to true adds toggle button to toggle search toolbar (default: false)
        /// </summary>
        /// <param name = "searchToggleButton">Indicates if toggle button is displayed</param>
        public Grid SetSearchToggleButton(bool searchToggleButton)
        {
            _searchToggleButton = searchToggleButton;
            return this;
        }


        /// <summary>
        ///     If enabled all sort icons are visible for all columns which are sortable (default: false)
        /// </summary>
        /// <param name = "showAllSortIcons">Boolean indicating if all sorting icons should be displayed</param>
        public Grid SetShowAllSortIcons(bool showAllSortIcons)
        {
            _showAllSortIcons = showAllSortIcons;
            return this;
        }

        /// <summary>
        ///     Sets direction in which sort icons are displayed (default: vertical)
        /// </summary>
        /// <param name = "sortIconDirection">Direction in which sort icons are displayed</param>
        public Grid SetSortIconDirection(Direction sortIconDirection)
        {
            _sortIconDirection = sortIconDirection;
            return this;
        }

        /// <summary>
        ///     If enabled columns are sorted when header is clicked (default: true)
        ///     Warning, if disabled and setShowAllSortIcons is set to false, sorting will
        ///     be effectively disabled
        /// </summary>
        /// <param name = "sortOnHeaderClick">Boolean indicating if columns will sort on headerclick</param>
        /// <returns></returns>
        public Grid SetSortOnHeaderClick(bool sortOnHeaderClick)
        {
            _sortOnHeaderClick = sortOnHeaderClick;
            return this;
        }
        /// <summary>
        /// 设置列的action，默认true
        /// </summary>
        /// <param name="colMenu"></param>
        /// <returns></returns>
        public Grid SetColMenu(bool colMenu)
        {
            _colMenu = colMenu;
            return this;
        }
        /// <summary>
        ///     The initial sorting name when we use datatypes xml or json (data returned from server).
        ///     This parameter is added to the url. If set and the index (name) matches the name from the
        ///     colModel then by default an image sorting icon is added to the column, according to
        ///     the parameter sortorder.
        /// </summary>
        /// <param name = "sortName"></param>
        public Grid SetSortName(string sortName)
        {
            _sortName = sortName;
            return this;
        }

        /// <summary>
        ///     The initial sorting order when we use datatypes xml or json (data returned from server).
        ///     This parameter is added to the url. Two possible values - asc or desc. (default: asc)
        /// </summary>
        /// <param name = "sortOrder">Sortorder</param>
        public Grid SetSortOrder(SortOrder sortOrder)
        {
            _sortOrder = sortOrder;
            return this;
        }

        /// <summary>
        ///     This option enabled the toolbar of the grid.  When we have two toolbars (can be set using setToolbarPosition)
        ///     then two elements (div) are automatically created. The id of the top bar is constructed like
        ///     “t_”+id of the grid and the bottom toolbar the id is “tb_”+id of the grid. In case when
        ///     only one toolbar is created we have the id as “t_” + id of the grid, independent of where
        ///     this toolbar is created (top or bottom). You can use jquery to add elements to the toolbars.
        /// </summary>
        /// <param name = "toolbar">Boolean indicating if toolbar is enabled</param>
        public Grid SetToolbar(bool toolbar)
        {
            _toolbar = toolbar;
            return this;
        }

        /// <summary>
        ///     Sets toolbarposition (default: top)
        /// </summary>
        /// <param name = "toolbarPosition">Position of toolbar</param>
        public Grid SetToolbarPosition(ToolbarPosition toolbarPosition)
        {
            _toolbarPosition = toolbarPosition;
            return this;
        }

        /// <summary>
        ///     When enabled this option place a pager element at top of the grid below the caption
        ///     (if available). If another pager is defined both can coexists and are refreshed in sync.
        ///     (default: false)
        /// </summary>
        /// <param name = "topPager">Boolean indicating if toppager is enabled</param>
        public Grid SetTopPager(bool topPager)
        {
            _topPager = topPager;
            return this;
        }

        /// <summary>
        ///     The url of the file that holds the request
        ///     可以不设置，默认为 Api/Core/datalist
        /// </summary>
        /// <param name = "url">Data url</param>
        public Grid SetUrl(string url)
        {
            _url = url;
            return this;
        }
        /// <summary>
        /// 持久化url，可以不设置，
        /// 默认值为：Api/Core/Persistence
        /// </summary>
        /// <param name="editUrl"></param>
        /// <returns></returns>
        public Grid SetEditUrl(string editUrl)
        {
            _editUrl = editUrl;
            return this;
        }
        /// <summary>
        /// 设置增改查表单生成方式，jqgrid默认，元数据生成，无
        /// </summary>
        /// <param name="formType"></param>
        /// <returns></returns>
        public Grid SetFormType(OperEnum formType)
        {
            _dataFormType = formType;
            return this;
        }

        /// <summary>
        ///     If true, jqGrid displays the beginning and ending record number in the grid,
        ///     out of the total number of records in the query.
        ///     This information is shown in the pager bar (bottom right by default)in this format:
        ///     “View X to Y out of Z”.
        ///     If this value is true, there are other parameters that can be adjusted,
        ///     including 'emptyrecords' and 'recordtext'. (default: false)
        /// </summary>
        /// <param name = "viewRecords">Boolean indicating if recordnumbers are shown in grid</param>
        public Grid SetViewRecords(bool viewRecords)
        {
            _viewRecords = viewRecords;
            return this;
        }

        /// <summary>
        ///     If this option is not set, the width of the grid is a sum of the widths of the columns
        ///     defined in the colModel (in pixels). If this option is set, the initial width of each
        ///     column is set according to the value of shrinkToFit option.
        /// </summary>
        /// <param name = "width">Width in pixels</param>
        public Grid SetWidth(int width)
        {
            _width = width;
            return this;
        }

        /// <summary>
        /// Set to true when page is being added to document asyncronously,
        /// prevents javascript from being wrapped in $(document).ready()
        /// </summary>
        /// <param name="asyncPageLoad"></param>
        /// <returns></returns>
        public Grid SetAsyncLoad(bool asyncPageLoad)
        {
            _asyncLoad = asyncPageLoad;
            return this;
        }

        /// <summary>
        /// Set to true when filtering grid loaded with SetLoadOnce(true)
        /// to filter the data case insesitive
        /// </summary>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public Grid SetIgnoreCase(bool ignoreCase)
        {
            _ignoreCase = ignoreCase;
            return this;
        }

        /// <summary>
        ///     This event fires after each inserted row.
        ///     Variables available in call:
        ///     'rowid': Id of the inserted row
        ///     'rowdata': An array of the data to be inserted into the row. This is array of type name-
        ///     value, where the name is a name from colModel
        ///     'rowelem': The element from the response. If the data is xml this is the xml element of the row;
        ///     if the data is json this is array containing all the data for the row
        ///     Warning: this event does not fire if gridview option is set to true
        /// </summary>
        /// <param name = "onAfterInsertRow">Script to be executed</param>
        public Grid OnAfterInsertRow(string onAfterInsertRow)
        {
            _onAfterInsertRow = onAfterInsertRow;
            return this;
        }
        /// <summary>
        /// 新增 表单初始化后事件，用于表单赋初值
        /// </summary>
        /// <param name="onAfterInitDataForm"></param>
        /// <returns></returns>
        public Grid OnAddAfterInitDataForm(string onAddAfterInitDataForm)
        {
            _onAddAfterInitDataForm = onAddAfterInitDataForm;
            return this;
        }
        public Grid OnEditAfterInitDataForm(string onEditAfterInitDataForm)
        {
            _onEditAfterInitDataForm = onEditAfterInitDataForm;
            return this;
        }
        /// <summary>
        ///     This event fires before requesting any data. Does not fire if datatype is function
        ///     Variables available in call: None
        /// </summary>
        /// <param name = "onBeforeRequest">Script to be executed</param>
        public Grid OnBeforeRequest(string onBeforeRequest)
        {
            _onBeforeRequest = onBeforeRequest;
            return this;
        }

        /// <summary>
        ///     This event fires when the user clicks on the row, but before selecting it.
        ///     Variables available in call:
        ///     'rowid': The id of the row.
        ///     'e': The event object
        ///     This event should return boolean true or false. If the event returns true the selection
        ///     is done. If the event returns false the row is not selected and any other action if defined
        ///     does not occur.
        /// </summary>
        /// <param name = "onBeforeSelectRow">Script to be executed</param>
        public Grid OnBeforeSelectRow(string onBeforeSelectRow)
        {
            _onBeforeSelectRow = onBeforeSelectRow;
            return this;
        }

        /// <summary>
        ///     This fires after all the data is loaded into the grid and all the other processes are complete.
        ///     Also the event fires independent from the datatype parameter and after sorting paging and etc.
        ///     Variables available in call: None
        /// </summary>
        /// <param name = "onGridComplete">Script to be executed</param>
        public Grid OnGridComplete(string onGridComplete)
        {
            _onGridComplete = onGridComplete;
            return this;
        }

        /// <summary>
        ///     A pre-callback to modify the XMLHttpRequest object (xhr) before it is sent. Use this to set
        ///     custom headers etc. The XMLHttpRequest is passed as the only argument.
        ///     Variables available in call:
        ///     'xhr': The XMLHttpRequest
        /// </summary>
        /// <param name = "onLoadBeforeSend">Script to be executed</param>
        public Grid OnLoadBeforeSend(string onLoadBeforeSend)
        {
            _onLoadBeforeSend = onLoadBeforeSend;
            return this;
        }

        /// <summary>
        ///     This event is executed immediately after every server request.
        ///     Variables available in call:
        ///     'xhr': The XMLHttpRequest
        /// </summary>
        /// <param name = "onLoadComplete">Script to be executed</param>
        public Grid OnLoadComplete(string onLoadComplete)
        {
            _onLoadComplete = onLoadComplete;
            return this;
        }

        /// <summary>
        ///     A function to be called if the request fails.
        ///     Variables available in call:
        ///     'xhr': The XMLHttpRequest
        ///     'status': String describing the type of error
        ///     'error': Optional exception object, if one occurred
        /// </summary>
        /// <param name = "onLoadError">Script to be executed</param>
        public Grid OnLoadError(string onLoadError)
        {
            _onLoadError = onLoadError;
            return this;
        }

        /// <summary>
        ///     Fires when we click on a particular cell in the grid.
        ///     Variables available in call:
        ///     'rowid': The id of the row
        ///     'iCol': The index of the cell,
        ///     'cellcontent': The content of the cell,
        ///     'e': The event object element where we click.
        ///     (Be aware that this available when we are not using cell editing module
        ///     and is disabled when using cell editing).
        /// </summary>
        /// <param name = "onCellSelect">Script to be executed</param>
        public Grid OnCellSelect(string onCellSelect)
        {
            _onCellSelect = onCellSelect;
            return this;
        }

        /// <summary>
        ///     Raised immediately after row was double clicked.
        ///     Variables available in call:
        ///     'rowid': The id of the row,
        ///     'iRow': The index of the row (do not mix this with the rowid),
        ///     'iCol': The index of the cell.
        ///     'e': The event object
        /// </summary>
        /// <param name = "onDblClickRow">Script to be executed</param>
        public Grid OnDblClickRow(string onDblClickRow)
        {
            _onDblClickRow = onDblClickRow;
            return this;
        }

        /// <summary>
        ///     Fires after clicking hide or show grid (hidegrid:true)
        ///     Variables available in call:
        ///     'gridstate': The state of the grid - can have two values - visible or hidden
        /// </summary>
        /// <param name = "onHeaderClick">Script to be executed</param>
        public Grid OnHeaderClick(string onHeaderClick)
        {
            _onHeaderClick = onHeaderClick;
            return this;
        }

        /// <summary>
        ///     This event fires after click on [page button] and before populating the data.
        ///     Also works when the user enters a new page number in the page input box
        ///     (and presses [Enter]) and when the number of requested records is changed via
        ///     the select box.
        ///     If this event returns 'stop' the processing is stopped and you can define your
        ///     own custom paging
        ///     Variables available in call:
        ///     'pgButton': first,last,prev,next in case of button click, records in case when
        ///     a number of requested rows is changed and user when the user change the number of the requested page
        /// </summary>
        /// <param name = "onPaging">Script to be executed</param>
        public Grid OnPaging(string onPaging)
        {
            _onPaging = onPaging;
            return this;
        }

        /// <summary>
        ///     Raised immediately after row was right clicked.
        ///     Variables available in call:
        ///     'rowid': The id of the row,
        ///     'iRow': The index of the row (do not mix this with the rowid),
        ///     'iCol': The index of the cell.
        ///     'e': The event object
        ///     Warning - this event does not work in Opera browsers, since Opera does not support oncontextmenu event
        /// </summary>
        /// <param name = "onRightClickRow">Script to be executed</param>
        public Grid OnRightClickRow(string onRightClickRow)
        {
            _onRightClickRow = onRightClickRow;
            return this;
        }

        /// <summary>
        ///     This event fires when multiselect option is true and you click on the header checkbox.
        ///     Variables available in call:
        ///     'aRowids':  Array of the selected rows (rowid's).
        ///     'status': Boolean variable determining the status of the header check box - true if checked, false if not checked.
        ///     Be awareS that the aRowids alway contain the ids when header checkbox is checked or unchecked.
        /// </summary>
        /// <param name = "onSelectAll">Script to be executed</param>
        public Grid OnSelectAll(string onSelectAll)
        {
            _onSelectAll = onSelectAll;
            return this;
        }

        /// <summary>
        ///     Raised immediately when row is clicked.
        ///     Variables available in function call:
        ///     'rowid': The id of the row,
        ///     'status': Tthe status of the selection. Can be used when multiselect is set to true.
        ///     true if the row is selected, false if the row is deselected.
        ///     <param name = "onSelectRow">Script to be executed</param>
        /// </summary>
        public Grid OnSelectRow(string onSelectRow)
        {
            _onSelectRow = onSelectRow;
            return this;
        }

        /// <summary>
        ///     Raised immediately after sortable column was clicked and before sorting the data.
        ///     Variables available in call:
        ///     'index': The index name from colModel
        ///     'iCol': The index of column,
        ///     'sortorder': The new sorting order - can be 'asc' or 'desc'.
        ///     If this event returns 'stop' the sort processing is stopped and you can define your own custom sorting
        /// </summary>
        /// <param name = "onSortCol">Script to be executed</param>
        public Grid OnSortCol(string onSortCol)
        {
            _onSortCol = onSortCol;
            return this;
        }

        /// <summary>
        ///     Event which is called when we start resizing a column.
        ///     Variables available in call:
        ///     'event':  The event object
        ///     'index': The index of the column in colModel.
        /// </summary>
        /// <param name = "onResizeStart">Script to be executed</param>
        public Grid OnResizeStart(string onResizeStart)
        {
            _onResizeStart = onResizeStart;
            return this;
        }

        /// <summary>
        ///     Event which is called after the column is resized.
        ///     Variables available in call:
        ///     'newwidth': The new width of the column
        ///     'index': The index of the column in colModel.
        /// </summary>
        /// <param name = "onResizeStop">Script to be executed</param>
        public Grid OnResizeStop(string onResizeStop)
        {
            _onResizeStop = onResizeStop;
            return this;
        }

        /// <summary>
        ///     If this event is set it can serialize the data passed to the ajax request.
        ///     The function should return the serialized data. This event can be used when
        ///     custom data should be passed to the server - e.g - JSON string, XML string and etc.
        ///     Variables available in call:
        ///     'postData': Posted data
        /// </summary>
        /// <param name = "onSerializeGridData">Script to be executed</param>
        public Grid OnSerializeGridData(string onSerializeGridData)
        {
            _onSerializeGridData = onSerializeGridData;
            return this;
        }

        /// <summary>
        /// JSON data is handled in a fashion very similar to that of xml data. What is important is that the definition of the jsonReader matches the data being received
        /// </summary>
        /// <param name="jsonReader"></param>
        /// <returns></returns>
        public Grid SetJsonReader(DataReaders.JsonReader jsonReader)
        {
            _jsonReader = jsonReader;
            return this;
        }

        public Grid EnableTreeGrid(TreeGridModel treeGridModel = TreeGridModel.Adjacency, int rootLevel = 0)
        {
            //_treeGridRootLevel = 0;
            _enabledTreeGrid = true;
            _treeGridModel = treeGridModel;

            return this;
        }

        public Grid SetRowAttr(string rowAttr)
        {
            _rowAttr = rowAttr;
            return this;
        }
        /// <summary>
        /// 设置树表 点击展开的 列名，点击此列 会展开
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public Grid SetExpandColumn(string columnName)
        {
            _expandColumn = columnName;
            return this;
        }
        public string RenderJavascript()
        {
            // Create javascript
            var script = new StringBuilder();

            // Start script
            if (_asyncLoad.HasValue && _asyncLoad.Value)
                script.AppendLine("jQuery(window).ready(function () {");
            else
                script.AppendLine("jQuery(document).ready(function () {");
            #region 设置jqgrid宽度大小以及动态调整宽度大小
            //设置jqgrid的宽度
            script.AppendLine(@"
                $(window).on('resize.jqGrid', function () {
                    var parent_width = $('###gridid##').closest('.##wrapper##').width();
                    $('###gridid##').jqGrid('setGridWidth', parent_width);
                });
                $(document).on('settings.ace.jqGrid', function (ev, event_name, collapsed) {
                    if (event_name === 'sidebar_collapsed' || event_name === 'main_container_fixed') {
                        var parent_width = $('###gridid##').closest('.##wrapper##').width();
                        //setTimeout的是WebKit的仅仅一次的DOM变化，然后重绘!!!
                        setTimeout(function () {
                            $('###gridid##').jqGrid('setGridWidth', parent_width);
                        }, 0);
                    }
                });
                //最大化时设置
                $('.widget-box').on('fullscreened.ace.widget', function (e) {
                    e.preventDefault();
                    var parent_width = $('###gridid##').closest('.##wrapper##').width();
                    $('###gridid##').jqGrid('setGridWidth', parent_width);
                    var offsetWidget = $('###gridid##').offset();
                    availableHeight = $(window).height() - offsetWidget.top;
                    $('###gridid##').setGridHeight(availableHeight - 55);
                });");
            //当为tabcontrol的时候的特殊处理
            if (_tabItemName.IsPresent())
            {
                //tabitem可见的时候设置and also set width when tab pane becomes visible                
                script.AppendLine(@"
                    $('###tabcontrol## a[data-toggle=\""tab\""]').on('shown.bs.tab', function (e) {
                        if ($(e.target).attr('href') === '###tabitem##') {
                            var parent_width = $('###gridid##').closest('.tab-pane').width();
                            $('###gridid##').jqGrid('setGridWidth', parent_width);
                        }
                    });");
                script.Replace("##tabcontrol##", _tabControlName).Replace("##tabitem##", _tabItemName);

            }
            #endregion

            script.AppendLine("jQuery('###gridid##').jqGrid({");

            // Make sure there is at most one key
            if (_columns.Count(r => r.IsKey) > 1)
            {
                throw new ArgumentException("Too many key columns added. Maximum allowed id 1.");
            }

            // Altrows
            if (_altRows.HasValue)
                script.AppendFormat("altRows:{0},", _altRows.ToString().ToLower()).AppendLine();

            // Altclass
            if (_altClass.IsPresent()) script.AppendFormat("altclass:'{0}',", _altClass).AppendLine();

            // Autoencode
            if (_autoEncode.HasValue)
                script.AppendFormat("autoencode:{0},", _autoEncode.ToString().ToLower()).AppendLine();

            // Autowidth
            if (_autoWidth.HasValue)
                script.AppendFormat("autowidth:{0},", _autoWidth.ToString().ToLower()).AppendLine();

            // Caption
            if (_caption.IsPresent()) script.AppendFormat("caption:'{0}',", _caption).AppendLine();

            // Datatype
            script.AppendLine(string.Format("datatype:'{0}',", _dataType.ToString().ToLower()));

            if (_dataType == DataType.Json && _jsonReader != null)
            {
                script.AppendLine("jsonReader:{'repeatitems':" + _jsonReader.RepeatItems.ToString().ToLower() + ", 'id': '" + _jsonReader.Id + "' },");
            }

            // Emptyrecords
            if (_emptyRecords.IsPresent())
                script.AppendFormat("emptyrecords:'{0}',", _emptyRecords).AppendLine();

            // FooterRow
            if (_footerRow.HasValue)
                script.AppendFormat("footerrow:{0},", _footerRow.ToString().ToLower()).AppendLine();

            // UserDataOnFooter
            if (_userDataOnFooter.HasValue)
                script.AppendFormat("userDataOnFooter:{0},", _footerRow.ToString().ToLower()).AppendLine();

            // Forcefit
            if (_forceFit.HasValue)
                script.AppendFormat("forceFit:{0},", _forceFit.ToString().ToLower()).AppendLine();

            // Gridview
            if (_gridView.HasValue)
                script.AppendFormat("gridview:{0},", _gridView.ToString().ToLower()).AppendLine();

            // HeaderTitles
            if (_headerTitles.HasValue)
                script.AppendFormat("headertitles:{0},", _headerTitles.ToString().ToLower()).AppendLine();

            // Height (set 100% if no value is specified except when scroll is set to true otherwise layout is not as it is supposed to be)
            if (!_height.HasValue)
            {
                if ((!_scroll.HasValue || _scroll == false) && !_scrollInt.HasValue)
                    script.AppendFormat("height:'{0}',", "100%").AppendLine();
            }
            else script.AppendFormat("height:{0},", _height).AppendLine();

            // Hiddengrid
            if (_hiddenGrid.HasValue)
                script.AppendFormat("hiddengrid:{0},", _hiddenGrid.ToString().ToLower()).AppendLine();

            // Hidegrid
            if (_hideGrid.HasValue)
                script.AppendFormat("hidegrid:{0},", _hideGrid.ToString().ToLower()).AppendLine();

            // HoverRows
            if (_hoverRows.HasValue)
                script.AppendFormat("hoverrows:{0},", _hoverRows.ToString().ToLower()).AppendLine();

            // Loadonce
            if (_loadOnce.HasValue)
                script.AppendFormat("loadonce:{0},", _loadOnce.ToString().ToLower()).AppendLine();

            // Loadtext
            if (!_loadText.IsMissing()) script.AppendFormat("loadtext:'{0}',", _loadText).AppendLine();

            // LoadUi
            if (_loadUi.HasValue) script.AppendFormat("loadui:'{0}',", _loadUi.ToString().ToLower()).AppendLine();

            // MultiBoxOnly
            if (_multiBoxOnly.HasValue)
                script.AppendFormat("multiboxonly:{0},", _multiBoxOnly.ToString().ToLower()).AppendLine();

            // MultiKey
            if (_multiKey.HasValue) script.AppendFormat("multikey:'{0}',", _multiKey.ToString().ToLower()).AppendLine();

            // MultiSelect
            if (_multiSelect.HasValue)
                script.AppendFormat("multiselect:{0},", _multiSelect.ToString().ToLower()).AppendLine();

            // MultiSelectWidth
            if (_multiSelectWidth.HasValue)
                script.AppendFormat("multiselectWidth:{0},", _multiSelectWidth).AppendLine();
            //PostData
            if (_postData.IsPresent())
            {
                script.AppendFormat("postData:{0},", _postData).AppendLine();
            }
            //subGrid, // set the subGrid property to true to show expand buttons for each row
            if (_subGrid) script.AppendLine("subGrid: true,");
            // subGridRowExpanded javascript function that will take care of showing the child grid
            if (!_subGridRowExpanded.IsMissing())
            {
                script.AppendFormat("subGridRowExpanded: {0},", _subGridRowExpanded).AppendLine();
                script.AppendLine(@" 
                subGridOptions : {
                    // configure the icons from theme rolloer
                    plusicon: 'fa fa-caret-down',
                    minusicon: 'fa fa-caret-up',
                    openicon: 'fa fa-hand-o-right',
                    reloadOnExpand :false,//展开不加载
                    selectOnExpand : true //选中加载
                },");
            }
            // Page
            if (_page.HasValue) script.AppendFormat("page:{0},", _page).AppendLine();

            // Pager
            if (_pager.IsPresent()) script.AppendFormat("pager:'#{0}',", _pager).AppendLine();

            // PagerPos
            if (_pagerPos.HasValue) script.AppendFormat("pagerpos:'{0}',", _pagerPos.ToString().ToLower()).AppendLine();

            // PgButtons
            if (_pgButtons.HasValue)
                script.AppendFormat("pgbuttons:{0},", _pgButtons.ToString().ToLower()).AppendLine();

            // PgInput
            if (_pgInput.HasValue)
                script.AppendFormat("pginput:{0},", _pgInput.ToString().ToLower()).AppendLine();

            // PGText
            if (_pgText.IsPresent()) script.AppendFormat("pgtext:'{0}',", _pgText).AppendLine();

            // RecordPos
            if (_recordPos.HasValue) script.AppendFormat("recordpos:'{0}',", _recordPos.ToString().ToLower()).AppendLine();

            // RecordText
            if (_recordText.IsPresent())
                script.AppendFormat("recordtext:'{0}',", _recordText).AppendLine();

            // Request Type
            if (_requestType.HasValue)
            {
                script.AppendFormat("mtype:'{0}',", _requestType.ToString().ToLower()).AppendLine();
            }
            else
            {
                _requestType = RequestType.Post;
                script.AppendFormat("mtype:'{0}',", _requestType.ToString().ToLower()).AppendLine();
            }
            if (_colMenu.HasValue)
            {
                script.AppendFormat("colMenu :{0},", _colMenu.ToString().ToLower()).AppendLine();
            }
            // ResizeClass
            if (_resizeClass.IsPresent())
                script.AppendFormat("resizeclass:'{0}',", _resizeClass).AppendLine();

            // Rowlist
            if (_rowList != null)
                script.AppendFormat("rowList:[{0}],",
                                    string.Join(",", ((from p in _rowList select p.ToString()).ToArray()))).AppendLine();

            // Rownum
            if (_rowNum.HasValue) script.AppendFormat("rowNum:{0},", _rowNum).AppendLine();

            // Rownumbers
            if (_rowNumbers.HasValue)
                script.AppendFormat("rownumbers:{0},", _rowNumbers.ToString().ToLower()).AppendLine();

            // RowNumWidth
            if (_rowNumWidth.HasValue) script.AppendFormat("rownumWidth:{0},", _rowNumWidth).AppendLine();
            //列拖拽排序
            //script.AppendLine("sortable: true,");

            // Virtual scroll (make sure either scroll or scrollint is set, never both)
            if (_scrollInt.HasValue)
            {
                script.AppendFormat("scroll:{0},", _scrollInt).AppendLine();
            }
            else if (_scroll.HasValue)
            {
                script.AppendFormat("scroll:{0},", _scroll.ToString().ToLower()).AppendLine();
            }

            // ScrollOffset
            if (_scrollOffset.HasValue) script.AppendFormat("scrollOffset:{0},", _scrollOffset).AppendLine();

            // ScrollRows
            if (_scrollRows.HasValue)
                script.AppendFormat("scrollrows:{0},", _scrollRows.ToString().ToLower()).AppendLine();

            // ScrollTimeout
            if (_scrollTimeout.HasValue) script.AppendFormat("scrollTimeout:{0},", _scrollTimeout).AppendLine();

            // Sortname
            if (_sortName.IsPresent()) script.AppendFormat("sortname:'{0}',", _sortName).AppendLine();

            // Sorticons
            if (_showAllSortIcons.HasValue || _sortIconDirection.HasValue || _sortOnHeaderClick.HasValue)
            {
                // Set defaults
                if (!_showAllSortIcons.HasValue) _showAllSortIcons = false;
                if (!_sortIconDirection.HasValue) _sortIconDirection = Direction.Vertical;
                if (!_sortOnHeaderClick.HasValue) _sortOnHeaderClick = true;

                script.AppendFormat("viewsortcols:[{0},'{1}',{2}],", _showAllSortIcons.ToString().ToLower(),
                                    _sortIconDirection.ToString().ToLower(), _sortOnHeaderClick.ToString().ToLower()).AppendLine();
            }

            // Shrink to fit缩小以适合
            if (_shrinkToFit.HasValue)
                script.AppendFormat("shrinkToFit:{0},", _shrinkToFit.ToString().ToLower()).AppendLine();

            // Sortorder
            if (_sortOrder.HasValue) script.AppendFormat("sortorder:'{0}',", _sortOrder.ToString().ToLower()).AppendLine();

            // Toolbar
            if (_toolbar.HasValue)
                script.AppendFormat("toolbar:[{0},'{1}'],", _toolbar.ToString().ToLower(), _toolbarPosition.ToString().ToLower()).
                    AppendLine();

            // Toppager
            if (_topPager.HasValue)
                script.AppendFormat("toppager:{0},", _topPager.ToString().ToLower()).AppendLine();

            // Url
            if (!_url.IsMissing())
            {
                script.AppendFormat("url:'{0}',", _url).AppendLine();
            }
            else
            {
                script.AppendLine($"url:'{ _applicationContext.BaseUrl }/Api/Core/datalist',");
            }
            script.AppendFormat("tn:'{0}',", TableName).AppendLine();
            //EditUrl
            if (_editUrl.IsPresent())
            {
                script.AppendFormat("editurl:'{0}',", _editUrl).AppendLine();
            }
            // View records
            if (_viewRecords.HasValue)
                script.AppendFormat("viewrecords:{0},", _viewRecords.ToString().ToLower()).AppendLine();

            // Width
            if (_width.HasValue) script.AppendFormat("width:'{0}',", _width).AppendLine();

            // IgnoreCase
            if (_ignoreCase.HasValue) script.AppendFormat("ignoreCase:{0},", _ignoreCase.ToString().ToLower()).AppendLine();

            // onAfterInsertRow
            if (_onAfterInsertRow.IsPresent())
                script.AppendFormat("afterInsertRow: function(rowid, rowdata, rowelem) {{{0}}},", _onAfterInsertRow).
                    AppendLine();

            // jqGrid Hacking detected beause jqGrid didn't implement default search value correctly we gonna fix this in here
            if (_columns.Any(x => x.HasDefaultSearchValue))
            {
                #region jqGrid javascript onbefore request hack

                var defaultValueColumns = _columns.Where(x => x.HasDefaultSearchValue).Select(x => new { field = x.Index, op = x.SearchOption, data = x.DefaultSearchValue });

                var onbeforeRequestHack = @"
                function() {
                        var defaultValueColumns = " + (defaultValueColumns).ToJson() + @";
                        var colModel = this.p.colModel;

                        if (defaultValueColumns.length > 0) {
                            var postData = this.p.postData;

                            var filters = {};

                            if (postData.hasOwnProperty('filters')) {
                                filters = JSON.parse(postData.filters);
                            }

                            var rules = [];

                            if (filters.hasOwnProperty('rules')) {
                                rules = filters.rules;
                            }

                            $.each(defaultValueColumns, function (defaultValueColumnIndex, defaultValueColumn) {
                                $.each(rules, function (index, rule) {
                                    if (defaultValueColumn.field == rule.field) {
                                        delete rules[index];
                                        return;
                                    }
                                });

                                rules.push(defaultValueColumn);
                            });

                            filters.groupOp = 'AND';
                            filters.rules = rules;

                            postData._search = true;
                            postData.filters = JSON.stringify(filters);

                            $(this).setGridParam({ search: true, 'postData': postData});
                        }

                        this.p.beforeRequest = function() { " + ((!_onBeforeRequest.IsMissing()) ? _onBeforeRequest : "") + @" };
                        this.p.beforeRequest.call(this);
                    } ";

                #endregion jqGrid javascript onbefore request hack

                script.AppendFormat("beforeRequest: {0},", onbeforeRequestHack).AppendLine();
            }
            // onBeforeRequest
            else if (!_onBeforeRequest.IsMissing())
            {
                script.AppendFormat("beforeRequest: function() {{{0}}},", _onBeforeRequest).AppendLine();
            }

            // onBeforeSelectRow
            if (!_onBeforeSelectRow.IsMissing())
                script.AppendFormat("beforeSelectRow: function(rowid, e) {{{0}}},", _onBeforeSelectRow).AppendLine();

            // onGridComplete
            //if (!_onGridComplete.IsMissing())
            script.AppendFormat("gridComplete: function() {{registAttachmentFuntion('" + _id + "');{0}}},", _onGridComplete).AppendLine();

            // onLoadBeforeSend
            if (!_onLoadBeforeSend.IsMissing())
                script.AppendFormat("loadBeforeSend: function(xhr, settings) {{{0}}},", _onLoadBeforeSend).AppendLine();
            #region 加密数据

            DynamicParameters param = new DynamicParameters();
            param.Add("TableName", TableName);
            var dataEncrypt = _dataAccessor.QueryWhere<FapDataEncrypt>("TableUid=@TableName", param);
            StringBuilder encryptJs = new StringBuilder();
            if (dataEncrypt != null && dataEncrypt.Any())
            {
                encryptJs.AppendLine(@"
                var ids = jQuery('###gridid##').jqGrid('getDataIDs');  
                for(var i=0;i < ids.length;i++){  
                    var ret = jQuery('###gridid##').jqGrid('getRowData',ids[i]); ");
                foreach (var data in dataEncrypt)
                {
                    encryptJs.AppendLine("if(ret.Fid=='" + data.FidValue + "'){ debugger");
                    encryptJs.AppendLine("jQuery('###gridid##').jqGrid('setRowData',ret.Id,{" + data.ColumnName + ":'******'}) ;  ");
                    encryptJs.AppendLine("}");
                }
                encryptJs.AppendLine("}");
            }

            #endregion
            // onLoadComplete，默认适应ACE

            script.AppendFormat(@"
                    loadComplete: function(xhr) {{
                        var table = this;" + encryptJs.ToString() + @";
                        resetGridSize(table,'.##wrapper##'); 
                        setTimeout(function(){{
                           updatePagerIcons(table);
                           enableTooltips(table);                            
						}}, 0);
                        {0} }},", _onLoadComplete.IsPresent()?_onLoadComplete : "").AppendLine();

            // onLoadError
            if (!_onLoadError.IsMissing())
                script.AppendFormat("loadError: function(xhr, status, error) {{{0}}},", _onLoadError).AppendLine();

            // onCellSelect
            if (!_onCellSelect.IsMissing())
                script.AppendFormat("onCellSelect: function(rowid, iCol, cellcontent, e) {{{0}}},", _onCellSelect).
                    AppendLine();

            // onDblClickRow
            if (!_onDblClickRow.IsMissing())
                script.AppendFormat("ondblClickRow: function(rowid, iRow, iCol, e) {{{0}}},", _onDblClickRow).AppendLine
                    ();

            // onHeaderClick
            if (!_onHeaderClick.IsMissing())
                script.AppendFormat("onHeaderClick: function(gridstate) {{{0}}},", _onHeaderClick).AppendLine();

            // onPaging
            if (!_onPaging.IsMissing())
                script.AppendFormat("onPaging: function(pgButton) {{{0}}},", _onPaging).AppendLine();

            // onRightClickRow
            if (!_onRightClickRow.IsMissing())
                script.AppendFormat("onRightClickRow: function(rowid, iRow, iCol, e) {{{0}}},", _onRightClickRow).
                    AppendLine();

            // onSelectAll
            if (!_onSelectAll.IsMissing())
                script.AppendFormat("onSelectAll: function(aRowids, status) {{{0}}},", _onSelectAll).AppendLine();

            // onSelectRow event            

            // onSelectRow event

            if (_editUrl.EqualsWithIgnoreCase("clientArray") && _pager.IsPresent())
            {
                script.AppendLine("onSelectRow:editRow,");
            }
            else if (!_onSelectRow.IsMissing())
            {
                script.AppendFormat("onSelectRow: function(rowid, status) {{{0}}},", _onSelectRow).AppendLine();
            }
            // onSortCol
            if (!_onSortCol.IsMissing())
                script.AppendFormat("onSortCol: function(index, iCol, sortorder) {{{0}}},", _onSortCol).AppendLine();

            // onResizeStart
            if (!_onResizeStart.IsMissing())
                script.AppendFormat("resizeStart: function(jqGridEvent, index) {{{0}}},", _onResizeStart).AppendLine();

            // onResizeStop
            if (!_onResizeStop.IsMissing())
                script.AppendFormat("resizeStop: function(newwidth, index) {{{0}}},", _onResizeStop).AppendLine();

            // onSerializeGridData
            if (!_onSerializeGridData.IsMissing())
                script.AppendFormat("serializeGridData: function(postData) {{{0}}},", _onSerializeGridData).AppendLine();

            // rowattr
            if (!_rowAttr.IsMissing())
                script.AppendFormat("rowattr: function(rd) {{{0}}},", _rowAttr).AppendLine();

            // TreeGrid controls
            if (_enabledTreeGrid)
            {
                if (_columns.Count(r => r.IsExpandable) > 1)
                {
                    throw new ArgumentException("Too many key columns set as expandable. Maximum allowed is 1.");
                }
                var keyColumn = _columns.FirstOrDefault(r => r.IsKey);
                //var expandableColumn = _columns.FirstOrDefault(r => r.IsExpandable);
                //if (keyColumn == null)
                //{
                //    throw new ArgumentException("Enabling treegrid needs at least one column set as Key.");
                //}
                //if (expandableColumn == null)
                if (_expandColumn.IsMissing())
                {
                    throw new ArgumentException("Enabling treegrid needs at least one column set as expandable.");
                }
                // enable tree grid
                script.AppendLine("loadonce:false,treeGrid: true,");
                // which column is expandable
                script.AppendFormat("ExpandColumn : '{0}',", _expandColumn).AppendLine();
                // expand a node when click on the node name 
                script.AppendLine("ExpandColClick : true,");
                // datatype
                script.AppendLine("treedatatype:'json',");

                script.AppendFormat("treeGridModel : '{0}',", _treeGridModel.ToString().ToLower()).AppendLine();
                script.AppendLine(@"
                treeReader:{
					'parent_id_field':'Pid',
					'level_field':'level',
					'leaf_field':'isLeaf',
					'expanded_field':'expanded',
					'loaded':'loaded',
					'icon_field':'icon'
				},");
                // script.AppendFormat("tree_root_level : {0},", _treeGridRootLevel).AppendLine();
                script.AppendFormat("treeIcons : {0},", "{ plus: 'orange ace-icon fa fa-folder bigger-140', minus: 'orange ace-icon fa fa-folder-open bigger-140', leaf: 'green fa fa-building-o bigger-140' }").AppendLine();
            }

            // Colmodel
            script.AppendLine("colModel: [");
            if (_enabledTreeGrid)
            {
                script.AppendLine("{name:'Tid',hidden:true,key:true,index:'Tid'},");
                var keycol = _columns.FirstOrDefault(c => c.IsKey == true);
                if (keycol != null)
                {
                    keycol.IsKey = false;
                }
            }
            var colModel = string.Join(",", _columns.Select(c => c.ToString()));
            //var colModel = string.Join(",", ((from c in _columns select c.ToString()).ToArray()));
            script.AppendLine(colModel);
            script.AppendLine("]");

            // End jqGrid call
            script.AppendLine("});");

            script.AppendLine("$(window).triggerHandler('resize.jqGrid');//触发窗口调整,使Grid得到正确的大小");


            if (_editRowModel == EditRowModel.Dialog)
            {
                #region JqGrid操作

                string hasSearch = "false";
                string hasRefresh = "false";
                if ((_dataFormType & OperEnum.Search) > 0)
                {
                    hasSearch = "true";
                }
                if ((_dataFormType & OperEnum.Refresh) > 0)
                {
                    hasRefresh = "true";
                }
                if (_enabledTreeGrid)
                {
                    hasSearch = "false";
                    //hasRefresh = "false";
                }
                script.AppendLine(@" 
                jQuery('###gridid##').jqGrid('navGrid', '###pager##',
                    { 	//navbar options
                        edit: false,
                        add: false,
                        del: false,
                        search: " + hasSearch + @",
                        searchicon: 'ace-icon fa fa-search orange',
                        refresh:  " + hasRefresh + @",
                        refreshicon : 'ace-icon fa fa-refresh green',
                        view: false  
                    },{},{},{},
                    {
                        //search form
                        recreateForm: true,
                        afterShowSearch: function (e) {
                            var form = $(e[0]);
                            form.closest('.ui-jqdialog').find('.ui-jqdialog-title').wrap('<div class=" + "\"widget-header\"" + @"  />')
                            style_search_form(form); ");
                //显示查询方案下拉框
                if ((_dataFormType & OperEnum.QueryProgram) > 0)
                {
                    script.AppendLine(@"loadQueryProgram(form, '##gridid##', '" + TableName + @"'); ");
                }
                script.AppendLine(@"  },
                        afterRedraw: function () {
                            style_search_filters($(this)); ");
                //显示查询方案保存按钮
                if ((_dataFormType & OperEnum.QueryProgram) > 0)
                {
                    script.AppendLine(@"addQueryProgram($(this), '##gridid##', '" + TableName + @"'); ");
                }
                script.AppendLine(@" }
                        ,
                        multipleSearch: true,                            
                        multipleGroup:true                        
                        //showQuery: true
                        
                    },{}
                )
              ");
                //向后排列
                if ((_dataFormType & OperEnum.Import) > 0)
                {
                    ImportToolbar(script);
                }
                //都可以导出数据
                if ((_dataFormType & OperEnum.ExportExcel) > 0)
                {
                    ExportExcelToolbar(script);
                }
                if ((_dataFormType & OperEnum.ExportWord) > 0)
                {
                    ExportWordToolbar(script);

                }
                //以下工具栏向前排
                //查看工具
                if ((_dataFormType & OperEnum.View) > 0)
                {
                    ViewToolbar(script);
                }
                if ((_dataFormType & OperEnum.Delete) > 0)
                {
                    DeleteToolbar(script);
                }
                if ((_dataFormType & OperEnum.Update) > 0)
                {
                    EditToolbar(script);
                }
                if ((_dataFormType & OperEnum.BatchUpdate) > 0)
                {
                    BatchUpdateToolbar(script);
                }
                if ((_dataFormType & OperEnum.Add) > 0)
                {
                    AddToolbar(script);
                }

                //设置列头分组
                GroupHeaders(script);
                #endregion

                #region 搜索栏设置
                // Search clear button
                if (_searchToolbar == true && _searchClearButton.HasValue && !_pager.IsMissing() &&
                    _searchClearButton == true)
                {
                    script.AppendLine("jQuery('###gridid##').jqGrid('navButtonAdd',\"#" + _pager +
                                     "\",{caption:\"清空搜索\",title:\"清空搜索\",buttonicon :'ace-icon fa fa-circle-o-notch green', onClickButton:function(){jQuery('###gridid##')[0].clearToolbar(); }}); ");
                }

                if (_searchToolbar == true && _searchToggleButton.HasValue && !_pager.IsMissing() && _searchToggleButton == true)
                {
                    script.AppendLine("jQuery('###gridid##').jqGrid('navButtonAdd',\"#" + _pager +
                                  "\",{caption:\"搜索栏\",title:\"搜索栏\",buttonicon :'ace-icon fa fa-eye green', onClickButton:function(){jQuery('###gridid##')[0].toggleToolbar(); }}); ");
                }

                // Search toolbar
                if (_searchToolbar == true)
                {
                    script.Append("jQuery('###gridid##').jqGrid('filterToolbar', {stringResult:" + _stringResult.ToString().ToLower());
                    if (_searchOnEnter.HasValue)
                        script.AppendFormat(", searchOnEnter:{0}", _searchOnEnter.ToString().ToLower());
                    script.AppendLine("});");
                }

                #endregion
            }
            else
            {
                #region row inline
                StringBuilder initData = new StringBuilder("{");
                if (_defaultValues.Any())
                {
                    List<string> list = new List<string>();
                    foreach (var dv in _defaultValues)
                    {
                        list.Add($"'{dv.Field}':'{dv.Value}'");
                    }
                    initData.Append(string.Join(",", list));
                }
                initData.Append("}");
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("   $('###gridid##').navGrid('###pager##', {edit: false, add: false, del: false, refresh: false,search:false, view: false});");
                sb.AppendLine("            $('###gridid##').inlineNav('###pager##',");
                sb.AppendLine("                // the buttons to appear on the toolbar of the grid");
                sb.AppendLine("                { ");
                sb.AppendLine("                    edit: true, ");
                if (_editUrl.EqualsWithIgnoreCase("clientArray"))
                {
                    sb.AppendLine("                    restoreAfterSelect : false,saveAfterSelect : true, ");
                }
                else
                {
                    sb.AppendLine("                    restoreAfterSelect : false,saveAfterSelect : false, ");
                }
                sb.AppendLine("                    add: true,addicon:'ace-icon fa fa-plus-circle', ");
                sb.AppendLine("                    del: true,delicon:'ace-icon fa fa-trash-o red', ");
                sb.AppendLine("                    cancel: true,saveicon:'ace-icon fa fa-check',cancelicon:'ace-icon fa  fa-ban',");
                sb.AppendLine("                    editParams: {");
                sb.AppendLine("                        keys: true,");
                sb.AppendLine("                    },");
                sb.AppendLine("                    addParams: {");
                sb.AppendLine("                        keys: true,initdata:" + initData.ToString() + "");
                sb.AppendLine("                    }");
                sb.AppendLine("                });");
                if (_editUrl.EqualsWithIgnoreCase("clientArray") && _pager.IsPresent())
                {
                    var gid = _id.Replace('-', '_');
                    sb.AppendLine("            var lastSelection_" + gid + ";");
                    sb.AppendLine("            function editRow(id) {");
                    sb.AppendLine("                if (id &&id !== lastSelection_" + gid + ") {");
                    sb.AppendLine("                    var grid = $('###gridid##');");
                    sb.AppendLine("                     grid.jqGrid('editRow',id, {keys: true} );");
                    sb.AppendLine("                     lastSelection_" + gid + " = id;");
                    sb.AppendLine("                }");
                    sb.AppendLine("             window.setTimeout(function () {");
                    sb.AppendLine("              $('###gridid##_ilsave').removeClass('ui-state-disabled');");
                    sb.AppendLine("              $('###gridid##_ilcancel').removeClass('ui-state-disabled');");
                    //sb.AppendLine("              $('#" + _id + "_iladd').addClass('ui-state-disabled');");
                    sb.AppendLine("               });  ");
                    sb.AppendLine("            }");
                }

                //删除按钮
                //DeleteToolbar(script);
                script.AppendLine(sb.ToString());
                #endregion
            }

            // 单界面ajax销毁使用
            script.AppendLine(@"
                    $(document).one('ajaxloadstart.page', function(e) {
            			//$('###gridid##').jqGrid('gridUnload');
                        $.jgrid.gridUnload( '##gridid##' ); 
            			$('.ui-jqdialog').remove();
            		    });");

            // End script
            script.AppendLine("});");

            // Insert grid id where needed (in columns)
            script.Replace("##gridid##", _id).Replace("##pager##", _pager).Replace("##wrapper##", _insideElementClass);

            return script.ToString();
        }
        /// <summary>
        /// 批量编辑
        /// </summary>
        /// <param name="script"></param>
        private void BatchUpdateToolbar(StringBuilder script)
        {
            script.AppendLine(@" 
                    jQuery('###gridid##').jqGrid('navButtonAdd', '###pager##',{
                      caption:'',
                      title:'批量编辑',
                      position:'first',  
                      buttonicon:'ace-icon fa fa-pencil-square-o',
                      onClickButton : function() {                      
                        loadBatchUpdateMessageBox('批量编辑','##gridid##','" + HttpUtility.UrlEncode(_querySet.QueryCols) + "','" + TableName + @"',0);
                    }
                  });");
        }

        private void ExportExcelToolbar(StringBuilder script)
        {
            script.AppendLine(@" 
                    jQuery('###gridid##').jqGrid('navButtonAdd', '###pager##',{
                      caption:'',
                      title:'导出',
                      position:'last',  
                      buttonicon:'ace-icon fa fa-file-excel-o green',
                      onClickButton : function() {
                      
                        loadExportExcelMessageBox('导出Excel','##gridid##','" + HttpUtility.UrlEncode(_querySet.QueryCols) + "','" + TableName + @"',0);
                    }
                  });");
        }
        private void ExportWordToolbar(StringBuilder script)
        {
            script.AppendLine(@" 
                    jQuery('###gridid##').jqGrid('navButtonAdd', '###pager##',{
                      caption:'',
                      title:'导出',
                      position:'last',  
                      buttonicon:'ace-icon fa fa-file-word-o',
                      onClickButton : function() {
                      
                        loadExportWordMessageBox('导出Word','##gridid##','" + HttpUtility.UrlEncode(_querySet.QueryCols) + "','" + TableName + @"',0);
                    }
                  });");
        }

        private void ImportToolbar(StringBuilder script)
        {
            #region 导入
            script.AppendLine(@" 
                    jQuery('###gridid##').jqGrid('navButtonAdd', '###pager##',{
                      caption:'',
                      title:'导入',
                      position:'last',  
                      buttonicon:'ace-icon fa fa-cloud-upload',
                      onClickButton : function() {
                      
                        loadImportDataMessageBox('导入','##gridid##','" + _querySet.QueryCols + "','" + TableName + @"',0);
                    }
                  });");

            #endregion
        }

        private void EditToolbar(StringBuilder script)
        {
            script.AppendLine(@" 
                    jQuery('###gridid##').jqGrid('navButtonAdd', '###pager##',{
                      caption:'',
                      title:'编辑',
                      position:'first',
                      buttonicon:'ace-icon fa fa-pencil blue',
                      onClickButton : function() {
                      var gsr = jQuery('###gridid##').jqGrid('getGridParam', 'selrow');
                      if (gsr) {
                        var ret = jQuery('###gridid##').jqGrid('getRowData', gsr);
                        loadFormMessageBox('编辑','##gridid##','fa fa-pencil-square-o','" + TableName + @"',ret.Fid,'" + HttpUtility.UrlEncode(_querySet.QueryCols) + "',function(){");
            if (_onEditAfterInitDataForm.IsPresent())
            {
                script.AppendLine(_onEditAfterInitDataForm);
            }
            script.AppendLine(@"      });
                      } else {
                        bootbox.alert('请选择一条数据');
                      }
                    }
                  });");
        }

        private void AddToolbar(StringBuilder script)
        {
            script.AppendLine(@" 
                    jQuery('###gridid##').jqGrid('navButtonAdd', '###pager##',{
                      caption:'',
                      title:'新增',
                      position:'first',  
                      buttonicon:'ace-icon fa fa-plus-circle purple',
                      onClickButton : function() {
                      loadFormMessageBox('新增','##gridid##','fa fa-plus-circle','" + TableName + @"',0,'" + HttpUtility.UrlEncode(_querySet.QueryCols) + "',function(){");
            if (_onAddAfterInitDataForm.IsPresent())
            {
                script.AppendLine(_onAddAfterInitDataForm);
            }
            if (_defaultValues.Any())
            {
                foreach (var dv in _defaultValues)
                {
                    script.AppendLine("$('#frm-" + TableName + " #" + dv.Field + "').val('" + dv.Value + "')");
                }
            }
            script.AppendLine(@" 
                        });
                    }
                  });");
        }
        private void DeleteToolbar(StringBuilder script)
        {
            //保存的时候校验此值 (加上表名，避免连续打开连个表单，session就不同了)
            script.AppendLine(@" 
                    jQuery('###gridid##').jqGrid('navButtonAdd', '###pager##',{
                      caption:'',
                      title:'删除',
                      position:'first',  
                      buttonicon:'ace-icon fa fa-trash-o red',
                      onClickButton : function() {
                        deleteGridRow('##gridid##','" + TableName + @"');
                    }
                  });");
        }
        private void ViewToolbar(StringBuilder script)
        {
            script.AppendLine(@" 
                    jQuery('###gridid##').jqGrid('navButtonAdd', '###pager##',{
                      caption:'',
                      title:'查看', 
                      position:'first',  
                      buttonicon:'ace-icon fa fa-search-plus grey',
                      onClickButton : function() {
                       var gsr = jQuery('###gridid##').jqGrid('getGridParam', 'selrow');
                      if (gsr) {
                        var ret = jQuery('###gridid##').jqGrid('getRowData', gsr);
                        viewFormMessageBox(ret.Fid,'" + TableName + "','" + HttpUtility.UrlEncode(_querySet.QueryCols) + "'" + @");
                        }else{
                            $.msg('请选择一条数据查看')
                        }
                    }
                    
                  });");
        }
        /// <summary>
        /// 列头分组
        /// </summary>
        /// <param name="script"></param>
        private void GroupHeaders(StringBuilder script)
        {
            if (_groupHeaders != null && _groupHeaders.Any())
            {
                string strGroupHeaders = string.Join(',', _groupHeaders.Select(h => $"{{\"numberOfColumns\": {h.NumberOfColumns}, \"titleText\": \"{h.TitleText}\", \"startColumnName\": \"{h.StartColumnName}\" }}"));
                script.AppendLine(@" 
            $('###gridid##').setGroupHeaders(
                {
                    useColSpanStyle: true,
                    groupHeaders: [
                            " + strGroupHeaders + @"
                        ]
                }); ");
            }
        }
        /// <summary>
        /// Renders the required Html Elements
        /// </summary>
        /// <returns></returns>
        public string RenderHtmlElements()
        {
            var dataBuilder = new StringBuilder();
            if (_tabExtraData != null)
            {
                foreach (var data in _tabExtraData)
                {
                    dataBuilder.AppendFormat("data-{0}='{1}' ", data.Key, data.Value);
                }
            }

            // Create table which is used to render grid
            var gridHtml = new StringBuilder();
            gridHtml.AppendFormat("<table id=\"{0}\" {1}></table>", _id, dataBuilder.ToString()).AppendLine();

            // Create pager element if is set
            if (!_pager.IsMissing())
            {
                gridHtml.AppendFormat("<div id=\"{0}\"></div>", _pager).AppendLine();
            }

            // Create toppager element if is set
            if (_topPager == true)
            {
                gridHtml.AppendFormat("<div id=\"{0}_toppager\"></div>", _id).AppendLine();
            }

            var existFile = _fapColumns.Where(f => f.CtrlType == FapColumn.CTRL_TYPE_FILE);
            if (existFile != null && existFile.Any())
            {
                //参照链接查看层
                gridHtml.AppendLine("<div id=\"dialog-attachment" + _id + "\" class=\"hide\">");
                gridHtml.AppendLine("  <div class=\"row\"><div id=\"attachmentdiv" + _id + "\" class=\"col-lg-12\">");
                gridHtml.AppendLine("   </div></div>");
                gridHtml.AppendLine("</div>");
            }
            return gridHtml.ToString();
        }

        /// <summary>
        ///     Creates and returns javascript + required html elements to render grid
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // Create javascript
            var script = new StringBuilder();

            // Start script
            script.AppendLine("<script type=\"text/javascript\">");
            //压缩js
            JavaScriptCompressor compressor = new JavaScriptCompressor();
            compressor.Encoding = Encoding.UTF8;
            script.Append(compressor.Compress(RenderJavascript()));
            script.AppendLine("</script>");

            // Return script + required elements
            return script.ToString() + RenderHtmlElements();
        }

        public string ToHtmlString()
        {
            return ToString();
        }
    }
}