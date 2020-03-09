using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.MultiLanguage;
using Fap.Core.Rbac;

namespace Fap.AspNetCore.Controls.JqGrid
{
    public static class GridHelper
    {
        public static Grid Grid(this HtmlHelper helper, IDbContext dataAccessor, IRbacService  rbacService, IFapApplicationContext applicationContext, IMultiLangService multiLang, string id)
        {
            return new Grid(dataAccessor, rbacService, applicationContext, multiLang, id);
        }
    }
    
}
