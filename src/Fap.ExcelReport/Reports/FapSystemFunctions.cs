using ExcelReportGenerator.Rendering;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fap.ExcelReport.Reports
{
    /// <summary>
    /// 定义系统函数{sf:methodName}
    /// </summary>
    public class FapSystemFunctions : SystemFunctions
    {
        /// <summary>
        /// 生成图片，openxml 暂不支持
        /// </summary>
        /// <param name="bid"></param>
        /// <returns></returns>
        public static string ConvertImage(string bid)
        {
            return $"$[{bid}]";
        }
    }
}
