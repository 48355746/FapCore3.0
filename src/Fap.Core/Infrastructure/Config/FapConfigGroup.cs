using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Config
{
    /// <summary>
    /// 配置组
    /// </summary>
    public class FapConfigGroup : BaseModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string CfCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string CfName { get; set; }
        /// <summary>
        /// 父Fid
        /// </summary>
        public string Pid { get; set; }
        /// <summary>
        /// 配置模块
        /// </summary>
        public string CfModule { get; set; }
        /// <summary>
        /// 配置模块 的显性字段MC
        /// </summary>
        [Computed]
        public string CfModuleMC { get; set; }

    }
}
