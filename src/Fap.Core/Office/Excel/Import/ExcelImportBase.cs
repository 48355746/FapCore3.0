using Fap.Core.DataAccess;
using Fap.Core.Exceptions;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Fap.Core.Office.Excel.Import
{
    /// <summary>
    /// Excel导入的抽象类
    /// </summary>
    public abstract class ExcelImportBase
    {
        private string fileName = null; //文件名
       // private ExcelVersion excelVersion = ExcelVersion.XLSX; //EXCEL版本

        protected IDbContext _dataAccessor;

        /// <summary>
        /// EXCEL版本
        /// </summary>
        //public ExcelVersion ExcelVersion
        //{
        //    get
        //    {
        //        return excelVersion;
        //    }
        //}

        public ExcelImportBase(IDbContext dataAccsessor, string fileName)
        {
            _dataAccessor = dataAccsessor;
            this.fileName = fileName;
        }

        public abstract void Import(IWorkbook workbook, SheetMetadata sheetMetadata);

        /// <summary>
        /// 从Excel文件中导入数据
        /// </summary>
        public void Import()
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    IWorkbook workbook = WorkbookFactory.Create((Stream)fs); 
                    //if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                    //{
                    //    workbook = new XSSFWorkbook(fs);
                    //    excelVersion = ExcelVersion.XLSX;
                    //}
                    //else if (fileName.IndexOf(".xls") > 0) // 2003版本
                    //{
                    //    workbook = new HSSFWorkbook(fs);
                    //    excelVersion = ExcelVersion.XLS;
                    //}

                    SheetMetadata sheetMetadata = this.CollectMetadata(workbook);
                    this.Import(workbook, sheetMetadata);
                }
            }
            catch (Exception ex)
            {
                throw new FapException(ex.Message, ex);
            }
        }

        /// <summary>
        /// 收集模板的元数据信息
        /// </summary>
        /// <param name="workbook"></param>
        /// <returns></returns>
        protected virtual SheetMetadata CollectMetadata(IWorkbook workbook)
        {
            ISheet sheet = workbook.GetSheet(SheetMetadata.METADATA_SHEET_NAME);
            if (sheet == null)
            {
                return null;
            }

            ICell cell = sheet.GetRow(0).GetCell(0);
            if (cell == null)
            {
                return null;
            }

            object obj = ExcelUtils.GetCellValue(cell);
            string xmlString = obj.ToString();
            SheetMetadata sheetMetadata = ExcelUtils.XmlDeserialize<SheetMetadata>(xmlString);
            return sheetMetadata;

        }
    }
}
