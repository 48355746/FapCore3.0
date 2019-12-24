using Fap.AspNetCore.Model;
using Fap.AspNetCore.ViewModel;
using Fap.Core.DI;
using System;
using Fap.Core.Extensions;
using System.Collections.Generic;
using System.Text;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.DataAccess;
using System.Linq;
using Fap.Core.Rbac;
using Dapper;
using Fap.Core.Infrastructure.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Ardalis.GuardClauses;
using Fap.AspNetCore.Infrastructure;
using Microsoft.AspNetCore.Antiforgery;
using System.Threading.Tasks;
using Fap.AspNetCore.Extensions;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Fap.Core.Utility;
using System.IO;
using Fap.Core.Office.Excel.Export;
using Fap.Core.Office;

namespace Fap.AspNetCore.Serivce
{
    [Service(Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton)]
    public class GridFormService : IGridFormService
    {
        private readonly IFapApplicationContext _applicationContext;
        private readonly IDbContext _dbContext;
        private readonly IRbacService _rbacService;
        private IAntiforgery _antiforgery;
        private readonly IOfficeService _officeService;
        private readonly ILogger<GridFormService> _logger;
        public GridFormService(
            IFapApplicationContext fapApplicationContext,
            ILogger<GridFormService> logger,
            IOfficeService officeService,
            IDbContext dbContext,
            IRbacService rbacService,
            IAntiforgery antiforgery)
        {
            _dbContext = dbContext;
            _applicationContext = fapApplicationContext;
            _rbacService = rbacService;
            _antiforgery = antiforgery;
            _officeService = officeService;
            _logger = logger;
        }
        public JqGridData QueryPageDataResultView(JqGridPostData jqGridPostData)
        {
            Pageable queryOption = AnalysisPostData(jqGridPostData);
            //queryOption.Where = AnalysisWhere(queryOption.Where);
            PageDataResultView result = QueryPagedDynamicData();
            return result.GetJqGridJsonData();

            PageDataResultView QueryPagedDynamicData()
            {
                try
                {
                    IEnumerable<FapColumn> fapColumns = _dbContext.Columns(jqGridPostData.QuerySet.TableName);
                    PageInfo<dynamic> pi = _dbContext.QueryPage(queryOption);

                    //组装成DataResultView对象
                    PageDataResultView dataResultView = new PageDataResultView();
                    dataResultView.Data = pi.Items.ToFapDynamicObjectList(fapColumns);
                    //当未获取数据的时候才获取默认值
                    //if (!dataObject.Data.Any())
                    //{
                    //wyf表单应用，表格暂时不用取默认值
                    //dataResultView.DefaultData = queryOption.Wraper.GetDefaultData();
                    //}
                    dataResultView.DataJson = JsonConvert.SerializeObject(pi.Items);
                    dataResultView.TotalNum = pi.TotalPages;
                    dataResultView.CurrentPageIndex = pi.PageNumber;
                    dataResultView.OrginData = pi.Items;
                    dataResultView.DataListForJqGrid = pi.Items;// as IEnumerable<IDictionary<string, object>>;
                    dataResultView.PageCount = pi.PageSize;
                    //统计字段
                    dataResultView.StatFieldData = pi.StatFieldData;
                    dataResultView.StatFieldDataJson = JsonConvert.SerializeObject(pi.StatFieldData);

                    return dataResultView;

                }
                catch (Exception)
                {
                    throw;
                }

            }
        }
        #region private
        private Pageable AnalysisPostData(JqGridPostData jqGridPostData)
        {
            IEnumerable<FapColumn> fapColumns = _dbContext.Columns(jqGridPostData.QuerySet.TableName);
            jqGridPostData.Filters = jqGridPostData.Filters.IsPresent() ? jqGridPostData.Filters.Replace("query ", "select ") : "";
            //矫正当前页为0的情况
            if (jqGridPostData.Page < 0)
            {
                jqGridPostData.Page = 1;
            }
            QuerySet qs = jqGridPostData.QuerySet;
            Pageable pageable = new Pageable(_dbContext) { TableName = qs.TableName, QueryCols = qs.QueryCols, HistoryTimePoint = jqGridPostData.TimePoint };
            //设置统计
            if (qs.Statsetlist != null && qs.Statsetlist.Any())
            {
                pageable.AddStatField(qs.Statsetlist);
            }
            if (qs.Parameters != null && qs.Parameters.Count > 0)
            {
                foreach (var param in qs.Parameters)
                {
                    pageable.AddParameter(param.ParamKey, param.ParamValue);
                }

            }
            //优先级高
            if (jqGridPostData.Sidx.IsPresent())
            {
                var sidxs = jqGridPostData.Sidx.SplitComma();
                foreach (var sidx in sidxs)
                {
                    if (sidx.IsPresent())
                    {
                        string[] odx = sidx.Trim().Split(' ');
                        if (odx != null)
                        {
                            var col = fapColumns.First(f => f.ColName == odx[0]);
                            string colName = col.ColName;
                            if (col.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                            {
                                colName += "MC";
                            }
                            if (odx.Length > 1)
                            {
                                pageable.OrderBy.AddOrderByCondtion(colName, odx[1]);
                            }
                            else
                            {
                                pageable.OrderBy.AddOrderByCondtion(colName, jqGridPostData.Sord);
                            }
                        }
                    }
                }
            }
            if (qs.OrderByList != null && qs.OrderByList.Count > 0)
            {
                foreach (var orderby in qs.OrderByList)
                {
                    pageable.OrderBy.AddOrderByCondtion(orderby.Field, orderby.Direction);

                }
            }

            //构造初始化条件,如果没有过滤条件，又设置了初始化条件则设置初始化条件。或者设置了过滤条件且初始化条件为全局条件则同样设置where条件
            if (qs.GlobalWhere.IsPresent())
            {
                pageable.Where = qs.GlobalWhere;
            }
            if (jqGridPostData.Filters.IsMissing() && qs.InitWhere.IsPresent())
            {
                if (pageable.Where.IsMissing())
                {
                    pageable.Where = qs.InitWhere;
                }
                else
                {
                    pageable.Where += " and " + qs.InitWhere;
                }
            }

            //页面级条件
            if (jqGridPostData.PageCondition.IsPresent())
            {
                JsonFilterToSql jfs = new JsonFilterToSql(_dbContext);
                if (qs.GlobalWhere.IsPresent())
                {
                    pageable.AddWhere(jfs.BuilderFilter(pageable.TableName, jqGridPostData.PageCondition), QuerySymbolEnum.AND);
                }
                else
                {
                    pageable.Where = jfs.BuilderFilter(pageable.TableName, jqGridPostData.PageCondition);
                }
            }
            //构造jqgrid过滤条件
            if (jqGridPostData.Filters.IsPresent())
            {
                //string strFilter = JqGridHelper.BuilderFilter(pageable.TableName, model.Filters);
                //if (!string.IsNullOrWhiteSpace(strFilter))
                //{
                //    pageable.Filter = strFilter;
                //}
                FilterCondition filterCondition = JsonFilterToSql.BuildFilterCondition(fapColumns, jqGridPostData.Filters);
                if (filterCondition != null)
                {
                    pageable.FilterCondition = filterCondition;
                }
            }
            //事件处理
            //actionSimplepageable?.Invoke(pageable);
            pageable.PageNumber = jqGridPostData.Page;
            pageable.PageSize = jqGridPostData.Rows;
            //数据权限
            string dataWhere = _rbacService.GetRoleDataWhere(qs.TableName);
            if (dataWhere.IsPresent())
            {
                if (pageable.Where.IsPresent())
                {
                    pageable.Where += " and  " + dataWhere;
                }
                else
                {
                    pageable.Where = dataWhere;
                }
            }
            //解析条件
            pageable.Where = AnalysisWhere(pageable.Where);
            return pageable;
            string AnalysisWhere(string where)
            {
                if (where.IsMissing())
                {
                    return "";
                }
                //获得安全sql
                where = where.FilterDangerSql();
                //替换部门权限占位符
                return where.Replace(FapPlatformConstants.DepartmentAuthority, _rbacService.GetUserDeptAuthorityWhere()).ReplaceIgnoreCase("query", "select ");
            }
        }
        #endregion
        private static object lockSave = new object();
        public async Task<ResponseViewModel> PersistenceAsync(IFormCollection formCollection)
        {
            //操作符 
            OperEnum operEnum = GetOperEnum();
            if (operEnum == OperEnum.none)
            {
                return ResponseViewModelUtils.Failure("未知的持久化操作符");
            }
            string tableName = GetTableName();
            if (tableName.IsMissing())
            {
                return ResponseViewModelUtils.Failure("未知的持久化实体");
            }
            #region 防止多次点击重复保存以及CSRF攻击
            if (operEnum != OperEnum.del)
            {
                //令牌 form生成时赋值
                string avoid_repeat_token = string.Empty;
                if (formCollection.TryGetValue(FapWebConstants.AVOID_REPEAT_TOKEN, out StringValues vs))
                {
                    avoid_repeat_token = vs;
                }
                else
                {
                    return ResponseViewModelUtils.Failure("不存在防重复提交令牌");
                }
                lock (lockSave)
                {
                    string avoid_repeat_tokenKey = tableName.ToLower() + FapWebConstants.AVOID_REPEAT_TOKEN;
                    if (_applicationContext.Session.GetString(avoid_repeat_tokenKey) != avoid_repeat_token)
                    {
                        return ResponseViewModelUtils.Failure("请勿重复提交数据");
                    }
                    //移除重复提交标记
                    _applicationContext.Session.Remove(avoid_repeat_tokenKey);
                }
                //CSRF 令牌验证
                try
                {
                    await _antiforgery.ValidateRequestAsync(_applicationContext.HttpContext);
                }
                catch (Exception)
                {
                    return ResponseViewModelUtils.Failure("请求非法,校验CSRF异常");
                }
            }
            #endregion

            var (mainData, ChildsData) = BuilderData(tableName,formCollection);
            try
            {
                return SaveChange(operEnum, mainData, ChildsData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ResponseViewModelUtils.Failure("发生错误，操作失败");
            }
            string GetTableName()
            {
                if (formCollection.TryGetValue(FapWebConstants.FORM_TABLENAME, out StringValues tn))
                {
                    return tn;
                }
                if (_applicationContext.Request.Query.TryGetValue(FapWebConstants.QUERY_TABLENAME, out StringValues value))
                {
                    return value;
                }
                return string.Empty;
            }
            OperEnum GetOperEnum()
            {
                OperEnum operEnum;
                if (formCollection.TryGetValue(FapWebConstants.OPERATOR, out StringValues oper))
                {
                    operEnum = (OperEnum)Enum.Parse(typeof(OperEnum), oper);
                }
                else
                {
                    //没有oper的时候 可以根据id的值判断是新增还是编辑
                    string id = formCollection["Id"];
                    if (id.IsMissing() || id.ToInt() < 1)
                    {
                        operEnum = OperEnum.add;
                    }
                    else
                    {
                        operEnum = OperEnum.edit;
                    }
                }

                return operEnum;
            }

        }
        private (dynamic mainData, Dictionary<string, IEnumerable<dynamic>> childsData) BuilderData(string tableName, IFormCollection formCollection)
        {
            var columnList = _dbContext.Columns(tableName);
            //undefined 父文本编辑框控件要去掉,logicdelete逻辑删除,childsData子表格数据
            string[] exclude = new string[]
            {
                    FapWebConstants.IDS,
                    FapWebConstants.OPERATOR,
                    FapWebConstants.QUERY_TABLENAME ,
                    FapWebConstants.FORM_TABLENAME,
                    FapWebConstants.AVOID_REPEAT_TOKEN,
                    FapWebConstants.UNDEFINED,
                    FapWebConstants.LOGICDELETE,
                    FapWebConstants.CHILDS_DATALIST
            };
            dynamic fdo = formCollection.ToDynamicObject(columnList, exclude);
            Dictionary<string, IEnumerable<dynamic>> childDataDic = null;
            if (formCollection.ContainsKey("childsData"))
            {
                childDataDic = new Dictionary<string, IEnumerable<dynamic>>();
                JArray arrayGrids = JArray.Parse(formCollection["childsData"]);
                foreach (JObject item in arrayGrids)
                {
                    string childTableName = item.GetValue(FapWebConstants.QUERY_TABLENAME).ToString();

                    JArray childDataArray = item.GetValue("data") as JArray;
                    if (childDataArray.Any())
                    {
                        List<FapDynamicObject> childDatas = new List<FapDynamicObject>();
                        foreach (JObject cd in childDataArray)
                        {
                            var cfdo = cd.ToFapDynamicObject(_dbContext.Columns(childTableName), exclude);
                            childDatas.Add(cfdo);
                        }
                        childDataDic.Add(childTableName, childDatas);
                    }
                }
            }
            return (fdo, childDataDic);
        }
        [Transactional]
        public ResponseViewModel BatchUpdate(IFormCollection frmCollection)
        {
            ResponseViewModel rvm = new ResponseViewModel();

            frmCollection.TryGetValue("Ids", out StringValues Ids);
            frmCollection.TryGetValue(FapWebConstants.FORM_TABLENAME, out StringValues tableName);
            Guard.Against.NullOrWhiteSpace(Ids, nameof(Ids));
            Guard.Against.NullOrWhiteSpace(tableName, nameof(tableName));
            var (mainData, _) = BuilderData(tableName, frmCollection);
            var ids= Ids.ToString().SplitComma();
            foreach (var id in ids)
            {
                mainData.Id = id;
                _dbContext.UpdateDynamicData(mainData);
            }
            rvm.success = true;
            return rvm;
        }


        [Transactional]
        public ResponseViewModel SaveChange(OperEnum oper, FapDynamicObject mainData, Dictionary<string, IEnumerable<dynamic>> childDataList = null)
        {
            ResponseViewModel rvm = new ResponseViewModel();
            if (oper == OperEnum.add)
            {
                _dbContext.InsertDynamicData(mainData);
                SaveChildData(mainData, childDataList);
                return ResponseViewModelUtils.Sueecss(mainData, "创建成功");
            }
            else if (oper == OperEnum.edit)
            {
                //返回原因
                bool rv = _dbContext.UpdateDynamicData(mainData);
                SaveChildData(mainData, childDataList);
                rvm.success = rv;
                rvm.msg = rv ? "更新成功" : "更新失败，请重试";
                return rvm;
            }
            else if (oper == OperEnum.del)
            {
                bool rv = _dbContext.DeleteDynamicData(mainData);
                DeleteChildData(mainData);
                rvm.success = rv;
                rvm.msg = rv ? "删除成功" : "删除失败，请重试";
                return rvm;
            }
            return ResponseViewModelUtils.Sueecss();
            void SaveChildData(FapDynamicObject mainData, Dictionary<string, IEnumerable<dynamic>> childDataList)
            {
                if (childDataList != null && childDataList.Any())
                {
                    foreach (var item in childDataList)
                    {
                        //获取外键字段
                        var childColumnList = _dbContext.Columns(item.Key);
                        string foreignKey = childColumnList.First(f => f.RefTable == mainData.TableName).ColName;
                        //先删除后增加
                        _dbContext.DeleteExec(item.Key, $"{foreignKey}='{mainData.Get("Fid")}'");
                        foreach (var data in item.Value)
                        {
                            //赋值外键
                            data.Add(foreignKey, mainData.Get("Fid").ToString());
                            _dbContext.InsertDynamicData(data);
                        }
                    }
                }
            }
            void DeleteChildData(FapDynamicObject mainData)
            {
                string tableName = mainData.TableName;
                if (tableName.IsMissing())
                {
                    return;
                }
                //检查子表，删除子表数据
                var childtableList = _dbContext.Tables(t => t.ExtTable == mainData.TableName);

                foreach (var childTable in childtableList)
                {
                    var childColumnList = _dbContext.Columns(childTable.TableName);
                    string foreignKey = childColumnList.First(f => f.RefTable == mainData.TableName).ColName;
                    _dbContext.DeleteExec(childTable.TableName, $"{foreignKey} = @Fid", new DynamicParameters(new { Fid = mainData.Get(FapDbConstants.FAPCOLUMN_FIELD_Fid) }));
                }

            }
        }
        /// <summary>
        /// 导出excel数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns>filename</returns>
        public string ExportExcelData(JqGridPostData model)
        {
            string tableName = model.QuerySet.TableName;
            FapTable ftb = _dbContext.Table(tableName);
            string fileName = $"{ftb.TableComment}_{UUIDUtils.Fid}.xlsx";
            string filePath = Path.Combine(Environment.CurrentDirectory, FapPlatformConstants.TemporaryFolder, fileName);
            Pageable pageable = AnalysisPostData(model);
            pageable.QueryCols = model.QuerySet.ExportCols;
            string sql = pageable.Wraper.Sql();
            if (pageable.Parameters != null && pageable.Parameters.Count > 0)
            {
                foreach (var param in pageable.Parameters)
                {
                    sql = sql.Replace("@" + param.Key, "'" + param.Value + "'");
                }
            }
            ExportModel em = new ExportModel() { DataSql = sql, FileName = filePath, TableName = tableName, ExportCols = pageable.QueryCols };

            bool result = _officeService.ExportExcel(em);
            return result ? fileName : "";
        }

        public string ExportExcelTemplate(QuerySet querySet)
        {
            querySet.InitWhere = "1=2";
            FapTable ftb = _dbContext.Table(querySet.TableName);
            string fileName = $"{ftb.TableComment}_{UUIDUtils.Fid}_模板.xlsx";
            string filePath = Path.Combine(Environment.CurrentDirectory, FapPlatformConstants.TemporaryFolder, fileName);

            ExportModel em = new ExportModel() { DataSql = querySet.ToString(), FileName = filePath, TableName = querySet.TableName, ExportCols = querySet.QueryCols };

            bool result = _officeService.ExportExcel(em);
            return result ? fileName : "";
        }
        public bool ImportExcelData(string tableName)
        {
            try
            {
                var files = _applicationContext.Request.Form.Files;
                List<string> excelFiles = new List<string>();
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        string fileName = UUIDUtils.Fid + files[0].FileName;

                        string fullPath = Path.Combine(Environment.CurrentDirectory, FapPlatformConstants.TemporaryFolder, fileName);

                        using (FileStream fs = System.IO.File.Create(fullPath))
                        {
                            files[0].CopyTo(fs);
                        }
                        excelFiles.Add(fullPath);
                    }
                    foreach (var file in excelFiles)
                    {
                        _officeService.ImportExcel(file, tableName, Core.Office.Excel.Import.ImportMode.INCREMENT_NOLOGIC);

                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

    }

    public enum OperEnum
    {
        add, edit, del, none
    }
}

