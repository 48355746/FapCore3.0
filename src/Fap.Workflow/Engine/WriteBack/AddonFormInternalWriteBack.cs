using Dapper;
using Fap.Core.Configuration;
using Fap.Core.DataAccess.BaseAccess;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Utility;
using Fap.Workflow.Model;

namespace Fap.Workflow.Engine.WriteBack
{
    /// <summary>
    /// 外挂表单的内置单据的回写
    /// </summary>
    public class AddonFormInternalWriteBack : WriteBackRuleBase
    {
        public AddonFormInternalWriteBack(string processId, string taskId, IDbSession dbSession, IFapConfigService config)
        {
            this.ProcessId = processId;
            this.DbSession = dbSession;
            this.TaskId = taskId;
            this._config = config;
        }

        /// <summary>
        /// 实现回写
        /// </summary>
        public override void WriteBackToBusiness()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ProcessId", ProcessId);
            WfFormInstance form = DbSession.QueryFirstOrDefaultWhere<WfFormInstance>("ProcessId=@ProcessId", parameters);
            if (form == null) return;

            //回写到业务表
            bool result = _config.ExecBillWriteBack(form.AppInstanceTableName, form.AppInstanceId, DbSession);
            if (result)
            {
                //设置流程实例的回写状态
                WfProcessInstance process = this.DbSession.Get<WfProcessInstance>(this.ProcessId);
                if (process != null)
                {
                    process.WriteBackState = 1;
                    process.WriteBackTime = PublicUtils.CurrentDateTimeStr;
                    this.DbSession.Update<WfProcessInstance>(process);
                }
            }
        }

        /// <summary>
        /// 流程过程中更新单据
        /// </summary>
        public override void UpdateBill(EnumProcessState state)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ProcessId", ProcessId);
            WfFormInstance form = DbSession.QueryFirstOrDefaultWhere<WfFormInstance>("ProcessId=@ProcessId", parameters);
            if (form == null) return;
            //更新到单据表
            if (state == EnumProcessState.Startup)
            {
                base.UpdateBillDataWhenStartup(form.AppInstanceTableName, form.AppInstanceId);
            }
            else if (state == EnumProcessState.Processing)
            {
                base.UpdateBillDataWhenProcessing(form.AppInstanceTableName, form.AppInstanceId);
            }
            else if (state == EnumProcessState.Completed)
            {
                base.UpdateBillDataWhenComplete(form.AppInstanceTableName, form.AppInstanceId);
            }
        }

        /// <summary>
        /// 流程失败时处理
        /// </summary>
        public override void HandleWhenError()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ProcessId", ProcessId);
            WfFormInstance form = DbSession.QueryFirstOrDefaultWhere<WfFormInstance>("ProcessId=@ProcessId", parameters);
            if (form == null) return;

            //删除该表单
            DbSession.Delete<WfFormInstance>(form);
        }


    }
}
