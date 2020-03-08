using Fap.Core.Extensions;
using Fap.Core.Utility;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Fap.Core.DataAccess;
using Fap.Core.Exceptions;
using Fap.Core.Office.Excel.Import;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Infrastructure.Domain;

namespace Fap.Core.Office.Excel.Export
{
    /// <summary>
    /// Excel导出的抽象类
    /// </summary>
    public abstract class ExcelExportBase
    {
        protected IDbContext _dataAccessor;


        private string fileName = null; //文件名
        private ExcelVersion excelVersion = ExcelVersion.XLSX; //EXCEL版本

        /// <summary>
        /// 要导出的数据
        /// </summary>
        private IList<SheetData> dataToExport = new List<SheetData>();
        /// <summary>
        /// 要导出的字典数据
        /// </summary>
        private Dictionary<string, DictionarySheetData> dictionaryToExport = new Dictionary<string, DictionarySheetData>();
        /// <summary>
        /// 主表Sheet的元数据描述
        /// </summary>
        private SheetMetadata sheetMetadata = new SheetMetadata();

        private ICellStyle headerCellStyle;
        private ICellStyle dataCellStyle;

        /// <summary>
        /// 成功导出数据的数量
        /// </summary>
        private int totalToFinish = 0;


        /// <summary>
        /// Excel导出委托
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="rowData"></param>
        public delegate void ExcelExportEventHandler(string sheetName, RowData rowData, int rowCount);
        /// <summary>
        ///  Excel导出委托事件
        /// </summary>
        public event ExcelExportEventHandler OnProcess;

        public delegate void ExcelExportDictionaryEventHandler(string sheetName, DictionaryRowData rowData, int rowCount);
        public event ExcelExportDictionaryEventHandler OnProcessDictionary;

        public delegate void ExcelExportStartEventHandler(int rowTotalToExport);
        public event ExcelExportStartEventHandler OnStart;

        public delegate void ExcelExportFinishEventHandler(int rowTotalFinish);
        public event ExcelExportFinishEventHandler OnFinish;

        protected IList<SheetData> DataToExport
        {
            get { return dataToExport; }
            set { dataToExport = value; }
        }

        protected Dictionary<string, DictionarySheetData> DictionaryToExport
        {
            get { return dictionaryToExport; }
            set { dictionaryToExport = value; }
        }

        protected SheetMetadata SheetMetadata
        {
            get
            {
                return sheetMetadata;
            }
            set
            {
                sheetMetadata = value;
            }
        }
        /// <summary>
        /// 存储参照表数据
        /// </summary>
        protected IDictionary<string, IEnumerable<FapDict>> RefTableCache { get; } = new Dictionary<string, IEnumerable<FapDict>>();
        public ExcelExportBase(IDbContext dataAccessor, string fileName)
        {
            _dataAccessor = dataAccessor;
            this.fileName = fileName;
        }

        /// <summary>
        /// 获取数据的抽象方法
        /// </summary>
        public abstract void CollectData();

        /// <summary>
        /// 导出Excel文件
        /// </summary>
        public void Export()
        {
            //收集数据
            this.CollectData();

            if (dataToExport == null || dataToExport.Count == 0)
            {
                return;
            }

            this.totalToFinish = 0;
            //执行开始导出的事件
            if (OnStart != null)
            {
                int total = 0;
                foreach (var data in dataToExport)
                {
                    if (data.Data != null)
                    {
                        total += data.Data.Count;
                    }
                }
                OnStart(total);
            }

            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    IWorkbook workbook = WorkbookFactory.Create((Stream)fs); 
                    if(workbook is HSSFWorkbook)
                    {
                        excelVersion = ExcelVersion.XLS;
                    }
                    else
                    {
                        excelVersion = ExcelVersion.XLSX;
                    }
                    //if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                    //{
                    //    workbook = new XSSFWorkbook();
                    //    excelVersion = ExcelVersion.XLSX;
                    //}
                    //else if (fileName.IndexOf(".xls") > 0) // 2003版本
                    //{
                    //    workbook = new HSSFWorkbook();
                    //    excelVersion = ExcelVersion.XLS;
                    //}
                    //else
                    //{
                    //    throw new FapException("该文件不是EXCEL文件");
                    //}

                    //导出主表数据
                    foreach (var data in dataToExport)
                    {
                        this.ExportSheet(workbook, data.SheetName, data.ColumnTitle, data.Data);
                    }

                    //导出字典数据
                    foreach (var dictionary in dictionaryToExport)
                    {
                        this.ExportDictionarySheet(workbook, dictionary.Key, dictionary.Value);
                    }

                    //字典数据和主表列进行关联
                    BindDictionaryComboBox(workbook);

                    //关于主表Sheet的元数据页隐藏Sheet
                    ExportMetadataSheet(workbook, SheetMetadata.METADATA_SHEET_NAME);

                    //隐藏指定的Sheet
                    HideSheet(workbook);
                    //workbook.GetSheetAt(0).ProtectSheet("fap");
                    //写入到excel文件中
                    workbook.Write(fs);

                    //执行结束导出的事件
                    if (OnFinish != null)
                    {
                        OnFinish(totalToFinish);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FapException(ex.Message, ex);
            }
        }

        /// <summary>
        /// 将数据导出到指定页中
        /// </summary>
        /// <param name="columnTitle"></param>
        /// <param name="data"></param>
        private void ExportSheet(IWorkbook workbook, string sheetName, RowData columnTitle, IList<RowData> data)
        {
            if (workbook == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(sheetName))
            {
                sheetName = UUIDUtils.Fid;
            }
            ISheet sheet = workbook.CreateSheet(sheetName);
            int rowCount = 0;
            this.WriteColumnTitle(workbook, sheet, columnTitle, ref rowCount);
            this.WriteData(workbook, sheet, data, ref rowCount);
        }

        /// <summary>
        /// 将字典表数据导入到Excel页中
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheetName"></param>
        /// <param name="dictionaryData"></param>
        private void ExportDictionarySheet(IWorkbook workbook, string sheetName, DictionarySheetData dictionaryData)
        {
            if (workbook == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(sheetName))
            {
                sheetName = UUIDUtils.Fid;
            }
            ISheet sheet = workbook.CreateSheet(sheetName);
            int rowCount = 0;

            if (dictionaryData == null || dictionaryData.Data == null || dictionaryData.Data.Count == 0)
            {
                return;
            }

            foreach (var row in dictionaryData.Data)
            {
                this.WriteDictionaryRowData(workbook, sheet, row, ref rowCount);

                totalToFinish++;
                if (OnProcessDictionary != null)
                {
                    OnProcessDictionary(sheet.SheetName, row, totalToFinish);
                }
            }

        }

        /// <summary>
        /// 写入一行的数据
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rowData"></param>
        /// <param name="rowCount"></param>
        private void WriteDictionaryRowData(IWorkbook workbook, ISheet sheet, DictionaryRowData rowData, ref int rowCount)
        {
            if (rowData == null || rowData.KeyData == null || rowData.ValueData == null)
            {
                return;
            }
            IRow row = sheet.CreateRow(rowCount);

            ICellStyle cellStyle = this.GetDefaultCellType(workbook);

            SetCellValue(row, cellStyle, 0, rowData.KeyData);
            SetCellValue(row, cellStyle, 1, rowData.ValueData);
            rowCount++;
        }

        private void SetCellValue(IRow row, ICellStyle cellStyle, int i, CellData cellData)
        {
            ICell cell = row.CreateCell(i);
            if (cellStyle != null)
            {
                cell.CellStyle = cellStyle;
            }

            if (cellData.Type == CellDataType.STRING)
            {
                cell.SetCellValue(cellData.Data.ToStringOrEmpty());
            }
            else if (cellData.Type == CellDataType.INT)
            {
                cell.SetCellValue(cellData.Data.ToInt());
            }
            else if (cellData.Type == CellDataType.DOUBLE)
            {
                cell.SetCellValue(cellData.Data.ToDouble());
            }
            else if (cellData.Type == CellDataType.DATETIME)
            {
                cell.SetCellValue(string.Format("{0:yyyy-MM-dd HH:mm:ss}", (DateTime)cellData.Data));
            }
            else if (cellData.Type == CellDataType.DATE)
            {
                cell.SetCellValue(string.Format("{0:yyyy-MM-dd}", (DateTime)cellData.Data));
            }
            else if (cellData.Type == CellDataType.TIME)
            {
                cell.SetCellValue(string.Format("{0:HH:mm:ss}", (DateTime)cellData.Data));
            }
            else if (cellData.Type == CellDataType.BOOL)
            {
                cell.SetCellValue(cellData.Data.ToBool());
            }
            else
            {
                cell.SetCellValue(cellData.Data.ToStringOrEmpty());
            }
        }

        /// <summary>
        /// 写入一行的数据
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="rowData"></param>
        /// <param name="rowCount"></param>
        private void WriteRowData(IWorkbook workbook, ISheet sheet, RowData rowData, ref int rowCount)
        {
            if (rowData == null || rowData.Data == null || rowData.Data.Count == 0)
            {
                return;
            }
            IRow row = sheet.CreateRow(rowCount);

            ICellStyle cellStyle = null;
            if (rowData.IsHeader)
            {
                cellStyle = this.GetColumnHeaderCellType(workbook);
            }
            else
            {
                cellStyle = this.GetDefaultCellType(workbook);
            }

            int count = rowData.Data.Count;
            for (int i = 0; i < count; ++i)
            {
                CellData cellData = rowData.Data[i];
                if (cellData == null) continue;
                SetCellValue(row, cellStyle, i, cellData);
                //隐藏系统默认列
                if (rowCount == 1)
                {
                    //标题行
                    if(ExcelUtils.DefaultFieldNameList.Contains(cellData.Data.ToString(),new FapStringEqualityComparer()))
                    {
                        sheet.SetColumnHidden(i,true);
                    }
                }

            }
            rowCount++;
        }


        /// <summary>
        /// 写入列名
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="columnTitle"></param>
        private void WriteColumnTitle(IWorkbook workbook, ISheet sheet, RowData columnTitle, ref int rowCount)
        {
            if (columnTitle == null || columnTitle.Data == null || columnTitle.Data.Count == 0)
            {
                return;
            }
            this.WriteRowData(workbook, sheet, columnTitle, ref rowCount);
            if (OnProcess != null)
            {
                OnProcess(sheet.SheetName, columnTitle, rowCount);
            }
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="data"></param>
        /// <param name="rowCount"></param>
        private void WriteData(IWorkbook workbook, ISheet sheet, IList<RowData> data, ref int rowCount)
        {
            if (data == null || data.Count == 0)
            {
                return;
            }

            foreach (var row in data)
            {
                this.WriteRowData(workbook, sheet, row, ref rowCount);

                totalToFinish++;
                if (OnProcess != null)
                {
                    OnProcess(sheet.SheetName, row, totalToFinish);
                }
            }
        }

        /// <summary>
        /// 隐藏指定的Sheet
        /// </summary>
        private void HideSheet(IWorkbook workbook)
        {
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                string sheetName = workbook.GetSheetName(i);
                if (sheetName.StartsWith("R_") || sheetName.StartsWith("C_"))
                {
                    workbook.SetSheetHidden(i, SheetState.VeryHidden);
                    //workbook.SetSheetHidden(i, SheetState.Visible);
                }
                if (sheetName == SheetMetadata.METADATA_SHEET_NAME)
                {
                    workbook.SetSheetHidden(i, SheetState.VeryHidden);
                    //workbook.SetSheetHidden(i, SheetState.Visible);
                }
            }

            //设置第一个非隐藏的表单为可视的、选择的
            //NPOI中出现“是否要恢复此工作薄的内容？如果信任此工作薄的来源。。。”的问题的解决方法
            for (int k = 0; k < workbook.NumberOfSheets; k++)
            {
                if (workbook.IsSheetVeryHidden(k) == false)
                {
                    ISheet st = workbook.GetSheetAt(k);
                    workbook.SetSheetOrder(st.SheetName, 0);  // 调整到第一个
                    //workbook.FirstVisibleTab = 0;
                    //workbook.SetSelectedTab(0);
                    break;
                }
            }
        }

        /// <summary>
        /// 字典数据和主表列进行关联
        /// </summary>
        /// <param name="workbook"></param>
        private void BindDictionaryComboBox(IWorkbook workbook)
        {
            foreach (var data in dataToExport)
            {
                IList<ColumnProperty> colProperty = data.ColumnProperties;
                if (colProperty != null && colProperty.Count > 0)
                {
                    foreach (var item in colProperty)
                    {
                        ISheet sheet = workbook.GetSheet(data.SheetName);
                        SheetAddDataValidation(workbook, sheet, item);
                    }
                }
            }
        }

        /// <summary>
        /// Sheet中引用校验引用区域
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="item"></param>
        private void SheetAddDataValidation(IWorkbook workbook, ISheet sheet, ColumnProperty item)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.ConstraintReference)) //如果没有引用区域， 则退出
            {
                return;
            }

            CellRangeAddressList regions = new CellRangeAddressList(2, 65535, item.ColumnIndex, item.ColumnIndex);

            IDataValidation dataValidate = null;
            if (excelVersion == ExcelVersion.XLSX)
            {
                XSSFSheet xssfSheet = sheet as XSSFSheet;
                XSSFDataValidationHelper dvHelper = new XSSFDataValidationHelper(xssfSheet);
                XSSFDataValidationConstraint dvConstraint = (XSSFDataValidationConstraint)dvHelper.CreateFormulaListConstraint(item.ConstraintReference);
                dataValidate = dvHelper.CreateValidation(dvConstraint, regions);
                dataValidate.EmptyCellAllowed = true;
                dataValidate.ShowErrorBox = true;
                dataValidate.CreateErrorBox("系统提示", "请选择指定的" + item.ColumnHeader + "选项");
                dataValidate.ShowPromptBox = true;
                dataValidate.CreatePromptBox("", item.ColumnHeader);
            }
            else
            {
                IName range = workbook.CreateName();
                range.RefersToFormula = item.ConstraintReference;
                range.NameName = item.ConstraintName;

                DVConstraint constraint = DVConstraint.CreateFormulaListConstraint(item.ConstraintName);
                dataValidate = new HSSFDataValidation(regions, constraint);
                dataValidate.EmptyCellAllowed = true;
                dataValidate.ShowErrorBox = true;
                dataValidate.CreateErrorBox("系统提示", "请选择指定的" + item.ColumnHeader + "选项");
                dataValidate.ShowPromptBox = true;
                dataValidate.CreatePromptBox("", item.ColumnHeader);
            }

            sheet.AddValidationData(dataValidate);
        }

        /// <summary>
        /// 导出元数据页
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheetName"></param>
        private void ExportMetadataSheet(IWorkbook workbook, string sheetName)
        {
            if (sheetMetadata == null)
            {
                return;
            }
            ISheet sheet = workbook.CreateSheet(sheetName);
            ICell cell = sheet.CreateRow(0).CreateCell(0);
            cell.SetCellType(CellType.String);

            string xmlString = ExcelUtils.XmlSerialize<SheetMetadata>(sheetMetadata);
            //wyf
            //xmlString = PublicUtils.Base64(xmlString, true);
            cell.SetCellValue(xmlString);
        }
        #region 获取参照和编码数据
        /// <summary>
        /// 获取引用类型的字典数据
        /// </summary>
        /// <param name="column"></param>
        /// <param name="dictionaryToExport"></param>
        /// <returns></returns>
        protected void GetReferenceDictionaryData(FapColumn column, Dictionary<string, DictionarySheetData> dictionaryToExport)
        {
            string refTable = column.RefTable;
            string refID = column.RefID;
            string refCode = column.RefCode;
            string refName = column.RefName;
            string refCondition = column.RefCondition;
            if (refCondition.IsPresent())
            {
                //去掉权限
                refCondition = refCondition.Replace(FapDbConstants.EmployeeNoPower, "");
            }
            if (!(refCondition.IsPresent() && refCondition.IndexOf("$") < 0))
            {
                //去掉参数条件
                refCondition = "";
            }
            string code = "R_" + column.Id; //Get30String("R_" + refTable + refID + refName);
            if (!dictionaryToExport.ContainsKey(code))
            {
                if (refCode.IsMissing())
                {
                    refCode = "Id";
                }
                string refWhere = refCondition.IsMissing() ? "" : " where " + refCondition;
                string sql = string.Format("SELECT {0} AS FID, {1} AS CODE,{2} AS NAME FROM {3} {4}", refID, refCode, refName, refTable, refWhere);
                IEnumerable<FapDict> dicData = _dataAccessor.Query<FapDict>(sql);
                if (!RefTableCache.ContainsKey(refTable))
                {
                    RefTableCache.Add(refTable, dicData);
                }
                if (dicData != null && dicData.Count() > 0)
                {
                    DictionarySheetData dictionarySheetData = new DictionarySheetData();
                    dictionarySheetData.SheetName = code;
                    dictionarySheetData.Data = new List<DictionaryRowData>();
                    var groupDict = dicData.GroupBy(d => d.Name);
                    foreach (var dict in groupDict)
                    {
                        if (dict.Count() > 1)
                        {
                            var dicts = dict.ToList();
                            foreach (var item in dicts)
                            {
                                DictionaryRowData dictionaryRowData = new DictionaryRowData();
                                dictionaryRowData.KeyData = new CellData() { Data = item.Fid, Type = CellDataType.STRING };
                                dictionaryRowData.ValueData = new CellData() { Data = item.Name + "(" + item.Code + ")", Type = CellDataType.STRING };
                                dictionarySheetData.Data.Add(dictionaryRowData);
                            }
                        }
                        else
                        {
                            var item = dict.First();
                            DictionaryRowData dictionaryRowData = new DictionaryRowData();
                            dictionaryRowData.KeyData = new CellData() { Data = item.Fid, Type = CellDataType.STRING };
                            dictionaryRowData.ValueData = new CellData() { Data = item.Name, Type = CellDataType.STRING };
                            dictionarySheetData.Data.Add(dictionaryRowData);
                        }
                    }

                    dictionaryToExport.Add(code, dictionarySheetData);
                }
            }
        }

        /// <summary>
        /// 获取编码类型的字典数据
        /// </summary>
        /// <param name="column"></param>
        /// <param name="dictionaryToExport"></param>
        /// <returns></returns>
        protected void GetCodeDictionaryData(FapColumn column, Dictionary<string, DictionarySheetData> dictionaryToExport)
        {
            //string code = Get30String("C_" + refTable);
            string code = "C_" + column.Id;
            if (!dictionaryToExport.ContainsKey(code))
            {
                IEnumerable<FapDict> dicData = _dataAccessor.Dictionarys(column.ComboxSource);
                if (dicData != null && dicData.Count() > 0)
                {
                    DictionarySheetData dictionarySheetData = new DictionarySheetData();
                    dictionarySheetData.SheetName = code;
                    dictionarySheetData.Data = new List<DictionaryRowData>();

                    foreach (var item in dicData)
                    {
                        DictionaryRowData dictionaryRowData = new DictionaryRowData();
                        dictionaryRowData.KeyData = new CellData() { Data = item.Code, Type = CellDataType.STRING };
                        dictionaryRowData.ValueData = new CellData() { Data = item.Name, Type = CellDataType.STRING };
                        dictionarySheetData.Data.Add(dictionaryRowData);
                    }

                    dictionaryToExport.Add(code, dictionarySheetData);
                }
            }
        }
        #endregion

        #region 私有方法

        #endregion

        #region 单元格样式
        /// <summary>
        /// 获取默认单元格的样式
        /// </summary>
        /// <param name="workbook"></param>
        /// <returns></returns>
        private ICellStyle GetDefaultCellType(IWorkbook workbook)
        {
            if (dataCellStyle == null)
            {
                dataCellStyle = workbook.CreateCellStyle();
                dataCellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                //defaultStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                dataCellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                //defaultStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                //if (workbook is XSSFWorkbook)
                //{
                //    dataCellStyle = workbook.CreateCellStyle() as XSSFCellStyle;
                //    dataCellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                //    //defaultStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                //    dataCellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                //    //defaultStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                //}
                //else if (workbook is HSSFWorkbook)
                //{
                //    dataCellStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                //    dataCellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                //    //defaultStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                //    dataCellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                //    //defaultStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                //}
            }

            return dataCellStyle;
        }

        /// <summary>
        /// 获取列头单元格的样式
        /// </summary>
        protected virtual ICellStyle GetColumnHeaderCellType(IWorkbook workbook)
        {
            if (headerCellStyle == null)
            {
                headerCellStyle = workbook.CreateCellStyle();// as XSSFCellStyle;
                headerCellStyle.IsLocked = true;
                headerCellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                //headStyle.FillPattern = FillPattern.SolidForeground;
                //headStyle.FillBackgroundColor = 1 ;
                headerCellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                headerCellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                headerCellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                headerCellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                IFont font = workbook.CreateFont() as XSSFFont;
                font.FontHeightInPoints = 10;
                font.Boldweight = 1000;
                font.IsBold = true;
                headerCellStyle.SetFont(font);
                //if (workbook is XSSFWorkbook)
                //{
                //    headerCellStyle = workbook.CreateCellStyle();// as XSSFCellStyle;
                //    headerCellStyle.IsLocked = true;
                //    headerCellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                //    //headStyle.FillPattern = FillPattern.SolidForeground;
                //    //headStyle.FillBackgroundColor = 1 ;
                //    headerCellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                //    headerCellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                //    headerCellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                //    headerCellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                //    IFont font = workbook.CreateFont() as XSSFFont;
                //    font.FontHeightInPoints = 10;
                //    font.Boldweight = 1000;
                //    font.IsBold = true;
                //    headerCellStyle.SetFont(font);
                //}
                //else if (workbook is HSSFWorkbook)
                //{
                //    headerCellStyle = workbook.CreateCellStyle() as HSSFCellStyle;
                //    headerCellStyle.IsLocked = true;
                //    headerCellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                //    //headStyle.FillPattern = FillPattern.SolidForeground;
                //    //headStyle.FillBackgroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                //    headerCellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                //    headerCellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                //    headerCellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                //    headerCellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                //    HSSFFont font = workbook.CreateFont() as HSSFFont;
                //    font.FontHeightInPoints = 10;
                //    font.Boldweight = 1000;
                //    headerCellStyle.SetFont(font);
                //}
            }

            return headerCellStyle;
        }

        #endregion
    }

}
