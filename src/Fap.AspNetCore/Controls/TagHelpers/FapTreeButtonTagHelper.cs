using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Rbac;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
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
        private readonly IRbacService _rbacService;
        private readonly IFapPlatformDomain _fapPlatformDomain;
        public FapTreeButtonTagHelper(IRbacService rbacService, IFapPlatformDomain platformDomain)
        {
            _rbacService = rbacService;
            _fapPlatformDomain = platformDomain;
        }
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            //注册按钮
            _rbacService.RegisterMenuButton(new Core.Rbac.Model.FapMenuButton()
            {
                ButtonID = Id,
                ButtonName="树按钮",
                ButtonType = FapMenuButtonType.Tree,
                Description = Description
            });

            OperType |= OperEnum.Add | OperEnum.Update | OperEnum.Delete;
            output.TagName = "div";
            output.Content.Clear();
            output.Attributes.Add("class", "widget-toolbar");
            output.Content.AppendHtml(AddJavaScript(TreeId));
            if ((OperType & OperEnum.Add) > 0)
            {
                output.Content.AppendHtml(AddOper("add", "fa fa-plus-circle purple"));
            }
            if ((OperType & OperEnum.Update) > 0)
            {
                output.Content.AppendHtml(AddOper("edit", " fa fa-pencil blue"));
            }
            if ((OperType & OperEnum.Delete) > 0)
            {
                output.Content.AppendHtml(AddOper("delete", " fa fa-trash-o red"));
            }
            if ((OperType & OperEnum.Refresh) > 0)
            {
                output.Content.AppendHtml(AddOper("refresh", " fa fa-refresh"));
            }

            return Task.CompletedTask; //base.ProcessAsync(context, output);
        }
        private string AddOper(string oper, string icon)
        {
            return $@"
                    <a href='javascript:void(0)' data-ctrl='widgetAction' data-action='{oper}'>
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
                var ref = $('#" + treeid+ @"').jstree(true),
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
