using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Config;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Infrastructure.Model;
using Fap.Core.Rbac.Model;
using Fap.Core.Utility;
using Fap.Workflow.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Fap.Workflow.Engine.Message
{
    public sealed class MessageManager
    {
        private const string MESSAGE_TYPE_EMAIL = "email"; //邮件
        private const string MESSAGE_TYPE_INMSG = "inmsg"; //站内消息
        private const string MESSAGE_TYPE_CALENDAR = "calendar"; //日历
        private readonly IDbContext _dataAccessor;
        private readonly ILogger<MessageManager> _logger;
        public MessageManager(IDbContext dataAccessor, ILoggerFactory loggerFactory)
        {
            _dataAccessor = dataAccessor;
            _logger = loggerFactory.CreateLogger<MessageManager>();
        }
        #region 发送消息
        /// <summary>
        /// 流程执行过程中，发送消息
        /// </summary>
        /// <param name="activityIns">活动实例</param>
        /// <param name="wfTask">任务</param>
        public void SendMessageWhenProcessing(WfActivityInstance activityIns, WfTask wfTask)
        {
            //$"{{'approver':1,'applicant':1,'mail':1,'message':1}";
            string messageSetting = activityIns.MessageSetting;
            bool noticeApplicant = false;//通知申请人
            bool noticeApprover = false;//通知审批人
            bool sendMessage = false;//发消息
            bool sendMail = false;//发邮件
            if (messageSetting.IsPresent())
            {
                var jmset = JObject.Parse(messageSetting);
                noticeApplicant = jmset.GetValue("applicant").ToBool();
                noticeApprover = jmset.GetValue("approver").ToBool();
                sendMail = jmset.GetValue("mail").ToBool();
                sendMessage = jmset.GetValue("message").ToBool();
            }
            //通知申请人
            if (noticeApplicant && activityIns.AppEmpUid.IsPresent())
            {
                Employee employee = _dataAccessor.Get<Employee>(activityIns.AppEmpUid);
                if (employee == null)
                {
                    return;
                }
                if (sendMail)
                {
                    string title = $"您的{activityIns.AppName}已经审批到[{wfTask.ExecutorEmpName}],请知晓。";
                    string content = title + "详情请登录系统查看。";
                    SendMailToAssignee(title, employee.EmpName, $"{employee.EmpName}<{employee.Mailbox}>", content);

                }
                if (sendMessage)
                {
                    SendInmsgToAssignee(employee.Fid, $"您的{activityIns.AppName}已经审批到[{wfTask.ExecutorEmpName}],请知晓。");
                }
            }
            //通知审批人
            if (noticeApprover && wfTask.ExecutorEmpUid.IsPresent())
            {
                Employee employee = _dataAccessor.Get<Employee>(wfTask.ExecutorEmpUid);
                if (employee == null)
                {
                    return;
                }
                if (sendMail)
                {
                    var biz = _dataAccessor.Get<WfBusiness>(activityIns.BusinessUid, false);
                    string title = $"有一份\"{activityIns.AppName}\"的待处理任务需要您处理！";
                    string where = "TableName='" + biz.BillEntity + "' and Enabled=1 and ModuleUid='BillMailTmpl'";
                    var emailTemplates = _dataAccessor.QueryWhere<CfgEmailTemplate>(where, null, false);
                    //流程中通知业务处理人模板
                    CfgEmailTemplate emailTemplate = emailTemplates.Where(t => t.Code == "NoticeApprover").FirstOrDefault();
                    string content = string.Empty;
                    if (emailTemplate != null)
                    {
                        var process = _dataAccessor.Get<WfProcessInstance>(activityIns.ProcessInsUid, true);

                        //内容
                        content = emailTemplate.TemplateContent;

                        content = BuildeMessageContent(process, content);

                        if (content.Contains("${业务处理时间}"))
                        {
                            content = content.Replace("${业务处理时间}", activityIns.StartTime);
                        }

                        if (content.Contains("${业务流程状态}"))
                        {
                            content = content.Replace("${业务流程状态}", process.ProcessStateMC);
                        }

                        if (content.Contains("${业务审批结论}"))
                        {
                            content = content.Replace("${业务审批结论}", "");
                        }

                        if (content.Contains("${业务审批意见}"))
                        {
                            content = content.Replace("${业务审批意见}", "");
                        }
                        if (content.Contains("${业务处理人}"))
                        {
                            content = content.Replace("${业务处理人}", "");
                        }
                    }
                    else
                    {
                        content = $"有一份\"{activityIns.AppName}\"的待处理任务需要您处理！请登录系统操作。";
                    }
                    SendMailToAssignee(title, employee.EmpName, $"{employee.EmpName}<{employee.Mailbox}>", content);
                }
                if (sendMessage)
                {
                    string content = $"有一份\"{activityIns.AppName}\"的待处理任务需要您处理！";
                    SendInmsgToAssignee(employee.Fid, content);
                }
            }
        }
        /// <summary>
        /// 根据邮件模板生成邮件内容
        /// </summary>
        /// <param name="processInstance"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private string BuildeMessageContent(WfProcessInstance processInstance, string content)
        {
            if (content.Contains("${业务名称}"))
            {
                content = content.Replace("${业务名称}", processInstance.ProcessName);
            }

            if (content.Contains("${业务申请人}"))
            {
                content = content.Replace("${业务申请人}", processInstance.AppEmpName);
            }

            if (content.Contains("${业务申请时间}"))
            {
                content = content.Replace("${业务申请时间}", processInstance.StartTime);
            }

            dynamic bizData = _dataAccessor.Get(processInstance.BillTable, processInstance.BillUid, true);
            if (bizData == null)
            {
                return content;
            }

            IDictionary<string, object> dicBizData = bizData as IDictionary<string, object>;
            var bizCols = _dataAccessor.Columns(processInstance.BillTable);
            Regex regex = new Regex(FapPlatformConstants.VariablePattern, RegexOptions.IgnoreCase);
            var mat = regex.Matches(content);
            foreach (Match item in mat)
            {
                string fieldName = item.ToString().Substring(2, item.ToString().Length - 3);
                var col = bizCols.FirstOrDefault(c => c.ColComment == fieldName);
                if (col == null)
                    continue;
                if (col.RefTable.IsPresent())
                {
                    content = content.Replace(item.ToString(), dicBizData[col.ColName + "MC"].ToStringOrEmpty());
                }
                else
                {
                    content = content.Replace(item.ToString(), dicBizData[col.ColName].ToStringOrEmpty());
                }
            }
            return content;
        }

        /// <summary>
        /// 流程结束后，发送消息
        /// </summary>
        /// <param name="processIns">流程实例</param>
        /// <param name="approveResult">审批结果（通过or驳回）</param>
        public void SendMessageWhenProcessCompleted(WfProcessInstance processIns, string approveResult)
        {
            //发送消息
            //{'notice':1,'suspend':1,'mail':1,'message':1}
            string messageSetting = processIns.MessageSetting;
            bool sendNotice = false;
            bool sendMail = false;
            bool sendMessage = false;
            if (messageSetting.IsPresent())
            {
                var jmset = JObject.Parse(messageSetting);
                sendNotice = jmset.GetValue("notice").ToBool();
                sendMail = jmset.GetValue("mail").ToBool();
                sendMessage = jmset.GetValue("message").ToBool();
            }
            if (!sendNotice || processIns.AppEmpUid.IsMissing())
            {
                return;
            }
            //收件人 
            Employee employee = _dataAccessor.Get<Employee>(processIns.AppEmpUid);

            if (sendMail)
            {
                //流程完成通知业务申请人模板
                CfgEmailTemplate emailTemplate = _dataAccessor.QueryFirstOrDefaultWhere<CfgEmailTemplate>("Code='CompleteNoticeApplier' and Enabled=1 order by Id des");

                //标题
                string title = $"申请单[{processIns.ProcessName}],单号：{processIns.BizName}，已经审批完成，审批结果：{approveResult}";
                string content = string.Empty;
                if (emailTemplate != null)
                {
                    //邮件内容
                    content = emailTemplate.TemplateContent;
                    content = BuildeMessageContent(processIns, content);

                    if (content.Contains("${业务处理时间}"))
                    {
                        content = content.Replace("${业务处理时间}", processIns.EndTime);
                    }

                    if (content.Contains("${业务流程状态}"))
                    {
                        content = content.Replace("${业务流程状态}", "完成");
                    }

                    if (content.Contains("${业务审批结论}"))
                    {
                        content = content.Replace("${业务审批结论}", approveResult);
                    }
                    if (content.Contains("${业务审批意见}") || content.Contains("${业务处理人}"))
                    {
                        IEnumerable<WfTask> tasks = _dataAccessor.QueryWhere<WfTask>($" ProcessInsUid='{processIns.Fid}' and TaskState='Completed'").OrderBy(c => c.ExecuteTime);
                        StringBuilder suggestion = new StringBuilder();
                        foreach (var task in tasks)
                        {
                            suggestion.Append(task.ExecutorEmpName).Append(" : \"").Append(task.Suggestion).Append("\"");
                            suggestion.AppendLine("<br/>");
                        }
                        content = content.Replace("${业务审批意见}", suggestion.ToString());
                        content = content.Replace("${业务处理人}", suggestion.ToString());
                    }
                }
                else
                {
                    //邮件内容
                    content = $"申请单[{processIns.ProcessName}],单号：{processIns.BizName}，已经审批完成，审批结果：{approveResult}。具体情况请登录系统查看。";

                }
                SendMailToAssignee(title, employee.EmpName, $"{employee.EmpName}<{employee.Mailbox}>", content);
            }

            if (sendMessage)
            {
                //消息内容
                string content = $"申请单[{processIns.ProcessName}],单号：{processIns.BizName}，已经审批完成，审批结果：{approveResult}";
                SendInmsgToAssignee(employee.Fid, content);
            }

        }


        /// <summary>
        /// 流程出现异常（终止、暂停、撤销）后，发送消息
        /// </summary>
        /// <param name="process"></param>
        public void SendMessageWhenProcessExcept(WfProcessInstance processIns)
        {
            string messageSetting = processIns.MessageSetting;
            bool sendSuspent = false;
            bool sendMail = false;
            bool sendMessage = false;
            if (messageSetting.IsPresent())
            {
                var jmset = JObject.Parse(messageSetting);
                sendSuspent = jmset.GetValue("suspend").ToBool();
                sendMail = jmset.GetValue("mail").ToBool();
                sendMessage = jmset.GetValue("message").ToBool();
            }
            if (!sendSuspent || processIns.AppEmpUid.IsMissing())
            {
                return;
            }
            Employee employee = _dataAccessor.Get<Employee>(processIns.AppEmpUid);
            if(employee==null)
            {
                return;
            }
            string title = $"您发起的单号为：{processIns.BizName}的\"{processIns.ProcessName}\"流程被挂起";
            string content = $"您发起的流程【{processIns.ProcessName}】,单号:{processIns.BizName},已经被挂起，请知晓！";
            if (sendMail)
            {
                SendMailToAssignee(title, employee.EmpName, $"{employee.EmpName}<{employee.Mailbox}>", content);
            }
            if(sendMessage)
            {
                SendInmsgToAssignee(employee.Fid, title);
            }
        }
        /// <summary>
        /// 发送催办邮件消息
        /// </summary>
        /// <param name="bizUid">业务Fid</param>
        /// <param name="bizTypeUid">业务类型</param>
        public string SendUrgeMessage(string billUid, string businessUid)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("BillUid", billUid);
            param.Add("BusinessUid", businessUid);
            var receiver = _dataAccessor.Query("select WfTask.ExecutorEmpUid,WfTask.TaskStartTime from WfTask,WfActivityInstance where WfTask.ActivityInsUid=WfActivityInstance.Fid and WfTask.TaskState='Handling' and WfActivityInstance.BillUid=@BillUid and WfActivityInstance.BusinessUid=@BusinessUid  and (WfActivityInstance.ActivityState='Ready' or WfActivityInstance.ActivityState='Running')", param);

            if (receiver == null || !receiver.Any())
                return "没有要催办的人。";

            var reciverIds = receiver.Select(r => r.ExecutorEmpUid);
            var receipants = _dataAccessor.Query<Employee>("select * from Employee where Fid in @Fids",new DynamicParameters(new { Fids= reciverIds }));

            List<string> receipantIds = new List<string>(); //收件人Fid
            List<string> receipantNames = new List<string>(); //收件人姓名
            List<string> receipantMailboxes = new List<string>(); //收件人邮箱
            foreach (var receipant in receipants)
            {
                receipantIds.Add(receipant.Fid);
                receipantNames.Add(receipant.EmpName);
                receipantMailboxes.Add($"{receipant.EmpName}<{receipant.Mailbox}>");
            }
            WfBusiness biz = _dataAccessor.Get<WfBusiness>(businessUid);
            if (biz == null)
                return "未找到业务，催办失败！";
            string tableName = biz.BillEntity;
            dynamic bizData = _dataAccessor.QueryFirstOrDefault($"select * from {tableName} where Fid=@BillUid", param, true);
            string content = string.Format("您有一条：{0}的催办任务，制单人:{1},任务到达时间为:{2},请及时处理。", biz.BizName, bizData.BillEmpUidMC, receiver.First().TaskStartTime);
            string title = string.Format("{0}的催办任务", biz.BizName);
            try
            {
                //邮件
                SendMailToAssignee(title, receipantNames, receipantMailboxes, content);
                //站内信
                SendInmsgToAssignee(receipantIds, content);
                return "发送成功。";
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                return ex.Message;
            }


        }

        #endregion 发送消息

        #region 发送消息给业务处理人
        /// <summary>
        /// 发送邮件给业务处理人
        /// </summary>
        /// <param name="senderName"></param>
        /// <param name="senderMailbox"></param>
        /// <param name="receipantNames"></param>
        /// <param name="receipantMailboxes"></param>
        /// <param name="content"></param>
        private void SendMailToAssignee(string subject,
            List<string> receipantNames, List<string> receipantMailboxes,
            string content)
        {
            FapMail mail = CreateMail(subject, receipantNames.ToArray(), receipantMailboxes.ToArray(), content);
            _dataAccessor.Insert<FapMail>(mail);
        }

        private void SendMailToAssignee(string subject,
           string receipantName, string receipantMailbox,
           string content)
        {
            FapMail mail = CreateMail(subject, new string[] { receipantName }, new string[] { receipantMailbox }, content);
            _dataAccessor.Insert(mail);
        }

        /// <summary>
        /// 发送站内消息给业务处理人
        /// </summary>
        /// <param name="senderName"></param>
        /// <param name="senderMailbox"></param>
        /// <param name="receipantNames"></param>
        /// <param name="receipantMailboxes"></param>
        /// <param name="content"></param>
        private void SendInmsgToAssignee(
            List<string> receipantIds,
            string content)
        {
            var msgs = CreateMessage(receipantIds.ToArray(), content);
            if (msgs.Any())
            {
                _dataAccessor.InsertBatch(msgs);
            }
        }

        private void SendInmsgToAssignee(
            string receipantId,
            string content)
        {
            var msgs = CreateMessage(new string[] { receipantId }, content);
            if (msgs.Any())
            {
                _dataAccessor.InsertBatch(msgs);
            }
        }
        /// <summary>
        /// 发送日历事件给业务处理人
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="receipantIds"></param>
        /// <param name="title"></param>
        /// <param name="url"></param>
        private void SendCalendarToAssigee(
            List<string> receipantIds,
            string title, string url)
        {
            var calendars = CreateEssCalendar(receipantIds.ToArray(), title, url);
            if (calendars.Any())
            {
                _dataAccessor.InsertBatch(calendars);
            }
        }

        private void SendCalendarToAssigee(
            string receipantId,
            string title, string url)
        {
            var calendars = CreateEssCalendar(new string[] { receipantId }, title, url);

            if (calendars.Any())
            {
                _dataAccessor.InsertBatch(calendars);
            }
        }

        #endregion





        #region 创建消息实体对象
        /// <summary>
        /// 创建邮件对象
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="senderMail"></param>
        /// <param name="recipients"></param>
        /// <param name="recipientMails"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private FapMail CreateMail(
            string Subject,
            string[] recipients,
            string[] recipientMails,
            string content)
        {
            FapMail mail = new FapMail();
            mail.Subject = Subject;
            //mail.Sender = sender;
            //mail.SenderEmailAddress = senderMail;
            mail.MailContent = content;
            mail.IsSeparate = 1;
            mail.Recipient =string.Join(';', recipients);
            mail.RecipientEmailAddress =string.Join(';', recipientMails);

            return mail;
        }

        /// <summary>
        /// 创建站内消息对象
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="recipientId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private IEnumerable<FapMessage> CreateMessage(string[] recipientId, string content)
        {
            List<FapMessage> fapMessages = new List<FapMessage>();

            for (int i = 0; i < recipientId.Length; i++)
            {
                FapMessage message = new FapMessage();
                message.Title = "流程提醒";
                //message.SEmpUid = senderId;
                message.MsgContent = content;
                message.REmpUid = recipientId[i];
                message.SendTime = DateTimeUtils.CurrentDateTimeStr;
                message.MsgCategory = FapMessageCategory.Notice;
                fapMessages.Add(message);
            }

            return fapMessages;
        }

        /// <summary>
        /// 创建员工日历事件的实体
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="recipientId"></param>
        /// <param name="title"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private IEnumerable<EssCalendar> CreateEssCalendar(string[] recipientId, string title, string url)
        {
            List<EssCalendar> essCalendars = new List<EssCalendar>();
            for (int i = 0; i < recipientId.Length; i++)
            {
                EssCalendar calendar = new EssCalendar();
                calendar.EmpUid = recipientId[i];
                calendar.EventName = title;
                calendar.EventUrl = url;

                essCalendars.Add(calendar);
            }

            return essCalendars;
        }

        #endregion

    }


    /// <summary>
    /// 消息发送方式
    /// </summary>
    public enum MessageType
    {
        [Description("邮件")]
        email,
        [Description("站内消息")]
        inmsg,
        [Description("日历")]
        calendar,
    }
}
