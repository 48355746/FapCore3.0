using Fap.Core.DataAccess;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Core;
using Fap.Workflow.Engine.Enums;
using Fap.Workflow.Engine.Exceptions;
using Fap.Workflow.Engine.Manager;
using Fap.Workflow.Engine.Xpdl;
using Fap.Workflow.Engine.Xpdl.Entity;
using Fap.Workflow.Model;
using Fap.Wrokflow.Engine.Node;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fap.Workflow.Engine.Node
{
    /// <summary>
    /// 节点执行器的抽象类
    /// </summary>
    internal abstract class NodeMediator
    {
        protected readonly IDbContext _dataAccessor;
        protected readonly ILoggerFactory _loggerFactory;
        protected readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NodeMediator> _logger;
        #region 属性列表
        private ActivityForwardContext _activityForwardContext;
        protected ActivityForwardContext ActivityForwardContext
        {
            get
            {
                return _activityForwardContext;
            }
        }

        private BackwardContext _backwardContext;
        protected BackwardContext BackwardContext
        {
            get
            {
                return _backwardContext;
            }
        }
        protected WfAppRunner AppRunner { get; set; }
        private Linker _linker;
        internal Linker Linker
        {
            get
            {
                if (_linker == null)
                    _linker = new Linker();

                return _linker;
            }
        }


        /// <summary>
        /// 活动节点实例管理对象
        /// </summary>
        private ActivityInstanceManager activityInstanceManager;
        internal ActivityInstanceManager ActivityInstanceManager
        {
            get
            {
                if (activityInstanceManager == null)
                {
                    activityInstanceManager = new ActivityInstanceManager(_serviceProvider);
                }
                return activityInstanceManager;
            }
        }

        private TaskManager taskManager;
        internal TaskManager TaskManager
        {
            get
            {
                if (taskManager == null)
                {
                    taskManager = new TaskManager(_serviceProvider);
                }
                return taskManager;
            }
        }

        private ProcessInstanceManager processInstanceManager;
        internal ProcessInstanceManager ProcessInstanceManager
        {
            get
            {
                if (processInstanceManager == null)
                    processInstanceManager = new ProcessInstanceManager(_serviceProvider);
                return processInstanceManager;
            }
        }
        private WfNodeMediatedResult _wfNodeMediatedResult;
        internal WfNodeMediatedResult WfNodeMediatedResult
        {
            get
            {
                if (_wfNodeMediatedResult == null)
                {
                    _wfNodeMediatedResult = new WfNodeMediatedResult();
                }
                return _wfNodeMediatedResult;
            }
        }
        //回退时参数对象
        private ReturnDataContext _returnDataContext;

        public ReturnDataContext ReturnDataContext
        {
            get
            {
                if (_returnDataContext == null)
                {
                    _returnDataContext = new ReturnDataContext();
                }
                return _returnDataContext;
            }
        }
        #endregion

        #region 抽象方法列表
        /// <summary>
        /// 执行节点方法
        /// </summary>
        internal abstract void ExecuteWorkItem();
        #endregion

        #region 构造函数
        /// <summary>
        /// 向前流转时的NodeMediator的构造函数
        /// </summary>
        /// <param name="forwardContext">前进上下文</param>
        /// <param name="session">Session</param>
        internal NodeMediator(ActivityForwardContext forwardContext, WfAppRunner appRunner,IServiceProvider serviceProvider)
        {
            _activityForwardContext = forwardContext;
            _serviceProvider = serviceProvider;
            AppRunner = appRunner;
            _dataAccessor = _serviceProvider.GetService<IDbContext>();
            _loggerFactory =_serviceProvider.GetService<ILoggerFactory>();
            _logger = _loggerFactory.CreateLogger<NodeMediator>();
            Linker.FromActivity = forwardContext.Activity;
        }
        internal NodeMediator(WfAppRunner appRunner, IServiceProvider serviceProvider)
        {
            AppRunner = appRunner;
            _serviceProvider = serviceProvider;
            _dataAccessor = _serviceProvider.GetService<IDbContext>();
            _loggerFactory = _serviceProvider.GetService<ILoggerFactory>(); 
            _logger = _loggerFactory.CreateLogger<NodeMediator>();
        }
        /// <summary>
        /// 退回处理时的NodeMediator的构造函数
        /// </summary>
        /// <param name="backwardContext">退回上下文</param>
        /// <param name="session">Session</param>
        internal NodeMediator(BackwardContext backwardContext, WfAppRunner appRunner, IDbContext dbContext, ILoggerFactory loggerFactory)
        {
            _dataAccessor = dbContext;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<NodeMediator>();
            _backwardContext = backwardContext;
            AppRunner = appRunner;
            Linker.FromActivity = backwardContext.BackwardFromActivity;
        }

        #endregion

        #region 执行外部事件的方法
       
        #endregion

        #region 流程执行逻辑
        /// <summary>
        /// 遍历执行当前节点后面的节点
        /// </summary>
        /// <param name="isJumpforward">是否跳转</param>
        internal virtual void ContinueForwardCurrentNode(bool isJumpforward)
        {
           
            if (isJumpforward == false)
            {
                //非跳转模式的正常运行
                var nextActivityMatchedResult = this.ActivityForwardContext.ProcessModel.GetNextActivityList(this.Linker.FromActivityInstance.NodeId);

                if (nextActivityMatchedResult.MatchedType != NextActivityMatchedType.Successed
                    || nextActivityMatchedResult.Root.HasChildren == false)
                {
                    throw new WfRuntimeException("没有匹配的后续流转节点，流程虽然能处理当前节点，但是无法流转到下一步！");
                }

                ContinueForwardCurrentNodeRecurisivly(this.Linker.FromActivity,
                    this.Linker.FromActivityInstance,
                    nextActivityMatchedResult.Root,
                    isJumpforward);
            }
            else
            {
                //跳转模式运行
                var root = NextActivityComponentFactory.CreateNextActivityComponent();
                var nextActivityComponent = NextActivityComponentFactory.CreateNextActivityComponent(
                    this.Linker.FromActivity,
                    this.Linker.ToActivity);

                root.Add(nextActivityComponent);

                ContinueForwardCurrentNodeRecurisivly(this.Linker.FromActivity,
                    this.Linker.FromActivityInstance,
                    root,
                    isJumpforward);
            }
        }

        /// <summary>
        /// 递归执行节点
        /// 1)创建普通节点的任务
        /// 2)创建会签节点的任务
        /// </summary>
        /// <param name="fromActivity">起始活动</param>
        /// <param name="fromActivityInstance">起始活动实例</param>
        /// <param name="isJumpforward">是否跳跃</param>
        /// <param name="root">根节点</param>
        /// <param name="conditionKeyValuePair">条件key-value</param>
        protected void ContinueForwardCurrentNodeRecurisivly(ActivityEntity fromActivity,
            WfActivityInstance fromActivityInstance,
            NextActivityComponent root,
            Boolean isJumpforward)
        {
            foreach (NextActivityComponent comp in root)
            {
                if (comp.HasChildren)
                {
                    //此节点类型为分支或合并节点类型：首先需要实例化当前节点(自动完成)
                    NodeMediatorGateway gatewayNodeMediator = NodeMediatorGatewayFactory.CreateGatewayNodeMediator(comp.Activity,
                        this.ActivityForwardContext.ProcessModel,
                        AppRunner, _serviceProvider);

                    ICompleteAutomaticlly autoGateway = (ICompleteAutomaticlly)gatewayNodeMediator;
                    GatewayExecutedResult gatewayResult = autoGateway.CompleteAutomaticlly(
                        ActivityForwardContext.ProcessInstance,
                        comp.Transition.TransitionID,
                        fromActivityInstance);

                    if (gatewayResult.Status == GatewayExecutedStatus.Successed)
                    {
                        //遍历后续子节点
                        ContinueForwardCurrentNodeRecurisivly(fromActivity,
                            gatewayNodeMediator.GatewayActivityInstance,
                            comp,
                            isJumpforward);
                    }
                    else
                    {
                        WfRuntimeException e = new WfRuntimeException("第一个满足条件的节点已经被成功执行，此后的节点被阻止在XOrJoin节点!");
                        _logger.LogError($"第一个满足条件的节点已经被成功执行，此后的节点被阻止在XOrJoin节点!");                        
                    }
                }
                else if (comp.Activity.ActivityType == ActivityTypeEnum.TaskNode||comp.Activity.ActivityType== ActivityTypeEnum.TimerNode|| comp.Activity.ActivityType == ActivityTypeEnum.SignNode)                   //普通任务节点
                {
                    //此节点类型为任务节点：根据fromActivityInstance的类型判断是否可以创建任务
                    if (fromActivityInstance.ActivityState == ActivityStateEnum.Completed.ToString())
                    {
                        //创建新任务节点
                        NodeMediator taskNodeMediator = new NodeMediatorTask(AppRunner, _serviceProvider);
                        taskNodeMediator.CreateActivityTaskTransitionInstance(comp.Activity,
                            ActivityForwardContext.ProcessInstance,
                            fromActivityInstance,
                            comp.Transition.TransitionID,
                            comp.Transition.DirectionType == TransitionDirectionTypeEnum.Loop ?
                                TransitionTypeEnum.Loop : TransitionTypeEnum.Forward, //根据Direction方向确定是否是自身循环
                            isJumpforward == true ?
                                TransitionFlyingTypeEnum.ForwardFlying : TransitionFlyingTypeEnum.NotFlying);
                    }
                    else
                    {
                        //下一步的任务节点没有创建，需给出提示信息
                        if ((fromActivity.GatewayDirectionType | GatewayDirectionEnum.AllJoinType)
                            == GatewayDirectionEnum.AllJoinType)
                        {
                            WfRuntimeException e = new WfRuntimeException("等待其它需要合并的分支!");
                          _logger.LogWarning("等待其它需要合并的分支!");
                        }
                    }
                }
                else if (comp.Activity.ActivityType == ActivityTypeEnum.SubProcessNode)         //子流程节点
                {
                    //节点类型为subprocessnode
                    if (fromActivityInstance.ActivityState ==ActivityStateEnum.Completed.ToString())
                    {
                        //实例化subprocess节点数据
                        NodeMediator subNodeMediator = new NodeMediatorSubProcess(AppRunner,_serviceProvider);
                        subNodeMediator.CreateActivityTaskTransitionInstance(comp.Activity,
                            ActivityForwardContext.ProcessInstance,
                            fromActivityInstance,
                            comp.Transition.TransitionID,
                            comp.Transition.DirectionType == TransitionDirectionTypeEnum.Loop ?
                                TransitionTypeEnum.Loop : TransitionTypeEnum.Forward,
                            TransitionFlyingTypeEnum.NotFlying);
                    }
                }
                else if (comp.Activity.ActivityType == ActivityTypeEnum.EndNode)        //结束节点
                {
                    //此节点为完成结束节点，结束流程
                    NodeMediator endMediator = new NodeMediatorEnd(ActivityForwardContext, AppRunner,_serviceProvider);
                    endMediator.Linker.ToActivity = comp.Activity;

                    //创建结束节点实例及转移数据
                    endMediator.CreateActivityTaskTransitionInstance(comp.Activity, ActivityForwardContext.ProcessInstance,
                        fromActivityInstance, comp.Transition.TransitionID, TransitionTypeEnum.Forward,
                        TransitionFlyingTypeEnum.NotFlying);

                    //执行结束节点中的业务逻辑
                    endMediator.ExecuteWorkItem();
                }
                else
                {
                    WfRuntimeException e = new WfRuntimeException(string.Format("XML文件定义了未知的节点类型，执行失败，节点类型信息：{0}",
                        comp.Activity.ActivityType.ToString()));
                    _logger.LogError($"XML文件定义了未知的节点类型，执行失败，节点类型信息：{comp.Activity.ActivityType.ToString()}");
                }
            }
        }

        /// <summary>
        /// 创建工作项及转移数据
        /// </summary>
        /// <param name="toActivity">活动</param>
        /// <param name="processInstance">流程实例</param>
        /// <param name="fromActivityInstance">起始活动实例</param>
        /// <param name="transitionGUID">转移GUID</param>
        /// <param name="transitionType">转移类型</param>
        /// <param name="flyingType">跳跃类型</param>
        /// <param name="activityResource">活动资源</param>
        /// <param name="session">Session</param>
        internal virtual void CreateActivityTaskTransitionInstance(ActivityEntity toActivity,
            WfProcessInstance processInstance,
            WfActivityInstance fromActivityInstance,
            String transitionGUID,
            TransitionTypeEnum transitionType,
            TransitionFlyingTypeEnum flyingType
           )
        { }

        /// <summary>
        /// 创建任务的虚方法
        /// 1. 对于自动执行的工作项，无需重写该方法
        /// 2. 对于人工执行的工作项，需要重写该方法，插入待办的任务数据
        /// </summary>
        /// <param name="activityResource">活动资源</param>
        /// <param name="activity">活动实例</param>
        internal virtual void CreateNewTask(WfActivityInstance toActivityInstance,
            ActivityEntity activity)
        {
            if (activity.Performers == null)
            {
                throw new WorkflowException("无法创建任务，流程流转下一步的办理人员不能为空！");
            }

            if (this.AppRunner.NextActivity != null && this.AppRunner.NextActivity.ActivityID == toActivityInstance.NodeId && (this.AppRunner.NextActivity.Performers?.Any()??false))
            {
                //采用指定审批人
                TaskManager.Insert(toActivityInstance,
                  this.AppRunner.NextActivity.Performers);
            }
            else
            {
                //采用默认审批人
                TaskManager.Insert(toActivityInstance,
                    activity.Performers);
            }
        }

        /// <summary>
        /// 创建节点对象
        /// </summary>
        /// <param name="processInstance">流程实例</param>
        /// <param name="activity">活动</param>
        /// <param name="runner">执行者</param>
        protected WfActivityInstance CreateActivityInstanceObject(ActivityEntity activity,
            WfProcessInstance processInstance,
            WfAppRunner runner)
        {
            WfActivityInstance entity = ActivityInstanceManager.CreateActivityInstanceObject(
                processInstance,               
                activity,
                runner);

            return entity;
        }

        /// <summary>
        /// 创建退回类型的活动实例对象
        /// </summary>
        /// <param name="processInstance">流程实例</param>
        /// <param name="backwardType">退回类型</param>
        /// <param name="backSrcActivityInstanceID">退回的活动实例ID</param>
        /// <param name="runner">登录用户</param>
        /// <returns></returns>
        protected WfActivityInstance CreateBackwardToActivityInstanceObject(WfProcessInstance processInstance,WfActivityInstance activityInstance,
            BackwardTypeEnum backwardType,
            string backSrcActivityInstanceID,
            WfAppRunner runner)
        {
            WfActivityInstance entity = ActivityInstanceManager.CreateBackwardActivityInstanceObject(
                processInstance,
                this.BackwardContext.BackwardToTaskActivity,
                activityInstance,
                backwardType,
                this.BackwardContext.BackwardToTaskActivity.ActivityID,
                runner);

            return entity;
        }

        /// <summary>
        /// 插入连线实例的方法
        /// </summary>
        /// <param name="processInstance">流程实例</param>
        /// <param name="transitionId">转移ID</param>
        /// <param name="fromActivityInstance">起始活动实例</param>
        /// <param name="toActivityInstance">到达活动实例</param>
        /// <param name="transitionType">转移类型</param>
        /// <param name="flyingType">跳跃类型</param>
        /// <param name="runner">执行者</param>
        /// <param name="session">Session</param>
        internal virtual void InsertTransitionInstance(WfProcessInstance processInstance,
            String transitionId,
            WfActivityInstance fromActivityInstance,
            WfActivityInstance toActivityInstance,
            TransitionTypeEnum transitionType,
            TransitionFlyingTypeEnum flyingType,
            WfAppRunner runner)
        {
            var tim = new TransitionInstanceManager(_serviceProvider);
            var transitionInstanceObject = tim.CreateTransitionInstanceObject(processInstance,
                transitionId,
                fromActivityInstance,
                toActivityInstance,
                transitionType,
                flyingType,
                runner,
                (byte)ConditionParseResultEnum.Passed);

            tim.Insert( transitionInstanceObject);
        }

        /// <summary>
        /// 生成任务办理人ID字符串列表
        /// </summary>
        /// <param name="performerList">操作者列表</param>
        /// <returns>ID字符串列表</returns>
        protected string GenerateActivityAssignedUserIDs(IList<Performer> performerList)
        {
            StringBuilder strBuilder = new StringBuilder(1024);
            foreach (var performer in performerList)
            {
                if (strBuilder.ToString() != "")
                    strBuilder.Append(",");
                strBuilder.Append(performer.UserId);
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 生成办理人名称的字符串列表
        /// </summary>
        /// <param name="performerList">操作者列表</param>
        /// <returns>ID字符串列表</returns>
        protected string GenerateActivityAssignedUserNames(IList<Performer> performerList)
        {
            StringBuilder strBuilder = new StringBuilder(1024);
            foreach (var performer in performerList)
            {
                if (strBuilder.ToString() != "")
                    strBuilder.Append(",");
                strBuilder.Append(performer.UserName);
            }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 由节点分配的人员信息生成PerformerList数据结构
        /// </summary>
        /// <param name="activityInstance">活动实例</param>
        /// <returns>人员列表</returns>
        protected PerformerList AntiGenerateActivityPerformerList(WfActivityInstance activityInstance)
        {
            var performerList = new PerformerList();

            if (!string.IsNullOrEmpty(activityInstance.AssignedToUserIds)
                && !string.IsNullOrEmpty(activityInstance.AssignedToUserNames))
            {
                var assignedToUserIDs = activityInstance.AssignedToUserIds.Split(',');
                var assignedToUserNames = activityInstance.AssignedToUserNames.Split(',');

                for (var i = 0; i < assignedToUserIDs.Length; i++)
                {
                    performerList.Add(new Performer { UserId = assignedToUserIDs[i], UserName = assignedToUserNames[i] });
                }
            }
            return performerList;
        }
        #endregion
        /// <summary>
        /// 根据节点执行结果类型，生成消息
        /// </summary>
        /// <returns>消息内容</returns>
        internal string GetNodeMediatedMessage()
        {
            var message = string.Empty;
            if (WfNodeMediatedResult.Feedback == WfNodeMediatedFeedback.ForwardToNextSequenceTask)
            {
                message = "串行会(加)签，设置下一个执行节点的任务进入运行状态！";
            }
            else if (WfNodeMediatedResult.Feedback == WfNodeMediatedFeedback.WaitingForCompletedMore)
            {
                message = "并行会(加)签，等待节点到达足够多的完成比例！";
            }

            return message;
        }
    }
}
