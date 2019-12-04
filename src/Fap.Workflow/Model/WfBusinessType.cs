using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Workflow.Model
{
    /// <summary>
    /// 流程业务分类
    /// </summary>
    public class WfBusinessType : BaseModel
    {
        /// <summary>
        /// 分类名
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 分类编号
        /// </summary>
        public string TypeCode { get; set; }
        /// <summary>
        /// 父分类
        /// </summary>
        public string Pid { get; set; }
        /// <summary>
        /// 父分类 的显性字段MC
        /// </summary>
        [Computed]
        public string PidMC { get; set; }
        /// <summary>
        /// 系统预置
        /// </summary>
        public int IsSystem { get; set; }
        /// <summary>
        /// 单据
        /// </summary>
        public string BillTable { get; set; }
        /// <summary>
        /// 单据 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string BillTableMC { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 颜色
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 所属模块
        /// </summary>
        public string ModuleUid { get; set; }
        /// <summary>
        /// 所属模块 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string ModuleUidMC { get; set; }
        /// <summary>
        /// 人员类别
        /// </summary>
        public string EmpCategory { get; set; }

    }

}
