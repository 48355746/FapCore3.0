using Ardalis.GuardClauses;
using Dapper;
using Fap.Core.DataAccess.Interceptor;
using Fap.Core.DataAccess.SqlParser;
using Fap.Core.Exceptions;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Config;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Infrastructure.Model;
using Fap.Core.Infrastructure.Query;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Fap.Core.MultiLanguage;

namespace Fap.Core.DataAccess
{
    public class DbContext : IDbContext
    {
        private readonly IFapApplicationContext _applicationContext;
        private readonly ILogger<DbContext> _logger;
        private readonly IDbSession _dbSession;
        private readonly IFapPlatformDomain _fapPlatformDomain;
        private readonly IServiceProvider _serviceProvider;
        private ThreadLocal<string> threadLocalHistoryTimePoint = new ThreadLocal<string>();
        public DbContext(ILoggerFactory loggerFactory, IFapPlatformDomain fapPlatformDomain, IFapApplicationContext applicationContext, IDbSession dbSession, IServiceProvider serviceProvider)
        {
            _logger = loggerFactory.CreateLogger<DbContext>();
            _dbSession = dbSession;
            _fapPlatformDomain = fapPlatformDomain;
            _applicationContext = applicationContext;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 历史时间点
        /// </summary>
        public string HistoryDateTime
        {
            set { threadLocalHistoryTimePoint.Value = value; }
            get { return threadLocalHistoryTimePoint.Value; }
        }
        public DatabaseDialectEnum DatabaseDialect => _dbSession.DatabaseDialect;

        #region private
        private static ConcurrentDictionary<string, PropertyInfo> properties = new ConcurrentDictionary<string, PropertyInfo>();
        private (string, DynamicParameters) WrapSqlAndParam(string sqlOri, DynamicParameters parameters, bool withMC = false, bool withId = false)
        {
            return (ParseFapSql(sqlOri, withMC, withId), InitParamers(parameters));

            string ParseFapSql(string sqlOri, bool withMC = false, bool withId = false)
            {
                _logger.LogTrace($"wrap前的sql:{sqlOri}");
                FapSqlParser parse = new FapSqlParser(_fapPlatformDomain, sqlOri, withMC, withId);
                parse.CurrentLang = _applicationContext.Language.ToString();
                var sql = parse.ParserSqlStatement();
                _logger.LogTrace($"wrap后的sql:{sql}");
                return sql;
            }
        }
        private DynamicParameters InitParamers(DynamicParameters parameters = null)
        {
            if (parameters == null)
            {
                parameters = new DynamicParameters();
            }
            if (!parameters.ParameterNames.Contains(FapDbConstants.FAPCOLUMN_FIELD_CurrentDate))
            {
                if (HistoryDateTime.IsMissing())
                {
                    parameters.Add(FapDbConstants.FAPCOLUMN_FIELD_CurrentDate, DateTimeUtils.CurrentDateTimeStr);
                }
                else
                {
                    parameters.Add(FapDbConstants.FAPCOLUMN_FIELD_CurrentDate, HistoryDateTime);
                }
            }
            if (!parameters.ParameterNames.Contains(FapDbConstants.FAPCOLUMN_FIELD_Dr))
            {
                parameters.Add(FapDbConstants.FAPCOLUMN_FIELD_Dr, 0);
            }
            return parameters;
        }
        private void InitEntityToInsert<T>(T entity) where T : BaseModel
        {
            if (entity.Fid.IsMissing())
            {
                entity.Fid = UUIDUtils.Fid;
            }
            string tableName = typeof(T).Name;
            entity.CreateDate = DateTimeUtils.CurrentDateTimeStr;
            entity.EnableDate = DateTimeUtils.LastSecondDateTimeStr;
            entity.DisableDate = DateTimeUtils.PermanentTimeStr;
            entity.Ts = DateTimeUtils.Ts;
            entity.Dr = 0;
            entity.CreateBy = _applicationContext.EmpUid;
            entity.CreateName = _applicationContext.EmpName;
            entity.OrgUid = _applicationContext.OrgUid;
            entity.GroupUid = _applicationContext.GroupUid;
            //非系统默认列的默认值的生成
            IEnumerable<FapColumn> columns = Columns(tableName).Where(c => c.IsDefaultCol != 1 && (c.DefaultValueClass.IsPresent() || c.ColDefault.IsPresent()));
            foreach (var column in columns)
            {
                Type type = entity.GetType();
                if (!properties.TryGetValue(column.TableName + column.ColName, out PropertyInfo propertyInfo))
                {
                    properties.TryAdd(column.TableName + column.ColName, (propertyInfo = type.GetProperty(column.ColName)));
                }
                if (propertyInfo == null)
                {
                    continue;
                }
                //目前先支持string            
                object value = propertyInfo.GetValue(entity); //获取属性值

                //先判断是否有值，如果有值，则不赋值
                if (value != null && value.ToString().Trim().IsPresent())
                {
                    continue;
                }
                if (column.ColDefault.IsPresent())
                {
                    object cv = GetFieldDefaultValue(column);
                    propertyInfo.SetValue(entity, cv, null); //给对应属性赋值
                }
            }
            //是否有配置的编码
            if (tableName.ToLower().StartsWith("cfg") || tableName.ToLower().StartsWith("fap"))
            {
                return;
            }
            Dictionary<string, string> ccr = GetBillCode(tableName);
            if (ccr != null && ccr.Any())
            {
                foreach (var cc in ccr)
                {
                    Type type = entity.GetType();
                    System.Reflection.PropertyInfo propertyInfo = type.GetProperty(cc.Key); //获取指定名称的属性
                    if (propertyInfo == null)
                    {
                        continue;
                    }
                    //目前先支持string            
                    object value = propertyInfo.GetValue(entity, null); //获取属性值

                    //先判断是否有值，如果有值，则不赋值
                    if (value != null && !string.IsNullOrWhiteSpace(value.ToString().Trim()))
                    {
                        continue;
                    }
                    propertyInfo.SetValue(entity, cc.Value, null); //给对应属性赋值                    
                }
            }
        }
        private object GetFieldDefaultValue(FapColumn column)
        {
            string key = column.ColName;
            //判断表单中是否存在此字段，存在就跳过
            if (column.ColDefault.StartsWith("sql:"))
            {

            }
            else if (column.ColDefault.EqualsWithIgnoreCase(FapDbConstants.CurrentDate))
            {
                //当前时间
                return DateTimeUtils.CurrentDateTimeStr;
            }
            else if (column.ColDefault.EqualsWithIgnoreCase(FapDbConstants.CurrentEmployee))
            {
                if (!column.ColName.EndsWith("MC"))
                {
                    return _applicationContext.EmpUid;
                }
            }
            else if (column.ColDefault.EqualsWithIgnoreCase(FapDbConstants.CurrentDept))
            {
                if (!column.ColName.EndsWith("MC"))
                {
                    return _applicationContext.DeptUid;
                }
            }
            else if (column.ColDefault.EqualsWithIgnoreCase(FapDbConstants.CurrentDeptCode))
            {
                return _applicationContext.DeptCode;
            }
            else if (column.ColDefault.EqualsWithIgnoreCase(FapDbConstants.CurrentUser))
            {
                if (!column.ColName.EndsWith("MC"))
                {
                    return _applicationContext.UserUid;
                }
            }
            else if (column.ColDefault.EqualsWithIgnoreCase(FapDbConstants.UUID))
            {
                if (!column.ColName.EndsWith("MC"))
                {
                    return UUIDUtils.Fid;
                }
            }
            else
            {
                return column.ColDefault;
            }
            return "";
        }
        /// <summary>
        /// 初始化默认值
        /// </summary>
        /// <param name="dynEntity">FapDynamicObject</param>
        private void InitDynamicToInsert(FapDynamicObject dynEntity)
        {
            if (dynEntity == null)
            {
                return;
            }
            string tableName = dynEntity.TableName;
            if (!dynEntity.ContainsKey(FapDbConstants.FAPCOLUMN_FIELD_Fid) || (dynEntity.ContainsKey(FapDbConstants.FAPCOLUMN_FIELD_Fid) && dynEntity.Get(FapDbConstants.FAPCOLUMN_FIELD_Fid).ToString().IsMissing()))
            {
                dynEntity.SetValue(FapDbConstants.FAPCOLUMN_FIELD_Fid, UUIDUtils.Fid);
            }
            dynEntity.SetValue(FapDbConstants.FAPCOLUMN_FIELD_CreateDate, DateTimeUtils.CurrentDateTimeStr);
            dynEntity.SetValue(FapDbConstants.FAPCOLUMN_FIELD_EnableDate, DateTimeUtils.LastSecondDateTimeStr);
            dynEntity.SetValue(FapDbConstants.FAPCOLUMN_FIELD_DisableDate, DateTimeUtils.PermanentTimeStr);
            dynEntity.SetValue(FapDbConstants.FAPCOLUMN_FIELD_Ts, DateTimeUtils.Ts);
            dynEntity.SetValue(FapDbConstants.FAPCOLUMN_FIELD_Dr, 0);
            dynEntity.SetValue(FapDbConstants.FAPCOLUMN_FIELD_CreateBy, _applicationContext.EmpUid);
            dynEntity.SetValue(FapDbConstants.FAPCOLUMN_FIELD_CreateName, _applicationContext.EmpName);
            dynEntity.SetValue(FapDbConstants.FAPCOLUMN_FIELD_OrgUid, _applicationContext.OrgUid);
            dynEntity.SetValue(FapDbConstants.FAPCOLUMN_FIELD_GroupUid, _applicationContext.GroupUid);

            //非系统默认列的默认值的生成
            IEnumerable<FapColumn> columns = Columns(tableName).Where(c => c.IsDefaultCol != 1 && (c.DefaultValueClass.IsPresent() || c.ColDefault.IsPresent()));
            foreach (var column in columns)
            {
                //先判断是否有值，如果有值，则不赋值
                object value = dynEntity.Get(column.ColName);
                if (value != null && !string.IsNullOrWhiteSpace(value.ToString().Trim()))
                {
                    continue;
                }
                if (column.ColDefault.IsPresent())
                {
                    string key = column.ColName;
                    object cv = GetFieldDefaultValue(column);
                    dynEntity.SetValue(key, cv);
                }
            }
            //是否有配置的编码
            Dictionary<string, string> ccr = GetBillCode(tableName);
            if (ccr != null && ccr.Any())
            {
                foreach (var cc in ccr)
                {
                    if (dynEntity.ContainsKey(cc.Key) && (dynEntity.Get(cc.Key) == null || dynEntity.Get(cc.Key).ToString().IsMissing()))
                    {
                        dynEntity.SetValue(cc.Key, cc.Value);
                    }
                    else if (!dynEntity.ContainsKey(cc.Key))
                    {
                        dynEntity.SetValue(cc.Key, cc.Value);
                    }
                }
            }
        }

        private object lockSequence = new object();
        private Dictionary<string, string> GetBillCode(string tableName)
        {
            Dictionary<string, string> dictCodes = new Dictionary<string, string>();

            if (_fapPlatformDomain.CfgBillCodeRuleSet.TryGetValue(tableName, out IEnumerable<CfgBillCodeRule> bcs))
            {
                if (bcs != null && bcs.Any())
                {
                    foreach (var bc in bcs)
                    {
                        string prefix = bc.Prefix;
                        string date = string.Empty;
                        string dateformat = bc.DateFormat;
                        if (dateformat.IsPresent())
                        {
                            date = DateTime.Now.ToString(dateformat);
                        }
                        string seqName = bc.BillEntity + "_" + bc.FieldName;
                        if (bc.ReCountContidion.IsPresent())
                        {
                            if (bc.ReCountContidion.EqualsWithIgnoreCase("year"))
                            {
                                seqName += DateTime.Now.ToString("yyyy");
                            }
                            else if (bc.ReCountContidion.EqualsWithIgnoreCase("month"))
                            {
                                seqName += DateTime.Now.ToString("yyyyMM");
                            }
                            else
                            {
                                seqName += DateTime.Now.ToString("yyyyMMdd");
                            }
                        }
                        int seq = GetSequence(seqName);
                        int totalWidth = 0;
                        if (bc.SequenceLen > 0)
                        {
                            totalWidth = bc.SequenceLen;
                        }
                        string symbol = bc.Symbol;
                        if (symbol.IsMissing())
                        {
                            symbol = "0";
                        }
                        bc.BillCode = prefix + date + seq.ToString().PadLeft(totalWidth, Convert.ToChar(symbol));
                        dictCodes.Add(bc.FieldName, bc.BillCode);
                    }
                }
            }

            //单据没有配置的时候 返回默认的值
            if (IsBill(tableName))
            {
                if (!dictCodes.ContainsKey("BillCode"))
                {
                    int seq = GetSequence(tableName);
                    //bc.BillCode = seq.ToString().PadLeft(7, '0');
                    string billcode = seq.ToString().PadLeft(7, '0');
                    dictCodes.Add("BillCode", billcode);
                }
                dictCodes.Add("BillStatus", BillStatus.DRAFT);
                dictCodes.Add("BillTime", DateTimeUtils.CurrentDateTimeStr);
                dictCodes.Add("BillEmpUid", _applicationContext.EmpUid);
                dictCodes.Add("BillEmpUidMC", _applicationContext.EmpName);
                dictCodes.Add("AppEmpUid", _applicationContext.EmpUid);
                dictCodes.Add("AppEmpUidMC", _applicationContext.EmpName);
            }
            return dictCodes;
            bool IsBill(string tableName)
            {
                FapTable tb = Table(tableName);
                if (tb != null && tb.TableFeature != null && tb.TableFeature.Contains("BillFeature"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// 获取自增序号
            /// </summary>
            /// <param name="seqName">唯一名称</param>
            /// <returns></returns>
            int GetSequence(string seqName)
            {
                lock (lockSequence)
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("SeqName", seqName);
                    CfgSequenceRule sr = _dbSession.QueryFirstOrDefault<CfgSequenceRule>("select * from CfgSequenceRule where SeqName=@SeqName", param);
                    if (sr != null)
                    {
                        sr.CurrValue += sr.StepBy;
                        //InitEntityToUpdate(sr);
                        _dbSession.Update<CfgSequenceRule>(sr);
                        return sr.CurrValue;
                    }
                    else
                    {
                        sr = new CfgSequenceRule { SeqName = seqName, MinValue = 0, StepBy = 1, CurrValue = 1 };
                        //InitEntityToInsert(sr);
                        _dbSession.Insert<CfgSequenceRule>(sr);
                        return 1;
                    }
                }
            }
        }
        /// <summary>
        /// 获取初始化表单数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public FapDynamicObject GetDefualtData(string tableName)
        {
            var fapColumns = Columns(tableName);
            FapDynamicObject dynEntity = new FapDynamicObject(fapColumns);
            //非系统默认列的默认值的生成
            //IEnumerable<FapColumn> columns = fapColumns.Where(c => c.IsDefaultCol != 1 && (c.DefaultValueClass.IsPresent() || c.ColDefault.IsPresent()));
            foreach (var column in fapColumns)
            {
                string key = column.ColName;
                if (column.ColDefault.IsPresent())
                {
                    object cv = GetFieldDefaultValue(column);
                    dynEntity.SetValue(key, cv);
                }
                else
                {
                    dynEntity.SetValue(key, "");
                }
                if (column.IsMultiLang == 1)
                {
                    //设置多语字段
                    var langs= typeof(MultiLanguageEnum).EnumItems();
                    foreach (var lang in langs)
                    {
                        dynEntity.SetValue(key+lang, "");
                    }
                }
            }
            //是否有配置的编码
            Dictionary<string, string> ccr = GetBillCode(tableName);
            if (ccr != null && ccr.Any())
            {
                foreach (var cc in ccr)
                {
                    if (dynEntity.ContainsKey(cc.Key) && (dynEntity.Get(cc.Key) == null || dynEntity.Get(cc.Key).ToString().IsMissing()))
                    {
                        dynEntity.SetValue(cc.Key, cc.Value);
                    }
                    else if (!dynEntity.ContainsKey(cc.Key))
                    {
                        dynEntity.SetValue(cc.Key, cc.Value);
                    }
                }
            }
            return dynEntity;
        }

        #region inner Get       
        private T GetById<T>(long id) where T : BaseModel
        {
            Guard.Against.Zero(id, nameof(id));

            string sqlOri = $"select * from {typeof(T).Name} where id=@Id";
            DynamicParameters param = new DynamicParameters();
            param.Add("Id", id);
            return _dbSession.QueryFirstOrDefault<T>(sqlOri, param);
        }
        private Task<T> GetByIdAsync<T>(long id) where T : BaseModel
        {
            Guard.Against.Zero(id, nameof(id));

            string sqlOri = $"select * from {typeof(T).Name} where id=@Id";
            DynamicParameters param = new DynamicParameters();
            param.Add("Id", id);
            return _dbSession.QueryFirstOrDefaultAsync<T>(sqlOri, param);
        }
        private dynamic GetById(string tableName, long id)
        {
            Guard.Against.NullOrEmpty(tableName, nameof(tableName));
            Guard.Against.Zero(id, nameof(id));

            string sqlOri = $"select * from {tableName} where id=@Id";
            DynamicParameters param = new DynamicParameters();
            param.Add("Id", id);
            return _dbSession.QueryFirstOrDefault(sqlOri, param);
        }
        private Task<dynamic> GetByIdAsync(string tableName, long id)
        {
            Guard.Against.NullOrEmpty(tableName, nameof(tableName));
            Guard.Against.Zero(id, nameof(id));
            string sqlOri = $"select * from {tableName} where id=@Id";
            DynamicParameters param = new DynamicParameters();
            param.Add("Id", id);
            return _dbSession.QueryFirstOrDefaultAsync(sqlOri, param);
        }
        #endregion


        #region Interceptor

        private IDataInterceptor GetTableInterceptor(string dataInterceptorClass)
        {
            IDataInterceptor dataInterceptor = null;
            if (dataInterceptorClass.IsPresent())
            {
                //此处不能缓存，容易使session丢失，若要缓存的话需要重新赋值session
                try
                {
                    Type type = System.Type.GetType(dataInterceptorClass);
                    if (type != null && type.GetInterface("IDataInterceptor") != null)
                    {
                        //dataInterceptor = (IDataInterceptor)Activator.CreateInstance(type, new object[] { _serviceProvider, this });
                        dataInterceptor = (IDataInterceptor)ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, type);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    return null;
                }
            }

            return dataInterceptor;
        }
        private void BeforeInsert<T>(T entity, IDataInterceptor dataInterceptor) where T : BaseModel
        {
            if (dataInterceptor != null)
            {
                dataInterceptor.BeforeEntityInsert(entity);
            }
        }
        private void BeforeDynamicInsert(FapDynamicObject dynEntity, IDataInterceptor dataInterceptor)
        {
            if (dataInterceptor != null)
            {
                dataInterceptor.BeforeDynamicObjectInsert(dynEntity);
            }
        }
        private void AfterInsert<T>(T model, IDataInterceptor dataInterceptor) where T : BaseModel
        {
            if (dataInterceptor != null)
            {
                dataInterceptor.AfterEntityInsert(model);
            }
        }
        private void AfterDynamicInsert(FapDynamicObject dynEntity, IDataInterceptor dataInterceptor)
        {
            if (dataInterceptor != null)
            {
                dataInterceptor.AfterDynamicObjectInsert(dynEntity);
            }
        }
        private void InitEntityToUpdate<T>(T model) where T : BaseModel
        {
            model.UpdateBy = _applicationContext.EmpUid;
            model.UpdateName = _applicationContext.EmpName;
            model.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
        }
        private void InitDynamicToUpdate(FapDynamicObject dynEntity)
        {
            //这里不要设置时间戳，更新的时候时间戳会作为条件，防止并发
            dynEntity.SetValue(FapDbConstants.FAPCOLUMN_FIELD_UpdateBy, _applicationContext.EmpUid);
            dynEntity.SetValue(FapDbConstants.FAPCOLUMN_FIELD_UpdateName, _applicationContext.EmpName);
            dynEntity.SetValue(FapDbConstants.FAPCOLUMN_FIELD_UpdateDate, DateTimeUtils.CurrentDateTimeStr);
        }
        private void BeforeUpdate<T>(T entity, IDataInterceptor dataInterceptor) where T : BaseModel
        {
            if (dataInterceptor != null)
            {
                dataInterceptor.BeforeEntityUpdate(entity);
            }
        }
        private void BeforeDynamicUpdate(FapDynamicObject dynEntity, IDataInterceptor dataInterceptor)
        {
            if (dataInterceptor != null)
            {
                dataInterceptor.BeforeDynamicObjectUpdate(dynEntity);
            }
        }
        private void AfterUpdate<T>(T entity, IDataInterceptor dataInterceptor) where T : BaseModel
        {
            if (dataInterceptor != null)
            {
                dataInterceptor.AfterEntityUpdate(entity);
            }
        }
        private void AfterDynamicUpdate(FapDynamicObject dynEntity, IDataInterceptor dataInterceptor)
        {
            if (dataInterceptor != null)
            {
                dataInterceptor.AfterDynamicObjectUpdate(dynEntity);
            }
        }

        #endregion
        #region update
        private bool UpdateEntity<T>(T entityToUpdate, FapTable table) where T : BaseModel
        {
            string tableName = table.TableName;
            T oldDataClone = GetById<T>(entityToUpdate.Id);
            if (entityToUpdate.Ts != oldDataClone.Ts)
            {
                _logger.LogInformation("时间戳改变，说明数据已经过期，被其他人修改了,需要刷新数据");
                return false;
            }
            if (oldDataClone.DisableDate != DateTimeUtils.PermanentTimeStr)
            {
                _logger.LogInformation("此数据已经失效，不能再更新");
                return false;
            }
            //预处理
            InitEntityToUpdate<T>(entityToUpdate);
            bool execResult = false;
            try
            {
                BeginTransaction();
                //更新前，通过数据拦截器处理数据
                IDataInterceptor interceptor = GetTableInterceptor(table.DataInterceptor);
                BeforeUpdate(entityToUpdate, interceptor);
                //逻辑处理时，还需要根据是否要历史追溯来判断是否逻辑            
                bool isTrace = table.TraceAble == 1;
                if (isTrace)
                {
                    execResult = TraceUpdate<T>(entityToUpdate, oldDataClone, table);
                }
                else
                {
                    execResult = _dbSession.UpdateWithTimestamp(entityToUpdate);
                }
                if (!execResult)
                {
                    Rollback();
                }
                else
                {
                    AfterUpdate(entityToUpdate, interceptor);
                    Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"更新失败:{ex.Message}");
                Rollback();
            }
            //RemoveCache(table.TableName);
            return execResult;
            /// <summary>
            /// 历史追踪更新
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="newEntity"></param>
            /// <returns></returns>
            /// <remarks>修改老数据失效，clone老数据并更新</remarks>
            bool TraceUpdate<T1>(T1 newEntity, T1 oldDataClone, FapTable table) where T1 : BaseModel
            {
                string tableName = table.TableName;
                var fieldList = Columns(tableName).Select(f => f.ColName);
                string columnList = string.Join(',', fieldList.Where(f => !f.EqualsWithIgnoreCase("ID")));
                string paramList = string.Join(',', fieldList.Where(f => !f.EqualsWithIgnoreCase("ID")).Select(f => $"@{f}"));
                //设置clone数据的enabledate为当前时间
                var currDate = DateTimeUtils.LastSecondDateTimeStr;
                try
                {
                    //clone老数据到新数据
                    var oldData = GetById(table.TableName, oldDataClone.Id);
                    long newId = _dbSession.Insert(tableName, columnList, paramList, oldData);
                    //根据传值更新新数据
                    newEntity.Id = newId;
                    newEntity.EnableDate = currDate;
                    _dbSession.Update(newEntity);
                    //修改老数据失效
                    oldDataClone.DisableDate = currDate;
                    return _dbSession.UpdateWithTimestamp<T1>(oldDataClone);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"跟踪更新失败:{ex.Message}");
                    throw;
                }
            }
        }
        private async Task<bool> UpdateEntityAsync<T>(T entityToUpdate, FapTable table) where T : BaseModel
        {
            string tableName = table.TableName;
            T oldDataClone = await GetByIdAsync<T>(entityToUpdate.Id);
            if (entityToUpdate.Ts != oldDataClone.Ts)
            {
                _logger.LogInformation("时间戳改变，说明数据已经过期，被其他人修改了,需要刷新数据");
                return false;
            }
            if (oldDataClone.DisableDate != DateTimeUtils.PermanentTimeStr)
            {
                _logger.LogInformation("此数据已经失效，不能再更新");
                return false;
            }
            //预处理
            InitEntityToUpdate<T>(entityToUpdate);
            BeginTransaction();
            //更新前，通过数据拦截器处理数据
            IDataInterceptor interceptor = GetTableInterceptor(table.DataInterceptor);
            BeforeUpdate(entityToUpdate, interceptor);
            //逻辑处理时，还需要根据是否要历史追溯来判断是否逻辑            
            bool isTrace = table.TraceAble == 1;
            bool execResult = false;
            try
            {
                if (isTrace)
                {
                    execResult = await TraceUpdateAsync<T>(entityToUpdate, oldDataClone, table);
                }
                else
                {
                    execResult = _dbSession.UpdateWithTimestamp(entityToUpdate);
                }
                if (!execResult)
                {
                    Rollback();
                }
                else
                {
                    AfterUpdate(entityToUpdate, interceptor);
                    Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"更新失败:{ex.Message}");
                Rollback();
            }

            //RemoveCache(table.TableName);
            return execResult;
            /// <summary>
            /// 历史追踪更新
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="newEntity"></param>
            /// <returns></returns>
            /// <remarks>修改老数据失效，clone老数据并更新</remarks>
            async Task<bool> TraceUpdateAsync<T1>(T1 newEntity, T1 oldDataClone, FapTable table) where T1 : BaseModel
            {
                string tableName = table.TableName;
                var fieldList = Columns(tableName).Select(f => f.ColName);
                string columnList = string.Join(',', fieldList.Where(f => !f.EqualsWithIgnoreCase("ID")));
                string paramList = string.Join(',', fieldList.Where(f => !f.EqualsWithIgnoreCase("ID")).Select(f => $"@{f}"));
                //设置clone数据的enabledate为当前时间
                var currDate = DateTimeUtils.LastSecondDateTimeStr;
                try
                {
                    //clone老数据到新数据
                    long newId = await _dbSession.InsertAsync(tableName, columnList, paramList, oldDataClone);
                    //根据传值更新新数据
                    newEntity.Id = newId;
                    newEntity.EnableDate = currDate;
                    await _dbSession.UpdateAsync(newEntity);
                    //修改老数据失效
                    oldDataClone.DisableDate = currDate;
                    return _dbSession.UpdateWithTimestamp<T1>(oldDataClone);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"跟踪更新失败:{ex.Message}");
                }
                return false;
            }
        }
        #endregion
        #region delete



        /// <summary>
        /// 设置新的数据有效期状态有效，且删除状态Dr为1
        /// </summary>
        /// <typeparam name="newData">DapperRow</typeparam>
        /// <param name="currDate"></param>
        private void SetNewDynamicToDelete(FapDynamicObject newData, string enableDate, string currDate)
        {
            newData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_EnableDate, enableDate);
            newData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_DisableDate, DateTimeUtils.PermanentTimeStr);
            newData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_Dr, 1);
            newData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_UpdateBy, _applicationContext.EmpUid);
            newData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_UpdateDate, currDate);
            newData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_UpdateName, _applicationContext.EmpName);

        }
        /// <summary>
        /// 逻辑删除，不设置EnableDate
        /// </summary>
        /// <param name="newData">FapDynamicObject</param>
        private void SetDynamicToLogicDelete(FapDynamicObject newData)
        {
            var currDate = DateTimeUtils.CurrentDateTimeStr;
            newData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_DisableDate, DateTimeUtils.PermanentTimeStr);
            newData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_Dr, 1);
            newData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_UpdateBy, _applicationContext.EmpUid);
            newData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_UpdateDate, currDate);
            newData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_UpdateName, _applicationContext.EmpName);

        }
        /// <summary>
        /// 设置新的数据有效期状态有效，且删除状态Dr为1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        private void SetNewEntityToDelete<T>(T entity, string currDate) where T : BaseModel
        {
            entity.EnableDate = currDate;
            entity.DisableDate = DateTimeUtils.PermanentTimeStr;
            entity.Dr = 1;
            entity.UpdateBy = _applicationContext.EmpUid;
            entity.UpdateName = _applicationContext.EmpName;
            entity.UpdateDate = currDate;
        }
        /// <summary>
        /// 逻辑删除，不设置EnableDate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        private void SetEntityToLogicDelete<T>(T entity) where T : BaseModel
        {
            entity.Dr = 1;
            entity.UpdateBy = _applicationContext.EmpUid;
            entity.UpdateName = _applicationContext.EmpName;
            entity.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
        }
        /// <summary>
        /// 删除逻辑，设置旧数据过期即可，其他信息保持不变
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldEntity"></param>
        //private void SetOldEntityInvalid<T>(T oldEntity, string currDate) where T : BaseModel
        //{
        //    //设置旧entity为失效态
        //    oldEntity.Dr = 0;
        //    oldEntity.DisableDate = currDate;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldEntity">FapDynamicObject</param>
        /// <param name="currDate"></param>
        private void SetOldDynamicInvalid(FapDynamicObject oldEntity, string currDate)
        {
            //设置旧entity为失效态
            oldEntity.SetValue(FapDbConstants.FAPCOLUMN_FIELD_Dr, 0);
            oldEntity.SetValue(FapDbConstants.FAPCOLUMN_FIELD_DisableDate, currDate);
        }
        private void BeforeDelete<T>(T model, IDataInterceptor dataInterceptor) where T : BaseModel
        {
            if (dataInterceptor != null)
            {
                dataInterceptor.BeforeEntityDelete(model);
            }
        }
        private void BeforeDynamicDelete(FapDynamicObject dynEntity, IDataInterceptor dataInterceptor)
        {
            if (dataInterceptor != null)
            {
                dataInterceptor.BeforeDynamicObjectDelete(dynEntity);
            }
        }
        private void AfterDelete<T>(T model, IDataInterceptor dataInterceptor) where T : BaseModel
        {
            if (dataInterceptor != null)
            {
                dataInterceptor.AfterEntityDelete(model);
            }
        }
        private void AfterDynamicDelete(FapDynamicObject dynEntity, IDataInterceptor dataInterceptor)
        {
            if (dataInterceptor != null)
            {
                dataInterceptor.AfterDynamicObjectDelete(dynEntity);
            }
        }

        #endregion


        #endregion

        #region 基础操作
        public int Execute(string sqlOri, DynamicParameters parameters = null)
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters);
            return _dbSession.Execute(sql, dynParams);
        }
        public Task<int> ExecuteAsync(string sqlOri, DynamicParameters parameters = null)
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters);
            return _dbSession.ExecuteAsync(sql, dynParams);
        }
        public object ExecuteScalar(string sqlOri, DynamicParameters parameters = null)
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters);
            return _dbSession.ExecuteScalar(sql, dynParams);
        }
        public Task<object> ExecuteScalarAsync(string sqlOri, DynamicParameters parameters = null)
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters);
            return _dbSession.ExecuteScalarAsync(sql, dynParams);
        }
        public T ExecuteScalar<T>(string sqlOri, DynamicParameters parameters = null)
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters);
            return _dbSession.ExecuteScalar<T>(sql, dynParams);
        }
        public Task<T> ExecuteScalarAsync<T>(string sqlOri, DynamicParameters parameters = null)
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters);
            return _dbSession.ExecuteScalarAsync<T>(sql, dynParams);
        }
        /// <summary>
        /// 查询原始sql，不进行包装
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<dynamic> QueryOriSql(string sqlOri, DynamicParameters parameters = null)
        {
            return _dbSession.Query(sqlOri, parameters);
        }
        /// <summary>
        /// 查询原始sql，不进行包装
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<T> QueryOriSql<T>(string sqlOri, DynamicParameters parameters = null) where T : class
        {
            return _dbSession.Query<T>(sqlOri, parameters);
        }
        public IEnumerable<dynamic> Query(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.Query(sql, dynParams);
        }
        public Task<IEnumerable<dynamic>> QueryAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryAsync(sql, dynParams);
        }

        public IEnumerable<T> Query<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : class
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.Query<T>(sql, dynParams);
        }
        public Task<IEnumerable<T>> QueryAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : class
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryAsync<T>(sql, dynParams);
        }
        public IEnumerable<dynamic> QueryAll(string tableName, bool withMC = false)
        {
            string sql = $"select * from {tableName}";
            return Query(sql, null, withMC);
        }
        public Task<IEnumerable<dynamic>> QueryAllAsync(string tableName, bool withMC = false)
        {
            string sql = $"select * from {tableName}";
            return QueryAsync(sql, null, withMC);
        }
        public IEnumerable<T> QueryAll<T>(bool withMC = false) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            string sql = $"select * from {tableName}";
            return Query<T>(sql, null, withMC);
        }
        public Task<IEnumerable<T>> QueryAllAsync<T>(bool withMC = false) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            string sql = $"select * from {tableName}";
            return QueryAsync<T>(sql, null, withMC);
        }
        /// <summary>
        /// 仅当元素个数大于等于1时返回第一个元素，否则抛异常InvalidOperationException: Sequence contains no elements
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        /// <remarks>InvalidOperationException: Sequence contains no elements</remarks>
        public dynamic QueryFirst(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirst(sql, dynParams);
        }
        /// <summary>
        /// 仅当元素个数大于等于1时返回第一个元素，否则抛异常InvalidOperationException: Sequence contains no elements
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        /// <remarks>InvalidOperationException: Sequence contains no elements</remarks>
        public Task<dynamic> QueryFirstAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirstAsync(sql, dynParams);
        }
        /// <summary>
        /// 当元素大于等于1个时，返回第一个元素，否则返回null
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        public dynamic QueryFirstOrDefault(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirstOrDefault(sql, dynParams);
        }
        /// <summary>
        /// 当元素大于等于1个时，返回第一个元素，否则返回null
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        public Task<dynamic> QueryFirstOrDefaultAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirstOrDefaultAsync(sql, dynParams);
        }
        /// <summary>
        /// 仅仅存在单个数据的时候返回，否则抛异常InvalidOperationException
        /// </summary>
        /// <param name="sqlOri">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="withMC">是否编码关联名称</param>
        /// <returns>dynamic,可通过IDictionary<string,object>访问</returns>
        /// <remarks>InvalidOperationException:Sequence contains no elements,InvalidOperationException: Sequence contains more than one element</remarks>
        public dynamic QuerySingle(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingle(sql, dynParams);
        }
        /// <summary>
        /// 仅仅存在单个数据的时候返回，否则抛异常InvalidOperationException
        /// </summary>
        /// <param name="sqlOri">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="withMC">是否编码关联名称</param>
        /// <returns>dynamic,可通过IDictionary<string,object>访问</returns>
        /// <remarks>InvalidOperationException:Sequence contains no elements,InvalidOperationException: Sequence contains more than one element</remarks>
        public Task<dynamic> QuerySingleAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingleAsync(sql, dynParams);
        }
        /// <summary>
        /// 仅仅存在单个或0个数据的时候返回，否则抛异常InvalidOperationException
        /// </summary>
        /// <param name="sqlOri">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="withMC">是否编码关联名称</param>
        /// <returns>dynamic,可通过IDictionary<string,object>访问</returns>
        /// <remarks>InvalidOperationException: Sequence contains more than one element</remarks>
        public dynamic QuerySingleOrDefault(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingleOrDefault(sql, dynParams);
        }
        /// <summary>
        /// 仅仅存在单个或0个数据的时候返回，否则抛异常InvalidOperationException
        /// </summary>
        /// <param name="sqlOri">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="withMC">是否编码关联名称</param>
        /// <returns>dynamic,可通过IDictionary<string,object>访问</returns>
        /// <remarks>InvalidOperationException: Sequence contains more than one element</remarks>
        public Task<dynamic> QuerySingleOrDefaultAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingleOrDefaultAsync(sql, dynParams);
        }
        /// <summary>
        /// 仅当元素个数大于等于1时返回第一个元素，否则抛异常InvalidOperationException: Sequence contains no elements
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        /// <remarks>InvalidOperationException: Sequence contains no elements</remarks>
        public T QueryFirst<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirst<T>(sql, dynParams);
        }
        /// <summary>
        /// 仅当元素个数大于等于1时返回第一个元素，否则抛异常InvalidOperationException: Sequence contains no elements
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        /// <remarks>InvalidOperationException: Sequence contains no elements</remarks>
        public Task<T> QueryFirstAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirstAsync<T>(sql, dynParams);
        }
        /// <summary>
        /// 当元素大于等于1个时，返回第一个元素，否则返回null
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        public T QueryFirstOrDefault<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirstOrDefault<T>(sql, dynParams);
        }
        /// <summary>
        /// 当元素大于等于1个时，返回第一个元素，否则返回null
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        public Task<T> QueryFirstOrDefaultAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirstOrDefaultAsync<T>(sql, dynParams);
        }
        /// <summary>
        /// 仅仅存在单个数据的时候返回，否则抛异常InvalidOperationException
        /// </summary>
        /// <param name="sqlOri">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="withMC">是否编码关联名称</param>
        /// <returns>dynamic,可通过IDictionary<string,object>访问</returns>
        /// <remarks>InvalidOperationException:Sequence contains no elements,InvalidOperationException: Sequence contains more than one element</remarks>
        public T QuerySingle<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingle<T>(sql, dynParams);

        }
        /// <summary>
        /// 仅仅存在单个数据的时候返回，否则抛异常InvalidOperationException
        /// </summary>
        /// <param name="sqlOri">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="withMC">是否编码关联名称</param>
        /// <returns>dynamic,可通过IDictionary<string,object>访问</returns>
        /// <remarks>InvalidOperationException:Sequence contains no elements,InvalidOperationException: Sequence contains more than one element</remarks>
        public Task<T> QuerySingleAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingleAsync<T>(sql, dynParams);
        }
        /// <summary>
        /// 仅仅存在单个或0个数据的时候返回，否则抛异常InvalidOperationException
        /// </summary>
        /// <param name="sqlOri">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="withMC">是否编码关联名称</param>
        /// <returns>dynamic,可通过IDictionary<string,object>访问</returns>
        /// <remarks>InvalidOperationException: Sequence contains more than one element</remarks>
        public T QuerySingleOrDefault<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingleOrDefault<T>(sql, dynParams);
        }
        /// <summary>
        /// 仅仅存在单个或0个数据的时候返回，否则抛异常InvalidOperationException
        /// </summary>
        /// <param name="sqlOri">查询语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="withMC">是否编码关联名称</param>
        /// <returns>dynamic,可通过IDictionary<string,object>访问</returns>
        /// <remarks>InvalidOperationException: Sequence contains more than one element</remarks>
        public Task<T> QuerySingleOrDefaultAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            var (sql, dynParams) = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingleOrDefaultAsync<T>(sql, dynParams);
        }
        public IEnumerable<T> QueryWhere<T>(string where, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            string sql = $"select * from {typeof(T).Name}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            return Query<T>(sql, parameters, withMC);
        }
        public Task<IEnumerable<T>> QueryWhereAsync<T>(string where, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            string sql = $"select * from {typeof(T).Name}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            return QueryAsync<T>(sql, parameters, withMC);
        }
        public IEnumerable<dynamic> QueryWhere(string tableName, string where, DynamicParameters parameters = null, bool withMC = false)
        {
            string sql = $"select * from {tableName}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            return Query(sql, parameters, withMC);
        }
        public Task<IEnumerable<dynamic>> QueryWhereAsync(string tableName, string where, DynamicParameters parameters = null, bool withMC = false)
        {
            string sql = $"select * from {tableName}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            return QueryAsync(sql, parameters, withMC);
        }
        /// <summary>
        /// 当元素大于等于1个时，返回第一个元素，否则返回null
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        public dynamic QueryFirstOrDefaultWhere(string tableName, string where, DynamicParameters parameters = null, bool withMC = false)
        {
            string sql = $"select * from {tableName}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            return QueryFirstOrDefault(sql, parameters, withMC);
        }
        /// <summary>
        /// 当元素大于等于1个时，返回第一个元素，否则返回null
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        public Task<dynamic> QueryFirstOrDefaultWhereAsync(string tableName, string where, DynamicParameters parameters = null, bool withMC = false)
        {
            string sql = $"select * from {tableName}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            return QueryFirstOrDefaultAsync(sql, parameters, withMC);
        }
        /// <summary>
        /// 当元素大于等于1个时，返回第一个元素，否则返回null
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        public T QueryFirstOrDefaultWhere<T>(string where, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            string sql = $"select * from {typeof(T).Name}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            return QueryFirstOrDefault<T>(sql, parameters, withMC);
        }
        /// <summary>
        /// 当元素大于等于1个时，返回第一个元素，否则返回null
        /// </summary>
        /// <param name="sqlOri"></param>
        /// <param name="parameters"></param>
        /// <param name="withMC"></param>
        /// <returns></returns>
        public Task<T> QueryFirstOrDefaultWhereAsync<T>(string where, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            string sql = $"select * from {typeof(T).Name}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            return QueryFirstOrDefaultAsync<T>(sql, parameters, withMC);
        }
        #endregion

        #region Transaction
        //活动事务数量,防止重复提交
        private static ThreadLocal<int> ActiveTransactionNumber = new ThreadLocal<int>(() => 0);
        // private int ActiveTransactionNumber = 0;
        public void BeginTransaction()
        {
            if (ActiveTransactionNumber.Value == 0)
            {
                _logger.LogTrace($"当前线程ID{Thread.CurrentThread.ManagedThreadId},开启事务");
                _dbSession.BeginTransaction();
            }
            ActiveTransactionNumber.Value++;
        }
        public void Commit()
        {
            ActiveTransactionNumber.Value--;
            if (ActiveTransactionNumber.Value == 0)
            {
                try
                {
                    _logger.LogTrace($"当前线程ID{Thread.CurrentThread.ManagedThreadId},提交事务");
                    _dbSession.Commit();

                }
                catch (Exception ex)
                {
                    Rollback();
                    _logger.LogError($"事务提交异常:{ex.Message}");
                    throw;
                }
                finally
                {
                    if (ActiveTransactionNumber.Value == 0)
                    {
                        _dbSession.Dispose();
                    }
                }
            }
        }
        public void Rollback()
        {
            if (ActiveTransactionNumber.Value > 0)
            {
                ActiveTransactionNumber.Value = 0;
                try
                {
                    _logger.LogTrace($"当前线程ID{Thread.CurrentThread.ManagedThreadId},回滚事务");
                    _dbSession.Rollback();

                }
                catch (Exception ex)
                {
                    _logger.LogError($"回滚事务异常:{ex.Message}");
                    throw;
                }
                finally
                {
                    if (ActiveTransactionNumber.Value == 0)
                    {
                        _dbSession.Dispose();
                    }
                }
            }
        }
        #endregion

        #region Get
        public dynamic Get(string tableName, long id, bool withMC = false)
        {
            string sqlOri = $"select * from {tableName} where id=@Id";
            DynamicParameters param = new DynamicParameters();
            param.Add("Id", id);
            return QueryFirstOrDefault(sqlOri, param, withMC);
        }
        public Task<dynamic> GetAsync(string tableName, long id, bool withMC = false)
        {
            string sqlOri = $"select * from {tableName} where id=@Id";
            DynamicParameters param = new DynamicParameters();
            param.Add("Id", id);
            return QueryFirstOrDefaultAsync(sqlOri, null, withMC);
        }
        public dynamic Get(string tableName, string fid, bool withMC = false)
        {
            string sqlOri = $"select * from {tableName} where fid=@Fid";
            DynamicParameters param = new DynamicParameters();
            param.Add("Fid", fid);
            return QueryFirstOrDefault(sqlOri, param, withMC);
        }
        public Task<dynamic> GetAsync(string tableName, string fid, bool withMC = false)
        {
            string sqlOri = $"select * from {tableName} where fid=@Fid";
            DynamicParameters param = new DynamicParameters();
            param.Add("Fid", fid);
            return QueryFirstOrDefaultAsync(sqlOri, param, withMC);
        }
        public T Get<T>(long id, bool withMC = false) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            string sqlOri = $"select * from {tableName} where Id=@Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", id);
            return QueryFirstOrDefault<T>(sqlOri, parameters, withMC);
        }
        public Task<T> GetAsync<T>(long id, bool withMC = false) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            string sqlOri = $"select * from {tableName} where Id=@Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", id);
            return QueryFirstOrDefaultAsync<T>(sqlOri, parameters, withMC);
        }
        public T Get<T>(string fid, bool withMC = false) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            string sqlOri = $"select * from {tableName} where Fid=@Fid";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Fid", fid);
            return QueryFirstOrDefault<T>(sqlOri, parameters, withMC);
        }
        public Task<T> GetAsync<T>(string fid, bool withMC = false) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            string sqlOri = $"select * from {tableName} where Fid=@Fid";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Fid", fid);
            return QueryFirstOrDefaultAsync<T>(sqlOri, parameters, withMC);
        }
        public int Count(string tableName, string where = "", DynamicParameters parameters = null)
        {
            string sqlOri = $"select count(1) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalar<int>(sqlOri, parameters);
        }
        public Task<int> CountAsync(string tableName, string where = "", DynamicParameters parameters = null)
        {
            string sqlOri = $"select count(1) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalarAsync<int>(sqlOri, parameters);
        }
        public int Count<T>(string where = "", DynamicParameters parameters = null)
        {
            string tableName = typeof(T).Name;
            string sqlOri = $"select count(1) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalar<int>(sqlOri, parameters);
        }
        public Task<int> CountAsync<T>(string where = "", DynamicParameters parameters = null)
        {
            string tableName = typeof(T).Name;
            string sqlOri = $"select count(1) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalarAsync<int>(sqlOri, parameters);
        }
        public decimal Sum(string tableName, string colName, string where = "", DynamicParameters parameters = null)
        {
            string sqlOri = $"select sum({colName}) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalar<decimal>(sqlOri, parameters);
        }
        public Task<decimal> SumAsync(string tableName, string colName, string where = "", DynamicParameters parameters = null)
        {
            string sqlOri = $"select sum({colName}) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalarAsync<decimal>(sqlOri, parameters);
        }
        public decimal Sum<T>(string colName, string where = "", DynamicParameters parameters = null)
        {
            string tableName = typeof(T).Name;
            string sqlOri = $"select sum({colName}) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalar<decimal>(sqlOri, parameters);
        }
        public Task<decimal> SumAsync<T>(string colName, string where = "", DynamicParameters parameters = null)
        {
            string tableName = typeof(T).Name;
            string sqlOri = $"select sum({colName}) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalarAsync<decimal>(sqlOri, parameters);
        }


        #endregion

        #region CRUD Entity


        public long Insert<T>(T entityToInsert) where T : BaseModel
        {
            try
            {
                BeginTransaction();
                long id = InsertEntity(entityToInsert);
                Commit();
                return id;
            }
            catch (Exception ex)
            {
                Rollback();
                _logger.LogError($"insert 事务异常:{ex.Message}");
                throw;
            }

            long InsertEntity<T1>(T1 entityToInsert) where T1 : BaseModel
            {
                string tableName = typeof(T).Name;
                FapTable table = Table(tableName);
                //初始化基础数据以及默认值
                InitEntityToInsert<T1>(entityToInsert);
                IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
                //insert前事件
                BeforeInsert<T1>(entityToInsert, dataInterceptor);
                long id = _dbSession.Insert<T1>(entityToInsert);
                //insert 后事件            
                AfterInsert<T1>(entityToInsert, dataInterceptor);
                //RemoveCache(tableName);
                return id;
            }
        }
        public async Task<long> InsertAsync<T>(T entityToInsert) where T : BaseModel
        {
            try
            {
                BeginTransaction();
                long id = await InsertEntityAsync(entityToInsert);
                Commit();
                return id;
            }
            catch (Exception ex)
            {
                Rollback();
                _logger.LogError($"insert 事务异常:{ex.Message}");
                throw;
            }
            async Task<long> InsertEntityAsync<T1>(T1 entityToInsert) where T1 : BaseModel
            {
                string tableName = typeof(T1).Name;
                FapTable table = Table(tableName);
                //初始化基础数据以及默认值
                InitEntityToInsert<T1>(entityToInsert);
                IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
                //insert前事件
                BeforeInsert<T1>(entityToInsert, dataInterceptor);
                long id = await _dbSession.InsertAsync<T1>(entityToInsert);
                //insert 后事件            
                AfterInsert<T1>(entityToInsert, dataInterceptor);
                //RemoveCache(tableName);
                return id;
            }
        }

        /// <summary>
        /// 批量新增，返回影响行数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityListToInsert"></param>
        /// <returns></returns>
        public List<long> InsertBatch<T>(IEnumerable<T> entityListToInsert) where T : BaseModel
        {
            if (entityListToInsert == null || !entityListToInsert.Any())
            {
                return Array.Empty<long>().ToList();
            }
            List<long> vsid = new List<long>();
            foreach (var entity in entityListToInsert)
            {
                try
                {
                    long result = Insert(entity);
                    vsid.Add(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"insertBatch entities method throw exception:{ex.Message}");
                    continue;
                }
            }
            return vsid;
        }
        public async Task<List<long>> InsertBatchAsync<T>(IEnumerable<T> entityListToInsert) where T : BaseModel
        {
            if (entityListToInsert == null || !entityListToInsert.Any())
            {
                return Array.Empty<long>().ToList();
            }
            List<long> vsid = new List<long>();
            foreach (var entity in entityListToInsert)
            {
                try
                {
                    long result = await InsertAsync(entity);
                    vsid.Add(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"insertBatch entities method throw exception:{ex.Message}");
                    continue;
                }
            }
            return vsid;
        }

        public bool Update<T>(T entityToUpdate) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            FapTable table = Table(tableName);

            return UpdateEntity(entityToUpdate, table);
        }
        public async Task<bool> UpdateAsync<T>(T entityToUpdate) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            FapTable table = Table(tableName);

            return await UpdateEntityAsync(entityToUpdate, table);

        }

        public bool UpdateBatch<T>(IEnumerable<T> entityListToUpdate) where T : BaseModel
        {
            if (entityListToUpdate == null || entityListToUpdate.Count() < 1) return false;

            string tableName = typeof(T).Name;
            FapTable table = Table(tableName);

            foreach (var entity in entityListToUpdate)
            {
                UpdateEntity(entity, table);
            }
            return true;

        }
        public async Task<bool> UpdateBatchAsync<T>(IEnumerable<T> entityListToUpdate) where T : BaseModel
        {
            if (entityListToUpdate == null || entityListToUpdate.Count() < 1) return false;

            string tableName = typeof(T).Name;
            FapTable table = Table(tableName);
            foreach (var entity in entityListToUpdate)
            {
                await UpdateEntityAsync(entity, table);
            }
            return true;

        }
        /// <summary>
        /// 逻辑删除，彻底删除请使用exec
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityToDelete"></param>
        /// <param name="dbSession"></param>
        /// <returns></returns>
        public bool Delete<T>(T entityToDelete) where T : BaseModel
        {
            if (entityToDelete == null)
            {
                return false;
            }
            string tableName = typeof(T).Name;
            FapTable table = Table(tableName);
            //是否历史追溯
            bool isTrace = table.TraceAble == 1;
            return DeleteEntity(entityToDelete, table, isTrace);
            bool DeleteEntity<T1>(T1 entityToDelete, FapTable table, bool isTrace) where T1 : BaseModel
            {
                bool execResult = false;
                try
                {
                    BeginTransaction();
                    //删除前，通过数据拦截器处理数据
                    IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
                    BeforeDelete<T1>(entityToDelete, dataInterceptor);
                    if (isTrace)
                    {
                        execResult = TraceDelete<T1>(entityToDelete);
                    }
                    else
                    {
                        //逻辑删除
                        SetEntityToLogicDelete<T1>(entityToDelete);
                        execResult = _dbSession.UpdateWithTimestamp<T1>(entityToDelete);
                    }
                    if (!execResult)
                    {
                        Rollback();
                        _logger.LogInformation($"线程{Thread.CurrentThread.ManagedThreadId}回滚了");
                    }
                    else
                    {
                        //删除后
                        AfterDelete<T1>(entityToDelete, dataInterceptor);
                        Commit();
                        _logger.LogInformation($"线程{Thread.CurrentThread.ManagedThreadId}提交了");
                    }
                }
                catch (Exception ex)
                {
                    Rollback();
                    _logger.LogError($"更新dynamic失败：{ex.Message}");
                    throw ex;
                }
                return execResult;
                bool TraceDelete<T2>(T2 newEntity) where T2 : BaseModel
                {
                    string tableName = table.TableName;
                    var fieldList = Columns(tableName).Select(f => f.ColName);
                    string columnList = string.Join(',', fieldList.Where(f => !f.EqualsWithIgnoreCase("ID")));
                    string paramList = string.Join(',', fieldList.Where(f => !f.EqualsWithIgnoreCase("ID")).Select(f => $"@{f}"));
                    //设置clone数据的enabledate为当前时间
                    var currDate = DateTimeUtils.LastSecondDateTimeStr;
                    try
                    {
                        //clone老数据到新数据
                        var oldData = GetById(table.TableName, newEntity.Id);
                        long newId = _dbSession.Insert(tableName, columnList, paramList, oldData);
                        //修改老数据失效
                        newEntity.DisableDate = currDate;
                        bool execRv = _dbSession.UpdateWithTimestamp<T2>(newEntity);
                        if (execRv)
                        {
                            //根据传值更新新数据
                            newEntity.Id = newId;
                            SetNewEntityToDelete<T2>(newEntity, currDate);
                            _dbSession.Update(newEntity);
                        }
                        return execRv;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"跟踪更新失败:{ex.Message}");
                        throw ex;
                    }
                }
            }
        }
        public async Task<bool> DeleteAsync<T>(T entityToDelete) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            FapTable table = Table(tableName);
            //是否历史追溯
            bool isTrace = table.TraceAble == 1;
            return await DeleteEntityAsync(entityToDelete, table, isTrace);
            async Task<bool> DeleteEntityAsync<T1>(T1 entityToDelete, FapTable table, bool isTrace) where T1 : BaseModel
            {
                bool execResult = false;
                try
                {
                    BeginTransaction();
                    //删除前，通过数据拦截器处理数据
                    IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
                    BeforeDelete<T1>(entityToDelete, dataInterceptor);
                    if (isTrace)
                    {
                        execResult = await TraceDeleteAsync<T1>(entityToDelete);
                    }
                    else
                    {
                        //逻辑删除
                        SetEntityToLogicDelete<T1>(entityToDelete);
                        execResult = _dbSession.UpdateWithTimestamp<T1>(entityToDelete);
                    }
                    if (!execResult)
                    {
                        Rollback();
                    }
                    else
                    {
                        //删除后
                        AfterDelete<T1>(entityToDelete, dataInterceptor);
                        Commit();
                    }
                }
                catch (Exception ex)
                {
                    Rollback();
                    _logger.LogError($"更新dynamic失败：{ex.Message}");
                    throw ex;
                }
                return execResult;
                async Task<bool> TraceDeleteAsync<T2>(T2 newEntity) where T2 : BaseModel
                {
                    string tableName = table.TableName;
                    var fieldList = Columns(tableName).Select(f => f.ColName);
                    string columnList = string.Join(',', fieldList.Where(f => !f.EqualsWithIgnoreCase("ID")));
                    string paramList = string.Join(',', fieldList.Where(f => !f.EqualsWithIgnoreCase("ID")).Select(f => $"@{f}"));
                    //设置clone数据的enabledate为当前时间
                    var currDate = DateTimeUtils.LastSecondDateTimeStr;
                    try
                    {
                        //clone老数据到新数据
                        var oldData = GetById(table.TableName, newEntity.Id);
                        long newId = await _dbSession.Insert(tableName, columnList, paramList, oldData);

                        //修改老数据失效
                        newEntity.DisableDate = currDate;
                        bool rv = _dbSession.UpdateWithTimestamp<T2>(newEntity);
                        if (rv)
                        {
                            //根据传值更新新数据
                            newEntity.Id = newId;
                            SetNewEntityToDelete<T2>(newEntity, currDate);
                            await _dbSession.UpdateAsync(newEntity);
                        }
                        return rv;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"跟踪更新失败:{ex.Message}");
                        throw ex;
                    }
                }
            }
        }
        /// <summary>
        /// 直接语句删除
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <param name="parameters"></param>
        /// <param name="dbSession"></param>
        /// <returns></returns>
        public int DeleteExec(string tableName, string where = "", DynamicParameters parameters = null)
        {
            string sql = $"delete from {tableName}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            int c = Execute(sql, parameters);
            //RemoveCache(tableName);
            return c;
        }
        public async Task<int> DeleteExecAsync(string tableName, string where = "", DynamicParameters parameters = null)
        {
            string sql = $"delete from {tableName}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            int c = await ExecuteAsync(sql, parameters);
            // RemoveCache(tableName);
            return c;
        }
        public bool Delete<T>(string fid) where T : BaseModel
        {
            T entity = Get<T>(fid, false);
            return Delete<T>(entity);
        }
        public Task<bool> DeleteAsync<T>(string fid) where T : BaseModel
        {
            T entity = Get<T>(fid, false);
            return DeleteAsync<T>(entity);
        }
        public bool Delete<T>(long id) where T : BaseModel
        {
            T entity = Get<T>(id, false);
            return Delete<T>(entity);
        }
        public async Task<bool> DeleteAsync<T>(long id) where T : BaseModel
        {
            T entity = await GetAsync<T>(id, false);
            return await DeleteAsync<T>(entity);
        }

        public void DeleteBatch<T>(IEnumerable<T> entityListToDelete) where T : BaseModel
        {
            foreach (var entity in entityListToDelete)
            {
                try
                {
                    Delete(entity);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"删除实体失败：{ex.Message}");
                    throw ex;
                }
            }
        }
        public async Task DeleteBatchAsync<T>(IEnumerable<T> entityListToDelete) where T : BaseModel
        {
            foreach (var entity in entityListToDelete)
            {
                try
                {
                    await DeleteAsync(entity);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"删除实体失败：{ex.Message}");
                    throw ex;
                }
            }
        }
        #endregion

        #region CRUD dynamic
        public long InsertDynamicData(FapDynamicObject fapDynData)
        {
            string tableName = fapDynData.TableName;
            Guard.Against.NullOrWhiteSpace(tableName, nameof(tableName));

            FapTable table = Table(tableName);

            Guard.Against.Null(table, nameof(table));

            try
            {
                BeginTransaction();
                long id = InsertDynamicData();
                Commit();
                return id;
            }
            catch (Exception ex)
            {
                Rollback();
                _logger.LogError($"insert dynamic error:{ex.Message}");
                throw ex;
            }
            long InsertDynamicData()
            {
                InitDynamicToInsert(fapDynData);
                //新增前，通过数据拦截器处理数据
                IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
                BeforeDynamicInsert(fapDynData, dataInterceptor);
                //新增数据
                var keys = fapDynData.Keys.Where(k => !k.EndsWith("MC") && !k.EqualsWithIgnoreCase(FapDbConstants.FAPCOLUMN_FIELD_Id));

                string columnList = string.Join(',', keys);
                string paramList = string.Join(',', keys.Select(k => $"@{k}"));
                long id = _dbSession.Insert(table.TableName, columnList, paramList, fapDynData);// obj);
                //动态需要手动设置Id的值，实体InsertEntity不需要
                fapDynData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_Id, id);
                //新增后，通过数据拦截器处理数据
                AfterDynamicInsert(fapDynData, dataInterceptor);
                //RemoveCache(table.TableName);
                return id;
            }
        }
        public List<long> InsertDynamicDataBatch(IEnumerable<FapDynamicObject> dataObjects)
        {
            if (dataObjects == null || dataObjects.Count() < 1)
            {
                return Array.Empty<long>().ToList();
            }
            List<long> vsid = new List<long>();
            foreach (var dyEntity in dataObjects)
            {
                try
                {
                    long id = InsertDynamicData(dyEntity);
                    vsid.Add(id);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            // RemoveCache(table.TableName);
            return vsid;
        }

        public bool UpdateDynamicData(FapDynamicObject fapDynData)
        {
            string tableName = fapDynData.TableName;

            Guard.Against.NullOrEmpty(tableName, nameof(tableName));

            FapTable table = Table(tableName);

            Guard.Against.Null(table, nameof(table));

            long id = fapDynData.Get(FapDbConstants.FAPCOLUMN_FIELD_Id).ToLong();

            Guard.Against.Zero(id, nameof(id));

            return UpdateDynamicData();

            bool UpdateDynamicData()
            {
                bool execResult = false;
                InitDynamicToUpdate(fapDynData);
                try
                {
                    BeginTransaction();
                    IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
                    BeforeDynamicUpdate(fapDynData, dataInterceptor);
                    bool isTrace = table.TraceAble.ToString().ToBool();
                    if (isTrace)
                    {
                        dynamic oriData = GetById(tableName, id);
                        execResult = TraceDynamicUpdate(oriData);
                    }
                    else
                    {
                        execResult = DynamicObjectUpdate();
                    }
                    if (!execResult)
                    {
                        Rollback();
                    }
                    else
                    {
                        AfterDynamicUpdate(fapDynData, dataInterceptor);
                        Commit();
                    }
                }
                catch (Exception ex)
                {
                    Rollback();
                    execResult = false;
                    _logger.LogError($"更新dynamic失败：{ex.Message}");
                    throw ex;
                }
                return execResult;
                bool DynamicObjectUpdate()
                {
                    return _dbSession.Update(fapDynData);
                }
                bool TraceDynamicUpdate(dynamic oriData)
                {
                    var fieldList = Columns(tableName).Select(c => c.ColName);
                    //将旧数据变成历史数据， 更新后的数据为最新数据
                    try
                    {
                        var currDate = DateTimeUtils.CurrentDateTimeStr;
                        var enableDate = DateTimeUtils.LastSecondDateTimeStr;
                        //复制一份old data 形成新数据，修改EnableDate为当前日期                       
                        oriData.EnableDate = enableDate;
                        string columnList = string.Join(',', fieldList.Where(f => !f.EqualsWithIgnoreCase("ID")));
                        string paramList = string.Join(',', fieldList.Where(f => !f.EqualsWithIgnoreCase("ID")).Select(f => $"@{f}"));
                        long newId = _dbSession.Insert(tableName, columnList, paramList, oriData);

                        //更新新数据
                        fapDynData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_Id, newId);
                        fapDynData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_UpdateDate, currDate);
                        fapDynData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_UpdateBy, _applicationContext.EmpUid);
                        fapDynData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_UpdateName, _applicationContext.EmpName);
                        _dbSession.Update(fapDynData);
                        //修改老数据过期时间
                        FapDynamicObject oldUpdate = new FapDynamicObject(Columns(tableName));
                        long id = oriData.Id;
                        long ts = oriData.Ts;
                        oldUpdate.SetValue(FapDbConstants.FAPCOLUMN_FIELD_Id, id);
                        oldUpdate.SetValue(FapDbConstants.FAPCOLUMN_FIELD_Ts, ts);
                        oldUpdate.SetValue(FapDbConstants.FAPCOLUMN_FIELD_DisableDate, enableDate);
                        return _dbSession.Update(oldUpdate);

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        throw ex;
                    }

                }

            }
        }

        public void UpdateDynamicDataBatch(IEnumerable<FapDynamicObject> dataObjects)
        {
            if (dataObjects == null || dataObjects.Count() == 0)
            {
                return;
            }
            foreach (var d in dataObjects)
            {
                UpdateDynamicData(d);
            }
        }
        /// <summary>
        /// 删除，批量删除请设置Fid多个 用逗号隔开，或Id用逗号隔开
        /// </summary>
        /// <param name="delDynamicData"></param>
        /// <returns></returns>
        public bool DeleteDynamicData(FapDynamicObject delDynamicData)
        {
            string tableName = delDynamicData.TableName;
            Guard.Against.NullOrEmpty(tableName, nameof(tableName));
            string fids = delDynamicData.Get(FapDbConstants.FAPCOLUMN_FIELD_Fid)?.ToString();
            string ids = delDynamicData.Get(FapDbConstants.FAPCOLUMN_FIELD_Id)?.ToString();
            if (fids.IsMissing() && ids.IsMissing())
            {
                throw new FapException("删除数据请设置Fid或Id");
            }
            IEnumerable<dynamic> delList;
            //获取要删除的数据
            if (ids.IsPresent())
            {
                var idList = ids.SplitComma();
                delList = Query($"select * from {tableName} where Id in @Ids", new DynamicParameters(new { Ids = idList }));
            }
            else
            {
                var fidList = fids.SplitComma();
                delList = Query($"select * from {tableName} where Fid in @Fids", new DynamicParameters(new { Fids = fidList }));
            }
            Guard.Against.Null(delList, nameof(delList));

            FapTable table = Table(tableName);

            return TraceDynamicDelete();

            bool TraceDynamicDelete()
            {
                try
                {
                    BeginTransaction();
                    //删除前，通过数据拦截器处理数据
                    IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
                    bool isTrace = table.TraceAble.ToString().ToBool();
                    foreach (var dynamicData in delList.ToFapDynamicObjectList(Columns(tableName)))
                    {
                        BeforeDynamicDelete(dynamicData, dataInterceptor);
                        if (isTrace) //逻辑删除(历史追溯)
                        {
                            TraceDelete(dynamicData);
                        }
                        else //逻辑删除
                        {
                            FapDynamicObject dyData = new FapDynamicObject(Columns(tableName));
                            long id = dynamicData.Get(FapDbConstants.FAPCOLUMN_FIELD_Id).ToLong();
                            long ts = dynamicData.Get(FapDbConstants.FAPCOLUMN_FIELD_Ts).ToLong();
                            dyData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_Id, id);
                            dyData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_Ts, ts);
                            SetDynamicToLogicDelete(dyData);
                            _dbSession.Update(dyData);
                        }
                        AfterDynamicDelete(dynamicData, dataInterceptor);
                    }
                    Commit();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    Rollback();
                    throw ex;
                }
                return true;

                bool TraceDelete(FapDynamicObject dynamicData)
                {
                    long id = dynamicData.Get(FapDbConstants.FAPCOLUMN_FIELD_Id).ToLong();
                    long ts = dynamicData.Get(FapDbConstants.FAPCOLUMN_FIELD_Ts).ToLong();
                    var fieldList = Columns(tableName).Select(c => c.ColName);
                    var currDate = DateTimeUtils.CurrentDateTimeStr;
                    var enableDate = DateTimeUtils.LastSecondDateTimeStr;
                    //insert new data                   
                    SetNewDynamicToDelete(dynamicData, enableDate, currDate);
                    string columnList = string.Join(',', fieldList.Where(f => !f.EqualsWithIgnoreCase("ID")));
                    string paramList = string.Join(',', fieldList.Where(f => !f.EqualsWithIgnoreCase("ID")).Select(f => $"@{f}"));
                    long newId = _dbSession.Insert(tableName, columnList, paramList, dynamicData);
                    //_logger.LogInformation($"-----{newId}-----");
                    //update old data invalid
                    FapDynamicObject dyData = new FapDynamicObject(Columns(tableName));
                    dyData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_Id, id);
                    dyData.SetValue(FapDbConstants.FAPCOLUMN_FIELD_Ts, ts);
                    SetOldDynamicInvalid(dyData, enableDate);
                    return _dbSession.Update(dyData);
                }
            }
        }
        public void DeleteDynamicDataBatch(IEnumerable<FapDynamicObject> dataObjects)
        {
            foreach (var dataObject in dataObjects)
            {
                try
                {
                    DeleteDynamicData(dataObject);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"删除失败：{ex.Message}");
                    //continue;
                    throw ex;
                }
            }

        }
        #endregion

        #region jqgrid paging statistics query
        public PageInfo<T> QueryPage<T>(Pageable pageable) where T : BaseModel
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            InitParamers(dynamicParameters);
            string sql = PagingSQL(pageable);

            //返回总数
            string[] sqls = sql.Split(';', StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, object> parameterMap = pageable.Parameters;
            //设置参数值
            foreach (var item in parameterMap)
            {
                dynamicParameters.Add(item.Key, item.Value);
            }
            PageInfo<T> page = new PageInfo<T>();
            page.PageSize = pageable.PageSize;
            page.TotalCount = ExecuteScalar<int>(sqls[1], dynamicParameters);
            page.Items = Query<T>(sqls[0], dynamicParameters, true);
            page.MaxIdValue = page.Items.Max(a => a.Id);
            page.CurrentPage = pageable.CurrentPage;
            string statSql = StatisticsSql(pageable);
            if (statSql.IsPresent())
            {
                page.StatFieldData = Query(statSql, dynamicParameters);
            }
            return page;
        }
        public PageInfo<dynamic> QueryPage(Pageable pageable)
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            InitParamers(dynamicParameters);
            string sql = string.Empty;
            sql = PagingSQL(pageable);

            //返回总数
            string[] sqls = sql.Split(';', StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, object> parameterMap = pageable.Parameters;
            //设置参数值
            foreach (var item in parameterMap)
            {
                dynamicParameters.Add(item.Key, item.Value);
            }
            PageInfo<dynamic> page = new PageInfo<dynamic>();
            page.PageSize = pageable.PageSize;
            page.TotalCount = ExecuteScalar<int>(sqls[1], dynamicParameters);
            FapSqlParser parse = new FapSqlParser(_fapPlatformDomain, sqls[0], true)
            {
                IsGridQuery = true,
                CurrentLang = _applicationContext.Language.ToString()
            };
            var sqlQuery = parse.ParserSqlStatement();
            page.Items = _dbSession.Query(sqlQuery, dynamicParameters);
            if (page.Items.Count() > 0)
            {
                page.MaxIdValue = page.Items.Max(a => a.Id);
            }
            page.CurrentPage = pageable.CurrentPage;
            string statSql = StatisticsSql(pageable);
            if (statSql.IsPresent())
            {
                page.StatFieldData = QueryFirstOrDefault(statSql, dynamicParameters);
            }
            return page;
        }
        #region page sql
        private string PagingSQL(Pageable pageable)
        {
            //Orderby条件
            string orderBy = pageable.Wraper.MakeOrderBySql();
            if (!string.IsNullOrEmpty(orderBy))
            {
                orderBy = $" ORDER BY {orderBy} ";
            }
            else
            {
                orderBy = " order by Id ";
            }

            //Join条件
            string join = string.Empty;// pageable.Wraper.MakeJoinSql();

            if (pageable.MaxId != null)
            {
                pageable.AddWhere($"Id>{pageable.MaxId}");
            }
            string where = pageable.Wraper.MakeWhere();
            StringBuilder sql = new StringBuilder();
            var databaseDialect = _dbSession.DatabaseDialect;
            if (databaseDialect == DatabaseDialectEnum.MSSQL)
            {
                sql.Append($"select {pageable.Wraper.MakeSelectSql()} from {pageable.Wraper.MakeFromSql()} {join} {where} {orderBy}  offset {pageable.Offset} rows fetch next {pageable.PageSize} rows only ;");

                //总数sql
                sql.Append($"SELECT count(0) FROM {pageable.Wraper.MakeFromSql()} {join} {where}");
            }
            else if (databaseDialect == DatabaseDialectEnum.MYSQL)
            {
                sql.Append($"select {pageable.Wraper.MakeSelectSql()} from {pageable.Wraper.MakeFromSql()} {join} {where} {orderBy}   limit {pageable.Offset},{pageable.PageSize};");

                //总数sql
                sql.Append($"SELECT count(0) FROM {pageable.Wraper.MakeFromSql()} {join} {where}");
            }
            else
            {
                throw new NotImplementedException("jqgrid 分页脚本为实现，暂不支持");
            }
            return sql.ToString();

        }

        private string StatisticsSql(Pageable pageable)
        {
            Dictionary<string, StatSymbolEnum> statFields = pageable.StatFields;
            if (statFields.Any())
            {
                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.Append("SELECT ");
                foreach (var statField in statFields)
                {
                    if (statField.Value == StatSymbolEnum.None)
                    {
                        continue;
                    }
                    if (statField.Value == StatSymbolEnum.Description)
                    {
                        sqlBuilder.Append(statField.Key + ",");
                    }
                    else
                    {
                        //string statTitle = MyEnumHelper.GetDescription(typeof(EnumStatSymbol), statField.Value);
                        string statSymbol = Enum.GetName(typeof(StatSymbolEnum), statField.Value);
                        string statFieldName = statField.Key;
                        sqlBuilder.AppendFormat("{0}({1}) {2},", statSymbol, statFieldName, statFieldName);
                    }
                }
                sqlBuilder.Remove(sqlBuilder.Length - 1, 1);

                sqlBuilder.Append(" FROM ").Append(pageable.Wraper.MakeFromSql());

                //Join条件
                //string join = pageable.Wraper.MakeJoinSql();
                //sqlBuilder.Append($" {join} ");
                //Where条件
                sqlBuilder.Append($" {pageable.Wraper.MakeWhere()} ");
                return sqlBuilder.ToString();
            }
            return string.Empty;
        }


        #endregion

        #endregion

        #region Metadata
        public FapTable Table(string tableName)
        {
            if (!_fapPlatformDomain.TableSet.TryGetValueByName(tableName, out FapTable fapTable))
            {
                Guard.Against.Null(fapTable, nameof(fapTable));
            }
            return fapTable;
        }
        public IEnumerable<FapTable> Tables(string tableCategory)
        {
            return Tables(t => t.TableCategory == tableCategory);
        }
        public IEnumerable<FapTable> Tables(Func<FapTable, bool> predicate)
        {
            return _fapPlatformDomain.TableSet.TryGetValue(predicate);
        }
        public IEnumerable<FapColumn> Columns(string tableName)
        {
            if (!_fapPlatformDomain.ColumnSet.TryGetValueByTable(tableName, out IEnumerable<FapColumn> fapColumns))
            {
                Guard.Against.Null(fapColumns, nameof(fapColumns));
            }
            return fapColumns;
        }
        public FapColumn Column(string tableName, string colName)
        {
            return Columns(tableName).FirstOrDefault(c => c.ColName.EqualsWithIgnoreCase(colName));
        }
        public IEnumerable<FapDict> Dictionarys(string category)
        {
            if (!_fapPlatformDomain.DictSet.TryGetValueByCategory(category, out IEnumerable<FapDict> fapDictionarys))
            {
                Guard.Against.Null(fapDictionarys, nameof(fapDictionarys));
            }
            return fapDictionarys;
        }
        public FapDict Dictionary(string category, string code)
        {
            return Dictionarys(category).FirstOrDefault(d => d.Code.EqualsWithIgnoreCase(code));
        }
        #endregion

        #region history trace
        /// <summary>
        /// 获取指定数据的历史变化
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fid"></param>
        /// <returns></returns>
        public IEnumerable<DataChangeHistory> QueryDataHistory(string tableName, string fid)
        {
            string sql = $"SELECT * FROM {tableName} where  Fid=@Fid ORDER BY Id ASC";
            FapSqlParser parse = new FapSqlParser(_fapPlatformDomain, sql, true);
            sql = parse.ParserSelectSqlNoWhere();

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Fid", fid);
            InitParamers(parameters);

            var list = _dbSession.Query(sql, parameters).ToList();
            FapTable table = _fapPlatformDomain.TableSet.FirstOrDefault(t => t.TableName == tableName);
            var columns = _fapPlatformDomain.ColumnSet.Where(c => c.TableName == tableName);
            for (int i = 0; i < list.Count(); i++)
            {
                if (i == 0)
                {
                    DataChangeHistory dch = CompareDataChange(list[i], null, table, columns);
                    yield return dch;
                }
                else
                {
                    DataChangeHistory dch = CompareDataChange(list[i], list[i - 1], table, columns);
                    yield return dch;
                }
            }
            DataChangeHistory CompareDataChange(dynamic currentData, dynamic preData, FapTable table, IEnumerable<FapColumn> columnList)
            {
                IDictionary<string, object> currRow = currentData as IDictionary<string, object>;
                currRow.TryGetValue(FapDbConstants.FAPCOLUMN_FIELD_UpdateDate, out object datetime);
                DataChangeHistory dcHistory = new DataChangeHistory();
                dcHistory.ChangeDateTime = datetime.ToStringOrEmpty();
                dcHistory.DataName = table.TableComment;
                if (preData == null)
                {
                    dcHistory.ChangeType = DataChangeTypeEnum.ADD;
                    currRow.TryGetValue(FapDbConstants.FAPCOLUMN_FIELD_CreateName, out object creator);
                    dcHistory.Operator = creator == null ? "初始化" : creator.ToStringOrEmpty();
                    return dcHistory;
                }

                currRow.TryGetValue(FapDbConstants.FAPCOLUMN_FIELD_Dr, out object dr);
                currRow.TryGetValue(FapDbConstants.FAPCOLUMN_FIELD_UpdateName, out object updateName);
                if ("1" == dr.ToStringOrEmpty()) //删除标志
                {
                    dcHistory.ChangeType = DataChangeTypeEnum.DELETE;
                    dcHistory.Operator = updateName == null ? "未知" : updateName.ToString();
                    return dcHistory;
                }

                dcHistory.ChangeType = DataChangeTypeEnum.UPDATE;
                dcHistory.Operator = updateName == null ? "未知" : updateName.ToString();
                List<DataItemChangeHistory> dicHistory = new List<DataItemChangeHistory>();
                IDictionary<string, object> preRow = preData as IDictionary<string, object>;
                var keyList = currRow.Keys.ExcludeBaseColumns();
                foreach (var key in keyList)
                {
                    //if (FapDbConstants.FAPCOLUMN_FIELD_Id.Equals(key, StringComparison.CurrentCultureIgnoreCase)
                    //    || FapDbConstants.FAPCOLUMN_FIELD_Fid.Equals(key, StringComparison.CurrentCultureIgnoreCase)
                    //    || FapDbConstants.FAPCOLUMN_FIELD_CreateBy.Equals(key, StringComparison.CurrentCultureIgnoreCase)
                    //    || FapDbConstants.FAPCOLUMN_FIELD_CreateDate.Equals(key, StringComparison.CurrentCultureIgnoreCase)
                    //    || FapDbConstants.FAPCOLUMN_FIELD_CreateName.Equals(key, StringComparison.CurrentCultureIgnoreCase)
                    //    || FapDbConstants.FAPCOLUMN_FIELD_DisableDate.Equals(key, StringComparison.CurrentCultureIgnoreCase)
                    //    || FapDbConstants.FAPCOLUMN_FIELD_EnableDate.Equals(key, StringComparison.CurrentCultureIgnoreCase)
                    //    || FapDbConstants.FAPCOLUMN_FIELD_Ts.Equals(key, StringComparison.CurrentCultureIgnoreCase)
                    //    || FapDbConstants.FAPCOLUMN_FIELD_UpdateBy.Equals(key, StringComparison.CurrentCultureIgnoreCase)
                    //    || FapDbConstants.FAPCOLUMN_FIELD_UpdateDate.Equals(key, StringComparison.CurrentCultureIgnoreCase)
                    //    || FapDbConstants.FAPCOLUMN_FIELD_UpdateName.Equals(key, StringComparison.CurrentCultureIgnoreCase)
                    //    )
                    //{
                    //    continue;
                    //}
                    if (key.EndsWith("MC"))
                    {
                        continue;
                    }
                    string colKey = key;
                    FapColumn col = columnList.Where(c => c.ColName == key).FirstOrDefault();
                    if (col.CtrlType == FapColumn.CTRL_TYPE_FILE || col.CtrlType == FapColumn.CTRL_TYPE_IMAGE)
                    {
                        continue;
                    }
                    if (col.CtrlType == FapColumn.CTRL_TYPE_COMBOBOX || col.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                    {
                        colKey = key + "MC";
                    }
                    currRow.TryGetValue(colKey, out object currentValue);
                    preRow.TryGetValue(colKey, out object preValue);
                    if (col.CtrlType == FapColumn.CTRL_TYPE_CHECKBOX)
                    {
                        currentValue = currentValue.ToInt() == 1 ? "是" : "否";
                        preValue = preValue.ToInt() == 1 ? "是" : "否";
                    }
                    if (!IsObjectEquals(currentValue, preValue))
                    {
                        if (currentValue == null || currentValue.ToString() == "")
                        {
                            currentValue = "空";
                        }
                        if (preValue == null || preValue.ToString() == "")
                        {
                            preValue = "空";
                        }
                        if (preValue == currentValue)
                        {
                            continue;
                        }
                        DataItemChangeHistory dich = new DataItemChangeHistory();
                        if (col != null)
                        {
                            dich.ItemName = $"\"{ col.ColComment}\"";
                            dich.ChangeMessage = "从\"" + preValue + "\"更新成\"" + currentValue + "\"";
                        }
                        else
                        {
                            dich.ChangeMessage = "从\"" + preValue + "]更新成\"" + currentValue + "\"";
                        }
                        dicHistory.Add(dich);
                    }
                }

                dcHistory.ItemChangeList = dicHistory;
                return dcHistory;
            }
            bool IsObjectEquals(object value1, object value2)
            {
                if ((value1 == null && value2 != null) || (value1 != null && value2 == null))
                {
                    return false;
                }
                if (value1 is int && value2 is int)
                {
                    return ((int)value1) == ((int)value2);
                }
                else if (value1 is double && value2 is double)
                {
                    return ((double)value1) == ((double)value2);
                }
                else if (value1 is string && value2 is string)
                {
                    return ((string)value1) == ((string)value2);
                }

                return true;
            }
        }

        #endregion
    }
}

