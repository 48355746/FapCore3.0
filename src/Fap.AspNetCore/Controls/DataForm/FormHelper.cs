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
        /// X-Editable表单
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static XEditableForm XEditableForm(this HtmlHelper helper, IDbContext dataAccessor, IFapApplicationContext applicationContext, IMultiLangService multiLangService, IRbacService rbacService, string id)
        {
            return new XEditableForm(applicationContext,  dataAccessor, multiLangService, rbacService);
        }
       
    } 
    
}
