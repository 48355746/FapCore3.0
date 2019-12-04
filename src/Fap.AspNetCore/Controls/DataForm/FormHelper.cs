using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.MultiLanguage;
using Fap.Core.Rbac;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System;

namespace Fap.AspNetCore.Controls.DataForm
{
    public static class FormHtmlHelper
    {
        /// <summary>
        /// Fap表单
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static FapForm DataForm(this HtmlHelper helper, IServiceProvider serviceProvider, string id, FormStatus formStatus = FormStatus.Add)
        {
            return new FapForm(serviceProvider, id, formStatus);
        }
        /// <summary>
        /// X-Editable表单
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static XEditableForm XEditableForm(this HtmlHelper helper, IDbContext dataAccessor, IFapApplicationContext applicationContext, IMultiLangService multiLangService, IRbacService rbacService, string id)
        {
            return new XEditableForm(applicationContext,  dataAccessor, multiLangService, rbacService);
        }
        /// <summary>
        /// 自由表单
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static FapFreeForm FreeForm(this HtmlHelper helper, IDbContext dataAccessor,ILoggerFactory loggerFactory,IFapPlatformDomain platformDomain, IFapApplicationContext applicationContext, IMultiLangService multiLangService, string id, FormStatus formStatus = FormStatus.Add)
        {
            return new FapFreeForm(dataAccessor,loggerFactory, applicationContext, multiLangService, id, formStatus);
        }
    } 
    
}
