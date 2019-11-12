using Castle.Core.Internal;
using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Encryption;
using Fap.Core.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Fap.Core.Infrastructure.License
{
    public class FileTool
    {
        //private const string path =  System.Environment.SystemDirectory + @"\fap.reg";
        //private const string dir = @"D:\Windows\System32\";
        /// <summary>
        /// 将项目名称和注册码写入文件
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="TGljZW5zZQ"></param>
        public static void WriteRegToFile(RegFileData data)
        {
            if (data == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(data.ProjectName))
            {
                data.ProjectName =UUIDUtils.Fid;
            }
            lock (typeof(FileTool))
            {
                string regfilePath = Path.Combine(Directory.GetCurrentDirectory(), "license.json");
                JObject reg = new JObject();
                string key1 = EncryptionDes.Encrypt("projectname");
                string value1 = EncryptionDes.Encrypt(data.ProjectName ?? "");
                reg[key1] = value1;
                string key2 = EncryptionDes.Encrypt("license");
                string value2 = EncryptionDes.Encrypt(data.TGljZW5zZQ ?? "");
                reg[key2] = value2;
                string key3 = EncryptionDes.Encrypt("invalidtime");
                string value3 = EncryptionDes.Encrypt(data.TrialExpire ?? "");
                reg[key3] = value3;
                string regContent = reg.ToString();
                File.WriteAllText(regfilePath, regContent);
            }

        }
        /// <summary>
        /// 读取项目名称和注册码
        /// </summary>
        /// <returns></returns>
        public static RegFileData ReadRegFromFile()
        {
            string regfilePath = Path.Combine(Directory.GetCurrentDirectory(), "license.json");
            if (!File.Exists(regfilePath))
            {
                return new RegFileData();
            }
            string regContent = File.ReadAllText(regfilePath);
            if (regContent.IsNullOrEmpty())
            {
                return new RegFileData();
            }
            JObject reg = JObject.Parse(regContent);
            string key1 = EncryptionDes.Encrypt("projectname");
            string projectName = reg.GetValue(key1).ToString();
            string key2 = EncryptionDes.Encrypt("license");
            string license = reg.GetValue(key2).ToString();
            string key3 = EncryptionDes.Encrypt("invalidtime");
            string invalidTime = reg.GetValue(key3).ToString();

            if (projectName.IsMissing() || license.IsMissing())// || invalidTime.IsNullOrEmpty())
            {
                return new RegFileData();
            }
            RegFileData data = new RegFileData();

            data.ProjectName = EncryptionDes.Decrypt(projectName);
            data.TGljZW5zZQ = EncryptionDes.Decrypt(license);
            data.TrialExpire = EncryptionDes.Decrypt(invalidTime);
            return data;
        }

    }

   
}
