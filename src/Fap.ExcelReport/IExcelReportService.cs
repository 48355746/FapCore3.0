using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fap.ExcelReport
{
    
    public interface IExcelReportService
    {
        Task<string> Render(string rptUid);
    }
}
