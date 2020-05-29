using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Fap.Core.Rbac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Fap.Workflow.Model;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Service;
using Fap.Workflow.Engine.Xpdl;
using Fap.Core.Extensions;
using Dapper;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using System.Net.Mime;
using System;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Utility;
using Ardalis.GuardClauses;

namespace Fap.Hcm.Web.Areas.Workflow.Controllers
{
    [Area("Workflow")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[area]/Api/Business")]
    public class BusinessApiController : FapController
    {
        private readonly IWorkflowService _workflowService;

        public BusinessApiController(IServiceProvider serviceProvider, IWorkflowService workflowService) : base(serviceProvider)
        {
            _workflowService = workflowService;
        }       
        /// <summary>
        /// 提交流程（申请人）
        /// </summary>
        /// <returns></returns>
        [HttpPost("SubmitProcess")]
        public JsonResult SubmitProcess(WfViewModel wfvm)
        {
            Guard.Against.Null(wfvm, nameof(WfViewModel));
            var formData = _dbContext.Get(wfvm.BillTable,wfvm.BillUid,true);          
            string appEmpUid = formData.AppEmpUid;
            string appEmpName = formData.AppEmpUidMC;
            WfAppRunner runner = new WfAppRunner();
            runner.BillTableName = wfvm.BillTable;
            runner.ProcessUid = wfvm.ProcessUid;
            runner.BillData = formData;
            runner.BillUid = wfvm.BillUid;
            runner.UserId = appEmpUid;
            runner.UserName = appEmpName;
            runner.AppName = wfvm.AppName;
            runner.BusinessUid = wfvm.BusinessUid;
            ActivityEntity ae = new ActivityEntity();
            ae.ActivityID = wfvm.AvtivityId;
            ae.Performers = wfvm.NextPerformers;
            runner.NextActivity = ae;
            WfExecutedResult result = _workflowService.StartProcess(runner);
            return Json(result);
        }
        [HttpPost("RunProcess")]
        public JsonResult RunProcess(WfViewModel wfvm)
        {
            Guard.Against.Null(wfvm, nameof(WfViewModel));
            var formData = _dbContext.Get(wfvm.BillTable, wfvm.BillUid, true);
            WfAppRunner runner = new WfAppRunner();
            runner.BillTableName = wfvm.BillTable;
            runner.ProcessUid = wfvm.ProcessUid;
            runner.BillData = formData;
            runner.BillUid = wfvm.BillUid;
            runner.UserId = _applicationContext.EmpUid;
            runner.UserName = _applicationContext.EmpName;
            runner.AppName = wfvm.AppName;
            runner.BusinessUid = wfvm.BusinessUid;
            runner.CurrActivityInsUid = wfvm.CurrentActivityInsUid;
            runner.CurrProcessInsUid = wfvm.CurrentProcessInsUid;
            runner.CurrWfTaskUid = wfvm.CurrentWfTaskUid;
            runner.CurrNodeId = wfvm.CurrentNodeId;
            runner.ApproveState = wfvm.ApproveState;// WfApproveState.Agree
            runner.Comment = wfvm.ApproveComment;
            if (wfvm.AvtivityId.IsPresent())
            {
                ActivityEntity ae = new ActivityEntity();
                ae.ActivityID = wfvm.AvtivityId;
                ae.Performers = wfvm.NextPerformers;
                runner.NextActivity = ae;
            }
            WfExecutedResult result = _workflowService.RunProcess(runner);

            return Json(result);
        }
        /// <summary>
        /// 作废单据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Invalid/{tablename}/{fid}")]
        public JsonResult GetInvalidBill(string tableName, string fid)
        {
            string sql = $"update {tableName} set BillStatus='{BillStatus.CANCELED}' where Fid=@Fid";
            _dbContext.Execute(sql, new DynamicParameters(new { Fid = fid }));
            return Json(ResponseViewModelUtils.Sueecss());
        }
        /// <summary>
        /// 撤回
        /// </summary>
        /// <param name="bizUid"></param>
        /// <param name="processUid"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Withdrawn/{processUid}/{billUid}")]
        public JsonResult GetWithdrawnBill(string billUid, string processUid)
        {
            string sql = $"select * from WfProcessInstance where ProcessUid=@ProcessUid and BillUid=@BillUid and ProcessState='{WfProcessInstanceState.Running}'";
            var wpins = _dbContext.QueryFirstOrDefault<WfProcessInstance>(sql, new DynamicParameters(new { ProcessUid = processUid, BillUid = billUid }));
            if (wpins == null)
            {
                return Json(ResponseViewModelUtils.Failure("撤回失败，实例不存在或已完成。"));
            }
            else
            {
                WfAppRunner runner = new WfAppRunner();
                runner.CurrProcessInsUid = wpins.Fid;
                runner.BillTableName = wpins.BillTable;
                runner.BillUid = wpins.BillUid;
                _workflowService.WithdrawProcess(runner);
            }
            return Json(ResponseViewModelUtils.Sueecss());
        }
        /// <summary>
        /// 回退单据（驳回）
        /// </summary>
        /// <param name="bizUid"></param>
        /// <param name="processUid"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SendBack")]
        public JsonResult PostSendBackBill(WfViewModel wfvm)
        {
            string sql = "select * from WfProcessInstance where Fid=@ProcessInsUid";
            var wpins = _dbContext.QueryFirstOrDefault<WfProcessInstance>(sql, new DynamicParameters(new { ProcessInsUid = wfvm.CurrentProcessInsUid}));
            WfExecutedResult result = null;
            if (wpins == null)
            {
                result = new WfExecutedResult() { Status = WfExecutedStatus.Exception, Message = "不存在流程实例，不需要驳回" };
            }
            else
            {
                WfAppRunner runner = new WfAppRunner();
                runner.BillTableName = wfvm.BillTable;
                runner.ProcessUid = wfvm.ProcessUid;
                runner.BillUid = wfvm.BillUid;
                runner.UserId = _applicationContext.EmpUid;
                runner.UserName = _applicationContext.EmpName;
                runner.AppName = wfvm.AppName;
                runner.BusinessUid = wfvm.BusinessUid;
                runner.CurrActivityInsUid = wfvm.CurrentActivityInsUid;
                runner.CurrProcessInsUid = wfvm.CurrentProcessInsUid;
                runner.CurrWfTaskUid = wfvm.CurrentWfTaskUid;
                runner.CurrNodeId = wfvm.CurrentNodeId;
                runner.ApproveState = wfvm.ApproveState;// WfApproveState.Agree
                runner.Comment = wfvm.ApproveComment;
                result = _workflowService.SendBackProcess(runner);

            }
            return Json(result);
        }
        [HttpGet]
        [Route("UrgeBill")]
        public JsonResult UrgeBill(string billUid,string businnessUid)
        {
            ResponseViewModel rvm = new ResponseViewModel();
            rvm.msg= _workflowService.UrgeFlow(billUid, businnessUid);
            return Json(rvm);
        }
        /// <summary>
        /// 获取第一个审批节点
        /// </summary>
        /// <param name="bizUid">业务Id</param>
        /// <param name="processUid">流程</param>
        /// <returns>活动实体</returns>
        [HttpGet]
        [Route("FirstAcitvity")]
        public JsonResult GetFirstAcitvity(string billUid, string processUid)
        {

            var firstEntity = _workflowService.GetFirstActivity(processUid, billUid);

            return Json(firstEntity);

        }
        /// <summary>
        /// 获取下一个审批节点
        /// </summary>
        /// <param name="billUid"></param>
        /// <param name="processUid"></param>
        /// <param name="activityInsUid"></param>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("NextAcitvity")]
        public JsonResult GetNextAcitvity(string billUid, string processUid, string activityInsUid, string nodeId)
        {
            if (_workflowService.NodeIsComplete(activityInsUid))
            {
                var activityEntity = _workflowService.GetNextActivity(processUid, nodeId, billUid);
                return Json(new { result = 1, data = activityEntity });
            }
            else
            {
                return Json(new { result = 0 });
            }

        }
    }
}