using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fap.Core.DataAccess;
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
        public int CurrentPage { get; set; }
        /// <summary>
        /// 每页的个数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                return TotalCount > 0 ? ((TotalCount + PageSize - 1) / PageSize) : 1;
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
        public IEnumerable<dynamic> DataListForJqGrid { get; set; }


        /// <summary>
        /// 查询结果数据集合的JSON字符串(jqGrid的JSON格式)
        /// </summary>
        public JqGridData GetJqGridJsonData()
        {
            if (DataListForJqGrid == null || !DataListForJqGrid.Any())
            {
                return new JqGridData { Total = 0, Page = 0, Records = 0, Rows =Enumerable.Empty<IDictionary<string,object>>(), Userdata = StatFieldData };
            }          
            var jsonObj = new JqGridData
            {
                Total = this.PageCount,
                Page = this.CurrentPage,
                Records = this.TotalCount,
                Rows = DataListForJqGrid,// DataListForJqGrid.ToArray(),
                Userdata = StatFieldData
            };
            return jsonObj;
        }
        public JqGridData GetJqGridTreeJsonData()
        {
            if (DataListForJqGrid == null || !DataListForJqGrid.Any())
            {
                return new JqGridData { Total = 0, Page = 0, Records = 0, Rows = Enumerable.Empty<IDictionary<string, object>>(), Userdata = StatFieldData };
            }
            //remark：此处Id 和 Pid 一定是字符串，否则树折叠有问题
            var enumerator= DataListForJqGrid.GetEnumerator();
            while (enumerator.MoveNext())
            //foreach (var data in DataListForJqGrid)
            {
                var data = enumerator.Current;
                IDictionary<string, object> item = data as IDictionary<string, object>;
                item["Id"] = item["Id"].ToString();
                if (item["Pid"].ToString().IsMissing() || item["Pid"].ToString() == "~" || item["Pid"].ToString() == "#")
                {
                    item["Pid"] = null;
                }
                else
                {
                    dynamic d= DataListForJqGrid.FirstOrDefault(d => (d as IDictionary<string,object>)["Fid"].ToString() == item["Pid"].ToString());
                    if (d == null)
                    {
                        item["Pid"] = null;
                    }
                    else
                    {
                        item["Pid"] = (d as IDictionary<string, object>)["Id"].ToString();
                    }
                }
                item["level"] = item["TreeLevel"].ToInt();
                item["loaded"] = true;
                item["isLeaf"] = item["IsFinal"].ToString() == "1" ? true : false;
                if (item["level"].ToInt() < 2)
                {
                    item["expanded"] = true;
                }
                //else
                //{
                //    item["expanded"] = false;
                //}

            }
            var jsonObj = new JqGridData
            {
                Total = this.PageCount,
                Page = this.CurrentPage,
                Records = this.TotalCount,
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
