using Fap.Core.Annex.Utility.Events;
using System.IO;

namespace Fap.Core.Annex
{
    /// <summary>
    /// 文件服务器接口(对内使用)
    /// </summary>
    internal interface IFileService
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileInfo"></param>
        /// <param name="updateEvent"></param>
        /// <returns></returns>
        bool Upload(Stream stream, FapFileInfo fileInfo, FileUploadEventHandler updateEvent);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        Stream Download(FapFileInfo fileInfo);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        bool Delete(FapFileInfo fileInfo);
    }
}
