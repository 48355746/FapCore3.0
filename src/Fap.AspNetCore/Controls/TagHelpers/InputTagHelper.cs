using Fap.Core.MultiLanguage;
using Fap.Core.Rbac;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Fap.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.AspNetCore.Controls.TagHelpers
{
    public class InputTagHelper:TagHelper
    {
        private readonly IRbacService _rbacService;
        private readonly IMultiLangService _multiLangService;
        public InputTagHelper(IRbacService rbacService, IMultiLangService multiLangService)
        {
            _rbacService = rbacService;
            _multiLangService = multiLangService;
        }
        public string MultiPlaceholder { get; set; }
        public bool FapChecked { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (MultiPlaceholder.IsPresent())
            {
                string id = output.Attributes["id"].Value?.ToString()?? (output.Attributes["name"].Value?.ToString()??"");
                string langkey = _rbacService.GetCurrentMenu()?.Fid??"" + "_" + id;
                string placeholder = _multiLangService.GetOrAndMultiLangValue(MultiLanguageOriginEnum.MultiLangTag, langkey, MultiPlaceholder);
                output.Attributes.Add("placeholder", placeholder);

            }
            if (FapChecked&&(output.Attributes["type"].Value?.ToString()??"").EqualsWithIgnoreCase("checkbox"))
            {
                output.Attributes.Add("checked", "checked");
            }
            base.Process(context, output);
        }
    }
}
