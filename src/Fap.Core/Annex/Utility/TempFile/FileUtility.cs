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

        public static string GetTemporaryDir()
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
        /// 从临时文件夹中删除所有文件，包括文件夹和文件
        /// </summary>
        public static void DeleteYestodayTemporaryFolder()
        {
            DeleteFolder(GetTemporaryDir());
            void DeleteFolder(string dir)
            {
                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    try
                    {
                        if (File.Exists(d))
                        {

                            FileInfo fi = new FileInfo(d);
                            //删除昨天的
                            if (fi.CreationTime < DateTime.Now.AddDays(-1))
                            {
                                if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                                    fi.Attributes = FileAttributes.Normal;
                                File.Delete(d);//直接删除其中的文件  
                            }
                        }
                        else
                        {
                            DirectoryInfo d1 = new DirectoryInfo(d);
                            if (d1.GetFiles().Length != 0)
                            {
                                DeleteFolder(d1.FullName);////递归删除子文件夹
                            }
                            Directory.Delete(d);
                        }
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }
        }
    }
}
