using Fap.AspNetCore.Model;
using Fap.AspNetCore.ViewModel;
using Fap.Core.DI;
using System;
using Fap.Core.Extensions;
using System.Collections.Generic;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.DataAccess;
using System.Linq;
using Fap.Core.Rbac;
using Dapper;
using Fap.Core.Infrastructure.Query;
using Fap.AspNetCore.Infrastructure;
using Microsoft.AspNetCore.Antiforgery;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Fap.Core.Utility;
using System.IO;
using Fap.Core.Office.Excel.Export;
using Fap.Core.Office;
using Fap.Core.Annex.Utility.Zip;
using Fap.AspNetCore.Binder;
using Microsoft.Extensions.Caching.Memory;
using Fap.Core.Rbac.Model;
using System.Text.RegularExpressions;
using Fap.Core.MultiLanguage;
using Fap.Core.Infrastructure.Model;
using MailKit;
using Ardalis.GuardClauses;

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
        private readonly IMemoryCache _memoryCache;
        private readonly IMultiLangService _multiLangService;
        public GridFormService(
            IFapApplicationContext fapApplicationContext,
            ILogger<GridFormService> logger,
            IOfficeService officeService,
            IDbContext dbContext,
            IRbacService rbacService,
            IAntiforgery antiforgery,
            IMemoryCache memoryCache, IMultiLangService multiLangService)
        {
            _dbContext = dbContext;
            _applicationContext = fapApplicationContext;
            _rbacService = rbacService;
            _antiforgery = antiforgery;
            _officeService = officeService;
            _logger = logger;
            _memoryCache = memoryCache;
            _multiLangService = multiLangService;
        }

        public PageDataResultView QueryPageDataResultView(JqGridPostData jqGridPostData)
        {
            Pageable pageable = AnalysisPostData(jqGridPostData);
            //queryOption.Where = AnalysisWhere(queryOption.Where);
            PageDataResultView result = QueryPagedDynamicData();
            return result;

            PageDataResultView QueryPagedDynamicData()
            {
                try
                {
                    IEnumerable<FapColumn> fapColumns = _dbContext.Columns(jqGridPostData.QuerySet.TableName);
                    PageInfo<dynamic> pi = _dbContext.QueryPage(pageable);

                    //组装成DataResultView对象
                    PageDataResultView dataResultView = new PageDataResultView();
                    dataResultView.Data = pi.Items.ToFapDynamicObjectList(fapColumns);

                    dataResultView.DataJson = JsonConvert.SerializeObject(pi.Items);
                    dataResultView.TotalCount = pi.TotalCount;
                    dataResultView.CurrentPage = pi.CurrentPage;
                    dataResultView.OrginData = pi.Items;
                    dataResultView.DataListForJqGrid = pi.Items;//暂不启用加密设置 GetEncryptData(jqGridPostData.QuerySet.TableName, pi.Items as IEnumerable<IDictionary<string, object>>);
                    dataResultView.PageSize = pi.PageSize;
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
        private IEnumerable<IDictionary<string, object>> GetEncryptData(string tn, IEnumerable<IDictionary<string, object>> dataList)
        {
            string encrypt_key = $"encrypt_{tn}";
            var encryptCache = _memoryCache.GetOrCreate(encrypt_key, (ce) =>
            {
                ce.SetSlidingExpiration(TimeSpan.FromMinutes(60));
                DynamicParameters param = new DynamicParameters();
                param.Add("TableName", tn);
                var elist = _dbContext.QueryWhere<FapDataEncrypt>($"{nameof(FapDataEncrypt.RefTable)}=@TableName", param);
                return elist;
            });
            if (encryptCache.Any())
            {
                IDictionary<string, IEnumerable<string>> keyValues = new Dictionary<string, IEnumerable<string>>();
                foreach (var et in encryptCache)
                {
                    string dataKey = $"encrypt_{et.Fid}";
                    var fids = _memoryCache.GetOrCreate(dataKey, (ce) =>
                    {
                        ce.SetSlidingExpiration(TimeSpan.FromMinutes(60));
                        string sql = $"select Fid from {et.RefTable} where {et.Condition}";
                        return _dbContext.Query<string>(sql);

                    });
                    if (fids.Any())
                    {
                        var tmpList = dataList.Where(d => fids.Contains(d["Fid"]));
                        foreach (var di in tmpList)
                        {
                            if (di.ContainsKey(et.ColumnName))
                            {
                                di[et.ColumnName] = et.ReplaceChart;
                            }
                        }
                    }

                }
            }
            return dataList;

        }
        public Pageable AnalysisPostData(JqGridPostData jqGridPostData)
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
                pageable.AddWhere(qs.GlobalWhere);
            }
            if (jqGridPostData.Filters.IsMissing() && qs.InitWhere.IsPresent())
            {
                pageable.AddWhere(qs.InitWhere);
            }

            //页面级条件
            JsonFilterToSql jfs = new JsonFilterToSql(_dbContext);
            if (jqGridPostData.PageCondition.IsPresent())
            {
                pageable.AddWhere(jfs.BuilderFilter(pageable.TableName, jqGridPostData.PageCondition), QuerySymbolEnum.AND);

            }
            //构造jqgrid过滤条件
            if (jqGridPostData.Filters.IsPresent())
            {
                pageable.AddWhere(jfs.BuilderFilter(pageable.TableName, jqGridPostData.Filters), QuerySymbolEnum.AND);
                //string filterWhere = JsonFilterToSql.BuildFilterCondition(fapColumns, jqGridPostData.Filters);

            }
            //事件处理
            //actionSimplepageable?.Invoke(pageable);
            pageable.CurrentPage = jqGridPostData.Page;
            pageable.PageSize = jqGridPostData.Rows;
            //数据权限
            string dataWhere = DataWhere();
            if (dataWhere.IsPresent())
            {
                pageable.AddWhere(dataWhere);
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
                if (where.IndexOf(FapPlatformConstants.DepartmentAuthority) > -1)
                {
                    where = where.Replace(FapPlatformConstants.DepartmentAuthority, DeptWhere());
                }
                return where.ReplaceIgnoreCase("query", "select ");
            }
            string DeptWhere()
            {
                var roledepts = _rbacService.GetRoleDeptList(_applicationContext.CurrentRoleUid);
                if (roledepts.Any())
                {
                    return string.Join(",", roledepts.Select(d => "'" + d.DeptUid + "'"));
                }
                else
                {
                    return "'meiyou'";
                }
            }
            //数据权限
            string DataWhere()
            {
                string where = string.Empty;
                var roleDatas = _rbacService.GetRoleDataList(_applicationContext.CurrentRoleUid);
                if (roleDatas != null && roleDatas.Any())
                {
                    var rd = roleDatas.FirstOrDefault<FapRoleData>(r => r.TableUid == qs.TableName);
                    if (rd != null)
                    {
                        where = rd.SqlCondition;
                        string pattern = FapPlatformConstants.VariablePattern;
                        Regex reg = new Regex(pattern);
                        MatchCollection matchs = reg.Matches(where);
                        foreach (var mtch in matchs)
                        {

                            int length = mtch.ToString().Length - 3;
                            string colName = mtch.ToString().Substring(2, length);
                            if (colName.EqualsWithIgnoreCase("DeptUid"))
                            {
                                where = where.Replace(mtch.ToString(), _applicationContext.DeptUid);
                            }
                            else if (colName.EqualsWithIgnoreCase("CurrentRoleUid"))
                            {
                                where = where.Replace(mtch.ToString(), _applicationContext.CurrentRoleUid);
                            }
                            else if (colName.EqualsWithIgnoreCase("EmpUid"))
                            {
                                where = where.Replace(mtch.ToString(), _applicationContext.EmpUid);
                            }
                            else if (colName.EqualsWithIgnoreCase("DeptCode"))
                            {
                                string deptCode = _applicationContext.DeptCode;
                                if (deptCode.IsMissing())
                                {
                                    OrgDept dept = _dbContext.Get<OrgDept>(_applicationContext.DeptUid);
                                    deptCode = dept.DeptCode;
                                }
                                where = where.Replace(mtch.ToString(), deptCode);
                            }
                        }
                    }
                }
                return where;
            }
        }
        #endregion
        [Transactional]
        public async Task<ResponseViewModel> PersistenceAsync(FormModel frmModel)
        {
            //操作符 
            if (frmModel.Oper == FormOperEnum.none)
            {
                return ResponseViewModelUtils.Failure("未知的持久化操作符");
            }
            if (frmModel.TableName.IsMissing())
            {
                return ResponseViewModelUtils.Failure("未知的持久化实体");
            }
            #region 防止多次点击重复保存以及CSRF攻击
            if (frmModel.Oper != FormOperEnum.del)
            {
                if (frmModel.AvoidDuplicateKey.IsMissing())
                {
                    return ResponseViewModelUtils.Failure("不存在防重复提交令牌");
                }

                //对比缓存中是否存在次key，不存在加入
                string avoid_repeat_tokenKey = $"{frmModel.TableName}_{_applicationContext.UserUid}_{FapWebConstants.AVOID_REPEAT_TOKEN}";
                if (_memoryCache.TryGetValue(avoid_repeat_tokenKey, out string cv))
                {
                    if (cv == frmModel.AvoidDuplicateKey)
                    {
                        return ResponseViewModelUtils.Failure("请勿频繁提交数据");
                    }
                    else
                    {
                        //_memoryCache.Remove(avoid_repeat_tokenKey);
                        _memoryCache.Set(avoid_repeat_tokenKey, frmModel.AvoidDuplicateKey, DateTimeOffset.Now.AddSeconds(30));
                    }
                }
                else
                {
                    _memoryCache.Set(avoid_repeat_tokenKey, frmModel.AvoidDuplicateKey, DateTimeOffset.Now.AddSeconds(30));
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
            try
            {
                return SaveChange(frmModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ResponseViewModelUtils.Failure("操作失败:" + ex.Message);
            }

        }

        [Transactional]
        public ResponseViewModel Persistence(FormModel frmModel)
        {
            //操作符 
            if (frmModel.Oper == FormOperEnum.none)
            {
                return ResponseViewModelUtils.Failure("未知的持久化操作符");
            }
            if (frmModel.TableName.IsMissing())
            {
                return ResponseViewModelUtils.Failure("未知的持久化实体");
            }
            #region 防止多次点击重复保存以及CSRF攻击
            if (frmModel.Oper != FormOperEnum.del)
            {
                if (frmModel.AvoidDuplicateKey.IsMissing())
                {
                    return ResponseViewModelUtils.Failure("不存在防重复提交令牌");
                }

                //对比缓存中是否存在次key，不存在加入
                string avoid_repeat_tokenKey = $"{frmModel.TableName}_{_applicationContext.UserUid}_{FapWebConstants.AVOID_REPEAT_TOKEN}";
                if (_memoryCache.TryGetValue(avoid_repeat_tokenKey, out string cv))
                {
                    if (cv == frmModel.AvoidDuplicateKey)
                    {
                        return ResponseViewModelUtils.Failure("请勿重复提交数据");
                    }
                    else
                    {
                        //_memoryCache.Remove(avoid_repeat_tokenKey);
                        _memoryCache.Set(avoid_repeat_tokenKey, frmModel.AvoidDuplicateKey, DateTimeOffset.Now.AddMinutes(30));
                    }
                }
                else
                {
                    _memoryCache.Set(avoid_repeat_tokenKey, frmModel.AvoidDuplicateKey, DateTimeOffset.Now.AddMinutes(30));
                }
            }

            #endregion
            try
            {
                return SaveChange(frmModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ResponseViewModelUtils.Failure("操作失败:" + ex.Message);
            }

        }

        private ResponseViewModel SaveChange(FormModel formModel)
        {
            ResponseViewModel rvm = new ResponseViewModel();
            var mainData = formModel.MainData;
            var childDataList = formModel.ChildDataList;
            if (formModel.Oper == FormOperEnum.add)
            {
                _dbContext.InsertDynamicData(mainData);
                SaveChildData(mainData, childDataList);
                return ResponseViewModelUtils.Sueecss(mainData, _multiLangService.GetOrAndMultiLangValue(MultiLanguageOriginEnum.Page, "insert_success", "创建成功"));
            }
            else if (formModel.Oper == FormOperEnum.edit)
            {
                //返回原因
                bool rv = _dbContext.UpdateDynamicData(mainData);
                SaveChildData(mainData, childDataList);
                rvm.data = mainData;
                rvm.success = rv;
                rvm.msg = rv ? _multiLangService.GetOrAndMultiLangValue(MultiLanguageOriginEnum.Page, "update_success", "更新成功") : _multiLangService.GetOrAndMultiLangValue(MultiLanguageOriginEnum.Page, "update_failure", "更新失败，请重试");
                return rvm;
            }
            else if (formModel.Oper == FormOperEnum.del)
            {
                bool rv = false;
                if (formModel.LogicDelete)
                {
                    //删除可能存在批量
                    rv = _dbContext.DeleteDynamicData(mainData);
                }
                else
                {
                    rv = _dbContext.DeleteNoLogicDynamicData(mainData);
                }
                DeleteChildData(mainData);
                rvm.success = rv;
                rvm.msg = rv ? _multiLangService.GetOrAndMultiLangValue(MultiLanguageOriginEnum.Page, "delete_success", "删除成功") : _multiLangService.GetOrAndMultiLangValue(MultiLanguageOriginEnum.Page, "delete_failure", "删除失败，请重试");
                return rvm;
            }
            else if (formModel.Oper == FormOperEnum.batch_edit)
            {
                var ids = formModel.Ids.SplitComma();
                foreach (var id in ids)
                {
                    mainData.SetValue("Id", id);
                    _dbContext.UpdateDynamicData(mainData);
                }
                rvm.success = true;
                rvm.msg = _multiLangService.GetOrAndMultiLangValue(MultiLanguageOriginEnum.Page, "batch_update_success", "批量更新成功！");
                return rvm;
            }
            return ResponseViewModelUtils.Sueecss();
            void SaveChildData(FapDynamicObject mainData, IDictionary<string, IEnumerable<FapDynamicObject>> childDataList)
            {
                if (childDataList.Any())
                {
                    foreach (var item in childDataList)
                    {
                        //获取外键字段
                        var childColumnList = _dbContext.Columns(item.Key);
                        string foreignKey = childColumnList.First(f => f.RefTable == mainData.TableName).ColName;
                        IList<long> ids = new List<long>();
                        foreach (var data in item.Value)
                        {
                            if (data.Get("Id").ToString().IsMissing())
                            {
                                //赋值外键
                                data.SetValue(foreignKey, mainData.Get("Fid").ToString());
                                long id = _dbContext.InsertDynamicData(data);
                                ids.Add(id);
                            }
                            else
                            {
                                ids.Add(data.Get("Id").ToLong());
                                _dbContext.UpdateDynamicData(data);
                            }
                        }
                        if (ids.Count > 0)
                        {
                            //先删除后增加
                            _dbContext.DeleteExec(item.Key, $"{foreignKey}='{mainData.Get("Fid")}' and Id not in @Ids", new DynamicParameters(new { Ids = ids }));
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
                var childtableList = _dbContext.Tables(t => t.MainTable == mainData.TableName);

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
            string sql = pageable.Wraper.MakeExportSql();
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
        public bool ImportWordTemplate(string tableName)
        {
            try
            {
                var files = _applicationContext.Request.Form.Files;
                List<string> excelFiles = new List<string>();
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        string fullPath = Path.Combine(Environment.CurrentDirectory, FapPlatformConstants.Template, $"{tableName.ToLower()}.docx");

                        using (FileStream fs = System.IO.File.Create(fullPath))
                        {
                            files[0].CopyTo(fs);
                        }
                        excelFiles.Add(fullPath);
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
        public bool ImportExcelReportTemplate(string fid)
        {
            try
            {
                var files = _applicationContext.Request.Form.Files;
                // List<string> excelFiles = new List<string>();
                if (files != null && files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        string fullPath = Path.Combine(Environment.CurrentDirectory, FapPlatformConstants.Template, $"{fid}.xlsx");

                        using (FileStream fs = System.IO.File.Create(fullPath))
                        {
                            files[0].CopyTo(fs);
                        }
                        // excelFiles.Add(fullPath);
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
        public string PrintWordTemplate(GridModel gridModel)
        {
            string tableName = gridModel.TableName;
            var columns = _dbContext.Columns(tableName);
            IList<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            foreach (var row in gridModel.Rows)
            {
                var keys = row.Keys;
                Dictionary<string, string> dic = new Dictionary<string, string>()
                {
                    {"当前日期",DateTimeUtils.CurrentDateStr},
                    { "登录人",_applicationContext.EmpName}
                };
                foreach (var key in keys)
                {
                    var column = columns.FirstOrDefault(c => c.ColName == key);
                    if (column != null)
                    {
                        if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                        {
                            dic.Add(column.ColComment, row.Get(key + "MC").ToString());
                        }
                        else if (column.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX)
                        {
                            dic.Add(column.ColComment, _dbContext.Dictionary(column.ComboxSource, row.Get(key).ToString())?.Name);
                        }
                        else
                        {
                            dic.Add(column.ColComment, row.Get(key).ToString());
                        }
                    }
                }
                list.Add(dic);
            }

            string templateFile = Path.Combine(Environment.CurrentDirectory, FapPlatformConstants.Template, tableName.ToLower() + ".docx");
            IList<string> oriFile = new List<string>();
            foreach (var dic in list)
            {
                string fileName = $"{_dbContext.Table(tableName).TableComment}_{UUIDUtils.Fid}.docx";
                string outputFile = Path.Combine(Environment.CurrentDirectory, FapPlatformConstants.TemporaryFolder, fileName);
                _officeService.PrintWordTemplate(templateFile, outputFile, dic);
                oriFile.Add(outputFile);
            }
            if (oriFile.Count == 1)
            {
                return Path.GetFileName(oriFile.First());
            }
            string zipFileName = UUIDUtils.Fid;
            //将这些文件打包成ZIP文件，返回zip流
            ZipHelper zipHelper = new ZipHelper();
            zipHelper.ZipMultiFiles(oriFile, Path.Combine(Environment.CurrentDirectory, FapPlatformConstants.TemporaryFolder, zipFileName));
            return zipFileName;
        }
        public ChartResult EChart(ChartViewModel chartViewModel, JqGridPostData jqGridPostData)
        {
            Guard.Against.NullOrEmpty(chartViewModel.Groups, nameof(chartViewModel.Groups));
            string tableName = jqGridPostData.QuerySet.TableName;
            //页面级条件
            JsonFilterToSql jfs = new JsonFilterToSql(_dbContext);
            List<string> lwhere = new List<string>();
            if (jqGridPostData.PageCondition.IsPresent())
            {
                lwhere.Add(jfs.BuilderFilter(tableName, jqGridPostData.PageCondition));
            }
            //构造jqgrid过滤条件
            if (jqGridPostData.Filters.IsPresent())
            {
                var filters = jfs.BuilderFilter(tableName, jqGridPostData.Filters);
                if (filters.IsPresent())
                {
                    lwhere.Add(filters);
                }
            }
            if (jqGridPostData.QuerySet.GlobalWhere.IsPresent())
            {
                lwhere.Add(jqGridPostData.QuerySet.GlobalWhere);
            }
            string where = $" where {tableName}.EnableDate<='{DateTimeUtils.CurrentDateTimeStr}' and {tableName}.DisableDate>='{DateTimeUtils.CurrentDateTimeStr}' and {tableName}.Dr=0 ";
            if (lwhere.Count > 0)
            {
                where += " and " + string.Join(" and ", lwhere);
            }

            List<string> groupByList = new List<string>();
            List<string> colNameList = new List<string>();
            BuildGroupBy(chartViewModel.Groups, groupByList, colNameList);
            string colName = string.Join(',', colNameList);
            string groupBy = $" group by {string.Join(',', groupByList)}";
            //统计列
            List<string> aggCols = new List<string>();
            //存放ccount得sql
            Dictionary<string, string> ccSqlDics = new Dictionary<string, string>();
            //统计项
            List<Aggregate> agglist = new List<Aggregate>();
            if (chartViewModel.Aggregates != null)
            {
                foreach (var aggregate in chartViewModel.Aggregates)
                {
                    if (aggregate.AggType == AggregateEnum.CCOUNT)
                    {
                        //特殊处理CCount
                        ccSqlDics.Add(aggregate.Field, $"select {colName},{aggregate.Field},COUNT({aggregate.Field}) as C from {tableName} {where} {groupBy},{aggregate.Field}");
                    }
                    else
                    {
                        aggCols.Add($"{aggregate.AggType}({aggregate.Field}) as '{aggregate.Alias}'");
                    }
                    agglist.Add(aggregate);
                }
            }
            string sql = $"select {colName} from {tableName} {where} {groupBy}";
            if (aggCols.Count > 0)
            {
                sql = $"select {colName},{string.Join(',', aggCols)} from {tableName} {where} {groupBy}";
            }
            DynamicParameters param = new DynamicParameters();
            foreach (var p in jqGridPostData.QuerySet.Parameters)
            {
                param.Add(p.ParamKey, p.ParamValue);
            }
            sql = sql.ReplaceIgnoreCase("query", "select ");
            var dataList = _dbContext.QueryOriSql(sql, param);
            DataProcessed(ccSqlDics, tableName, chartViewModel.Groups, dataList, agglist);
            return new ChartResult { Aggregates = agglist, DataSet = dataList }; ;
        }

        private void BuildGroupBy(IEnumerable<GroupBy> groupBys, List<string> groupByList, List<string> colNameList)
        {
            foreach (var gf in groupBys)
            {
                if (gf.Format.IsPresent())
                {
                    if (_dbContext.DatabaseDialect == Core.DataAccess.DatabaseDialectEnum.MSSQL)
                    {
                        if (gf.Format.EqualsWithIgnoreCase("yyyy"))
                        {
                            groupByList.Add($"CONVERT(varchar(4) ,{gf.Field}, 120)");
                            colNameList.Add($"CONVERT(varchar(4) ,{gf.Field}, 120) as '{gf.Alias}'");
                        }
                        else if (gf.Format.EqualsWithIgnoreCase("yyyymm"))
                        {
                            groupByList.Add($"CONVERT(varchar(7) ,{gf.Field}, 120)");
                            colNameList.Add($"CONVERT(varchar(7) ,{gf.Field}, 120) as '{gf.Alias}'");
                        }
                        else if (gf.Format.EqualsWithIgnoreCase("yyyymmdd"))
                        {
                            groupByList.Add($"CONVERT(varchar(10) ,{gf.Field}, 120)");
                            colNameList.Add($"CONVERT(varchar(10) ,{gf.Field}, 120) as '{gf.Alias}'");
                        }
                        else
                        {
                            groupByList.Add($"{gf.Field}");
                            colNameList.Add($"{gf.Field} as '{gf.Alias}'");
                        }
                    }
                    else if (_dbContext.DatabaseDialect == Core.DataAccess.DatabaseDialectEnum.MYSQL)
                    {
                        if (gf.Format.EqualsWithIgnoreCase("yyyy"))
                        {
                            groupByList.Add($"DATE_FORMAT({gf.Field},'%Y')");
                            colNameList.Add($"DATE_FORMAT({gf.Field},'%Y') as '{gf.Alias}'");
                        }
                        else if (gf.Format.EqualsWithIgnoreCase("yyyymm"))
                        {
                            groupByList.Add($"DATE_FORMAT({gf.Field},'%Y-%m')");
                            colNameList.Add($"DATE_FORMAT({gf.Field},'%Y-%m')  as '{gf.Alias}'");
                        }
                        else if (gf.Format.EqualsWithIgnoreCase("yyyymmdd"))
                        {
                            groupByList.Add($"DATE_FORMAT({gf.Field},'%Y-%m-%d')");
                            colNameList.Add($"DATE_FORMAT({gf.Field},'%Y-%m-%d')  as '{gf.Alias}'");
                        }
                    }
                    else
                    {
                        groupByList.Add($"{gf.Field}");
                        colNameList.Add($"{gf.Field} as '{gf.Alias}'");
                    }
                }
                else
                {
                    groupByList.Add($"{gf.Field}");
                    colNameList.Add($"{gf.Field} as '{gf.Alias}'");
                }
            }
        }

        private void DataProcessed(Dictionary<string, string> ccSqlDics, string tableName, IEnumerable<GroupBy> groupBys, IEnumerable<dynamic> dataList, List<Aggregate> agglist)
        {
            var groupBy = groupBys.First();
            var dataResult = dataList as IEnumerable<IDictionary<string, object>>;
            foreach (var ccSqlDic in ccSqlDics)
            {
                string colName = ccSqlDic.Key;
                var dcolumn = _dbContext.Column(tableName, colName);
                var dics = _dbContext.Dictionarys(dcolumn.ComboxSource);
                var ca = agglist.First(a => a.Field == colName);
                foreach (var dic in dics)
                {
                    agglist.Add(new Aggregate { Field = dic.Code, AggType = ca.AggType, Alias = dic.Name, ChartType = ca.ChartType });
                }
                agglist.Remove(ca);
                var ccDataList = _dbContext.QueryOriSql(ccSqlDic.Value);
                dataResult.ToList().ForEach((di) =>
                {
                    foreach (var dic in dics)
                    {
                        var data = ccDataList.FirstOrDefault(d => ((d as IDictionary<string, object>)[groupBy.Alias]?.ToString()??"").EqualsWithIgnoreCase((di[groupBy.Alias]?.ToString()??""))
                         && ((d as IDictionary<string, object>)[colName]?.ToString()??"").EqualsWithIgnoreCase(dic.Code));
                        di[dic.Name] = data?.C ?? 0;
                    }
                });
            }
            if (groupBys.Count() > 1)
            {
                foreach (var gb in groupBys)
                {
                    if (gb.Field == groupBy.Field)
                    {
                        continue;
                    }
                    agglist.Add(new Aggregate { Field = gb.Field, AggType = AggregateEnum.COUNT, Alias = gb.Alias, ChartType ="bar" });

                }
            }
            var column = _dbContext.Column(tableName, groupBy.Field);
            if (column.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX && column.ComboxSource.IsPresent())
            {
                var dics = _dbContext.Dictionarys(column.ComboxSource);
                dataResult.ToList().ForEach((di) =>
                {
                    di[groupBy.Alias] = dics.FirstOrDefault(d => d.Code == di[groupBy.Alias]?.ToString())?.Name ?? "未知";
                });
            }
            else if (column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
            {
                string refSql = $"select {column.RefID} Code, {column.RefName} Name from {column.RefTable} where {column.RefID} in @Ids";
                var Ids = dataList.Select(d => (d as IDictionary<string, object>)[groupBy.Alias]);
                var refs = _dbContext.Query(refSql, new DynamicParameters(new {Ids }));
                dataResult.ToList().ForEach((di) =>
                {
                    di[groupBy.Alias] = refs.FirstOrDefault(d => d.Code == di[groupBy.Alias]?.ToString())?.Name ?? "未知";
                });
            }
        }
        [Transactional]
        public void DeleteFormulaCase(string caseUid)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("FcUid", caseUid);
            _dbContext.Execute("delete from FapFormula where FcUid=@FcUid", param);
            _dbContext.Execute("delete from FapFormulaCase where Fid=@FcUid", param);
        }
        /// <summary>
        /// 保存公式项
        /// </summary>
        /// <param name="formula"></param>
        [Transactional]
        public void SaveFormulaItem(FapFormulaItems formula)
        {
            string tableName = formula.TableName;
            IEnumerable<FapFormula> list = formula.Formulas;
            //删除完毕重新加
            _dbContext.Execute("delete from FapFormula where FcUid=@FcUid", new DynamicParameters(new { FcUid = formula.FcUid }));
            if (list != null && list.Any())
            {
                var cols = _dbContext.Columns(formula.TableName);
                var entityMappingList = _dbContext.QueryWhere<CfgEntityMapping>($"{nameof(CfgEntityMapping.OriEntity)}=@OriEntity", new DynamicParameters(new { OriEntity = formula.MappingTable }), true);
                list.ToList().ForEach((f) =>
                {
                    f.FcUid = formula.FcUid;
                    if (f.FmuDesc.IsPresent())
                    {
                        if (f.FmuDesc.StartsWith("[引用]", StringComparison.OrdinalIgnoreCase))
                        {
                            f.FmuContent = SqlUtils.ParsingFormulaMappingSql(entityMappingList, tableName, f.ColName, f.FmuDesc, _dbContext);
                        }
                        else if (f.FmuDesc.StartsWith("[累计]", StringComparison.OrdinalIgnoreCase))
                        {
                            f.FmuContent = SqlUtils.ParsingFormulaGrandTotalSql(cols, f.ColName, formula.MappingTable, f.FmuDesc, _dbContext.DatabaseDialect);
                        }
                        else
                        {
                            f.FmuContent = SqlUtils.ParsingFormulaVariableSql(cols, f.ColName, f.FmuDesc, _dbContext.DatabaseDialect);
                        }
                    }
                });
                _dbContext.InsertBatch(list);
            }
        }
        /// <summary>
        /// 公式计算
        /// </summary>
        /// <param name="formulaCaseUid">公式套</param>
        /// <returns>异常信息</returns>
        public IList<string> FormulaCalculate(string formulaCaseUid)
        {
            var formulas = _dbContext.QueryWhere<FapFormula>("FcUid=@FcUid and FmuDesc!='' and Enabled=1", new DynamicParameters(new { FcUid = formulaCaseUid }));
            //先计算引用
            var associateList = formulas.Where(f => f.FmuDesc.StartsWith("[引用]", StringComparison.OrdinalIgnoreCase) && f.FmuContent.IsPresent()).OrderBy(f => f.OrderBy);
            //非引用非累计
            var formulaList = formulas.Where(f => !f.FmuDesc.StartsWith("[引用]", StringComparison.OrdinalIgnoreCase)
            && !f.FmuDesc.StartsWith("[累计]", StringComparison.OrdinalIgnoreCase) && f.FmuContent.IsPresent()).OrderBy(f => f.OrderBy);
            //累计
            var grandTotalList = formulas.Where(f => f.FmuDesc.StartsWith("[累计]", StringComparison.OrdinalIgnoreCase) && f.FmuContent.IsPresent()).OrderBy(f => f.OrderBy);

            List<string> exceptionList = new List<string>();
            ExecFormula(associateList);
            ExecFormula(formulaList);
            ExecFormula(grandTotalList);
            string tableName = formulas.FirstOrDefault()?.TableName;
            if (tableName.IsPresent())
            {
                _dbContext.ExecuteOriginal($"update {tableName} set JoinCalculate=1");
            }
            return exceptionList;
            void ExecFormula(IEnumerable<FapFormula> fapFormulas)
            {
                foreach (var ff in fapFormulas)
                {
                    try
                    {
                        _dbContext.ExecuteOriginal(ff.FmuContent);
                    }
                    catch (Exception ex)
                    {
                        exceptionList.Add($"{ff.ColComment}:{ex.Message}");
                    }
                }
            }
        }
    }

    public enum FormOperEnum
    {
        add, edit, batch_edit, del, none
    }
}

