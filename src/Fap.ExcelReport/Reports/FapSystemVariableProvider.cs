using ExcelReportGenerator.Rendering.Providers.VariableProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.ExcelReport.Reports
{
    /// <summary>
    /// 系统变量{sv:ReportDate}
    /// </summary>
    public class FapSystemVariableProvider: SystemVariableProvider
    {
        public string ReportDate
        {
            get { return DateTime.Now.ToString("yyyy-MM-dd"); }
        }
        public string ReportTime
        {
            get { return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

    }
}
