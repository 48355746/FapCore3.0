using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Dapper;
using Fap.AspNetCore.Controls;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Fap.Core.Infrastructure.Enums;
using Fap.Workflow.Model;
using Fap.Workflow.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fap.Hcm.Web.Areas.Workflow.Controllers
{
    [Area("Workflow")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[area]/Api")]
    public class WorkflowApiController : FapController
    {
        private readonly IWorkflowService _workflowService;
        public WorkflowApiController(IServiceProvider serviceProvider, IWorkflowService workflowService) : base(serviceProvider)
        {
            _workflowService = workflowService;
        }
        /// <summary>
        /// 流程模板分类
        /// </summary>
        /// <returns></returns>
        [HttpGet("ProcessGroup")]
        public JsonResult WorkflowProcessGroup()
        {
            IEnumerable<WfProcessGroup> processGroups = _dbContext.QueryAll<WfProcessGroup>();
            List<TreeDataView> oriList = processGroups.Select(t => new TreeDataView { Id = t.Fid.ToString(), Pid = t.Pid.ToString(), Text = t.GroupName, Icon = "icon-folder orange ace-icon fa fa-users" }).ToList<TreeDataView>();

            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = _multiLangService.GetOrAndMultiLangValue(Core.MultiLanguage.MultiLanguageOriginEnum.MultiLangTag, "workflow_category", "流程分类"),
                State = new NodeState { Opened = true },
                Icon = "icon-folder blue ace-icon fa fa-sitemap",
            };
            tree.Add(treeRoot);
            TreeViewHelper.MakeTree(treeRoot.Children, oriList, treeRoot.Id);
            return Json(tree);
        }
        [HttpPost("OperProcessGroup")]
        public JsonResult OperProcessGroup(TreePostData postData)
        {
            ResponseViewModel vm = new ResponseViewModel();
            if (postData.Operation == TreeNodeOper.DELETE_NODE)
            {
                int c = _dbContext.DeleteExec(nameof(WfProcessGroup), " Fid=@Fid", new DynamicParameters(new { Fid = postData.Id }));
                vm.success = c > 0 ? true : false;
            }
            else if (postData.Operation == TreeNodeOper.CREATE_NODE)
            {
                WfProcessGroup processGroup = new WfProcessGroup()
                {
                    Pid = postData.Id,
                    GroupName = postData.Text
                };
                _dbContext.Insert<WfProcessGroup>(processGroup);
                vm.success = true;
                vm.data = processGroup.Fid;
            }
            else if (postData.Operation == TreeNodeOper.RENAME_NODE)
            {
                var processGroup = _dbContext.Get<WfProcessGroup>(postData.Id);
                processGroup.GroupName = postData.Text;
                vm.success = _dbContext.Update<WfProcessGroup>(processGroup);
            }
            else if (postData.Operation == "move_node")
            {
                var processGroup = _dbContext.Get<WfProcessGroup>(postData.Id);
                processGroup.Pid = postData.Parent;
                vm.success = _dbContext.Update(processGroup);
            }
            else if (postData.Operation == TreeNodeOper.COPY_NODE)
            {
                throw new NotImplementedException();
            }
            return Json(vm);
        }

        [HttpPost("Process")]
        public JsonResult OperProcess(string fid, string status)
        {
            var process = _dbContext.Get<WfProcess>(fid);
            process.Status = status;
            _dbContext.Update(process);
            var rv = ResponseViewModelUtils.Sueecss();
            return Json(rv);
        }
        /// <summary>
        /// 升级流程模板
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("UpgradeFlowTemplate")]
        public JsonResult UpgradeFlowTemplate(int id)
        {
            _workflowService.UpgradeProcessTemplateDirectly(id);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        /// <summary>
        /// 保存流程
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        [HttpPost("WorkflowDiagraph")]
        public JsonResult SaveWorkflowDiagraph(string xml)
        {
            xml = HttpUtility.UrlDecode(xml);

            _workflowService.SaveProcessTemplate(xml);

            return Json(ResponseViewModelUtils.Sueecss());
        }
        /// <summary>
        /// 获取单据模板
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [HttpGet("BillTemplate")]
        public JsonResult GetBillTemplate(string tableName)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("TableName", tableName);
            string sql = $"select Fid,FFCode,FFName from CfgFreeForm where BillTable=@TableName";
            var list = _dbContext.Query(sql, param);
            return Json(list);
        }
        /// <summary>
        /// 获取所有的流程
        /// </summary>
        /// <returns></returns>
        [HttpGet("WfProcess")]
        public JsonResult GetWfTemplate()
        {
            string sql = $"select Fid,ProcessName from WfProcess";
            var list = _dbContext.Query(sql);
            return Json(list);
        }
        [HttpGet("BusinessType")]
        public JsonResult GetBusinessType()
        {
            var businessTypes = _dbContext.Query<WfBusinessType>("select * from WfBusinessType");

            var oriList = businessTypes.Select(t => new TreeDataView { Id = t.Fid.ToString(), Pid = t.Pid, Data = new { group = "1", typecode = t.TypeCode, typename = t.TypeName }, Text = t.TypeName, Icon = "icon-folder purple ace-icon fa fa-share-alt" });


            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = _multiLangService.GetOrAndMultiLangValue(Core.MultiLanguage.MultiLanguageOriginEnum.MultiLangTag, "business_category", "业务分类"),
                Data = new { group = "0" },
                State = new NodeState { Opened = true },
                Icon = "icon-folder blue ace-icon fa fa-sitemap",
            };
            tree.Add(treeRoot);
            TreeViewHelper.MakeTree(treeRoot.Children, oriList, treeRoot.Id);
            return Json(tree);
        }
        [HttpPost("OperBusinessType")]
        public JsonResult OperBusinessType(TreePostData postData)
        {
            ResponseViewModel vm = new ResponseViewModel();
            if (postData.Operation == TreeNodeOper.DELETE_NODE)
            {
                int c = _dbContext.DeleteExec(nameof(WfBusinessType), " Fid=@Fid", new DynamicParameters(new { Fid = postData.Id }));
                vm.success = c > 0 ? true : false;
            }
            else if (postData.Operation == TreeNodeOper.CREATE_NODE)
            {
                WfBusinessType bizType = new WfBusinessType()
                {
                    Pid = postData.Id,
                    TypeName = postData.Text
                };
                _dbContext.Insert(bizType);
                vm.success = true;
                vm.data = bizType.Fid;
            }
            else if (postData.Operation == TreeNodeOper.RENAME_NODE)
            {
                var bizType = _dbContext.Get<WfBusinessType>(postData.Id);
                bizType.TypeName = postData.Text;
                vm.success = _dbContext.Update(bizType);
            }
            else if (postData.Operation == "move_node")
            {
                var bizType = _dbContext.Get<WfBusinessType>(postData.Id);
                bizType.Pid = postData.Parent;
                vm.success = _dbContext.Update(bizType);
            }
            else if (postData.Operation == TreeNodeOper.COPY_NODE)
            {
                throw new NotImplementedException();
            }
            return Json(vm);
        }

    }
}