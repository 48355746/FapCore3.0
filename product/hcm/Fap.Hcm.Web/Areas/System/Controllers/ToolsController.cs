using System.Collections.Generic;
using Fap.Core.Rbac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using System;
using Fap.Core.Infrastructure.Domain;

namespace Fap.Hcm.Web.Areas.System.Controllers
{
    [Area("System")]
    public class ToolsController : FapController
    {
        public ToolsController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// 数据实体
        /// </summary>
        /// <returns></returns>
        public IActionResult DataEntity()
        {
            JqGridViewModel model = this.GetJqGridModel("FapTable");
            FormViewModel fm = new FormViewModel();
            fm.QueryOption = new Core.Infrastructure.Query.QuerySet()
            {
                TableName = "Employee",
                QueryCols = "*",
                InitWhere = "Fid='c4a711e5aee22b59bbe9'"
            };
            ViewBag.FormModel = fm;
            return View(model);
        }
        /// <summary>
        ///数据属性
        /// </summary>
        /// <returns></returns>
        public IActionResult DataEntityToDataProperty()
        {
            string tableName = Request.Query["tn"];

            JqGridViewModel model = this.GetJqGridModel("FapColumn", (q) =>
              {
                  q.InitWhere = "TableName=@TableName";
                  q.AddParameter("TableName", tableName);
              });

            return View(model);
        }
        /// <summary>
        /// 表定义
        /// </summary>
        /// <returns></returns>
        public IActionResult TableDefine()
        {
            FormViewModel model = new FormViewModel();
            model.QueryOption = new Core.Infrastructure.Query.QuerySet()
            {
                TableName = "Employee",
                QueryCols = "*",
                InitWhere = "Fid='c4a711e5aee22b59bbe9'"
            };
            return View(model);

        }
        /// <summary>
        /// 字典表
        /// </summary>
        /// <returns></returns>
        public IActionResult Dictionary()
        {
            JqGridViewModel model = GetJqGridModel("FapDict");
            return View(model);
        }
        /// <summary>
        /// 员工照片批量导入导出
        /// </summary>
        /// <returns></returns>
        public IActionResult EmpPhotoImpExp()
        {
            return View();
        }
    }
}