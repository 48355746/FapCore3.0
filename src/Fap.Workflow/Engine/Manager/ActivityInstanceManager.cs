using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.Utility;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Enums;
using Fap.Workflow.Engine.Xpdl;
using Fap.Workflow.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Workflow.Engine.Manager
{
    internal class ActivityInstanceManager : ManagerBase
    {
        private readonly ILogger<ActivityInstanceManager> _logger;
        public ActivityInstanceManager(IDbContext dataAccessor,ILoggerFactory loggerFactory) : base(dataAccessor, loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ActivityInstanceManager>();
        }

        /// <summary>
        /// 根据ID获取活动实例
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        internal WfActivityInstance GetByFid(string activityInsd)
        {
            return _dataAccessor.Get<WfActivityInstance>(activityInsd);
        }

        
      
        /// <summary>
        /// 判断是否是某个用户的办理任务
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        internal bool IsMineTask(WfActivityInstance activity, string userId)
        {
            bool isMine = activity.AssignedToUserIds.Contains(userId);
            return isMine;
        }

        /// <summary>
        /// 获取流程的第一个活动节点（开始节点）
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        internal WfActivityInstance GetStartupActivity(string processId)
        {
            
            //string sql = @"select top 1 * from WfActivityInstance where ProcessId=@ProcessId and ActivityType='"+WfActivityInstanceType.StartNode+"' order by Id desc";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ProcessId", processId);
            return _dataAccessor.QueryFirstOrDefault<WfActivityInstance>("select * from WfActivityInstance where   ProcessId=@ProcessId and ActivityType='" + WfActivityType.StartNode + "' order by Id desc", parameters,false);
        }

        
        #region 创建活动实例
        /// <summary>
        /// 创建活动实例的对象
        /// </summary>
        /// <param name="activityEntity"></param>
        /// <param name="processInstance"></param>
        /// <returns></returns>
        internal WfActivityInstance CreateActivityInstanceObject(WfProcessInstance processIns,
            ActivityEntity activityEntity,
            WfAppRunner runner)
        {
            WfActivityInstance activity = new WfActivityInstance();
            activity.Fid = UUIDUtils.Fid;
            activity.ActivityName = activityEntity.ActivityName;
            activity.ProcessUid = processIns.ProcessUid;
            activity.ActivityType = activityEntity.ActivityType.ToString();
            activity.ApproverMethod = activityEntity.ApproverMethod.ToString();
            activity.DirectionType = activityEntity.GatewayDirectionType.ToString();
            activity.ProcessInsUid = processIns.Fid;
            activity.NodeId = activityEntity.ActivityID;
            activity.NodeName = activityEntity.ActivityCode;
            activity.AppName = runner.AppName;
            activity.BillUid = runner.BillUid;
            activity.TokensRequired = 1;
            activity.TokensHad = 1;
            activity.AssignedToUserIds = runner.UserId;
            activity.AssignedToUserNames = runner.UserName;
            activity.ActivityState = WfActivityInstanceState.Ready;
            activity.CanRenewInstance = 0;
            activity.BusinessUid = runner.BusinessUid;
            activity.AppEmpUid = processIns.AppEmpUid;
            activity.AppStartTime = processIns.StartTime;
            activity.ProcessState = processIns.ProcessState;
            activity.StartTime = DateTimeUtils.CurrentDateTimeStr;
            if (activityEntity.BillTemplate.IsMissing())
            {
                activity.FormTemplate = processIns.FormTemplateUid;
            }
            else
            {
                activity.FormTemplate = activityEntity.BillTemplate;
            }
            if (activityEntity.FieldItems!=null&&activityEntity.FieldItems.Any())
            {
                activity.FormPower = activityEntity.FieldItems.ToJson();
            }
            activity.PassingRate = activityEntity.PassRate;
            activity.AutoExecTime =DateTimeUtils.DateTimeFormat(DateTime.Now.AddMinutes(activityEntity.Expiration));
            activity.MessageSetting = $"{{'approver':{(activityEntity.NoticeApprover ? 1 : 0)},'applicant':{(activityEntity.NoticeApplicant ? 1 : 0)},'mail':{(activityEntity.IsMail ? 1 : 0)},'message':{(activityEntity.IsMessage ? 1 : 0)}}}";
            return activity;
        }
        /// <summary>
        /// 查询分支实例的个数
        /// </summary>
        /// <param name="splitActivityNodeId"></param>
        /// <param name="processInstanceID"></param>
        /// <returns></returns>
        internal int GetInstanceGatewayCount(string splitActivityId, string processInstanceUid)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("ProcessInsUid", processInstanceUid);
            param.Add("SourceActivityNodeId", splitActivityId);
            return _dataAccessor.Count("WfTransitionInstance", "ProcessInsUid=@ProcessInsUid and SourceActivityNodeId=@SourceActivityNodeId", param);

        }
        /// <summary>
        /// 根据主节点复制子节点
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        internal WfActivityInstance CreateActivityInstanceObject(WfActivityInstance main)
        {
            WfActivityInstance instance = new WfActivityInstance();
            instance.Fid = UUIDUtils.Fid;
            instance.ActivityName = main.ActivityName;
            instance.ActivityType = main.ActivityType;
            instance.DirectionType = main.DirectionType;
            instance.ProcessInsUid = main.ProcessInsUid;
            instance.AppName = main.AppName;
            instance.BillUid = main.BillUid;
            instance.ProcessState = main.ProcessState;
            instance.AppEmpUid = main.AppEmpUid;
            instance.TokensRequired = 1;
            instance.TokensHad = 1;
            instance.AssignedToUserIds = main.AssignedToUserIds;
            instance.AssignedToUserNames = main.AssignedToUserNames;
            instance.ActivityState = main.ActivityState;
            instance.CanRenewInstance = 0;
            instance.StartTime = DateTimeUtils.CurrentDateTimeStr;
            instance.NodeId = main.NodeId;
            instance.NodeName = main.NodeName;
            instance.BusinessUid = main.BusinessUid;
            instance.AppStartTime = main.AppStartTime;

            return instance;
        }

        /// <summary>
        /// 创建活动实例的对象
        /// </summary>
        /// <param name="activityNode"></param>
        /// <param name="processInstance"></param>
        /// <returns></returns>
        internal WfActivityInstance CreateBackwardActivityInstanceObject(WfProcessInstance process,
            ActivityEntity activityNode,
            WfActivityInstance activity,
            BackwardTypeEnum backwardType,
            string backSrcActivityId,
            WfAppRunner runner)
        {
            WfActivityInstance instance = new WfActivityInstance();
            instance.Fid = UUIDUtils.Fid;
            instance.ActivityName = activityNode.ActivityName;
            instance.ActivityType = activityNode.ActivityType.ToString();
            instance.DirectionType = activityNode.GatewayDirectionType.ToString();
            instance.ProcessInsUid = process.Fid;
            instance.NodeId = activityNode.ActivityID;
            instance.NodeName = activityNode.ActivityName;
            instance.AppName = activity.AppName;
            instance.BillUid = activity.BillUid;
            instance.BackwardType = backwardType.ToString();
            instance.BackSrcActivityId = backSrcActivityId;
            instance.TokensRequired = 1;
            instance.TokensHad = 1;
            instance.AssignedToUserIds = runner.UserId;
            instance.AssignedToUserNames = runner.UserName;
            instance.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
            instance.ActivityState = WfActivityInstanceState.Ready;
            instance.CanRenewInstance = 0;
            activity.BusinessUid = process.BusinessUid;
            instance.AppEmpUid = process.AppEmpUid;
            instance.StartTime = process.StartTime;
            instance.ProcessState = process.ProcessState;
            instance.StartTime = DateTimeUtils.CurrentDateTimeStr;
            if(activityNode.FieldItems.Any())
            {
                instance.FormPower = activityNode.FieldItems.ToJson(); 
            }

            return instance;
        }

        /// <summary>
        /// 更新活动节点的Token数目
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="runner"></param>
        /// <param name="wfLinqDataContext"></param>
        internal void IncreaseTokensHad(string activityInsId, WfAppRunner runner)
        {
            WfActivityInstance activityInstance = GetByFid(activityInsId);
            activityInstance.TokensHad += 1;
            Update(activityInstance);
        }

        #endregion

        /// <summary>
        /// 检查活动实例是否可以完成
        /// </summary>
        /// <param name="activityInsUid"></param>
        /// <returns></returns>
        internal bool IsComplete(string activityInsUid)
        {
            WfActivityInstance wfActivityInstance = _dataAccessor.Get<WfActivityInstance>(activityInsUid,false);
            string activityType = wfActivityInstance.ActivityType;
            string method = wfActivityInstance.ApproverMethod;
            //检查是否当前节点已经审批完成，否则不查找下一级节点
            if ((activityType == WfActivityType.TaskNode || activityType == WfActivityType.TimerNode) && method == ApproverMethodEnum.Queue.ToString())
            {
                //队列中存在等待状态的 即为未完成
                DynamicParameters param = new DynamicParameters();
                param.Add("ActivityInsUid", activityInsUid);
                int count = _dataAccessor.Count("WfTask", $"ActivityInsUid=@ActivityInsUid and TaskState='{WfTaskState.Waiting}'", param);
                return count > 0 ? false : true;
            }
            else if (activityType == WfActivityType.SignNode)
            {
                //会签，检查通过率
                DynamicParameters param = new DynamicParameters();
                param.Add("ActivityInsUid", activityInsUid);
                int totalCount = _dataAccessor.Count("WfTask", $"ActivityInsUid=@ActivityInsUid", param);
                int completeCount = _dataAccessor.Count("WfTask", $"ActivityInsUid=@ActivityInsUid and TaskState='{WfTaskState.Completed}'", param);
                if (completeCount / totalCount * 100 >= wfActivityInstance.PassingRate)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 活动实例完成
        /// </summary>
        /// <param name="activityInsUid"></param>
        /// <param name="runner"></param>
        internal void Complete(string activityInsUid,
            WfAppRunner runner, bool isLogic = true)
        {
            WfActivityInstance activityInstance = _dataAccessor.Get<WfActivityInstance>(activityInsUid,false);
            activityInstance.ActivityState = WfActivityInstanceState.Completed;
            activityInstance.ApproveState = runner.ApproveState;
            activityInstance.EndTime = DateTimeUtils.CurrentDateTimeStr;

            _dataAccessor.Update<WfActivityInstance>(activityInstance);
            #region 活动节点完成逻辑

            #endregion
        }

        /// <summary>
        /// 获取运行状态的活动实例
        /// </summary>
        /// <param name="activityGUID"></param>
        /// <returns></returns>
        internal WfActivityInstance GetActivityInstanceRunning(string processInsd, string nodeId)
        {
            return GetActivityByState(processInsd, nodeId, WfActivityInstanceState.Running);
        }
   
        /// <summary>
        /// 根据状态获取活动实例
        /// </summary>
        /// <param name="processInstanceID"></param>
        /// <param name="activityGUID"></param>
        /// <param name="activityState"></param>
        /// <returns></returns>
        internal WfActivityInstance GetActivityByState(string processInsUid,
            string nodeId,
            string activityState)
        {
            string sql = "ProcessInsUid = @ProcessInsUid AND NodeId = @NodeId AND ActivityState = @ActivityState";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ProcessInsUid", processInsUid);
            parameters.Add("NodeId", nodeId);
            parameters.Add("ActivityState", activityState);

            return _dataAccessor.QueryFirstOrDefaultWhere<WfActivityInstance>(sql, parameters,false);
        }
        /// <summary>
        /// 获取活动的任务实例
        /// </summary>
        /// <param name="processInsUid"></param>
        /// <returns></returns>
        internal IEnumerable<WfActivityInstance> GetRunningActivityInstanceList(string processInsUid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ProcessInsUid", processInsUid);
            return  _dataAccessor.QueryWhere<WfActivityInstance>($"ProcessInsUid=@ProcessInsUid and ActivityState in('{WfActivityInstanceState.Ready}','{WfActivityInstanceState.Running}')", parameters, false);
        }

        /// <summary>
        /// 撤销活动实例
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="runner"></param>
        internal void Withdraw(string activityId, WfAppRunner runner)
        {
            //SetActivityState(activityId, WfActivityInstanceState.Withdrawed, runner.UserId, runner.UserName);
            string currenDateTime = DateTimeUtils.CurrentDateTimeStr;
            var activity = GetByFid(activityId);
            activity.ActivityState = WfActivityInstanceState.Withdrawed;
            activity.ApproveState = WfApproveState.Disagree;
            activity.UpdateBy = runner.UserId;
            activity.UpdateName = runner.UserName;
            activity.UpdateDate = currenDateTime;
           
            activity.EndTime = currenDateTime;
            Update(activity);
        }

        /// <summary>
        /// 退回活动实例
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="runner"></param>
        internal void SendBack(string activityId, WfAppRunner runner)
        {
            string currenDateTime = DateTimeUtils.CurrentDateTimeStr;
            var activity = GetByFid(activityId);
            activity.ActivityState = WfActivityInstanceState.Sendbacked;
            activity.ApproveState = WfApproveState.Disagree;
            
           
            activity.EndTime = currenDateTime;
            Update(activity);
        }

        /// <summary>
        /// 驳回活动实例
        /// </summary>
        /// <param name="activityInsd"></param>
        internal void Revoke(string activityInsd,string processInsUid)
        {
            string currenDateTime = DateTimeUtils.CurrentDateTimeStr;
            var activity = GetByFid(activityInsd);
            activity.ActivityState = WfActivityInstanceState.Revoked;
            activity.ApproveState = WfApproveState.Disagree;           
            activity.EndTime = currenDateTime;
            Update(activity);
            //其他任务实例设置挂起状态
            var otherActivityIns= _dataAccessor.QueryWhere<WfActivityInstance>($" ProcessInsUid=@ProcessInsUid and fid<>@ActivityInsUid and ActivityState in('{WfActivityInstanceState.Ready}','{WfActivityInstanceState.Running}')", new DynamicParameters(new { ProcessInsUid = processInsUid, ActivityInsUid = activityInsd }), false);
            if(otherActivityIns!=null&&otherActivityIns.Any())
            {
                foreach (var act in otherActivityIns)
                {
                    act.ApproveState = WfActivityInstanceState.Suspended;
                    act.EndTime = currenDateTime;
                }
                _dataAccessor.UpdateBatch<WfActivityInstance>(otherActivityIns);
            }
        }

        /// <summary>
        /// 设置活动实例状态
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="nodeState"></param>
        /// <param name="runner"></param>
        private void SetActivityState(string activityId,
            string nodeState,
            string userId,
            string userName)
        {
            string currenDateTime = DateTimeUtils.CurrentDateTimeStr;
            var activity = GetByFid(activityId);
            activity.ActivityState = nodeState;
           
            activity.EndTime = currenDateTime;
            Update(activity);
        }
        

        #region 活动实例记录维护
        internal WfActivityInstance Insert(WfActivityInstance entity)
        {
            _dataAccessor.Insert<WfActivityInstance>(entity);
            return entity;
        }

        internal void Update(WfActivityInstance entity)
        {
            _dataAccessor.Update<WfActivityInstance>(entity);
        }

        /// <summary>
        /// 删除活动实例
        /// </summary>
        /// <param name="activityInstanceID"></param>
        /// <param name="wfLinqDataContext"></param>
        internal void Delete(string activityInstanceID)
        {
            _dataAccessor.Delete<WfActivityInstance>(activityInstanceID);
        }


        #endregion
    }
}
