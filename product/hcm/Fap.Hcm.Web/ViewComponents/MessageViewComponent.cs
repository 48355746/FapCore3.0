using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Model;
using Fap.Workflow.Model;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fap.Core.Extensions;
using Fap.Hcm.Web.Models;

namespace Fap.Hcm.Web.ViewComponents
{
    public class MessageViewComponent : ViewComponent
    {
        private IDbContext _dbContext;
        private IFapApplicationContext _applicationContext;
        public MessageViewComponent(IDbContext dbContext, IFapApplicationContext applicationContext)
        {
            _dbContext = dbContext;
            _applicationContext = applicationContext;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            AttentionModel model = new AttentionModel();
            DynamicParameters param = new DynamicParameters();
            param.Add("EmpUid", _applicationContext.EmpUid);
            string sqlToDo = $"select count(0) C from WfTask,WfActivityInstance,WfProcessInstance where WfTask.ActivityInsUid=WfActivityInstance.Fid and WfTask.ProcessInsUid= WfProcessInstance.Fid and  WfProcessInstance.ProcessState='Running' and  WfActivityInstance.ActivityState in('{WfActivityInstanceState.Running}','{WfActivityInstanceState.Ready}') and WfTask.TaskState='{WfTaskState.Handling}' and WfTask.ExecutorEmpUid='{_applicationContext.EmpUid}'";
            int todoCount = _dbContext.ExecuteScalar<int>(sqlToDo);
            var agents = _dbContext.QueryAll<WfAgentSetting>().Where(a => a.Agent == _applicationContext.EmpUid && a.State == 1);
            //获取待办
            var listCount = _dbContext.Query($"select count(0) C,WfTask.BusinessUid from WfTask,WfActivityInstance,WfProcessInstance where WfTask.ActivityInsUid=WfActivityInstance.Fid and WfTask.ProcessInsUid= WfProcessInstance.Fid and  WfProcessInstance.ProcessState='Running' and  WfActivityInstance.ActivityState in('{WfActivityInstanceState.Running}','{WfActivityInstanceState.Ready}') and WfTask.TaskState='{WfTaskState.Handling}' and WfTask.ExecutorEmpUid in @Agents group by WfTask.BusinessUid", new DynamicParameters(new { Agents = agents.Select(a => a.Principal) }));
            int agentCount = 0;
            foreach (var item in listCount)
            {
                //检查是否代理了此业务
                var existAgents = agents.Where(a => a.BusinessUid.IsMissing() || a.BusinessUid == item.BusinessUid);
                if (existAgents.Any())
                {
                    var cc = listCount.FirstOrDefault(b => b.BusinessUid == item.BusinessUid);
                    agentCount += (cc != null ? (cc.C == null ? 0 : cc.C) : 0);
                }
            }
            model.TodoCount = todoCount;
            model.AgentCount = agentCount;
            //消息
            IEnumerable<FapMessage> list = _dbContext.QueryWhere<FapMessage>("REmpUid=@EmpUid and HasRead=0 ", param, true).OrderBy(c => c.SendTime);
            model.Messages = list;
            //通知
            model.Notifications= Nofifications(param)??Enumerable.Empty<Notifications>();

            return await Task.FromResult<IViewComponentResult>(View(model)).ConfigureAwait(false);
        }

        private IEnumerable<Notifications> Nofifications(DynamicParameters param)
        {
            //考核待打分
            int assessCount = _dbContext.Count("PerfExaminer", "EmpUid=@EmpUid  and ProgramUid in(select fid from perfprogram where PrmStatus='Starting')", param);
            if (assessCount > 0)
            {
                yield return new Notifications { Description = "考核待打分", Icon = "fa-gavel btn-primary", NCount = assessCount, NavigateUrl = "Assess/Manage/MyAssess" };
            }
            //伙伴申请
            int partnerCount = _dbContext.Count(nameof(EssPartner), $"{nameof(EssPartner.PartnerUid)}=@EmpUid and {nameof(EssPartner.RequestResult)}='None'", param);
            if (partnerCount > 0)
            {
                yield return new Notifications { Description = "伙伴申请", Icon = "fa-volume-up btn-pink", NCount = partnerCount, NavigateUrl = "SelfService/Ess/MyPartner" };
            }
            //简历待评价
            int resumeCommentCount = _dbContext.Count("RcrtResumeReview", "EmpUid=@EmpUid and Review=''", param);
            if (resumeCommentCount > 0)
            {
                yield return new Notifications { Description = "简历待评价", Icon = "fa-commenting btn-success", NCount = resumeCommentCount, NavigateUrl = "Recruit/Manage/MyRecruit" };
            }
            //面试评价
            int interviewCommentCount = _dbContext.Count("RcrtInterview", "EmpUid=@EmpUid and (IvResult>0 or IvReview!='')", param);
            if (interviewCommentCount > 0)
            {
                yield return new Notifications { Description = "面试待评价", Icon = "fa-commenting-o btn-info", NCount = interviewCommentCount, NavigateUrl = "Recruit/Manage/MyRecruit" };
            }
        }
    }
}
