using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Manager;
using Fap.Workflow.Engine.Xpdl;
using Fap.Workflow.Engine.Xpdl.Node;
using Fap.Workflow.Model;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Logging;
using Fap.Core.DataAccess;
using System;

namespace Fap.Workflow.Engine.Core
{
    /// <summary>
    /// 运行时的创建类
    /// 静态方法：创建执行实例的运行者对象
    /// </summary>
    internal class WfRuntimeManagerFactory
    {
        #region WfRuntimeManager 创建启动运行时对象
        /// <summary>
        /// 启动流程
        /// </summary>
        /// <param name="runner">执行者</param>
        /// <param name="result">结果对象</param>
        /// <returns>运行时实例对象</returns>
        public static WfRuntimeManager CreateRuntimeInstanceStartup(WfAppRunner runner,IServiceProvider serviceProvider,
            ref WfExecutedResult result)
        {
            return CreateRuntimeInstanceStartup(runner,serviceProvider, null, null, ref result);
        }

        public static WfRuntimeManager CreateRuntimeInstanceStartup(WfAppRunner runner, IServiceProvider serviceProvider,
            WfProcessInstance parentProcessInstance,
            SubProcessNode subProcessNode,
            ref WfExecutedResult result)
        {
            //检查流程是否可以被启动
            var rmins = new WfRuntimeManagerStartup(serviceProvider);

            rmins.WfExecutedResult = result = new WfExecutedResult();
            var pim = new ProcessInstanceManager(serviceProvider);
            WfProcessInstance processInstance = null;

            if (subProcessNode == null)
            {
                //正常流程启动
                processInstance = pim.GetProcessInstanceLatest(
                    runner.ProcessUid,
                    runner.BillUid);
            }
            else
            {
                //子流程启动
                processInstance = pim.GetProcessInstanceLatest(
                    subProcessNode.SubProcessId, runner.BillUid);
            }

            //不能同时启动多个主流程
            if (processInstance != null
                && processInstance.ParentProcessInsUid == null
                && processInstance.ProcessState == WfProcessInstanceState.Running)
            {
                result.Status = WfExecutedStatus.Exception;
                result.ExceptionType = WfExceptionType.Started_IsRunningAlready;
                result.Message = "流程已经处于运行状态，不要重复提交！";

            }
            else
            {

                //processInstance 为空，此时继续执行启动操作
                rmins.AppRunner = runner;
                rmins.ParentProcessInstance = parentProcessInstance;
                rmins.InvokedSubProcessNode = subProcessNode;

                //获取流程第一个可办理节点
                rmins.ProcessModel = new ProcessModel(serviceProvider, runner.ProcessUid, runner.BillUid);
            }

            //var firstActivity = rmins.ProcessModel.GetFirstActivity();

            //rmins.AppRunner.NextActivityPerformers = ActivityResource.CreateNextActivityPerformers(firstActivity.ActivityID,
            //    runner.UserId,
            //    runner.UserName);


            return rmins;
        }
        #endregion

        #region WfRuntimeManager 创建应用执行运行时对象
        /// <summary>
        /// 创建运行时实例对象
        /// </summary>
        /// <param name="runner">执行者</param>
        /// <param name="result">结果对象</param>
        /// <returns>运行时实例对象</returns>
        public static WfRuntimeManager CreateRuntimeInstanceAppRunning(
            WfAppRunner runner, IServiceProvider serviceProvider,
           ref WfExecutedResult result)
        {
            //检查传人参数是否有效
            var rmins = new WfRuntimeManagerAppRunning(serviceProvider);
            rmins.WfExecutedResult = result = new WfExecutedResult();
            if (string.IsNullOrEmpty(runner.BillUid)
                || runner.ProcessUid == null || runner.CurrActivityInsUid == null || runner.CurrProcessInsUid == null || runner.CurrWfTaskUid == null || runner.CurrNodeId == null)
            {
                result.Status = WfExecutedStatus.Exception;
                result.ExceptionType = WfExceptionType.RunApp_ErrorArguments;
                result.Message = "方法参数错误，无法运行流程！";
                return rmins;
            }

            //传递runner变量
            rmins.AppRunner = runner;

            var aim = new ActivityInstanceManager(serviceProvider);
            WfActivityInstance wfActivityInstance = aim.GetByFid(runner.CurrActivityInsUid);
            if(wfActivityInstance.ActivityState == WfActivityInstanceState.Completed)
            {
                result.Status = WfExecutedStatus.Exception;
                result.Message = "任务已经完成，不需要再审批。";
                return rmins;
            }
            rmins.RunningActivityInstance = wfActivityInstance;
            rmins.ProcessModel = new ProcessModel(serviceProvider, runner.ProcessUid, runner.BillUid);
            var tm = new TaskManager(serviceProvider);
            rmins.TaskView = tm.GetTask(runner.CurrWfTaskUid);
            return rmins;
        }
        #endregion

        #region WfRuntimeManager 创建跳转运行时对象
        /// <summary>
        /// 创建跳转实例信息
        /// </summary>
        /// <param name="runner">执行者</param>
        /// <param name="result">结果对象</param>
        /// <returns>运行时实例对象</returns>
        //public static WfRuntimeManager CreateRuntimeInstanceJump(WfAppRunner runner,
        //    ref WfExecutedResult result)
        //{
        //    var rmins = new WfRuntimeManagerJump();
        //    rmins.WfExecutedResult = result = new WfExecutedResult();

        //    if (string.IsNullOrEmpty(runner.AppName)
        //       || String.IsNullOrEmpty(runner.BizUid)
        //       || runner.ProcessId == null
        //       || runner.NextActivityPerformers == null)
        //    {
        //        result.Status = WfExecutedStatus.Exception;
        //        result.ExceptionType = WfExceptionType.Jump_ErrorArguments;
        //        result.Message = "方法参数错误，无法运行流程！";
        //        return rmins;
        //    }

        //流程跳转时，只能跳转到一个节点
        //if (runner.NextActivityPerformers.Count() > 1)
        //{
        //    result.Status = WfExecutedStatus.Exception;
        //    result.ExceptionType = WfExceptionType.Jump_OverOneStep;
        //    result.Message = string.Format("不能跳转到多个节点！节点数:{0}",
        //        runner.NextActivityPerformers.Count());
        //    return rmins;
        //}

        //获取当前运行节点信息
        //var aim = new ActivityInstanceManager();
        //TaskViewEntity taskView = null;
        //var runningNode = aim.GetRunningNode(runner, out taskView);

        ////传递runner变量
        //rmins.TaskView = taskView;
        //rmins.AppRunner = runner;
        //rmins.AppRunner.AppName = runner.AppName;
        //rmins.AppRunner.BizUid = runner.BizUid;
        //rmins.AppRunner.ProcessId = runner.ProcessId;
        //rmins.AppRunner.UserID = runner.UserID;
        //rmins.AppRunner.UserName = runner.UserName;

        //var processModel = ProcessModelFactory.Create(taskView.ProcessId, taskView.Version);
        //rmins.ProcessModel = processModel;


        //跳转到从未执行过的节点上
        //    var activityResource = new ActivityResource(runner, runner.NextActivityPerformers, runner.Conditions);
        //    rmins.ActivityResource = activityResource;
        //    //rmins.RunningActivityInstance = runningNode;

        //    return rmins;
        //}
        #endregion

        #region WfRuntimeManager 创建撤销运行时对象
        /// <summary>
        /// 撤销操作
        /// 包括：
        /// 1) 正常流转
        /// 2) 多实例节点流转
        /// </summary>
        /// <param name="runner">执行者</param>
        /// <param name="result">结果对象</param>
        /// <returns>运行时实例对象</returns>
        internal static WfRuntimeManager CreateRuntimeInstanceWithdraw(WfAppRunner runner, IServiceProvider serviceProvider,
            ref WfExecutedResult result)
        {
            WfRuntimeManager rmins = new WfRuntimeManagerWithdraw(serviceProvider);
            rmins.WfExecutedResult = result = new WfExecutedResult();

            var aim = new TaskManager(serviceProvider);
            if(aim.ExistApproval(runner.CurrProcessInsUid))
            {
                result.Status = WfExecutedStatus.Exception;
                result.ExceptionType = WfExceptionType.Withdraw_NotInReady;
                result.Message = "流程存在审批人已经审批的情况，不能撤回。";

                return rmins;
            }
            rmins.AppRunner = runner;
           
            return rmins;
        }


        //#endregion

        //#region WfRuntimeManager 创建退回运行时对象
        ///// <summary>
        ///// 退回操作
        ///// </summary>
        ///// <param name="runner">执行者</param>
        ///// <param name="result">结果对象</param>
        ///// <returns>运行时实例对象</returns>
        internal static WfRuntimeManager CreateRuntimeInstanceSendBack(WfAppRunner runner, IServiceProvider serviceProvider,
            ref WfExecutedResult result)
        {
            WfRuntimeManager rmins = new WfRuntimeManagerSendBack(serviceProvider);
            rmins.WfExecutedResult = result = new WfExecutedResult();

            var aim = new ActivityInstanceManager(serviceProvider);
            var runningActivityInstanceList = aim.GetRunningActivityInstanceList(runner.CurrProcessInsUid).AsList();            

            //当前没有运行状态的节点存在，流程不存在，或者已经结束或取消
            if (runningActivityInstanceList == null || runningActivityInstanceList.Count() == 0)
            {
                result.Status = WfExecutedStatus.Exception;
                result.ExceptionType = WfExceptionType.Sendback_NotInRunning;
                result.Message = "当前没有运行状态的节点存在，流程不存在，或者已经结束或取消";

                return rmins;
            }
            rmins.AppRunner = runner;
            return rmins;
            
        }

        ///// <summary>
        ///// 根据不同退回场景创建运行时管理器
        ///// </summary>
        ///// <param name="runningActivityInstanceList">运行节点列表</param>
        ///// <param name="sendbackOperation">退回类型</param>
        ///// <param name="runner">执行者</param>
        ///// <param name="result">结果对象</param>
        ///// <returns>运行时管理器</returns>
        //private static WfRuntimeManager CreateRuntimeInstanceSendbackByCase(
        //    List<WfActivityInstance> runningActivityInstanceList,
        //    SendbackOpertaionTypeEnum sendbackOperation,
        //    WfAppRunner runner,
        //    ref WfExecutedResult result)
        //{
        //    WfRuntimeManager rmins = new WfRuntimeManagerSendBack();
        //    rmins.WfExecutedResult = result = new WfExecutedResult();

        //    WfActivityInstance runningNode = runningActivityInstanceList.Where(x => x.ActivityState == (int)ActivityStateEnum.Running).OrderBy(x => x.ID).FirstOrDefault();
        //    if (runningNode == null)
        //        runningNode = runningActivityInstanceList[0];
        //    WfProcessInstance processInstance = (new ProcessInstanceManager()).GetById(runningNode.ProcessInstanceID);
        //    IProcessModel processModel = ProcessModelFactory.Create(processInstance.ProcessId, processInstance.Version);

        //    var aim = new ActivityInstanceManager();

        //    //以下处理，需要知道上一步是独立节点的信息
        //    //获取上一步流转节点信息，可能经过And, Or等路由节点
        //    var tim = new TransitionInstanceManager();
        //    bool hasGatewayNode = false;
        //    var currentNode = runningNode;

        //    var lastActivityInstanceList = tim.GetPreviousActivityInstance(currentNode, true, out hasGatewayNode).ToList();
        //    if (lastActivityInstanceList == null || lastActivityInstanceList.Count == 0 || lastActivityInstanceList.Count > 1)
        //    {
        //        result.Status = WfExecutedStatus.Exception;
        //        result.ExceptionType = WfExceptionType.Sendback_NullOrHasTooMany;
        //        result.Message = "当前没有可以退回的节点，或者有多个可以退回的节点，无法选择！";

        //        return rmins;
        //    }

        //    TransitionInstanceEntity lastTaskTransitionInstance = null;
        //    lastTaskTransitionInstance = tim.GetLastTaskTransition(runner.AppName,
        //        runner.BizUid, runner.ProcessId);

        //    if (lastTaskTransitionInstance.TransitionType == (short)TransitionTypeEnum.Loop)
        //    {
        //        result.Status = WfExecutedStatus.Exception;
        //        result.ExceptionType = WfExceptionType.Sendback_IsLoopNode;
        //        result.Message = "当前流转是自循环，无需退回！";

        //        return rmins;
        //    }

        //    //设置退回节点的相关信息
        //    var previousActivityInstance = lastActivityInstanceList[0];
        //    if (previousActivityInstance.ActivityType == (short)ActivityTypeEnum.StartNode)
        //    {
        //        result.Status = WfExecutedStatus.Exception;
        //        result.ExceptionType = WfExceptionType.Sendback_PreviousIsStartNode;
        //        result.Message = "上一步是开始节点，无需退回！";

        //        return rmins;
        //    }

        //    if (sendbackOperation == SendbackOpertaionTypeEnum.Normal)
        //    {
        //        //简单串行模式下的退回
        //        rmins = new WfRuntimeManagerSendBack();
        //        rmins.WfExecutedResult = result = new WfExecutedResult();

        //        rmins.ProcessModel = processModel;
        //        rmins.BackwardContext.ProcessInstance = processInstance;
        //        rmins.BackwardContext.BackwardToTaskActivity = processModel.GetActivity(previousActivityInstance.ActivityGUID);
        //        rmins.BackwardContext.BackwardToTaskActivityInstance = previousActivityInstance;
        //        rmins.BackwardContext.BackwardToTargetTransitionGUID =
        //            hasGatewayNode == false ? lastTaskTransitionInstance.TransitionGUID : String.Empty;        //如果中间有Gateway节点，则没有直接相连的TransitonGUID

        //        rmins.BackwardContext.BackwardFromActivity = processModel.GetActivity(runningNode.ActivityGUID);
        //        rmins.BackwardContext.BackwardFromActivityInstance = runningNode;
        //        rmins.BackwardContext.BackwardTaskReciever = WfBackwardTaskReciever.Instance(previousActivityInstance.ActivityName,
        //            previousActivityInstance.EndedByUserID, previousActivityInstance.EndedByUserName);

        //        //封装AppUser对象
        //        rmins.AppRunner.AppName = runner.AppName;
        //        rmins.AppRunner.BizUid = runner.BizUid;
        //        rmins.AppRunner.ProcessId = runner.ProcessId;
        //        rmins.AppRunner.UserID = runner.UserID;
        //        rmins.AppRunner.UserName = runner.UserName;
        //        rmins.AppRunner.NextActivityPerformers = ActivityResource.CreateNextActivityPerformers(previousActivityInstance.ActivityGUID,
        //            previousActivityInstance.EndedByUserID,
        //            previousActivityInstance.EndedByUserName);
        //        rmins.ActivityResource = new ActivityResource(runner, rmins.AppRunner.NextActivityPerformers);

        //        return rmins;
        //    }

        //    //如果有其它模式，没有处理到，则直接抛出异常
        //    throw new WorkflowException("未知的退回场景，请报告给技术支持人员！");
        //}
        //#endregion

        //#region WfRuntimeManager 创建返签运行时对象
        ///// <summary>
        ///// 流程返签，先检查约束条件，然后调用wfruntimeinstance执行
        ///// </summary>
        ///// <param name="runner">执行者</param>
        ///// <param name="result">结果对象</param>
        ///// <returns>运行时实例对象</returns>
        //public static WfRuntimeManager CreateRuntimeInstanceReverse(WfAppRunner runner,
        //    ref WfExecutedResult result)
        //{
        //    var rmins = new WfRuntimeManagerReverse();
        //    rmins.WfExecutedResult = result = new WfExecutedResult();
        //    var pim = new ProcessInstanceManager();
        //    var processInstance = pim.GetProcessInstanceLatest(runner.AppName, runner.BizUid, runner.ProcessId);
        //    if (processInstance == null || processInstance.ProcessState != (short)ProcessStateEnum.Completed)
        //    {
        //        result.Status = WfExecutedStatus.Exception;
        //        result.ExceptionType = WfExceptionType.Reverse_NotInCompleted;
        //        result.Message = string.Format("当前应用:{0}，实例ID：{1}, 没有完成的流程实例，无法让流程重新运行！",
        //            runner.AppName, runner.BizUid);
        //        return rmins;
        //    }

        //    var tim = new TransitionInstanceManager();
        //    var endTransitionInstance = tim.GetEndTransition(runner.AppName, runner.BizUid, runner.ProcessId);

        //    var processModel = ProcessModelFactory.Create(processInstance.ProcessId, processInstance.Version);
        //    var endActivity = processModel.GetActivity(endTransitionInstance.ToActivityGUID);

        //    var aim = new ActivityInstanceManager();
        //    var endActivityInstance = aim.GetById(endTransitionInstance.ToActivityInstanceID);

        //    bool hasGatewayNode = false;
        //    var lastTaskActivityInstance = tim.GetPreviousActivityInstance(endActivityInstance, false,
        //        out hasGatewayNode).ToList()[0];
        //    var lastTaskActivity = processModel.GetActivity(lastTaskActivityInstance.ActivityGUID);

        //    //封装返签结束点之前办理节点的任务接收人
        //    rmins.AppRunner.NextActivityPerformers = ActivityResource.CreateNextActivityPerformers(lastTaskActivityInstance.ActivityGUID,
        //        lastTaskActivityInstance.EndedByUserID,
        //        lastTaskActivityInstance.EndedByUserName);

        //    rmins.ActivityResource = new ActivityResource(runner, rmins.AppRunner.NextActivityPerformers);
        //    rmins.AppRunner.AppName = runner.AppName;
        //    rmins.AppRunner.BizUid = runner.BizUid;
        //    rmins.AppRunner.ProcessId = runner.ProcessId;
        //    rmins.AppRunner.UserID = runner.UserID;
        //    rmins.AppRunner.UserName = runner.UserName;

        //    rmins.BackwardContext.ProcessInstance = processInstance;
        //    rmins.BackwardContext.BackwardToTaskActivity = lastTaskActivity;
        //    rmins.BackwardContext.BackwardToTaskActivityInstance = lastTaskActivityInstance;
        //    rmins.BackwardContext.BackwardToTargetTransitionGUID =
        //        hasGatewayNode == false ? endTransitionInstance.TransitionGUID : String.Empty;
        //    rmins.BackwardContext.BackwardFromActivity = endActivity;
        //    rmins.BackwardContext.BackwardFromActivityInstance = endActivityInstance;
        //    rmins.BackwardContext.BackwardTaskReciever = WfBackwardTaskReciever.Instance(lastTaskActivityInstance.ActivityName,
        //        lastTaskActivityInstance.EndedByUserID,
        //        lastTaskActivityInstance.EndedByUserName);

        //    return rmins;
        //}
        #endregion
    }
}
