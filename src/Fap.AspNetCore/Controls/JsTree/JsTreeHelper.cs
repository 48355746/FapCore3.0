using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Rbac;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Fap.AspNetCore.Controls
{
    public static class JsTreeHelper
    {
        public static JsTree Tree(this HtmlHelper helper, IDbContext dataAccessor,  IRbacService rbacService,IFapApplicationContext applicationContext, string id)
        {
            return new JsTree(dataAccessor, rbacService, applicationContext,id);
        }
    }
   
}
