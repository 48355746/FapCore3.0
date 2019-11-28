using System;
using System.Collections.Generic;

namespace Fap.Core.Office.Excel
{
    /// <summary>
    /// Excel页的元数据描述
    /// </summary>
    [Serializable]
    public class SheetMetadata
    {
        /// <summary>
        /// 元数据的SheetName
        /// </summary>
        public const string METADATA_SHEET_NAME = "__MAIN_Metadata__";

        private List<ColumnMd> _columns = new List<ColumnMd>();
        public List<ColumnMd> Columns
        {
            get
            {
                return _columns;
            }
            set
            {
                _columns = value;
            }
        }
    }
    public class ColumnMd
    {
        /// <summary>
        /// 列标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 列对应的字段名
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 是否是字典类型
        /// </summary>
        public bool IsNeedDictionary { get; set; }
        /// <summary>
        /// 字典表SheetName
        /// </summary>
        public string DictionarySheetName { get; set; }
        /// <summary>
        /// 字典表页的数据区域
        /// 例子：$B$1:$B$8
        /// </summary>
        public string DictionarySheetRange { get; set; }
        public int StartRowIndex { get; set; }
        public int EndRowIndex { get; set; }

        //[System.Xml.Serialization.XmlIgnore]
        //[System.Web.Script.Serialization.ScriptIgnore]
        //public Tuple<int, int> DictionaryRowRangeIndex { get; set; }
    }
}
