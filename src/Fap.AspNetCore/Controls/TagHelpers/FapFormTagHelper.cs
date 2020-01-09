using Fap.AspNetCore.Controls.DataForm;
using Fap.AspNetCore.ViewModel;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Query;
using Fap.Core.Rbac;
using Fap.Core.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Fap.Core.MultiLanguage;
using Fap.Core.Infrastructure.Domain;
using Ardalis.GuardClauses;

namespace Fap.AspNetCore.Controls.TagHelpers
{
    public class FapFormTagHelper : TagHelper
    {
        private IServiceProvider _serviceProvider;
        public FapFormTagHelper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        /// <summary>
        /// 控件ID
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// 子表默认值集合
        /// </summary>
        //public IEnumerable<SubTableDefaultValue> SubtablelistDefaultdata { get; set; }
        /// <summary>
        /// 查询设置
        /// </summary>
        public FormViewModel FormModel { get; set; }

        public FormStatus FormStatus { get; set; } = FormStatus.Add;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Content.Clear();
            Guard.Against.NullOrWhiteSpace(Id, nameof(Id));
            FapForm form = new FapForm(serviceProvider:_serviceProvider, Id);
            if (this.FormStatus != FormStatus.Add)
            {
                form.SetFormStatus(this.FormStatus);
            }
            if (FormModel.DefaultData != null && FormModel.DefaultData.Count > 0)
            {
                form.SetCustomDefaultData(FormModel.DefaultData);
            }
            if (FormModel.SubDefaultDataList!= null && FormModel.SubDefaultDataList.Any())
            {
                form.SetSubTableListDefualtData(FormModel.SubDefaultDataList);
            }
            if (FormModel.QueryOption != null)
            {
                form.SetQueryOption(FormModel.QueryOption);
            }

            output.Content.AppendHtml(form.ToString());

        }
    }
}
