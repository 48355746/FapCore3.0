using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.Utility;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Enums;
using Fap.Workflow.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fap.Workflow.Engine.Manager
{
    internal class TransitionInstanceManager : ManagerBase
    {
        public TransitionInstanceManager(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        internal WfTransitionInstance CreateTransitionInstanceObject(WfProcessInstance processInstance,
            string transitionId,
            WfActivityInstance fromActivityInstance,
            WfActivityInstance toActivityInstance,
            TransitionTypeEnum transitionType,
            TransitionFlyingTypeEnum flyingType,
            WfAppRunner runner,
            int conditionParseResult)
        {
            var entity = new WfTransitionInstance();
            entity.Fid =UUIDUtils.Fid;
            entity.ProcessUid = processInstance.ProcessUid;
            entity.ProcessInsUid = processInstance.Fid;
            entity.TransitionNodeId = transitionId;
            entity.TransitionType = transitionType.ToString();// WfTransitionInstanceType.Forward;
            entity.JumpType = (int)flyingType;

            //构造活动节点数据
            entity.SourceActivityInsUid = fromActivityInstance.Fid;
            entity.SourceActivityNodeId = fromActivityInstance.NodeId;
            entity.SourceActivityNodeType = fromActivityInstance.ActivityType;
            entity.SourceActivityNodeName = fromActivityInstance.ActivityName;
            entity.TargetActivityInsUid = toActivityInstance.Fid;
            entity.TargetActivityNodeId = toActivityInstance.NodeId;
            entity.TargetActivityNodeType = toActivityInstance.ActivityType;
            entity.TargetActivityNodeName = toActivityInstance.ActivityName;

            entity.WalkState = conditionParseResult;
            //entity.CreatedByUserID = runner.UserID;
            //entity.CreatedByUserName = runner.UserName;
            //entity.CreatedDateTime = System.DateTime.Now;

            return entity;
        }

        internal WfTransitionInstance GetById(string transitionId)
        {
            return _dataAccessor.Get<WfTransitionInstance>(transitionId,false);
        }

        internal WfTransitionInstance GetEndTransition(string processId)
        {
            var nodeList = GetTransiton(processId, WfActivityType.EndNode).ToList();

            if (nodeList == null || nodeList.Count == 0)
            {
                throw new WorkflowException("没有流程结束的流转记录！");
            }

            return nodeList[0];
        }

        internal WfTransitionInstance GetLastTaskTransition(string processId)
        {
            var nodeList = GetTaskTransiton(processId).ToList();

            if (nodeList.Count == 0)
            {
                throw new WorkflowException("没有符合条件的最后流转任务的实例数据，请查看流程其它信息！");
            }

            return nodeList[0];
        }

        // <summary>
        /// 根据去向节点类型选择转移数据
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="appInstanceID"></param>
        /// <param name="processGUID"></param>
        /// <param name="toActivityType"></param>
        /// <returns></returns>
        internal IEnumerable<WfTransitionInstance> GetTransiton(string processId, string toActivityType)
        {
            var sql = @"select * from WfTransitionInstance a, WfActivityInstance b where a.ToActivityId = b.Fid and b.ActivityType=@ActivityType and a.ProcessId=@ProcessId
                        ORDER BY a.Id DESC";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ProcessId", processId);
            parameters.Add("ActivityType", toActivityType);
            var transitionList = _dataAccessor.Query<WfTransitionInstance>(sql, parameters,false);
            return transitionList;
        }

        internal IEnumerable<WfTransitionInstance> GetTaskTransiton(string processId)
        {
            var sql = @"select * from WfTransitionInstance a, WfActivityInstance b where a.ToActivityId = b.Fid and b.ActivityType in ('"+WfActivityType.SignNode+"', '"+WfActivityType.TaskNode+@"') and a.ProcessId=@ProcessId
                        ORDER BY a.Id DESC";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ProcessId", processId);
            var transitionList = _dataAccessor.Query<WfTransitionInstance>(sql, parameters,false);
            return transitionList;
        }

        internal void Insert(WfTransitionInstance entity)
        {
            _dataAccessor.Insert<WfTransitionInstance>(entity);
        }



        /// <summary>
        /// 删除转移实例
        /// </summary>
        /// <param name="transitionInstanceID"></param>
        /// <param name="wfLinqDataContext"></param>
        internal void Delete(string transitionInstanceID)
        {
            _dataAccessor.Delete<WfTransitionInstance>(transitionInstanceID);
        }

        internal IEnumerable<WfTransitionInstance> GetTransitionList(string processId)
        {
            var whereSql = @"SELECT * FROM WfTransitionInstance WHERE ProcessId=@ProcessId ORDER BY Fid DESC";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ProcessId", processId);
            return _dataAccessor.Query<WfTransitionInstance>(whereSql, parameters,false);
        }

        /// <summary>
        /// 读取节点的上一步节点信息
        /// </summary>
        /// <param name="runningActivity">当前节点</param>
        /// <param name="hasPassedGatewayNode">是否经由路由节点</param>
        /// <returns></returns>
        internal List<WfActivityInstance> GetPreviousActivity(WfActivityInstance runningActivity,
            bool isSendback,
            out bool hasPassedGatewayNode)
        {
            hasPassedGatewayNode = false;
            var transitionList = GetTransitionList(runningActivity.ProcessInsUid).ToList();

            string backSrcActivityInstanceId = "";
            if (isSendback == true && runningActivity.BackSrcActivityId != null)
            {
                //节点时曾经发生退回的节点
                backSrcActivityInstanceId = runningActivity.BackSrcActivityId;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(runningActivity.MainActivityId))//如果是会签节点找到主节点ID
                    backSrcActivityInstanceId = runningActivity.MainActivityId;
                else
                    backSrcActivityInstanceId = runningActivity.Fid;
            }

            var aim = new ActivityInstanceManager(_serviceProvider);
            var runningTransitionList = transitionList.Where(o => o.TargetActivityInsUid == backSrcActivityInstanceId).ToList();

            List<WfActivityInstance> previousActivityList = new List<WfActivityInstance>();
            foreach (var entity in runningTransitionList)
            {
                //如果是逻辑节点，则继续查找
                if (entity.SourceActivityNodeType == WfActivityType.GatewayNode)
                {
                    GetPreviousOfGatewayActivity(transitionList, entity.SourceActivityNodeId, previousActivityList);
                    hasPassedGatewayNode = true;
                }
                else
                {
                    previousActivityList.Add(aim.GetByFid(entity.SourceActivityNodeId));
                }
            }
            return previousActivityList;
        }

        private void GetPreviousOfGatewayActivity(IList<WfTransitionInstance> transitionList,
            string toActivityId,
            List<WfActivityInstance> previousActivityList)
        {
            var previousTransitionList = transitionList.Where(o => o.TargetActivityNodeId == toActivityId).ToList();

            var aim = new ActivityInstanceManager(_serviceProvider);
            foreach (var entity in previousTransitionList)
            {
                if (entity.SourceActivityNodeType == WfActivityType.TaskNode
                    || entity.SourceActivityNodeType == WfActivityType.PluginNode
                    || entity.SourceActivityNodeType == WfActivityType.ScriptNode
                    || entity.SourceActivityNodeType == WfActivityType.StartNode)
                {
                    previousActivityList.Add(aim.GetByFid(entity.SourceActivityInsUid));
                }
                else if (entity.SourceActivityNodeType == WfActivityType.GatewayNode)
                {
                    GetPreviousOfGatewayActivity(transitionList, entity.SourceActivityNodeId, previousActivityList);
                }
            }
        }

    }
}
