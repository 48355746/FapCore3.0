using Fap.Core.Infrastructure.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fap.Hcm.Web.Models
{
    public class GridViewModel
    {
        public string TableLabel { get; set; }
        public string TableName { get; set; }
        public string Cols { get; set; }
        public string Condition { get; set; }
        /// <summary>
        /// 初始化表单默认值
        /// </summary>
        public List<DefaultValue> DefaultValues { get; set; } = new List<DefaultValue>();
    }
    //public class MultiGridViewModel
    //{
    //    public List<GridViewModel> gridViewModels;
    //}
}
