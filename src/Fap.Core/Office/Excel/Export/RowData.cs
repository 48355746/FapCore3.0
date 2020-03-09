using System;
using System.Collections.Generic;

namespace Fap.Core.Office.Excel.Export
{
    /// <summary>
    /// Excel单元格数据
    /// </summary>
    public class CellData
    {
        public object Data { get; set; }

        private CellDataType _type = CellDataType.STRING;
        public CellDataType Type { 
            get { return _type; }
            set { _type = value; }
        }
    }

    /// <summary>
    /// Excel一行数据
    /// </summary>
    public class RowData
    {
        public IList<CellData> Data { get; set; }
        public bool IsHeader { get; set; }
        public bool IsFooter { get; set; }
    }

    /// <summary>
    /// Excel一列属性
    /// </summary>
    public class ColumnProperty
    {
        /// <summary>
        /// 列表Index
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// 列标题
        /// </summary>
        public string ColumnHeader { get; set; }
        /// <summary>
        /// 列约束名称
        /// </summary>
        public string ConstraintName { get; set; }
        /// <summary>
        /// 列约束引用
        /// </summary>
        public string ConstraintReference { get; set; }
    }

    /// <summary>
    /// Excel的Sheet页的数据
    /// </summary>
    public class SheetData
    {
        /// <summary>
        /// 页的名称
        /// </summary>
        public string SheetName { get; set; }
        /// <summary>
        /// 列的名称
        /// </summary>
        public RowData ColumnTitle { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public IList<RowData> Data { get; set; }
        /// <summary>
        /// 列属性
        /// </summary>
        public IList<ColumnProperty> ColumnProperties { get; set; }
    }

    /// <summary>
    /// Excel单元格数据的类型
    /// </summary>
    public enum CellDataType
    {
        STRING,
        INT,
        DOUBLE,
        DATETIME,
        DATE,
        TIME,
        BOOL
    }

    /// <summary>
    /// 字典表的行数据
    /// </summary>
    public class DictionaryRowData
    {
        public CellData KeyData { get; set; }
        public CellData ValueData { get; set; }
    }

    /// <summary>
    /// 字典表页的数据
    /// </summary>
    public class DictionarySheetData
    {
        /// <summary>
        /// 页的名称
        /// </summary>
        public string SheetName { get; set; }

        /// <summary>
        /// 字典的数据
        /// </summary>
        public List<DictionaryRowData> Data { get; set; }

        /// <summary>
        /// 字典数据的Value在Excel对应的区域地址
        /// </summary>
        public string DictionaryExcelRangeAddress
        {
            get
            {
                if(Data!=null && Data.Count>0) {
                    return SheetName + "!$B$1:$B$" + Data.Count;
                }
                return "";
            }
        }

        /// <summary>
        /// 字典数据的Value在Excel对应的区域
        /// </summary>
        public string DictionaryExcelRange
        {
            get
            {
                if (Data != null && Data.Count > 0)
                {
                    return "$B$1:$B$" + Data.Count;
                }
                return "";
            }
        }

        public int DictionaryRowStartIndex
        {
            get
            {
                return 1;
            }
        }

        public int DictionaryRowEndIndex
        {
            get
            {
                if (Data != null && Data.Count > 0)
                {
                    return Data.Count;
                }
                return -1;
            }
        }

        public Tuple<int, int> DictionaryRowRangeIndex
        {
            get
            {
                if (Data != null && Data.Count > 0)
                {
                    return new Tuple<int, int>(1, Data.Count);
                }
                return null;
            }
        }
    }
}
