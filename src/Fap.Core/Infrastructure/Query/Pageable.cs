using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ardalis.GuardClauses;
using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.MultiLanguage;
using Fap.Core.Utility;

namespace Fap.Core.Infrastructure.Query
{
    /// <summary>
    /// 单表数据查询请求对象
    /// 适用于主表、扩展表的查询
    /// </summary>
    public class Pageable
    {
        private IDbContext _dbContext;
        public Pageable(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        /// <summary>
        /// 历史时间点， 只用于查询历史信息
        /// </summary>
        public string HistoryTimePoint { get; set; }
        public FapTable FapTable
        {
            get
            {
                return _dbContext.Table(TableName);
            }
        }
        /// <summary>
        /// 表名。
        /// 说明：要查询的表
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 分页最大ID条件，存在的时候 可以根据MaxId过滤，提高性能
        /// </summary>
        public long? MaxId { get; set; }
        /// <summary>
        /// 显示的列名。
        /// 说明：如果为空，查询主表和扩展表的列；如果不空，查询指定的列
        /// 举例：m.Name, m.Age, e.HighestEducation
        /// m. 表示主表， e. 表示扩展表
        /// 规则：Where、OrderBy条件中使用m.和e.前缀， m.表示主表的别名， e.表示扩展表的别名，举例：m.Name0, e.Name0
        /// </summary>
        public string QueryCols { get; set; }
        /// <summary>
        /// 要隐藏的列，仅仅用于DataForm
        /// 不改变元数据属性的情况下，不显示列
        /// </summary>
        public string HiddenCols { get; set; } = string.Empty;
        #region Where条件
        /// <summary>
        /// 查询条件
        /// 说明：可以由主表、扩展表的字段作为查询条件
        /// 举例：m.Age>@Age, e.HighestEducation=@HighestEducation
        /// m. 表示主表， e. 表示扩展表
        /// </summary>
        public string Where { get; set; }

        /// <summary>
        /// 增加查询条件
        /// </summary>
        /// <param name="where"></param>
        /// <param name="symbol">AND还是OR</param>
        public void AddWhere(string where, QuerySymbolEnum symbol = QuerySymbolEnum.AND)
        {
            if (Where.IsMissing())
            {
                Where = where;
            }
            else
            {
                if (symbol == QuerySymbolEnum.AND)
                {
                    Where += " AND (" + where + ")";
                }
                else if (symbol == QuerySymbolEnum.OR)
                {
                    Where += " OR (" + where + ")";
                }
            }
        }

        #endregion    
        /// <summary>
        /// 排序条件
        /// 说明：可以由主表、扩展表的字段作为排序条件
        /// 举例：m.Age desc, e.HighestEducation
        /// m. 表示主表， e. 表示扩展表
        /// </summary>
        public OrderByCondition OrderBy { get; set; } = new OrderByCondition();

        /// <summary>
        /// 是否要查询扩展表的数据
        /// </summary>
        public bool IsQueryExtTable { get; set; }

        #region 预处理参数
        /// <summary>
        /// 添加预处理参数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddParameter(string key, object value)
        {
            if (key != null && key.StartsWith("@"))
            {
                key = key.TrimStart('@');
            }
            if (Parameters.ContainsKey(key))
            {
                return;
            }
            if (value == null)
            {
                Parameters.Add(key, "");
                return;
            }
            Parameters.Add(key, value);

        }

        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

        #endregion

        #region 处理统计字段
        /// <summary>
        /// 增加统计字段
        /// </summary>
        /// <param name="key"></param>
        /// <param name="statSymbol"></param>
        public void AddStatField(string key, StatSymbolEnum statSymbol)
        {
            if (string.IsNullOrWhiteSpace(key) || statSymbol == StatSymbolEnum.None) return;
            if (StatFields.ContainsKey(key)) return;

            StatFields.Add(key, statSymbol);
        }
        /// <summary>
        /// 增加统计字段
        /// </summary>
        /// <param name="statSet"></param>
        public void AddStatField(List<StatSet> statSet)
        {
            if (statSet == null || statSet.Count == 0) return;

            foreach (StatSet item in statSet)
            {
                this.AddStatField(item.Field, item.StatSymbol);
            }
        }

        public Dictionary<string, StatSymbolEnum> StatFields { get; set; } = new Dictionary<string, StatSymbolEnum>();

        #endregion



        [NonSerialized]
        private SimpleQueryOptionBuilder _wraper;

        /// <summary>
        /// QueryOption对象的包装组装类，用于SQL的组装
        /// </summary>
        public SimpleQueryOptionBuilder Wraper
        {
            get
            {
                if (_wraper == null)
                {
                    _wraper = new SimpleQueryOptionBuilder(this, _dbContext);
                }
                return _wraper;
            }
        }

        /// <summary>
        /// 当前页码，从1开始
        /// 说明：适用于分页查询
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// 每页的记录个数
        /// 说明：适用于分页查询
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 记录总数量
        /// 说明：适用于分页查询
        /// </summary>

        public int TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages
        {
            get
            {
                return (TotalCount + PageSize - 1) / PageSize;
            }
        }

        /// <summary>
        /// 开始分页偏移量
        /// </summary>
        public int Offset
        {
            get
            {
                if (MaxId != null)
                {
                    return 0;
                }
                else
                {
                    return (CurrentPage - 1) * PageSize;
                }
            }
        }
    }

    /// <summary>
    /// SimpleQueryOption的组装类，用于SQL的组装
    /// </summary>
    public sealed class SimpleQueryOptionBuilder
    {
        private Pageable _pageAble;
        private IDbContext _dbContext;
        public SimpleQueryOptionBuilder(Pageable pageAble, IDbContext dbContext)
        {
            _pageAble = pageAble;
            _dbContext = dbContext;
        }
        /// <summary>
        /// 主表
        /// </summary>
        public FapTable MainTable
        {
            get
            {
                var _mainTable = _dbContext.Table(_pageAble.TableName);

                return _mainTable;
            }
        }

        public IEnumerable<FapColumn> MainColumnList
        {
            get
            {
                var _mainColumnList = _dbContext.Columns(_pageAble.TableName);

                return _mainColumnList;
            }
        }

        //public FapTable ExtTable
        //{
        //    get
        //    {
        //        if (!_pageAble.IsQueryExtTable)
        //        {
        //            return null;
        //        }
        //        FapTable mainTable = MainTable;
        //        if (mainTable.MainTable.IsPresent())
        //        {
        //            return _dbContext.Table(MainTable.ExtTable);
        //        }
        //        return null;
        //    }
        //}

        //public IEnumerable<FapColumn> ExtColumnList
        //{
        //    get
        //    {
        //        if (!_pageAble.IsQueryExtTable)
        //        {
        //            return null;
        //        }

        //        FapTable mainTable = MainTable;
        //        if (mainTable.ExtTable.IsPresent())
        //        {
        //            return _dbContext.Columns(MainTable.ExtTable);
        //        }

        //        return null;
        //    }
        //}
        /// <summary>
        /// 生成Select的SQL
        /// </summary>
        public string MakeSelectSql()
        {
            //处理要显示的字段
            if (_pageAble.QueryCols.IsPresent())
            {
                return _pageAble.QueryCols;
            }

            return "*";
        }


        /// <summary>
        /// 组装成From部分的SQL
        /// </summary>
        /// <returns></returns>
        public string MakeFromSql()
        {
            return MainTable.TableName;

        }
        public string MakeWhere()
        {
            if (_pageAble.Where.IsPresent())
            {
                return $" where {_pageAble.Where}";
            }
            return string.Empty;
        }
        public string MakeExportSql()
        {
            //Orderby条件
            string orderBy = MakeOrderBySql();
            if (!string.IsNullOrEmpty(orderBy))
            {
                orderBy = $" ORDER BY {orderBy} ";
            }
            else
            {
                orderBy = " order by Id ";
            }
            //Join条件
            string join = string.Empty; //MakeJoinSql();
            string where = MakeWhere();
            return $"select {MakeSelectSql()} from {MakeFromSql()} {join} {where} {orderBy} ";

        }
        /// <summary>
        /// 组装成Join部分的SQL
        /// </summary>
        /// <returns></returns>
        //public string MakeJoinSql()
        //{
        //    StringBuilder sqlBuilder = new StringBuilder();
        //    //查询扩展表
        //    if (_pageAble.IsQueryExtTable)
        //    {
        //        FapTable mainTable = MainTable;
        //        if (mainTable.ExtTable.IsPresent())
        //        {
        //            sqlBuilder.Append(" LEFT JOIN ").Append(mainTable.ExtTable);
        //            sqlBuilder.Append(" ON ");
        //            sqlBuilder.Append(mainTable.ExtTable + ".Fid=").Append(_pageAble.TableName + ".Fid");
        //        }
        //    }

        //    return sqlBuilder.ToString();
        //}

        /// <summary>
        /// 组装Orderby部分的SQL语句
        /// </summary>
        /// <returns></returns>
        public string MakeOrderBySql()
        {
            string orderBy = string.Join(',', _pageAble.OrderBy?.OrderByConditions.Select(o => $"{o.Field} {o.Order}"));
            if (orderBy.IsMissing())
            {
                List<FapColumn> columnList = MainColumnList.Where(c => !string.IsNullOrWhiteSpace(c.SortDirection)).ToList();
                if (columnList != null && columnList.Count > 0)
                {
                    return string.Join(',', columnList.Select(o => $"{o.ColName} {o.SortDirection}"));

                }
                else
                {
                    //默认Id倒序
                    return "Id DESC";
                }
            }
            return orderBy;

        }

    }

    #region 过滤条件的相关类定义
    /// <summary>
    /// 过滤条件对象项
    /// </summary>
    public class FilterConditionItem
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 字段名
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 操作符
        /// </summary>
        public string Operator { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// 是否MC字段
        /// </summary>
        public bool HasMC
        {
            get
            {
                return Field.EndsWith("MC");
            }
        }

        /// <summary>
        /// 获取原始的字段名
        /// </summary>
        public string OrginalField
        {
            get
            {
                if (HasMC)
                {
                    return Field.Substring(0, Field.LastIndexOf("MC"));
                }
                else
                {
                    return Field;
                }
            }
        }
    }


    #endregion

    #region 排序的相关类定义
    /// <summary>
    /// 排序条件项
    /// </summary>
    public class OrderByConditionItem
    {
        public string Field { get; set; }

        private string _order = "ASC";
        public string Order
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_order))
                {
                    _order = "ASC";
                }
                return _order;
            }
            set
            {
                _order = value;
            }
        }

        /// <summary>
        /// 是否MC字段
        /// </summary>
        public bool HasMC
        {
            get
            {
                return Field.EndsWith("MC");
            }
        }

        /// <summary>
        /// 获取原始的字段名
        /// </summary>
        public string OrginalField
        {
            get
            {
                if (HasMC)
                {
                    return Field.Substring(0, Field.LastIndexOf("MC"));
                }
                else
                {
                    return Field;
                }
            }
        }
    }

    /// <summary>
    /// 排序条件
    /// </summary>
    public class OrderByCondition
    {
        /// <summary>
        /// 排序条件集合
        /// </summary>
        public List<OrderByConditionItem> OrderByConditions { get; } = new List<OrderByConditionItem>();

        /// <summary>
        /// 添加过滤条件
        /// </summary>
        /// <param name="tableName">字段所在的表名</param>
        /// <param name="field">字段名</param>
        /// <param name="op">操作符</param>
        /// <param name="data">值</param>
        public void AddOrderByCondtion(string field, string order)
        {
            if (OrderByConditions.Exists(o => o.Field == field))
            {
                return;
            }
            OrderByConditions.Add(new OrderByConditionItem() { Field = field, Order = order });
        }
    }
    #endregion

}
