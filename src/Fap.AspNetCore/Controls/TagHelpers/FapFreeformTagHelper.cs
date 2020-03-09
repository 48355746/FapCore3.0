using Fap.AspNetCore.Controls.DataForm;
using Fap.AspNetCore.ViewModel;
using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Query;
using Fap.Core.MultiLanguage;
using Fap.Core.Rbac;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.AspNetCore.Controls.TagHelpers
{
    public class FapFreeformTagHelper : TagHelper
    {
        private IServiceProvider _serviceProvider;
        public FapFreeformTagHelper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        /// <summary>
        /// 控件ID
        /// </summary>
        public string Id { get; set; } = "freeform";
        /// <summary>
        /// 默认值设置
        /// </summary>
        //public Dictionary<string, string> DefaultData { get; set; }
        /// <summary>
        /// 子表默认值集合
        /// </summary>
        //public IEnumerable<SubTableDefaultValue> SubtablelistDefaultdata { get; set; }
        /// <summary>
        /// 查询设置
        /// </summary>
        public FormViewModel FormModel { get; set; }
        /// <summary>
        /// 单据状态
        /// </summary>        
        //public FormStatus FormStatus { get; set; } = FormStatus.Add;
        /// <summary>
        /// 表单模板
        /// </summary>
        public string FormTemplate { get; set; }
        /// <summary>
        /// 子表状态
        /// </summary>
        public bool GridReadonly { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Content.Clear();
            BaseForm form = new FapFreeForm(serviceProvider: _serviceProvider, Id);
            if (FormTemplate.IsPresent())
            {
                form.SetFromTemplate(FormTemplate);
            }
            if (FormModel.FormStatus != FormStatus.Add)
            {
                form.SetFormStatus(FormModel.FormStatus);
            }
            if (FormModel.DefaultData != null && FormModel.DefaultData.Count > 0)
            {
                form.SetCustomDefaultData(FormModel.DefaultData);
            }
            if (FormModel.SubDefaultDataList != null && FormModel.SubDefaultDataList.Any())
            {
                form.SetSubTableListDefualtData(FormModel.SubDefaultDataList);
            }
            if (FormModel.QueryOption != null)
            {
                form.SetQueryOption(FormModel.QueryOption);
            }
            if (this.GridReadonly)
            {
                form.SetGridReadonly(this.GridReadonly);
            }
            output.Content.AppendHtml(form.ToString());

        }
    }
}
