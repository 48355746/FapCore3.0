using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Metadata;
using Fap.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Dapper;
using Fap.Core.Rbac.Model;
using Fap.Core.Infrastructure.Query;

namespace Fap.AspNetCore.Model
{
    public class JqgridJsonFilterToSql
    {
        private IDbContext _dbContext;
        IFapPlatformDomain _platformDomain;
        public JqgridJsonFilterToSql(IDbContext dbContext,IFapPlatformDomain platformDomain)
        {
            _dbContext = dbContext;
            _platformDomain = platformDomain;
        }
        private static Dictionary<string, string> Q2Oper;
        private static Dictionary<string, string> Q2OperDesc;
        static JqgridJsonFilterToSql()
        {
            Q2Oper = new Dictionary<string, string>();
            Q2OperDesc = new Dictionary<string, string>();
            //['eq','ne','lt','le','gt','ge','bw','bn','in','ni','ew','en','cn','nc']  
            Q2Oper.Add("eq", " = ");
            Q2OperDesc.Add("eq", "等于");
            Q2Oper.Add("ne", " <> ");
            Q2OperDesc.Add("ne", "不等于");
            Q2Oper.Add("lt", " < ");
            Q2OperDesc.Add("lt", "小于");
            Q2Oper.Add("le", " <= ");
            Q2OperDesc.Add("le", "小于等于");
            Q2Oper.Add("gt", " > ");
            Q2OperDesc.Add("gt", "大于");
            Q2Oper.Add("ge", " >= ");
            Q2OperDesc.Add("ge", "大于等于");
            Q2Oper.Add("bw", " LIKE ");
            Q2OperDesc.Add("bw", "开始于");
            Q2Oper.Add("bn", " NOT LIKE ");
            Q2OperDesc.Add("bn", "不开始于");
            Q2Oper.Add("in", " IN ");
            Q2OperDesc.Add("in", "包含在");
            Q2Oper.Add("ni", " NOT IN ");
            Q2OperDesc.Add("ni", "不包含在");
            Q2Oper.Add("ew", " LIKE ");
            Q2OperDesc.Add("ew", "结束于");
            Q2Oper.Add("en", " NOT LIKE ");
            Q2OperDesc.Add("en", "不结束于");
            Q2Oper.Add("cn", " LIKE ");
            Q2OperDesc.Add("cn", "包含");
            Q2Oper.Add("nc", " NOT LIKE ");
            Q2OperDesc.Add("nc", "不包含");
            Q2Oper.Add("nu", " IS NULL ");
            Q2OperDesc.Add("nu", "为空");
            Q2Oper.Add("nn", " IS NOT NULL ");
            Q2OperDesc.Add("nn", "不为空");
        }
        public static string ExecuteData(string f, string o, string d)
        {
            if (o.Equals("bw") || o.Equals("bn")) return "'" + d + "%'";
            else if (o.Equals("ew") || o.Equals("en")) return "'%" + d + "'";
            else if (o.Equals("cn") || o.Equals("nc")) return "'%" + d + "%'";
            else if (o.Equals("nu") || o.Equals("nn")) return "";
            else if (o.Equals("in") || o.Equals("ni"))
            {
                //sql
                if (d.StartsWith("("))
                {
                    return d;
                }
                else
                {
                    //2，3，4
                    List<string> ins = new List<string>();
                    string[] array = d.Split(',');
                    for (int i = 0; i < array.Length; i++)
                    {
                        ins.Add("'" + array[i] + "'");
                    }
                    return "(" + string.Join(",", ins) + ")";
                }
            }
            else return "'" + d + "'";
        }
        /// <summary>
        /// 格式化过虑条件,针对MC字段特殊处理
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public string BuilderFilter(string tableName, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return "";
            }
            StringBuilder sqlBuilder = new StringBuilder();
            IEnumerable<FapColumn> cols = _platformDomain.ColumnSet.Where(c => c.TableName == tableName);
            if (filter.IsPresent())
            {
                JObject jsono = JObject.Parse(filter);
                if (jsono == null) return "";
                string parentGroup = jsono.GetValue("groupOp").ToString();//AND、OR

                List<JObject> root = new List<JObject>();
                root.Add(jsono);
                RecurseFilter(parentGroup, tableName, root, cols, sqlBuilder);
            }

            string result = sqlBuilder.ToString();
            result = result.Trim().TrimEnd("AND".ToArray()).TrimEnd("OR".ToArray());
            return result;
        }

        /// <summary>
        /// 递归生成过滤条件（字符串）
        /// </summary>
        /// <param name="parentGroup"></param>
        /// <param name="tableName"></param>
        /// <param name="jsonos"></param>
        /// <param name="cols"></param>
        /// <param name="sqlBuilder"></param>
        private void RecurseFilter(string parentGroup, string tableName, IEnumerable<JObject> jsonos, IEnumerable<FapColumn> cols, StringBuilder sqlBuilder)
        {
            foreach (var jsono in jsonos)
            {
                string group = jsono.GetValue("groupOp").ToString();//AND、OR
                List<JObject> rules = jsono.GetValue("rules").Children<JObject>().ToList();
                int i = 0;
                if (rules.Count > 0)
                {
                    StringBuilder empSb = new StringBuilder();
                    foreach (JObject o in rules)
                    {
                        //特殊处理MC参照字段
                        string colName =o.GetValue("field").ToString();
                        string op = o.GetValue("op").ToString();
                        string data = o.GetValue("data").ToString();
                        if (colName.EndsWith("MC", false, System.Globalization.CultureInfo.CurrentCulture))
                        {
                            string tempCol = colName.Substring(0, colName.Length - 2);
                            FapColumn col = cols.FirstOrDefault(c => c.ColName == tempCol);
                            if (col != null)
                            {
                                if (col.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                                {
                                    if (!string.IsNullOrEmpty(op))// && !string.IsNullOrEmpty(data))
                                    {
                                        i++;
                                        //部门特殊处理,开始于
                                        if (col.RefTable.EqualsWithIgnoreCase("orgdept") && op.Trim().EqualsWithIgnoreCase("bw") && !data.StartsWith("("))
                                        {
                                            DynamicParameters param = new DynamicParameters();
                                            param.Add("DeptName", data);
                                            var dept = _dbContext.QueryFirstOrDefaultWhere<OrgDept>("DeptName=@DeptName", param);
                                            if (dept == null)
                                                continue;
                                            if (i != 1)
                                                empSb.Append(" ").Append(group).Append(" ");
                                            empSb.Append(tableName).Append(".").Append(tempCol).Append(" in (select fid from orgdept where deptcode like ").Append("'" + dept.DeptCode + "%'").Append(") ");
                                        }
                                        else
                                        {
                                            //field = executeField(field);
                                            data = ExecuteData(colName, op, data);
                                            if (i != 1)
                                                empSb.Append(" ").Append(group).Append(" ");
                                            if (op.Equals("in") || op.Equals("ni"))
                                            {
                                                empSb.Append(tableName).Append(".").Append(tempCol).Append(" in (select ").Append(col.RefID).Append(" from ").Append(col.RefTable).Append(" where ").Append(col.RefName).Append(Q2Oper[op]).Append(" (").Append(data).Append(")) ");

                                            }
                                            else
                                            {
                                                empSb.Append(tableName).Append(".").Append(tempCol).Append(" in (select ").Append(col.RefID).Append(" from ").Append(col.RefTable).Append(" where ").Append(col.RefName).Append(Q2Oper[op]).Append(data).Append(") ");
                                            }
                                        }
                                    }
                                    continue;
                                }
                            }
                        }
                        string field = tableName + "." + colName;

                        if (!string.IsNullOrEmpty(op))// && !string.IsNullOrEmpty(data))
                        {
                            i++;
                            //field = executeField(field);
                            data = ExecuteData(field, op, data);
                            if (i != 1)
                                empSb.Append(" ").Append(group).Append(" ");
                            empSb.Append(field).Append(Q2Oper[op]).Append(data);
                        }
                    }
                    if (empSb.Length > 0)
                    {
                        sqlBuilder.Append("(");
                        sqlBuilder.Append(empSb.ToString());
                        sqlBuilder.Append(")");
                    }
                    //处理AND和OR
                    sqlBuilder.Append(" ").Append(parentGroup).Append(" ");
                }

                //子条件
                if (jsono.GetValue("groups") != null)
                {
                    List<JObject> subs = jsono.GetValue("groups").Children<JObject>().ToList();
                    //foreach (var sub in subs)
                    //{
                    //    RecurseFilterCondition(tableName, filterConditions, sub);
                    //}

                    RecurseFilter(group, tableName, subs, cols, sqlBuilder);
                }
            }
        }

        /// <summary>
        /// 格式化过虑条件成过虑条件对象
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static FilterCondition BuildFilterCondition(IEnumerable<FapColumn> fapColumns, string filter)
        {
            FilterCondition filterCondition = null;
            if (!string.IsNullOrEmpty(filter))
            {
                string tableName = fapColumns.First().TableName;
                JObject jsono = JObject.Parse(filter);
                if (jsono != null)
                {
                    filterCondition = new FilterCondition();
                    //List<FilterCondition> filterConditions = new List<FilterCondition>();
                    //filterConditions.Add(filterCondition);

                    string group = jsono.GetValue("groupOp").ToString();
                    filterCondition.CombinationType = group;
                    var rulestr = jsono.GetValue("rules");
                    if (rulestr != null)
                    {
                        JEnumerable<JObject> rules = rulestr.Children<JObject>();
                        foreach (JObject o in rules)
                        {
                            string field = o.GetValue("field").ToString();
                            string op = o.GetValue("op").ToString();
                            string data = o.GetValue("data").ToString();
                            if (!string.IsNullOrEmpty(op))// && !string.IsNullOrEmpty(data))
                            {
                                data = ExecuteData(field, op, data);
                                op = Q2Oper[op];
                                if (op.ToLower().Contains("like"))
                                {
                                    if (fapColumns.First(f => f.ColName.EqualsWithIgnoreCase(field)).CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                                    {
                                        field += "MC";
                                    }
                                }
                                filterCondition.AddFilterCondtion(tableName, field, op, data);
                            }
                        }
                    }
                    if (jsono.GetValue("groups") != null)
                    {
                        JEnumerable<JObject> subs = jsono.GetValue("groups").Children<JObject>();
                        //foreach (var sub in subs)
                        //{
                        //    RecurseFilterCondition(tableName, filterConditions, sub);
                        //}

                        RecurseFilterCondition(tableName, fapColumns, filterCondition, subs);
                    }

                }
            }
            return filterCondition;
        }

        /// <summary>
        /// 递归获取过滤条件
        /// </summary>
        private static void RecurseFilterCondition(string tableName, IEnumerable<FapColumn> fapColumns, FilterCondition filterCondition, JEnumerable<JObject> jsonos)
        {
            foreach (var jsono in jsonos)
            {
                //foreach (var rootFilterCondition in rootFilterConditions)
                //{
                FilterCondition groupsCondition = new FilterCondition();

                bool hasGroup = jsono.Property("groupOp") != null;
                if (!hasGroup) return;
                string group = jsono.GetValue("groupOp").ToString();
                groupsCondition.CombinationType = group;

                JEnumerable<JObject> rules = jsono.GetValue("rules").Children<JObject>();
                foreach (JObject o in rules)
                {
                    string field = o.GetValue("field").ToString();
                    string op = o.GetValue("op").ToString();
                    string data = o.GetValue("data").ToString();

                    if (!string.IsNullOrEmpty(op))// && !string.IsNullOrEmpty(data))
                    {
                        data = ExecuteData(field, op, data);
                        op = Q2Oper[op];
                        if (op.ToLower().Contains("like"))
                        {
                            if (fapColumns.First(f => f.ColName.EqualsWithIgnoreCase(field)).CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                            {
                                field += "MC";
                            }
                        }
                        groupsCondition.AddFilterCondtion(tableName, field, op, data);
                    }
                }
                filterCondition.AddGroupFilterCondition(groupsCondition);

                var groupsObj = jsono.GetValue("groups");
                if (groupsObj != null)
                {
                    JEnumerable<JObject> groups = groupsObj.Children<JObject>();

                    RecurseFilterCondition(tableName, fapColumns, groupsCondition, groups);
                }
                //}
            }
        }
        #region 构造条件描述
        /// <summary>
        /// 构造条件描述
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<FilterDescModel> BuilderFilterDesc(string tableName, string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                return null;
            }
            //Dictionary<string, List<JqGridFilterDescViewModel>> dicList = new Dictionary<string, List<JqGridFilterDescViewModel>>();
            List<FilterDescModel> sqlBuilder = new List<FilterDescModel>();

            IEnumerable<FapColumn> cols = _platformDomain.ColumnSet.Where(c => c.TableName == tableName);
            if (!string.IsNullOrEmpty(filter))
            {
                JObject jsono = JObject.Parse(filter);
                if (jsono == null) return null;
                string parentGroup = jsono.GetValue("groupOp").ToString();//AND、OR

                List<JObject> root = new List<JObject>();
                root.Add(jsono);
                RecurseFilterDesc(parentGroup, tableName, root, cols, sqlBuilder);
            }


            return sqlBuilder;
        }

        private void RecurseFilterDesc(string parentGroup, string tableName, IEnumerable<JObject> jsonos, IEnumerable<FapColumn> cols, List<FilterDescModel> sqlBuilder)
        {
            foreach (var jsono in jsonos)
            {
                string group = jsono.GetValue("groupOp").ToString() == "AND" ? "所有" : "任意";//AND、OR
                List<JObject> rules = jsono.GetValue("rules").Children<JObject>().ToList();
                int i = 0;
                if (rules.Count > 0)
                {
                    foreach (JObject o in rules)
                    {
                        //特殊处理MC参照字段
                        string colName = o.GetValue("field").ToString();
                        string op = o.GetValue("op").ToString();
                        string data = o.GetValue("data").ToString();
                        if (colName.EndsWith("MC", false, System.Globalization.CultureInfo.CurrentCulture))
                        {
                            colName = colName.Substring(0, colName.Length - 2);
                        }
                        FapColumn col = cols.FirstOrDefault(c => c.ColName == colName);
                        if (col.CtrlType ==FapColumn.CTRL_TYPE_COMBOBOX)
                        {
                            if(_platformDomain.DictSet.TryGetValueByCodeAndCategory(data, col.RefTable, out FapDict fapDict))
                            {
                                data = fapDict.Name;
                            }
                        }

                        if (!string.IsNullOrEmpty(op))// && !string.IsNullOrEmpty(data))
                        {
                            FilterDescModel model = new FilterDescModel();
                            i++;
                            //field = executeField(field);
                            if (i != 1)
                                model.Group = group;
                            model.FilterDesc = col.ColComment;
                            model.FilterOper = Q2OperDesc[op];
                            model.FilterResult = data;
                            sqlBuilder.Add(model);
                        }
                    }


                    //处理AND和OR
                    //sqlBuilder.Append(" ").Append(parentGroup).Append(" ");
                }

                //子条件
                if (jsono.GetValue("groups") != null)
                {
                    List<JObject> subs = jsono.GetValue("groups").Children<JObject>().ToList();

                    RecurseFilterDesc(group, tableName, subs, cols, sqlBuilder);
                }
            }
        }

        #endregion
    }
}
