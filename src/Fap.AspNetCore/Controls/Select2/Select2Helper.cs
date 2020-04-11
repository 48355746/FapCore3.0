using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Fap.Core.DataAccess;

namespace Fap.AspNetCore.Controls
{
    /// <summary>
    /// Select2
    /// 引用select2.css，select2.js
    /// </summary>
    public static class Select2Helper
    {
        public static Select2 Select2(this HtmlHelper helper, IDbContext db, string id)
        {
            return new Select2(db, id);
        }
    }
    ////@(Html.Select2("sel-processState").SetPlaceholder("流程状态").SetWidth(150).SetSelect2Mode(new Select2Model { IdField = "Code", NameField = "Name", TableName = "FapDict", Where = "Category='WfProcessState'" }))
   
}
