using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Fap.AspNetCore.Infrastructure;
using Fap.Workflow.Model;
using Microsoft.AspNetCore.Mvc;
using Fap.Core.Extensions;
using Fap.AspNetCore.ViewModel;
using Fap.Core.Infrastructure.Query;
using Fap.Workflow.Engine.Xpdl;
using Newtonsoft.Json;
using Fap.Core.Infrastructure.Metadata;
using Ardalis.GuardClauses;

namespace Fap.Hcm.Web.Areas.Workflow.Controllers
{
    [Area("Workflow")]
    public class BusinessController : FapController
    {
        public BusinessController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// 我的申请
        /// </summary>
        /// <returns></returns>
        public IActionResult MyApply()
        {
            //获取审批业务
            IEnumerable<WfBusiness> bizTypes = _dbContext.QueryWhere<WfBusiness>($"{nameof(WfBusiness.BizStatus)}=1",null,true);
            return View(bizTypes);
        }
        /// <summary>
        /// 申请列表
        /// </summary>
        /// <param name="id">业务类型fid</param>
        /// <returns></returns>
        public PartialViewResult Apply(string fid)
        {
            WfBusiness business = _dbContext.Get<WfBusiness>(fid);
            WfProcess wfProcess = _dbContext.Get<WfProcess>(business.WfProcessUid);
            ViewBag.WfProcess = wfProcess;
            ViewBag.WfBusiness = business;
            var model = this.GetJqGridModel(business.BillEntity, (qs) =>
            {
                qs.GlobalWhere = "(BillEmpUid=@EmpUid or AppEmpUid=@EmpUid)";
                qs.AddParameter("EmpUid", _applicationContext.EmpUid);
            });
            return PartialView(model);
        }
        /// <summary>
        /// 申请单
        /// </summary>
        /// <param name="processUid">流程</param>
        /// <param name="billUid">单据</param>
        /// <returns></returns>
        public IActionResult ApplyBill(string businessUid, string billUid = "")
        {
            WfBusiness business = _dbContext.Get<WfBusiness>(businessUid);
            WfProcess process = _dbContext.Get<WfProcess>(business.WfProcessUid);
            FormViewModel model = GetFormViewModel(business.BillEntity, business.BillEntity, billUid);            
            if (process.BillTable.EqualsWithIgnoreCase("TmTravelApply"))
            {
                //设置子表默认值
                SubTableDefaultValue sub = new SubTableDefaultValue() { TableName = "TmTravelBudget", Data = new Dictionary<string, string> { ["Currency"] = "fcdc11e5828cc236d2ab", ["CurrencyMC"] = "人民币" } };
                model.SubDefaultDataList.AsList().Add(sub);
            }
            //关联的业务类型Uid
            ViewBag.Business = business;
            //当前流程
            ViewBag.Process = process;
            return View(model);
        }
        /// <summary>
        /// 查看申请单
        /// </summary>
        /// <param name="processUid"></param>
        /// <param name="businessUid">业务</param>
        /// <param name="billUid">单据</param>
        /// <returns></returns>
        public IActionResult ApplyViewBill(string processUid, string businessUid, string billUid)
        {
            WfProcess process = _dbContext.Get<WfProcess>(processUid);
            FormViewModel model = this.GetFormViewModel(process.BillTable, "frm-" + process.BillTable, billUid);
          
            model.FormStatus = AspNetCore.Controls.DataForm.FormStatus.View;

            //关联的业务类型Uid
            ViewBag.BusinessUid = businessUid;
            //当前流程
            ViewBag.Process = process;
            //获取审批历史
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ProcessUid", processUid);
            parameters.Add("BillUid", billUid);
            var activityIns = _dbContext.QueryWhere<WfActivityInstance>("ActivityType<>'GatewayNode' and ProcessUid=@ProcessUid and BillUid=@BillUid", parameters, true).OrderBy(a => a.StartTime);
            var tasks = _dbContext.QueryWhere<WfTask>("ProcessUid=@ProcessUid and BillUid=@BillUid", parameters, true).OrderBy(t => t.TaskStartTime).OrderBy(t => t.ExecuteTime);
            ViewBag.ActivityIns = activityIns;
            ViewBag.WfTasks = tasks;

            return View(model);
        }
        /// <summary>
        /// 审批列表
        /// </summary>
        /// <returns></returns>
        public PartialViewResult Approval(string businessUid, string billTable, int agent = 0)
        {
            var model = this.GetJqGridModel(billTable, qs =>
            {
                if (agent == 1)
                {
                    //获取代理人(状态可用且代理此业务，代理业务为null时，默认代理所有业务)
                    var agents = _dbContext.QueryAll<WfAgentSetting>().Where(a => a.Agent == _applicationContext.EmpUid && a.State == 1 && (a.BusinessUid.IsMissing() || a.BusinessUid == businessUid));// $" Agent='{_applicationContext.EmpUid}' and [State]=1");

                    if (agents.Any())
                    {
                        qs.GlobalWhere = $"Fid in (select WfTask.BillUid from WfTask,WfActivityInstance,WfProcessInstance where WfTask.ActivityInsUid=WfActivityInstance.Fid and WfTask.ProcessInsUid= WfProcessInstance.Fid and  WfProcessInstance.ProcessState='Running' and  WfActivityInstance.ActivityState in('{WfActivityInstanceState.Running}','{WfActivityInstanceState.Ready}') and WfTask.TaskState='{WfTaskState.Handling}' and WfTask.ExecutorEmpUid in({string.Join(',', agents.Select(a => $"'{a.Principal}'"))}) and WfTask.BusinessUid='{businessUid}' )";

                    }
                    else
                    {
                        qs.GlobalWhere = "1=2";
                    }
                }
                else
                {
                    qs.GlobalWhere = $"Fid in (select WfTask.BillUid from WfTask,WfActivityInstance,WfProcessInstance where WfTask.ActivityInsUid=WfActivityInstance.Fid and WfTask.ProcessInsUid= WfProcessInstance.Fid and  WfProcessInstance.ProcessState='Running' and  WfActivityInstance.ActivityState in('{WfActivityInstanceState.Running}','{WfActivityInstanceState.Ready}') and WfTask.TaskState='{WfTaskState.Handling}' and WfTask.ExecutorEmpUid='{_applicationContext.EmpUid}' and WfTask.BusinessUid='{businessUid}' )";
                }
            });
            ViewBag.Agent = agent;
            var bizs = _dbContext.QueryFirstOrDefaultWhere<WfBusiness>("Fid=@BusinessUid and BizStatus=1", new DynamicParameters(new { BusinessUid = businessUid }), true);
            ViewBag.Business = bizs;
            return PartialView(model);
        }
        /// <summary>
        /// 已办理任务
        /// </summary>
        /// <param name="BusinessUid"></param>
        /// <param name="billTable"></param>
        /// <returns></returns>
        public PartialViewResult Approvaled(string businessUid, string billTable)
        {
            var model = this.GetJqGridModel(billTable, qs =>
            {
                qs.GlobalWhere = $"Fid in (select BillUid from WfTask where TaskState='{WfTaskState.Completed}' and (ExecutorEmpUid='{_applicationContext.EmpUid}' or AgentEmpUid='{_applicationContext.EmpUid}') and BusinessUid='{businessUid}' )";

            });
            var bizs = _dbContext.QueryFirstOrDefaultWhere<WfBusiness>("Fid=@BusinessUid and BizStatus=1", new DynamicParameters(new { BusinessUid = businessUid }), true);
            ViewBag.Business = bizs;
            return PartialView(model);
        }
        /// <summary>
        /// 流程图
        /// </summary>
        /// <returns></returns>
        public IActionResult FlowChart(string processUid, string billUid)
        {
            //多个实例的时候 取最后的实例，因为存在驳回再提交的情况
            var processInstance = _dbContext.QueryWhere<WfProcessInstance>("ProcessUid=@ProcessUid and BillUid=@BillUid", new DynamicParameters(new { ProcessUid = processUid, BillUid = billUid })).OrderByDescending(p => p.Id).FirstOrDefault();

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("ProcessInsUid", processInstance.Fid);
            var diagramInstance = _dbContext.QueryFirstOrDefaultWhere<WfDiagramInstance>("ProcessInsUid=@ProcessInsUid", parameters);
            if (diagramInstance == null)
            {
                return Content("未找到流程图");
            }
            var activityIns = _dbContext.QueryWhere<WfActivityInstance>("ProcessInsUid=@ProcessInsUid  and ActivityType not in('StartNode','GatewayNode')", parameters);
            //var transitionIns = _dbContext.QueryWhere<WfTransitionInstance>(" ProcessInsUid=@ProcessInsUid", parameters);

            var tasks = _dbContext.QueryWhere<WfTask>("ProcessInsUid=@ProcessInsUid", parameters, true);
            var updateActivityStates = activityIns.Select(a => $"<update id=\"{a.NodeId}\" state=\"{a.ActivityState}\" tooltips=\"{string.Join(",", tasks.Where(t => t.ActivityInsUid == a.Fid).Select(t => t.ExecutorEmpName + ":" + t.TaskStateMC))}\"/>");
            //var updateTransitionStates = transitionIns.Select(a => $"<update id=\"{a.TransitionNodeId}\" state=\"Completed\" tooltips=\"\"/>");

            ViewBag.UpdateXml = $"<process>{string.Join("", updateActivityStates)}</process>";
            ViewBag.Xml = diagramInstance.XmlContent.Replace("workflowProcess", "mxGraphModel");
            return View();
        }
        /// <summary>
        /// 审批单
        /// </summary>
        /// <param name="BillUid"></param>
        /// <param name="BusinessUid"></param>
        /// <param name="agent"></param>
        /// <returns></returns>
        public IActionResult ApprovalBill(string billUid, string businessUid, int agent = 0)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("BillUid", billUid);
            param.Add("BusinessUid", businessUid);
            string sql = string.Empty;
            if (agent == 1)
            {
                //获取代理人
                var agents = _dbContext.QueryAll<WfAgentSetting>().Where(a => a.Agent == _applicationContext.EmpUid && a.State == 1);// _dbContext.QueryWhere<WfAgentSetting>($" Agent='{_applicationContext.EmpUid}' and [State]=1");
                if (agents.Any())
                {
                    sql = $"select WfTask.* from WfTask,WfActivityInstance where WfTask.ActivityInsUid=WfActivityInstance.Fid and  WfActivityInstance.ActivityState in('{WfActivityInstanceState.Running}','{WfActivityInstanceState.Ready}') and WfTask.ExecutorEmpUid in(@Agent) and WfTask.BusinessUid=@BusinessUid and WfTask.BillUid=@BillUid and WfTask.TaskState='Handling'";
                    param.Add("Agent", agents.Select(a => a.Principal));
                }
                else
                {
                    sql = $"select WfTask.* from WfTask where 1=2";
                }
            }
            else
            {
                sql = $"select WfTask.* from WfTask,WfActivityInstance where WfTask.ActivityInsUid=WfActivityInstance.Fid and  WfActivityInstance.ActivityState in('{WfActivityInstanceState.Running}','{WfActivityInstanceState.Ready}') and WfTask.ExecutorEmpUid='{_applicationContext.EmpUid}' and WfTask.BusinessUid=@BusinessUid and WfTask.BillUid=@BillUid and WfTask.TaskState='Handling'";
            }
            WfTask wfTask = _dbContext.QueryFirstOrDefault<WfTask>(sql, param);
            if (wfTask == null)
            {
                return Content("没有找到要审批的任务");
            }
            string activityInsUid = wfTask.ActivityInsUid;
            //获取表单类型 以及模板，字段权限
            WfProcess wfProcess = _dbContext.Get<WfProcess>(wfTask.ProcessUid);
            WfActivityInstance wfActivityInstance = _dbContext.Get<WfActivityInstance>(activityInsUid);

            //表单模板
            ViewBag.FormTemplate = wfActivityInstance.FormTemplate;
            //表单表
            string tn = wfProcess.BillTable;
            ViewBag.Process = wfProcess;
            ViewBag.FormPower = wfActivityInstance.FormPower;
            //权限字段解析
            int needEdit = 0;//存在需要编辑字段，审批的时候首先要保存
            if (wfActivityInstance.FormPower.IsPresent())
            {
                IList<FieldEntity> powers = JsonConvert.DeserializeObject<IList<FieldEntity>>(wfActivityInstance.FormPower);
                if (powers != null && powers.Any())
                {
                    var allColumns = _dbContext.Columns(tn);
                    foreach (var field in powers)
                    {
                        string colName = field.FieldName;
                        var column = allColumns.FirstOrDefault(col => col.ColName == colName);
                        if (column != null && column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                        {
                            field.FieldName += "MC";
                        }
                        field.FieldNameMC = column.ColComment;
                        if (field.Property == Fap.Workflow.Engine.Enums.FieldPropertyEnum.Editable)
                        {
                            needEdit = 1;
                        }
                    }
                    ViewBag.FormPower = powers.ToJsonIgnoreNullValue();
                }
            }
            else
            {
                ViewBag.FormPower = "undefined";
            }
            //是否编辑表单
            ViewBag.IsEdit = needEdit;
            //审批任务
            ViewBag.WfTask = wfTask;
            FormViewModel model = this.GetFormViewModel(tn, "frm-" + tn, billUid);          
           
            return View(model);
        }
        /// <summary>
        /// 查看审批单
        /// </summary>
        /// <param name="BillUid"></param>
        /// <param name="BusinessUid"></param>
        /// <returns></returns>
        public IActionResult ApprovalViewBill(string billUid, string businessUid)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("BillUid", billUid);
            param.Add("BusinessUid", businessUid);
            string sql = $"select * from WfTask where  (ExecutorEmpUid='{_applicationContext.EmpUid}' or AgentEmpUid='{_applicationContext.EmpUid}') and BusinessUid=@BusinessUid and BillUid=@BillUid and TaskState in('Completed','Revoked')";

            WfTask wfTask = _dbContext.QueryFirstOrDefault<WfTask>(sql, param);
            if (wfTask == null)
            {
                return Content("没有找到要审批的数据");
            }
            string activityInsUid = wfTask.ActivityInsUid;
            //获取表单类型 以及模板，字段权限
            WfProcess wfProcess = _dbContext.Get<WfProcess>(wfTask.ProcessUid);
            WfActivityInstance wfActivityInstance = _dbContext.Get<WfActivityInstance>(activityInsUid);

            //表单模板
            ViewBag.FormTemplate = wfActivityInstance.FormTemplate;
            //表单表
            string tn = wfProcess.BillTable;
            ViewBag.Process = wfProcess;
            ViewBag.FormPower = wfActivityInstance.FormPower;
            //权限字段解析
            int needEdit = 0;//存在需要编辑字段，审批的时候首先要保存
            if (wfActivityInstance.FormPower.IsPresent())
            {
                IList<FieldEntity> powers = JsonConvert.DeserializeObject<IList<FieldEntity>>(wfActivityInstance.FormPower);
                if (powers != null && powers.Any())
                {
                    var allColumns = _dbContext.Columns(tn);
                    foreach (var field in powers)
                    {
                        string colName = field.FieldName;
                        var column = allColumns.FirstOrDefault(col => col.ColName == colName);
                        if (column != null && column.CtrlType == FapColumn.CTRL_TYPE_REFERENCE)
                        {
                            field.FieldName += "MC";
                        }
                        field.FieldNameMC = column.ColComment;
                        if (field.Property == Fap.Workflow.Engine.Enums.FieldPropertyEnum.Editable)
                        {
                            needEdit = 1;
                        }
                    }
                    ViewBag.FormPower = powers.ToJsonIgnoreNullValue();
                }
            }
            else
            {
                ViewBag.FormPower = "undefined";
            }
            //是否编辑表单
            ViewBag.IsEdit = needEdit;
            //审批任务
            ViewBag.WfTask = wfTask;
            FormViewModel model = this.GetFormViewModel(tn, "frm-" + tn,billUid);            
            return View(model);
        }

        /// <summary>
        /// 我的代理人
        /// </summary>
        /// <returns></returns>
        public IActionResult MyAgent()
        {
            JqGridViewModel model = this.GetJqGridModel("WfAgentSetting", qs =>
            {
                qs.GlobalWhere = $"Principal='{_applicationContext.EmpUid}'";
                qs.AddDefaultValue("Principal", _applicationContext.EmpUid);
                qs.AddDefaultValue("WfAgentSettingPrincipalMC", _applicationContext.EmpName);
            });
            return View(model);
        }
        /// <summary>
        /// 待办任务
        /// </summary>
        /// <returns></returns>
        public IActionResult TodoTask()
        {
            //获取审批业务
            IEnumerable<WfBusiness> businessList = _dbContext.QueryWhere<WfBusiness>("BizStatus=1", null, true);
            //获取待办
            var listCount = _dbContext.Query($"select count(0) C,WfTask.BusinessUid from WfTask,WfActivityInstance,WfProcessInstance where WfTask.ActivityInsUid=WfActivityInstance.Fid and WfTask.ProcessInsUid= WfProcessInstance.Fid and  WfProcessInstance.ProcessState='Running' and  WfActivityInstance.ActivityState in('{WfActivityInstanceState.Running}','{WfActivityInstanceState.Ready}') and WfTask.TaskState='{WfTaskState.Handling}' and WfTask.ExecutorEmpUid='{_applicationContext.EmpUid}' group by WfTask.BusinessUid");
            foreach (var item in businessList)
            {
                var cc = listCount.FirstOrDefault(b => b.BusinessUid == item.Fid);
                item.Exp = cc != null ? (cc.C == null ? 0 : cc.C) : 0;
            }
            return View(businessList);
        }
        /// <summary>
        /// 已办任务
        /// </summary>
        /// <returns></returns>
        public IActionResult DoneTask()
        {
            //获取审批业务
            IEnumerable<WfBusiness> businesses = _dbContext.QueryWhere<WfBusiness>("BizStatus=1", null, true);

            return View(businesses);
        }
        /// <summary>
        /// 代理任务
        /// </summary>
        /// <returns></returns>
        public IActionResult AgentTask()
        {
            //获取审批业务
            IEnumerable<WfBusiness> bizs = _dbContext.QueryWhere<WfBusiness>("BizStatus=1", null, true);
            //获取代理人
            var agents = _dbContext.QueryAll<WfAgentSetting>().Where(a => a.Agent == _applicationContext.EmpUid && a.State == 1);// _dbContext.QueryWhere<WfAgentSetting>($" Agent='{_applicationContext.EmpUid}' and [State]=1");

            //获取待办
            var listCount = _dbContext.Query($"select count(0) C,WfTask.BusinessUid from WfTask,WfActivityInstance,WfProcessInstance where WfTask.ActivityInsUid=WfActivityInstance.Fid and WfTask.ProcessInsUid= WfProcessInstance.Fid and  WfProcessInstance.ProcessState='Running'  and  WfActivityInstance.ActivityState in('{WfActivityInstanceState.Running}','{WfActivityInstanceState.Ready}') and WfTask.TaskState='{WfTaskState.Handling}' and WfTask.ExecutorEmpUid in (@Agents) group by WfTask.BusinessUid", new DynamicParameters(new { Agents = agents.Select(a => a.Principal) }));
            foreach (var item in bizs)
            {
                //检查是否代理了此业务
                var existAgents = agents.Where(a => a.BusinessUid.IsMissing() || a.BusinessUid == item.Fid);
                if (existAgents.Any())
                {
                    var cc = listCount.FirstOrDefault(b => b.BusinessUid == item.Fid);
                    item.Exp = cc != null ? (cc.C == null ? 0 : cc.C) : 0;
                }
                else
                {
                    item.Exp = 0;
                }
            }

            return View(bizs);
        }
        /// <summary>
        /// 业务管理界面
        /// </summary>
        /// <param name="id">模块FID</param>
        /// <returns></returns>
        [Route("Workflow/Business/BizManage/{moduleUid}")]
        public ActionResult BizManage(string moduleUid)
        {
            if (string.IsNullOrWhiteSpace(moduleUid))
            {
                return Content("缺少业务参数");
            }
            DynamicParameters param = new DynamicParameters();
            param.Add("ModuleUid", moduleUid);
            IEnumerable<WfBusiness> businessList = _dbContext.QueryWhere<WfBusiness>("ModuleUid=@ModuleUid and BizStatus = 1", param,null,true);
            if (businessList == null || !businessList.Any())
            {
                return Content("业务分类不存在");
            }

            return View(businessList);
        }
        /// <summary>
        /// 申请列表管理
        /// </summary>
        /// <param name="id">业务类型fid</param>
        /// <returns></returns>
        public ActionResult Manage(string fid)
        {
            WfBusiness wfb = _dbContext.Get<WfBusiness>(fid);          
            if (wfb == null)
            {
                return Content("业务未关联流程");
            }
            WfProcess wfProcess = _dbContext.Get<WfProcess>(wfb.WfProcessUid);
            ViewBag.WfProcess = wfProcess;
            ViewBag.WfBusiness = wfb;
            var model = this.GetJqGridModel(wfb.BillEntity, (qs) =>
            {
                qs.GlobalWhere = "BillStatus<>'DRAFT' and BillStatus<>'CANCELED' and BillStatus<>'ENDED' and BillStatus<>'CLOSED'";
                qs.AddOrderBy("Id", "desc");
            });
            return PartialView(model);
        }
    }
}