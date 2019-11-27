using Fap.Core.Exceptions;
using System;
using System.IO;

namespace Fap.Core.Annex.Utility.TempFile
{
    internal class FileUtility
    {
        /// <summary>
        /// 临时文件夹名
        /// </summary>
        private const string TEMP_DIR_NAME = "temps";

        private static string GetTemporaryDir()
        {
            string dirPath = Path.Combine(Environment.CurrentDirectory, TEMP_DIR_NAME);
           
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            return dirPath;
        }

        /// <summary>
        /// 在临时文件夹中创建文件夹
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static string CreateTemporaryFolder(string folderName)
        {
            string folderPath =Path.Combine(GetTemporaryDir(), folderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return folderPath;
        }

        /// <summary>
        /// 从文件流中创建文件
        /// </summary>
        /// <param name="fileFullName">文件名全路径</param>
        /// <param name="stream">文件流</param>
        public static void CreateFileFromStream(string fileFullName, Stream stream)
        {
            if (stream == null)
            {
                return;
            }

            using (Stream fileStream = stream)
            {
                using (StreamWriter streamWriter = new StreamWriter(fileFullName))
                {
                    fileStream.CopyTo(streamWriter.BaseStream);
                    streamWriter.Flush();
                }
            }
        }

        /// <summary>
        /// 从临时文件夹中删除指定的文件夹
        /// </summary>
        /// <param name="folderName"></param>
        public static void DeleteFolderFromTemporaryFolder(string folderName)
        {
            try
            {
                string folderPath = Path.Combine(GetTemporaryDir(),folderName);
                if (Directory.Exists(folderPath))
                {
                    DeleteAllFromTemporaryFolder(folderPath);

                    Directory.Delete(folderPath);
                }
            }
            catch (Exception ex)
            {
                throw new FapException("删除临时文件异常" + ex.Message);
            }
        }

        /// <summary>
        /// 从临时文件夹中删除所有文件，包括文件夹和文件
        /// </summary>
        public static void DeleteAllFromTemporaryFolder(string strPath)
        {
            //删除这个目录下的所有子目录
            if (Directory.GetDirectories(strPath).Length > 0)
            {
                foreach (string var in Directory.GetDirectories(strPath))
                {
                    Directory.Delete(var, true);
                }
            }
            //删除这个目录下的所有文件
            if (Directory.GetFiles(strPath).Length > 0)
            {
                foreach (string var in Directory.GetFiles(strPath))
                {
                    File.Delete(var);
                }
            }
        }
    }
}
