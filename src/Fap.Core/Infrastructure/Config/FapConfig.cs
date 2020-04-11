using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Config
{
    /// <summary>
    /// 配置项
    /// </summary>
    [Serializable]
    public class FapConfig : BaseModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string ParamName { get; set; }
        /// <summary>
        /// 键
        /// </summary>
        public string ParamKey { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string ParamValue { get; set; }
        /// <summary>
        /// 配置组
        /// </summary>
        public string ConfigGroup { get; set; }
        /// <summary>
        /// 配置组 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string ConfigGroupMC { get; set; }
        /// <summary>
        /// 控件类型
        /// </summary>
        public string CtrlType { get; set; }
        /// <summary>
        /// 控件类型 的显性字段MC
        /// </summary>
        [ComputedAttribute]
        public string CtrlTypeMC { get; set; }
        /// <summary>
        /// 关联组件
        /// </summary>
        public string AssoComponent { get; set; }
        /// <summary>
        /// 关联组件 的显性字段MC
        /// </summary>
        [Computed]
        public string AssoComponentMC { get; set; }
        /// <summary>
        /// 启用
        /// </summary>
        public int Enabled { get; set; }
        /// <summary>
        /// 参数组
        /// </summary>
        public string ParamGroup { get; set; }
        /// <summary>
        /// 编码项
        /// </summary>
        public string DictList { get; set; }
        /// <summary>
        /// 编码数据源
        /// </summary>
        [Computed]
        public List<SelectModel> DictSource { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public int SortBy { get; set; }

    }
    public class SelectModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
    }
    /// <summary>
    /// 分组后的配置
    /// </summary>
    public class GrpConfig
    {
        public string Grp { get; set; }
        public List<FapConfig> Configs { get; set; }
    }
}
