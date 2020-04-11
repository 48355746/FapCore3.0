using Dapper;
using Fap.AspNetCore.Model;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Config;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Infrastructure.Interface;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Rbac.Model;
using Fap.Core.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Workflow.Engine.WriteBack
{
    /// <summary>
    /// 单据回写类
    /// </summary>
    [Service]
    public class BillWriteBack : IWriteBackRule
    {
        private readonly IDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BillWriteBack> _logger;
        public BillWriteBack(IDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<BillWriteBack>();
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
            dynamic billData = _dbContext.Get(tableName, fid, true);
            //业务实体 只有设置了映射才会有            
            dynamic bizData = null;
            dynamic fapBizData = null;
            string bizTableName = string.Empty;
            string effectiveTime = billData.EffectiveTime;
            if (effectiveTime.IsPresent())
            {
                DateTime effTime;
                if (DateTime.TryParse(effectiveTime, out effTime))
                {
                    //生效时间大于当前时间
                    if (effTime > DateTime.Now)
                    {
                        return;
                    }
                }
            }
            //设置生效状态
            billData.EffectiveState = "1";
            IDictionary<string, object> dicBillData = billData as IDictionary<string, object>;
            var fapBillData = dicBillData.ToFapDynamicObject(_dbContext.Columns(tableName));
            _dbContext.UpdateDynamicData(fapBillData);
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
                                fdo.SetValue(bizCol, fapBillData.Get(billCol));
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
                        object fidValue = fapBillData.Get(association);

                        if (rule.FieldMapping.IsPresent())
                        {
                            JArray mappings = JArray.Parse(rule.FieldMapping);
                            if (mappings != null && mappings.Any())
                            {
                                //根据关联值获取业务值
                                bizData = _dbContext.Get(bizTableName, fidValue.ToString());
                                if (bizData != null)
                                {
                                    IDictionary<string, object> dicBizData = bizData as IDictionary<string, object>;
                                    fapBizData = dicBillData.ToFapDynamicObject(_dbContext.Columns(bizTableName));
                                    //更新业务对象
                                    foreach (JObject map in mappings)
                                    {
                                        //id存放字段的映射,billtablename.colname,biztablename.colname
                                        string id = map.GetValue("id").ToString();
                                        string[] arryMap = id.Split(',');
                                        string billCol = arryMap[0].Split('.')[1];

                                        string bizCol = arryMap[1].Split('.')[1];
                                        fapBizData.Add(bizCol, fapBillData.Get(billCol).ToString());
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
                                        fdo.SetValue(bizCol, fapBillData.Get(billCol));
                                    }
                                    fdo.SetValue(FapDbConstants.FAPCOLUMN_FIELD_Fid, UUIDUtils.Fid);

                                    //insert
                                    _dbContext.InsertDynamicData(fdo);
                                    bizData = _dbContext.Get(bizTableName, fdo.Get(FapDbConstants.FAPCOLUMN_FIELD_Fid).ToString());
                                    IDictionary<string, object> dicBizData = bizData as IDictionary<string, object>;
                                    fapBizData = dicBillData.ToFapDynamicObject(_dbContext.Columns(bizTableName));
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
                        string sql = GetFillSql(rule.CustomSql, billData, bizData, tableName, bizTableName);

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
        private string GetFillSql(string customSql, dynamic billData, dynamic bizData, string billTable, string bizTable)
        {
            var billDataJson = billData as IDictionary<string, object>;
            IDictionary<string, object> bizDataJson = null;
            if (bizData != null)
            {
                bizDataJson = bizData as IDictionary<string, object>;
            }

            string _customSql = customSql.ToLower();
            foreach (var item in billDataJson)
            {
                string field = "${" + billTable + "_" + item.Key.ToStringOrEmpty() + "}".ToLower();
                if (_customSql.Contains(field))
                {
                    _customSql = _customSql.Replace(field, item.Value.ToStringOrEmpty());
                }
            }

            if (bizDataJson != null)
            {
                foreach (var item in bizDataJson)
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
        private void NotifyPayroll(CfgBillWriteBackRule rule, dynamic billData, dynamic bizObject)
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
                string empUid = bizObject.Fid;
                Employee empInfo = _dbContext.Get<Employee>(empUid);
                List<FapDynamicObject> payToDos = new List<FapDynamicObject>();
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
                        dynamic payToDo = BuilderPayToDo(rule, billData, empInfo, caseUid);
                        payToDos.Add(payToDo);
                    }
                }
                if (payToDos.Count > 0)
                {
                    _dbContext.InsertDynamicDataBatch(payToDos);
                }
                else
                {
                    dynamic payToDo = BuilderPayToDo(rule, billData, empInfo, "");
                    _dbContext.InsertDynamicData(payToDo);
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
        private void NotifyInsurance(CfgBillWriteBackRule rule, dynamic billData, dynamic bizObject)
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
                string empUid = bizObject.Fid;
                Employee empInfo = _dbContext.Get<Employee>(empUid);
                List<FapDynamicObject> insToDos = new List<FapDynamicObject>();
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
                        dynamic insToDo = BuilderInsToDo(rule, billData, empInfo, caseUid);
                        insToDos.Add(insToDo);
                    }
                }
                if (insToDos.Count > 0)
                {
                    _dbContext.InsertDynamicDataBatch(insToDos);
                }
                else
                {
                    dynamic insToDo = BuilderInsToDo(rule, billData, empInfo, "");
                    _dbContext.InsertDynamicData(insToDo);
                }
            }


        }

        private dynamic BuilderInsToDo(CfgBillWriteBackRule rule, dynamic billData, Employee empInfo, string caseUid)
        {
            //影响此保险组
            dynamic insToDo = new FapDynamicObject(_dbContext.Columns("InsToDo"));
            insToDo.EmpUid = empInfo.Fid;
            insToDo.EmpCode = empInfo.EmpCode;
            insToDo.DeptUid = empInfo.DeptUid;
            insToDo.DeptCode = empInfo.DeptCode;
            insToDo.CaseUid = caseUid;//受影响保险组
            insToDo.TableUid = rule.DocEntityUid;//单据实体
            insToDo.BizDate = DateTimeUtils.CurrentDateStr;//业务日期--当前时间
            insToDo.OperEmpUid = billData.BillEmpUid;//变动处理人--制单人
            insToDo.OperFlag = 0;//变动应用 默认为0
            insToDo.TransID = billData.Id;//单据ID
            return insToDo;
        }
        private dynamic BuilderPayToDo(CfgBillWriteBackRule rule, dynamic billData, Employee empInfo, string caseUid)
        {
            //影响此薪资套
            dynamic payToDo = new FapDynamicObject(_dbContext.Columns("PayToDo"));
            payToDo.EmpUid = empInfo.Fid;
            payToDo.EmpCode = empInfo.EmpCode;
            payToDo.DeptUid = empInfo.DeptUid;
            payToDo.DeptCode = empInfo.DeptCode;
            payToDo.CaseUid = caseUid;//受影响薪资套
            payToDo.TableUid = rule.DocEntityUid;//单据实体
            payToDo.BizDate = DateTimeUtils.CurrentDateStr;//业务日期--当前时间
            payToDo.OperEmpUid = billData.BillEmpUid;//变动处理人--制单人
            payToDo.OperFlag = 0;//变动应用 默认为0
            payToDo.TransID = billData.Id;//单据ID
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
