
namespace Fap.Core.Annex
{
    /// <summary>
    /// 文件信息对象
    /// </summary>
    public class FapFileInfo
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public long? FileId
        {
            get;
            set;
        }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName
        {
            get;
            set;
        }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath
        {
            get;
            set;
        }
        /// <summary>
        /// 文件大小
        /// </summary>
        public long FileSize
        {
            get;
            set;
        }
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string FileSuffix
        {
            get;
            set;
        }
        ///// <summary>
        ///// 文件长度
        ///// </summary>
        //public long FileLength
        //{
        //    get;
        //    set;
        //}
        ///// <summary>
        ///// 文件流
        ///// </summary>
        //public Stream FileStream
        //{
        //    get;
        //    set;
        //}
    }
}
