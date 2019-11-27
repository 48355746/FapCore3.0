using Fap.Core.Extensions;
using Fap.Core.Rbac;
using Microsoft.AspNetCore.Html;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yahoo.Yui.Compressor;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Rbac.Model;
using Ardalis.GuardClauses;

namespace Fap.AspNetCore.Controls
{
    public class JsTree : HtmlString
    {
        private string _id;
        private bool _async = false;
        private string _url;
        private string _editUrl;
        private string _jsonData;
        //private bool _checkbox;
        //private bool _allowDragAndDrop;
        //private bool _showMenu;
        //private bool _allowSearch;
        //private bool _allowSort;
        //private bool _keepState;
        //private bool _unique;
        //private bool _wholeRow;
        //private bool _changed;
        //private bool _massLoad;
        //private bool _conditionalSelect;
        private TreeModel _treeModel;
        //private string _valueField = "Fid";
        //private string _parentValueField = "Pid";
        //public string _displayField;
        //public string _initContidtion;
        private string _nodeIcon = "icon-folder  ace-icon fa fa-folder blue";
        private string _rootNodeIcon = "icon-folder purple ace-icon fa fa-flag";
        private string _rootText = "";
        private string _types;
        //注册事件
        private string _openNodeEvent;
        private string _loadedEvent;
        private string _selectNodeEvent;
        private string _deleteNodeEvent;
        private string _refreshEvent;

        /// <summary>
        /// 同步树数据源
        /// 同步的时候必须输入数据源
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public JsTree SetJsonData(string jsonData)
        {
            _jsonData = jsonData;
            return this;
        }
        /// <summary>
        /// 设置树的实体表,这样就可以直接加载树了
        /// 设置此属性就不用设置url，但要设置SetAsync为false
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public JsTree SetTreeModel(TreeModel treeModel)
        {
            _treeModel = treeModel;
            LoadTreeData();
            return this;
        }
        /// <summary>
        /// 是否为部门树
        /// </summary>
        /// <param name="power">加权限</param>
        /// <returns></returns>
        public JsTree IsOrgDept(bool power)
        {
            IEnumerable<OrgDept> powerDepts = null;
            if (power)
            {
                powerDepts = _rbacService.GetUserDeptList();
            }
            else
            {
                powerDepts =_platformDomain.OrgDeptSet.OrderBy(d => d.DeptOrder);
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
            TreeViewHelper.MakeTree(treeRoot.Children, treeList, treeRoot.Id);
            tree.Add(treeRoot);

            string rej = tree.ToJsonIgnoreNullValue();
            //设置tree的json数据
            SetJsonData(rej);
            return this;
        }
        private List<string> plugins = new List<string>();
        /// <summary>
        /// 显示checkbox框多选
        /// </summary>
        /// <param name="checkBox"></param>
        /// <returns></returns>
        public JsTree SetPluginCheckBox(bool checkBox)
        {
            if (checkBox)
            {
                plugins.Add("'checkbox'");
            }
            return this;
        }
        /// <summary>
        /// 显示右键菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public JsTree SetPluginContextmenu(bool menu)
        {
            if (menu)
            {
                plugins.Add("'contextmenu'");
            }
            return this;
        }
        /// <summary>
        /// 可以拖拽
        /// </summary>
        /// <param name="dnd"></param>
        /// <returns></returns>
        public JsTree SetPluginDnd(bool dnd)
        {
            if (dnd)
            {
                plugins.Add("'dnd'");
            }
            return this;
        }
        /// <summary>
        /// 异步加载叶节点
        /// </summary>
        /// <param name="massload"></param>
        /// <returns></returns>
        public JsTree SetPluginMassLoad(bool massload)
        {
            if (massload)
            {
                plugins.Add("'massload'");
            }
            return this;
        }
        /// <summary>
        /// 可以搜索
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public JsTree SetPluginSearch(bool search)
        {
            if (search)
            {
                plugins.Add("'search'");
            }
            return this;
        }
        /// <summary>
        /// 可以排序
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        public JsTree SetPluginSort(bool sort)
        {
            if (sort)
            {
                plugins.Add("'sort'");
            }
            return this;
        }
        /// <summary>
        /// 保留状态，展开状态等
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public JsTree SetPluginState(bool state)
        {
            if (state)
            {
                plugins.Add("'state'");
            }
            return this;
        }
        /// <summary>
        /// 自定义类型
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public JsTree SetCustomTypes(string types)
        {
            _types = types;
            plugins.Add("'types'");
            return this;
        }
        /// <summary>
        /// 惟一性名称
        /// </summary>
        /// <param name="unique"></param>
        /// <returns></returns>
        public JsTree SetPluginUnique(bool unique)
        {
            plugins.Add("'unique'");
            return this;
        }
        /// <summary>
        /// 整行块选中
        /// </summary>
        /// <param name="wholerow"></param>
        /// <returns></returns>
        public JsTree SetPluginWholeRow(bool wholerow)
        {
            plugins.Add("'wholerow'");
            return this;
        }
        /// <summary>
        /// 新增时候激发chang事件
        /// </summary>
        /// <param name="changed"></param>
        /// <returns></returns>
        public JsTree SetPluginChange(bool changed)
        {
            plugins.Add("'changed'");
            return this;
        }
        /// <summary>
        /// 是否阻碍默认select事件
        /// </summary>
        /// <param name="conditionalselect"></param>
        /// <returns></returns>
        public JsTree SetPluginConditionalSelect(bool conditionalselect)
        {
            plugins.Add("'conditionalselect'");
            return this;
        }
        private IDbContext _dbContext;
        private IFapApplicationContext _applicationContext;
        private IFapPlatformDomain _platformDomain;
        private IRbacService _rbacService;
        public JsTree(IDbContext dataAccessor, IFapApplicationContext applicationContext, IFapPlatformDomain platformDomain,IRbacService rbacService,string id):base("")
        {
            _id = id;
            _dbContext = dataAccessor;
            _applicationContext = applicationContext;
            _platformDomain = platformDomain;
            _rbacService = rbacService;
        }
        /// <summary>
        /// 是否异步加载，true的话一定要设置url
        /// 
        /// </summary>
        /// <param name="async"></param>
        /// <returns></returns>
        public JsTree SetAsync(bool async)
        {
            _async = async;
            return this;
        }
        /// <summary>
        /// 设置获取数据的Url，不与SetJsonData 一起使用
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public JsTree SetUrl(string url)
        {
            _url = url;
            return this;
        }
        /// <summary>
        /// 设置编辑的Url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public JsTree SetEditUrl(string url)
        {
            _editUrl = url;
            return this;
        }
        /// <summary>
        /// 节点展开事件
        /// </summary>
        /// <param name="openNodeEvent"></param>
        /// <returns></returns>
        public JsTree OnOpenNodedEvent(string openNodeEvent)
        {
            _openNodeEvent = openNodeEvent;
            return this;
        }
        /// <summary>
        /// 树加载完事件
        /// </summary>
        /// <param name="loadedEvent"></param>
        /// <returns></returns>
        public JsTree OnLoadedEvent(string loadedEvent)
        {
            _loadedEvent = loadedEvent;
            return this;
        }
        /// <summary>
        /// 注册选中事件
        /// </summary>
        /// <param name="selectNodeEvent"></param>
        /// <returns></returns>
        public JsTree OnSelectNodeEvent(string selectNodeEvent)
        {
            _selectNodeEvent = selectNodeEvent;
            return this;
        }
        /// <summary>
        /// 注册删除节点事件
        /// </summary>
        /// <param name="deleteNodeEvent"></param>
        /// <returns></returns>
        public JsTree OnDeleteNodeEvent(string deleteNodeEvent)
        {
            _deleteNodeEvent = deleteNodeEvent;
            return this;
        }
        /// <summary>
        /// 刷新事件
        /// </summary>
        /// <param name="refreshEvent"></param>
        /// <returns></returns>
        public JsTree OnRefreshEvent(string refreshEvent)
        {
            _refreshEvent = refreshEvent;
            return this;
        }
        public override string ToString()
        {
            // Create javascript
            var script = new StringBuilder();
            // Start script
            script.AppendLine("<script type=\"text/javascript\">");
            JavaScriptCompressor compressor = new JavaScriptCompressor();
            compressor.Encoding = Encoding.UTF8;
            script.Append(compressor.Compress(RenderJavascript()));
            script.AppendLine("</script>");


            // Return script + required elements
            return script + RenderHtmlElements();
        }

        private string RenderHtmlElements()
        {
            StringBuilder jsTreeHtml = new StringBuilder();
            jsTreeHtml.AppendFormat("<div id=\"{0}\"></div>", _id).AppendLine();
            return jsTreeHtml.ToString();
        }

        private string RenderJavascript()
        {
            StringBuilder script = new StringBuilder();
            script.Append(" $(function () {").AppendLine();
            string plugin = string.Empty;
            if (plugins.Any())
            {
                plugin = "'plugins':[" + string.Join(",", plugins) + "],";
            }
            if (_async)
            {
                script.AppendLine("$('#" + _id + "').jstree({");
                script.AppendLine(plugin);
                script.AppendLine("    'core' : {");
                script.AppendLine("     \"check_callback\" : true,");
                script.AppendLine("     'force_text' : true,");
                script.AppendLine("     \"themes\": {\"stripes\": true},");
                script.AppendLine("        'data' : {");
                script.AppendLine("            'url' : '" + _url + "',");
                script.AppendLine("            'dataType' : 'json', // needed only if you do not supply JSON headers");
                script.AppendLine("             'data' : function (node) { return { 'id' : node.id };}");
                script.AppendLine("        },");
                if (!string.IsNullOrWhiteSpace(_types))
                {
                    script.AppendLine("'types':{" + _types + "}");
                }
                script.AppendLine("    }");
                script.AppendLine("})");
            }
            else
            {
                script.AppendLine("$('#" + _id + "').jstree({");
                script.AppendLine(plugin);
                script.AppendLine("    'core' : {");
                script.AppendLine("     \"check_callback\" : true,");
                script.AppendLine("     'force_text' : true,");
                script.AppendLine("     \"themes\": {\"stripes\": true},");
                script.AppendLine("        'data' : " +( _jsonData.IsMissing()?"{ }": _jsonData));

                if (!string.IsNullOrWhiteSpace(_types))
                {
                    script.AppendLine("'types':{" + _types + "}");
                }
                script.AppendLine("    }");
                script.AppendLine("})");
            }
            //if (_openNodeEvent.IsPresent())
            //{
            //    script.AppendFormat(@".bind('open_node.jstree', function(e,data){{doScroll(e,data);{0}}})", _openNodeEvent).AppendLine();
            //    //script.AppendLine(@".bind('open_node.jstree', function(e,data){  doScroll && doScroll(e,data); treeOpenNodeCallback && treeOpenNodeCallback(e,data);  })");
            //}
            if (_loadedEvent.IsPresent())
            {
                script.AppendFormat(@".bind('loaded.jstree', function(e,data){{{0}}})", _loadedEvent).AppendLine();
                //script.AppendLine(@".bind('loaded.jstree', function(e,data){  treeLoadedCallback && treeLoadedCallback(e,data);})");
            }
            if (_deleteNodeEvent.IsPresent())
            {
                script.AppendFormat(@".bind('delete_node.jstree', function(e,data){{{0}}})", _deleteNodeEvent).AppendLine();
                //script.AppendLine(@".bind('delete_node.jstree', function(e,data){  treeDeleteNodeCallback && treeDeleteNodeCallback(e,data);})");
            }
            if (_selectNodeEvent.IsPresent())
            {
                script.AppendFormat(@".bind('select_node.jstree', function(e,data){{{0}}})", _selectNodeEvent).AppendLine();
                //script.AppendLine(@".bind('select_node.jstree', function(e,data){  treeSelectNodeCallback && treeSelectNodeCallback(e,data);})");
            }
            if (_refreshEvent.IsPresent())
            {
                script.AppendFormat(@".bind('refresh.jstree', function(e,data){{{0}}})", _refreshEvent).AppendLine();
                //script.AppendLine(@".bind('refresh.jstree', function(e,data){  treeRefreshCallback && treeRefreshCallback(e,data);})");
            }
            if (!string.IsNullOrWhiteSpace(_editUrl))
            {
                script.AppendLine(@"
                .on('delete_node.jstree', function (e, data) {
					$.get('" + _editUrl + @"?operation=delete_node', { 'id' : data.node.id }).done(function (d) {
							if(d.id==='0'){bootbox.alert('删除失败，可能被占用。');data.instance.refresh();}
						})
						.fail(function () {
							data.instance.refresh();
						});
				})
				.on('create_node.jstree', function (e, data) {
					$.get('" + _editUrl + @"?operation=create_node', { 'id' : data.node.parent, 'position' : data.position, 'text' : data.node.text })
						.done(function (d) {
							data.instance.set_id(data.node, d.id);
                             data.instance.edit(data.node,'自定义');
						})
						.fail(function () {
							data.instance.refresh();
						});
				})
				.on('rename_node.jstree', function (e, data) {
					$.get('" + _editUrl + @"?operation=rename_node', { 'id' : data.node.id, 'text' : data.text })
						.fail(function () {
							data.instance.refresh();
						});
				})
				.on('move_node.jstree', function (e, data) {
					$.get('" + _editUrl + @"?operation=move_node', { 'id' : data.node.id, 'parent' : data.parent, 'position' : data.position })
						.fail(function () {
							data.instance.refresh();
						});
				})
				.on('copy_node.jstree', function (e, data) {
					$.get('" + _editUrl + @"?operation=copy_node', { 'id' : data.original.id, 'parent' : data.parent, 'position' : data.position })
						.always(function () {
							data.instance.refresh();
						});
				});	");


            }
            script.AppendLine(" });");
            return script.ToString();
        }
        public string ToHtmlString()
        {
            return ToString();
        }

        private void LoadTreeData()
        {
            if (_treeModel != null && _treeModel.TableName.IsMissing())
            {
                Guard.Against.Null(_treeModel.TableName, nameof(_treeModel.TableName));
            }
            if (_treeModel != null && _treeModel.DisplayField.IsMissing())
            {
                Guard.Against.Null(_treeModel.DisplayField, nameof(_treeModel.DisplayField));
            }
            if (_treeModel != null && !_treeModel.NodeIcon.IsMissing())
            {
                _nodeIcon = _treeModel.NodeIcon;
            }
            if (_treeModel != null && !_treeModel.RootNodeIcon.IsMissing())
            {
                _rootNodeIcon = _treeModel.RootNodeIcon;
            }
            if (_treeModel != null && !_treeModel.RootText.IsMissing())
            {
                _rootText = _treeModel.RootText;
            }
            string sql = string.Format("select Fid as Id,{0} as Text,Pid,'{1}' as Icon,{2} as Code,{3} as Ext1,{4} as Ext2 from {5} ", _treeModel.DisplayField, _nodeIcon, _treeModel.CodeField, _treeModel.Ext1Field, _treeModel.Ext2Field, _treeModel.TableName);
            if (!_treeModel.InitCondition.IsMissing())
            {
                sql += " where " + _treeModel.InitCondition;
            }
            if (_treeModel.SortBy.IsPresent())
            {
                sql += " order by " + _treeModel.SortBy;
            }
            var dataList =_dbContext.Query(sql);

            //将List<dynamic>转换成List<TreeDataView>
            List<TreeDataView> treeList = new List<TreeDataView>();
            foreach (var data in dataList)
            {
                treeList.Add(new TreeDataView() { Id = data.Id, Text = data.Text, Pid = data.Pid, Data = new { Code = data.Code, Ext1 = data.Ext1, Ext2 = data.Ext2 }, Icon = data.Icon });
            }

            List<TreeDataView> tree = new List<TreeDataView>();
            string parentID = "0";
            if (_rootText.IsMissing())
            {
                var pt = treeList.FirstOrDefault<TreeDataView>(t => t.Pid == "0" || t.Pid.IsMissing() || t.Pid == "#" || t.Pid == "~");
                if (pt != null)
                {
                    _rootText = pt.Text;
                    parentID = pt.Id;
                }
            }
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = parentID,
                Text = _rootText,
                State = new NodeState { Opened = true },
                Icon = _rootNodeIcon,
            };
            TreeViewHelper.MakeTree(treeRoot.Children, treeList, treeRoot.Id);
            tree.Add(treeRoot);

            string rej = tree.ToJsonIgnoreNullValue();
            //设置tree的json数据
            SetJsonData(rej);
        }
    }
    /// <summary>
    /// 树模型
    /// </summary>
    public class TreeModel
    {
        public TreeModel()
        {
            CodeField = "''";
            Ext1Field = "''";
            Ext2Field = "''";
        }
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 显示字段
        /// </summary>
        public string DisplayField { get; set; }
        /// <summary>
        /// data Tag数据Code
        /// </summary>
        public string CodeField { get; set; }
        /// <summary>
        ///  data Tag数据Other
        /// </summary>
        public string Ext1Field { get; set; }

        /// <summary>
        ///  data Tag数据Other
        /// </summary>
        public string Ext2Field { get; set; }
        /// <summary>
        /// 初始化条件
        /// </summary>
        public string InitCondition { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public string SortBy { get; set; }
        /// <summary>
        /// 根节点名称
        /// </summary>
        public string RootText { get; set; }
        /// <summary>
        /// 根节点图标
        /// </summary>
        public string RootNodeIcon { get; set; }
        /// <summary>
        /// 节点图标
        /// </summary>
        public string NodeIcon { get; set; }
        /// <summary>
        /// 带权限控制
        /// </summary>
        public bool WithPermissions { get; set; }
    }
}
