using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Tracker
{
    public class SynchResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 是否异常的远程地址
        /// </summary>
        public bool IsExceptionRemoteAddress { get; set; }
        /// <summary>
        /// 尝试次数
        /// </summary>
        public int TryNumber { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrMsg { get; set; }
    }
}
