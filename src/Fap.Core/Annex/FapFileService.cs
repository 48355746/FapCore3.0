using Dapper;
using Fap.Core.Annex.Database;
using Fap.Core.Annex.FileDirectory;
using Fap.Core.Annex.Ftp;
using Fap.Core.Annex.Utility.TempFile;
using Fap.Core.Annex.Utility.Zip;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Exceptions;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Config;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Model;
using Fap.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fap.Core.Annex
{
    /// <summary>
    /// Fap文件服务提供者
    /// </summary>
    [Service(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Scoped)]
    public class FapFileService : IFapFileService
    {
        /// <summary>
        /// 文件目录
        /// </summary>
        public const string FILESERVICE_FILE = "File";
        /// <summary>
        /// FTP
        /// </summary>
        public const string FILESERVICE_FTP = "Ftp";
        /// <summary>
        /// 数据库
        /// </summary>
        public const string FILESERVICE_DATABASE = "Database";

        private IDbContext _dataAccessor;
        private ILoggerFactory _logger;
        private IFapConfigService _configService;

        private IFileService _fileService;
        private string _fileServiceType = FILESERVICE_FTP; //默认是数据库

        public FapFileService(IDbContext dataAccessor, IFapConfigService configService, ILoggerFactory logger, string fileServiceType = "")
        {
            _dataAccessor = dataAccessor;
            _logger = logger;
            _configService = configService;
            _fileServiceType = fileServiceType;
            _fileService = this.GetFileService();
        }

        ///// <summary>
        ///// 获取当前文件服务的类型, 默认是数据库
        ///// </summary>
        ///// <returns></returns>
        //public string GetCurrentFileServiceType()
        //{
        //    string type = FILESERVICE_DATABASE;
        //    object obj = FapPlatformConfig.PlatformProperties["system.fileservice.type"];
        //    if (obj != null)
        //    {
        //        type = obj.ToString();
        //    }
        //    return type;
        //}

        /// <summary>
        /// 获取文件服务对象
        /// </summary>
        /// <param name="fileServiceType"></param>
        /// <returns></returns>
        IFileService GetFileService()
        {
            if (_fileService == null)
            {
                if (_fileServiceType.IsMissing())
                {
                    _fileServiceType = _configService.GetSysParamValue("system.fileservice.type");
                }

                if (_fileServiceType.Equals(FapFileService.FILESERVICE_FILE, StringComparison.CurrentCultureIgnoreCase))
                {
                    ILogger<FileDirectoryService> log = _logger.CreateLogger<FileDirectoryService>();
                    _fileService = new FileDirectoryService(_dataAccessor, log, _configService);
                }
                else if (_fileServiceType.Equals(FapFileService.FILESERVICE_FTP, StringComparison.CurrentCultureIgnoreCase))
                {
                    ILogger<FtpService> log = _logger.CreateLogger<FtpService>();
                    _fileService = new FtpService(log, _configService);
                }
                else if (_fileServiceType.Equals(FapFileService.FILESERVICE_DATABASE, StringComparison.CurrentCultureIgnoreCase))
                {
                    _fileService = new DatabaseService(_dataAccessor);
                }

                if (_fileService == null) //默认是数据库
                {
                    _fileService = new DatabaseService(_dataAccessor);
                    _fileServiceType = FILESERVICE_DATABASE;
                }
            }

            return _fileService;
        }

        internal IFileService GetFileService(string fileServiceType)
        {
            IFileService fileService = null;
            if (fileServiceType.Equals(FILESERVICE_FILE, StringComparison.CurrentCultureIgnoreCase))
            {
                ILogger<FileDirectoryService> log = _logger.CreateLogger<FileDirectoryService>();
                fileService = new FileDirectoryService(_dataAccessor, log, _configService);
            }
            else if (fileServiceType.Equals(FILESERVICE_FTP, StringComparison.CurrentCultureIgnoreCase))
            {
                ILogger<FtpService> log = _logger.CreateLogger<FtpService>();
                fileService = new FtpService(log, _configService);
            }
            else //if (fileServiceType.Equals(FapFileService.FILESERVICE_DATABASE, StringComparison.CurrentCultureIgnoreCase))
            {
                fileService = new DatabaseService(_dataAccessor);
            }
            return fileService;
        }

        //public bool UploadFile(Stream stream, FapFileInfo fileInfo, Events.FileUploadEventHandler uploadEvent)
        //{
        //    bool success = GetFileService().Upload(stream, fileInfo, uploadEvent);
        //    if (success)
        //    {

        //    }

        //    return false;
        //}

        #region 上传文件
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public string UploadFile(Stream stream, string fileName, string bid)
        {
            return UploadFile(stream, new FapAttachment() { FileName = fileName, Bid = bid });
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">文件全路径</param>
        /// <returns></returns>
        public string UploadFile(string filePath, string bid)
        {
            if (!File.Exists(filePath))
            {
                throw new FapException("文件不存在");
            }

            FileStream stream = new FileStream(filePath, FileMode.Open);
            return UploadFile(stream, new FapAttachment() { FileName = Path.GetFileName(filePath), Bid = bid });
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="attachement"></param>
        /// <returns></returns>
        public string UploadFile(Stream stream, FapAttachment attachement)
        {
            if (stream == null || attachement == null)
            {
                return "";
            }

            FapFileInfo fileInfo = new FapFileInfo();
            fileInfo.FileSize = (attachement.FileSize == 0 ? (stream.Length / 1024) : attachement.FileSize);
            fileInfo.FileName = attachement.FileName;
            //fileInfo.FileId = ""+attachement.Id;
            fileInfo.FileSuffix = (string.IsNullOrWhiteSpace(attachement.FileExtension) ? Path.GetExtension(attachement.FileName) : attachement.FileExtension);

            attachement.FileSize = fileInfo.FileSize;
            attachement.FileExtension = fileInfo.FileSuffix;
            attachement.SaveModel = this._fileServiceType;

            long id = _dataAccessor.Insert<FapAttachment>(attachement);

            fileInfo.FileId = id;

            bool success = GetFileService().Upload(stream, fileInfo, (a, b) =>
            {
                fileInfo.FilePath = b.FileFullName;
            });
            if (success)
            {
                //newAttachment.FilePath = fileInfo.FilePath;
                //dataAccessor.EUpdate<FapAttachment>(attachement, false);
                _dataAccessor.Execute("update FapAttachment set FilePath=@FilePath where Id=@Id", new DynamicParameters(new { FilePath = fileInfo.FilePath, Id = id }));
            }
            else
            {
                _dataAccessor.Delete<FapAttachment>(attachement);
            }

            return attachement.Fid;
        }

        #endregion

        #region 下载文件
        ///// <summary>
        ///// 根据文件路径，下载文件
        ///// </summary>
        ///// <param name="filePath"></param>
        ///// <returns></returns>
        //public Stream DownloadFile(string filePath)
        //{
        //    if (string.IsNullOrWhiteSpace(filePath))
        //    {
        //        return null;
        //    }
        //    return GetFileService().Download(filePath);
        //}

        /// <summary>
        /// 根据Fid，下载文件
        /// </summary>
        /// <param name="fid"></param>
        /// <returns>返回的是文件流</returns>
        public FapAttachment DownloadFileByFid(string fid, out Stream stream)
        {
            stream = null;
            if (string.IsNullOrWhiteSpace(fid))
            {
                return null;
            }

            FapAttachment attachment = _dataAccessor.Get<FapAttachment>(fid);
            if (attachment == null)
            {
                return null;
            }
            stream = this.GetFileStream(attachment);
            return attachment;
        }

        /// <summary>
        /// 根据Bid，下载一个文件
        /// </summary>
        /// <param name="fid"></param>
        /// <returns>返回的是文件流</returns>
        public FapAttachment DownloadOneFileByBid(string bid, out Stream stream)
        {
            stream = null;
            if (string.IsNullOrWhiteSpace(bid))
            {
                return null;
            }

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Bid", bid);
            var attachments = _dataAccessor.QueryWhere<FapAttachment>(" Bid=@Bid", parameters);

            if (attachments.Any())
            {
                var attachment = attachments.OrderByDescending(a => a.Id).First();
                stream = this.GetFileStream(attachment);
                return attachment;
            }
            return null;
        }

        /// <summary>
        /// 下载zip文件委托事件
        /// </summary>
        /// <param name="stream"></param>
        public delegate void DownloadZipEventHandler(Stream stream);

        /// <summary>
        /// 根据Bid，将多个文件打包成ZIP文件
        /// </summary>
        /// <param name="bid"></param>
        /// <param name="handler">委托事件</param>
        public void DownloadZip(string bid, DownloadZipEventHandler handler)
        {
            if (string.IsNullOrWhiteSpace(bid))
            {
                return;
            }
            string sql = "select * from FapAttachment where Bid=@Bid";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Bid", bid);
            var attachments = _dataAccessor.Query<FapAttachment>(sql, parameters);

            this.DownloadZip(attachments, handler);
        }


        /// <summary>
        /// 查询FapAttachment记录，将多个文件打包成ZIP文件
        /// </summary>
        /// <param name="attachmentFids"></param>
        /// <param name="handler">委托事件</param>
        public void DownloadZip(List<string> attachmentFids, DownloadZipEventHandler handler)
        {
            if (attachmentFids == null || attachmentFids.Count == 0)
            {
                return;
            }

            string sql = "select * from FapAttachment where Fid in (" +string.Join(',',attachmentFids.Select(a=>$"'{a}'"))+ ")";
            IEnumerable<FapAttachment> attachments = _dataAccessor.Query<FapAttachment>(sql);

            this.DownloadZip(attachments, handler);
        }

        /// <summary>
        /// 将FapAttachment文件对象的文件，打包成ZIP文件
        /// </summary>
        /// <param name="attachments"></param>
        /// <param name="handler">委托事件</param>
        public void DownloadZip(IEnumerable<FapAttachment> attachments, DownloadZipEventHandler handler)
        {
            if (attachments == null || attachments.Count() == 0)
            {
                return;
            }

            //先创建临时文件夹，以便存放要打包的文件
            string temporaryFolderName =UUIDUtils.Fid;
            string temporaryFolderPath = FileUtility.CreateTemporaryFolder(temporaryFolderName);
            //压缩文件路径
            string zipFilePath = Path.Combine(temporaryFolderPath, temporaryFolderName + ".zip");

            //将文件存放到临时文件夹中，并记录这些文件的路径
            IList<string> filePathListToZip = new List<string>(); //记录要打包的所有文件的路径 
            foreach (var attachment in attachments)
            {
                string fileName = attachment.FileName;
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = UUIDUtils.Fid + attachment.FileExtension;
                }

                string filePath = Path.Combine(temporaryFolderPath, fileName);
                Stream stream = this.GetFileStream(attachment);
                if (stream != null)
                {
                    FileUtility.CreateFileFromStream(filePath, stream);

                    filePathListToZip.Add(filePath);
                }
            }

            //将这些文件打包成ZIP文件，返回zip流
            ZipHelper zipHelper = new ZipHelper();
            zipHelper.ZipMultiFiles(filePathListToZip, zipFilePath);

            Stream zipStream = new FileStream(zipFilePath, FileMode.Open);

            if (handler != null)
            {
                handler(zipStream);
            }


            //删除临时文件夹,不再删除，任务调度删除
            //FileUtility.DeleteFolderFromTemporaryFolder(temporaryFolderName);
        }


        public Stream GetFileStream(FapAttachment attachment)
        {
            FapFileInfo fileInfo = new FapFileInfo();
            fileInfo.FilePath = attachment.FilePath;
            fileInfo.FileId = attachment.Id;

            string fileServiceType = attachment.SaveModel;
            return GetFileService(fileServiceType).Download(fileInfo);
        }

        #endregion

        #region 删除文件
        ///// <summary>
        ///// 根据文件路径，删除文件
        ///// </summary>
        ///// <param name="filePath"></param>
        ///// <returns></returns>
        //public bool DeleteFile(string filePath)
        //{
        //    if (string.IsNullOrWhiteSpace(filePath))
        //    {
        //        return false;
        //    }
        //    return GetFileService().Delete(filePath);
        //}

        /// <summary>
        /// 根据Fid，删除文件
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public bool DeleteFileByFid(string fid)
        {
            if (string.IsNullOrWhiteSpace(fid))
            {
                return false;
            }

            FapAttachment attachment = _dataAccessor.Get<FapAttachment>(fid);
            if (attachment == null)
            {
                return false;
            }

            try
            {
                FapFileInfo fileInfo = new FapFileInfo();
                fileInfo.FilePath = attachment.FilePath;
                fileInfo.FileId = attachment.Id;

                string fileServiceType = attachment.SaveModel;
                bool deleted = GetFileService(fileServiceType).Delete(fileInfo);
                if (deleted)
                {
                    _dataAccessor.DeleteExec("FapAttachment", "Id=" + attachment.Id);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.CreateLogger<FapFileService>().LogError(ex.Message);
            }

            return false;
        }

        #endregion


        #region 共用方法
        /// <summary>
        /// 将流转为为文件
        /// </summary>
        /// <param name="stream">字节流</param>
        /// <param name="fileName">文件名</param>
        public void StreamToFile(Stream stream, string fileName)
        {
            // 把 Stream 转换成 byte[] 
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            // 把 byte[] 写入文件 
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(bytes);
            bw.Close();
            fs.Close();
        }
        /// <summary>
        /// 获取所有的附件列表
        /// </summary>
        /// <param name="bid"></param>
        /// <returns></returns>
        public IEnumerable<FapAttachment> FileList(string bid)
        {
            return _dataAccessor.QueryWhere<FapAttachment>("Bid=@Bid", new DynamicParameters(new { Bid = bid }));
        }

        #endregion
    }
}
