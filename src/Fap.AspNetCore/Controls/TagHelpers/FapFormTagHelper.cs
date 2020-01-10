using Fap.AspNetCore.Controls.DataForm;
using Fap.AspNetCore.ViewModel;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Linq;
using Fap.Core.Extensions;

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

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Content.Clear();

            if (Id.IsMissing())
            {
                Id = FormModel.QueryOption.TableName;
            }
            
            FapForm form = new FapForm(serviceProvider:_serviceProvider, Id);
            if (FormModel.FormStatus== FormStatus.View)
            {
                form.SetFormStatus(FormStatus.View);
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
