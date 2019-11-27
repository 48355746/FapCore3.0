using Fap.Core.Annex.Utility.Events;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Fap.Core.Extensions;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Config;
using Fap.Core.Exceptions;

namespace Fap.Core.Annex.FileDirectory
{
    public class FileDirectoryService : IFileService
    {
        private IDbContext _dataAccessor;
        private IFapPlatformDomain _appDomain;
        private ILogger<FileDirectoryService> _logger;
        private IFapConfigService _configService;


        public FileDirectoryService(IDbContext dataAccessor, IFapPlatformDomain appDomain, ILogger<FileDirectoryService> logger, IFapConfigService configService)
        {
            _dataAccessor = dataAccessor;
            _appDomain = appDomain;
            _logger = logger;
            _configService = configService;

        }
        /// <summary>
        /// 获取文件存放路径
        /// </summary>
        /// <returns></returns>
        private string GetFilePath()
        {
            string path = _configService.GetSysParamValue("file.directory.path");
            if (path.IsPresent())
            {
                if (path.Contains("\\") || path.Contains("/"))
                {
                    path = path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
                }
            }
            return path;
        }

        public bool Upload(Stream stream, FapFileInfo fileInfo, FileUploadEventHandler updateEvent)
        {
            string fileRepositoryPath = GetFilePath();
            if (fileInfo == null || stream == null)
            {
                return false;
            }
            //文件存储策略
            FileDirectoryStrategy strategy = new FileDirectoryStrategy();

            string lv = _configService.GetSysParamValue("file.directory.level");
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
            string fullpathToSave = Path.Combine(fileRepositoryPath, directoryPath);
            try
            {
                if (!Directory.Exists(fullpathToSave))
                {
                    Directory.CreateDirectory(fullpathToSave);
                }
                string fileToSave = Path.Combine(fullpathToSave, fileName + fileInfo.FileSuffix);

                using (var outStream = new FileStream(fileToSave, FileMode.Create, FileAccess.Write))
                {
                    using (stream)
                    {
                        stream.CopyTo(outStream);
                        if (updateEvent != null)
                        {
                            updateEvent(fileInfo, new FileUploadEventArgs()
                            {
                                Total = stream.Length,
                                Uploaded = stream.Length,
                                FileFullName = Path.Combine(directoryPath, fileName + fileInfo.FileSuffix),
                                FileName = fileName + fileInfo.FileSuffix,
                                FileDirectory = directoryPath
                            });
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
            string fileRepositoryPath = GetFilePath();
            string fileName = Path.Combine(fileRepositoryPath, fileInfo.FilePath);

            if (!System.IO.File.Exists(fileName))
            {
                return null;
            }

            Stream outStream = null;
            try
            {
                outStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new FapException(ex.Message, ex);
            }
            //Stream outStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            //FapFileInfo fileInfo = new FapFileInfo();
            //fileInfo.FileStream = outStream;
            //return fileInfo;
            return outStream;
        }

        public bool Delete(FapFileInfo fileInfo)
        {
            string fileRepositoryPath = GetFilePath();
            string fileName = Path.Combine(fileRepositoryPath, fileInfo.FilePath);

            if (!System.IO.File.Exists(fileName))
            {
                return true;
            }

            System.IO.File.Delete(fileName);
            return true;
        }
    }


}
