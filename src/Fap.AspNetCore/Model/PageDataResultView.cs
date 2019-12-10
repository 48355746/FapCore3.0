using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fap.Core.Extensions;

namespace Fap.AspNetCore.Model
{
    public class PageDataResultView : DataResultView
    {
        private List<Sort> _Sorts;
        //private List<dynamic> _Items;

        public PageDataResultView()
        {
            //this._Items = new List<dynamic>();
            this._Sorts = new List<Sort>();
        }

        ///// <summary>
        ///// 当前页上的数据集合
        ///// </summary>
        //public List<dynamic> Items
        //{
        //    get { return _Items; }
        //    private set { _Items = value; }
        //}


        /// <summary>
        /// 当前页码，从1开始
        /// </summary>
        public int CurrentPageIndex { get; set; }
        /// <summary>
        /// 每页的个数
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// 记录总数量
        /// </summary>
        public int TotalNum { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage
        {
            get
            {
                return TotalNum > 0 ? ((TotalNum + PageCount - 1) / PageCount) : 1;
            }
        }

        public void AddSort(string columnName, SortType sortType)
        {
            this._Sorts.Add(new Sort() { Column = columnName, Type = sortType });
        }

        public void AddSort(string columnName)
        {
            this._Sorts.Add(new Sort() { Column = columnName });
        }

        public List<Sort> Sorts
        {
            get { return _Sorts; }
        }

        /// <summary>
        /// 查询结果数据集合（符合jqGrid）
        /// </summary>
        public IEnumerable<IDictionary<string,object>> DataListForJqGrid { get; set; }


        /// <summary>
        /// 查询结果数据集合的JSON字符串(jqGrid的JSON格式)
        /// </summary>
        public JqGridData GetJqGridJsonData()
        {
            if (DataListForJqGrid == null || DataListForJqGrid.Count() == 0)
            {
                return new JqGridData { Total = 0, Page = 0, Records = 0, Rows =Enumerable.Empty<IDictionary<string,object>>(), Userdata = StatFieldData };
            }

            var jsonObj = new JqGridData
            {
                Total = this.TotalPage,
                Page = this.CurrentPageIndex,
                Records = this.TotalNum,
                Rows = DataListForJqGrid,// DataListForJqGrid.ToArray(),
                Userdata = StatFieldData
            };
            return jsonObj;
        }

        /// <summary>
        /// 原始的结果数据集合（dappermap）
        /// </summary>
        [NonSerialized]
        private IEnumerable<dynamic> _orgin_data;
        public IEnumerable<dynamic> OrginData { set { _orgin_data = value; } get { return _orgin_data; } }
    }
}
