using Dapper;
using Fap.Core.Configuration;
using Fap.Core.DataAccess.BaseAccess;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Utility;
using Fap.Model.Constants;
using Fap.Model.MetaData;
using Fap.Workflow.Model;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Fap.Workflow.Engine.WriteBack
{
    /// <summary>
    /// 流程回写规则的父类
    /// </summary>
    public abstract class WriteBackRuleBase : IWriteBackRule
    {
        protected IDbSession DbSession { get; set; }
        /// <summary>
        /// 流程实例ID
        /// </summary>
        protected string ProcessId { get; set; }
        /// <summary>
        /// 任务实例ID
        /// </summary>
        protected string TaskId { get; set; }

        protected IFapConfigService _config;

        public abstract void WriteBackToBusiness();
        public abstract void UpdateBill(EnumProcessState state);
        public abstract void HandleWhenError();

        /// <summary>
        /// 更新单据表(发起时)
        /// </summary>
        /// <param name="billTableName"></param>
        /// <param name="billId"></param>
        internal void UpdateBillDataWhenStartup(string billTableName, string billId)
        {
            FapTable table = DbSession.QueryFirstOrDefault<FapTable>($"select * from FapTable where TableName='{billTableName}'");
            if (table == null || string.IsNullOrWhiteSpace(table.TableFeature)) return;

            if (("," + table.TableFeature + ",").IndexOf(",BillFeature,") >= 0)
            {
                //单据数据
                dynamic billData = DbSession.Get(billTableName, billId);
                if (billData == null) return;
                WfProcessInstance process = DbSession.Get<WfProcessInstance>(ProcessId);
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("ProcessId", ProcessId);
                IEnumerable<WfTask> tasks = DbSession.QueryWhere<WfTask>("ProcessId=@ProcessId", parameters);
                if (tasks != null && tasks.Count() > 0)
                {
                    billData.CurrApprover = tasks.First().ExecutorEmpUid;
                }
                //更新单据的字段数据
                billData.SubmitTime = PublicUtils.GetSysDateTimeStr();
                billData.BillStatus = BillStatus.PROCESSING;

                DbSession.UpdateDynamicData(billData);
            }
        }

        /// <summary>
        /// 更新单据表(处理中)
        /// </summary>
        /// <param name="billTableName"></param>
        /// <param name="billId"></param>
        internal void UpdateBillDataWhenProcessing(string billTableName, string billId)
        {
            FapTable table = DbSession.Table(billTableName);
            if (table == null || string.IsNullOrWhiteSpace(table.TableFeature)) return;

            if (("," + table.TableFeature + ",").IndexOf(",BillFeature,") >= 0)
            {
                //单据数据
                dynamic billData = DbSession.Get(billTableName, billId);
                if (billData == null) return;

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("ProcessId", ProcessId);
                //WfProcessInstance process = DbSession.QueryFirstOrDefaultEntityByWhere<WfProcessInstance>("Fid=@ProcessId", parameters);
                //WfTask task = DbSession.QueryFirstOrDefaultEntityBySql<WfTask>("select * from WfTask where ProcessId=@ProcessId order by Fid desc", parameters);
                //WfTask task = DbSession.QueryFirstOrDefaultEntityByWhere<WfTask>("ProcessId=@ProcessId",parameters,"Id desc");
                //order by  查不到事务中的数据
                //WfTaskAdvice taskAdvice = DbSession.QueryFirstOrDefaultEntityBySql<WfTaskAdvice>("select * from WfTaskAdvice where TaskId='" + task.Fid + "' order by Fid desc");
                WfProcessInstance process = DbSession.Get<WfProcessInstance>(ProcessId);
                //WfTask task = DbSession.Get<WfTask>(TaskId);
                IEnumerable<WfTask> tasks = DbSession.QueryWhere<WfTask>(" ProcessId=@ProcessId and TaskState='Handling'", parameters);

                WfTask task = tasks.FirstOrDefault();
                if(task==null)
                {
                    throw new Exception("不存在新任务");
                }
                //更新单据的字段数据
                if (process.ProcessState == WfProcessInstanceState.Running)
                {
                    billData.BillStatus = BillStatus.PROCESSING;
                    billData.CurrApprover = task.ExecutorEmpUid;
                    billData.ApprovalTime = PublicUtils.CurrentDateTimeStr; //task.ExecuteTime;
                    billData.ApprovalComments = task.Suggestion;
                    DbSession.UpdateDynamicData(billData);
                }
                else if (process.ProcessState == WfProcessInstanceState.Canceled)
                {
                    billData.BillStatus = BillStatus.CANCELED;
                    billData.CurrApprover = task.ExecutorEmpUid;
                    billData.ApprovalTime = PublicUtils.CurrentDateTimeStr;// task.ExecuteTime;
                    billData.ApprovalComments = task.Suggestion;
                    DbSession.UpdateDynamicData(billData);
                }
                else if (process.ProcessState == WfProcessInstanceState.Deleted)
                {
                    billData.BillStatus = BillStatus.CLOSED;
                    billData.CurrApprover = task.ExecutorEmpUid;
                    billData.ApprovalTime = PublicUtils.CurrentDateTimeStr;// task.ExecuteTime;
                    billData.ApprovalComments = task.Suggestion;
                    DbSession.UpdateDynamicData(billData);
                }
                else if (process.ProcessState == WfProcessInstanceState.Ended)
                {
                    billData.BillStatus = BillStatus.CLOSED;
                    billData.CurrApprover = task.ExecutorEmpUid;
                    billData.ApprovalTime = PublicUtils.CurrentDateTimeStr;// task.ExecuteTime;
                    billData.ApprovalComments = task.Suggestion;
                    DbSession.UpdateDynamicData(billData);
                }
                else if (process.ProcessState == WfProcessInstanceState.Suspended)
                {
                    billData.BillStatus = BillStatus.SUSPENDED;
                    billData.CurrApprover = task.ExecutorEmpUid;
                    billData.ApprovalTime = PublicUtils.CurrentDateTimeStr;// task.ExecuteTime;
                    billData.ApprovalComments = task.Suggestion;
                    DbSession.UpdateDynamicData(billData);
                }
                else if (process.ProcessState == WfProcessInstanceState.Withdrawed)
                {
                    billData.BillStatus = BillStatus.WITHDRAWED;
                    billData.CurrApprover = task.ExecutorEmpUid;
                    billData.ApprovalTime = PublicUtils.CurrentDateTimeStr;// task.ExecuteTime;
                    billData.ApprovalComments = task.Suggestion;
                    DbSession.UpdateDynamicData(billData);
                }
                else if (process.ProcessState == WfProcessInstanceState.Revoked)
                {
                    billData.BillStatus = BillStatus.REVOKED;
                    billData.CurrApprover = task.ExecutorEmpUid;
                    billData.ApprovalTime = PublicUtils.CurrentDateTimeStr;// task.ExecuteTime;
                    billData.ApprovalComments = task.Suggestion;
                    DbSession.UpdateDynamicData(billData);
                }
            }
        }
        /// <summary>
        /// 更新单据表(完成时)
        /// </summary>
        /// <param name="billTableName"></param>
        /// <param name="billId"></param>
        internal void UpdateBillDataWhenComplete(string billTableName, string billId)
        {
            FapTable table = DbSession.Table(billTableName);
            if (table == null || string.IsNullOrWhiteSpace(table.TableFeature)) return;

            if (("," + table.TableFeature + ",").IndexOf(",BillFeature,") >= 0)
            {
                //单据数据
                dynamic billData = DbSession.Get(billTableName, billId);
                if (billData == null) return;

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("ProcessId", ProcessId);
                //WfProcessInstance process = DbSession.QueryFirstOrDefaultEntityByWhere<WfProcessInstance>("Fid=@ProcessId", parameters);
                //WfTask task = DbSession.QueryFirstOrDefaultEntityBySql<WfTask>("select top 1 * from WfTask where ProcessId=@ProcessId order by Fid desc", parameters);
                //WfTaskAdvice taskAdvice = DbSession.QueryFirstOrDefaultEntityBySql<WfTaskAdvice>("select top 1 * from WfTaskAdvice where TaskId='" + task.Fid + "' order by Fid desc");
                WfProcessInstance process = DbSession.Get<WfProcessInstance>(ProcessId);
                WfTask task = DbSession.Get<WfTask>(TaskId);
                //更新单据的字段数据
                billData.CurrApprover = task.ExecutorEmpUid;
                billData.ApprovalTime = task.ExecuteTime;
                billData.ApprovalComments = task.Suggestion;
                if (task.ApproveState == WfApproveState.Agree)
                {
                    billData.BillStatus = BillStatus.PASSED; //同意
                }
                else if (task.ApproveState == WfApproveState.Disagree)
                {
                    billData.BillStatus = BillStatus.REJECTED; //不同意
                }

                billData.EffectiveState = 0;
                //billData.EffectiveTime = PublicUtils.GetSysDateTimeStr();

                DbSession.UpdateDynamicData(billData);

            }
        }
    }
}
