using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Rbac;
using Fap.Core.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Fap.AspNetCore.Controls.TagHelpers
{
    public class FapTreeTagHelper : TagHelper
    {
        private IDbContext _dataAccessor;
        private IFapPlatformDomain _appDomain;
        private IFapApplicationContext _applicationContext;
        private IRbacService _rbacService;
        public string Id { get; set; }
        /// <summary>
        /// 是否异步
        /// </summary>
        public bool IsAsync { get; set; }
        /// <summary>
        /// 是否部门树
        /// </summary>
        public bool IsOrgdept { get; set; } = false;
        /// <summary>
        /// 部门树是否加权限
        /// </summary>
        public bool HasPower { get; set; } = true;
        /// <summary>
        /// 获取数据URL
        /// </summary>
        public string GetUrl { get; set; }
        /// <summary>
        /// 编辑数据Url
        /// </summary>
        public string EditUrl { get; set; }
        /// <summary>
        /// 拖动插件
        /// </summary>
        public bool PluginDnd { get; set; } = false;
        /// <summary>
        /// 多选框
        /// </summary>
        public bool PluginCheckbox { get; set; } = false;
        /// <summary>
        /// 树模型
        /// </summary>
        public TreeModel TreeMode { get; set; }
        /// <summary>
        /// Json数据
        /// </summary>
        public string JsonData { get; set; }
        /// <summary>
        /// 打开node后事件
        /// </summary>
        public string OnOpenNoded { get; set; }
        /// <summary>
        /// 数据载入后事件
        /// </summary>
        public string OnLoaded { get; set; }

        public FapTreeTagHelper(IDbContext dataAccessor,IFapApplicationContext applicationContext, IFapPlatformDomain appDomain, IRbacService rbacService)
        {
            _dataAccessor = dataAccessor;
            _applicationContext = applicationContext;
            _appDomain = appDomain;
            _rbacService = rbacService;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Content.Clear();
            string id = "tree";
            if (Id.IsPresent())
            {
                id = $"tree-{Id}";
            }
            JsTree tree = new JsTree(_dataAccessor, _applicationContext, _appDomain, _rbacService, id);
            tree.SetAsync(IsAsync);
            if (IsOrgdept)
            {
                tree.IsOrgDept(HasPower);
            }
            if (GetUrl.IsPresent())
            {
                tree.SetUrl(GetUrl);
            }
            if (EditUrl.IsPresent())
            {
                tree.SetEditUrl(EditUrl);
            }
            if (JsonData.IsPresent())
            {
                tree.SetJsonData(JsonData);
            }
            if (TreeMode != null)
            {
                tree.SetTreeModel(TreeMode);
            }
            if (OnOpenNoded.IsPresent())
            {
                tree.OnOpenNodedEvent(OnOpenNoded);
            }
            if (OnLoaded.IsPresent())
            {
                tree.OnLoadedEvent(OnLoaded);
            }
            tree.SetPluginDnd(PluginDnd);
            tree.SetPluginCheckBox(PluginCheckbox);
            output.Content.AppendHtml(tree.ToString());

        }

    }
}
