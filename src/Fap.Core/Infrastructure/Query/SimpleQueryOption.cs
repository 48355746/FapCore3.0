using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ardalis.GuardClauses;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Metadata;
using Fap.Core.MultiLanguage;
using Fap.Core.Utility;

namespace Fap.Core.Infrastructure.Query
{
    /// <summary>
    /// 单表数据查询请求对象
    /// 适用于主表、扩展表的查询
    /// </summary>
    public class SimpleQueryOption
    {
        private IFapPlatformDomain _platformDomain;
        private IFapApplicationContext _applicationContext;
        public SimpleQueryOption(IFapPlatformDomain platformDomain, IFapApplicationContext applicationContext)
        {
            _platformDomain = platformDomain;
            _applicationContext = applicationContext;
        }
        /// <summary>
        /// 时间点， 只用于查询历史信息
        /// </summary>
        public string TimePoint { get; set; }
        public FapTable FapTable
        {
            get
            {
                return _platformDomain.TableSet.FirstOrDefault(t => t.TableName == TableName);
            }
        }
        /// <summary>
        /// 表名。
        /// 说明：要查询的表
        /// </summary>
        public string TableName { get; set; }

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
        private string _where;
        /// <summary>
        /// 查询条件
        /// 说明：可以由主表、扩展表的字段作为查询条件
        /// 举例：m.Age>@Age, e.HighestEducation=@HighestEducation
        /// m. 表示主表， e. 表示扩展表
        /// </summary>
        public string Where
        {
            get { return _where; }
            set { _where = value; }
        }
        /// <summary>
        /// 增加查询条件
        /// </summary>
        /// <param name="where"></param>
        /// <param name="symbol">AND还是OR</param>
        public void AddWhere(string where, QuerySymbolEnum symbol = QuerySymbolEnum.AND)
        {
            if (_where.IsMissing())
            {
                _where = where;
            }
            else
            {
                if (symbol == QuerySymbolEnum.AND)
                {
                    _where += " AND (" + where + ")";
                }
                else if (symbol == QuerySymbolEnum.OR)
                {
                    _where += " OR (" + where + ")";
                }
            }
        }

        #endregion

        #region  过滤条件    

        private FilterCondition _filterCondition;

        /// <summary>
        /// 过滤条件对象
        /// </summary>
        public FilterCondition FilterCondition
        {
            get
            {
                return _filterCondition;
            }
            set
            {
                _filterCondition = value;
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

        /// <summary>
        /// 获取原始SQL语句
        /// 导出数据sql
        /// </summary>
        public string OrginSql
        {
            get
            {
                string currentDate = DateTimeUtils.CurrentDateTimeStr;
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append("SELECT ");
                sqlBuilder.Append(this.Wraper.MakeSelectSql().Replace("@CurrentDate", "'" + currentDate + "'").Replace(this.TableName + "_", "", StringComparison.OrdinalIgnoreCase));
                sqlBuilder.Append(" FROM ").Append(this.TableName);
                //string valideWhere = this.Wraper.MakeWhereSql().Replace("@CurrentDate","'"+currentDate+"'");
                sqlBuilder.Append(" WHERE ").Append(" 1=1 ");
                //if (!string.IsMissing(this.Where))
                //{
                //    sqlBuilder.Append(" and ").Append(this.Where);
                //}
                sqlBuilder.Append(" and ").Append(this.Wraper.MakeWhereSql().Replace("@CurrentDate", "'" + currentDate + "'"));
                if (this.FilterCondition != null)
                {
                    sqlBuilder.Append(" and ").Append(this.Wraper.MakeFilterSql());
                }
                if (this.OrderBy != null)
                {
                    sqlBuilder.Append(" ORDER BY ").Append(this.Wraper.MakeOrderBySql());
                }
                return sqlBuilder.ToString();
            }
        }



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
                    _wraper = new SimpleQueryOptionBuilder(this, _platformDomain, _applicationContext);
                }
                return _wraper;
            }
        }

        /// <summary>
        /// 当前页码，从1开始
        /// 说明：适用于分页查询
        /// </summary>
        public int CurrentPageIndex { get; set; }
        /// <summary>
        /// 每页的个数
        /// 说明：适用于分页查询
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 记录总数量
        /// 说明：适用于分页查询
        /// </summary>
        [NonSerialized]
        private int _totalNum = 0;
        public int TotalNum { get { return _totalNum; } set { _totalNum = value; } }

        [NonSerialized]
        private int _totalPage = 0;
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage
        {
            get
            {
                _totalPage = (TotalNum + PageCount - 1) / PageCount;
                return _totalPage;
            }
        }

        [NonSerialized]
        private int _recordFirstIndex = 0;
        /// <summary>
        /// 当前页上开始记录Index
        /// </summary>
        public int RecordFirstIndex
        {
            get
            {
                _recordFirstIndex = (CurrentPageIndex - 1) * PageCount + 1;
                return _recordFirstIndex;
            }
        }

        [NonSerialized]
        private int _recordLastIndex = 0;
        /// <summary>
        /// 当前页上结束记录Index
        /// </summary>
        public int RecordLastIndex
        {
            get
            {
                _recordLastIndex = CurrentPageIndex * PageCount;
                return _recordLastIndex;
            }
        }
    }

    /// <summary>
    /// SimpleQueryOption的组装类，用于SQL的组装
    /// </summary>
    public sealed class SimpleQueryOptionBuilder
    {
        private SimpleQueryOption _queryOption;
        private IFapPlatformDomain _platformDomain;
        public SimpleQueryOptionBuilder(SimpleQueryOption queryOption, IFapPlatformDomain platformDomain, IFapApplicationContext applicationContext)
        {
            _queryOption = queryOption;
            _platformDomain = platformDomain;
        }

        /// <summary>
        /// 最后返回的字段对象集合
        /// </summary>
        public List<FapColumn> ResultColumns { get; } = new List<FapColumn>();

        /// <summary>
        /// 主表
        /// </summary>
        public FapTable MainTable
        {
            get
            {
                var _mainTable = _platformDomain.TableSet.FirstOrDefault(t => t.TableName == _queryOption.TableName);

                if (_mainTable == null)
                {
                    Guard.Against.Null(_mainTable, "主表元数据");
                }
                return _mainTable;
            }
        }

        public IEnumerable<FapColumn> MainColumnList
        {
            get
            {
                var _mainColumnList = _platformDomain.ColumnSet.Where(c => c.TableName == _queryOption.TableName);
                if (_mainColumnList == null || _mainColumnList.Count() == 0)
                {
                    Guard.Against.Null(_mainColumnList, "主表字段元数据");
                }

                return _mainColumnList;
            }
        }

        public FapTable ExtTable
        {
            get
            {
                if (!_queryOption.IsQueryExtTable)
                {
                    return null;
                }
                FapTable mainTable = MainTable;
                if (mainTable.ExtTable.IsPresent())
                {
                    return _platformDomain.TableSet.FirstOrDefault(t => t.TableName == MainTable.ExtTable);
                }
                return null;
            }
        }

        public IEnumerable<FapColumn> ExtColumnList
        {
            get
            {
                if (!_queryOption.IsQueryExtTable)
                {
                    return null;
                }

                FapTable mainTable = MainTable;
                if (mainTable.ExtTable.IsPresent())
                {
                    return _platformDomain.ColumnSet.Where(t => t.TableName == MainTable.ExtTable);
                }

                return null;
            }
        }

        private List<SqlColumnInfo> ParseOrginSelectSql(string columnSqlSegment)
        {
            List<SqlColumnInfo> columnInfoList = new List<SqlColumnInfo>();
            string[] columnInfos = columnSqlSegment.Split(',');
            foreach (var str in columnInfos)
            {
                if (str.IsMissing()) continue;

                string item = str.Trim();
                SqlColumnInfo columnInfo = ParseColumnStr(item);
                if (columnInfo != null)
                {
                    columnInfoList.Add(columnInfo);
                }
            }

            return columnInfoList.OrderBy(c => c.ColumnOrder).ToList();
        }

        /// <summary>
        /// 解析select前面部分的SQL
        /// </summary>
        /// <param name="columnSql"></param>
        /// <returns></returns>
        private SqlColumnInfo ParseColumnStr(string columnSql)
        {
            SqlColumnInfo columnInfo = null;
            string[] temp = columnSql.Split('.');
            if (temp.Length > 1)
            {
                if ("m".Equals(temp[0], StringComparison.CurrentCultureIgnoreCase))
                {
                    columnInfo = new SqlColumnInfo();
                    columnInfo.TableAlias = MainTable.TableName;
                    columnInfo.ColumnName = temp[1];
                    columnInfo.ColumnAlias = columnInfo.ColumnName;// columnInfo.TableAlias + "_" + columnInfo.ColumnName;
                    columnInfo.ColumnInCorrespond = _platformDomain.ColumnSet.FirstOrDefault(c => c.TableName == MainTable.TableName && c.ColName == columnInfo.ColumnName);
                    columnInfo.ColumnOrder = columnInfo.ColumnInCorrespond.ColOrder;
                }
                else if ("e".Equals(temp[0], StringComparison.CurrentCultureIgnoreCase))
                {
                    //查询扩展表
                    if (_queryOption.IsQueryExtTable)
                    {
                        if (ExtTable != null)
                        {
                            columnInfo = new SqlColumnInfo();
                            columnInfo.TableAlias = ExtTable.TableName;
                            columnInfo.ColumnName = temp[1];
                            columnInfo.ColumnAlias = columnInfo.ColumnName;// columnInfo.TableAlias + "_" + columnInfo.ColumnName;
                            columnInfo.ColumnInCorrespond = _platformDomain.ColumnSet.FirstOrDefault(c => c.TableName == ExtTable.TableName && c.ColName == columnInfo.ColumnName);
                            columnInfo.ColumnOrder = columnInfo.ColumnInCorrespond.ColOrder;
                        }
                    }
                }
            }
            else
            {
                columnInfo = new SqlColumnInfo();
                columnInfo.TableAlias = MainTable.TableName;
                columnInfo.ColumnName = temp[0];
                columnInfo.ColumnAlias = columnInfo.TableAlias + "_" + columnInfo.ColumnName;
                columnInfo.ColumnInCorrespond = _platformDomain.ColumnSet.FirstOrDefault(c => c.TableName == MainTable.TableName && c.ColName == columnInfo.ColumnName);
                columnInfo.ColumnOrder = columnInfo.ColumnInCorrespond.ColOrder;
            }
            return columnInfo;
        }


        /// <summary>
        /// 生成Select的SQL
        /// </summary>
        public string MakeSelectSql()
        {
            //处理要显示的字段
            if (_queryOption.QueryCols.IsPresent())
            {
                return _queryOption.QueryCols;
            }

            return "*";
        }
        /// <summary>
        /// 构造参照名称字段
        /// </summary>
        /// <param name="refColumn"></param>
        /// <returns></returns>
        private static FapColumn GeneralReferenceColumn(FapColumn refColumn)
        {
            FapColumn refMCColumn = (FapColumn)refColumn.Clone();
            refMCColumn.IsCustomColumn = 1; //属于自定义字段
            refMCColumn.ColName = refMCColumn.ColName + "MC";
            refMCColumn.CtrlType = FapColumn.CTRL_TYPE_TEXT;
            refMCColumn.ShowAble = 0;//不显示
            return refMCColumn;
        }

        /// <summary>
        /// 组装成From部分的SQL
        /// </summary>
        /// <returns></returns>
        public string MakeFromSql()
        {
            return MainTable.TableName;

        }

        /// <summary>
        /// 组装成Join部分的SQL
        /// </summary>
        /// <returns></returns>
        public string MakeJoinSql()
        {
            StringBuilder sqlBuilder = new StringBuilder();
            //查询扩展表
            if (_queryOption.IsQueryExtTable)
            {
                FapTable mainTable = MainTable;
                if (mainTable.ExtTable.IsPresent())
                {
                    sqlBuilder.Append(" LEFT JOIN ").Append(mainTable.ExtTable);
                    sqlBuilder.Append(" ON ");
                    sqlBuilder.Append(mainTable.ExtTable + ".Fid=").Append(_queryOption.TableName + ".Fid");
                }
            }

            return sqlBuilder.ToString();
        }
        /// <summary>
        /// 组装成Where部分的SQL
        /// </summary>
        /// <returns></returns>
        public string MakeWhereSql()
        {
            //过滤条件
            string filter = MakeFilterSql();
            //预处理：加上有效时间
            if (_queryOption.Where.IsPresent())
            {
                if (filter.IsPresent())
                {
                    return $"({_queryOption.Where}) and ({filter})";
                }
                else
                {
                    return _queryOption.Where;
                }
            }
            else
            {
                if (filter.IsPresent())
                {
                    filter = $" ({filter}) ";
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 组装filter部分的SQL语句
        /// </summary>
        /// <returns></returns>
        public string MakeFilterSql()
        {
            if (_queryOption.FilterCondition != null)
            {
                StringBuilder sqlBuilder = new StringBuilder();
                List<FilterCondition> rootFilterConditions = new List<FilterCondition>();
                rootFilterConditions.Add(_queryOption.FilterCondition);

                this.BuilderSubFilterSql(_queryOption.FilterCondition.CombinationType, rootFilterConditions, sqlBuilder);
                if (sqlBuilder.Length > 0)
                {
                    //去掉最后的AND、OR
                    sqlBuilder.Length = sqlBuilder.Length - _queryOption.FilterCondition.CombinationType.Length;
                }
                else
                {
                    return "";
                }
                string result = sqlBuilder.ToString();
                result = result.TrimEnd("AND".ToArray()).TrimEnd("OR".ToArray());
                return result;
            }

            return "";
        }

        private void BuilderSubFilterSql(string operatorType, List<FilterCondition> filterConditions, StringBuilder sqlBuilder)
        {
            if (filterConditions == null) return;

            foreach (var filterCondition in filterConditions)
            {
                if (filterCondition != null)
                {

                    if (filterCondition.FilterConditions.Count > 0)
                    {
                        sqlBuilder.Append(" (");
                        //处理主条件
                        foreach (var item in filterCondition.FilterConditions)
                        {
                            sqlBuilder.Append(" ");
                            if (!item.HasMC) //正常字段
                            {
                                if (item.Operator.Trim().EqualsWithIgnoreCase("IS NULL"))
                                {
                                    sqlBuilder.Append("(").Append(item.TableName).Append(".").Append(item.Field).Append(" ").Append(item.Operator).Append(" OR ")
                                        .Append(item.TableName).Append(".").Append(item.Field).Append("='')");
                                }
                                else if (item.Operator.Trim().EqualsWithIgnoreCase("IS NOT NULL"))
                                {
                                    sqlBuilder.Append("(").Append(item.TableName).Append(".").Append(item.Field).Append(" ").Append(item.Operator).Append(" AND ")
                                        .Append(item.TableName).Append(".").Append(item.Field).Append("!='')");
                                }
                                else
                                {
                                    sqlBuilder.Append(item.TableName).Append(".").Append(item.Field).Append(" ").Append(item.Operator).Append(item.Data);
                                }
                            }
                            else //MC字段
                            {
                                FapColumn column = _platformDomain.ColumnSet.FirstOrDefault(c => c.TableName == item.TableName && c.ColName == item.OrginalField);
                                if (column != null)
                                {
                                    if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                                    { //参照类型                                       

                                        sqlBuilder.Append(item.TableName).Append(".").Append(item.OrginalField).Append(" IN (");

                                        string refWhere = string.Empty;
                                        if (item.Operator.Trim().EqualsWithIgnoreCase("IS NULL"))
                                        {
                                            refWhere = "(" + column.RefName + " IS NULL OR " + column.RefName + "='')";
                                        }
                                        else if (item.Operator.Trim().EqualsWithIgnoreCase("IS NOT NULL"))
                                        {
                                            refWhere = "(" + column.RefName + " IS NOT NULL AND " + column.RefName + "!='')";
                                        }
                                        else
                                        {
                                            refWhere = column.RefName + item.Operator + item.Data;
                                        }
                                        sqlBuilder.AppendFormat("select {0} as code from {1} where {2}", column.RefID, column.RefTable, refWhere);
                                        //}
                                        sqlBuilder.Append(")");
                                    }
                                    else if (column.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX) //下拉框类型
                                    {
                                        sqlBuilder.Append(item.TableName).Append(".").Append(item.Field).Append(" ").Append(item.Operator).Append(item.Data);
                                    }
                                }
                            }

                            //处理AND和OR
                            sqlBuilder.Append(" ").Append(filterCondition.CombinationType);
                        }

                        sqlBuilder.Length = sqlBuilder.Length - filterCondition.CombinationType.Length;

                        sqlBuilder.Append(") ");

                        //处理AND和OR
                        if (!string.IsNullOrWhiteSpace(operatorType))
                        {
                            sqlBuilder.Append(operatorType);
                        }
                    }


                    //处理子条件
                    if (filterCondition.GroupsFilterCondition != null && filterCondition.GroupsFilterCondition.Count > 0)
                    {
                        this.BuilderSubFilterSql(filterCondition.CombinationType, filterCondition.GroupsFilterCondition, sqlBuilder);
                    }
                }
            }

        }

        /// <summary>
        /// 组装Orderby部分的SQL语句
        /// </summary>
        /// <returns></returns>
        public string MakeOrderBySql()
        {
            string orderBy = string.Join(',', _queryOption.OrderBy?.OrderByConditions.Select(o => $"{o.Field} {o.Order}"));
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

    /// <summary>
    /// 过滤条件对象
    /// </summary>
    [Serializable]
    public class FilterCondition
    {
        private string _combinationType = "AND";
        /// <summary>
        /// 过滤条件项之间的组合关系， "AND"、"OR"
        /// </summary>
        public string CombinationType
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_combinationType))
                {
                    _combinationType = "AND";
                }
                return _combinationType.Trim();
            }
            set
            {
                _combinationType = value;
            }
        }


        private List<FilterConditionItem> _filterConditions = new List<FilterConditionItem>();
        /// <summary>
        /// 过滤条件集合
        /// </summary>
        public List<FilterConditionItem> FilterConditions
        {
            get
            {
                return _filterConditions;
            }
        }
        /// <summary>
        /// 添加过滤条件
        /// </summary>
        /// <param name="tableName">字段所在的表名</param>
        /// <param name="field">字段名</param>
        /// <param name="op">操作符</param>
        /// <param name="data">值</param>
        public void AddFilterCondtion(string tableName, string field, string op, string data)
        {
            _filterConditions.Add(new FilterConditionItem() { TableName = tableName, Field = field, Operator = op, Data = data });
        }


        private List<FilterCondition> _subFilterCondition = new List<FilterCondition>();
        /// <summary>
        /// 分组滤条件， 对应的是groups
        /// </summary>
        public List<FilterCondition> GroupsFilterCondition
        {
            get
            {
                return _subFilterCondition;
            }
        }

        public void AddGroupFilterCondition(FilterCondition groupsFilterCondition)
        {
            this._subFilterCondition.Add(groupsFilterCondition);
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
