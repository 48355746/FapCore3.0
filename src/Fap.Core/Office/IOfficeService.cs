using Fap.Core.Office.Excel.Export;
using Fap.Core.Office.Excel.Import;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Office
{
    public interface IOfficeService
    {
        bool ImportExcel(string fileName, string tableName, ImportMode importMode);
        bool ExportExcel(ExportModel exportModel);
    }
}
