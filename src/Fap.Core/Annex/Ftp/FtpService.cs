using Fap.Core.Annex.Utility.Events;
using FluentFTP;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Config;

namespace Fap.Core.Annex.Ftp
{
    public class FtpService : IFileService
    {        
        private string host = "localhost";
        private string username = "";
        private string password = "";
        private ILogger<FtpService> _logger;
        private IFapConfigService _configService;
        /// <summary>
        /// ftp存放的根目录
        /// </summary>
        private string ftpRootPath = "fap";

        public FtpService(ILogger<FtpService> logger, IFapConfigService configService)
        {
            _logger = logger;
            _configService = configService;
            host = _configService.GetSysParamValue("ftp.path");
            username = _configService.GetSysParamValue("ftp.username");
            password = _configService.GetSysParamValue("ftp.password");
            ftpRootPath = _configService.GetSysParamValue("ftp.directory.root");
            

        }

        public bool Upload(Stream stream, FapFileInfo fileInfo, FileUploadEventHandler updateEvent)
        {
            if (fileInfo == null || stream == null)
            {
                return false;
            }
            FtpStrategy strategy = new FtpStrategy();
           
            string lv = _configService.GetSysParamValue("ftp.directory.level");
            if (lv.IsPresent())
            {
                strategy.Level = lv.ToInt();
            }
            else
            {
                strategy.Level = 3;
            }
            string fileName = "";
            string directoryPath = strategy.GetFullPath("" + fileInfo.FileId, out fileName);
            string destPath_to_save =Path.Combine(ftpRootPath,directoryPath);
            string filePath_to_save =Path.Combine(destPath_to_save , fileName + fileInfo.FileSuffix);
            try
            {
                using (FtpClient conn = new FtpClient())
                {
                    conn.Host = host;
                    conn.Credentials = new NetworkCredential(username, password);
                    conn.Connect();
                    if (!conn.DirectoryExists(destPath_to_save))
                    {
                        conn.CreateDirectory(destPath_to_save);
                    }

                    using (Stream outStream = conn.OpenWrite(filePath_to_save))
                    {
                        using (var inStream = stream)
                        {
                            stream.CopyTo(outStream);

                            if (updateEvent != null)
                            {
                                updateEvent(fileInfo, new FileUploadEventArgs()
                                {
                                    Total = inStream.Length,
                                    Uploaded = inStream.Length,
                                    FileFullName =Path.Combine(directoryPath,fileName + fileInfo.FileSuffix),
                                    FileName = fileName + fileInfo.FileSuffix,
                                    FileDirectory = directoryPath
                                });
                            }
                        }
                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return false;
        }

        public Stream Download(FapFileInfo fileInfo)
        {
            string fileName =Path.Combine(ftpRootPath,fileInfo.FilePath);

            using (FtpClient conn = new FtpClient())
            {
                conn.Host = host;
                conn.Credentials = new NetworkCredential(username, password);
                conn.Connect();
                if (!conn.FileExists(fileName))
                {
                    return null;
                }

                Stream stream = conn.OpenRead(fileName);
                return stream;
            }
        }

        public bool Delete(FapFileInfo fileInfo)
        {
            string fileName =Path.Combine(ftpRootPath,fileInfo.FilePath);

            using (FtpClient conn = new FtpClient())
            {
                conn.Host = host;
                conn.Credentials = new NetworkCredential(username, password);
                //conn.Connect();
                if (!conn.FileExists(fileName))
                {
                    return true;
                }

                conn.DeleteFile(fileName);
            }
            return true;
        }
    }
}
