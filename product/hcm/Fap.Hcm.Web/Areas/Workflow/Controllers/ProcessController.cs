using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fap.Core.Extensions;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Fap.Workflow.Model;
using Microsoft.AspNetCore.Mvc;
using Fap.Core.Utility;
using System.Text;
using System.IO;
using System.Web;

namespace Fap.Hcm.Web.Areas.Workflow.Controllers
{
    [Area("Workflow")]
    public class ProcessController : FapController
    {
        public ProcessController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        /// <summary>
        /// 流程模板
        /// </summary>
        /// <returns></returns>
        public IActionResult Template()
        {
            //显示有用的和禁用的
            string initCondition = $"(Status='{WfProcessState.Using}' or Status='{WfProcessState.Forbidden}')";
            var model = this.GetJqGridModel("WfProcess", (queryOption) =>
            {
                queryOption.GlobalWhere = initCondition;
                //queryOption.HiddenCols = "FormType,State";
                queryOption.AddDefaultValue("Status", WfProcessState.Using);
            });
            return View(model);
        }
        /// <summary>
        /// 设计
        /// </summary>
        /// <returns></returns>
        public IActionResult Design(string fid)
        {
            WfProcess tmplate = _dbContext.Get<WfProcess>(fid);
            string diagramUid = tmplate.DiagramId;
            WfDiagram diagram = null;
            string xml = string.Empty;
            if (diagramUid.IsPresent())
            {
                diagram = _dbContext.Get<WfDiagram>(diagramUid);
            }
            if (diagram == null)
            {
                diagram = new WfDiagram() { Fid = UUIDUtils.Fid, ProcessName = tmplate.ProcessName, XmlContent = "" };

            }
            if (diagram.XmlContent.IsMissing())
            {
                xml = string.Join("", new string[]{$"<mxGraphModel dx=\"1426\" dy=\"779\" grid=\"1\" gridSize=\"10\" guides=\"1\" tooltips=\"1\" connect=\"1\" arrows=\"1\" fold=\"1\" page=\"1\" pageScale=\"1\" pageWidth=\"827\" pageHeight=\"1169\" wfProcessUid=\"{fid}\" wfDiagramUid=\"{diagram.Fid}\" wfDiagramName=\"{diagram.ProcessName}\" wfDesc=\"\" wfResultNotice=\"1\" wfSuspendNotice=\"0\" wfMail=\"1\" wfMessage=\"1\" frmType=\"Internal\" billTable=\"\" background=\"#ffffff\">",
                                                    "  <root>",
                                                    "    <mxCell id=\"0\"/>",
                                                    "    <mxCell id=\"1\" parent=\"0\"/>   ",
                                                    "  </root>",
                                                    "</mxGraphModel>" });
            }
            else
            {
                xml = diagram.XmlContent.Replace("workflowProcess", "mxGraphModel");
            }
            ViewBag.XML = xml;
            return View();
        }
        /// <summary>
        /// 审批人选择
        /// </summary>
        /// <returns></returns>
        public IActionResult Approver(string billTable)
        {
            var empModel = this.GetJqGridModel("Employee", (qs) =>
            {
                qs.QueryCols = "Id,Fid,EmpCode,EmpName,DeptUid";
                qs.GlobalWhere = "IsMainJob=1";//主职
            });
            var roleModel = this.GetJqGridModel("FapBizRole", (qs) =>
            {
                qs.QueryCols = "Id,Fid,BizRoleName";
            });
            var dynRoleModel = this.GetJqGridModel("FapBizDynRole", (qs) =>
            {
                qs.QueryCols = "Id,Fid,RoleName,CustomSql,BindType";
                qs.GlobalWhere = "State=1";//有效
            });
            MultiJqGridViewModel model = new MultiJqGridViewModel();
            model.JqGridViewModels.Add("employee", empModel);
            model.JqGridViewModels.Add("role", roleModel);
            model.JqGridViewModels.Add("dynrole", dynRoleModel);
            ViewBag.BillTable = billTable;
            return View(model);
        }
        public IActionResult Transition(string billTable)
        {
            ViewBag.BillTable = billTable;
            return View();
        }
        public IActionResult OpenFile()
        {
            return View();
        }
        /// <summary>
        /// 另存为文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public FileResult SaveAsFile(string filename, string xml)
        {
            // convert string to stream             
            byte[] byteArray = Encoding.UTF8.GetBytes(HttpUtility.UrlDecode(xml));
            //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
            MemoryStream stream = new MemoryStream(byteArray);
            return File(stream, "text/xml", filename);
        }
        public PartialViewResult HistoryVersion(int id)
        {
            WfProcess template = _dbContext.Get<WfProcess>(id);
            string collectionId = template.CollectionId;
            string initCondition = $"CollectionId='{collectionId}' AND Status='{WfProcessState.Historical}'";
            JqGridViewModel model = this.GetJqGridModel("WfProcess", (queryOption) =>
            {
                queryOption.GlobalWhere = initCondition;

            });
            ViewBag.Id = template.Id;
            return PartialView(model);
        }
        /// <summary>
        /// 业务流程
        /// </summary>
        /// <returns></returns>
        public IActionResult BizProcess()
        {
            JqGridViewModel model = this.GetJqGridModel("WfBusiness");
            return View(model);
        }
        public IActionResult BizRole()
        {
            MultiJqGridViewModel model = new MultiJqGridViewModel();
            JqGridViewModel bizRole = this.GetJqGridModel("Employee", (qs) =>
            {
                qs.InitWhere = "1=2";
                qs.GlobalWhere = "IsMainJob=1";
                qs.QueryCols = "Id,Fid,EmpCategory,EmpCode,EmpName,Gender,DeptUid,EmpPosition";
            });

            JqGridViewModel dynRole = this.GetJqGridModel("FapBizDynRole");
            model.JqGridViewModels.Add("FapBizRole", bizRole);
            model.JqGridViewModels.Add("FapBizDynRole", dynRole);
            return View(model);
        }
        /// <summary>
        /// 流程监控
        /// </summary>
        /// <returns></returns>
        public IActionResult Monitoring()
        {
            var model = this.GetJqGridModel("WfProcessInstance", (qs) =>
            {
                qs.InitWhere = $"ProcessState='{ WfProcessInstanceState.Running}'";
            });
            return View(model);
        }
        /// <summary>
        /// 代理人管理
        /// </summary>
        /// <returns></returns>
        public IActionResult Agent()
        {
            JqGridViewModel model = this.GetJqGridModel("WfAgentSetting");
            return View(model);
        }
    }
}