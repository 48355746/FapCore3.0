using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Workflow.Model
{
    /// <summary>
    /// 流程实体
    /// </summary>
    public class ProcessEntity
    {
        public int ID { get; set; }
        /// <summary>
        /// 流程Uid
        /// </summary>
        public string ProcessUid { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// 正在使用
        /// </summary>
        public bool IsUsing { get; set; }
        /// <summary>
        /// 流程分类
        /// </summary>
        public string AppType { get; set; }
        /// <summary>
        /// 应用地址
        /// </summary>
        public string PageUrl { get; set; }
        /// <summary>
        /// xml文件名
        /// </summary>
        public string XmlFileName { get; set; }
        /// <summary>
        /// xml文件路径
        /// </summary>
        public string XmlFilePath { get; set; }
        /// <summary>
        /// xml文件内容
        /// </summary>
        public string XmlContent { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 业务数据
        /// </summary>
        public dynamic BizData { get; set; }
      
    }
}
