using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Office.Excel.Export
{
    /// <summary>
    /// 导出实体对象的数据到Excel表中
    /// </summary>
    public class ExcelEntityDataExport : ExcelExportBase
    {
        private string tableName;
        private string exportColumns;
        private string exportDataSql;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName">导出表名</param>
        /// <param name="fileName">文件名</param>
        /// <param name="exportWhere">导出条件</param>
        /// <param name="exportCols">导出列</param>
        /// <param name="dataSql">导出数据sql</param>
        public ExcelEntityDataExport(IDbContext dataAccessor, ExportModel exportModel)
            : base(dataAccessor, exportModel.FileName)
        {
            _dataAccessor = dataAccessor;
            this.tableName = exportModel.TableName;
            exportDataSql = exportModel.DataSql;
            exportColumns = exportModel.ExportCols;
        }

        /// <summary>
        /// 收集数据
        /// </summary>
        public override void CollectData()
        {
            //主表的信息
            FapTable table = _dataAccessor.Table(tableName);
            IEnumerable<FapColumn> columnList = _dataAccessor.Columns(tableName);
            if (exportColumns.IsPresent() && exportColumns != "*")
            {
                var cols = exportColumns.SplitComma();
                columnList = columnList.Where(f => cols.Contains(f.ColName, new FapStringEqualityComparer()));
            }

            //字典表
            foreach (var column in columnList.Where(c => c.RefTable.IsPresent()))
            {
                if (column.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX)
                {
                    this.GetCodeDictionaryData(column, DictionaryToExport);
                }
                else if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                {
                    this.GetReferenceDictionaryData(column, DictionaryToExport);
                }
            }

            //主表元数据
            foreach (var column in columnList)
            {
                string code = "C_" + column.Id;
                if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                {
                    code = "R_" + column.Id;
                }
                if ((column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE || column.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX) && this.DictionaryToExport.ContainsKey(code))
                {
                    string range = this.DictionaryToExport[code].DictionaryExcelRange;
                    int startIndex = this.DictionaryToExport[code].DictionaryRowStartIndex;
                    int endIndex = this.DictionaryToExport[code].DictionaryRowEndIndex;
                    this.SheetMetadata.Columns.Add(new ColumnMd() { StartRowIndex = startIndex, EndRowIndex = endIndex, DictionarySheetRange = range, DictionarySheetName = code, Field = column.ColName, IsNeedDictionary = true, Title = column.ColComment });
                }
                else
                {
                    this.SheetMetadata.Columns.Add(new ColumnMd() { Field = column.ColName, IsNeedDictionary = false, Title = column.ColComment });
                }
            }

            //主表
            SheetData mainSheetData = new SheetData();
            mainSheetData.SheetName = tableName + "主表";
            mainSheetData.Data = new List<RowData>();

            mainSheetData.ColumnTitle = this.GetColumnTitle(columnList);
            mainSheetData.Data.Add(this.GetColumnName(columnList));
            mainSheetData.ColumnProperties = this.GetColumnProperty(columnList);

            //主表的数据
            List<RowData> dataList = this.GetEntityDataList(tableName, columnList);
            foreach (var data in dataList)
            {
                mainSheetData.Data.Add(data);
            }

            this.DataToExport.Add(mainSheetData);

            /*
            //扩展表
            if (!string.IsNullOrWhiteSpace(table.ExtTable))
            {
                SheetData extSheetData = new SheetData();
                extSheetData.SheetName = tableName + "扩展表";
                extSheetData.Data = new List<RowData>();
                string extTableName = table.ExtTable;
                FapTable extTable = dataAccessor.GetSingleTable(extTableName);
                List<FapColumn> extColumnList = dataAccessor.GetColumnList(tableName);
                extSheetData.ColumnTitle = this.GetColumnTitle(columnList, defaultColumnNameList);
                extSheetData.Data.Add(this.GetColumnName(columnList, defaultColumnNameList));

                //扩展表的数据
                List<RowData> extDataList = this.GetEntityDataList(extTableName, columnList, defaultColumnNameList);
                foreach (var data in extDataList)
                {
                    extSheetData.Data.Add(data);
                }

                this.DataToExport.Add(extSheetData);
            }
             * */
        }

        /// <summary>
        /// 标题行（字段中文名）
        /// </summary>
        /// <param name="columnList"></param>
        /// <param name="defaultColumnList"></param>
        /// <returns></returns>
        private RowData GetColumnTitle(IEnumerable<FapColumn> columnList)
        {
            RowData columnTitle = new RowData();
            columnTitle.Data = new List<CellData>();
            columnTitle.IsHeader = true;
            foreach (var column in columnList)
            {
                columnTitle.Data.Add(new CellData() { Data = column.ColComment, Type = CellDataType.STRING });
                //if (!ExcelUtils.DefaultFieldNameList.Contains(column.ColName))
                //{
                //    if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE || column.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX)
                //    {
                //        columnTitle.Data.Add(new CellData() { Data = column.ColComment, Type = CellDataType.STRING });
                //    }
                //    else
                //    {
                //        columnTitle.Data.Add(new CellData() { Data = column.ColComment, Type = CellDataType.STRING });
                //    }
                //}
            }
            return columnTitle;
        }

        /// <summary>
        /// 列属性
        /// </summary>
        /// <param name="columnList"></param>
        /// <param name="defaultColumnList"></param>
        /// <returns></returns>
        private List<ColumnProperty> GetColumnProperty(IEnumerable<FapColumn> columnList)
        {

            List<ColumnProperty> columnProperty = new List<ColumnProperty>();

            int index = 0;
            foreach (var column in columnList)
            {
                string sheetName = "C_" + column.Id;
                if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                {
                    sheetName = "R_" + column.Id;
                }              
                if ((column.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX|| column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE) && this.DictionaryToExport.ContainsKey(sheetName))
                {
                    DictionarySheetData dictionarySheetData = this.DictionaryToExport[sheetName];
                    ColumnProperty cp = new ColumnProperty();
                    cp.ColumnHeader = column.ColComment;
                    cp.ColumnIndex = index;
                    cp.ConstraintName = sheetName;// Get30String("C_" + refTable);
                    cp.ConstraintReference = dictionarySheetData.DictionaryExcelRangeAddress; //!$A1:$A3

                    columnProperty.Add(cp);
                }
                index++;                
            }
            return columnProperty;
        }

        /// <summary>
        /// 标题行（字段名）
        /// </summary>
        /// <param name="columnList"></param>
        /// <param name="defaultColumnList"></param>
        /// <returns></returns>
        private RowData GetColumnName(IEnumerable<FapColumn> columnList)
        {
            RowData columnName = new RowData();
            columnName.Data = new List<CellData>();
            columnName.IsHeader = true;

            foreach (var column in columnList)
            {
                columnName.Data.Add(new CellData() { Data = column.ColName, Type = CellDataType.STRING });
                //if (!ExcelUtils.DefaultFieldNameList.Contains(column.ColName))
                //{
                //    if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE || column.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX)
                //    {
                //        columnName.Data.Add(new CellData() { Data = column.ColName, Type = CellDataType.STRING });
                //    }
                //    else
                //    {
                //        columnName.Data.Add(new CellData() { Data = column.ColName, Type = CellDataType.STRING });
                //    }
                //}
            }
            return columnName;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnList"></param>
        /// <param name="defaultColumnList"></param>
        /// <returns></returns>
        private List<RowData> GetEntityDataList(string tableName, IEnumerable<FapColumn> columnList)
        {
            //先获取列名
            Dictionary<string, FapColumn> columnMap = new Dictionary<string, FapColumn>();
            foreach (var column in columnList)
            {
                columnMap.Add(column.ColName, column);
                //if (!ExcelUtils.DefaultFieldNameList.Contains(column.ColName))
                //{
                //    columnMap.Add(column.ColName, column);
                //}
            }

            //再根据列，组织数据
            List<RowData> dataToExcel = new List<RowData>();

            IEnumerable<dynamic> dataList = null;
            //自定义sql
            if (exportDataSql.IsMissing())
            {
                exportDataSql = $"select * from {tableName}";
            }
            dataList = _dataAccessor.Query(exportDataSql, null, true);

            if (dataList != null)
            {
                RowData rowDataToExcel = null;
                CellData cellData = null;

                foreach (IDictionary<string, object> row in dataList)
                {
                    rowDataToExcel = new RowData();
                    rowDataToExcel.IsHeader = false;
                    rowDataToExcel.Data = new List<CellData>();

                    foreach (var item in columnMap)
                    {
                        FapColumn column = item.Value;
                        if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                        {
                            cellData = new CellData();

                            string mc = row[item.Key + "MC"].ToStringOrEmpty();
                            if (mc.IsPresent())
                            {
                                string refTable = column.RefTable;
                                string refName = column.RefName;
                                if (RefTableCache.TryGetValue(refTable, out IEnumerable<FapDict> refDatas))
                                {
                                    if (refDatas.Count(k => k.Name == mc) > 1)
                                    {
                                        string refID = column.RefID;
                                        string refCode = column.RefCode;
                                        var refData = refDatas.FirstOrDefault(d => d.Fid == row[item.Key].ToString());
                                        cellData.Data = mc + $"({refData.Code})";
                                    }
                                    else
                                    {
                                        //标准导出
                                        cellData.Data = mc;
                                    }
                                }
                            }
                            else
                            {
                                cellData.Data = "";
                            }

                            cellData.Type = CellDataType.STRING;
                            rowDataToExcel.Data.Add(cellData);
                        }
                        else if (column.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX)
                        {
                            cellData = new CellData();
                            //if (exportDataSql.IsPresent())
                            //{
                            //    //自定义sql的时候从缓存取字典名称
                            //    FapDict fapDict = _dataAccessor.Dictionary(column.RefTable, row[item.Key].ToStringOrEmpty());
                            //    cellData.Data = fapDict?.Name;
                            //}
                            //else
                            //{
                            cellData.Data = row[item.Key + "MC"];
                            //}
                            cellData.Type = CellDataType.STRING;
                            rowDataToExcel.Data.Add(cellData);
                        }
                        else
                        {
                            cellData = new CellData();
                            cellData.Data = row[item.Key];
                            cellData.Type = CellDataType.STRING;
                            rowDataToExcel.Data.Add(cellData);
                        }
                    }

                    dataToExcel.Add(rowDataToExcel);
                }
            }

            return dataToExcel;
        }

        ///// <summary>
        ///// 获取引用类型的字典数据
        ///// </summary>
        ///// <param name="column"></param>
        ///// <param name="dictionaryToExport"></param>
        ///// <returns></returns>
        //private void GetReferenceDictionaryData(FapColumn column, List<string> defaultColumnNameList, Dictionary<string, DictionarySheetData> dictionaryToExport)
        //{
        //    string refTable = column.RefTable;
        //    string refID = column.RefID;
        //    string refCode = column.RefCode;
        //    string refName = column.RefName;
        //    //string code = Get30String("R_" + refTable + refID + refName);
        //    string code = "R_" + column.Id;
        //    if (!dictionaryToExport.ContainsKey(code))
        //    {
        //        if (refCode.IsNullOrEmpty())
        //        {
        //            refCode = "Id";
        //        }
        //        //兼容 sql2012以上版本，mysql oracle
        //        string sql = string.Format("SELECT {0} AS CODE, CASE {1} WHEN NULL THEN '' WHEN '' THEN '' ELSE concat({1},'(',{2},')') END AS NAME FROM {3}", refID, refName, refCode, refTable);
        //        List<dynamic> dicData = _dataAccessor.Query(sql);
        //        if (dicData != null && dicData.Count > 0)
        //        {
        //            DictionarySheetData dictionarySheetData = new DictionarySheetData();
        //            dictionarySheetData.SheetName = code;
        //            dictionarySheetData.Data = new List<DictionaryRowData>();

        //            foreach (var item in dicData)
        //            {
        //                DictionaryRowData dictionaryRowData = new DictionaryRowData();
        //                dictionaryRowData.KeyData = new CellData() { Data = item.CODE, Type = CellDataType.STRING };
        //                dictionaryRowData.ValueData = new CellData() { Data = item.NAME, Type = CellDataType.STRING };
        //                dictionarySheetData.Data.Add(dictionaryRowData);
        //            }

        //            dictionaryToExport.Add(code, dictionarySheetData);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 获取编码类型的字典数据
        ///// </summary>
        ///// <param name="column"></param>
        ///// <param name="dictionaryToExport"></param>
        ///// <returns></returns>
        //private void GetCodeDictionaryData(FapColumn column, List<string> defaultColumnNameList, Dictionary<string, DictionarySheetData> dictionaryToExport)
        //{
        //    string refTable = column.RefTable;
        //    //string code = Get30String("C_" + refTable);
        //    string code = "C_" + column.Id;
        //    if (!dictionaryToExport.ContainsKey(code))
        //    {
        //        string sql = "SELECT Code AS CODE, Name AS NAME FROM FapDict WHERE Category = '" + refTable + "' ";
        //        List<dynamic> dicData = _dataAccessor.Query(sql);
        //        if (dicData != null && dicData.Count > 0)
        //        {
        //            DictionarySheetData dictionarySheetData = new DictionarySheetData();
        //            dictionarySheetData.SheetName = code;
        //            dictionarySheetData.Data = new List<DictionaryRowData>();

        //            foreach (var item in dicData)
        //            {
        //                DictionaryRowData dictionaryRowData = new DictionaryRowData();
        //                dictionaryRowData.KeyData = new CellData() { Data = item.CODE, Type = CellDataType.STRING };
        //                dictionaryRowData.ValueData = new CellData() { Data = item.NAME, Type = CellDataType.STRING };
        //                dictionarySheetData.Data.Add(dictionaryRowData);
        //            }

        //            dictionaryToExport.Add(code, dictionarySheetData);
        //        }
        //    }
        //}

        //private string Get30String(string str)
        //{
        //    if (str.Length > 15)
        //    {
        //        return str.Substring(0, 15);
        //    }
        //    return str;
        //}
    }
}
