using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Encryption;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.License
{
    /// <summary>
    /// License工具
    /// </summary>
    public sealed class TGljZW5zZQTool
    {
        /// <summary>
        /// 申请码生成规则：RSA(项目名称+申请日期+结束日期+硬盘序列号/CPU序列号+随机数+整个MD5)
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="requestDate"></param>
        /// <returns></returns>
        public static string GetRequestCode(string projectName, string requestDate, string endDate)
        {

            HardwareInfo hardwareInfo = new HardwareInfo();
            string opersystem = hardwareInfo.OperSystemDesc;

            string macAddress = hardwareInfo.GetMACAddress();

            //string idMd5 = MyMD5Helper.GetMD532(TGljZW5zZQInfo.SYMBOL + TGljZW5zZQInfo.SEMICOLON + hardDiskID);
            StringBuilder builder = new StringBuilder();
            builder.Append(projectName).Append(TGljZW5zZQInfo.SYMBOL);
            builder.Append(requestDate).Append(TGljZW5zZQInfo.SYMBOL);
            builder.Append(endDate).Append(TGljZW5zZQInfo.SYMBOL);
            builder.Append(opersystem).Append(TGljZW5zZQInfo.COMMA).Append(macAddress).Append(TGljZW5zZQInfo.SYMBOL);
            builder.Append(new Random().NextDouble());

            builder.Append(TGljZW5zZQInfo.SYMBOL).Append(builder.ToString().Md5());

            return EncryptionDes.Encrypt(builder.ToString());
        }

        /// <summary>
        /// 校验申请码
        /// </summary>
        /// <param name="requestCode"></param>
        /// <returns></returns>
        public static bool CheckRequestCode(string requestCode, out RequestCodeInfo requestCodeInfo, out string errMsg)
        {
            errMsg = "";
            requestCodeInfo = new RequestCodeInfo();
            if (string.IsNullOrWhiteSpace(requestCode))
            {
                errMsg = "申请码为空";
                return false;
            }

            try
            {
                //RSA(项目名称+申请日期+结束日期+硬盘序列号/CPU序列号+随机数+整个MD5)
                string code = "";
                try
                {
                    code = EncryptionDes.Decrypt(requestCode);
                }
                catch (Exception)
                {
                    errMsg = "申请码不正确";
                    return false;
                }


                string[] parts = code.Split(new string[] { TGljZW5zZQInfo.SYMBOL }, StringSplitOptions.None);
                if (parts.Length != 6)
                {
                    errMsg = "申请码结构不符合要求";
                    return false;
                }

                string md5 = parts[5];
                string newMd5 = (parts[0] + TGljZW5zZQInfo.SYMBOL + parts[1] + TGljZW5zZQInfo.SYMBOL + parts[2] + TGljZW5zZQInfo.SYMBOL + parts[3] + TGljZW5zZQInfo.SYMBOL + parts[4]).Md5();
                if (md5 != newMd5)
                {
                    errMsg = "申请码校验有误";
                    return false;
                }

                //if (parts[0] != projectName)
                //{
                //    errMsg = "申请的项目名称不符合";
                //    return false;
                //}
                requestCodeInfo.ProjectName = parts[0];

                //if (string.Compare(parts[1], PublicUtils.GetSysDateTimeStr())>=0)
                //{
                //    errMsg = "申请日期晚于当前时间";
                //    return false;
                //}
                requestCodeInfo.RequestDate = parts[1];

                //if (string.Compare(parts[2], PublicUtils.GetSysDateTimeStr()) < 0)
                //{
                //    errMsg = "申请码已失效";
                //    return false;
                //}
                requestCodeInfo.EndDate = parts[2];

                requestCodeInfo.UniqueId = parts[3];
            }
            catch (Exception e)
            {
                errMsg = "申请码校验出现异常，其原因如下：" + e.Message;
                return false;
            }

            return true;
        }

        public static RequestCodeInfo GetRequsetCodeInfo(string requestCode)
        {
            RequestCodeInfo info = new RequestCodeInfo();
            string code = EncryptionDes.Decrypt(requestCode);

            string[] parts = code.Split(new string[] { TGljZW5zZQInfo.SYMBOL }, StringSplitOptions.None);
            info.ProjectName = parts[0];
            info.RequestDate = parts[1];
            info.EndDate = parts[2];

            return info;
        }

        /// <summary>
        /// 从注册表中获取注册码信息
        /// </summary>
        /// <returns></returns>
        public static RegFileData GetTGljZW5zZQDataFromReg()
        {
            //if (MyRegistryHelper.IsRegistryExist(Registry.CurrentUser, FapTGljZW5zZQInfo.REG_PATH, DESEncrypt.Encrypt3(FapTGljZW5zZQInfo.REG_SUB_PROJECTNAME)))
            //{
            //    string projectName = MyRegistryHelper.GetRegistryData(Registry.CurrentUser, FapTGljZW5zZQInfo.REG_PATH, DESEncrypt.Encrypt3(FapTGljZW5zZQInfo.REG_SUB_PROJECTNAME));
            //    return DESEncrypt.Decrypt2(projectName);
            //}
            //else
            //{
            //    return "";
            //}
            RegFileData data = FileTool.ReadRegFromFile();
            return data;
        }
        /// <summary>
        /// 将注册码信息写入注册表
        /// </summary>
        /// <param name="projectName"></param>
        public static void SetTGljZW5zZQDataToReg(string projectName, string TGljZW5zZQ, string trialDate)
        {
            //MyRegistryHelper.SetRegistryData(Registry.CurrentUser, FapTGljZW5zZQInfo.REG_PATH, DESEncrypt.Encrypt3(FapTGljZW5zZQInfo.REG_SUB_PROJECTNAME), DESEncrypt.Encrypt2(projectName));
            RegFileData data = new RegFileData();
            data.ProjectName = projectName;
            data.TGljZW5zZQ = TGljZW5zZQ;
            data.TrialExpire = trialDate;
            FileTool.WriteRegToFile(data);
        }

        //public static string GetTGljZW5zZQMsg()
        //{
        //    string message = "";
        //    string currentDateTime = PublicUtils.CurrentDateTimeStr;
        //        RegFileData regFileData = TGljZW5zZQTool.GetTGljZW5zZQDataFromReg();
        //        string TGljZW5zZQ = regFileData.TGljZW5zZQ;
        //        string projectName = regFileData.ProjectName;
        //        string trialExpire = regFileData.TrialExpire;
        //        if (!string.IsNullOrWhiteSpace(TGljZW5zZQ))
        //        {
        //            string errMsg = "";
        //            TGljZW5zZQInfo licenseInfo = TGljZW5zZQInfo.Parse(TGljZW5zZQ, out errMsg);
        //            if (licenseInfo == null)
        //            {
        //                message = errMsg;
        //            }
        //            else
        //            {
        //                if(licenseInfo.CheckLicense(projectName, out errMsg)) 
        //                {
        //                    message = errMsg;
        //                }


        //                if (licenseInfo.Version == "release") //正式版
        //                {
        //                    IsReleaseVersion = true;
        //                }
        //                else
        //                {
        //                    IsReleaseVersion = false;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrWhiteSpace(trialExpire))
        //            {
        //                IsNormalServiceState = string.Compare(trialExpire, currentDateTime) >= 0 ? 1 : 2;
        //                IsReleaseVersion = false;
        //            }
        //            else
        //            {
        //                trialExpire = PublicUtils.GetDateTimeStr(DateTime.Now.AddMonths(1));
        //                regFileData.TrialExpire = trialExpire;
        //                FileTool.WriteRegToFile(regFileData);
        //                IsNormalServiceState = 1;
        //                IsReleaseVersion = false;
        //            }
        //        }

        //    }
        //}

        ///// <summary>
        ///// 从注册表中获取注册码
        ///// </summary>
        ///// <returns></returns>
        //public static string GetTGljZW5zZQFromReg()
        //{
        //    if (MyRegistryHelper.IsRegistryExist(Registry.CurrentUser, FapTGljZW5zZQInfo.REG_PATH, DESEncrypt.Encrypt3(FapTGljZW5zZQInfo.REG_SUB_TGljZW5zZQ)))
        //    {
        //        string TGljZW5zZQ = MyRegistryHelper.GetRegistryData(Registry.CurrentUser, FapTGljZW5zZQInfo.REG_PATH, DESEncrypt.Encrypt3(FapTGljZW5zZQInfo.REG_SUB_TGljZW5zZQ));
        //        return DESEncrypt.Decrypt2(TGljZW5zZQ);
        //    }
        //    else
        //    {
        //        return "";
        //    }
        //}

        ///// <summary>
        ///// 将注册码写入注册表
        ///// </summary>
        ///// <param name="projectName"></param>
        //public static void SetTGljZW5zZQToReg(string TGljZW5zZQ)
        //{
        //    MyRegistryHelper.SetRegistryData(Registry.CurrentUser, FapTGljZW5zZQInfo.REG_PATH, DESEncrypt.Encrypt3(FapTGljZW5zZQInfo.REG_SUB_TGljZW5zZQ), DESEncrypt.Encrypt2(TGljZW5zZQ));
        //}

        ///// <summary>
        ///// 从注册表中获取试用到期时间
        ///// </summary>
        ///// <returns></returns>
        //public static string GetTrialExpireFromReg()
        //{
        //    if (MyRegistryHelper.IsRegistryExist(Registry.CurrentUser, FapTGljZW5zZQInfo.REG_PATH, DESEncrypt.Encrypt3(FapTGljZW5zZQInfo.REG_SUB_TRIAL_EXPIRE)))
        //    {
        //        string trailExpire = MyRegistryHelper.GetRegistryData(Registry.CurrentUser, FapTGljZW5zZQInfo.REG_PATH, DESEncrypt.Encrypt3(FapTGljZW5zZQInfo.REG_SUB_TRIAL_EXPIRE));
        //        return DESEncrypt.Decrypt2(trailExpire);
        //    }
        //    else
        //    {
        //        return "";
        //    }
        //}

        ///// <summary>
        ///// 将试用到期时间写入注册表
        ///// </summary>
        ///// <param name="projectName"></param>
        //public static void SetTrialExpireToReg(string trialExpire)
        //{
        //    MyRegistryHelper.SetRegistryData(Registry.CurrentUser, FapTGljZW5zZQInfo.REG_PATH, DESEncrypt.Encrypt3(FapTGljZW5zZQInfo.REG_SUB_TRIAL_EXPIRE), DESEncrypt.Encrypt2(trialExpire));
        //}
    }
}
