using Dapper;
using Fap.AspNetCore.Model;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Config;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Infrastructure.Interface;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Infrastructure.Model;
using Fap.Core.Rbac.Model;
using Fap.Core.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Workflow.Service
{
    /// <summary>
    /// 单据回写类
    /// </summary>
    [Service]
    public class WriteBackService : IWriteBackService
    {
        private readonly IDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WriteBackService> _logger;
        private readonly IFapApplicationContext _applicationContext;
        public WriteBackService(IDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<WriteBackService>();
            _applicationContext = serviceProvider.GetService<IFapApplicationContext>();
        }
        #region 审批完成单据回写
        /// <summary>
        /// 根据配置，执行单据回写业务表
        /// </summary>
        /// <param name="tableName">单据表</param>
        /// <param name="fid">单据数据Fid</param>
        public void WriteBackToBusiness(string tableName, string fid)
        {
            //单据实体
            IDictionary<string, object> billData = _dbContext.Get(tableName, fid, true) as IDictionary<string, object>;
            //业务实体 只有设置了映射才会有            
            FapDynamicObject fapBizData = null;
            IDictionary<string, object> dicBizData = null;
            string bizTableName = string.Empty;
            billData.TryGetValue("EffectiveTime", out object effectiveTime);
            if (effectiveTime != null && effectiveTime.ToString().IsPresent())
            {
                DateTime effTime;
                if (DateTime.TryParse(effectiveTime.ToString(), out effTime))
                {
                    //生效时间大于当前时间
                    if (effTime > DateTime.Now)
                    {
                        return;
                    }
                }
            }
            if (billData.ContainsKey("EffectiveState"))
            {
                //设置生效状态
                billData["EffectiveState"] = 1;
            }
            var fapBillData = billData.ToFapDynamicObject(_dbContext.Columns(tableName));
            if (billData.ContainsKey("EffectiveState"))
            {
                _dbContext.UpdateDynamicData(fapBillData);
            }
            DynamicParameters param = new DynamicParameters();
            param.Add("TableName", tableName);
            IEnumerable<CfgBillWriteBackRule> rules = _dbContext.QueryWhere<CfgBillWriteBackRule>("DocEntityUid=@TableName", param);
            if (rules != null && rules.Any())
            {
                foreach (var rule in rules)
                {
                    bizTableName = rule.BizEntityUid;
                    //获取单据实体数据
                    //没有关联字段，就是新增
                    if (rule.Association.IsMissing() && rule.FieldMapping.IsPresent())
                    {
                        //获取mapping
                        JArray mappings = JArray.Parse(rule.FieldMapping);
                        if (mappings != null && mappings.Any())
                        {
                            FapDynamicObject fdo = new FapDynamicObject(_dbContext.Columns(bizTableName));

                            foreach (JObject map in mappings)
                            {
                                //id存放字段的映射,billtablename.colname,biztablename.colname
                                string id = map.GetValue("id").ToString();
                                string[] arryMap = id.Split(',');
                                string billCol = arryMap[0].Split('.')[1];

                                string bizCol = arryMap[1].Split('.')[1];
                                billData.TryGetValue(billCol, out object bo);
                                fdo.SetValue(bizCol, bo ?? "");
                            }
                            //insert
                            _dbContext.InsertDynamicData(fdo);
                            //提醒到工资保险
                            NotifyPayroll(rule, fapBillData, fdo);
                            NotifyInsurance(rule, fapBillData, fdo);
                        }

                    }
                    else
                    {
                        //更新
                        string association = rule.Association;
                        //获取关联字段值，这个是更新业务表的条件
                        billData.TryGetValue(association, out object fidValue);
                        if (fidValue != null && rule.FieldMapping.IsPresent())
                        {
                            JArray mappings = JArray.Parse(rule.FieldMapping);
                            if (mappings != null && mappings.Any())
                            {
                                //根据关联值获取业务值
                                dicBizData = _dbContext.Get(bizTableName, fidValue.ToString()) as IDictionary<string, object>;
                                if (dicBizData != null)
                                {
                                    fapBizData = dicBizData.ToFapDynamicObject(_dbContext.Columns(bizTableName));
                                    //更新业务对象
                                    foreach (JObject map in mappings)
                                    {
                                        //id存放字段的映射,billtablename.colname,biztablename.colname
                                        string id = map.GetValue("id").ToString();
                                        string[] arryMap = id.Split(',');
                                        string billCol = arryMap[0].Split('.')[1];

                                        string bizCol = arryMap[1].Split('.')[1];
                                        billData.TryGetValue(billCol, out object bo);
                                        fapBizData.SetValue(bizCol, bo ?? "");
                                    }
                                    //bizData.TableName = bizTableName;
                                    //update
                                    _dbContext.UpdateDynamicData(fapBizData);
                                    //提醒到工资保险
                                    NotifyPayroll(rule, fapBillData, fapBizData);
                                    NotifyInsurance(rule, fapBillData, fapBizData);
                                }
                                else
                                {
                                    //新增业务对象
                                    FapDynamicObject fdo = new FapDynamicObject(_dbContext.Columns(bizTableName));

                                    foreach (JObject map in mappings)
                                    {
                                        //id存放字段的映射,billtablename.colname,biztablename.colname
                                        string id = map.GetValue("id").ToString();
                                        string[] arryMap = id.Split(',');
                                        string billCol = arryMap[0].Split('.')[1];

                                        string bizCol = arryMap[1].Split('.')[1];
                                        billData.TryGetValue(billCol, out object bo);
                                        fdo.SetValue(bizCol, bo ?? "");
                                    }
                                    fdo.SetValue(FapDbConstants.FAPCOLUMN_FIELD_Fid, UUIDUtils.Fid);

                                    //insert
                                    _dbContext.InsertDynamicData(fdo);
                                    dicBizData = _dbContext.Get(bizTableName, fdo.Get(FapDbConstants.FAPCOLUMN_FIELD_Fid).ToString()) as IDictionary<string, object>;
                                    fapBizData = billData.ToFapDynamicObject(_dbContext.Columns(bizTableName));
                                    //提醒到工资保险
                                    NotifyPayroll(rule, fapBillData, fapBizData);
                                    NotifyInsurance(rule, fapBillData, fapBizData);
                                }
                            }
                        }
                    }
                    //执行自定义update sql                    
                    if (rule.CustomSql.IsPresent())
                    {
                        string sql = GetFillSql(rule.CustomSql, billData, dicBizData, tableName, bizTableName);

                        _dbContext.Execute(sql);

                    }

                    //回调类
                    if (rule.CallBackClass.IsPresent())
                    {
                        try
                        {
                            Type type = System.Type.GetType(rule.CallBackClass);
                            if (type != null && type.GetInterface("IBillWritebackService") != null)
                            {
                                IBillWritebackService wb = (IBillWritebackService)ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, type);
                                wb.Exec(fapBillData, fapBizData);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex.Message);
                        }
                    }
                }

            }

        }
        private string GetFillSql(string customSql, IDictionary<string, object> billData, IDictionary<string, object> bizData, string billTable, string bizTable)
        {
            string _customSql = customSql.ToLower();
            if (billData != null)
            {
                foreach (var item in billData)
                {
                    string field = "${" + billTable + "_" + item.Key.ToStringOrEmpty() + "}".ToLower();
                    if (_customSql.Contains(field))
                    {
                        _customSql = _customSql.Replace(field, item.Value.ToStringOrEmpty());
                    }
                }
            }

            if (bizData != null)
            {
                foreach (var item in bizData)
                {
                    string field = "${" + bizTable + "_" + item.Key.ToStringOrEmpty() + "}".ToLower();
                    if (_customSql.Contains(field))
                    {
                        _customSql = _customSql.Replace(field, item.Value.ToStringOrEmpty());
                    }
                }
            }

            return _customSql;
        }

        /// <summary>
        /// 提醒到工资
        /// </summary>
        /// <param name="da"></param>
        /// <param name="rule"></param>
        /// <param name="billData">单据数据</param>
        /// <param name="bizObject">业务数据</param>
        private void NotifyPayroll(CfgBillWriteBackRule rule, FapDynamicObject billData, FapDynamicObject bizObject)
        {
            //影响工资,业务单据必须是Employee
            if (rule.IsNotifyPayroll == 1 && rule.BizEntityUid == "Employee")
            {
                string payCaseSql = "select Fid,EmpCondition from PayCase where TableName!=''";
                var payCases = _dbContext.Query(payCaseSql);
                if (payCases == null || payCases.Count() < 1)
                {
                    return;
                }
                string empUid = bizObject.Get("Fid").ToString();
                Employee empInfo = _dbContext.Get<Employee>(empUid);
                List<PayToDo> payToDos = new List<PayToDo>();
                foreach (var pc in payCases)
                {
                    //解析工资套中的员工条件
                    string strWhere = "Fid=@Fid";
                    JsonFilterToSql jsont2sql = new JsonFilterToSql(_dbContext);
                    string payWhere = jsont2sql.BuilderFilter("Employee", pc.EmpCondition);
                    if (payWhere.IsPresent())
                    {
                        strWhere += " and " + payWhere;
                    }
                    DynamicParameters param = new DynamicParameters();
                    param.Add("Fid", empUid);
                    if (_dbContext.Count("Employee", strWhere, param) > 0)
                    {
                        string caseUid = pc.Fid;
                        var payToDo = BuilderPayToDo(rule, billData, empInfo, caseUid);
                        payToDos.Add(payToDo);
                    }
                }
                if (payToDos.Count > 0)
                {
                    _dbContext.InsertBatch(payToDos);
                }
                else
                {
                    var payToDo = BuilderPayToDo(rule, billData, empInfo, "");
                    _dbContext.Insert(payToDo);
                }
            }


        }
        /// <summary>
        /// 提醒到保险
        /// </summary>
        /// <param name="da"></param>
        /// <param name="rule"></param>
        /// <param name="billData">单据数据</param>
        /// <param name="bizObject">业务数据</param>
        private void NotifyInsurance(CfgBillWriteBackRule rule, FapDynamicObject billData, FapDynamicObject bizObject)
        {
            //影响工资同时影响保险,业务单据必须是Employee
            if (rule.IsNotifyPayroll == 1 && rule.BizEntityUid == "Employee")
            {
                string insCaseSql = "select Fid,EmpCondition from InsCase where TableName!=''";
                var insCases = _dbContext.Query(insCaseSql);
                if (insCases == null || insCases.Count() < 1)
                {
                    return;
                }
                bizObject.TryGetValue("Fid", out object empUid);
                Employee empInfo = _dbContext.Get<Employee>(empUid.ToString());
                List<InsToDo> insToDos = new List<InsToDo>();
                foreach (var cs in insCases)
                {
                    //解析保险组中的员工条件
                    string strWhere = "Fid=@Fid";
                    JsonFilterToSql jsont2sql = new JsonFilterToSql(_dbContext);
                    string insWhere = jsont2sql.BuilderFilter("Employee", cs.EmpCondition);
                    if (insWhere.IsPresent())
                    {
                        strWhere += " and " + insWhere;
                    }
                    DynamicParameters param = new DynamicParameters();
                    param.Add("Fid", empUid);
                    if (_dbContext.Count("Employee", strWhere, param) > 0)
                    {
                        string caseUid = cs.Fid;
                        var insToDo = BuilderInsToDo(rule, billData, empInfo, caseUid);
                        insToDos.Add(insToDo);
                    }
                }
                if (insToDos.Count > 0)
                {
                    _dbContext.InsertBatch(insToDos);
                }
                else
                {
                    var insToDo = BuilderInsToDo(rule, billData, empInfo, "");
                    _dbContext.Insert(insToDo);
                }
            }


        }

        private InsToDo BuilderInsToDo(CfgBillWriteBackRule rule, FapDynamicObject billData, Employee empInfo, string caseUid)
        {
            //影响此保险组
            InsToDo insToDo = new InsToDo();
            insToDo.EmpUid = empInfo.Fid;
            insToDo.EmpCode = empInfo.EmpCode;
            insToDo.DeptUid = empInfo.DeptUid;
            insToDo.DeptCode = empInfo.DeptCode;
            insToDo.CaseUid = caseUid;//受影响保险组
            insToDo.TableUid = rule.DocEntityUid;//单据实体
            insToDo.BizDate = DateTimeUtils.CurrentDateStr;//业务日期--当前时间
            insToDo.OperEmpUid = billData.Get("BillEmpUid") == null ? _applicationContext.EmpUid : billData.Get("BillEmpUid").ToString();//变动处理人--制单人
            insToDo.OperFlag = "0";//变动应用 默认为0
            insToDo.TransID = billData.Get("Id").ToString();//单据ID
            return insToDo;
        }
        private PayToDo BuilderPayToDo(CfgBillWriteBackRule rule, FapDynamicObject billData, Employee empInfo, string caseUid)
        {
            //影响此薪资套
            PayToDo payToDo = new PayToDo();
            payToDo.EmpUid = empInfo.Fid;
            payToDo.EmpCode = empInfo.EmpCode;
            payToDo.DeptUid = empInfo.DeptUid;
            payToDo.DeptCode = empInfo.DeptCode;
            payToDo.CaseUid = caseUid;//受影响薪资套
            payToDo.TableUid = rule.DocEntityUid;//单据实体
            payToDo.BizDate = DateTimeUtils.CurrentDateStr;//业务日期--当前时间
            payToDo.OperEmpUid = billData.Get("BillEmpUid")==null?_applicationContext.EmpUid: billData.Get("BillEmpUid").ToString();//变动处理人--制单人
            payToDo.OperFlag = "0";//变动应用 默认为0
            payToDo.TransID = billData.Get("Id").ToString();//单据ID
            return payToDo;
        }


        public void Approved(string billTable, string billUid)
        {
            string sql = $"update {billTable} set BillStatus='{BillStatus.PASSED}' where Fid='{billUid}'";
            _dbContext.Execute(sql);
        }

        public void HandleWhenError()
        {
            throw new NotImplementedException();
        }

        public void Rejected(string billTable, string billUid)
        {
            string sql = $"update {billTable} set BillStatus='{BillStatus.REVOKED}' where Fid='{billUid}'";
            _dbContext.Execute(sql);
        }
        #endregion
    }
}
