using Ardalis.GuardClauses;
using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Exceptions;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Utility;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Core;
using Fap.Workflow.Engine.Entity;
using Fap.Workflow.Engine.Event;
using Fap.Workflow.Engine.Manager;
using Fap.Workflow.Engine.Message;
using Fap.Workflow.Engine.Utility;
using Fap.Workflow.Engine.WriteBack;
using Fap.Workflow.Engine.Xpdl;
using Fap.Workflow.Engine.Xpdl.Entity;
using Fap.Workflow.Model;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;

namespace Fap.Workflow.Service
{
    [Service]
    public partial class WorkflowService : IWorkflowService
    {
        private IDbContext _dataAccessor;
        private ILoggerFactory _loggerFactory;
        private readonly ILogger<WorkflowService> _logger;
        private readonly IFapApplicationContext _applicationContext;
        private IProcessModel _processModel;
        public WorkflowService(IDbContext dataAccessor, IFapApplicationContext applicationContext, ILoggerFactory loggerFactory)
        {
            _dataAccessor = dataAccessor;
            _loggerFactory = loggerFactory;
            _applicationContext = applicationContext;
            _logger = loggerFactory.CreateLogger<WorkflowService>();
        }

        #region 流程CRUD
        /// <summary>
        /// 直接升级流程模板
        /// </summary>
        /// <param name="id"></param>
        [Transactional]
        public void UpgradeProcessTemplateDirectly(long id)
        {
            WfProcess wfProcess = _dataAccessor.Get<WfProcess>(id);
            if (wfProcess != null)
            {
                WfProcess newWfProcess = wfProcess.Clone() as WfProcess;
                //升级流程图
                newWfProcess.Fid = UUIDUtils.Fid;
                newWfProcess.Version += 1;


                //升级流程图
                WfDiagram wfDiagram = _dataAccessor.Get<WfDiagram>(wfProcess.DiagramId);
                if (wfDiagram != null)
                {
                    wfDiagram.Version = newWfProcess.Version;
                    wfDiagram.Fid = UUIDUtils.Fid;
                    wfDiagram.ProcessUid = newWfProcess.Fid;
                    _dataAccessor.Insert<WfDiagram>(wfDiagram);
                }

                //更新升级后的流程模板
                newWfProcess.DiagramId = wfDiagram.Fid;
                _dataAccessor.Insert<WfProcess>(newWfProcess);
                //将原来的流程模板变成历史状态
                wfProcess.Status = WfProcessState.Historical;
                _dataAccessor.Update<WfProcess>(wfProcess);
            }

        }
        /// <summary>
        /// 是否允许删除流程模板
        /// </summary>
        /// <param name="processUids"></param>
        public bool AllowDeleteProcessTemplate(string processUids)
        {
            Guard.Against.NullOrEmpty(processUids, nameof(processUids));

            string[] fids = processUids.Split(',');
            int count = _dataAccessor.ExecuteScalar<int>("select count(0) from WfBusiness a, WfProcess b where a.WfProcessUid=b.Fid and b.Fid IN @Fids", new DynamicParameters(new { Fids = fids }));
            return count == 0;
        }
        /// <summary>
        /// 保存流程
        /// </summary>
        /// <param name="xml"></param>
        [Transactional]
        public void SaveProcessTemplate(string xml)
        {
            xml = xml.Replace("mxGraphModel", "workflowProcess");
            XmlDocument doc = new XmlDocument();

            doc.LoadXml(xml);
            var workflowNode = doc.GetElementsByTagName("workflowProcess")[0];
            string processUid = XMLHelper.GetXmlAttribute(workflowNode, "wfProcessUid");
            string diagramUid = XMLHelper.GetXmlAttribute(workflowNode, "wfDiagramUid");
            string diagramName = XMLHelper.GetXmlAttribute(workflowNode, "wfDiagramName");
            string formType = XMLHelper.GetXmlAttribute(workflowNode, "frmType");
            string billTable = XMLHelper.GetXmlAttribute(workflowNode, "billTable");
            string formTemplate = XMLHelper.GetXmlAttribute(workflowNode, "billTemplate");
            string notice = XMLHelper.GetXmlAttribute(workflowNode, "wfResultNotice");
            string suspendNotice = XMLHelper.GetXmlAttribute(workflowNode, "wfSuspendNotice");
            string wfMail = XMLHelper.GetXmlAttribute(workflowNode, "wfMail");
            string wfMessage = XMLHelper.GetXmlAttribute(workflowNode, "wfMessage");
            string messageSetting = $"{{'notice':{notice},'suspend':{suspendNotice},'mail':{wfMail},'message':{wfMessage}}}";
            WfDiagram wfDiagram = _dataAccessor.Get<WfDiagram>(diagramUid);
            WfProcess wfProcess = _dataAccessor.Get<WfProcess>(processUid);
            wfProcess.DiagramId = diagramUid;
            wfProcess.ProcessName = diagramName;
            wfProcess.FormType = formType;
            wfProcess.FormTemplateUid = formTemplate;
            wfProcess.BillTable = billTable;
            wfProcess.MessageSetting = messageSetting;
            if (wfDiagram == null)
            {
                //新增
                wfDiagram = new WfDiagram() { Fid = diagramUid, ProcessName = diagramName, ProcessUid = processUid, XmlContent = xml };

                //保存流程图
                _dataAccessor.Insert<WfDiagram>(wfDiagram);
                _dataAccessor.Update<WfProcess>(wfProcess);

            }
            else //更新
            {
                wfDiagram.ProcessName = diagramName;
                wfDiagram.XmlContent = xml;

                //保存流程图
                _dataAccessor.Update<WfDiagram>(wfDiagram);
                _dataAccessor.Update<WfProcess>(wfProcess);

            }


        }
        #endregion

        #region 流程启动
        private AutoResetEvent waitHandler = new AutoResetEvent(false);


        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="starter"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        [Transactional]
        public WfExecutedResult StartProcess(WfAppRunner runner)
        {
            WfExecutedResult result = new WfExecutedResult();
            try
            {
                WfRuntimeManager runtimeManager = WfRuntimeManagerFactory.CreateRuntimeInstanceStartup(runner, _dataAccessor, _loggerFactory, ref result);
                runtimeManager.OnWfProcessExecuted += delegate (object sender, WfEventArgs args)
               {
                   result = args.WfExecutedResult;
                   waitHandler.Set();
               };
                if (result.Status != WfExecutedStatus.Exception)
                {
                    runtimeManager.Execute();

                    waitHandler.WaitOne();

                    if (runtimeManager.WfExecutedResult.Status == WfExecutedStatus.Success)
                    {
                        if (runner.BillData.BillStatus != BillStatus.PASSED)
                        {
                            //改变单据状态
                            runner.BillData.BillStatus = BillStatus.PROCESSING;
                            string updateSql = $"update {runner.BillTableName} set SubmitTime='{DateTimeUtils.CurrentDateTimeStr}', BillStatus='{BillStatus.PROCESSING}' where id={runner.BillData.Id}";
                            _dataAccessor.Execute(updateSql);
                        }
                    }
                }
                result = runtimeManager.WfExecutedResult;

            }
            catch (System.Exception e)
            {
                result.Status = WfExecutedStatus.Failed;
                result.Message = string.Format("流程启动发生错误，内部异常:{0}", e.Message);
                _logger.LogError($"{WorkflowConstants.WF_PROCESS_ERROR}:{e.Message}");

            }
            return result;
        }

        /// <summary>
        /// 运行流程
        /// </summary>
        /// <param name="conn">连接</param>
        /// <param name="runner">运行人</param>
        /// <param name="trans">事务</param>
        /// <returns>运行结果</returns>
        [Transactional]
        public WfExecutedResult RunProcess(WfAppRunner runner)
        {
            WfExecutedResult result = new WfExecutedResult();
            try
            {

                var runtimeInstance = WfRuntimeManagerFactory.CreateRuntimeInstanceAppRunning(runner, _dataAccessor, _loggerFactory, ref result);

                if (result.Status != WfExecutedStatus.Exception)
                {

                    runtimeInstance.OnWfProcessExecuted += delegate (object sender, WfEventArgs args)
                    {
                        result = args.WfExecutedResult;
                        waitHandler.Set();
                    };
                    bool isRunning = runtimeInstance.Execute();

                    waitHandler.WaitOne();

                    //if (runtimeInstance.WfExecutedResult.Status == WfExecutedStatus.Success)
                    //{
                    //    TaskAdviceManager tam = new TaskAdviceManager(accessor);
                    //    tam.RecordWhenStartup(runner.ProcessId, runner.TaskId, runner.Comment);
                    //}

                    result = runtimeInstance.WfExecutedResult;
                }

                return result;
            }
            catch (System.Exception e)
            {
                result.Status = WfExecutedStatus.Failed;
                var error = e.InnerException != null ? e.InnerException.Message : e.Message;
                result.Message = string.Format("流程启动发生错误，内部异常:{0}", e.Message);
                _logger.LogError($"{WorkflowConstants.WF_PROCESS_ERROR}:{e.Message}");
            }

            return result;
        }


        #endregion

        #region 获取节点信息
        /// <summary>
        /// 获取流程的第一个可办理节点
        /// </summary>
        /// <returns></returns>
        [Transactional]
        public ActivityEntity GetFirstActivity(string processId, string billUid)
        {
            _processModel = new ProcessModel(_dataAccessor, _loggerFactory, processId, billUid);
            var firstActivity = _processModel.GetFirstActivity();
            return firstActivity;
        }
        /// <summary>
        /// 获取开始节点
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        //public ActivityEntity GetStartActivity(string processId,string billUid)
        //{
        //    _processModel = new ProcessModel(_wfProcessService,processId,billUid);
        //    var startActivity = _processModel.GetStartActivity();
        //    return startActivity;
        //}


        /// <summary>
        /// 获取任务类型的节点列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ActivityEntity> GetTaskActivityList(string processId, string billUid)
        {
            _processModel = new ProcessModel(_dataAccessor, _loggerFactory, processId, billUid);
            var activityList = _processModel.GetAllTaskActivityList();

            return activityList;
        }
        /// <summary>
        /// 判断节点是否审批完成，可以进入下一个节点
        /// </summary>
        /// <param name="activityInsUid"></param>
        /// <returns></returns>
        public bool NodeIsComplete(string activityInsUid)
        {
            bool result = false;

            ActivityInstanceManager activityInstanceManager = new ActivityInstanceManager(_dataAccessor, _loggerFactory);
            result = activityInstanceManager.IsComplete(activityInsUid);

            return result;
        }
        /// <summary>
        /// 获取当前节点的下一个节点信息
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public ActivityEntity GetNextActivity(string processId,
            string nodeId, string billUid)
        {
            _processModel = new ProcessModel(_dataAccessor, _loggerFactory, processId, billUid);
            var nextActivity = _processModel.GetNextActivity(nodeId);
            return nextActivity;
        }

        /// <summary>
        /// 获取第一可办理节点的执行人
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        public IList<Participant> GetFirstActivityParticipants(string processId, string billUid)
        {
            _processModel = new ProcessModel(_dataAccessor, _loggerFactory, processId, billUid);
            var firstActivity = _processModel.GetFirstActivity();
            return _processModel.GetActivityParticipants(firstActivity.ActivityID);
        }

        /// <summary>
        /// 获取当前节点的下一个节点信息
        /// </summary>
        /// <param name="processId"></param>
        /// <param name="nodeId"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IList<NodeView> GetNextNodeList(string processId, string nodeId, string billUid)
        {
            _processModel = new ProcessModel(_dataAccessor, _loggerFactory, processId, billUid);
            var nextSteps = _processModel.GetNextActivityTree(nodeId);
            return nextSteps;
        }
        /// <summary>
        /// 获取下一步的执行人
        /// </summary>
        /// <returns></returns>

        public PerformerList GetPerformerOfNextStep(string processId, string nodeId, string billUid)
        {
            _processModel = new ProcessModel(_dataAccessor, _loggerFactory, processId, billUid);
            return _processModel.GetActivityPerformers(nodeId);
        }


        ///// <summary>
        ///// 简单模式：根据应用获取流程下一步节点（不考虑有多个后续节点的情况）
        ///// </summary>
        ///// <param name="runner"></param>
        ///// <param name="condition"></param>
        ///// <param name="roleService"></param>
        ///// <returns></returns>
        //public NodeView GetNextActivity(WfAppRunner runner,
        //    IDictionary<string, string> condition = null,
        //    IUserRoleService roleService = null)
        //{
        //    NodeView nodeView = null;
        //    var list = GetNextActivityTree(runner, condition, roleService).ToList();
        //    if (list != null && list.Count() > 0)
        //    {
        //        nodeView = list[0];
        //    }
        //    return nodeView;
        //}

        ///// <summary>
        ///// 简单模式：根据应用获取流程下一步节点（不考虑有多个后续节点的情况）
        ///// </summary>
        ///// <param name="taskID"></param>
        ///// <param name="condition"></param>
        ///// <param name="roleService"></param>
        ///// <returns></returns>
        //public NodeView GetNextActivity(int taskID,
        //    IDictionary<string, string> condition = null,
        //    IUserRoleService roleService = null)
        //{
        //    NodeView nodeView = null;
        //    var list = GetNextActivityTree(taskID, condition, roleService).ToList();
        //    if (list != null && list.Count() > 0)
        //    {
        //        nodeView = list[0];
        //    }
        //    return nodeView;
        //}

        ///// <summary>
        ///// 根据应用获取流程下一步节点列表
        ///// </summary>
        ///// <param name="runner">应用执行人</param>
        ///// <param name="condition">条件</param>
        ///// <returns></returns>
        //public IList<NodeView> GetNextActivityTree(WfAppRunner runner, 
        //    IDictionary<string, string> condition = null,
        //    IUserRoleService roleService = null)
        //{
        //    var tm = new TaskManager();
        //    var taskView = tm.GetTaskOfMine(runner.AppInstanceID, runner.ProcessGUID, runner.UserID);
        //    var processModel = new XpdlModel(taskView.ProcessGUID, taskView.Version);
        //    var nextSteps = processModel.GetNextActivityTree(taskView.ProcessInstanceID,
        //        taskView.ActivityGUID,
        //        condition,
        //        roleService);

        //    return nextSteps;
        //}

        ///// <summary>
        ///// 获取下一步活动列表树
        ///// </summary>
        ///// <param name="taskID"></param>
        ///// <returns></returns>
        //public IList<NodeView> GetNextActivityTree(int taskID, 
        //    IDictionary<string, string> condition = null,
        //    IUserRoleService roleService = null)
        //{
        //    var taskView = (new TaskManager()).GetTaskView(taskID);
        //    var processModel = new XpdlModel(taskView.ProcessGUID, taskView.Version);
        //    var nextSteps = processModel.GetNextActivityTree(taskView.ProcessInstanceID, 
        //        taskView.ActivityGUID, 
        //        condition,
        //        roleService);

        //    return nextSteps;
        //}

        ///// <summary>
        ///// 获取已经完成的节点
        ///// </summary>
        ///// <param name="taskID"></param>
        ///// <returns></returns>
        //public IList<NodeImage> GetActivityInstanceCompleted(int taskID)
        //{
        //    IList<NodeImage> imageList = new List<NodeImage>();

        //    var tm = new TaskManager();
        //    var task = tm.GetTaskView(taskID);

        //    var am = new ActivityInstanceManager();
        //    var list = am.GetCompletedActivityInstanceList(task.AppInstanceID, task.ProcessGUID);

        //    foreach (WfActivity a in list)
        //    {
        //        imageList.Add(new NodeImage
        //        {
        //            ID = a.ID,
        //            ActivityName = a.ActivityName
        //        });
        //    }
        //    return imageList;
        //}

        ///// <summary>
        ///// 获取已经完成的节点记录
        ///// </summary>
        ///// <param name="runner"></param>
        ///// <returns></returns>
        //public IList<NodeImage> GetActivityInstanceCompleted(WfAppRunner runner)
        //{
        //    IList<NodeImage> imageList = new List<NodeImage>();
        //    var am = new ActivityInstanceManager();
        //    var list = am.GetCompletedActivityInstanceList(runner.AppInstanceID, runner.ProcessGUID);

        //    foreach (WfActivity a in list)
        //    {
        //        imageList.Add(new NodeImage 
        //        { 
        //            ID = a.ID, 
        //            ActivityName = a.ActivityName 
        //        });
        //    }
        //    return imageList;
        //}

        ///// <summary>
        ///// 获取当前活动实体
        ///// </summary>
        ///// <param name="processGUID"></param>
        ///// <param name="activityGUID"></param>
        ///// <returns></returns>
        //public ActivityEntity GetActivityEntity(string processGUID, string version, string activityGUID)
        //{
        //    var processModel = new XpdlModel(processGUID, version);
        //    return processModel.GetActivity(activityGUID);

        //}

        ///// <summary>
        ///// 获取活动节点下的角色数据
        ///// </summary>
        ///// <param name="processGUID"></param>
        ///// <param name="version"></param>
        ///// <param name="activityGUID"></param>
        ///// <returns></returns>
        //public IList<Role> GetActivityRoles(string processGUID, string version, string activityGUID)
        //{
        //    var processModel = new XpdlModel(processGUID, version);
        //    return processModel.GetActivityRoles(activityGUID);
        //}
        #endregion

        #region 流程跳转

        /// <summary>
        /// 流程跳转
        /// </summary>
        /// <param name="conn">连接</param>
        /// <param name="runner">执行操作人</param>
        /// <param name="trans">事务</param>
        /// <returns>跳转结果</returns>
        //public WfExecutedResult JumpProcess(WfAppRunner runner)
        //{
        //    try
        //    {
        //        WfExecutedResult jumpResult = null;
        //        var runtimeInstance = WorkflowRuntimeManagerFactory.CreateRuntimeInstanceJump(runner, ref jumpResult, _dataAccessor,_session,_loggerFactory,_sendService);

        //        if (jumpResult.Status == WfExecutedStatus.Exception)
        //        {
        //            return jumpResult;
        //        }

        //        runtimeInstance.OnWfProcessInstanceExecuted += delegate(object sender, WorkflowEventArgs args)
        //        {
        //            jumpResult = args.WfExecutedResult;
        //            waitHandler.Set();
        //        };

        //        bool isJumping = runtimeInstance.Execute();

        //        waitHandler.WaitOne();

        //        return jumpResult;
        //    }
        //    catch (System.Exception e)
        //    {
        //        WfExecutedResult jumpResult = new WfExecutedResult();
        //        jumpResult.Status = WfExecutedStatus.Failed;
        //        jumpResult.Message = string.Format("流程跳转时发生异常，详细错误:{0}", e.Message);
        //        return jumpResult;
        //    }

        //}

        //private void runtimeInstance_OnWfProcessInstanceJump(object sender, WfEventArgs args)
        //{
        //    _jumpResult = args.WfExecutedResult;
        //    waitHandler.Set();
        //}
        #endregion

        #region 流程撤销、回退和返签（已经结束的流程可以被复活）
        /// <summary>
        /// 撤销流程
        /// </summary>
        /// <param name="runner">撤销人runner</param>
        /// <returns>撤销结果</returns>
        public WfExecutedResult WithdrawProcess(WfAppRunner runner)
        {
            WfExecutedResult _withdrawedResult = null;
            try
            {
                var runtimeInstance = WfRuntimeManagerFactory.CreateRuntimeInstanceWithdraw(runner, _dataAccessor, _loggerFactory, ref _withdrawedResult);

                //不满足撤销操作，返回异常结果信息
                if (_withdrawedResult.Status != WfExecutedStatus.Exception)
                {

                    runtimeInstance.OnWfProcessExecuted += (sender, args) =>
                      {
                          _withdrawedResult = args.WfExecutedResult;
                          waitHandler.Set();
                      };
                    bool isWithdrawed = runtimeInstance.Execute();

                    waitHandler.WaitOne();
                }


                return _withdrawedResult;
            }
            catch (System.Exception e)
            {
                _withdrawedResult.Status = WfExecutedStatus.Failed;
                _withdrawedResult.Message = string.Format("流程撤销发生异常！，详细错误：{0}", e.Message);
                _logger.LogError(_withdrawedResult.Message);
            }
            return _withdrawedResult;
        }
        /// <summary>
        /// 回退
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        [Transactional]
        public WfExecutedResult SendBackProcess(WfAppRunner runner)
        {
            WfExecutedResult _sendbackResult = null;
            try
            {

                var runtimeInstance = WfRuntimeManagerFactory.CreateRuntimeInstanceSendBack(runner, _dataAccessor, _loggerFactory, ref _sendbackResult);

                if (_sendbackResult.Status != WfExecutedStatus.Exception)
                {
                    runtimeInstance.OnWfProcessExecuted += (sender, args) =>
                    {
                        _sendbackResult = args.WfExecutedResult;
                        waitHandler.Set();
                    };
                    bool isRejected = runtimeInstance.Execute();

                    waitHandler.WaitOne();
                }


                return _sendbackResult;
            }
            catch (System.Exception e)
            {
                _sendbackResult.Status = WfExecutedStatus.Failed;
                _sendbackResult.Message = string.Format("流程退回发生异常！，详细错误：{0}", e.Message);
                _logger.LogError(_sendbackResult.Message);
            }
            return _sendbackResult;
        }


        /// <summary>
        /// 流程返签
        /// </summary>
        /// <param name="conn">连接</param>
        /// <param name="ender">结束人</param>
        /// <param name="trans">事务</param>
        /// <returns>返签结果</returns>
        //public WfExecutedResult ReverseProcess(WfAppRunner ender)
        //{
        //    try
        //    {
        //        WfExecutedResult reversedResult = null;
        //        var runtimeInstance = WorkflowRuntimeManagerFactory.CreateRuntimeInstanceReverse(ender, ref reversedResult, _dataAccessor,_session,_loggerFactory,_sendService);

        //        if (reversedResult.Status == WfExecutedStatus.Exception)
        //        {
        //            return reversedResult;
        //        }

        //        runtimeInstance.OnWfProcessInstanceExecuted += delegate(object sender, WorkflowEventArgs args)
        //        {
        //            reversedResult = args.WfExecutedResult;
        //            waitHandler.Set();
        //        };
        //        bool isReversed = runtimeInstance.Execute();

        //        waitHandler.WaitOne();

        //        return reversedResult;
        //    }
        //    catch (System.Exception e)
        //    {
        //        WfExecutedResult reversedResult = new WfExecutedResult();
        //        reversedResult.Status = WfExecutedStatus.Failed;
        //        reversedResult.Message = string.Format("流程返签发生异常，详细错误:{0}", e.Message);
        //        return reversedResult;
        //    }
        //}

        /// <summary>
        /// 挂起流程实例
        /// </summary>
        /// <param name="processInstanceID"></param>
        /// <param name="activityInsId"></param>
        /// <param name="activityId"></param>
        /// <param name="runner"></param>
        /// <returns></returns>     
        [Transactional]
        public bool SuspendProcess(string processId, string comment)
        {
            bool result = true;
            try
            {
                var pim = new ProcessInstanceManager(_dataAccessor, _loggerFactory);
                pim.Suspend(processId);

                var tam = new TaskAdviceManager(_dataAccessor, _applicationContext, _loggerFactory);
                tam.RecordWhenSuspend(processId, comment);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 恢复流程
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        [Transactional]
        public bool ResumeProcess(string processId, string comment)
        {
            bool result = true;
            try
            {
                var pim = new ProcessInstanceManager(_dataAccessor, _loggerFactory);
                pim.Resume(processId);
                var tam = new TaskAdviceManager(_dataAccessor, _applicationContext, _loggerFactory);
                tam.RecordWhenResume(processId, comment);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 转办任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        [Transactional]
        public bool TurnTask(string taskId, WfAppRunner runner)
        {
            TaskManager tim = new TaskManager(_dataAccessor, _loggerFactory);
            WfTask task = tim.GetTask(taskId);
            if (task == null)
            {
                throw new FapException("任务不存在");
            }

            Performer turner = runner.Turners[0];
            ActivityInstanceManager aim = new ActivityInstanceManager(_dataAccessor, _loggerFactory);
            WfActivityInstance activity = aim.GetByFid(task.ActivityInsUid);
            activity.AssignedToUserIds = activity.AssignedToUserIds.Replace(runner.UserId, turner.UserId);
            activity.AssignedToUserNames = activity.AssignedToUserNames.Replace(runner.UserName, turner.UserName);
            aim.Update(activity);

            task.ExecutorEmpUid = turner.UserId;
            task.ExecutorEmpName = turner.UserName;
            task.UpdateBy = runner.UserId;
            task.UpdateName = runner.UserName;
            task.UpdateDate = DateTimeUtils.CurrentDateStr;
            tim.Update(task);

            TaskAdviceManager tam = new TaskAdviceManager(_dataAccessor, _applicationContext, _loggerFactory);
            tam.RecordWhenTurnTask(runner.ProcessUid, runner.CurrNodeId, runner.Comment, turner.UserId, turner.UserName);


            return true;
        }

        /// <summary>
        /// 指派代办任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        [Transactional]
        public bool AssignAgentTask(string taskId, WfAppRunner runner)
        {

            TaskManager tim = new TaskManager(_dataAccessor, _loggerFactory);
            WfTask task = tim.GetTask(taskId);
            if (task == null)
            {
                throw new FapException("任务不存在");
            }
            Performer agent = runner.Agents[0];

            TaskAdviceManager tam = new TaskAdviceManager(_dataAccessor, _applicationContext, _loggerFactory);
            tam.RecordWhenAssignAgentTask(runner.ProcessUid, runner.CurrNodeId, runner.Comment, agent.UserId, agent.UserName);

            task.AgentEmpUid = agent.UserId;
            task.AgentEmpName = agent.UserName;
            task.UpdateBy = runner.UserId;
            task.UpdateName = runner.UserName;
            task.UpdateDate = DateTimeUtils.CurrentDateTimeStr;
            tim.Update(task);

            return true;
        }

        #endregion

        #region 取消（运行的）流程、废弃执行中或执行完的流程


        /// <summary>
        /// 终止流程
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        [Transactional]
        public bool EndProcess(string processId, string comment)
        {
            var pim = new ProcessInstanceManager(_dataAccessor, _loggerFactory);
            pim.End(processId);

            TaskAdviceManager tam = new TaskAdviceManager(_dataAccessor, _applicationContext, _loggerFactory);
            tam.RecordWhenEnd(processId, comment);


            return true;
        }

        /// <summary>
        /// 删除流程
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        [Transactional]
        public bool DeleteProcess(string processId, string comment)
        {
            WfProcessInstance process = _dataAccessor.Get<WfProcessInstance>(processId);
            if (process != null)
            {
                process.ProcessState = WfProcessInstanceState.Deleted;
                _dataAccessor.Update<WfProcessInstance>(process);

                _dataAccessor.Execute("delete from wfTask where ProcessId='" + processId + "'");
                _dataAccessor.Execute("delete from WfActivityInstance where ProcessId='" + processId + "'");

                TaskAdviceManager tam = new TaskAdviceManager(_dataAccessor, _applicationContext, _loggerFactory);
                tam.RecordWhenDelete(processId, comment);


                return true;
            }
            return false;
        }
        /// <summary>
        /// 单据回写
        /// </summary>
        [Transactional]
        public void BillWriteBack()
        {
            //获取没有回写的流程实例
            IEnumerable<WfProcessInstance> processes = _dataAccessor.QueryWhere<WfProcessInstance>($"ProcessState='Completed' and ApproveResult='Agree' and (WriteBackState=0 or WriteBackState is null)");
            if (processes != null && processes.Any())
            {
                IWriteBackRule writeBackRule = new BillWriteBack(_dataAccessor);
                foreach (var process in processes)
                {
                    writeBackRule.WriteBackToBusiness(process.BillTable, process.BillUid);
                }
            }
        }
        /// <summary>
        /// 流程否决
        /// </summary>
        /// <param name="taskId"></param>
        //public bool RejectProcess(string taskId, string comment)
        //{
        //    bool result = false;
        //    _dataAccessor.DbTransaction(session =>
        //    {
        //        var tim = new TaskManager(_dataAccessor, session, _loggerFactory);
        //        WfTask task = tim.GetTask(taskId);
        //        if (!task.IsCanHandle)
        //        {
        //            result = false;
        //        }
        //        else
        //        {

        //            if (task != null)
        //            {
        //                _dataAccessor.Execute("update WfProcessInstance set ProcessState='" + WfProcessInstanceState.Completed + "', ApproveState='" + WfApproveState.Disagree + "' where Fid='" + task.ProcessInsUid + "'");
        //                _dataAccessor.Execute("update WfTask set TaskState='" + WfTaskState.Rejected + "' where Fid='" + task.Fid);
        //                _dataAccessor.Execute("update WfActivity set ActivityState='" + WfActivityInstanceState.Completed + "', ApproveState='" + WfApproveState.Disagree + "' where Fid='" + task.ActivityInsUid + "'");

        //                TaskAdviceManager tam = new TaskAdviceManager(_dataAccessor, session, _session, _loggerFactory);
        //                tam.RecordWhenReject(task.ProcessInsUid, task.Fid, comment);

        //                result = true;
        //            }
        //            else
        //            {
        //                result = false;
        //            }
        //        }

        //    });
        //    return result;
        //}
        #endregion

        #region 任务读取和处理
        ///// <summary>
        ///// 设置任务为已读状态(根据任务ID获取任务)
        ///// </summary>
        ///// <param name="runner">执行人</param>
        ///// <returns>任务读取的标志</returns>
        //public bool SetTaskRead(WfAppRunner taskRunner)
        //{
        //    bool isRead = false;
        //    try
        //    {
        //        var taskManager = new TaskManager();
        //        taskManager.SetTaskRead(taskRunner);
        //        isRead = true;
        //    }
        //    catch (System.Exception)
        //    {
        //        throw;
        //    }

        //    return isRead;
        //}

        ///// <summary>
        ///// 获取运行中的任务
        ///// </summary>
        ///// <param name="query">查询实体</param>
        ///// <returns>任务列表</returns>
        //public IList<TaskViewEntity> GetRunningTasks(TaskQueryEntity query)
        //{
        //    int allRowsCount = 0;
        //    var taskManager = new TaskManager();
        //    var taskList = taskManager.GetRunningTasks(query, out allRowsCount);
        //    if (taskList != null)
        //        return taskList.ToList();
        //    else
        //        return null;
        //}

        /// <summary>
        /// 获取任务（分页）
        /// </summary>
        /// <param name="query">查询对象</param>
        /// <returns>任务列表</returns>
        //public PageDataView<WfTask> GetPagedTasks(PageCriteria criteria)
        //{
        //    return new TaskManager(_dataAccessor, _loggerFactory).GetPagedTasks(criteria);
        //}

        /// <summary>
        /// 获取任务列表
        /// </summary>
        /// <param name="taskQueryEntity"></param>
        /// <returns></returns>
        public IEnumerable<TaskViewModel> GetTasks(TaskQueryEntity taskQueryEntity)
        {
            string sql = @"select a.Fid as TaskId, a.TaskName, a.TaskState, a.TaskType, a.ExecutorName, 
            a.ExecutorRealName, a.TaskStartTime, a.TaskEndTime, b.Fid as ProcessId, b.ProcessName, b.StarterName, b.StarterRealName  
            from WfTask a, WfProcessInstance b where a.ProcessId=b.Fid  ";

            if (taskQueryEntity != null)
            {
                if (!string.IsNullOrWhiteSpace(taskQueryEntity.TaskState))
                {
                    sql += " and a.TaskState='" + taskQueryEntity.TaskState + "'";
                }
                else if (!string.IsNullOrWhiteSpace(taskQueryEntity.StarterId))
                {
                    sql += " and b.StarterName='" + taskQueryEntity.StarterId + "'";
                }
                else if (!string.IsNullOrWhiteSpace(taskQueryEntity.ExecutorId))
                {
                    sql += " and a.ExecutorName='" + taskQueryEntity.ExecutorId + "'";
                }
            }

            sql += " order by a.Fid desc";
            IEnumerable<TaskViewModel> model = _dataAccessor.QueryOriSql<TaskViewModel>(sql);
            return model;
        }

        /// <summary>
        /// 获取指定人员的可办理的任务
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<WfTask> GetTodoTaskList(string userId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ExecutorName", userId);
            IEnumerable<WfTask> tasks = _dataAccessor.QueryWhere<WfTask>("ExecutorName=@ExecutorName AND TaskState IN ('Waiting', 'Handling')", parameters);
            return tasks;
        }
        #endregion


        /// <summary>
        /// 获取流程实例
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        public WfProcessInstance GetProcessInstance(string processInsId)
        {
            WfProcessInstance process = _dataAccessor.Get<WfProcessInstance>(processInsId);
            return process;
        }




        /// <summary>
        /// 发送催办消息
        /// </summary>
        /// <param name="billUid">业务数据ID</param>
        /// <param name="businessUid">业务类型Uid</param>
        /// <returns></returns>
        public string UrgeFlow(string billUid, string businessUid)
        {
            MessageManager messageManager = new MessageManager(_dataAccessor, _loggerFactory);
            return messageManager.SendUrgeMessage(billUid, businessUid);
        }
    }
}
