using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Office.Excel.Export
{
    public class ExportModel
    {
        public string TableName { get; set; }
        public string FileName { get; set; }
        public string SqlWhere { get; set; }
        public string ExportCols { get; set; }
        public string DataSql { get; set; }

    }
}
