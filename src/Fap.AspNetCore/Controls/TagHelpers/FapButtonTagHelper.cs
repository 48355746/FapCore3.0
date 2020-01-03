using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using Fap.Core.Extensions;
using System.Text;
using System.Threading.Tasks;
using Fap.Core.Rbac;
using Fap.Core.Infrastructure.Enums;

namespace Fap.AspNetCore.Controls.TagHelpers
{
    public class FapButtonTagHelper : TagHelper
    {
        public ButtonTag Tag { get; set; }
        public string Entity { get; set; }
        public string Id { get; set; }
        public string LinkHref { get; set; }
        public string Content { get; set; }
        public string ClassName { get; set; }
        public string IconBefore { get; set; }
        public string IconAfter { get; set; }
        private readonly IRbacService _rbacService;
        public FapButtonTagHelper(IRbacService rbacService)
        {
            _rbacService = rbacService;
        }
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var button = new Core.Rbac.Model.FapMenuButton()
            {
                ButtonID = Id,
                ButtonName = Content,                
                Description = Content
            };
            
            StringBuilder builder = new StringBuilder();
            if (Tag == ButtonTag.link)
            {
                button.ButtonType = FapMenuButtonType.Link;

                output.TagName = "a";
                output.Attributes.Add("class", ClassName);
                output.Attributes.Add("id", Id);
                output.Attributes.Add("href", LinkHref.IsMissing()? "javascript:void(0)" : LinkHref);
              
                builder.Append($"  <i class=\"ace-icon {IconBefore}\"></i>");
                builder.Append(Content);
               
            }
            else
            {
                button.ButtonType = FapMenuButtonType.Button;
                output.TagName = "button";
                output.Attributes.Add("class", ClassName);
                //builder.Append($" <button {Attribute} class=\"btn {ClassName}\">");
                builder.Append($"<i class=\"ace-icon {IconBefore}\"></i>");
                builder.Append(Content);
                builder.Append($" <span class=\"ace-icon {IconAfter} icon-on-right\"></span>");
                //builder.Append("</button>");
            }
            output.Content.SetHtmlContent(builder.ToString());
            //注册按钮
            _rbacService.RegisterMenuButton(button);
            return base.ProcessAsync(context, output);
        }
    }
    public enum ButtonTag
    {
        button, link
    }
}
