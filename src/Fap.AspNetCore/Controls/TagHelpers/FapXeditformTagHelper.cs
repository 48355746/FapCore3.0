using Fap.Core.Extensions;
using Fap.AspNetCore.Controls.DataForm;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Query;
using Fap.Core.MultiLanguage;
using Fap.Core.Rbac;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Fap.AspNetCore.Controls.TagHelpers
{
    public class FapXeditformTagHelper : TagHelper
    {
        private IDbContext _dataAccessor;
        private ILogger<FapXeditformTagHelper> _logger;
        private IFapPlatformDomain _appDomain;
        private IFapApplicationContext _applicationContext;
        private IMultiLangService _multiLang;
        private IRbacService _rbacService;
        public FapXeditformTagHelper(IDbContext dataAccessor,  ILoggerFactory logger, IFapPlatformDomain appDomain, IFapApplicationContext applicationContext, IMultiLangService multiLang, IRbacService rbacService)
        {
            _dataAccessor = dataAccessor;
            _logger = logger.CreateLogger<FapXeditformTagHelper>();
            _appDomain = appDomain;
            _applicationContext = applicationContext;
            _multiLang = multiLang;
            _rbacService = rbacService;

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
        /// 查询设置
        /// </summary>
        public SimpleQueryOption QueryOption { get; set; }

        public XEditableSaveModel SaveModel { get; set; } = XEditableSaveModel.Single;
        public XEditableFormModel FormModel { get; set; } = XEditableFormModel.Inline;
        public bool EditEnabed { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Content.Clear();
            string id = "jqgriddataform";
            if (Id.IsPresent())
            {
                id = Id;
            }
            XEditableForm form = new XEditableForm( _applicationContext,_appDomain,_dataAccessor, _multiLang, _rbacService);
            if (QueryOption != null)
            {
                form.SetQueryOption(QueryOption);
            }
            form.SetSaveModel(SaveModel);
            form.SetFormModel(FormModel);
            form.SetEditEnabled(EditEnabed);
            output.Content.AppendHtml(form.ToString());

        }
    }
}
