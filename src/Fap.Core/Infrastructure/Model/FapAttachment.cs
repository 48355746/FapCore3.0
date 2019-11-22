using Fap.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Model
{
    public class FapAttachment : BaseModel
    {
        /// <summary>
        /// 业务标识
        /// </summary>
        public string Bid { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 扩展名
        /// </summary>
        public string FileExtension { get; set; }
        /// <summary>
        /// 大小
        /// </summary>
        public long FileSize { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// 文件内容
        /// </summary>
        public byte[] FileContent { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string FileIcon { get; set; }
        /// <summary>
        /// 文件保存模式
        /// </summary>
        public string SaveModel { get; set; }
    }
}
