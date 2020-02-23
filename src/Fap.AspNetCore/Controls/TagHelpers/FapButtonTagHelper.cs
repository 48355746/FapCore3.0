using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using Fap.Core.Extensions;
using System.Text;
using System.Threading.Tasks;
using Fap.Core.Rbac;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.MultiLanguage;

namespace Fap.AspNetCore.Controls.TagHelpers
{
    public class FapButtonTagHelper : TagHelper
    {
        public ButtonTag BtnTag { get; set; } = ButtonTag.link;
        public string Entity { get; set; }
        public string Id { get; set; }
        public string LinkHref { get; set; }
        public string Content { get; set; }
        public string ClassName { get; set; }
        public string IconBefore { get; set; }
        public string IconAfter { get; set; }
        private readonly IRbacService _rbacService;
        private readonly IFapApplicationContext _applicationContext;
        private readonly IMultiLangService _multiLangService;
        public FapButtonTagHelper(IRbacService rbacService, IFapApplicationContext applicationContext, IMultiLangService multiLangService)
        {
            _rbacService = rbacService;
            _applicationContext = applicationContext;
            _multiLangService = multiLangService;
        }
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var button = new Core.Rbac.Model.FapMenuButton()
            {
                ButtonID = Id,
                ButtonName = Content,
                Description = Content,
                Enabled = 1,
                ButtonType=BtnTag==ButtonTag.link?FapMenuButtonType.Link:FapMenuButtonType.Button

            };
            //注册按钮,同时获取多语名称
            string permission = _rbacService.GetMenuButtonAuthority(_applicationContext.CurrentRoleUid, button);

            if (permission.IsPresent() && permission.Equals("1"))
            {
                StringBuilder builder = new StringBuilder();
                if (BtnTag == ButtonTag.link)
                {
                    button.ButtonType = FapMenuButtonType.Link;
                    output.TagName = "a";
                    output.Attributes.Add("class", ClassName);
                    output.Attributes.Add("id", Id);
                    output.Attributes.Add("href", LinkHref.IsMissing() ? "javascript:void(0)" : LinkHref);
                    output.Attributes.Add("title", button.Description);
                    builder.Append($"  <i class=\"ace-icon {IconBefore}\"></i>");
                    builder.Append(button.Description);

                }
                else
                {
                    button.ButtonType = FapMenuButtonType.Button;
                    output.TagName = "button";
                    output.Attributes.Add("class", ClassName);
                    //builder.Append($" <button {Attribute} class=\"btn {ClassName}\">");
                    builder.Append($"<i class=\"ace-icon {IconBefore}\"></i>");
                    builder.Append(button.Description);
                    builder.Append($" <span class=\"ace-icon {IconAfter} icon-on-right\"></span>");
                    //builder.Append("</button>");
                }


                output.Content.SetHtmlContent(builder.ToString());
            }
            return base.ProcessAsync(context, output);
        }

    }
    public enum ButtonTag
    {
        button, link
    }
}
