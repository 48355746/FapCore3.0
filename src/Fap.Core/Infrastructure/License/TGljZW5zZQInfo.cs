using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Encryption;
using Fap.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fap.Core.Infrastructure.License
{
    /// <summary>
    /// 注册码信息对象
    /// </summary>
    public class TGljZW5zZQInfo
    {
        public const string SYMBOL = "@^@";
        public const string SYMBOL2 = "@~@";
        public const string SEMICOLON = ";";
        public const string COMMA = ",";
        public const string VERSION_TRIAL = "trial";
        public const string VERSION_RELEASE = "release";
        public const string VERSION_DEVELOP = "develop";

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 有效开始时间
        /// </summary>
        public string StartDate { get; set; }
        /// <summary>
        /// 有效结束时间
        /// </summary>
        public string EndDate { get; set; }
        /// <summary>
        /// 版本： trial、release
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 最大用户数
        /// </summary>
        public int MaxUsers { get; set; }
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string UniqueId { get; set; }
        /// <summary>
        /// 功能模块权限
        /// </summary>
        public List<string> ModulePerm { get; set; }

        /// <summary>
        /// 生成注册码
        /// </summary>
        /// <returns></returns>
        public string MakeLicense()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(ProjectName).Append(SYMBOL);
            builder.Append(StartDate).Append(SYMBOL);
            builder.Append(EncryptionDes.Encrypt(EndDate)).Append(SYMBOL);
            builder.Append(Version).Append(SYMBOL);
            builder.Append(MaxUsers).Append(SYMBOL);
            builder.Append(UniqueId).Append(SYMBOL);
            builder.Append(string.Join(',',ModulePerm)).Append(SYMBOL);
            builder.Append(new Random().NextDouble());

            builder.Append(SYMBOL).Append(builder.ToString().Md5());

            return EncryptionDes.Encrypt(builder.ToString());
        }

        /// <summary>
        /// 解析注册码字符串
        /// </summary>
        /// <param name="licenseCode"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static TGljZW5zZQInfo Parse(string licenseCode, out string errMsg)
        {
            errMsg = "";
            try
            {
                TGljZW5zZQInfo licenseInfo = new TGljZW5zZQInfo();
                string code = "";
                try
                {
                    code = EncryptionDes.Decrypt(licenseCode);
                }
                catch (Exception)
                {
                    errMsg = "注册码不正确";
                    return null;
                }


                string[] parts = code.Split(new string[] { SYMBOL }, StringSplitOptions.None);
                if (parts.Length != 9)
                {
                    errMsg = "注册码结构不符合要求";
                    return null;
                }

                string md5 = parts[8];
                string newMd5 = (parts[0] + SYMBOL + parts[1] + SYMBOL + parts[2] + SYMBOL + parts[3] + SYMBOL + parts[4] + SYMBOL + parts[5] + SYMBOL + parts[6] + SYMBOL + parts[7]).Md5();
                if (md5 != newMd5)
                {
                    errMsg = "注册码校验有误";
                    return null;
                }

                //if (parts[0] != projectName)
                //{
                //    errMsg = "注册的项目名称不符合";
                //    return null;
                //}
                licenseInfo.ProjectName = parts[0];

                licenseInfo.StartDate = parts[1];
                licenseInfo.EndDate = EncryptionDes.Decrypt(parts[2]);
                if (string.IsNullOrWhiteSpace(parts[3]))
                {
                    licenseInfo.Version = "Trial";
                }
                else
                {
                    licenseInfo.Version = parts[3];
                }

                licenseInfo.MaxUsers = parts[4].ToInt();
                licenseInfo.UniqueId = parts[5];
                if (!string.IsNullOrWhiteSpace(parts[6]))
                {
                    licenseInfo.ModulePerm = parts[6].Split(',').ToList();
                }
                else
                {
                    licenseInfo.ModulePerm = new List<string>();
                }

                return licenseInfo;
            }
            catch (Exception e)
            {
                errMsg = "注册码解析出现异常，其原因如下：" + e.Message;

                return null;
            }
        }

        /// <summary>
        /// 校验注册码
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public RegisterInfo CheckLicense(string projectName)
        {
            RegisterInfo registerInfo = new RegisterInfo();
            //QUxMUFJPSkVDVA 表示“所有项目”， 如果是所有项目，则不校验
            if (this.ProjectName != "QUxMUFJPSkVDVA" && this.ProjectName != projectName)
            {
                registerInfo.RegisterState = EnumRegisterState.UnRegister;
                registerInfo.RegisterMessage = "注册的项目名称不符合";
                return registerInfo;
            }

            if (this.Version == VERSION_DEVELOP)
            {
                DateTime endDate = DateTime.ParseExact(this.EndDate, "yyyy-MM-dd HH:mm:ss", null);
                TimeSpan ts = endDate.Subtract(DateTime.Now);
                registerInfo.ExpireWhenTrial = ts.Days;
                registerInfo.ExpireDateTimeWhenTrial = this.EndDate;
                if (registerInfo.ExpireWhenTrial > 0)
                {
                    registerInfo.RegisterState = EnumRegisterState.Develop;
                    registerInfo.RegisterMessage = "可注册为开发版，到期时间为" + this.EndDate;
                }
                else
                {
                    registerInfo.RegisterState = EnumRegisterState.UnRegister;
                    registerInfo.RegisterMessage = "开发版已过期，请重新注册。";
                }
                registerInfo.AuthoredModules = this.ModulePerm;
                registerInfo.ProjectName = this.ProjectName;
            }
            else
            {

                HardwareInfo hardwareInfo = new HardwareInfo();
                string operSystem = hardwareInfo.OperSystemDesc;

                string macAddress = hardwareInfo.GetMACAddress();

                if (this.UniqueId != operSystem + COMMA + macAddress)
                {
                    registerInfo.RegisterState = EnumRegisterState.UnRegister;
                    registerInfo.RegisterMessage = "注册码的唯一ID不正确";
                    return registerInfo;
                }

                if (string.Compare(this.StartDate,DateTimeUtils.CurrentDateTimeStr) > 0)
                {
                    registerInfo.RegisterState = EnumRegisterState.UnRegister;
                    registerInfo.RegisterMessage = "注册码还未生效";
                    return registerInfo;
                }

                if (string.Compare(this.EndDate, DateTimeUtils.CurrentDateTimeStr) < 0)
                {
                    registerInfo.RegisterState = EnumRegisterState.UnRegister;
                    registerInfo.RegisterMessage = "注册码已失效";
                    return registerInfo;
                }

                if (this.Version == VERSION_TRIAL) //试用版
                {
                    if (!string.IsNullOrWhiteSpace(this.EndDate))
                    {
                        registerInfo.RegisterState = EnumRegisterState.Trial;

                        DateTime endDate = DateTime.ParseExact(this.EndDate, "yyyy-MM-dd HH:mm:ss", null);
                        TimeSpan ts = endDate.Subtract(DateTime.Now);
                        registerInfo.ExpireWhenTrial = ts.Days;
                        registerInfo.ExpireDateTimeWhenTrial = this.EndDate;

                        registerInfo.RegisterMessage = "可注册为试用版，到期时间为" + this.EndDate;
                    }
                    else
                    {
                        registerInfo.RegisterState = EnumRegisterState.UnRegister;
                        registerInfo.RegisterMessage = "没有到期时间";
                    }

                }
                else if (this.Version == VERSION_RELEASE) //正式版
                {
                    registerInfo.RegisterState = EnumRegisterState.Release;
                    registerInfo.RegisterMessage = "可注册为正式版";
                }

                registerInfo.AuthoredModules = this.ModulePerm;
                registerInfo.ProjectName = this.ProjectName;
            }
            return registerInfo;
        }
    }
}
