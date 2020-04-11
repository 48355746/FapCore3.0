using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UEditorNetCore;

namespace Fap.Hcm.Web.Areas.System.Controllers
{
    /// <summary>
    /// 百度富文本编辑器
    /// </summary>
    [Area("System")]
    [Route("{area}/[controller]")] //配置路由//配置路由
    public class UEditorController : Controller
    {
        private UEditorService ue;
        public UEditorController(UEditorService ue)
        {
            this.ue = ue;
        }

        public void Do()
        {
            ue.DoAction(HttpContext);
        }
    }
}