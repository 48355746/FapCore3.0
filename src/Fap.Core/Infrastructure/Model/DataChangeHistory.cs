using Fap.Core.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Model
{
    /// <summary>
    /// 数据变化类
    /// </summary>
    public class DataChangeHistory
    {
        /// <summary>
        /// 数据名称
        /// </summary>
        public string DataName { get; set; }
        /// <summary>
        /// 数据变化时间
        /// </summary>
        public string ChangeDateTime { get; set; }
        /// <summary>
        /// 处理人
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// 数据项变化集合
        /// </summary>
        public List<DataItemChangeHistory> ItemChangeList { get; set; }

        /// <summary>
        /// 数据变化类型
        /// </summary>
        public DataChangeTypeEnum ChangeType { get; set; }

        public string Message
        {
            get
            {
                StringBuilder msgBuilder = new StringBuilder();
                if (ChangeType == DataChangeTypeEnum.ADD)
                {
                    msgBuilder.Append("新增一条").Append(DataName).Append("记录");
                }
                else if (ChangeType == DataChangeTypeEnum.DELETE)
                {
                    msgBuilder.Append("删除一条").Append(DataName).Append("记录");
                }
                else if (ChangeType == DataChangeTypeEnum.UPDATE)
                {
                    if (ItemChangeList != null)
                    {
                        foreach (DataItemChangeHistory item in ItemChangeList)
                        {
                            msgBuilder.Append(item.ItemName).Append(item.ChangeMessage).Append(",");
                        }
                    }
                }
                return msgBuilder.ToString().TrimEnd(',');
            }
        }
    }

    /// <summary>
    /// 数据项变化类
    /// </summary>
    public class DataItemChangeHistory
    {
        /// <summary>
        /// 数据项名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 数据项变化的信息
        /// </summary>
        public string ChangeMessage { get; set; }

    }
}
