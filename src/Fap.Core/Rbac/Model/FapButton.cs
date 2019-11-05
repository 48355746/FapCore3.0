using Fap.Core.MetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Rbac.Model
{
    public class FapButton:BaseModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string BtnCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string BtnName { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
    }
}
