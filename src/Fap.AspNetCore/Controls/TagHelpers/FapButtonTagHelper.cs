using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using Fap.Core.Extensions;
using System.Text;
using System.Threading.Tasks;

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
        private readonly IFapPlatformDomain _platformDomain;
        private readonly IDbContext _dbContext;
        private readonly IFapApplicationContext _applicationContext;
        public FapButtonTagHelper(IFapPlatformDomain platformDomain, IDbContext dbContext, IFapApplicationContext applicationContext)
        {
            _platformDomain = platformDomain;
            _dbContext = dbContext;
            _applicationContext = applicationContext;
        }
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            StringBuilder builder = new StringBuilder();
            if (Tag == ButtonTag.link)
            {
                output.TagName = "a";
                output.Attributes.Add("class", ClassName);
                output.Attributes.Add("id", Id);
                output.Attributes.Add("href", LinkHref.IsMissing()? "javascript:void(0)" : LinkHref);
                //builder.Append($"< a href = \"#\" id=\"{Id}\" class=\"{ClassName}\">");
                builder.Append($"  <i class=\"ace-icon {IconBefore}\"></i>");
                builder.Append(Content);
                //builder.Append("</a>");
            }
            else
            {
                output.TagName = "button";
                output.Attributes.Add("class", ClassName);
                //builder.Append($" <button {Attribute} class=\"btn {ClassName}\">");
                builder.Append($"<i class=\"ace-icon {IconBefore}\"></i>");
                builder.Append(Content);
                builder.Append($" <span class=\"ace-icon {IconAfter} icon-on-right\"></span>");
                //builder.Append("</button>");
            }
            output.Content.SetHtmlContent(builder.ToString());
            
            return base.ProcessAsync(context, output);
        }
    }
    public enum ButtonTag
    {
        button, link
    }
}
