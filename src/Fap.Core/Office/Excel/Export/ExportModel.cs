using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Office.Excel.Export
{
    public class ExportModel
    {
        /// <summary>
        /// 导出实体
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 指定导出的列
        /// </summary>
        public string ExportCols { get; set; }
        /// <summary>
        /// sql语句
        /// </summary>
        public string DataSql { get; set; }

    }
}
