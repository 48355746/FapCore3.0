using Fap.Core.MultiLanguage;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Fap.AspNetCore.Controls.TagHelpers
{
    public class FapMultilangTagHelper : TagHelper
    {
        private readonly IMultiLangService _multiLangService;
        /// <summary>
        /// 多语言键
        /// </summary>
        public string LangKey { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultContent { get; set; }
        public FapMultilangTagHelper(IMultiLangService multiLangService)
        {
            _multiLangService = multiLangService;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "";
            output.Content.Clear();
            string content = _multiLangService.GetOrAndMultiLangValue(MultiLanguageOriginEnum.MultiLangTag, LangKey, DefaultContent);
            output.Content.Append(content);

        }

    }
}
