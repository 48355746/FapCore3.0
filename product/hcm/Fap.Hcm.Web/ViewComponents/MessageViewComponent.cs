using Dapper;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Extensions;
using Fap.Core.Rbac;
using Fap.Model.Infrastructure;
using Fap.Workflow.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fap.Hcm.Web.ViewComponents
{
    public class MessageViewComponent:ViewComponent
    {
        private IDbContext _dataAccessor;
        private IFapSessionStorage _session;
        public MessageViewComponent(IDbContext dataAccessor,IFapSessionStorage session)
        {
            _dataAccessor = dataAccessor;
            _session = session;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("EmpUid", _session.EmpUid);
            string sqlToDo = $"select count(0) C from WfTask,WfActivityInstance,WfProcessInstance where WfTask.ActivityInsUid=WfActivityInstance.Fid and WfTask.ProcessInsUid= WfProcessInstance.Fid and  WfProcessInstance.ProcessState='Running' and  WfActivityInstance.ActivityState in('{WfActivityInstanceState.Running}','{WfActivityInstanceState.Ready}') and WfTask.TaskState='{WfTaskState.Handling}' and WfTask.ExecutorEmpUid='{_session.EmpUid}'";
            int todoCount = _dataAccessor.ExecuteScalar<int>(sqlToDo);
            var agents = _dataAccessor.QueryAll<WfAgentSetting>().Where(a => a.Agent == _session.EmpUid && a.State == 1);
            //获取待办
            var listCount = _dataAccessor.Query($"select count(0) C,WfTask.BizTypeUid from WfTask,WfActivityInstance,WfProcessInstance where WfTask.ActivityInsUid=WfActivityInstance.Fid and WfTask.ProcessInsUid= WfProcessInstance.Fid and  WfProcessInstance.ProcessState='Running' and  WfActivityInstance.ActivityState in('{WfActivityInstanceState.Running}','{WfActivityInstanceState.Ready}') and WfTask.TaskState='{WfTaskState.Handling}' and WfTask.ExecutorEmpUid in (@Agents) group by WfTask.BizTypeUid", new DynamicParameters(new { Agents = agents.Select(a => a.Principal) }));
            int agentCount = 0;
            foreach (var item in listCount)
            {
                //检查是否代理了此业务
                var existAgents = agents.Where(a => a.BusinessType.IsNullOrEmpty() || a.BusinessType == item.BizTypeUid);
                if (existAgents.Any())
                {
                    var cc = listCount.FirstOrDefault(b => b.BizTypeUid == item.BizTypeUid);
                    agentCount+=( cc != null ? (cc.C == null ? 0 : cc.C) : 0);
                }
            }
            ViewBag.ToDoCount = todoCount;
            ViewBag.AgentCount = agentCount;
            //消息通知
            IEnumerable<FapMessage> list = _dataAccessor.QueryWhere<FapMessage>("REmpUid=@EmpUid and HasRead=0 ", param, true).OrderBy(c=>c.SendTime);
            return await Task.FromResult<IViewComponentResult>(View(list));
        }
    }
}
