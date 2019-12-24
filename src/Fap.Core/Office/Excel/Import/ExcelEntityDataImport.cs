using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Metadata;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Office.Excel.Import
{
    public class ExcelEntityDataImport : ExcelImportBase
    {
        /// <summary>
        /// EXCEL中标题区的开始行号（从0开始）
        /// </summary>
        public int HeaderStartIndex { get; set; }

        /// <summary>
        /// EXCEL中标题区的结束行号（从0开始，必须大于等于HeaderStartIndex）
        /// </summary>
        public int HeaderEndIndex { get; set; }

        /// <summary>
        /// EXCEL中数据区的开始行号（从0开始）
        /// </summary>
        public int DataStartIndex { get; set; }

        private SheetMetadata sheetMetadata;

        private string tableName;

        /// <summary>
        /// 导入模式
        /// </summary>
        private ImportMode mode;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataAccessor"></param>
        /// <param name="fileName">文件路径</param>
        /// <param name="tableName">表名</param>
        /// <param name="mode">导入模式</param>
        public ExcelEntityDataImport(IDbContext dataAccessor, string fileName, string tableName, ImportMode mode)
            : base(dataAccessor, fileName)
        {
            this.tableName = tableName;
            this.mode = mode;
        }

        /// <summary>
        /// 导入数据的具体方法
        /// </summary>
        /// <param name="workbook"></param>
        public override void Import(IWorkbook workbook, SheetMetadata sheetMetadata)
        {
            int sheetCount = workbook.NumberOfSheets;
            if (sheetCount == 0)
            {
                return;
            }
            this.sheetMetadata = sheetMetadata;

            FapTable table =_dataAccessor.Table(tableName);
            if (table == null)
            {
                return;
            }

            if (mode == ImportMode.FORCE) //强制导入
            {
                _dataAccessor.DeleteExec(tableName);
                if (!string.IsNullOrWhiteSpace(table.ExtTable))
                {
                    _dataAccessor.DeleteExec(table.ExtTable);
                }
            }

            ImportDataToTable(workbook, workbook.GetSheetAt(0), this.tableName);


        }

        /// <summary>
        /// 导入数据到表中
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        private void ImportDataToTable(IWorkbook workbook, ISheet sheet, string tableName)
        {
            List<string> columnNameList = new List<string>();
            var entityColumnList =_dataAccessor.Columns(tableName);
            var entityColumnNameList = entityColumnList.Select(c => c.ColName); ;

            //获得Excel中列名
            var sheetColumnNameList = this.GetColumnNameListFromSheet(workbook, sheet, entityColumnList);
            //获取Excel中数据
            var excelData = this.GetDataListFromSheet(workbook, sheet, sheetColumnNameList);


            if (mode == ImportMode.FORCE) //强制导入
            {
                List<FapDynamicObject> dataListToInsert = new List<FapDynamicObject>(); //要新增的数据
                foreach (Dictionary<string, string> rowData in excelData)
                {
                    if (!rowData.ContainsKey("Fid"))
                    {
                        rowData["Fid"] = "";
                    }
                    dynamic entityObj = this.MakeEntityObj(tableName, rowData);
                    if (entityObj != null) dataListToInsert.Add(entityObj);
                }
                _dataAccessor.InsertDynamicDataBatch(dataListToInsert);
            }
            else if (mode == ImportMode.INCREMENT_LOGIC || mode == ImportMode.INCREMENT_NOLOGIC) //逻辑增量导入 / 非逻辑增加导入
            {
                //比较导入数据和已有数据
                var existDatas = _dataAccessor.Query($"select * from { tableName }");
                IEnumerable<FapDynamicObject> updateList = new List<FapDynamicObject>();
                IEnumerable<FapDynamicObject> insertList = new List<FapDynamicObject>();
                foreach (Dictionary<string, string> rowData in excelData)
                {
                    if (!rowData.ContainsKey("Fid"))
                    {
                        rowData["Fid"] = "";
                    }
                    var existData = existDatas.FirstOrDefault(d => d.Fid == rowData["Fid"]);
                    if (existData != null)
                    {
                        //数据库存在相同Fid的数据，进行比对
                        bool notEqual = false;
                        var existDicData = existData as IDictionary<string, object>;

                        foreach (var key in rowData.Keys)
                        {
                            if (existDicData.ContainsKey(key) && rowData[key] == existDicData[key].ToStringOrEmpty())
                            {
                                continue;
                            }
                            notEqual = true;
                            break;
                        }
                        if (notEqual)
                        {
                            dynamic entityObj = this.MakeEntityObj(tableName, rowData);
                            updateList.AsList().Add(entityObj);
                        }
                    }
                    else
                    {
                        //不存在即新增
                        dynamic entityObj = this.MakeEntityObj(tableName, rowData);
                        insertList.AsList().Add(entityObj);
                    }

                }
                _dataAccessor.InsertDynamicDataBatch(insertList);
                _dataAccessor.UpdateDynamicDataBatch(updateList);
            }


        }

        /// <summary>
        /// 构建动态实体对象
        /// </summary>
        private dynamic MakeEntityObj(string tableName, Dictionary<string, string> rowData)
        {
            if (rowData == null || rowData.Count == 0)
            {
                return null;
            }

            //判断一行数据是否为空
            bool isBlankRow = true;
            foreach (var row in rowData)
            {
                if (!string.IsNullOrWhiteSpace(row.Value))
                {
                    isBlankRow = false;
                    break;
                }
            }

            if (!isBlankRow)
            {
                FapDynamicObject entityObj = new FapDynamicObject(_dataAccessor.Columns(tableName));
                foreach (var row in rowData)
                {
                    entityObj.SetValue(row.Key, row.Value);
                }
                return entityObj;
            }
            return null;
        }

        /// <summary>
        /// 从Excel中获取字段名
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <returns></returns>
        private List<ColumnInfo> GetColumnNameListFromSheet(IWorkbook workbook, ISheet sheet, IEnumerable<FapColumn> entityColumnList)
        {
            List<ColumnInfo> columnInfoList = new List<ColumnInfo>();
            if (sheet == null)
            {
                return columnInfoList;
            }

            //获取标题行的列名
            IRow columnTitleRow = sheet.GetRow(0);
            IRow columnNameRow = sheet.GetRow(1);
            IRow columnValidateRow = sheet.GetRow(2);
            int cellCount = columnNameRow.LastCellNum; //一行最后一个cell的编号 即总的列数
            for (int i = columnNameRow.FirstCellNum; i < cellCount; ++i)
            {
                ICell cell = columnNameRow.GetCell(i);
                if (cell != null)
                {
                    string cellValue = cell.StringCellValue;
                    //筛选字段名
                    if (!string.IsNullOrWhiteSpace(cellValue))
                    {
                        string columnName = cellValue.Trim();
                        FapColumn column = entityColumnList.Where(c => c.ColName == columnName).FirstOrDefault();
                        ColumnMd columnMd = this.GetColumnMetadata(columnName);
                        if (column != null)
                        {
                            ColumnInfo columnInfo = new ColumnInfo();
                            columnInfo.ColumnTitle = column.ColComment;
                            columnInfo.ColumnName = column.ColName;
                            columnInfo.ColumnType = column.ColType;
                            if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE || column.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX)
                            {
                                columnInfo.IsNeedDictionary = true;
                                if (columnMd != null)
                                {
                                    columnInfo.DictionarySheetName = columnMd.DictionarySheetName;
                                    columnInfo.DictionaryExcelRange = columnMd.DictionarySheetRange;
                                    columnInfo.DictionaryRowRangeIndex = new Tuple<int, int>(columnMd.StartRowIndex, columnMd.EndRowIndex);
                                    //columnInfo.DictionaryRowRangeIndex = columnMd.DictionaryRowRangeIndex;
                                }

                                //ICell validateCell = columnValidateRow.GetCell(i);
                                //if (validateCell != null)
                                //{validateCell.CellFormula
                                //    CellRangeAddress dr = new CellRangeAddress(2, 2, i, i);
                                //}
                            }

                            columnInfoList.Add(columnInfo);
                        }
                        else
                        {
                            //如果字段名不存在，则设置为空字符串，用于后面判断

                        }
                    }
                }
            }
            return columnInfoList;
        }

        /// <summary>
        /// 从Excel中获取数据
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="sheet"></param>
        /// <param name="sheetColumnInfoList"></param>
        /// <returns></returns>
        private List<Dictionary<string, string>> GetDataListFromSheet(IWorkbook workbook, ISheet sheet, List<ColumnInfo> sheetColumnInfoList)
        {
            List<Dictionary<string, string>> data = null;
            if (sheet == null)
            {
                return data;
            }

            data = new List<Dictionary<string, string>>();

            int columnCount = sheetColumnInfoList.Count;
            int startRow = 2;
            int rowCount = sheet.LastRowNum;
            for (int i = startRow; i <= rowCount; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue; //没有数据的行默认是null　

                //一行的数据
                Dictionary<string, string> rowData = new Dictionary<string, string>();
                int cellCount = Math.Min(row.LastCellNum, columnCount);
                for (int j = 0; j < cellCount; ++j)
                {
                    ColumnInfo columnInfo = sheetColumnInfoList[j];
                    string columnName = columnInfo.ColumnName;
                    if (string.IsNullOrWhiteSpace(columnName)) continue;

                    ColumnMd columMd = this.GetColumnMetadata(columnName);
                    if (!rowData.ContainsKey(columnName))
                    {
                        string value = "";
                        object cellValue = ExcelUtils.GetCellValue(row.GetCell(j));
                        if (cellValue is DateTime)
                        {
                            value = string.Format("{0:yyyy-MM-dd HH:mm:ss}", (DateTime)cellValue);
                        }
                        else
                        {
                            value = cellValue.ToStringOrEmpty();
                        }

                        if (columnInfo.IsNeedDictionary)
                        {
                            //从字典页中读取所对应的Key值
                            value = this.GetDictionaryKeyByValue(workbook, columMd.DictionarySheetName, columMd, value).ToStringOrEmpty();
                            rowData.Add(columnName, value);
                        }
                        else
                        {
                            rowData.Add(columnName, value);
                        }
                    }
                }

                data.Add(rowData);
            }

            return data;
        }



        /// <summary>
        /// 获取列元数据对象
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        private ColumnMd GetColumnMetadata(string columnName)
        {
            if (sheetMetadata == null || sheetMetadata.Columns == null || sheetMetadata.Columns.Count == 0)
            {
                return null;
            }

            return sheetMetadata.Columns.Where(c => c.Field == columnName).FirstOrDefault();
        }

        /// <summary>
        /// 根据Value值，查找字典Sheet中对应的Key值
        /// Key值在第一列， Value值在第二列
        /// </summary>
        /// <param name="workbook"></param>
        /// <param name="dictionarySheetName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private object GetDictionaryKeyByValue(IWorkbook workbook, string dictionarySheetName, ColumnMd columnMd, string value)
        {
            if (dictionarySheetName.IsMissing())
            {
                return null;
            }
            ISheet sheet = workbook.GetSheet(dictionarySheetName);
            if (sheet == null)
            {
                return null;
            }

            IRow row = null;
            ICell cell = null;
            for (int i = columnMd.StartRowIndex - 1; i < columnMd.EndRowIndex; i++)
            {
                row = sheet.GetRow(i);
                cell = row.GetCell(1);
                object obj = ExcelUtils.GetCellValue(cell);
                if (obj != null && obj.ToString().Trim() == value)
                {
                    return ExcelUtils.GetCellValue(row.GetCell(0));
                }
            }

            return "";
        }


    }

    public class ColumnInfo
    {
        /// <summary>
        /// 列标题
        /// </summary>
        public string ColumnTitle { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 列格式类型
        /// </summary>
        public string ColumnType { get; set; }
        /// <summary>
        /// 是否需要字典转义
        /// </summary>
        public bool IsNeedDictionary { get; set; }
        /// <summary>
        /// 字典SheetName
        /// </summary>
        public string DictionarySheetName { get; set; }
        /// <summary>
        /// 字典数据所在区域
        /// </summary>
        public string DictionaryExcelRange { get; set; }

        public Tuple<int, int> DictionaryRowRangeIndex { get; set; }
    }
}
