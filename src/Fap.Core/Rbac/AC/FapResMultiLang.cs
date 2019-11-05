using System;
using Dapper.Contrib.Extensions;
using Fap.Core.MetaData;

namespace Fap.Core.Rbac.Model
{
    /// <summary>
    /// 资源多语
    /// </summary>
    [Serializable]
    public class FapResMultiLang : BaseModel
    {
        /// <summary>
        /// 资源编号
        /// </summary>
        public string ResCode { get; set; }
        /// <summary>
        /// 中文简体
        /// </summary>
        public string ResZhCn { get; set; }
        /// <summary>
        /// 中文繁体
        /// </summary>
        public string ResZhTW { get; set; }
        /// <summary>
        /// 英文
        /// </summary>
        public string ResEn { get; set; }
        /// <summary>
        /// 日语
        /// </summary>
        public string ResJa { get; set; }

    }
}
