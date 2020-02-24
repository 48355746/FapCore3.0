using Dapper;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Xpdl.Entity;
using Fap.Workflow.Model;
using System;
using Microsoft.Extensions.Logging;
using Fap.Workflow.Engine.Exceptions;
using Fap.Workflow.Engine.WriteBack;
using Fap.Workflow.Engine.Message;
using Fap.Core.DataAccess;
using Fap.Core.Utility;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Extensions;

namespace Fap.Workflow.Engine.Manager
{
    internal class ProcessInstanceManager : ManagerBase
    {
        private readonly ILogger<ProcessInstanceManager> _logger;

        public ProcessInstanceManager(IDbContext dataAccessor,ILoggerFactory loggerFactory) : base(dataAccessor,  loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ProcessInstanceManager>();
        }

        /// <summary>
        /// 根据GUID获取流程实例数据
        /// </summary>
        /// <param name="processInstanceID"></param>
        /// <returns></returns>
        internal WfProcessInstance GetByFid(string processInstanceID)
        {
            return _dataAccessor.Get<WfProcessInstance>(processInstanceID, false);
        }
        /// <summary>
        /// 检查子流程是否结束
        /// </summary>
        /// <param name="activityInstanceID">活动实例ID</param>
        /// <param name="activityGUID">活动GUID</param>
        /// <returns>是否结束标志</returns>
        internal bool CheckSubProcessInstanceCompleted(string activityInstanceUid, string activityId)
        {
            bool isCompleted = false;
            //DynamicParameters param = new DynamicParameters();
            //param.Add("invokedActivityGUID", activityId);
            //param.Add("invokedActivityInstanceID", activityInstanceUid);
            //var list = _dataAccessor.Query<WfProcessInstance>(
            //          @"SELECT * FROM WfProcessInstance
            //                    WHERE InvokedActivityInstanceID=@invokedActivityInstanceID 
            //                        AND InvokedActivityGUID=@invokedActivityGUID 
            //                        AND RecordStatusInvalid=0
            //                        AND ProcessState=4
            //                    ORDER BY CreatedDateTime DESC",
            //          param
            //     );

            //if (list.Count()== 1)
            //{
            //    isCompleted = true;
            //}

            return isCompleted;
        }
        /// <summary>
        /// 根据任务ID，获取流程实例
        /// </summary>
        /// <param name="taskUid"></param>
        /// <returns></returns>
        internal WfProcessInstance GetByTaskId(string taskUid)
        {
            WfTask wtsk = _dataAccessor.Get<WfTask>(taskUid, false);

            WfProcessInstance processInstance = _dataAccessor.Get<WfProcessInstance>(wtsk.ProcessInsUid, false);
            return processInstance;
        }

        internal WfProcessInstance GetRunningProcessInstance(string processUid, string bizUid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ProcessUid", processUid);
            parameters.Add("BizUid", bizUid);
            return _dataAccessor.QueryFirstOrDefaultWhere<WfProcessInstance>($"ProcessUid=@ProcessUid and BizUid=@BizUid and ProcessState='{WfProcessInstanceState.Running}'", parameters, false);

        }

        /// <summary>
        /// 创建新流程实例，根据流程模板
        /// </summary>
        /// <param name="runner"></param>
        /// <param name="func"></param>
        /// <param name="service"></param>
        /// <returns></returns>
        internal WfProcessInstance CreateProcessInstance(WfAppRunner runner, WfProcessInstance parentProcessInstance,
            WfActivityInstance subProcessNode, Action<WfAppRunner, WfProcessInstance, WfProcess> func)
        {
            WfProcess wfProcess = _dataAccessor.Get<WfProcess>(runner.ProcessUid, false);
            WfProcessInstance wfProcessInstance = new WfProcessInstance();
            wfProcessInstance.Fid = UUIDUtils.Fid;;
            wfProcessInstance.ProcessUid = wfProcess.Fid;
            wfProcessInstance.BizUid = runner.BillUid;
            wfProcessInstance.BizName = runner.BillData.BillCode;//易标识
            wfProcessInstance.AppEmpUid = runner.UserId;
            wfProcessInstance.AppEmpName = runner.UserName;
            wfProcessInstance.StartTime =DateTimeUtils.CurrentDateStr;
            wfProcessInstance.ProcessState = WfProcessInstanceState.Running;
            wfProcessInstance.ProcessName = wfProcess.ProcessName;
            wfProcessInstance.ProcessDesc = wfProcess.Description;
            wfProcessInstance.IsHasForm = 1;
            wfProcessInstance.BizTypeUid = runner.BusinessUid;           
            wfProcessInstance.IsHandling = 0; //未处理状态
            wfProcessInstance.MessageSetting = wfProcess.MessageSetting;
            wfProcessInstance.FormTemplateUid = wfProcess.FormTemplateUid;
            wfProcessInstance.FormType = wfProcess.FormType;
            wfProcessInstance.BillTable = wfProcess.BillTable;
            if (parentProcessInstance != null)
            {
                //流程的Parent信息
                wfProcessInstance.ParentProcessInsUid = parentProcessInstance.Fid;
                wfProcessInstance.ParentProcessUid = parentProcessInstance.ProcessUid;
                wfProcessInstance.InvokeActivityInsUid = subProcessNode.Fid;
                wfProcessInstance.InvokeActivityUid = subProcessNode.NodeId.ToInt();
            }
            if (func != null)
            {
                func(runner, wfProcessInstance, wfProcess);
            }

            return wfProcessInstance;
        }
        internal WfProcessInstance Insert(WfProcessInstance wfProcessInstance)
        {
            long id = _dataAccessor.Insert<WfProcessInstance>(wfProcessInstance);
            wfProcessInstance.Id = id;
            return wfProcessInstance;
        }
        /// <summary>
        /// 获取流程实例
        /// </summary>
        /// <param name="processUid">流程</param>
        /// <param name="bizUid">业务</param>
        /// <returns></returns>
        internal WfProcessInstance GetProcessInstance(string processUid, string bizUid)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ProcessUid", processUid);
            parameters.Add("BizUid", bizUid);
            return _dataAccessor.QueryFirstOrDefaultWhere<WfProcessInstance>(@"ProcessUid=@ProcessUid and BizUid=@BizUid", parameters, false);
        }

        /// <summary>
        /// 获取流程的发起人
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        internal Performer GetProcessInitiator(string processId)
        {
            var entity = _dataAccessor.Get<WfProcessInstance>(processId, false);
            Performer performer = new Performer() { UserId = entity.AppEmpUid, UserName = entity.AppEmpName };
            return performer;
        }

        /// <summary>
        /// 获取最近的流程运行实例
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        internal WfProcessInstance GetProcessInstanceLatest(string processId, string bizUid)
        {
            WfProcessInstance instance = GetProcessInstance(processId, bizUid);
            return instance;
        }

        #region 流程业务规则处理
    
        /// <summary>
        /// 撤销流程
        /// </summary>
        /// <param name="runner"></param>
        internal void Withdrawn(WfAppRunner runner)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("ProcessInsUid", runner.CurrProcessInsUid);
            //删除任务
            _dataAccessor.DeleteExec("WfTask", "ProcessInsUid=@ProcessInsUid", param);
            //删任务实例
            _dataAccessor.DeleteExec("WfActivityInstance", " ProcessInsUid=@ProcessInsUid", param);
            //删流程实例
            _dataAccessor.DeleteExec("WfProcessInstance", "Fid=@ProcessInsUid", param);
            //更改业务数据单据状态为草稿态
            string sql = $"update {runner.BillTableName} set BillStatus='{BillStatus.DRAFT}' where Fid='{runner.BillUid}'";
            _dataAccessor.Execute(sql, null);
        }
        #region 流程完成（通过。驳回）
        /// <summary>
        /// 流程完成，设置流程的状态为完成
        /// </summary>
        /// <param name="processInsUid">流程实例</param>
        /// <returns>是否成功</returns>
        internal void Complete(string processInsUid, string bizUid)
        {
            var processIns = _dataAccessor.Get<WfProcessInstance>(processInsUid, false);
            if (WfProcessInstanceState.Running == processIns.ProcessState)
            {
                processIns.ProcessState = WfProcessInstanceState.Completed;
                processIns.EndTime =DateTimeUtils.CurrentDateTimeStr;
                _dataAccessor.Update<WfProcessInstance>(processIns);
                string billTable = processIns.BillTable;

                MessageManager msgManager = new MessageManager(_dataAccessor, _loggerFactory);
                msgManager.SendMessageWhenProcessCompleted(processIns, "通过");
               
                IWriteBackRule bwb = new BillWriteBack(_dataAccessor);
                //改变单据状态为通过
                bwb.Approved(billTable, bizUid);
                //单据回写业务
                bwb.WriteBackToBusiness(billTable, bizUid);
            }
            else
            {
                _logger.LogWarning($"{processInsUid}流程不在运行状态，不能被完成！");
                throw new ProcessInstanceException("流程不在运行状态，不能被完成！");
            }
        }
        /// <summary>
        /// 驳回流程，将流程状态置为驳回
        /// </summary>
        /// <param name="processInsUid"></param>
        internal void Revoke(string processInsUid)
        {
            var processIns = _dataAccessor.Get<WfProcessInstance>(processInsUid, false);
            if (processIns.ProcessState == WfProcessInstanceState.Running)
            {
                processIns.ProcessState = WfProcessInstanceState.Revoked;
                processIns.ApproveResult = WfApproveState.Disagree;
                //processIns.IsHandling = 0;
                _dataAccessor.Update<WfProcessInstance>(processIns);

                MessageManager msgManager = new MessageManager(_dataAccessor, _loggerFactory);
                msgManager.SendMessageWhenProcessCompleted(processIns, "驳回");

                IWriteBackRule bwb = new BillWriteBack(_dataAccessor);
                //改变单据状态为驳回
                bwb.Rejected(processIns.BillTable, processIns.BizUid);

            }
            else
            {
                throw new ProcessInstanceException("流程不在运行状态，不能被驳回！");
            }
        }

        #endregion

        /// <summary>
        /// 挂起流程实例
        /// </summary>
        /// <param name="processInsUid">流程实例</param>
        internal void Suspend(string processInsUid)
        {
            var processInstance = _dataAccessor.Get<WfProcessInstance>(processInsUid, false);

            if (processInstance.ProcessState == WfProcessInstanceState.Running)
            {
                processInstance.ProcessState = WfProcessInstanceState.Suspended;


                _dataAccessor.Update<WfProcessInstance>(processInstance);
                MessageManager mm = new MessageManager(_dataAccessor, _loggerFactory);
                mm.SendMessageWhenProcessExcept(processInstance);
            }
            else
            {
                throw new ProcessInstanceException("流程不在运行状态，不能被挂起！");
            }
        }

        /// <summary>
        /// 恢复流程实例
        /// </summary>
        /// <param name="processInsUid">流程实例</param>
        internal void Resume(string processInsUid)
        {
            var bEntity = _dataAccessor.Get<WfProcessInstance>(processInsUid, false);
            if (bEntity.ProcessState == WfProcessInstanceState.Suspended)
            {
                bEntity.ProcessState = WfProcessInstanceState.Running;

                _dataAccessor.Update<WfProcessInstance>(bEntity);
            }
            else
            {
                throw new ProcessInstanceException("流程不在挂起状态，不能被恢复！");
            }
        }

        //        /// <summary>
        //        /// 恢复子流程
        //        /// </summary>
        //        /// <param name="processInstanceID"></param>
        //        /// <param name="runner"></param>
        //        /// <param name="session"></param>
        //        internal void RecallSubProcess(string invokedActivityId, WorkflowRunner runner)
        //        {
        //            _dataAccessor.QueryFirstOrDefaultEntityBySql<WfProcessInstance>();

        //            var list = Repository.Query<WfProcessInstance>(
        //                   session.Connection,
        //                   @"SELECT * FROM WfProcessInstanceInstance
        //                                WHERE InvokedActivityInstanceID=@invokedActivityInstanceID 
        //                                    AND ProcessState=5
        //                                ORDER BY CreatedDateTime DESC",
        //                   new
        //                   {
        //                       invokedActivityInstanceID = invokedActivityInstanceID
        //                   },
        //                   session.Transaction).ToList();

        //            if (list != null && list.Count() == 1)
        //            {
        //                var bEntity = list[0];

        //                bEntity.ProcessState = (short)ProcessStateEnum.Running;
        //                bEntity.LastUpdatedDateTime = System.DateTime.Now;
        //                bEntity.LastUpdatedByUserID = runner.UserID;
        //                bEntity.LastUpdatedByUserName = runner.UserName;

        //                Update(bEntity, session);
        //            }
        //            else
        //            {
        //                throw new ProcessInstanceException("流程不在挂起状态，不能被完成！");
        //            }
        //        }


        /// <summary>
        /// 返签流程，将流程状态置为返签，并修改流程未完成标志
        /// </summary>
        /// <param name="processId"></param>
        internal void Reverse(string processInsUid)
        {
            var bEntity = _dataAccessor.Get<WfProcessInstance>(processInsUid, false);
            if (bEntity.ProcessState == WfProcessInstanceState.Completed)
            {
                bEntity.ProcessState = WfProcessInstanceState.Running;

                _dataAccessor.Update<WfProcessInstance>(bEntity);
            }
            else
            {
                throw new ProcessInstanceException("流程不在运行状态，不能被返签！");
            }
        }

     



        /// <summary>
        /// 终止单据下所有流程的信息
        /// </summary>
        /// <returns></returns>
        internal void End(string processId)
        {
            //string sql = "SELECT * FROM WfProcessInstance WHERE Fid=@Fid and ProcessState in ('Running', 'Suspended')";
            //DynamicParameters parameter = new DynamicParameters();
            //parameter.Add("@Fid", processId);
            //WfProcessInstance process = _dataAccessor.QueryFirstOrDefaultEntityBySql<WfProcessInstance>(sql, parameter);
            //if (process != null)
            //{
            //    process.ProcessState = WfProcessInstanceState.Discarded;
            //    //process.UpdateBy = runner.UserId;
            //    //process.UpdateName = runner.UserName;
            //    _dataAccessor.UpdateEntity<WfProcessInstance>(process);
            //}
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("Fid", processId);

            string sql = $"UPDATE WfProcessInstance SET ProcessState='{ WfProcessInstanceState.Ended }'  WHERE Fid=@Fid and ProcessState in ('{WfProcessInstanceState.Suspended}')  ";
            _dataAccessor.Execute(sql, parameters);

           // _dataAccessor.Execute("update WfTask set TaskState='" + WfTaskState.Ended + "' where TaskState IN('" + WfTaskState.Handling + "', '" + WfTaskState.Waiting + "') and ProcessId='" + processId + "'  ", null);
            //_dataAccessor.Execute("update WfActivity set ActivityState='" + WfActivityInstanceState.Ended + "', ApproveState='" + WfApproveState.Disagree + "' where ActivityState IN('" + WfActivityInstanceState.Ready + "', '" + WfActivityInstanceState.Running + "') and ProcessId='" + processId + "'  ", null);

        }

    

        ///// <summary>
        ///// 删除不正常的流程实例（流程在取消状态，才可以进行删除！）
        ///// </summary>
        ///// <param name="processId"></param>
        ///// <returns></returns>
        //internal bool Delete(string processId)
        //{
        //    bool isDeleted = false;

        //    WfProcessInstance entity = _dataAccessor.Get<WfProcessInstance>(processId);

        //    if (entity.ProcessState == WfProcessInstanceState.Canceled)
        //    {
        //        _dataAccessor.DeleteEntity<WfProcessInstance>(entity);
        //        isDeleted = true;
        //    }
        //    else
        //    {
        //        throw new ProcessInstanceException("流程只有先取消，才可以删除！");
        //    }

        //    return isDeleted;

        //}
        #endregion


    }
}
