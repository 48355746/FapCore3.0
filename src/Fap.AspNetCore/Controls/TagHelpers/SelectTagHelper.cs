using Microsoft.AspNetCore.Razor.TagHelpers;
using Fap.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Fap.Core.Rbac;
using Fap.Core.MultiLanguage;

namespace Fap.AspNetCore.Controls.TagHelpers
{
    public class SelectTagHelper : TagHelper
    {
        private readonly IRbacService _rbacService;
        private readonly IMultiLangService _multiLangService;
        public SelectTagHelper(IRbacService rbacService,IMultiLangService multiLangService)
        {
            _rbacService = rbacService;
            _multiLangService = multiLangService;
        }
        public string MultiPlaceholder { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (MultiPlaceholder.IsPresent())
            {
                string id= output.Attributes["id"].Value?.ToString() ?? (output.Attributes["name"].Value?.ToString() ?? "");
                string langkey= id;
                string placeholder= _multiLangService.GetOrAndMultiLangValue(MultiLanguageOriginEnum.MultiLangTag, langkey, MultiPlaceholder);
                output.Attributes.Add("data-placeholder", placeholder);

            }
            base.Process(context, output);
        }
    }
}
