using Ardalis.GuardClauses;
using Dapper;
using Fap.Core.DataAccess.Interceptor;
using Fap.Core.DataAccess.SqlParser;
using Fap.Core.Exceptions;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Config;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Metadata;
using Fap.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.DataAccess
{
    public class DbContext : IDisposable, IDbContext
    {
        private readonly IFapApplicationContext _applicationContext;
        private readonly ILogger<DbContext> _logger;
        private readonly IDbSession _dbSession;
        private readonly IFapPlatformDomain _fapPlatformDomain;
        private readonly IServiceProvider _serviceProvider;
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
        public string HistoryDateTime { get; set; }
        #region private
        private static Dictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();
        private string WrapSqlAndParam(string sqlOri, DynamicParameters parameters, bool withMC = false, bool withId = false)
        {
            InitParamers(parameters);
            return ParseFapSql(sqlOri, withMC, withId);
            DynamicParameters InitParamers(DynamicParameters parameters)
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

                //日志
                foreach (var paramName in parameters.ParameterNames)
                {
                    _logger.LogInformation($"{parameters}={parameters.Get<object>(paramName)}");
                }

                return parameters;
            }
            string ParseFapSql(string sqlOri, bool withMC = false, bool withId = false)
            {
                _logger.LogInformation($"wrap前的sql:{sqlOri}");
                FapSqlParser parse = new FapSqlParser(_fapPlatformDomain, sqlOri, withMC, withId);
                var sql = parse.GetCompletedSql();
                _logger.LogInformation($"wrap后的sql:{sql}");
                return sql;
            }
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
            IEnumerable<FapColumn> columns = _fapPlatformDomain.ColumnSet.Where(c => c.TableName == tableName && c.IsDefaultCol != 1 && (c.DefaultValueClass.IsPresent() || c.ColDefault.IsPresent()));
            foreach (var column in columns)
            {
                Type type = entity.GetType();
                if (!properties.TryGetValue(column.TableName + column.ColName, out PropertyInfo propertyInfo))
                {
                    properties.Add(column.TableName + column.ColName, (propertyInfo = type.GetProperty(column.ColName)));
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
            else
            {
                return column.ColDefault;
            }
            return "";
        }
        private void InitDynamicToInsert(dynamic dynEntity)
        {
            if (dynEntity == null)
            {
                return;
            }
            string tableName = dynEntity.TableName;
            if (!dynEntity.ContainsKey("Fid") || (dynEntity.ContainsKey("Fid") && string.IsNullOrWhiteSpace(dynEntity.Fid)))
            {
                dynEntity.Fid = UUIDUtils.Fid;
            }
            dynEntity.CreateDate = DateTimeUtils.CurrentDateTimeStr;
            dynEntity.EnableDate = DateTimeUtils.LastSecondDateTimeStr;
            dynEntity.DisableDate = DateTimeUtils.PermanentTimeStr;
            dynEntity.Ts = DateTimeUtils.Ts;
            dynEntity.Dr = 0;
            dynEntity.CreateBy = _applicationContext.EmpUid;
            dynEntity.CreateName = _applicationContext.EmpName;
            dynEntity.OrgUid = _applicationContext.OrgUid;
            dynEntity.GroupUid = _applicationContext.GroupUid;

            //非系统默认列的默认值的生成
            IEnumerable<FapColumn> columns = _fapPlatformDomain.ColumnSet.Where(c => c.TableName == tableName && c.IsDefaultCol != 1 && (c.DefaultValueClass.IsPresent() || c.ColDefault.IsPresent()));
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
                    dynEntity.Add(key, cv);
                }
            }
            //是否有配置的编码
            Dictionary<string, string> ccr = GetBillCode(tableName);
            if (ccr != null && ccr.Any())
            {
                foreach (var cc in ccr)
                {
                    if (dynEntity.ContainsKey(cc.Key) && (dynEntity.Get(cc.Key) == "" || dynEntity.Get(cc.Key) == null))
                    {
                        dynEntity.Add(cc.Key, cc.Value);
                    }
                    else if (!dynEntity.ContainsKey(cc.Key))
                    {
                        dynEntity.Add(cc.Key, cc.Value);
                    }
                }
            }
        }
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
                //CfgBillCodeRule bc = new CfgBillCodeRule();
                //bc.FieldName = "BillCode";
                int seq = GetSequence(tableName);
                //bc.BillCode = seq.ToString().PadLeft(7, '0');
                string billcode = seq.ToString().PadLeft(7, '0');
                dictCodes.Add("BillCode", billcode);
                dictCodes.Add("BillStatus", BillStatus.DRAFT);
            }

            return dictCodes;
        }
        /// <summary>
        /// 获取自增序号
        /// </summary>
        /// <param name="seqName">唯一名称</param>
        /// <param name="stepBy">步长</param>
        /// <returns></returns>
        private int GetSequence(string seqName)
        {
            lock (obj)
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("SeqName", seqName);
                CfgSequenceRule sr = _dbSession.QueryFirstOrDefault<CfgSequenceRule>("select * from CfgSequenceRule where SeqName=@SeqName", param);
                if (sr != null)
                {
                    sr.CurrValue += sr.StepBy;
                    InitEntityToUpdate(sr);
                    _dbSession.Update<CfgSequenceRule>(sr);
                    return sr.CurrValue;
                }
                else
                {
                    sr = new CfgSequenceRule { SeqName = seqName, MinValue = 0, StepBy = 1, CurrValue = 1 };
                    InitEntityToInsert(sr);
                    _dbSession.Insert<CfgSequenceRule>(sr);
                    return 1;
                }
            }
        }
        private bool IsBill(string tableName)
        {
            FapTable tb = _fapPlatformDomain.TableSet.First<FapTable>(t => t.TableName == tableName);
            if (tb != null && tb.TableFeature != null && tb.TableFeature.Contains("BillFeature"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
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
                        dataInterceptor = (IDataInterceptor)Activator.CreateInstance(type, new object[] { _serviceProvider, this });
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

        private object obj = new object();
        private void BeforeInsert<T>(T entity, IDataInterceptor dataInterceptor) where T : BaseModel
        {
            if (dataInterceptor != null)
            {
                dataInterceptor.BeforeEntityInsert(entity);
            }
        }
        private void BeforeDynamicInsert(dynamic dynEntity, IDataInterceptor dataInterceptor)
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
        private void AfterDynamicInsert(dynamic dynEntity, IDataInterceptor dataInterceptor)
        {
            if (dataInterceptor != null)
            {
                dataInterceptor.AfterDynamicObjectInsert(dynEntity);
            }
        }
        private void InitEntityToUpdate<T>(T model) where T : BaseModel
        {
            //这里更新只是以ID为条件，没有加并发控制
            model.Ts = DateTimeUtils.Ts;
            model.UpdateBy = _applicationContext.EmpUid;
            model.UpdateName = _applicationContext.EmpName;
            model.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
        }
        private void InitDynamicToUpdate(dynamic dynEntity)
        {
            //这里不要设置时间戳，更新的时候时间戳会作为条件，防止并发
            dynEntity.UpdateBy = _applicationContext.EmpUid;
            dynEntity.UpdateName = _applicationContext.EmpName;
            dynEntity.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
        }
        private void BeforeUpdate<T>(T entity, IDataInterceptor dataInterceptor) where T : BaseModel
        {
            if (dataInterceptor != null)
            {
                dataInterceptor.BeforeEntityUpdate(entity);
            }
        }
        private void BeforeDynamicUpdate(dynamic dynEntity, IDataInterceptor dataInterceptor)
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
        private void AfterDynamicUpdate(dynamic dynEntity, IDataInterceptor dataInterceptor)
        {
            if (dataInterceptor != null)
            {
                dataInterceptor.AfterDynamicObjectUpdate(dynEntity);
            }
        }
        /// <summary>
        /// 历史追踪更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newEntity"></param>
        /// <param name="oldEntity"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        private T TraceUpdate<T>(T newEntity, T oldEntity) where T : BaseModel
        {
            newEntity.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
            newEntity.EnableDate = DateTimeUtils.LastSecondDateTimeStr;
            if (string.IsNullOrWhiteSpace(newEntity.DisableDate))
            {
                newEntity.DisableDate = DateTimeUtils.PermanentTimeStr;
            }
            newEntity.Ts = DateTimeUtils.Ts;

            if (oldEntity != null)
            {
                //设置旧数据的失效时间
                oldEntity.DisableDate = DateTimeUtils.LastSecondDateTimeStr;
                oldEntity.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
                oldEntity.Ts = DateTimeUtils.Ts;
            }
            try
            {
                BeginTransaction();
                _dbSession.Update<T>(oldEntity);
                long newId = _dbSession.Insert<T>(newEntity);
                Commit();
                newEntity.Id = newId;

            }
            catch (Exception)
            {
                Rollback();
                throw;
            }

            newEntity.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
            newEntity.Ts = DateTimeUtils.Ts;
            return newEntity;
        }
        private async Task<T> TraceUpdateAsync<T>(T newEntity, T oldEntity) where T : BaseModel
        {
            newEntity.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
            newEntity.EnableDate = DateTimeUtils.LastSecondDateTimeStr;
            if (string.IsNullOrWhiteSpace(newEntity.DisableDate))
            {
                newEntity.DisableDate = DateTimeUtils.PermanentTimeStr;
            }
            newEntity.Ts = DateTimeUtils.Ts;

            if (oldEntity != null)
            {
                //设置旧数据的失效时间
                oldEntity.DisableDate = DateTimeUtils.LastSecondDateTimeStr;
                oldEntity.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
                oldEntity.Ts = DateTimeUtils.Ts;
            }
            try
            {
                BeginTransaction();
                await _dbSession.UpdateAsync<T>(oldEntity);
                long newId = await _dbSession.InsertAsync<T>(newEntity);
                Commit();
                newEntity.Id = newId;

            }
            catch (Exception)
            {
                Rollback();
                throw;
            }

            newEntity.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
            newEntity.Ts = DateTimeUtils.Ts;
            return newEntity;
        }
        private long TraceDynamicUpdate(dynamic dynamicData, bool isTrace)
        {
            var fieldList = _fapPlatformDomain.ColumnSet.Where(c => c.TableName == dynamicData.TableName).Select(c => c.ColName);
            string fid = dynamicData.Fid;
            string tableName = dynamicData.TableName;
            dynamic oldData = Get(tableName, fid);
            if (isTrace)
            {   //将旧数据变成历史数据， 更新后的数据为最新数据
                try
                {
                    BeginTransaction();
                    if (oldData != null)
                    {
                        var currDate = DateTimeUtils.CurrentDateTimeStr;
                        //复制一份old data 形成新数据，修改EnableDate为当前日期
                        var newData = oldData;
                        newData.EnableDate = currDate;
                        string columnList = string.Join(',', fieldList.Where(f => !f.EqualsWithIgnoreCase("ID")));
                        string paramList = string.Join(',', fieldList.Where(f => !f.EqualsWithIgnoreCase("ID")).Select(f => $"@{f}"));
                        long newId = _dbSession.Insert(tableName, columnList, paramList, newData);

                        //修改老数据过期时间
                        dynamic oldUpdate = new FapDynamicObject();
                        oldUpdate.TableName = tableName;
                        oldUpdate.Id = oldData.Id;
                        oldUpdate.DisableDate = currDate;
                        oldUpdate.Ts = DateTimeUtils.Ts;
                        oldUpdate.UpdateDate = currDate;
                        oldUpdate.UpdateBy = _applicationContext.EmpUid;
                        oldUpdate.UpdateName = _applicationContext.EmpName;
                        _dbSession.Update(oldUpdate);

                        //更新新数据
                        dynamicData.Id = newId;
                        dynamicData.UpdateDate = currDate;
                        dynamicData.UpdateBy = _applicationContext.EmpUid;
                        dynamicData.UpdateName = _applicationContext.EmpName;
                        bool result = _dbSession.Update(dynamicData);
                        Commit();
                        return newId;
                    }
                    else
                    {
                        _logger.LogError("没查询到任何要更新的数据");
                        throw new FapException("没查询到任何要更新的数据");
                    }
                }
                catch (Exception)
                {
                    Rollback();
                    throw;
                }
            }
            else
            {    //直接更新数据
                dynamicData.Id = oldData.Id;
                bool result = _dbSession.Update(dynamicData);
                return Convert.ToInt32(dynamicData.Id);
            }
        }

        private T TraceDelete<T>(T newEntity, T oldEntity) where T : BaseModel
        {
            //设置新entity为删除状态
            InitEntityToDelete<T>(newEntity);
            //设置旧entity为失效态
            SetEntityInvalid(oldEntity);
            try
            {
                BeginTransaction();
                _dbSession.Insert(newEntity);
                _dbSession.Update(oldEntity);
                Commit();
            }
            catch (Exception)
            {
                Rollback();
                throw;
            }
            return newEntity;
        }
        private async Task<T> TraceDeleteAsync<T>(T newEntity, T oldEntity) where T : BaseModel
        {
            //设置新entity为删除状态
            InitEntityToDelete<T>(newEntity);
            SetEntityInvalid(oldEntity);
            try
            {
                BeginTransaction();
                await _dbSession.InsertAsync(newEntity);
                await _dbSession.UpdateAsync(oldEntity);
                Commit();
            }
            catch (Exception)
            {
                Rollback();
                throw;
            }
            return newEntity;
        }
        private int TraceDynamicDelete(dynamic dynamicData, bool isTrace)
        {

            if (!dynamicData.ContainsKey("Fid") && !dynamicData.ContainsKey("Id"))
            {
                Guard.Against.NullOrEmpty("请指定数据的Fid或者Id", "dynamicData");
                //throw new FapException("请指定数据的Fid或者Id");
            }
            if (dynamicData.TableName == null)
            {
                Guard.Against.NullOrEmpty("请指定表名", "dynamicData");
                //throw new FapException("请指定表名");
            }
            //if (!dynamicData.ContainsKey("Fid"))
            //{
            //    string sql = $"select Fid from {dynamicData.TableName} where Id={dynamicData.Id}";

            //    dynamicData.Fid = _dbSession.ExecuteScalar<string>(sql);

            //}
            try
            {
                int id = dynamicData.Get("Id");
                string tableName = dynamicData.TableName;
                List<string> fieldList = _fapPlatformDomain.ColumnSet.Where(c => c.TableName == dynamicData.TableName).Select(c => c.ColName).ToList();
                if (isTrace) //逻辑删除(历史追溯)
                {
                    try
                    {
                        BeginTransaction();
                        //insert new data
                        var currDate = DateTimeUtils.CurrentDateTimeStr;
                        var newData = Get(dynamicData.TableName, id);
                        newData.EnableDate = currDate;
                        newData.UpdateDate = currDate;
                        newData.UpdateBy = _applicationContext.EmpUid;
                        newData.UpdateName = _applicationContext.EmpName;
                        newData.Ts = DateTimeUtils.Ts;
                        newData.Dr = 1;
                        string columnList = string.Join(',', fieldList.Where(f => !f.EqualsWithIgnoreCase("ID")));
                        string paramList = string.Join(',', fieldList.Where(f => !f.EqualsWithIgnoreCase("ID")).Select(f => $"@{f}"));
                        long newId = _dbSession.Insert(tableName, columnList, paramList, newData);
                        //update old data invalid
                        dynamic dyData = new FapDynamicObject();
                        dyData.TableName = tableName;
                        dyData.Id = id;
                        dyData.DisableDate = DateTimeUtils.PermanentTimeStr;
                        dyData.UpdateDate = currDate;
                        dyData.UpdateBy = _applicationContext.EmpUid;
                        dyData.UpdateName = _applicationContext.EmpName;
                        dyData.Ts = DateTimeUtils.Ts;
                        _dbSession.Update(dyData);
                        Commit();
                        return id;

                    }
                    catch (Exception)
                    {
                        Rollback();
                        throw;
                    }


                }
                else //逻辑删除
                {
                    dynamic dyData = new FapDynamicObject();
                    dyData.TableName = tableName;
                    dyData.Id = id;
                    dyData.Dr = 1;
                    dyData.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
                    dyData.UpdateBy = _applicationContext.EmpUid;
                    dyData.UpdateName = _applicationContext.EmpName;
                    dyData.Ts = DateTimeUtils.Ts;
                    _dbSession.Update(dyData);
                    return id;
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return 0;
        }

        private void InitEntityToDelete<T>(T entity) where T : BaseModel
        {
            entity.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
            entity.Ts = DateTimeUtils.Ts;
            entity.Dr = 1;
            entity.UpdateBy = _applicationContext.EmpUid;
            entity.UpdateName = _applicationContext.EmpName;
            entity.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
        }
        private void SetEntityInvalid<T>(T oldEntity) where T : BaseModel
        {
            //设置旧entity为失效态
            oldEntity.Dr = 0;
            oldEntity.DisableDate = DateTimeUtils.LastSecondDateTimeStr;
            oldEntity.Ts = DateTimeUtils.Ts;
            oldEntity.UpdateBy = _applicationContext.EmpUid;
            oldEntity.UpdateName = _applicationContext.EmpName;
            oldEntity.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
        }
        private void InitDynamicToDelete(dynamic dynEntity)
        {
            dynEntity.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
            dynEntity.Ts = DateTimeUtils.Ts;
            dynEntity.Dr = 1;
            dynEntity.UpdateBy = _applicationContext.EmpUid;
            dynEntity.UpdateName = _applicationContext.EmpName;
            dynEntity.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
        }
        private void BeforeDelete<T>(T model, IDataInterceptor dataInterceptor) where T : BaseModel
        {
            if (dataInterceptor != null)
            {
                dataInterceptor.BeforeEntityDelete(model);
            }
        }
        private void BeforeDynamicDelete(dynamic dynEntity, IDataInterceptor dataInterceptor)
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
        private void AfterDynamicDelete(dynamic dynEntity, IDataInterceptor dataInterceptor)
        {
            if (dataInterceptor != null)
            {
                dataInterceptor.AfterDynamicObjectDelete(dynEntity);
            }
        }
        #endregion

        #region 基础操作
        public int Execute(string sqlOri, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters);
            return _dbSession.Execute(sql, parameters);
        }
        public Task<int> ExecuteAsync(string sqlOri, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters);
            return _dbSession.ExecuteAsync(sql, parameters);
        }
        public object ExecuteScalar(string sqlOri, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters);
            return _dbSession.ExecuteScalar(sql, parameters);
        }
        public Task<object> ExecuteScalarAsync(string sqlOri, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters);
            return _dbSession.ExecuteScalarAsync(sql, parameters);
        }
        public T ExecuteScalar<T>(string sqlOri, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters);
            return _dbSession.ExecuteScalar<T>(sql, parameters);
        }
        public Task<T> ExecuteScalarAsync<T>(string sqlOri, DynamicParameters parameters = null)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters);
            return _dbSession.ExecuteScalarAsync<T>(sql, parameters);
        }
        public IEnumerable<dynamic> Query(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.Query(sql, parameters);
        }
        public Task<IEnumerable<dynamic>> QueryAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryAsync(sql, parameters);
        }

        public IEnumerable<T> Query<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.Query<T>(sql, parameters);
        }
        public Task<IEnumerable<T>> QueryAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryAsync<T>(sql, parameters);
        }
        public IEnumerable<T> QueryAll<T>(bool withMC = false) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            string sql = $"select * from {tableName}";
            return Query<T>(sql);
        }
        public Task<IEnumerable<T>> QueryAllAsync<T>(bool withMC = false) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            string sql = $"select * from {tableName}";
            return QueryAsync<T>(sql);
        }
        public dynamic QueryFirst(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirst(sql, parameters);
        }
        public Task<dynamic> QueryFirstAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirstAsync(sql, parameters);
        }

        public dynamic QueryFirstOrDefault(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirstOrDefault(sql, parameters);
        }
        public Task<dynamic> QueryFirstOrDefaultAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirstOrDefaultAsync(sql, parameters);
        }

        public dynamic QuerySingle(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingle(sql, parameters);
        }
        public Task<dynamic> QuerySingleAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingleAsync(sql, parameters);
        }
        public dynamic QuerySingleOrDefault(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingleOrDefault(sql, parameters);
        }
        public Task<dynamic> QuerySingleOrDefaultAsync(string sqlOri, DynamicParameters parameters = null, bool withMC = false)
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingleOrDefaultAsync(sql, parameters);
        }
        public T QueryFirst<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirst<T>(sql, parameters);
        }
        public Task<T> QueryFirstAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirstAsync<T>(sql, parameters);
        }
        public T QueryFirstOrDefault<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirstOrDefault<T>(sql, parameters);
        }
        public Task<T> QueryFirstOrDefaultAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QueryFirstOrDefaultAsync<T>(sql, parameters);
        }
        public T QuerySingle<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingle<T>(sql, parameters);

        }
        public Task<T> QuerySingleAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingleAsync<T>(sql, parameters);
        }
        public T QuerySingleOrDefault<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingleOrDefault<T>(sql, parameters);
        }
        public Task<T> QuerySingleOrDefaultAsync<T>(string sqlOri, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            var sql = WrapSqlAndParam(sqlOri, parameters, withMC);
            return _dbSession.QuerySingleOrDefaultAsync<T>(sql, parameters);
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
        public dynamic QueryFirstOrDefaultWhere(string tableName, string where, DynamicParameters parameters = null, bool withMC = false)
        {
            string sql = $"select * from {tableName}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            return QueryFirstOrDefault(sql, parameters, withMC);
        }
        public Task<dynamic> QueryFirstOrDefaultWhereAsync(string tableName, string where, DynamicParameters parameters = null, bool withMC = false)
        {
            string sql = $"select * from {tableName}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            return QueryFirstOrDefaultAsync(sql, parameters, withMC);
        }
        public T QueryFirstOrDefaultWhere<T>(string where, DynamicParameters parameters = null, bool withMC = false) where T : BaseModel
        {
            string sql = $"select * from {typeof(T).Name}";
            if (where.IsPresent())
            {
                sql += $" where {where}";
            }
            return QueryFirstOrDefault<T>(sql, parameters, withMC);
        }
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
        private int ActiveTransactionNumber = 0;
        public void BeginTransaction()
        {
            if (ActiveTransactionNumber == 0)
            {
                _dbSession.BeginTransaction();
            }
            ActiveTransactionNumber++;
        }
        public void Commit()
        {
            ActiveTransactionNumber--;
            if (ActiveTransactionNumber == 0)
            {
                try
                {
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
                    Dispose();
                }
            }
        }
        public void Rollback()
        {
            if (ActiveTransactionNumber > 0)
            {
                ActiveTransactionNumber = 0;
                try
                {
                    _dbSession.Rollback();

                }
                catch (Exception ex)
                {
                    _logger.LogError($"回滚事务异常:{ex.Message}");
                    throw;
                }
                finally
                {
                    Dispose();
                }
            }
        }
        public void Dispose()
        {
            _dbSession.Dispose();
        }
        #endregion

        #region Get
        public dynamic Get(string tableName, int id, bool withMC = false)
        {
            string sqlOri = $"select * from {tableName} where id=@Id";
            DynamicParameters param = new DynamicParameters();
            param.Add("Id", id);
            return QueryFirstOrDefault(sqlOri, param, withMC);
        }
        public Task<dynamic> GetAsync(string tableName, int id, bool withMC = false)
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
        public T Get<T>(int id, bool withMC = false) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            string sqlOri = $"select * from {tableName} where Id=@Id";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Id", id);
            return QueryFirstOrDefault<T>(sqlOri, parameters, withMC);
        }
        public Task<T> GetAsync<T>(int id, bool withMC = false) where T : BaseModel
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
        public int Sum(string tableName, string colName, string where = "", DynamicParameters parameters = null)
        {
            string sqlOri = $"select sum({colName}) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalar<int>(sqlOri, parameters);
        }
        public Task<int> SumAsync(string tableName, string colName, string where = "", DynamicParameters parameters = null)
        {
            string sqlOri = $"select sum({colName}) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalarAsync<int>(sqlOri, parameters);
        }
        public int Sum<T>(string colName, string where = "", DynamicParameters parameters = null)
        {
            string tableName = typeof(T).Name;
            string sqlOri = $"select sum({colName}) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalar<int>(sqlOri, parameters);
        }
        public Task<int> SumAsync<T>(string colName, string where = "", DynamicParameters parameters = null)
        {
            string tableName = typeof(T).Name;
            string sqlOri = $"select sum({colName}) from {tableName}";
            if (where.IsPresent())
            {
                sqlOri += $" where {where}";
            }
            return ExecuteScalarAsync<int>(sqlOri, parameters);
        }


        #endregion

        #region CRUD Entity


        public long Insert<T>(T entityToInsert) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            try
            {
                BeginTransaction();
                long id = InsertEntity(entityToInsert, tableName);
                Commit();
                return id;
            }
            catch (Exception ex)
            {
                Rollback();
                _logger.LogError($"insert 事务异常:{ex.Message}");
                throw;
            }
            finally
            {
                Dispose();
            }


        }
        public async Task<long> InsertAsync<T>(T entityToInsert) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            try
            {
                BeginTransaction();
                long id = await InsertEntityAsync(entityToInsert, tableName);
                Commit();
                return id;
            }
            catch (Exception ex)
            {
                Rollback();
                _logger.LogError($"insert 事务异常:{ex.Message}");
                throw;
            }
            finally
            {
                Dispose();
            }


        }
        private long InsertEntity<T>(T entityToInsert, string tableName) where T : BaseModel
        {
            FapTable table = _fapPlatformDomain.TableSet.First<FapTable>(t => t.TableName == tableName);
            //初始化基础数据以及默认值
            InitEntityToInsert<T>(entityToInsert);
            IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
            //insert前事件
            BeforeInsert<T>(entityToInsert, dataInterceptor);
            long id = _dbSession.Insert<T>(entityToInsert);
            entityToInsert.Id = id;
            //insert 后事件            
            AfterInsert<T>(entityToInsert, dataInterceptor);
            //RemoveCache(tableName);
            return id;
        }
        private async Task<long> InsertEntityAsync<T>(T entityToInsert, string tableName) where T : BaseModel
        {
            FapTable table = _fapPlatformDomain.TableSet.First<FapTable>(t => t.TableName == tableName);
            //初始化基础数据以及默认值
            InitEntityToInsert<T>(entityToInsert);
            IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
            //insert前事件
            BeforeInsert<T>(entityToInsert, dataInterceptor);
            long id = await _dbSession.InsertAsync<T>(entityToInsert);
            entityToInsert.Id = id;
            //insert 后事件            
            AfterInsert<T>(entityToInsert, dataInterceptor);
            //RemoveCache(tableName);
            return id;
        }
        /// <summary>
        /// 批量新增，返回影响行数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityListToInsert"></param>
        /// <returns></returns>
        public long InsertBatch<T>(IEnumerable<T> entityListToInsert) where T : BaseModel
        {
            if (entityListToInsert == null || !entityListToInsert.Any())
            {
                return 0;
            }
            string tableName = typeof(T).Name;
            FapTable table = _fapPlatformDomain.TableSet.First<FapTable>(t => t.TableName == tableName);

            try
            {
                BeginTransaction();
                long result = InsertEntityBatch(entityListToInsert, table);
                Commit();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"insertBatch entities method throw exception:{ex.Message}");
                Rollback();
                throw;
            }
            finally
            {
                Dispose();
            }


        }
        public async Task<long> InsertBatchAsync<T>(IEnumerable<T> entityListToInsert) where T : BaseModel
        {
            if (entityListToInsert == null || !entityListToInsert.Any())
            {
                return 0;
            }
            string tableName = typeof(T).Name;
            FapTable table = _fapPlatformDomain.TableSet.First<FapTable>(t => t.TableName == tableName);

            try
            {
                BeginTransaction();
                long result = InsertEntityBatch(entityListToInsert, table);
                Commit();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"insertBatch entities method throw exception:{ex.Message}");
                Rollback();
                throw;
            }
            finally
            {
                Dispose();
            }


        }
        private long InsertEntityBatch<T>(IEnumerable<T> entityListToInsert, FapTable table) where T : BaseModel
        {
            IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
            foreach (var entity in entityListToInsert)
            {
                //预处理
                InitEntityToInsert<T>(entity);
                BeforeInsert(entity, dataInterceptor);
            }
            //批量insert，返回影响行数
            var result = _dbSession.Insert<IEnumerable<T>>(entityListToInsert);
            if (dataInterceptor != null)
            {
                foreach (var entity in entityListToInsert)
                {
                    AfterInsert(entity, dataInterceptor);
                }
            }
            //RemoveCache(table.TableName);
            return result;
        }
        private async Task<long> InsertEntityBatchAsync<T>(IEnumerable<T> entityListToInsert, FapTable table) where T : BaseModel
        {
            IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
            foreach (var entity in entityListToInsert)
            {
                //预处理
                InitEntityToInsert<T>(entity);
                BeforeInsert(entity, dataInterceptor);
            }
            //批量insert，返回影响行数
            var result = await _dbSession.InsertAsync<IEnumerable<T>>(entityListToInsert);
            if (dataInterceptor != null)
            {
                foreach (var entity in entityListToInsert)
                {
                    AfterInsert(entity, dataInterceptor);
                }
            }
            //RemoveCache(table.TableName);
            return result;
        }
        public T Update<T>(T entityToUpdate) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            FapTable table = _fapPlatformDomain.TableSet.First<FapTable>(t => t.TableName == tableName);
            //逻辑处理时，还需要根据是否要历史追溯来判断是否逻辑            
            bool logic = table.TraceAble == 1;
            T tResult = default(T);

            tResult = UpdateEntity(entityToUpdate, table, logic);
            return tResult;

        }
        public async Task<T> UpdateAsync<T>(T entityToUpdate) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            FapTable table = _fapPlatformDomain.TableSet.First<FapTable>(t => t.TableName == tableName);
            //逻辑处理时，还需要根据是否要历史追溯来判断是否逻辑            
            bool logic = table.TraceAble == 1;
            T tResult = default(T);

            tResult = await UpdateEntityAsync(entityToUpdate, table, logic);
            return tResult;

        }
        private T UpdateEntity<T>(T entityToUpdate, FapTable table, bool logic) where T : BaseModel
        {
            T tResult;
            //预处理
            InitEntityToUpdate<T>(entityToUpdate);
            //更新前，通过数据拦截器处理数据
            IDataInterceptor interceptor = GetTableInterceptor(table.DataInterceptor);
            BeforeUpdate(entityToUpdate, interceptor);
            if (logic)
            {
                T oldEntity = Get<T>(entityToUpdate.Fid);
                tResult = TraceUpdate<T>(entityToUpdate, oldEntity);
            }
            else
            {
                var success = _dbSession.Update<T>(entityToUpdate);
                tResult = entityToUpdate;
            }
            AfterUpdate(tResult, interceptor);
            //RemoveCache(table.TableName);
            return tResult;
        }
        private async Task<T> UpdateEntityAsync<T>(T entityToUpdate, FapTable table, bool logic) where T : BaseModel
        {
            T tResult;
            //预处理
            InitEntityToUpdate<T>(entityToUpdate);
            //更新前，通过数据拦截器处理数据
            IDataInterceptor interceptor = GetTableInterceptor(table.DataInterceptor);
            BeforeUpdate(entityToUpdate, interceptor);
            if (logic)
            {
                T oldEntity = await GetAsync<T>(entityToUpdate.Fid);
                tResult = await TraceUpdateAsync<T>(entityToUpdate, oldEntity);
            }
            else
            {
                var success = await _dbSession.UpdateAsync<T>(entityToUpdate);
                tResult = entityToUpdate;
            }
            AfterUpdate(tResult, interceptor);
            //RemoveCache(table.TableName);
            return tResult;
        }
        public bool UpdateBatch<T>(IEnumerable<T> entityListToUpdate) where T : BaseModel
        {
            if (entityListToUpdate == null || entityListToUpdate.Count() < 1) return false;

            string tableName = typeof(T).Name;
            FapTable table = _fapPlatformDomain.TableSet.First<FapTable>(t => t.TableName == tableName);

            //逻辑处理时，还需要根据是否要历史追溯来判断是否逻辑            
            bool isTrace = table.TraceAble == 1;

            UpdateEntityBatch(entityListToUpdate, table, isTrace);
            return true;

        }
        public async Task<bool> UpdateBatchAsync<T>(IEnumerable<T> entityListToUpdate) where T : BaseModel
        {
            if (entityListToUpdate == null || entityListToUpdate.Count() < 1) return false;

            string tableName = typeof(T).Name;
            FapTable table = _fapPlatformDomain.TableSet.First<FapTable>(t => t.TableName == tableName);

            //逻辑处理时，还需要根据是否要历史追溯来判断是否逻辑            
            bool isTrace = table.TraceAble == 1;

            await UpdateEntityBatchAsync(entityListToUpdate, table, isTrace);
            return true;

        }
        private void UpdateEntityBatch<T>(IEnumerable<T> entityListToUpdate, FapTable table, bool isTrace) where T : BaseModel
        {
            IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
            //更新前，通过数据拦截器处理数据
            foreach (var entity in entityListToUpdate)
            {
                //预处理
                InitEntityToUpdate<T>(entity);
                BeforeUpdate(entity, dataInterceptor);
            }
            if (isTrace)
            {
                //历史追溯
                foreach (var entity in entityListToUpdate)
                {
                    T oldEntity = Get<T>(entity.Fid);
                    TraceUpdate<T>(entity, oldEntity);
                }
            }
            else
            {
                var result = _dbSession.Update(entityListToUpdate);
            }
            if (dataInterceptor != null)
            {
                foreach (var entity in entityListToUpdate)
                {
                    AfterUpdate(entity, dataInterceptor);
                }
            }
            //RemoveCache(table.TableName);
        }
        private async Task UpdateEntityBatchAsync<T>(IEnumerable<T> entityListToUpdate, FapTable table, bool isTrace) where T : BaseModel
        {
            IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
            //更新前，通过数据拦截器处理数据
            foreach (var entity in entityListToUpdate)
            {
                //预处理
                InitEntityToUpdate<T>(entity);
                BeforeUpdate(entity, dataInterceptor);
            }
            if (isTrace)
            {
                //历史追溯
                foreach (var entity in entityListToUpdate)
                {
                    T oldEntity = await GetAsync<T>(entity.Fid);
                    await TraceUpdateAsync<T>(entity, oldEntity);
                }
            }
            else
            {
                var result = await _dbSession.UpdateAsync(entityListToUpdate);
            }
            if (dataInterceptor != null)
            {
                foreach (var entity in entityListToUpdate)
                {
                    AfterUpdate(entity, dataInterceptor);
                }
            }
            //RemoveCache(table.TableName);
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
            string tableName = typeof(T).Name;
            FapTable table = _fapPlatformDomain.TableSet.First<FapTable>(t => t.TableName == tableName);
            //是否历史追溯
            bool isTrace = table.TraceAble == 1;


            DeleteEntity(entityToDelete, table, isTrace);
            return true;

        }
        public async Task<bool> DeleteAsync<T>(T entityToDelete) where T : BaseModel
        {
            string tableName = typeof(T).Name;
            FapTable table = _fapPlatformDomain.TableSet.First<FapTable>(t => t.TableName == tableName);
            //是否历史追溯
            bool isTrace = table.TraceAble == 1;

            await DeleteEntityAsync(entityToDelete, table, isTrace);
            return true;

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
        public bool Delete<T>(int id) where T : BaseModel
        {
            T entity = Get<T>(id, false);
            return Delete<T>(entity);
        }
        public async Task<bool> DeleteAsync<T>(int id) where T : BaseModel
        {
            T entity = await GetAsync<T>(id, false);
            return await DeleteAsync<T>(entity);
        }
        private void DeleteEntity<T>(T entityToDelete, FapTable table, bool isTrace) where T : BaseModel
        {
            InitEntityToDelete<T>(entityToDelete);
            //删除前，通过数据拦截器处理数据
            IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
            BeforeDelete<T>(entityToDelete, dataInterceptor);
            if (isTrace)
            {
                T oldEntity = Get<T>(entityToDelete.Fid);
                TraceUpdate<T>(entityToDelete, oldEntity);
            }
            else
            {
                //逻辑删除
                _dbSession.Update<T>(entityToDelete);
            }
            //删除后
            AfterDelete<T>(entityToDelete, dataInterceptor);
            //RemoveCache(table.TableName);
        }
        private async Task DeleteEntityAsync<T>(T entityToDelete, FapTable table, bool isTrace) where T : BaseModel
        {
            InitEntityToDelete<T>(entityToDelete);
            //删除前，通过数据拦截器处理数据
            IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
            BeforeDelete<T>(entityToDelete, dataInterceptor);
            if (isTrace)
            {
                T oldEntity = await GetAsync<T>(entityToDelete.Fid);
                await TraceUpdateAsync<T>(entityToDelete, oldEntity);
            }
            else
            {
                //逻辑删除
                await _dbSession.UpdateAsync<T>(entityToDelete);
            }
            //删除后
            AfterDelete<T>(entityToDelete, dataInterceptor);
            //RemoveCache(table.TableName);
        }
        public bool DeleteBatch<T>(IEnumerable<T> entityListToDelete) where T : BaseModel
        {
            if (entityListToDelete == null || !entityListToDelete.Any())
            {
                return false;
            }
            string tableName = typeof(T).Name;
            FapTable table = _fapPlatformDomain.TableSet.First<FapTable>(t => t.TableName == tableName);
            //是否历史追溯
            bool isTrace = table.TraceAble == 1;

            DeleteEntityBatch(entityListToDelete, table, isTrace);

            return true;

        }
        public async Task<bool> DeleteBatchAsync<T>(IEnumerable<T> entityListToDelete) where T : BaseModel
        {
            if (entityListToDelete == null || !entityListToDelete.Any())
            {
                return false;
            }
            string tableName = typeof(T).Name;
            FapTable table = _fapPlatformDomain.TableSet.First<FapTable>(t => t.TableName == tableName);
            //是否历史追溯
            bool isTrace = table.TraceAble == 1;

            await DeleteEntityBatchAsync(entityListToDelete, table, isTrace);

            return true;

        }
        private void DeleteEntityBatch<T>(IEnumerable<T> entityListToDelete, FapTable table, bool isTrace) where T : BaseModel
        {
            //删除前，通过数据拦截器处理数据
            IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
            foreach (var entity in entityListToDelete)
            {
                InitEntityToDelete(entity);
                BeforeDelete(entity, dataInterceptor);
                if (isTrace)
                {
                    T oldEntity = Get<T>(entity.Fid);
                    TraceUpdate(entity, oldEntity);
                }
            }
            if (!isTrace)
            {
                //逻辑删除
                _dbSession.Update(entityListToDelete);
            }
            foreach (var entity in entityListToDelete)
            {
                //删除后
                AfterDelete<T>(entity, dataInterceptor);

            }
            //RemoveCache(table.TableName);
        }
        private async Task DeleteEntityBatchAsync<T>(IEnumerable<T> entityListToDelete, FapTable table, bool isTrace) where T : BaseModel
        {
            //删除前，通过数据拦截器处理数据
            IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
            foreach (var entity in entityListToDelete)
            {
                InitEntityToDelete(entity);
                BeforeDelete(entity, dataInterceptor);
                if (isTrace)
                {
                    T oldEntity = Get<T>(entity.Fid);
                    await TraceUpdateAsync(entity, oldEntity);
                }
            }
            if (!isTrace)
            {
                //逻辑删除
                await _dbSession.UpdateAsync(entityListToDelete);
            }
            foreach (var entity in entityListToDelete)
            {
                //删除后
                AfterDelete<T>(entity, dataInterceptor);

            }
            //RemoveCache(table.TableName);
        }
        #endregion

        #region CRUD dynamic
        public long InsertDynamicData(dynamic fapDynData)
        {
            if (fapDynData.TableName == null)
            {
                throw new FapException("请指定表名");
            }
            FapTable table = _fapPlatformDomain.TableSet.First<FapTable>(t => t.TableName == fapDynData.TableName);

            long id = InsertDynamicData(fapDynData, table);
            return id;

        }

        private long InsertDynamicData(dynamic fapDynData, FapTable table)
        {
            InitDynamicToInsert(fapDynData);
            //新增前，通过数据拦截器处理数据
            IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
            BeforeDynamicInsert(fapDynData, dataInterceptor);
            //新增数据
            //object obj = fapDynData.ToObject();
            string tableName = fapDynData.TableName;
            string columnList = string.Join(',', fapDynData.ColumnKeys());
            string paramList = string.Join(',', fapDynData.ParamKeys());
            long id = _dbSession.Insert(tableName, columnList, paramList, fapDynData);// obj);

            fapDynData.Id = id;
            //新增后，通过数据拦截器处理数据
            AfterDynamicInsert(fapDynData, dataInterceptor);
            //RemoveCache(table.TableName);
            return id;
        }

        public int InsertDynamicDataBatch(IEnumerable<FapDynamicObject> dataObjects)
        {
            if (dataObjects == null || dataObjects.Count() < 1)
            {
                return 0;
            }
            string tableName = dataObjects.First().TableName;
            if (tableName == null)
            {
                throw new FapException("请指定表名");
            }
            FapTable table = _fapPlatformDomain.TableSet.First<FapTable>(t => t.TableName == tableName);
            Parallel.ForEach(dataObjects, dynamicData =>
            {

                InsertDynamicDataBatch(dynamicData, tableName, table);


            });
            // RemoveCache(table.TableName);
            return dataObjects.Count();
        }

        private void InsertDynamicDataBatch(dynamic dynamicData, string tableName, FapTable table)
        {

            IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);

            dynamic dynData = dynamicData;
            InitDynamicToInsert(dynData);
            //新增前，通过数据拦截器处理数据
            BeforeDynamicInsert(dynData, dataInterceptor);
            //新增数据
            object obj = dynamicData.ToObject();
            string columnList = string.Join(',', dynamicData.ColumnKeys());
            string paramList = string.Join(',', dynamicData.ParamKeys());
            long id = _dbSession.Insert(tableName, columnList, paramList, obj);

            dynData.Id = id;
            //新增后，通过数据拦截器处理数据
            AfterDynamicInsert(dynData, dataInterceptor);

            //foreach (var dynamicData in dataObjects)
            //{
            //    dynamic dynData = dynamicData;
            //    InitDynamicToInsert(dynData, session);
            //    //新增前，通过数据拦截器处理数据
            //    BeforeDynamicInsert(dynData, dataInterceptor);
            //    //新增数据
            //    object obj = dynamicData.ToObject();
            //    string columnList =string.Join(',', fapDynData.ColumnKeys());
            //    string paramList =string.join(',', dynamicData.ParamKeys());
            //    long id = session.Insert(tableName, columnList, paramList, obj);

            //    dynData.Id = id;
            //    //新增后，通过数据拦截器处理数据
            //    AfterDynamicInsert(dynData, dataInterceptor);
            //}

        }

        public bool UpdateDynamicData(dynamic fapDynData)
        {
            string fid = string.Empty;
            long Id = 0;
            dynamic dataObject = fapDynData;
            //更新根据Fid
            if (!dataObject.ContainsKey("Fid") || dataObject.Fid == null)
            {
                if (dataObject.ContainsKey("Id") && dataObject.Id != null)
                {
                    Id = Convert.ToInt64(dataObject.Get("Id"));
                    fid = ExecuteScalar($"select Fid from {dataObject.TableName} where Id={Id}").ToString();
                    dataObject.Add("Fid", fid);
                }
                else
                {
                    throw new FapException("更新数据，请设置Key的值,Id or Fid");
                }
            }
            else
            {
                fid = dataObject.Fid;
            }

            if (dataObject.TableName == null)
            {
                throw new FapException("请指定表名");
            }
            if (fid.IsMissing() && Id == 0)
            {
                throw new FapException("更新数据，请设置Key的值");
            }
            if (fid.IsMissing() && Id > 0)
            {
                throw new FapException("更新数据，请设置Key的值,Id or Fid");
            }

            FapTable table = _fapPlatformDomain.TableSet.First<FapTable>(t => t.TableName == dataObject.TableName);
            bool isTrace = table.TraceAble.ToString().ToBool();
            InitDynamicToUpdate(dataObject);

            UpdateDynamicData(dataObject, table, isTrace);
            return true;


        }

        private void UpdateDynamicData(dynamic dataObject, FapTable table, bool isTrace)
        {
            IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
            BeforeDynamicUpdate(dataObject, dataInterceptor);
            TraceDynamicUpdate(dataObject, isTrace);
            AfterDynamicUpdate(dataObject, dataInterceptor);
            //RemoveCache(table.TableName);
        }

        public int UpdateDynamicDataBatch(IEnumerable<dynamic> dataObjects)
        {
            if (dataObjects == null || dataObjects.Count() == 0)
            {
                return 0;
            }
            string tableName = dataObjects.First().TableName;
            FapTable table = _fapPlatformDomain.TableSet.First<FapTable>(t => t.TableName == tableName);
            bool isTrace = table.TraceAble.ToString().ToBool();

            int i = UpdateDynamicDataBatch(dataObjects, table, isTrace);
            return i;



        }

        private int UpdateDynamicDataBatch(IEnumerable<dynamic> dataObjects, FapTable table, bool isTrace)
        {
            //更新前，通过数据拦截器处理数据
            IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
            int i = 0;
            foreach (var dataObject in dataObjects)
            {
                //更新根据Fid
                if (dataObject.Fid == null)
                {
                    throw new FapException("请指定数据的FID");
                }
                if (dataObject.TableName == null)
                {
                    throw new FapException("请指定表名");
                }
                InitDynamicToUpdate(dataObject);
                BeforeDynamicUpdate(dataObject, dataInterceptor);
                UpdateDynamicData(dataObject, table, isTrace);
                AfterDynamicUpdate(dataObject, dataInterceptor);
                i++;
            }


            return i;
        }

        public int DeleteDynamicData(dynamic dataObject)
        {
            if (dataObject.TableName == null)
            {
                throw new FapException("请指定表名");
            }
            FapTable table = _fapPlatformDomain.TableSet.First<FapTable>(t => t.TableName == dataObject.TableName);

            int i = DeleteDynamicData(dataObject, table);
            return i;


        }

        private int DeleteDynamicData(dynamic dataObject, FapTable table)
        {
            //预处理：默认字段
            InitDynamicToDelete(dataObject);
            //删除前，通过数据拦截器处理数据
            IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
            BeforeDynamicDelete(dataObject, dataInterceptor);

            int i = TraceDynamicDelete(dataObject, table.TraceAble.ToString().ToBool());
            AfterDynamicDelete(dataObject, dataInterceptor);
            //RemoveCache(table.TableName);
            return i;
        }

        public bool DeleteDynamicDataBatch(IEnumerable<dynamic> dataObjects)
        {
            FapTable table = _fapPlatformDomain.TableSet.First<FapTable>(t => t.TableName == dataObjects.First().TableName);

            DeleteDynamicDataBatch(dataObjects, table);
            return true;

        }

        private void DeleteDynamicDataBatch(IEnumerable<dynamic> dataObjects, FapTable table)
        {
            foreach (var dataObject in dataObjects)
            {
                //预处理：默认字段
                InitDynamicToDelete(dataObject);
                //删除前，通过数据拦截器处理数据
                IDataInterceptor dataInterceptor = GetTableInterceptor(table.DataInterceptor);
                BeforeDynamicDelete(dataObject, dataInterceptor);

                int i = TraceDynamicDelete(dataObject, table.TraceAble.ToString().ToBool());
                AfterDynamicDelete(dataObject, dataInterceptor);
            }
            // RemoveCache(table.TableName);
        }
        #endregion

    }
}

