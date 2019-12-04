using System.Collections.Generic;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Xpdl;
using Fap.Workflow.Engine.Xpdl.Entity;
using Fap.Workflow.Model;
using Fap.Workflow.Engine.Entity;

namespace Fap.Workflow.Service
{
    public interface IWorkflowService
    {
        void UpgradeProcessTemplateDirectly(long id);
        bool AllowDeleteProcessTemplate(string processUids);
        void SaveProcessTemplate(string xml);
        bool AssignAgentTask(string taskId, WfAppRunner runner);
        //bool CancelProcess(string processId, string comment);
        bool DeleteProcess(string processId, string comment);
        bool EndProcess(string processId, string comment);
        ActivityEntity GetFirstActivity(string processId, string bizUid);
        IList<Participant> GetFirstActivityParticipants(string processId, string bizUid);
        //FormEntity GetFormByProcess(string processId);
        /// <summary>
        /// 判断节点是否审批完成
        /// </summary>
        /// <param name="activityInsUid"></param>
        /// <returns></returns>
        bool NodeIsComplete(string activityInsUid);
        ActivityEntity GetNextActivity(string processId, string nodeId, string bizUid);
        IList<NodeView> GetNextNodeList(string processId, string nodeId, string bizUid);
        //PageDataView<WfTask> GetPagedTasks(PageCriteria criteria);
        PerformerList GetPerformerOfNextStep(string processId, string nodeId, string bizUid);
        WfProcessInstance GetProcessInstance(string processInsId);
        //ActivityEntity GetStartActivity(string processId, string bizUid);
        IEnumerable<ActivityEntity> GetTaskActivityList(string processId, string bizUid);
        IEnumerable<TaskViewModel> GetTasks(TaskQueryEntity taskQueryEntity);
        IEnumerable<WfTask> GetTodoTaskList(string userId);
        //bool RecallProcess(string processId, string comment);
        //bool RejectProcess(string taskId, string comment);
        bool ResumeProcess(string processId, string comment);
        WfExecutedResult StartProcess(WfAppRunner starter);
        WfExecutedResult RunProcess(WfAppRunner runner);
        bool SuspendProcess(string processId, string comment);
        bool TurnTask(string taskId, WfAppRunner runner);
        string UrgeFlow(string bizUid, string bizTypeUid);
        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        WfExecutedResult WithdrawProcess(WfAppRunner runner);
        /// <summary>
        /// 回退（驳回）
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        WfExecutedResult SendBackProcess(WfAppRunner runner);
        /// <summary>
        /// 单据回写
        /// </summary>
        void BillWriteBack();
    }
}