using Fap.AspNetCore.Controls.DataForm;
using Fap.AspNetCore.ViewModel;
using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Query;
using Fap.Core.MultiLanguage;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Fap.AspNetCore.Controls.TagHelpers
{
    public class FapFreeformTagHelper : TagHelper
    {
        private IDbContext _dataAccessor;
        private ILoggerFactory _loggerFactory;
        private IFapApplicationContext _session;
        private IMultiLangService _multiLang;
        private IFapPlatformDomain _appDomain;
        public FapFreeformTagHelper(IDbContext dataAccessor, ILoggerFactory loggerFactory, IFapPlatformDomain platformDomain, IFapApplicationContext session, IMultiLangService multiLang)
        {
            _dataAccessor = dataAccessor;
            _loggerFactory = loggerFactory;
            _session = session;
            _multiLang = multiLang;
            _appDomain = platformDomain;
        }
        /// <summary>
        /// 控件ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 默认值设置
        /// </summary>
        public Dictionary<string, string> DefaultData { get; set; }
        /// <summary>
        /// 子表默认值集合
        /// </summary>
        public IEnumerable<SubTableDefaultValue> SubtablelistDefaultdata { get; set; }
        /// <summary>
        /// 查询设置
        /// </summary>
        public SimpleQueryOption QueryOption { get; set; }
        /// <summary>
        /// 单据状态
        /// </summary>        
        public FormStatus FormStatus { get; set; } = FormStatus.Add;
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
            string id = "jqgriddataform";
            if (Id.IsPresent())
            {
                id = Id;
            }
            FapFreeForm form = new FapFreeForm(_dataAccessor, _loggerFactory, _appDomain, _session, _multiLang, id);
            if (FormTemplate.IsPresent())
            {
                form.SetFromTemplate(FormTemplate);
            }
            if (this.FormStatus != FormStatus.Add)
            {
                form.SetFormStatus(this.FormStatus);
            }
            if (DefaultData != null && DefaultData.Count > 0)
            {
                form.SetCustomDefaultData(DefaultData);
            }
            if (SubtablelistDefaultdata != null && SubtablelistDefaultdata.Any())
            {
                form.SetSubTableListDefualtData(SubtablelistDefaultdata);
            }
            if (QueryOption != null)
            {
                form.SetQueryOption(QueryOption);
            }
            if (this.GridReadonly)
            {
                form.SetGridReadonly(this.GridReadonly);
            }
            output.Content.AppendHtml(form.ToString());

        }
    }
}
