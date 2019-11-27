using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Model
{
    /// <summary>
    /// 站内消息
    /// </summary>
    public class FapMessage :BaseModel
    {
        /// <summary>
        /// 发送人
        /// </summary>
        public string SEmpUid { get; set; }
        /// <summary>
        /// 发送人 的显性字段MC
        /// </summary>
        [Computed]
        public string SEmpUidMC { get; set; }
        /// <summary>
        /// 接收人
        /// </summary>
        public string REmpUid { get; set; }
        /// <summary>
        /// 接收人 的显性字段MC
        /// </summary>
        [Computed]
        public string REmpUidMC { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string MsgContent { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public string SendTime { get; set; }
        /// <summary>
        /// 全局广播
        /// </summary>
        public int IsGlobal { get; set; }
        /// <summary>
        /// 已读
        /// </summary>
        public int HasRead { get; set; }
        /// <summary>
        /// 消息类型,默认为通知Notice，
        /// 站内信：Message
        /// </summary>
        public string MsgCategory { get; set; }
        /// <summary>
        /// 消息类型 的显性字段MC
        /// </summary>
        [Computed]
        public string MsgCategoryMC { get; set; }
        /// <summary>
        /// 业务链接
        /// </summary>
        public string URL { get; set; }

    }
}
