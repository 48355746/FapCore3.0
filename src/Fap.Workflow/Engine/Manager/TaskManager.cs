using Dapper;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Xpdl.Entity;
using Fap.Workflow.Model;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Fap.Workflow.Engine.Enums;
using System.Linq;
using Fap.Workflow.Engine.Message;
using Fap.Core.DataAccess;
using Fap.Core.Utility;

namespace Fap.Workflow.Engine.Manager
{
    internal class TaskManager : ManagerBase
    {
        public TaskManager(IDbContext dataAccessor, ILoggerFactory loggerFactory) : base(dataAccessor, loggerFactory)
        {
        }

        public WfTask GetTask(string taskUid)
        {
            return _dataAccessor.Get<WfTask>(taskUid,false);
        }

      

        #region TaskManager 任务数据基本操作
        /// <summary>
        /// 插入任务数据
        /// </summary>
        /// <param name="entity">任务实体</param>
        /// <param name="wfLinqDataContext">linq上下文</param>
        internal void Insert(WfTask entity)
        {
            _dataAccessor.Insert<WfTask>(entity);
        }

        /// <summary>
        /// 插入任务数据
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="performers"></param>
        /// <param name="wfLinqDataContext"></param>
        internal void Insert(WfActivityInstance activity,
            IList<Performer> performers,
            string taskState = "Handling",
            bool isSignTask = false)
        {
            int i = 0;

            foreach (Performer performer in performers)
            {
                i++;
                Insert(activity, performer, i, taskState, isSignTask);
            }

        }

        /// <summary>
        /// 插入任务数据
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="performer"></param>
        /// <param name="runner"></param>
        internal void Insert(WfActivityInstance activity,
            Performer performer,
            int sort,
            string taskState = "Handling",
            bool isSignTask = false)
        {
            Insert(activity, performer.UserId, performer.UserName,
                sort, taskState, isSignTask);
        }

        /// <summary>
        /// 插入任务数据(创建任务)
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="performerID"></param>
        /// <param name="performerName"></param>
        /// <param name="runnerID"></param>
        /// <param name="runnerName"></param>
        private void Insert(WfActivityInstance activityIns,
            string performerID,
            string performerName,
            int sort,
            string taskState = "Handling",
            bool isSignTask = false)
        {

            WfTask task = new WfTask();
            task.AppName = activityIns.AppName;
            task.BillUid = activityIns.BillUid;
            task.TaskName = string.Format("{0}-{1}", activityIns.AppName, activityIns.NodeName);
            task.ActivityInsUid = activityIns.Fid;
            task.ProcessUid = activityIns.ProcessUid;
            task.ProcessInsUid = activityIns.ProcessInsUid;
            task.NodeId = activityIns.NodeId;
            task.NodeName = activityIns.NodeName;
            task.TaskType = WfTaskType.Manual;
            task.TaskActivityType = activityIns.ActivityType;
            task.ExecutorEmpUid = performerID;
            task.ExecutorEmpName = performerName;
            if((activityIns.ActivityType==WfActivityType.TaskNode||activityIns.ActivityType==WfActivityType.TimerNode)&& activityIns.ApproverMethod==ApproverMethodEnum.Queue.ToString()&&sort>1)
            {
                //当节点为普通节点审批方式为顺序完成的时候，第二个审批人需要等待
                taskState = WfTaskState.Waiting;
            }
            task.TaskState = taskState; //1-办理状态
            task.TaskStartTime =DateTimeUtils.CurrentDateTimeStr;
            task.ApproverSort = sort;
            task.RecordState = 1;
            task.BusinessUid = activityIns.BusinessUid;
            //task.BizName = activityInstance.BizName;
            task.AppEmpUid = activityIns.AppEmpUid;
            task.AppStartTime = activityIns.AppStartTime;
            task.ProcessState = activityIns.ProcessState;
            task.IsSignTask = isSignTask ? 1 : 0;
            //插入任务数据
            Insert(task);

            if(task.TaskState==WfTaskState.Handling)
            {
                SendMessage(activityIns, task);
            }
        }

        private void SendMessage(WfActivityInstance activityIns, WfTask task)
        {
            MessageManager msgManager = new MessageManager(_dataAccessor,_loggerFactory);
            msgManager.SendMessageWhenProcessing(activityIns, task);
        }

        /// <summary>
        /// 重新生成任务(只限于会签多实例下的子节点)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="instance"></param>
        internal void Renew(WfActivityInstance sourceActivity,
            WfActivityInstance newInstance,
            string taskState = "Handling",
            bool isSignTask = false)
        {
            var performer = new Performer()
            {
                UserId = sourceActivity.AssignedToUserIds,
                UserName = sourceActivity.AssignedToUserNames
            };

            Insert(newInstance, performer, 1, taskState, isSignTask);
        }

        /// <summary>
        /// 更新任务数据
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="wfLinqDataContext"></param>
        internal void Update(WfTask entity)
        {
            _dataAccessor.Update<WfTask>(entity);
        }

        /// <summary>
        /// 读取任务，设置任务为待处理状态
        /// </summary>
        /// <param name="taskID"></param>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        internal void SetNextTaskHandling(string activityInsUid,int seq)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("ActivityInsUid", activityInsUid);
            param.Add("Seq", seq);
            WfTask wfTask = _dataAccessor.QueryFirstOrDefaultWhere<WfTask>("ActivityInsUid=@ActivityInsUid and ApproverSort=@Seq",param,false);
            if(wfTask!=null)
            {
                wfTask.TaskState = WfTaskState.Handling;
                _dataAccessor.Update<WfTask>(wfTask);
                var activityIns= _dataAccessor.Get<WfActivityInstance>(activityInsUid);
                SendMessage(activityIns, wfTask);
            }

        }

        /// <summary>
        /// 设置任务状态
        /// </summary>
        /// <param name="task"></param>
        /// <param name="logonUser"></param>
        /// <param name="taskState"></param>
        internal void SetTaskState(WfTask task,
            string userID,
            string userName,
            string taskState)
        {
            task.TaskState = taskState;
            task.UpdateBy = userID;
            task.UpdateName = userName;
            task.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
            Update(task);
        }

        internal void SetTaskState(string taskId,
            string userID,
            string userName,
            string taskState)
        {
            WfTask task = _dataAccessor.Get<WfTask>(taskId,false);
            if (task != null)
            {
                task.TaskState = taskState;
                if (task.TaskState == WfTaskState.Rejected
                    || task.TaskState == WfTaskState.Revoked
                    || task.TaskState == WfTaskState.Backed)
                {
                    task.ApproveState = WfApproveState.Disagree;
                }
                task.UpdateBy = userID;
                task.UpdateName = userName;
                task.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
                Update(task);
            }
        }

        internal void SetTaskState(string taskId,
            WfAppRunner runner,
            string taskState)
        {
            WfTask task = _dataAccessor.Get<WfTask>(taskId,false);
            if (task != null)
            {
                task.TaskState = taskState;
                if (task.TaskState == WfTaskState.Rejected
                    || task.TaskState == WfTaskState.Revoked
                    || task.TaskState == WfTaskState.Backed)
                {
                    task.ApproveState = WfApproveState.Disagree;
                }
                task.UpdateBy = runner.UserId;
                task.UpdateName = runner.UserName;
                task.Suggestion = runner.Comment;
                task.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
                Update(task);
            }
        }

        /// <summary>
        /// 设置活动下的所有任务的状态
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="taskState"></param>
        internal void SetTaskStateByActivityId(string activityId, string taskState, string approveState = null)
        {
            string currentDateTime = DateTimeUtils.CurrentDateTimeStr;
            IEnumerable<WfTask> tasks = _dataAccessor.QueryWhere<WfTask>("ActivityId='" + activityId + "'",null,false);
            foreach (var task in tasks)
            {
                task.TaskState = taskState;
                task.ExecuteTime = currentDateTime;
                task.TaskEndTime = currentDateTime;
                if (!string.IsNullOrWhiteSpace(approveState))
                {
                    task.ApproveState = WfApproveState.Disagree;
                }
            }
            _dataAccessor.UpdateBatch<WfTask>(tasks);
        }

        /// <summary>
        /// 设置任务完成
        /// </summary>
        /// <param name="taskID">任务ID</param>
        /// <param name="runner"></param>
        /// <param name="wfLinqDataContext"></param>
        internal void Complete(string taskID,
            WfAppRunner runner)
        {
            //完成任务
            WfTask task = GetTask(taskID);
            task.TaskState = WfTaskState.Completed;
            task.TaskEndTime = DateTimeUtils.CurrentDateTimeStr;
            task.ExecuteTime = DateTimeUtils.CurrentDateTimeStr;
            task.ApproveState = runner.ApproveState;
            task.Suggestion = runner.Comment;
            if(task.ExecutorEmpUid!=runner.UserId)
            {
                //代理人审批
                task.AgentEmpUid = runner.UserId;
                task.AgentEmpName = runner.UserName;
            }
            Update(task);

            //回写单据审批人
            WriteBackBill(task);

        }

        /// <summary>
        /// 创建新的委托任务
        /// </summary>
        /// <param name="entity"></param>
        //internal bool Entrust(TaskEntrustedEntity entity)
        //{
        //    var isOk = false;
        //var session = SessionFactory.CreateSession();
        //try
        //{
        //    //获取活动实例信息
        //    session.BeginTrans();

        //    var am = new ActivityInstanceManager();
        //    var activityInstance = am.GetByTask(entity.ID, session);

        //    if (activityInstance.ActivityState != (short)ActivityStateEnum.Ready
        //        && activityInstance.ActivityState != (short)ActivityStateEnum.Running)
        //    {
        //        throw new WorkflowException("没有可以委托的任务，因为活动实例的状态不在运行状态！");
        //    }

        //    //更新AssignedToUsers 信息
        //    activityInstance.AssignedToUserIDs = activityInstance.AssignedToUserIDs + "," + entity.EntrustToUserID;
        //    activityInstance.AssignedToUserNames = activityInstance.AssignedToUserNames + "," + entity.EntrustToUserName;
        //    am.Update(activityInstance, session);

        //    //插入委托任务
        //    Insert(activityInstance, entity.EntrustToUserID, entity.EntrustToUserName,
        //        entity.RunnerID, entity.RunnerName, session);

        //    session.Commit();

        //    isOk = true;
        //}
        //catch (System.Exception e)
        //{
        //    session.Rollback();
        //    throw new WorkflowException("任务委托失败，请查看异常信息！", e);
        //}
        //finally
        //{
        //    session.Dispose();
        //}
        //    return isOk;
        //}

        /// <summary>
        /// 任务删除
        /// </summary>
        /// <param name="taskID">任务ID</param>
        internal void Delete(string taskID)
        {
            _dataAccessor.Delete<WfTask>(taskID);
        }
        /// <summary>
        /// 任务设置撤销状态
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="runner"></param>
        internal void Withdraw(string taskId, WfAppRunner runner)
        {
            WfTask task = _dataAccessor.Get<WfTask>(taskId,false);
            if (task != null)
            {
                task.TaskState = WfTaskState.Withdrawed;
                task.ApproveState = WfApproveState.None;
                task.ExecuteTime = DateTimeUtils.CurrentDateTimeStr;
                task.UpdateBy = runner.UserId;
                task.UpdateName = runner.UserName;
                task.Suggestion = runner.Comment;
                task.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
                Update(task);
            }
        }
        /// <summary>
        /// 任务设置否决状态
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="runner"></param>
        internal void Rejected(string taskId, WfAppRunner runner)
        {
            WfTask task = _dataAccessor.Get<WfTask>(taskId,false);
            if (task != null)
            {
                task.TaskState = WfTaskState.Rejected;
                task.ApproveState = WfApproveState.Disagree;
                task.ExecuteTime = DateTimeUtils.CurrentDateTimeStr;
                task.UpdateBy = runner.UserId;
                task.UpdateName = runner.UserName;
                task.Suggestion = runner.Comment;
                task.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
                Update(task);
            }
        }
        /// <summary>
        /// 任务设置终止状态
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="runner"></param>
        internal void End(string taskId, WfAppRunner runner)
        {
            WfTask task = _dataAccessor.Get<WfTask>(taskId,false);
            if (task != null)
            {
                task.TaskState = WfTaskState.Ended;
                task.ApproveState = WfApproveState.None;
                task.ExecuteTime = DateTimeUtils.CurrentDateTimeStr;
                task.UpdateBy = runner.UserId;
                task.UpdateName = runner.UserName;
                task.Suggestion = runner.Comment;
                task.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
                Update(task);
            }
        }
        /// <summary>
        /// 任务设置驳回状态
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="runner"></param>
        internal void Revoke(string taskId, WfAppRunner runner)
        {
            WfTask task = _dataAccessor.Get<WfTask>(taskId,false);
            if (task != null)
            {
                task.TaskState = WfTaskState.Revoked;
                task.ApproveState = WfApproveState.Disagree;
                task.ExecuteTime =DateTimeUtils.CurrentDateTimeStr ;
                task.TaskEndTime = DateTimeUtils.CurrentDateTimeStr;
                task.UpdateBy = runner.UserId;
                task.UpdateName = runner.UserName;
                task.Suggestion = runner.Comment;
                task.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
                Update(task);

                WriteBackBill(task);
               
            }
        }
        private void WriteBackBill(WfTask task)
        {
            //回写单据审批人
            string processInsUid = task.ProcessInsUid;
            WfProcessInstance process = _dataAccessor.Get<WfProcessInstance>(processInsUid, false);
            string billTable = process.BillTable;
            string billUid = process.BillUid;
            string sql = $"update {billTable} set CurrApprover='{ task.ExecutorEmpUid}',ApprovalTime='{task.ExecuteTime}',ApprovalComments='{task.Suggestion}' where Fid='{billUid}' ";
            _dataAccessor.Execute(sql, null);
        }
        /// <summary>
        /// 删除其他未处理的任务（会签）
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="mainActivityId"></param>
        internal void DeleteNoCompletedTask(string processId, string mainActivityId)
        {
            string sql = "select t.Fid from WfTask t where t.ActivityId in (select a.Fid from  WfActivity a where a.processId = '" + processId + "' and a.MainActivityId = '" + mainActivityId + "' and t.ActivityId=a.Fid and t.TaskState in('" + WfTaskState.Handling + "', '" + WfTaskState.Waiting + "'))";
            IEnumerable<dynamic> tasks = _dataAccessor.Query(sql,null,false);
            if (tasks == null || tasks.Count() == 0)
            {
                return;
            }
            List<string> taskIds = new List<string>();
            foreach (var item in tasks)
            {
                taskIds.Add(item.Fid);
            }
            _dataAccessor.Execute("delete from WfTask where Fid IN @Fids", new DynamicParameters(new { Fids = taskIds }));
        }

        #endregion

        #region TaskManager 获取当前用户的办理任务

        /// <summary>
        /// 获取我的任务
        /// </summary>
        /// <param name="activityInstanceID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        internal WfTask GetTaskOfMine(string activityInstanceId,
            string userId)
        {
            //processState:2 -running 流程处于运行状态
            //activityType:4 -表示“任务”类型的节点
            //activityState: 1-ready（准备）, 2-running（）运行；
            //            string sql = string.Format(@"select a.* from WfTask a, WfProcessInstance b, WfActivity c where a.ProcessId=b.Fid 
            //					and a.ActivityId=c.Fid and  a.ActivityId=@ActivityId and a.ExecutorName=@UserId 
            //					and b.ProcessState='{0}' and (c.ActivityType='TaskNode' or c.ActivityType='SignNode' or c.ActivityType='SubProcessNode') 
            //					and (c.ActivityState='{1}' or c.ActivityState='{2}')", 
            //                    WfProcessState.Running,
            //                    WfActivityState.Ready,
            //                    WfActivityState.Running);

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ActivityId", activityInstanceId);
            parameters.Add("UserId", userId);

            return _dataAccessor.QueryFirstOrDefaultWhere<WfTask>("ActivityId=@ActivityId and (ExecutorName=@UserId or AgentName=@UserId)", parameters,false);
        }

        /// <summary>
        /// 获取所有运行中的任务
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        internal IEnumerable<WfTask> GetAllRunningTasks()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("TaskState", WfTaskState.Handling);
            IEnumerable<WfTask> tasks = _dataAccessor.QueryWhere<WfTask>("TaskState=@TaskState", parameters,false);
            return tasks;
        }
        /// <summary>
        /// 检查任务是否存在审批完成情况
        /// </summary>
        /// <param name="processInsUid"></param>
        /// <returns></returns>
        internal bool ExistApproval(string processInsUid)
        {
            int c= _dataAccessor.Count("WfTask", $" TaskState='{WfTaskState.Completed}' and ProcessInsUid='{processInsUid}'",null);
            if(c>0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取所有待办任务
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        internal IEnumerable<WfTask> GetAllReadyTasks()
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("TaskState", WfTaskState.Waiting);
            IEnumerable<WfTask> tasks = _dataAccessor.QueryWhere<WfTask>("TaskState=@TaskState", parameters,false);
            return tasks;
        }

        /// <summary>
        /// 获取当前用户待办任务
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        //internal PageDataView<WfTask> GetPagedTasks(PageCriteria criteria)
        //{
        //    //PageCriteria criteria = new PageCriteria();
        //    //criteria.Condition = "ExecutorName='" + userId + "' AND TaskState='" + WfTaskState.Handling + "'";
        //    //criteria.CurrentPage = 1;
        //    //criteria.Fields = "*";
        //    //criteria.PageSize = 10;
        //    //criteria.TableName = "WfTask";

        //    PageDataView<WfTask> tasks = _dataAccessor.QueryEntityPaged<WfTask>(criteria);
        //    return tasks;
        //}
        #endregion
    }
}
