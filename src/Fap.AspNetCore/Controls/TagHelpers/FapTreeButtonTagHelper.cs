using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Rbac;
using Fap.Core.Rbac.Model;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahoo.Yui.Compressor;

namespace Fap.AspNetCore.Controls.TagHelpers
{
    public class FapTreeButtonTagHelper : TagHelper
    {
        public string Id { get; set; }
        public string TreeId { get; set; }
        public OperEnum OperType { get; set; } = OperEnum.Refresh;
        public string Description { get; set; }
        /// <summary>
        /// 是否注册操作权限到菜单按钮表
        /// </summary>
        public bool RegisterButton { get; set; } = true;
        private readonly IRbacService _rbacService;
        public FapTreeButtonTagHelper(IRbacService rbacService)
        {
            _rbacService = rbacService;
        }
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            //鉴权
            string authorize = Authentication();
            if (authorize.IsMissing())
            {
                return Task.CompletedTask;
            }
            OperType = GetTreeOper(authorize);
            //OperType |= OperEnum.Add | OperEnum.Update | OperEnum.Delete;
            output.TagName = "div";
            output.Content.Clear();
            output.Attributes.Add("class", "widget-toolbar");
            output.Content.AppendHtml(AddJavaScript(TreeId));
            if ((OperType & OperEnum.Add) > 0)
            {
                output.Content.AppendHtml(AddOper("add","新增", "fa fa-plus-circle purple"));
            }
            if ((OperType & OperEnum.Update) > 0)
            {
                output.Content.AppendHtml(AddOper("edit","修改", " fa fa-pencil blue"));
            }
            if ((OperType & OperEnum.Delete) > 0)
            {
                output.Content.AppendHtml(AddOper("delete","删除", " fa fa-trash-o red"));
            }
            if ((OperType & OperEnum.Refresh) > 0)
            {
                output.Content.AppendHtml(AddOper("refresh", "刷新", " fa fa-refresh"));
            }

            return Task.CompletedTask; //base.ProcessAsync(context, output);
        }
        private OperEnum GetTreeOper(string authorize)
        {
            var power = authorize.SplitComma().Select(v => v.ToInt());
            int formType = 0;
            foreach (int p in power)
            {
                formType |= p;
            }
            return (OperEnum)formType;
        }
        private string Authentication()
        {
            if (RegisterButton)
            {
                FapMenuButton menuButton = new FapMenuButton()
                {
                    ButtonID = Id,
                    ButtonName = "树按钮",
                    ButtonType = FapMenuButtonType.Tree,
                    Description = Description,
                    Enabled=1
                };
                //注册按钮
                return _rbacService.GetMenuButtonAuthorized(menuButton);
            }
            return string.Empty;
        }
        private string AddOper(string oper,string title, string icon)
        {
            return $@"
                    <a href='javascript:void(0)' data-ctrl='widgetAction' title='{title}' data-action='{oper}'>
                        <i class='ace-icon {icon}'></i>
                    </a>
                    ";
        }
        private string AddJavaScript(string treeid)
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
            return script.ToString();
            string RenderJavascript()
            {
                return @"$(function () {
                $(document).off(ace.click_event, '[data-ctrl=widgetAction]').on(ace.click_event, '[data-ctrl=widgetAction]', function (ev) {
                ev.preventDefault();
                var $this = $(this);
                var $action = $this.data('action');
                var ref = $('#" + treeid + @"').jstree(true),
                    sel = ref.get_selected();
                if ($action == 'refresh') {
                    ref.refresh();
                    return;
                }
                if (!sel.length) { return false; }
                sel = sel[0];
                if ($action == 'add') {
                    sel = ref.create_node(sel);

                } else if ($action == 'edit') {
                    ref.edit(sel);
                } else if ($action == 'delete') {
                    bootbox.confirm('确认删除吗？', function (result) {
                        if (result)
                {
                    ref.delete_node(sel)
                        }
            })
                }
            })
           })";
            }
        }

    }
}
