using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Core.Office.Excel.Export
{
    /// <summary>
    /// 导出BaseModel的实体的Excel模板
    /// </summary>
    public class ExcelEntityTemplateExport : ExcelExportBase
    {
        private string tableName;

        public ExcelEntityTemplateExport(IDbContext dataAccessor, string tableName, string fileName)
            : base(dataAccessor, fileName)
        {
            this.tableName = tableName;
        }

        /// <summary>
        /// 收集数据
        /// </summary>
        public override void CollectData()
        {
            //主表的信息
            //FapTable table = _dataAccessor.Table(tableName);
            //仅仅导出可见列
            var columns = _dataAccessor.Columns(tableName);
            List<FapColumn> columnList = columns.Where(c => (c.ShowAble == 1||c.ColName=="Fid")).ToList();
            
            //字典表，参照数据
            foreach (var column in columnList)
            {
                if (column.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX)
                {
                    this.GetCodeDictionaryData(column,  this.DictionaryToExport);
                }
                else if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                {
                    this.GetReferenceDictionaryData(column,  this.DictionaryToExport);
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
                if (!ExcelUtils.DefaultFieldNameList.Contains(column.ColName))
                {
                    if (column.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX && this.DictionaryToExport.ContainsKey(code))
                    {
                        //string code = Get30String("C_" + column.RefTable);

                        string range = this.DictionaryToExport[code].DictionaryExcelRange;
                        int startIndex = this.DictionaryToExport[code].DictionaryRowStartIndex;
                        int endIndex = this.DictionaryToExport[code].DictionaryRowEndIndex;
                        this.SheetMetadata.Columns.Add(new ColumnMd() { StartRowIndex = startIndex, EndRowIndex = endIndex, DictionarySheetRange = range, DictionarySheetName = code, Field = column.ColName, IsNeedDictionary = true, Title = column.ColComment });

                    }
                    else if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE && this.DictionaryToExport.ContainsKey(code))
                    {
                        //string code = Get30String("R_" + column.RefTable + column.RefID + column.RefName);

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
            }

            //主表
            SheetData mainSheetData = new SheetData();
            mainSheetData.SheetName = tableName + "主表";
            mainSheetData.Data = new List<RowData>();

            mainSheetData.ColumnTitle = this.GetColumnTitle(columnList);
            mainSheetData.Data.Add(this.GetColumnName(columnList));
            mainSheetData.ColumnProperties = this.GetColumnProperty(columnList);

            this.DataToExport.Add(mainSheetData);


            /**
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

                this.DataToExport.Add(extSheetData);
            }
             * 
             * */
        }

        /// <summary>
        /// 标题行（字段中文名）
        /// </summary>
        /// <param name="columnList"></param>
        /// <param name="defaultColumnList"></param>
        /// <returns></returns>
        private RowData GetColumnTitle(List<FapColumn> columnList)
        {
            RowData columnTitle = new RowData();
            columnTitle.Data = new List<CellData>();
            columnTitle.IsHeader = true;
            foreach (var column in columnList)
            {
                if (!ExcelUtils.DefaultFieldNameList.Contains(column.ColName))
                {
                    if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE || column.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX)
                    {
                        columnTitle.Data.Add(new CellData() { Data = column.ColComment, Type = CellDataType.STRING });
                    }
                    else
                    {
                        columnTitle.Data.Add(new CellData() { Data = column.ColComment, Type = CellDataType.STRING });
                    }
                }
            }
            return columnTitle;
        }

        /// <summary>
        /// 列属性
        /// </summary>
        /// <param name="columnList"></param>
        /// <param name="defaultColumnList"></param>
        /// <returns></returns>
        private List<ColumnProperty> GetColumnProperty(List<FapColumn> columnList)
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
                if (!ExcelUtils.DefaultFieldNameList.Contains(column.ColName))
                {
                    if (column.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX && this.DictionaryToExport.ContainsKey(sheetName))
                    {
                        string refTable = column.RefTable;
                        //string sheetName = Get30String("C_" + refTable);
                        DictionarySheetData dictionarySheetData = this.DictionaryToExport[sheetName];

                        ColumnProperty cp = new ColumnProperty();
                        cp.ColumnHeader = column.ColComment;
                        cp.ColumnIndex = index;
                        cp.ConstraintName = sheetName;// Get30String("C_" + refTable);
                        cp.ConstraintReference = dictionarySheetData.DictionaryExcelRangeAddress; //!$A1:$A3

                        columnProperty.Add(cp);
                    }
                    else if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE && this.DictionaryToExport.ContainsKey(sheetName))
                    {
                        string refTable = column.RefTable;
                        string refID = column.RefID;
                        string refName = column.RefName;
                        //string sheetName = Get30String("R_" + refTable + refID + refName);
                        DictionarySheetData dictionarySheetData = this.DictionaryToExport[sheetName];

                        ColumnProperty cp = new ColumnProperty();
                        cp.ColumnHeader = column.ColComment;
                        cp.ColumnIndex = index;
                        cp.ConstraintName = sheetName;// Get30String("R_" + refTable + refID + refName);
                        cp.ConstraintReference = dictionarySheetData.DictionaryExcelRangeAddress; //!$A1:$A3

                        columnProperty.Add(cp);
                    }

                    index++;
                }
            }
            return columnProperty;
        }

        /// <summary>
        /// 标题行（字段名）
        /// </summary>
        /// <param name="columnList"></param>
        /// <param name="defaultColumnList"></param>
        /// <returns></returns>
        private RowData GetColumnName(List<FapColumn> columnList)
        {
            RowData columnName = new RowData();
            columnName.Data = new List<CellData>();
            columnName.IsHeader = true;

            foreach (var column in columnList)
            {
                if (!ExcelUtils.DefaultFieldNameList.Contains(column.ColName))
                {
                    if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE || column.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX)
                    {
                        columnName.Data.Add(new CellData() { Data = column.ColName, Type = CellDataType.STRING });
                    }
                    else
                    {
                        columnName.Data.Add(new CellData() { Data = column.ColName, Type = CellDataType.STRING });
                    }
                }
            }
            return columnName;
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
        //    string refCondition = column.RefCondition;
        //    if (refCondition.IsNotNullOrEmpty())
        //    {
        //        //去掉权限
        //        refCondition = refCondition.Replace(PlatformConstants.EmployeeNoPower, "");
        //    }
        //    if (!(refCondition.IsNotNullOrEmpty() && refCondition.IndexOf("$") < 0))
        //    {
        //        //去掉参数条件
        //        refCondition = "";
        //    }
        //    string code = "R_" + column.Id; //Get30String("R_" + refTable + refID + refName);
        //    if (!dictionaryToExport.ContainsKey(code))
        //    {
        //        if (refCode.IsNullOrEmpty())
        //        {
        //            refCode = "Id";
        //        }
        //        string refWhere = refCondition.IsNullOrEmpty() ? "" : " where " + refCondition;
        //        string sql = string.Format("SELECT {0} AS FID, {1} AS CODE,{2} AS NAME FROM {3} {4}", refID, refCode, refName, refTable, refWhere);
        //        List<FapDict> dicData = _dataAccessor.Query<FapDict>(sql);

        //        if (dicData != null && dicData.Count > 0)
        //        {
        //            DictionarySheetData dictionarySheetData = new DictionarySheetData();
        //            dictionarySheetData.SheetName = code;
        //            dictionarySheetData.Data = new List<DictionaryRowData>();
        //            var groupDict = dicData.GroupBy(d => d.Name);
        //            foreach (var dict in groupDict)
        //            {
        //                if (dict.Count() > 1)
        //                {
        //                    var dicts = dict.ToList();
        //                    foreach (var item in dicts)
        //                    {
        //                        DictionaryRowData dictionaryRowData = new DictionaryRowData();
        //                        dictionaryRowData.KeyData = new CellData() { Data = item.Fid, Type = CellDataType.STRING };
        //                        dictionaryRowData.ValueData = new CellData() { Data = item.Name + "(" + item.Code + ")", Type = CellDataType.STRING };
        //                        dictionarySheetData.Data.Add(dictionaryRowData);
        //                    }
        //                }
        //                else
        //                {
        //                    var item = dict.First();
        //                    DictionaryRowData dictionaryRowData = new DictionaryRowData();
        //                    dictionaryRowData.KeyData = new CellData() { Data = item.Fid, Type = CellDataType.STRING };
        //                    dictionaryRowData.ValueData = new CellData() { Data = item.Name, Type = CellDataType.STRING };
        //                    dictionarySheetData.Data.Add(dictionaryRowData);
        //                }
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
