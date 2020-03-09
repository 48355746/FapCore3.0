using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;

namespace Fap.AspNetCore.Controls.TagHelpers
{
    public class FapSelectTagHelper : TagHelper
    {
        private readonly IDbContext _dataAccessor;
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Placeholder
        /// </summary>
        public string Placeholder { get; set; }
        public int Width { get; set; }
        /// <summary>
        /// 模型
        /// </summary>
        public Select2Model SelectModel { get; set; }
        /// <summary>
        /// 是否多选
        /// </summary>
        public bool IsMulti { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// chang事件
        /// </summary>
        public string OnChange { get; set; }
        /// <summary>
        /// 数据源
        /// </summary>
        public List<Select2DataItem> SourceList { get; set; }
        public FapSelectTagHelper(IDbContext dataAccessor)
        {
            _dataAccessor = dataAccessor;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Content.Clear();
            string id = "fapselect";
            if (Id.IsPresent())
            {
                id = $"sel-{Id}";
            }
            Select2 select2 = new Select2(_dataAccessor, id);
            if (Placeholder.IsPresent())
            {
                select2.SetPlaceholder(Placeholder);
            }
            if (SourceList != null)
            {
                select2.SetDataSource(SourceList);
            }
            if (SelectModel != null)
            {
                select2.SetSelect2Mode(SelectModel);
            }
            if (IsMulti)
            {
                select2.SetMultiSelectAble(IsMulti);
            }
            if (DefaultValue.IsPresent())
            {
                select2.SetDefaultValue(DefaultValue);
            }
            if (OnChange.IsPresent())
            {
                select2.SetChangeEvent(OnChange);
            }
            if (Width > 0)
            {
                select2.SetWidth(Width);
            }
            output.Content.AppendHtml(select2.ToString());

        }

    }
}
