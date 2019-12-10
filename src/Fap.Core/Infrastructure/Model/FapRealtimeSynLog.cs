using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Model
{
    /// <summary>
    /// 实时同步日志
    /// </summary>
    public class FapRealtimeSynLog : BaseModel
    {
        /// <summary>
        /// 同步分类
        /// </summary>
        public string SynType { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public string SynOper { get; set; }
        /// <summary>
        /// 同步状态
        /// </summary>
        public int SynState { get; set; }
        /// <summary>
        /// 同步系统
        /// </summary>
        public string RemoteSystem { get; set; }
        /// <summary>
        /// 同步地址
        /// </summary>
        public string RemoteUrl { get; set; }
        /// <summary>
        /// 数据内容
        /// </summary>
        public string SynData { get; set; }
        /// <summary>
        /// 同步日志
        /// </summary>
        public string SynLog { get; set; }

    }
}
