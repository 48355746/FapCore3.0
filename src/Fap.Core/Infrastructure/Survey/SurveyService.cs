using Fap.Core.Infrastructure.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Fap.Core.Extensions;
using Fap.Core.DI;
using Fap.Core.DataAccess;
using Fap.Core.Utility;
using Dapper;
using NPOI.OpenXmlFormats.Dml;
using Fap.Core.Infrastructure.Domain;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;
using System.IO;
using Fap.AspNetCore.Model;
using Fap.Core.Rbac.Model;
using Google.Protobuf;
using Fap.Core.Message;
using Fap.Core.Infrastructure.Cache;
using Org.BouncyCastle.Utilities;

namespace Fap.Core.Infrastructure
{
    [Service]
    public class SurveyService : ISurveyService
    {
        private const string FAP_SURVEY_RESPONSELIST = "fap_survey_responselist";
        private readonly IDbContext _dbContext;
        private readonly IFapApplicationContext _applicationContext;
        private readonly IMessageService _messageService;
        private readonly ICacheService _cacheService;
        public SurveyService(IDbContext dbContext, ICacheService cacheService, IFapApplicationContext applicationContext, IMessageService messageService)
        {
            _dbContext = dbContext;
            _applicationContext = applicationContext;
            _messageService = messageService;
            _cacheService = cacheService;
        }
        /// <summary>
        /// 预览问卷
        /// </summary>
        /// <param name="fid"></param>
        public void PreviewSurvey(Survey survey)
        {
            JObject surveyJObj = JObject.Parse(survey.JSONContent);
            JArray arryContent = surveyJObj.GetValue("content") as JArray;
            List<SurQuestion> qsfList = new List<SurQuestion>();
            if (arryContent != null && arryContent.Any())
            {
                foreach (JObject obj in arryContent)
                {
                    SurQuestion qsf = new SurQuestion();
                    qsf.SurveyUid = survey.Fid;
                    qsf.Content = obj.GetStringValue("content");
                    if (obj.GetValue("absolute_id") != null)
                    {
                        qsf.AbsoluteId = obj.GetStringValue("absolute_id").ToInt();
                    }
                    qsf.TypeId = obj.GetStringValue("type_id");
                    qsf.SortIndex = obj.GetStringValue("order").ToInt();
                    if (obj.GetValue("has_other") != null)
                    {
                        qsf.HasOther = obj.GetStringValue("has_other").EqualsWithIgnoreCase("N") ? 0 : 1;
                    }
                    qsf.PageNum = obj.GetIntValue("page");
                    qsf.QIndex = obj.GetIntValue("index");
                    qsf.LastAbsoluteId = obj.GetIntValue("last_absolute_id");
                    qsf.LogicHide = obj.GetIntValue("logic_hide");
                    JArray arryChoice = obj.GetValue("choice") as JArray;
                    if (arryChoice != null && arryChoice.Any())
                    {
                        foreach (JObject jChoice in arryChoice)
                        {
                            jChoice["id"] = jChoice.GetValue("choice_absolute_id");
                        }
                    }
                    qsf.JSONContent = obj.ToString();
                    qsfList.Add(qsf);
                }
                //if (qsfList.Count > 0)
                //{
                //    _dbContextTxProxy.TransactionScopeDelegate(_dbContext =>
                //    {
                //        _dbContext.DeleteEntityByWhere<SurQuestion>("SurveyUid='" + survey.Fid + "'", false);
                //        _dbContext.InsertEntityBatch<SurQuestion>(qsfList);
                //    });
                //}

            }
            JObject jObj = new JObject();
            jObj["survey_id"] = survey.Id;
            jObj["survey_name"] = survey.SurName;
            jObj["status"] = "0";
            jObj["type"] = 0;
            if (qsfList.Any())
            {
                JArray arryQsts = new JArray();
                var qg = qsfList.GroupBy(a => a.PageNum);
                foreach (var qpg in qg)
                {
                    JObject jPage = new JObject();
                    jPage["index"] = qpg.Key;
                    var qstJsons = qpg.ToList().Select(f => f.JSONContent);
                    JArray qsfs = new JArray();
                    foreach (var q in qstJsons)
                    {
                        JObject jqst = JObject.Parse(q);
                        string typeid = jqst.GetValue("type_id").ToString();
                        if (typeid == "11")
                        {
                            //分页
                            continue;
                        }
                        if (typeid == "8" || typeid == "9" || typeid == "15")
                        {
                            //8多选or9矩阵
                            JArray jchoice = jqst.GetValue("choice") as JArray;
                            if (jchoice != null)
                            {
                                string values = "[";

                                for (int i = 0; i < jchoice.Count; i++)
                                {
                                    if (typeid == "8" || typeid == "15")
                                    {
                                        values += "false,";
                                    }
                                    else if (typeid == "9")
                                    {
                                        values += "null,";
                                    }
                                }
                                values = values.TrimEnd(',') + "]";

                                jqst["value"] = JArray.Parse(values);
                            }
                        }
                        else if (typeid == "13")
                        {
                            //矩阵多选题
                            string values = "[";
                            JArray jchoice = jqst.GetValue("choice") as JArray;
                            JArray jtitle = jqst.GetValue("checkbox_array_title") as JArray;
                            for (int i = 0; i < jtitle.Count; i++)
                            {
                                values += "[";
                                for (int j = 0; j < jchoice.Count; j++)
                                {
                                    values += "false,";
                                }
                                values = values.TrimEnd(',') + "],";
                            }
                            values = values.TrimEnd(',') + "]";
                            jqst["value"] = JArray.Parse(values);
                        }
                        else
                        {
                            jqst["value"] = null;
                        }
                        jqst["vote_type_id"] = "0";
                        jqst["id"] = jqst.GetValue("absolute_id");
                        qsfs.Add(jqst);
                    }
                    jPage["list"] = qsfs;
                    arryQsts.Add(jPage);
                }
                jObj["pages"] = arryQsts;
                jObj["page_count"] = qg.Count();
            }
            jObj["from"] = survey.Fid;
            jObj["logic_condition"] = surveyJObj.GetValue("survey_logic");
            jObj["redirect_relation"] = surveyJObj.GetValue("redirect_relation");
            jObj["description"] = surveyJObj.GetValue("test_content");
            jObj["scene"] = 1;
            jObj["platform"] = 2;
            jObj["token"] = survey.Fid;

            survey.JSONPreview = jObj.ToString();
        }
        /// <summary>
        /// 发布问卷
        /// </summary>
        /// <param name="fid"></param>
        [Transactional]
        public bool PublishSurvey(SurFilter surFilter)
        {
            string fid = surFilter.SurveyUid;
            Survey survey = _dbContext.Get<Survey>(fid);
            _dbContext.DeleteExec(nameof(SurFilter), "SurveyUid=@SurveyUid", new DynamicParameters(new { SurveyUid = survey.Fid }));
            JObject surveyJObj = JObject.Parse(survey.JSONContent);
            JArray arryContent = surveyJObj.GetValue("content") as JArray;
            List<SurQuestion> qsfList = new List<SurQuestion>();
            List<SurQuestionChoice> choiceList = new List<SurQuestionChoice>();
            List<SurArrayTitle> titleList = new List<SurArrayTitle>();
            //转化为发布态的逻辑关系
            JObject publishRRObj = new JObject();
            if (arryContent != null && arryContent.Any())
            {
                SplitQuestionAndChoice(survey, surveyJObj, arryContent, qsfList, choiceList, titleList, publishRRObj);
            }
            else
            {
                return false;
            }
            CollectionSet(survey,surFilter);

            PublishJson(surFilter, survey, qsfList, choiceList, titleList, publishRRObj);

            return true;
        }

        private void CollectionSet(Survey survey, SurFilter surFilter)
        {
            string empWhere = string.Empty;
            if (surFilter.FilterCondition.IsPresent())
            {
                //计算问卷人员
                JsonFilterToSql jfs = new JsonFilterToSql(_dbContext);
                empWhere = jfs.BuilderFilter("Employee", surFilter.FilterCondition);

            }
            var employees = _dbContext.Query<Employee>($"select {nameof(Employee.Fid)},{nameof(Employee.EmpName)},{nameof(Employee.Mailbox)} from {nameof(Employee)} {(empWhere.IsPresent() ? " where " : "")} {empWhere}");
            surFilter.Amounted = employees.Count();
            //收集设置
            _dbContext.Insert(surFilter);
            List<FapMail> mailList = new List<FapMail>();
            List<FapMessage> messageList = new List<FapMessage>();
            foreach (var emp in employees)
            {
                if (emp.Mailbox.IsPresent())
                {
                    string href =$"{_applicationContext.BaseUrl}/System/Survey/FillIn/{surFilter.SurveyUid}";
                    var mailContent = @$"<div class='row'>
    < div class='col-sm-12'>
	 <h1 class='blue'>Hi,{emp.EmpName}</h1>
	 <div>这里有一个调查问卷[{ survey.SurName}]需要你填写，点击下面链接进入。谢谢！</div>
   </div>
</div>
<div class='space-12 '></div>


<div class='break-12'></div>
<div class='row' style='height: 45px; background-color: transparent'>
 <div class='col text-center'>
  <span class='light-grey'><a href ='{href}'> click here to unsubscribe</a></span>
  </div>
</div>";
                    FapMail mail = new FapMail
                    {
                        Recipient = emp.EmpName,
                        RecipientEmailAddress = $"{emp.EmpName}<{emp.Mailbox}>",
                        MailCategory = "调查问卷",
                        Subject = "调查问卷邀请",
                        MailContent = mailContent,
                        IsSeparate = 0,
                        SendStatus = 0
                    };
                    mailList.Add(mail);
                    FapMessage message = new FapMessage
                    {
                        REmpUid=emp.Fid,
                        Title="调查问卷",
                        MsgContent=survey.SurName,
                        SendTime=DateTimeUtils.CurrentDateTimeStr,
                        IsGlobal=0,
                        HasRead=0,
                        URL=href,
                        MsgCategory="Notice",
                    };
                    messageList.Add(message);
                }
            }
            //发邮件
            _messageService.SendMailList(mailList);
            _messageService.SendMessageList(messageList);

        }

        private void SplitQuestionAndChoice(Survey survey, JObject surveyJObj, JArray arryContent, List<SurQuestion> qsfList, List<SurQuestionChoice> choiceList, List<SurArrayTitle> titleList, JObject publishRRObj)
        {
            foreach (JObject obj in arryContent)
            {
                SurQuestion qsf = new SurQuestion();
                qsf.Fid = UUIDUtils.Fid;
                qsf.SurveyUid = survey.Fid;
                qsf.Content = obj.GetValue("content").ToString();
                qsf.Required = obj.GetStringValue("required") == "N" ? 0 : 1;
                //if (obj.GetValue("absolute_id") != null)
                //{
                //    qsf.AbsoluteId = obj.GetValue("absolute_id").ToString().ToInt();
                //}
                qsf.TypeId = obj.GetStringValue("type_id");
                qsf.SortIndex = obj.GetIntValue("order");
                qsf.AbsoluteId = qsf.SortIndex;
                if (obj.GetValue("has_other") != null)
                {
                    qsf.HasOther = obj.GetStringValue("has_other").EqualsWithIgnoreCase("N") ? 0 : 1;
                }
                qsf.TitleQuote = obj.GetValue("title_quote") == null ? 0 : (obj.GetStringValue("title_quote") == "N" ? 0 : 1);
                qsf.ChoiceQuote = obj.GetValue("choice_quote") == null ? "0" : obj.GetStringValue("choice_quote");
                qsf.MaxValue = obj.GetValue("max") == null ? "0" : obj.GetStringValue("max");
                qsf.MinValue = obj.GetValue("min") == null ? "0" : obj.GetStringValue("min");
                qsf.PageNum = obj.GetIntValue("page");
                qsf.QIndex = obj.GetIntValue("index");
                qsf.LastAbsoluteId = arryContent.Count; //obj.GetValue("last_absolute_id").ToString().ToInt();
                qsf.LogicHide = obj.GetValue("logic_hide").ToString().ToInt();
                JArray arryChoice = obj.GetValue("choice") as JArray;
                if (arryChoice != null && arryChoice.Any())
                {
                    foreach (JObject jChoice in arryChoice)
                    {
                        jChoice["id"] = jChoice.GetValue("choice_absolute_id");
                        SurQuestionChoice choice = new SurQuestionChoice();
                        choice.Fid = UUIDUtils.Fid;
                        choice.QuestionUid = qsf.Fid;
                        choice.IsOther = jChoice.GetStringValue("is_other") == "N" ? 0 : 1;
                        choice.Required = jChoice.GetIntValue("required");
                        choice.SortIndex = jChoice.GetIntValue("order");
                        choice.ChoiceAbsoluteId = jChoice.GetIntValue("choice_absolute_id");
                        choice.Content = jChoice.GetStringValue("content");
                        choice.SurveyUid = survey.Fid;
                        choiceList.Add(choice);
                    }
                }
                JArray arryRadioTitle = obj.GetValue("radio_array_title") as JArray;
                if (arryRadioTitle != null && arryRadioTitle.Any())
                {
                    foreach (JObject radioObj in arryRadioTitle)
                    {
                        SurArrayTitle title = new SurArrayTitle();
                        title.Fid = UUIDUtils.Fid;
                        title.QuestionUid = qsf.Fid;
                        title.SortIndex = radioObj.GetIntValue("order");
                        title.Content = radioObj.GetStringValue("content");
                        title.SurveyUid = survey.Fid;
                        titleList.Add(title);
                    }
                }
                JArray arryCheckTitle = obj.GetValue("checkbox_array_title") as JArray;
                if (arryCheckTitle != null && arryCheckTitle.Any())
                {
                    foreach (JObject checkboxObj in arryCheckTitle)
                    {
                        SurArrayTitle title = new SurArrayTitle();
                        title.Fid = UUIDUtils.Fid;
                        title.QuestionUid = qsf.Fid;
                        title.SortIndex = checkboxObj.GetIntValue("order");
                        title.Content = checkboxObj.GetStringValue("content");
                        title.SurveyUid = survey.Fid;
                        titleList.Add(title);
                    }
                }
                qsf.JSONContent = obj.ToString();
                qsfList.Add(qsf);
            }

            if (qsfList.Count > 0)
            {
                //设置逻辑关系
                JObject jRR = surveyJObj.GetValue("redirect_relation") as JObject;

                if (jRR != null)
                {
                    IEnumerable<JProperty> drr = jRR.Properties();
                    //foreach (JProperty item in properties)
                    //{
                    //    Console.WriteLine(item.Name + ":" + item.Value);
                    //} 
                    //IDictionary<string, JObject> drr = jRR as IDictionary<string, JObject>;
                    if (drr != null && drr.Any())
                    {
                        foreach (var rr in drr)
                        {
                            //问题
                            string questionAID = rr.Name;
                            SurQuestion question = qsfList.First(q => q.AbsoluteId == questionAID.ToInt());

                            IEnumerable<JProperty> tc = JObjectToDic(rr.Value as JObject);
                            if (tc != null)
                            {
                                JArray jarryRR = new JArray();
                                JObject jChoice = new JObject();
                                //string rrs = "[";
                                foreach (var t in tc)
                                {
                                    JObject orr = new JObject();
                                    //rrs+="{\"choice_id\":";
                                    //选项
                                    SurQuestionChoice sqc = choiceList.First(c => c.ChoiceAbsoluteId == t.Name.ToInt() && c.QuestionUid == question.Fid);
                                    if (sqc != null)
                                    {
                                        orr["choice_id"] = sqc.Fid;
                                        //rrs +="\""+ sqc.Fid + "\",";

                                        IEnumerable<JProperty> tq = JObjectToDic(t.Value as JObject);
                                        if (tq != null && tq.Any())
                                        {
                                            //目标问题
                                            SurQuestion tarQuestion = qsfList.FirstOrDefault(q => q.AbsoluteId == tq.FirstOrDefault()?.Name.ToInt());
                                            if (tarQuestion != null)
                                            {
                                                orr["question_id"] = tarQuestion.Fid;
                                                //rrs += "\"question_id\":" + "\"" + tarQuestion.Fid+"\"";
                                            }
                                            JObject jtargt = new JObject();
                                            jtargt[tarQuestion.Fid] = 1;

                                            jChoice[sqc.Fid] = jtargt;

                                        }
                                        jarryRR.Add(orr);
                                    }
                                    // rrs += "},";
                                }
                                publishRRObj[question.Fid] = jChoice;
                                //rrs = rrs.TrimEnd(',') + "]";
                                question.RedirectRelation = jarryRR.ToString();
                            }
                        }
                    }
                }
                string where = "SurveyUid=@SurveyUid";
                DynamicParameters param = new DynamicParameters(new { SurveyUid = survey.Fid });
                //问题
                _dbContext.DeleteExec(nameof(SurQuestion), where, param);
                _dbContext.InsertBatchSql(qsfList);
                if (choiceList.Count > 0)
                {
                    //选项
                    _dbContext.DeleteExec(nameof(SurQuestionChoice), where, param);
                    _dbContext.InsertBatchSql(choiceList);
                }
                if (titleList.Count > 0)
                {
                    //矩阵标题
                    _dbContext.DeleteExec(nameof(SurArrayTitle), where, param);
                    _dbContext.InsertBatchSql(titleList);
                }
            }
        }

        private void PublishJson(SurFilter surFilter, Survey survey, List<SurQuestion> qsfList, List<SurQuestionChoice> choiceList, List<SurArrayTitle> titleList, JObject publishRRObj)
        {
            //生成发布json
            JObject jObj = new JObject();
            jObj["survey_id"] = survey.Fid;
            jObj["redirect_relation"] = publishRRObj;
            if (qsfList.Any())
            {
                JArray arryQsts = new JArray();
                var qg = qsfList.GroupBy(a => a.PageNum);
                foreach (var qpg in qg)
                {
                    JObject jPage = new JObject();
                    jPage["index"] = qpg.Key;
                    var qestions = qpg.ToList();
                    JArray qsfs = new JArray();
                    foreach (var q in qestions)
                    {
                        //跳过分页
                        if (q.TypeId == "11")
                            continue;
                        JObject jList = new JObject();
                        jList["id"] = q.Fid;
                        jList["survey_id"] = survey.Fid;
                        jList["content"] = q.SortIndex + "." + q.Content;
                        jList["type_id"] = q.TypeId;
                        jList["order"] = q.SortIndex;
                        jList["has_other"] = q.HasOther == 0 ? "N" : "Y";
                        jList["required"] = q.Required == 0 ? "N" : "Y";
                        jList["title_quote"] = q.TitleQuote == 0 ? "N" : "Y";
                        jList["choice_quote"] = q.ChoiceQuote;
                        jList["page"] = q.PageNum;
                        jList["max"] = q.MaxValue;
                        jList["min"] = q.MinValue;
                        jList["exclusive_options"] = "";
                        jList["logic_condition"] = null;
                        jList["is_logic_show_question"] = "0";
                        jList["vote_type_id"] = "0";
                        jList["type"] = QuestionType(q.TypeId);
                        jList["logic_hide"] = "0";

                        //以下特殊处理
                        if (q.RedirectRelation != null)
                        {
                            jList["redirect_relation"] = JArray.Parse(q.RedirectRelation);
                        }
                        else
                        {
                            jList["redirect_relation"] = "";
                        }
                        var choices = choiceList.Where(c => c.QuestionUid == q.Fid);
                        if (choices != null && choices.Any())
                        {
                            JArray arryC = new JArray();
                            foreach (var c in choices)
                            {
                                JObject jC = new JObject();
                                jC["id"] = c.Fid;
                                jC["question_id"] = q.Fid;
                                jC["order"] = c.SortIndex;
                                jC["real_choice_id"] = c.ChoiceAbsoluteId;
                                jC["content"] = c.Content;
                                jC["value"] = "0";
                                jC["is_other"] = c.IsOther == 0 ? "N" : "Y";
                                jC["required"] = c.Required;
                                arryC.Add(jC);
                            }
                            jList["choice"] = arryC;

                        }
                        else
                        {
                            jList["choice"] = false;
                        }
                        var titles = titleList.Where(c => c.QuestionUid == q.Fid);
                        if (titles != null && titles.Any())
                        {
                            JArray arryTitle = new JArray();
                            foreach (var t in titles)
                            {
                                JObject jt = new JObject();
                                jt["id"] = t.Fid;
                                jt["question_id"] = q.Fid;
                                jt["content"] = t.Content;
                                jt["order"] = t.SortIndex;
                                jt["other_content"] = new JArray();
                                //"id": "66425", 
                                //"question_id": "242193", 
                                //"content": "矩阵行2", 
                                //"order": "2", 
                                //"other_content": [ ]
                                arryTitle.Add(jt);
                            }
                            if (q.TypeId == "9")
                            {
                                jList["radio_array_title"] = arryTitle;
                            }
                            if (q.TypeId == "13")
                            {
                                jList["checkbox_array_title"] = arryTitle;
                            }

                        }

                        jList["value"] = null;

                        string typeid = q.TypeId;
                        if (typeid == "11")
                        {

                        }
                        else if (typeid == "8" || typeid == "9" || typeid == "15")
                        {
                            //8多选or9矩阵,15单选图片
                            //var jchoice = choiceList.Where(c => c.QuestionUid == q.Fid);
                            if (choices != null)
                            {
                                //string values = "[";
                                JArray arrV = new JArray();
                                for (int i = 0; i < choices.Count(); i++)
                                {
                                    if (typeid == "8" || typeid == "15")
                                    {
                                        arrV.Add(false);
                                        //values += "false,";
                                    }
                                    else if (typeid == "9")
                                    {
                                        arrV.Add(null);
                                        //values += "null,";
                                    }
                                }
                                //values = values.TrimEnd(',') + "]";

                                jList["value"] = arrV;// JArray.Parse(values);
                            }
                        }
                        else if (typeid == "13")
                        {
                            //多选矩阵
                            //string values = "[";
                            var jtitle = titleList.Where(t => t.QuestionUid == q.Fid);
                            JArray arrV = new JArray();
                            for (int i = 0; i < jtitle.Count(); i++)
                            {
                                JArray arrVC = new JArray();
                                //values += "[";
                                for (int j = 0; j < choices.Count(); j++)
                                {
                                    arrVC.Add(false);
                                    //values += "false,";
                                }
                                //values = values.TrimEnd(',') + "]";
                                arrV.Add(arrVC);
                            }
                            jList["value"] = arrV;// JArray.Parse(values);
                        }
                        else
                        {
                            jList["value"] = null;
                        }
                        qsfs.Add(jList);
                    }
                    jPage["list"] = qsfs;
                    arryQsts.Add(jPage);
                }
                jObj["pages"] = arryQsts;
                jObj["page_count"] = qg.Count();
            }

            jObj["type"] = 0;
            jObj["scene"] = 2;

            survey.JSONPublish = jObj.ToString();
            survey.SurStatus = SurveyStatus.Publishing;
            survey.PublishTime = surFilter.PublishTime;
            survey.SurStartDate = surFilter.SurStartDate;
            survey.SurEndDate = surFilter.SurEndDate;
            survey.Amounted = surFilter.Amounted;
            _dbContext.Update<Survey>(survey);
        }

        /// <summary>
        /// 收集个人问卷结果
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public bool SaveSelfCollectionSurvey(JObject jobj)
        {
            string surveyUid = jobj.GetStringValue("survey_id");
            //var referer= jobj.GetStringValue("referer");
            string empUid = _applicationContext.EmpUid;
            string userUid = _applicationContext.UserUid;
            //获取所有问题
            DynamicParameters param = new DynamicParameters();
            param.Add("SurveyUid", surveyUid);
            Survey survey = _dbContext.Get<Survey>(surveyUid);
            var questions = _dbContext.QueryWhere<SurQuestion>("SurveyUid=@SurveyUid", param).OrderBy(c => c.AbsoluteId);
            var titles = _dbContext.QueryWhere<SurArrayTitle>("SurveyUid=@SurveyUid", param);
            IEnumerable<JProperty> dicResponse = jobj.Properties();
            if (dicResponse != null && dicResponse.Any() && questions != null && questions.Any())
            {
                List<SurResult> results = new List<SurResult>();
                foreach (var q in questions)
                {
                    string key = "response_" + q.AbsoluteId;

                    //矩阵单选
                    if (q.TypeId == "9")
                    {
                        var ts = titles.Where(t => t.QuestionUid == q.Fid);
                        if (ts != null && ts.Any())
                        {
                            foreach (var t in ts)
                            {
                                SurResult sr = new SurResult();
                                sr.SurveyUid = surveyUid;
                                sr.QuestionUid = q.Fid;
                                sr.TitleUid = t.Fid;
                                string tkey = key + "_" + t.Fid;
                                var qoption = dicResponse.FirstOrDefault(p => p.Name == tkey);
                                if (qoption != null)
                                {
                                    sr.Answer = qoption.Value.ToString();
                                }
                                sr.Answers = "\"" + sr.Answer + "\"";
                                sr.UserUid = userUid;
                                sr.EmpUid = empUid;
                                sr.FillDate = DateTimeUtils.CurrentDateTimeStr;
                                results.Add(sr);
                            }
                        }
                    }
                    else if (q.TypeId == "13")
                    {
                        //矩阵多选
                        var ts = titles.Where(t => t.QuestionUid == q.Fid);
                        if (ts != null && ts.Any())
                        {
                            foreach (var t in ts)
                            {
                                string tkey = key + "_" + t.Fid;
                                var qoption = dicResponse.FirstOrDefault(p => p.Name == tkey);
                                if (qoption != null)
                                {
                                    foreach (JToken item in qoption.Value)
                                    {
                                        SurResult sr = new SurResult();
                                        sr.SurveyUid = surveyUid;
                                        sr.QuestionUid = q.Fid;
                                        sr.TitleUid = t.Fid;
                                        sr.Answer = item.ToString();
                                        sr.UserUid = userUid;
                                        sr.EmpUid = empUid;
                                        sr.FillDate = DateTimeUtils.CurrentDateTimeStr;
                                        sr.Answers = qoption.Value.ToString();
                                        results.Add(sr);
                                    }

                                }
                            }
                        }
                    }
                    else if (q.TypeId == "8" || q.TypeId == "15")
                    {
                        //多选,多选图片
                        var qexist = dicResponse.FirstOrDefault(p => p.Name == key);
                        if (qexist != null)
                        {
                            JArray qc = qexist.Value as JArray;
                            if (qc != null && qc.Count > 0)
                            {
                                foreach (JToken item in qc)
                                {
                                    SurResult sr = new SurResult();
                                    sr.SurveyUid = surveyUid;
                                    sr.QuestionUid = q.Fid;
                                    sr.Answer = item.ToString();
                                    string okey = "other_" + q.AbsoluteId + "_" + item.ToString();
                                    var qov = dicResponse.FirstOrDefault(p => p.Name == okey);
                                    if (qov != null)
                                    {
                                        sr.AnswerOther = qov.Value.ToString();
                                    }
                                    sr.Answers = qc.ToString();
                                    sr.UserUid = userUid;
                                    sr.EmpUid = empUid;
                                    sr.FillDate = DateTimeUtils.CurrentDateTimeStr;
                                    results.Add(sr);
                                }
                            }
                            //string keyOther = "other_" + q.AbsoluteId + "_" + sr.Answer;
                            //var qother = dicResponse.FirstOrDefault(p => p.Name == keyOther);
                            //if (qother != null)
                            //{
                            //    sr.AnswerOther = qother.Value.ToString();
                            //}
                        }
                    }
                    else
                    {
                        SurResult sr = new SurResult();
                        sr.SurveyUid = surveyUid;
                        sr.QuestionUid = q.Fid;
                        var qexist = dicResponse.FirstOrDefault(p => p.Name == key);
                        if (qexist != null)
                        {
                            sr.Answer = qexist.Value.ToString();
                            sr.Answers = "\"" + sr.Answer + "\"";
                            string keyOther = "other_" + q.AbsoluteId + "_" + sr.Answer;
                            var qother = dicResponse.FirstOrDefault(p => p.Name == keyOther);
                            if (qother != null)
                            {
                                sr.AnswerOther = qother.Value.ToString();
                            }
                        }
                        sr.UserUid = userUid;
                        sr.EmpUid = empUid;
                        sr.FillDate = DateTimeUtils.CurrentDateTimeStr;
                        results.Add(sr);
                    }

                }
                DateTime startTime = DateTimeUtils.ToDateTime(jobj.GetStringValue("time"));
                SurResponseList sul = new SurResponseList();
                sul.SurveyUid = surveyUid;
                sul.UserUid = userUid;
                sul.EmpUid = empUid;
                sul.SubmitTime = DateTimeUtils.CurrentDateTimeStr;
                TimeSpan tspsn = DateTime.Now.Subtract(startTime);
                sul.TimeLength = tspsn.Duration().TotalSeconds;
                if (sul.TimeLength > 60 && sul.TimeLength <= 3600)
                {
                    sul.TimeLenDesc = tspsn.Minutes + "分" + tspsn.Seconds + "秒";
                }
                else if (sul.TimeLength <= 60)
                {
                    sul.TimeLenDesc = tspsn.Seconds + "秒";
                }
                else if (sul.TimeLength > 3600)
                {
                    sul.TimeLenDesc = tspsn.Hours + "小时" + tspsn.Minutes + "分" + tspsn.Seconds + "秒";
                }
                else
                {
                    sul.TimeLenDesc = "0秒";
                }
                sul.ResponseStatus = "2";
                sul.IPAddress = _applicationContext.ClientIpAddress;

                sul.StartTime = jobj.GetStringValue("time");

                int count = _dbContext.Count(nameof(SurResponseList), "SurveyUid=@SurveyUid", param);

                survey.CollectionAmount = count + 1;
                survey.Completed = survey.CollectionAmount + "/" + survey.Amounted;

                sul.SortIndex = survey.CollectionAmount;
                //更新有效答题数量
                _dbContext.Update<Survey>(survey);
                //更新答题用户列表
                sul.Fid = UUIDUtils.Fid;
                _dbContext.Insert(sul);
                //更新答题结果
                results.ForEach((r) =>
                {
                    r.ResponseUid = sul.Fid;
                });
                _dbContext.InsertBatchSql(results);
                //移除报表缓存
                _cacheService.Remove(FAP_SURVEY_RESPONSELIST + survey.Fid);
                return true;
            }
            else
            {
                return false;
            }

        }
        #region 问卷报表

        /// <summary>
        /// 获取项目
        /// </summary>
        /// <param name="survey"></param>
        /// <returns></returns>
        public JObject GetProject(Survey survey)
        {
            JObject jProject = new JObject();
            jProject["id"] = survey.Fid;
            jProject["accept_standard"] = "survey";
            jProject["end_date"] = survey.SurEndDate;
            jProject["manager_id"] = null;
            jProject["pro_name"] = survey.SurName;
            jProject["pro_type"] = "2";
            jProject["eva_type"] = "0";
            jProject["test_type"] = "100";
            jProject["publisher_id"] = survey.UpdateBy;
            jProject["source_url"] = null;
            jProject["start_date"] = survey.SurStartDate;
            jProject["status"] = survey.SurStatus;
            jProject["eva_show_status"] = "1";
            jProject["poster"] = null;
            jProject["test_content"] = survey.SurContent;
            jProject["test_guide"] = null;
            jProject["test_env"] = null;
            jProject["is_alltester"] = "1";
            jProject["tester_count"] = "0";
            jProject["tester_max_count"] = null;
            jProject["tester_min_count"] = null;
            jProject["agree_rate"] = null;
            jProject["score_que_num"] = null;
            jProject["score_detail_rate"] = null;
            jProject["score_detail_score"] = null;
            jProject["score_detail_cash"] = null;
            jProject["additional_info"] = null;
            jProject["browser"] = null;
            jProject["firewall"] = null;
            jProject["manufacture"] = null;
            jProject["network"] = null;
            jProject["network_provider"] = null;
            jProject["phone_type"] = null;
            jProject["sys_type"] = null;
            jProject["wareless"] = null;
            jProject["created"] = survey.CreateDate;
            jProject["updated"] = survey.UpdateDate;
            jProject["need_review"] = "0";
            jProject["score"] = "0";
            jProject["bonus_score"] = "0";
            jProject["shidi_score"] = null;
            jProject["shidi_award"] = null;
            jProject["survey_url"] = null;
            jProject["survey_tag"] = null;
            jProject["survey_id"] = null;
            jProject["survey_import"] = "0";
            jProject["region"] = null;
            jProject["bug_tpl_id"] = null;//模板 
            jProject["bug_permission"] = "1";
            jProject["gift_score"] = "0";
            jProject["productline_id"] = "0";
            jProject["reject_user_id"] = null;
            jProject["reject_reason"] = null;
            jProject["reject_time"] = null;
            jProject["submit_user_id"] = survey.UpdateBy;
            jProject["submit_time"] = survey.UpdateDate;
            jProject["publish_user_id"] = survey.UpdateBy;
            jProject["publish_time"] = survey.PublishTime;
            jProject["close_user_id"] = null;
            jProject["close_time"] = null;
            jProject["delete_user_id"] = "0";
            jProject["delete_time"] = null;
            jProject["filter_type"] = "3";
            jProject["invited_condition_text"] = null;
            jProject["invited_condition"] = null;
            jProject["invited_count"] = "0";
            jProject["rank"] = null;
            jProject["eva_status"] = "0";
            jProject["exam"] = "0";
            jProject["internal"] = "0";
            jProject["enable_forum"] = "1";
            jProject["group_id"] = "7";
            jProject["auto_update"] = "0";
            jProject["test_step"] = null;
            jProject["recommend_type"] = "0";
            jProject["recommend_groupid"] = null;
            jProject["recommend_count"] = null;
            jProject["auto_recommend"] = null;
            jProject["show_normal"] = "0";
            jProject["type"] = "3";
            jProject["zone_type"] = "0";
            jProject["pre_publish"] = "0";
            jProject["export"] = "0";
            jProject["agree_rate_small"] = "0";
            jProject["tip"] = "0";
            jProject["eva_team"] = "-1";
            jProject["epiboly"] = "0";
            jProject["ensure_gold"] = "0";
            jProject["valid"] = "1";
            jProject["interface_user"] = survey.CreateBy;
            jProject["interface_department"] = "无";
            jProject["user_filter"] = null;
            jProject["version"] = null;
            jProject["project_url"] = null;
            jProject["test_url"] = null;
            jProject["app_group_id"] = "0";
            jProject["mobile_hide"] = "0";
            jProject["associate_status"] = "0";
            jProject["zhongce_user"] = survey.CreateBy;
            jProject["online_time"] = GetOnlineTime(DateTimeUtils.ToDateTime(survey.PublishTime));
            jProject["volid_count"] = survey.CollectionAmount;
            return jProject;
        }
        private string GetOnlineTime(DateTime publishTime)
        {
            var ts = DateTime.Now.Subtract(publishTime).Duration();
            return ts.Days.ToString() + "天" + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟" + ts.Seconds.ToString() + "秒";
        }
        /// <summary>
        /// 获取问卷问题列表
        /// </summary>
        /// <param name="survey"></param>
        /// <returns></returns>
        public JArray GetSurveyQuestion(Survey survey)
        {
            JArray jQuestion = new JArray();
            JObject jSurvey = JObject.Parse(survey.JSONPublish);
            JArray jPages = jSurvey.GetValue("pages") as JArray;
            if (jPages != null && jPages.Any())
            {
                foreach (JObject page in jPages)
                {
                    JArray jList = page.GetValue("list") as JArray;
                    if (jList != null && jList.Any())
                    {
                        foreach (JObject req in jList)
                        {
                            req["absolute_id"] = req.GetValue("id");
                            req.Remove("value");
                            if (req.HasKey("choice"))
                            {
                                JArray arrayChoice = req.GetValue("choice") as JArray;
                                if (arrayChoice != null && arrayChoice.Any())
                                {
                                    foreach (JObject choice in arrayChoice)
                                    {
                                        choice["choice_absolute_id"] = choice.GetValue("id");
                                    }
                                }
                            }
                            jQuestion.Add(req);
                        }
                    }
                }
            }
            return jQuestion;
        }
        /// <summary>
        /// 获取调查问卷统计结果JSON
        /// </summary>
        /// <param name="surveyUid"></param>
        /// <param name="sortIndex"></param>
        /// <returns></returns>
        public JObject GetSurveyReportResult(string surveyUid, int sortIndex)
        {
            Survey survey = _dbContext.Get<Survey>(surveyUid);
            JObject jSurResult = new JObject();
            //项目信息
            JObject jInfo = GetProject(survey);
            jInfo.Remove("online_time");
            jInfo.Remove("volid_count");
            jInfo["creater_id"] = survey.CreateBy;
            jInfo["download"] = 0;
            jInfo["static_count"] = survey.CollectionAmount;
            jInfo["foreword"] = "欢迎参加调查！答卷数据仅用于统计分析，请放心填写。题目选项无对错之分，按照实际情况选择即可。感谢您的帮助！";
            jInfo["title"] = survey.SurName;
            jSurResult["info"] = jInfo;

            DynamicParameters param = new DynamicParameters();
            param.Add("SurveyUid", survey.Fid);
            var questions = _dbContext.QueryWhere<SurQuestion>("SurveyUid=@SurveyUid", param).OrderBy(c => c.SortIndex);
            //List<SurQuestionChoice> choices = da.QueryWhere<SurQuestionChoice>("SurveyUid=@SurveyUid", param);
            //List<SurArrayTitle> titles = da.QueryWhere<SurArrayTitle>("SurveyUid=@SurveyUid", param);
            if (sortIndex == 0)
            {
                JArray arryQuestion = GetReportQuestionJson(questions.First());
                jSurResult["question"] = arryQuestion;
            }
            else
            {
                SurQuestion question = questions.FirstOrDefault<SurQuestion>(c => c.SortIndex == sortIndex);
                JArray arryQuestion = GetReportQuestionJson(question);
                jSurResult["question"] = arryQuestion;
            }


            return jSurResult;

        }
        /// <summary>
        /// 获取报表问题结果统计
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        private JArray GetReportQuestionJson(SurQuestion question)
        {
            JArray arryQuestion = new JArray();
            JObject jquestion = new JObject();
            arryQuestion.Add(jquestion);
            JArray jId = new JArray();
            jId.Add(question.Fid);
            jquestion["id"] = jId;
            jquestion["survey_id"] = question.SurveyUid;
            jquestion["content"] = question.SortIndex + "." + question.Content;
            jquestion["type_id"] = question.TypeId;
            jquestion["order"] = question.SortIndex + 1;
            jquestion["type"] = QuestionType(question.TypeId);
            jquestion["all_questions_count"] = question.LastAbsoluteId;

            var results = GetResponseResult(question.SurveyUid, question.Fid);
            if (question.TypeId == "6" || question.TypeId == "7" || question.TypeId == "14" || question.TypeId == "8" || question.TypeId == "15")
            {
                //单选题、选择列表
                JObject jResponse = new JObject();
                if (question.TypeId == "8" || question.TypeId == "15")
                {
                    jquestion["response_multiple"] = jResponse;
                }
                else
                {
                    jquestion["response_single"] = jResponse;
                }
                //获取满足条件的问卷填写总数
                int total = GetTotalBySurveyUid(question.SurveyUid);
                //获取回答此问题的人数
                int answer = GetQuestionAnswerCount(question.SurveyUid, question.Fid);
                string percent = total == 0 ? "0%" : Math.Round(answer * 100 / (double)total, 2) + "%";
                jResponse["total"] = total;
                jResponse["answered"] = answer;
                jResponse["percent"] = percent;
                JArray arrayChoice = new JArray();
                jResponse["choice"] = arrayChoice;
                var choices = GetQuestionChoice(question.Fid);
                foreach (var choice in choices)
                {
                    JObject jChoice = new JObject();
                    JArray jCid = new JArray();
                    jCid.Add(choice.Fid);
                    jChoice["id"] = jCid;
                    jChoice["question_id"] = question.Fid;
                    jChoice["order"] = choice.SortIndex;
                    jChoice["content"] = choice.Content;
                    int cCount = results.Count(c => c.Answer == choice.Fid);
                    jChoice["selected"] = cCount;
                    jChoice["selected_percent"] = answer == 0 ? "0%" : Math.Round(cCount * 100 / (double)answer, 2) + "%";
                    arrayChoice.Add(jChoice);
                }
                var others = choices.Where(c => c.IsOther == 1);
                JArray jOther = new JArray();
                jResponse["others"] = jOther;
                if (others != null && others.Any())
                {
                    foreach (var item in others)
                    {
                        JObject jo = new JObject();
                        jo["choice_title"] = item.Content;
                        JArray arro = new JArray();
                        jo["other_response_list"] = arro;
                        var rs = results.Where(r => r.Answer == item.Fid);
                        if (rs != null && rs.Any())
                        {
                            foreach (var r in rs)
                            {
                                JObject jot = new JObject();
                                jot["response_id"] = r.Fid;
                                jot["content"] = r.AnswerOther;
                                jot["question_id"] = r.QuestionUid;
                                jot["choice_id"] = r.Answer;
                                jot["user"] = r.CreateName;
                                arro.Add(jot);
                            }
                        }
                    }
                }


            }
            else if (question.TypeId == "1" || question.TypeId == "2")
            {
                //单行填空、多行填空
                JObject jResponse = new JObject();
                jquestion["response_text"] = jResponse;
                jResponse["uploadCount"] = "0";
                int total = GetTotalBySurveyUid(question.SurveyUid);
                int answer = GetQuestionAnswerCount(question.SurveyUid, question.Fid);
                string percent = total == 0 ? "0%" : Math.Round(answer * 100 / (double)total, 2) + "%";
                jResponse["total"] = total;
                jResponse["answered"] = answer;
                jResponse["percent"] = percent;
                JArray arrayResult = new JArray();
                jResponse["response_list"] = arrayResult;
                foreach (var result in results)
                {
                    JObject jResult = new JObject();

                    jResult["response_id"] = result.Fid;
                    jResult["question_id"] = result.QuestionUid;
                    jResult["content"] = result.Answer;
                    jResult["user"] = result.CreateName;
                    arrayResult.Add(jResult);
                }
            }
            else if (question.TypeId == "9" || question.TypeId == "13")
            {
                //单选矩阵,多选矩阵               
                JArray arrayChoice = new JArray();
                jquestion["choice"] = arrayChoice;
                var choices = GetQuestionChoice(question.Fid);
                foreach (var choice in choices)
                {
                    JObject jChoice = new JObject();
                    JArray jCid = new JArray();
                    jCid.Add(choice.Fid);
                    jChoice["id"] = jCid;
                    jChoice["question_id"] = question.Fid;
                    jChoice["order"] = choice.SortIndex;
                    jChoice["real_choice_id"] = "0";
                    jChoice["content"] = choice.Content;
                    jChoice["value"] = "0";
                    jChoice["is_other"] = choice.IsOther == 0 ? "N" : "Y";
                    arrayChoice.Add(jChoice);
                }
                int total = GetTotalBySurveyUid(question.SurveyUid);
                int answer = GetQuestionAnswerTitleCount(question.SurveyUid, question.Fid);
                JArray arrayTitle = new JArray();
                if (question.TypeId == "9")
                {
                    jquestion["response_radio_array"] = arrayTitle;
                }
                else
                {
                    jquestion["response_checkbox_array"] = arrayTitle;
                }
                var titles = GetQuestionTitle(question.Fid);
                foreach (var title in titles)
                {
                    JObject jTitle = new JObject();
                    JArray jCid = new JArray();
                    jCid.Add(title.Fid);
                    jTitle["id"] = jCid;
                    jTitle["question_id"] = question.Fid;
                    jTitle["order"] = title.SortIndex;
                    jTitle["content"] = title.Content;
                    //int cCount = results.Count(c => c.TitleUid == title.Fid);
                    jTitle["total"] = total;
                    jTitle["answered"] = answer;
                    jTitle["percent"] = answer == 0 ? "0%" : Math.Round(answer * 100 / (double)total, 2) + "%";

                    JArray jtchoice = new JArray();
                    jTitle["choice"] = jtchoice;
                    foreach (var choice in choices)
                    {
                        int tCount = results.Count(c => c.TitleUid == title.Fid);
                        int cCount = results.Count(c => c.Answer == choice.Fid && c.TitleUid == title.Fid);
                        JObject jtc = new JObject();
                        jtc["selected"] = cCount;
                        jtc["selected_percent"] = tCount == 0 ? "0%" : Math.Round(cCount * 100 / (double)tCount, 2) + "%";
                        jtchoice.Add(jtc);
                    }


                    arrayTitle.Add(jTitle);
                }
            }

            return arryQuestion;
        }
        /// <summary>
        /// 获取报表过滤条件
        /// </summary>
        /// <param name="surveyUid"></param>
        /// <returns></returns>
        public JArray GetReportFilter(string surveyUid)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("SurveyUid", surveyUid);
            var list = _dbContext.QueryWhere<SurReportFilter>("SurveyUid=@SurveyUid", param);
            JArray jfs = new JArray();
            if (list != null && list.Any())
            {
                foreach (var rf in list)
                {
                    jfs.Add(SurReportFilterToJSON(rf));
                }
            }
            return jfs;
        }
        /// <summary>
        /// 获取填写问卷人员总数
        /// </summary>
        /// <param name="surveyUid"></param>
        /// <returns></returns>
        private int GetTotalBySurveyUid(string surveyUid)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("SurveyUid", surveyUid);
            //
            string strWhere = "SurveyUid=@SurveyUid";
            //报表筛选条件
            string where = GetReportFilterSql(surveyUid);
            if (where.IsPresent())
            {
                return _dbContext.Count(nameof(SurResult), where);

            }
            else
            {
                return _dbContext.Count(nameof(SurResponseList), strWhere, param);
            }
        }
        /// <summary>
        /// 报表过滤条件
        /// </summary>
        /// <param name="surveyUid"></param>
        /// <returns></returns>
        private string GetReportFilterSql(string surveyUid)
        {

            List<string> whereList = new List<string>();
            DynamicParameters param = new DynamicParameters();
            param.Add("SurveyUid", surveyUid);
            var listFilter = _dbContext.QueryWhere<SurReportFilter>("SurveyUid=@SurveyUid", param);

            foreach (var filter in listFilter)
            {
                StringBuilder sbWhere = new StringBuilder();
                string condition = filter.ConditionId;
                sbWhere.Append("(");
                sbWhere.Append("QuestionUid='");
                sbWhere.Append(filter.QuestionUid);
                if (condition == "1")
                {
                    sbWhere.Append("' and Answers like '%");
                }
                else
                {
                    sbWhere.Append("' and Answers not like '%");
                }
                sbWhere.Append("\"");
                sbWhere.Append(filter.ChoiceUid);
                sbWhere.Append("\"");
                sbWhere.Append("%'");
                sbWhere.Append(")");
                whereList.Add(sbWhere.ToString());
            }

            if (whereList.Any())
            {
                return string.Join(" and ", whereList);
            }
            return "";
        }
        /// <summary>
        /// 获取问题回答数
        /// </summary>
        /// <param name="choiceUid"></param>
        /// <returns></returns>
        private int GetQuestionAnswerCount(string surveyUid, string questionUid)
        {
            //报表筛选条件
            string where = GetReportFilterSql(surveyUid);
            string sql = $"select {nameof(SurResult.QuestionUid)},{nameof(SurResult.UserUid)},{nameof(SurResult.Answer)} from {nameof(SurResult)} where {nameof(SurResult.QuestionUid)}=@QuestionUid";
            if (where.IsPresent())
            {
                sql += $" and  {nameof(SurResult.UserUid)} in (select  {nameof(SurResult.UserUid)} from {nameof(SurResult)} where " + where + ")";
            }

            return _dbContext.Query(sql, new DynamicParameters(new { QuestionUid = questionUid })).Distinct().Count();
        }
        /// <summary>
        /// 获取矩阵问题回答数
        /// </summary>
        /// <param name="questionUid"></param>
        /// <returns></returns>
        private int GetQuestionAnswerTitleCount(string surveyUid, string questionUid)
        {
            //报表筛选条件
            string where = GetReportFilterSql(surveyUid);
            string sql = $"select  {nameof(SurResult.QuestionUid)},{nameof(SurResult.CreateBy)} from {nameof(SurResult)} where {nameof(SurResult.QuestionUid)}=@QuestionUid";
            if (where.IsPresent())
            {
                sql += $" and  {nameof(SurResult.UserUid)} in (select  {nameof(SurResult.UserUid)} from {nameof(SurResult)} where " + where + ")";
            }
            return _dbContext.Query(sql, new DynamicParameters(new { QuestionUid = questionUid })).Distinct().Count();
        }
        /// <summary>
        /// 获取问题的选择项
        /// </summary>
        /// <param name="questionUid"></param>
        /// <returns></returns>
        private IEnumerable<SurQuestionChoice> GetQuestionChoice(string questionUid)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("QuestionUid", questionUid);
            var choices = _dbContext.QueryWhere<SurQuestionChoice>("QuestionUid=@QuestionUid", param).OrderBy(c => c.SortIndex);
            return choices;
        }
        /// <summary>
        /// 获取矩阵标题
        /// </summary>
        /// <param name="questionUid"></param>
        /// <returns></returns>
        private IEnumerable<SurArrayTitle> GetQuestionTitle(string questionUid)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("QuestionUid", questionUid);
            var titles = _dbContext.QueryWhere<SurArrayTitle>("QuestionUid=@QuestionUid", param);
            return titles;
        }
        /// <summary>
        /// 获取问题问卷结果
        /// </summary>
        /// <param name="choiceUid"></param>
        /// <returns></returns>
        private IEnumerable<SurResult> GetResponseResult(string surveyUid, string questionUid)
        {
            //报表筛选条件
            string where = GetReportFilterSql(surveyUid);
            DynamicParameters param = new DynamicParameters();
            param.Add("QuestionUid", questionUid);
            string strWhere = "QuestionUid=@QuestionUid";
            if (where.IsPresent())
            {
                strWhere += $" and  {nameof(SurResult.UserUid)} in (select  {nameof(SurResult.UserUid)} from {nameof(SurResult)} where " + where + ")";
            }
            return _dbContext.QueryWhere<SurResult>(strWhere, param);

        }
        [Transactional]
        public IEnumerable<SurReportFilter> SaveReportFilters(string surveyUid, string filters)
        {
            List<SurReportFilter> updates = new List<SurReportFilter>();
            List<SurReportFilter> adds = new List<SurReportFilter>();
            //string surveyUid = jFilters.GetStringValue("survey_id");
            DynamicParameters param = new DynamicParameters();
            param.Add("SurveyUid", surveyUid);
            var rfs = _dbContext.QueryWhere<SurReportFilter>("SurveyUid=@SurveyUid", param);
            JArray arrFilters = JArray.Parse(filters);
            if (arrFilters != null && arrFilters.Any())
            {
                foreach (JObject filter in arrFilters)
                {
                    SurReportFilter model = new SurReportFilter();
                    if (filter.HasKey("status"))
                    {
                        string fid = filter.GetStringValue("id");
                        var eModel = rfs.FirstOrDefault<SurReportFilter>(f => f.Fid == fid);
                        if (eModel != null)
                        {
                            model = eModel;
                            updates.Add(model);
                        }
                        else
                        {
                            adds.Add(model);
                        }
                    }
                    else
                    {
                        adds.Add(model);
                    }
                    model.SurveyUid = surveyUid;
                    model.ChoiceUid = filter.GetStringValue("choice_id");
                    model.TitleUid = filter.GetStringValue("title_id");
                    model.QuestionUid = filter.GetStringValue("question_id");
                    model.ConditionId = filter.GetStringValue("condition_id");
                    model.TypeId = filter.GetIntValue("type_id");
                }
            }

            if (updates.Any())
            {
                _dbContext.UpdateBatchSql<SurReportFilter>(updates);
            }
            if (adds.Any())
            {
                _dbContext.InsertBatchSql(adds);
            }

            return _dbContext.QueryWhere<SurReportFilter>("SurveyUid=@SurveyUid", param);
        }
        public JObject SurReportFilterToJSON(SurReportFilter filter)
        {
            JObject jFilter = new JObject();

            jFilter["id"] = filter.Fid;
            jFilter["survey_id"] = filter.SurveyUid;
            jFilter["question_id"] = filter.QuestionUid;
            jFilter["condition_id"] = filter.ConditionId;
            jFilter["choice_id"] = filter.ChoiceUid;
            jFilter["title_id"] = filter.TitleUid;
            jFilter["question_type"] = filter.TypeId;
            jFilter["status"] = "1";
            jFilter["create_time"] = filter.CreateDate;
            jFilter["update_time"] = filter.UpdateDate;
            return jFilter;

        }
        #endregion

        #region 个人问卷答题情况

        public JObject GetResponseList(string surveyUid)
        {
            JObject jUser = new JObject();
            string where = "SurveyUid=@SurveyUid";
            DynamicParameters param = new DynamicParameters();
            param.Add("SurveyUid", surveyUid);
            var filter = _dbContext.QueryFirstOrDefaultWhere<SurFilter>(where, param);
            bool isAnonymous = true;
            if (filter != null && filter.FilterModel.IsMissing())
            {
                isAnonymous = false;
            }
            var users = _dbContext.QueryWhere<SurResponseList>(where, param, true);
            if (users.Any())
            {
                //jUser["target"] = filter.Amounted;
                jUser["count"] = users.Count();
                JArray jList = new JArray();
                jUser["list"] = jList;
                int i = users.Count();
                foreach (var user in users)
                {
                    i--;
                    JObject ju = new JObject();
                    if (i != 0)
                    {
                        ju["user_id"] = "-" + i;
                    }
                    else
                    {
                        ju["user_id"] = "0";
                    }
                    ju["time"] = user.SubmitTime;
                    ju["submit_time"] = user.SubmitTime;
                    if (isAnonymous)
                    {
                        ju["name"] = "***";
                    }
                    else
                    {
                        ju["name"] = user.EmpUidMC;
                    }
                    ju["status"] = "2";
                    ju["res_id"] = user.Fid;
                    ju["answer_time"] = user.TimeLength;
                    jList.Add(ju);
                }
                jUser["error_code"] = 0;
            }
            else
            {
                jUser["count"] = 0;
                jUser["list"] = new JArray();

                jUser["error_code"] = 0;
            }
            return jUser;
        }
        /// <summary>
        /// 获取用户答卷详情
        /// </summary>
        /// <param name="responseUid">答卷用户</param>
        /// <param name="surveyUid">问卷</param>
        /// <param name="status">未使用</param>
        /// <param name="sortIndex">要获取的排序号</param>
        /// <returns></returns>
        public JObject GetSurveyResponse(string responseUid, string surveyUid, string status, int sortIndex)
        {

            Survey survey = _dbContext.Get<Survey>(surveyUid);
            string where = "SurveyUid=@SurveyUid";
            DynamicParameters param = new DynamicParameters();
            param.Add("SurveyUid", surveyUid);
            string key = FAP_SURVEY_RESPONSELIST+survey.Fid;
            List<SurResponseList> responseList = _cacheService.Get<List<SurResponseList>>(key);
            if (responseList == null)
            {
                responseList = _dbContext.QueryWhere<SurResponseList>(where, param, true).OrderBy(c => c.SortIndex).ToList();
                _cacheService.Add(key, responseList, TimeSpan.FromMinutes(5));
            }
            var filter = _dbContext.QueryFirstOrDefaultWhere<SurFilter>(where, param);
            bool isAnonymous = true;
            if (filter != null && filter.FilterModel.IsMissing())
            {
                isAnonymous = false;
            }
            SurResponseList currSurResponse = responseList[sortIndex - 1];
            JObject jSurResult = new JObject();
            //项目信息
            JObject jInfo = new JObject();
            jInfo["id"] = currSurResponse.Fid;
            jInfo["survey_id"] = surveyUid;
            jInfo["time"] = currSurResponse.StartTime;
            jInfo["submit_time"] = currSurResponse.SubmitTime;
            jInfo["user_id"] = currSurResponse.UserUid;
            jInfo["IP"] = currSurResponse.IPAddress;
            jInfo["referer"] = "";
            jInfo["fap_id"] = currSurResponse.EmpUid;
            jInfo["telephone"] = "";
            jInfo["valid"] = "I";
            jInfo["status"] = "2";
            jInfo["agent_status"] = "0";
            jInfo["pre_filter_status"] = "0";
            jInfo["pre_filter_invalid_type"] = "0";
            jInfo["invalid_type"] = "0";
            jInfo["score"] = "0";
            jInfo["cash"] = "0";
            jInfo["is_mobile"] = "0";
            jInfo["cheat_status"] = "0";
            jInfo["check_status"] = "0";
            jInfo["similarity_id"] = "0";
            jInfo["update_time"] = currSurResponse.UpdateDate;
            if (isAnonymous)
            {
                jInfo["user"] = "***";
            }
            else
            {
                jInfo["user"] = currSurResponse.EmpUidMC;
            }
            jInfo["title"] = survey.SurName;
            jInfo["foreword"] = survey.SurContent;
            jInfo["need_review"] = "0";
            jInfo["project_status"] = survey.SurStatus;
            jInfo["tester_count"] = survey.Amounted;
            jInfo["valid_survey_response"] = responseList.Count();
            jInfo["survey_response_count"] = responseList.Count();
            jInfo["survey_online_time"] = GetOnlineTime(DateTimeUtils.ToDateTime(survey.PublishTime));
            jInfo["res_order"] = sortIndex;
            jSurResult["info"] = jInfo;

            JArray arrQuestion = new JArray();

            var questions = _dbContext.QueryWhere<SurQuestion>("SurveyUid=@SurveyUid", param).OrderBy(c => c.SortIndex);
            DynamicParameters paramResult = new DynamicParameters();
            paramResult.Add("ResponseUid", currSurResponse.Fid);
            var results = _dbContext.QueryWhere<SurResult>("ResponseUid=@ResponseUid", paramResult);
            foreach (SurQuestion question in questions)
            {
                JObject jQuestion = GetTesterQuestionJson(question, results);
                arrQuestion.Add(jQuestion);
            }
            jSurResult["question"] = arrQuestion;



            return jSurResult;
        }
        private JObject GetTesterQuestionJson(SurQuestion question, IEnumerable<SurResult> surResults)
        {
            JObject jquestion = new JObject();

            jquestion["id"] = question.Fid;
            jquestion["survey_id"] = question.SurveyUid;
            jquestion["content"] = question.SortIndex + "." + question.Content;
            jquestion["type_id"] = question.TypeId;
            jquestion["order"] = question.SortIndex;
            jquestion["has_other"] = question.HasOther == 0 ? "N" : "Y";
            jquestion["required"] = question.Required == 0 ? "N" : "Y";
            jquestion["title_quote"] = question.TitleQuote == 0 ? "N" : "Y";
            jquestion["choice_quote"] = question.ChoiceQuote;
            jquestion["page"] = question.PageNum;
            jquestion["max"] = question.MaxValue;
            jquestion["min"] = question.MinValue;
            jquestion["exclusive_options"] = question.ExclusiveOptions;
            jquestion["logic_condition"] = "";
            jquestion["redirect_relation"] = "";
            jquestion["is_logic_show_question"] = "0";
            jquestion["vote_type_id"] = "0";
            jquestion["type"] = QuestionType(question.TypeId);

            if (question.TypeId == "6" || question.TypeId == "7" || question.TypeId == "14" || question.TypeId == "8" || question.TypeId == "15")
            {
                //单选题、选择列表
                JArray arrayChoice = new JArray();
                jquestion["choice"] = arrayChoice;
                var choices = GetQuestionChoice(question.Fid);
                foreach (var choice in choices)
                {
                    JObject jChoice = new JObject();

                    jChoice["id"] = choice.Fid;
                    jChoice["question_id"] = question.Fid;
                    jChoice["order"] = choice.SortIndex;
                    jChoice["content"] = choice.Content;
                    jChoice["value"] = "0";
                    jChoice["is_other"] = choice.IsOther == 0 ? "N" : "Y";
                    jChoice["required"] = choice.Required == 0 ? "N" : "Y";
                    SurResult rs = surResults.FirstOrDefault<SurResult>(r => r.Answer == choice.Fid);
                    jChoice["selected"] = rs == null ? false : true;
                    if (rs != null && rs.AnswerOther.IsPresent())
                    {
                        jChoice["other_content"] = rs.AnswerOther;
                    }
                    arrayChoice.Add(jChoice);
                }

            }
            else if (question.TypeId == "1" || question.TypeId == "2")
            {
                //单行填空、多行填空
                SurResult rs = surResults.FirstOrDefault<SurResult>(r => r.QuestionUid == question.Fid);
                jquestion["response_text"] = rs == null ? "" : rs.Answer;

            }
            else if (question.TypeId == "9" || question.TypeId == "13")
            {
                //单选矩阵,多选矩阵               
                JArray arrayChoice = new JArray();
                jquestion["choice"] = arrayChoice;
                var choices = GetQuestionChoice(question.Fid);
                foreach (var choice in choices)
                {
                    JObject jChoice = new JObject();
                    jChoice["id"] = choice.Fid;
                    jChoice["question_id"] = question.Fid;
                    jChoice["order"] = choice.SortIndex;
                    jChoice["real_choice_id"] = "0";
                    jChoice["content"] = choice.Content;
                    jChoice["value"] = "0";
                    jChoice["is_other"] = choice.IsOther == 0 ? "N" : "Y";
                    jChoice["required"] = choice.Required == 0 ? "N" : "Y";
                    arrayChoice.Add(jChoice);
                }
                JArray arrayTitle = new JArray();
                if (question.TypeId == "9")
                {
                    jquestion["radio_array_title"] = arrayTitle;
                }
                else
                {
                    jquestion["checkbox_array_title"] = arrayTitle;
                }
                var titles = GetQuestionTitle(question.Fid);
                foreach (var title in titles)
                {

                    JObject jTitle = new JObject();

                    jTitle["id"] = title.Fid;
                    jTitle["question_id"] = question.Fid;
                    jTitle["order"] = title.SortIndex;
                    jTitle["content"] = title.Content;
                    jTitle["is_other"] = "N";
                    if (question.TypeId == "9")
                    {
                        SurResult rs = surResults.FirstOrDefault<SurResult>(r => r.TitleUid == title.Fid);
                        jTitle["selected"] = rs != null ? rs.Answer : "";
                    }
                    else
                    {
                        var ts = surResults.Where<SurResult>(r => r.TitleUid == title.Fid);
                        JArray arrt = new JArray();
                        jTitle["selected"] = arrt;
                        if (ts != null && ts.Any())
                        {
                            foreach (var t in ts)
                            {
                                arrt.Add(t.Answer);
                            }
                        }
                    }
                    arrayTitle.Add(jTitle);
                }
            }

            return jquestion;
        }
        #endregion

        #region 导出
        public bool ExportSurveyStat(string fileName, string surveyUid)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("SurveyUid", surveyUid);
            Survey survey = _dbContext.Get<Survey>(surveyUid);
            string sqlWhere = "SurveyUid=@SurveyUid";
            var questions = _dbContext.QueryWhere<SurQuestion>(sqlWhere, param).OrderBy(c => c.SortIndex);
            var choices = _dbContext.QueryWhere<SurQuestionChoice>(sqlWhere, param).OrderBy(c => c.SortIndex);
            var titles = _dbContext.QueryWhere<SurArrayTitle>(sqlWhere, param).OrderBy(c => c.SortIndex);
            //统计
            string where = GetReportFilterSql(surveyUid);
            if (where.IsMissing())
            {
                where = "SurveyUid='" + surveyUid + "'";
            }
            string sql = $"select {nameof(SurResult.QuestionUid)},{nameof(SurResult.TitleUid)}, {nameof(SurResult.Answer)},COUNT({nameof(SurResult.Answer)}) as Id from {nameof(SurResult)} where {where}  group by {nameof(SurResult.Answer)},{nameof(SurResult.QuestionUid)},{nameof(SurResult.TitleUid)}";
            var sResults = _dbContext.Query<SurResult>(sql, param);
            //答题人个数
            int surveyNum = GetTotalBySurveyUid(surveyUid);

            IWorkbook surveyStatBook = new XSSFWorkbook();

            if (fileName.IndexOf(".xls") > 0) // 2003版本
            {
                surveyStatBook = new HSSFWorkbook();
            }
            NPOI.SS.UserModel.ISheet sheet = surveyStatBook.CreateSheet(survey.SurName);
            sheet.DefaultRowHeight = 15 * 20;

            int row = 1;
            foreach (SurQuestion question in questions)
            {
                // 第一行
                NPOI.SS.UserModel.IRow firstRow = sheet.CreateRow(row);

                //获取汇总
                var rresult = sResults.Where(r => r.QuestionUid == question.Fid);
                long total = 0;
                if (rresult != null)
                {
                    total = rresult.Sum(c => c.Id);
                }
                else
                {
                    continue;
                }
                if (question.TypeId == "6" || question.TypeId == "7" || question.TypeId == "8" || question.TypeId == "14" || question.TypeId == "15")
                {
                    //标题行
                    ICell cell = firstRow.CreateCell(0);
                    cell.SetCellValue(question.QIndex + "." + question.Content);
                    cell.CellStyle = GetCellStyle(surveyStatBook);
                    firstRow.CreateCell(1).CellStyle = GetCellStyle(surveyStatBook);
                    firstRow.CreateCell(2).CellStyle = GetCellStyle(surveyStatBook);

                    sheet.AddMergedRegion(new CellRangeAddress(row, row, 0, 2));
                    row++;
                    NPOI.SS.UserModel.IRow secondRow = sheet.CreateRow(row);
                    ICell cell0 = secondRow.CreateCell(0);
                    cell0.SetCellValue("选项");
                    cell0.CellStyle = GetCellStyle(surveyStatBook);
                    ICell cell1 = secondRow.CreateCell(1);
                    cell1.SetCellValue("计数");
                    cell1.CellStyle = GetCellStyle(surveyStatBook);
                    ICell cell2 = secondRow.CreateCell(2);
                    cell2.SetCellValue("占比");
                    cell2.CellStyle = GetCellStyle(surveyStatBook);
                    var qcs = choices.Where(c => c.QuestionUid == question.Fid);
                    if (qcs != null && qcs.Any())
                    {
                        foreach (SurQuestionChoice c in qcs)
                        {
                            row++;
                            NPOI.SS.UserModel.IRow cRow = sheet.CreateRow(row);
                            ICell cell30 = cRow.CreateCell(0);
                            cell30.SetCellValue(c.Content);
                            cell30.CellStyle = GetCellStyle(surveyStatBook);
                            var cr = rresult.FirstOrDefault(r => r.Answer == c.Fid);
                            ICell cell31 = cRow.CreateCell(1);
                            cell31.CellStyle = GetCellStyle(surveyStatBook);
                            ICell cell32 = cRow.CreateCell(2);
                            cell32.CellStyle = GetCellStyle(surveyStatBook);
                            string percent = "0.0%";
                            if (cr != null)
                            {
                                cell31.SetCellValue(cr.Id);
                                percent = Math.Round(cr.Id * 100 / (double)total, 2) + "%";
                            }
                            else
                            {
                                cell31.SetCellValue(0);
                            }
                            cell32.SetCellValue(percent);
                        }
                    }
                    row++;
                    NPOI.SS.UserModel.IRow threeRow = sheet.CreateRow(row);
                    ICell c1 = threeRow.CreateCell(0);
                    c1.SetCellValue("答题人数：" + surveyNum);
                    c1.CellStyle = GetCellStyle(surveyStatBook);
                    threeRow.CreateCell(1).CellStyle = GetCellStyle(surveyStatBook);
                    threeRow.CreateCell(2).CellStyle = GetCellStyle(surveyStatBook);
                    sheet.AddMergedRegion(new CellRangeAddress(row, row, 0, 2));
                    row++;
                }
                else if (question.TypeId == "1" || question.TypeId == "2")
                {
                    //标题行
                    ICell cell = firstRow.CreateCell(0);
                    cell.SetCellValue(question.QIndex + "." + question.Content);
                    cell.CellStyle = GetCellStyle(surveyStatBook);
                    firstRow.CreateCell(1).CellStyle = GetCellStyle(surveyStatBook);
                    firstRow.CreateCell(2).CellStyle = GetCellStyle(surveyStatBook);
                    sheet.AddMergedRegion(new CellRangeAddress(row, row, 0, 2));
                    row++;
                    NPOI.SS.UserModel.IRow threeRow = sheet.CreateRow(row);
                    ICell c0 = threeRow.CreateCell(0);
                    c0.SetCellValue("答题人数：" + surveyNum);
                    c0.CellStyle = GetCellStyle(surveyStatBook);
                    threeRow.CreateCell(1).CellStyle = GetCellStyle(surveyStatBook);
                    threeRow.CreateCell(2).CellStyle = GetCellStyle(surveyStatBook);
                    sheet.AddMergedRegion(new CellRangeAddress(row, row, 0, 2));
                    row++;
                }
                else if (question.TypeId == "9" || question.TypeId == "13")
                {
                    var qcs = choices.Where(c => c.QuestionUid == question.Fid);
                    var qts = titles.Where(t => t.QuestionUid == question.Fid);
                    if (qcs != null && qcs.Any())
                    {
                        //标题行
                        ICell cell = firstRow.CreateCell(0);
                        cell.SetCellValue(question.QIndex + "." + question.Content);
                        cell.CellStyle = GetCellStyle(surveyStatBook);
                        for (int fi = 1; fi < qcs.Count(); fi++)
                        {
                            firstRow.CreateCell(fi).CellStyle = GetCellStyle(surveyStatBook);
                        }

                        sheet.AddMergedRegion(new CellRangeAddress(row, row, 0, qcs.Count()));
                        row++;
                        NPOI.SS.UserModel.IRow tRow1 = sheet.CreateRow(row);
                        ICell c0 = tRow1.CreateCell(0);
                        c0.SetCellValue("");

                        c0.CellStyle = GetCellStyle(surveyStatBook);

                        int i = 1;
                        foreach (SurQuestionChoice c in qcs)
                        {
                            ICell c1 = tRow1.CreateCell(i);
                            c1.SetCellValue(c.Content);
                            c1.CellStyle = GetCellStyle(surveyStatBook);
                            i++;
                        }
                    }
                    if (qts != null && qts.Any())
                    {
                        foreach (var t in qts)
                        {
                            row++;
                            NPOI.SS.UserModel.IRow tRow2 = sheet.CreateRow(row);
                            ICell c0 = tRow2.CreateCell(0);
                            c0.SetCellValue(t.Content);
                            c0.CellStyle = GetCellStyle(surveyStatBook);
                            var rr = rresult.Where(r => r.TitleUid == t.Fid);
                            double totals = rr.Sum(a => a.Id);
                            int i = 1;
                            foreach (var c in qcs)
                            {
                                var cr = rresult.FirstOrDefault(r => r.Answer == c.Fid && r.TitleUid == t.Fid);
                                if (cr != null)
                                {
                                    string percent = Math.Round(cr.Id * 100 / totals, 2) + "%";
                                    ICell c1 = tRow2.CreateCell(i); c1.SetCellValue(percent);
                                    c1.CellStyle = GetCellStyle(surveyStatBook);
                                }
                                else
                                {
                                    ICell c1 = tRow2.CreateCell(i); c1.SetCellValue("0%");
                                    c1.CellStyle = GetCellStyle(surveyStatBook);
                                }
                                i++;
                            }
                        }
                    }
                    row++;
                    NPOI.SS.UserModel.IRow threeRow = sheet.CreateRow(row);
                    ICell c3 = threeRow.CreateCell(0); c3.SetCellValue("答题人数：" + surveyNum);
                    c3.CellStyle = GetCellStyle(surveyStatBook);
                    for (int fi = 1; fi < qcs.Count(); fi++)
                    {
                        threeRow.CreateCell(fi).CellStyle = GetCellStyle(surveyStatBook);
                    }
                    sheet.AddMergedRegion(new CellRangeAddress(row, row, 0, qcs.Count()));

                    row++;
                }

                row++;
            }


            // ...
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                //写入到excel文件中
                surveyStatBook.Write(fs);
            }
            surveyStatBook = null;
            return true;
        }

        public bool ExportSurveyUserDataStat(string fileName, string surveyUid, string category)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("SurveyUid", surveyUid);
            Survey survey = _dbContext.Get<Survey>(surveyUid);
            string sqlWhere = "SurveyUid=@SurveyUid";
            var questions = _dbContext.QueryWhere<SurQuestion>(sqlWhere, param).OrderBy(c => c.SortIndex);
            var choices = _dbContext.QueryWhere<SurQuestionChoice>(sqlWhere, param).OrderBy(c => c.SortIndex);
            var titles = _dbContext.QueryWhere<SurArrayTitle>(sqlWhere, param).OrderBy(c => c.SortIndex);
            var userList = _dbContext.QueryWhere<SurResponseList>(sqlWhere, param).OrderBy(c => c.SubmitTime);
            var results = _dbContext.QueryWhere<SurResult>(sqlWhere, param);

            IWorkbook surveyStatBook = null;

            if (fileName.IndexOf(".xls") > 0) // 2003版本
            {
                surveyStatBook = new HSSFWorkbook();
            }
            NPOI.SS.UserModel.ISheet sheet = surveyStatBook.CreateSheet(survey.SurName);
            sheet.DefaultRowHeight = 15 * 20;
            int row = 0;
            //创建标题
            IRow row0 = sheet.CreateRow(row);
            ICell c00 = row0.CreateCell(0);
            c00.SetCellValue("用户");
            ICell c01 = row0.CreateCell(1);
            c01.SetCellValue("答题时长");
            ICell c02 = row0.CreateCell(2);
            c02.SetCellValue("提交时间");
            int i = 3;
            foreach (SurQuestion q in questions)
            {
                if (q.TypeId == "6" || q.TypeId == "7" || q.TypeId == "14" || q.TypeId == "1" || q.TypeId == "2")
                {
                    //单选，下拉
                    ICell c03 = row0.CreateCell(i);
                    if (category == "1")
                    {
                        c03.SetCellValue("Q" + q.SortIndex + "." + q.Content + "【" + QuestionType(q.TypeId) + "】");
                    }
                    else
                    {
                        c03.SetCellValue("Q" + q.SortIndex);
                    }
                    i++;
                }
                else if (q.TypeId == "8" || q.TypeId == "15")
                {
                    //多选
                    var qcs = choices.Where(c => c.QuestionUid == q.Fid).OrderBy(c => c.SortIndex);
                    foreach (var c in qcs)
                    {
                        ICell c03 = row0.CreateCell(i);
                        if (category == "1")
                        {
                            c03.SetCellValue("Q" + q.SortIndex + "." + q.Content + "【" + QuestionType(q.TypeId) + "】-" + c.Content);
                        }
                        else
                        {
                            c03.SetCellValue("Q" + q.SortIndex + "_" + c.SortIndex);
                        }
                        i++;
                        if (c.IsOther == 1)
                        {
                            ICell c04 = row0.CreateCell(i);
                            if (category == "1")
                            {
                                c03.SetCellValue("Q" + q.SortIndex + "." + q.Content + "【" + QuestionType(q.TypeId) + "】-" + c.Content + "-Text");
                            }
                            else
                            {
                                c03.SetCellValue("Q" + q.SortIndex + "_" + c.SortIndex + "_Text");
                            }
                            i++;
                        }
                    }
                }
                else if (q.TypeId == "9")
                {//矩阵单选
                    var qct = titles.Where(t => t.QuestionUid == q.Fid).OrderBy(t => t.SortIndex);
                    foreach (var t in qct)
                    {
                        ICell c03 = row0.CreateCell(i);
                        if (category == "1")
                        {
                            c03.SetCellValue("Q" + q.SortIndex + "." + q.Content + "【" + QuestionType(q.TypeId) + "】-" + t.Content);
                        }
                        else
                        {
                            c03.SetCellValue("Q" + q.SortIndex + "_R" + t.SortIndex);
                        }
                        i++;
                    }
                }
                else if (q.TypeId == "13")
                {
                    //矩阵多选
                    var qct = titles.Where(t => t.QuestionUid == q.Fid).OrderBy(t => t.SortIndex);
                    var qcs = choices.Where(c => c.QuestionUid == q.Fid).OrderBy(c => c.SortIndex);
                    foreach (var t in qct)
                    {
                        foreach (var c in qcs)
                        {
                            ICell c03 = row0.CreateCell(i);
                            if (category == "1")
                            {
                                c03.SetCellValue("Q" + q.SortIndex + "." + q.Content + "【" + QuestionType(q.TypeId) + "】-" + t.Content + "-" + c.Content);
                            }
                            else
                            {
                                c03.SetCellValue("Q" + q.SortIndex + "_R" + t.SortIndex + "_" + c.SortIndex);
                            }
                            i++;
                        }

                    }
                }
            }

            foreach (SurResponseList user in userList)
            {
                row++;
                IRow rowc = sheet.CreateRow(row);
                ICell cc0 = rowc.CreateCell(0);
                cc0.SetCellValue("*");
                ICell cc1 = rowc.CreateCell(1);
                cc1.SetCellValue(user.TimeLength + "秒");
                ICell cc2 = rowc.CreateCell(2);
                cc2.SetCellValue(user.SubmitTime);
                i = 3;
                foreach (SurQuestion q in questions)
                {
                    if (q.TypeId == "6" || q.TypeId == "7" || q.TypeId == "14" || q.TypeId == "1" || q.TypeId == "2")
                    {
                        var cr = results.Where(r => r.QuestionUid == q.Fid && r.UserUid == user.UserUid);
                        //单选，下拉
                        ICell c03 = rowc.CreateCell(i);
                        if ((q.TypeId == "1" || q.TypeId == "2") && cr != null)
                        {
                            c03.SetCellValue(cr.First().Answer);
                        }
                        else
                        {
                            if (cr != null)
                            {
                                var cc = choices.FirstOrDefault(c => c.Fid == cr.First().Answer);
                                if (cc != null)
                                {
                                    if (category == "1")
                                    {
                                        c03.SetCellValue(cc.Content);
                                    }
                                    else
                                    {
                                        c03.SetCellValue(cc.SortIndex);
                                    }
                                }
                                else
                                {
                                    if (category == "2")
                                    {
                                        c03.SetCellValue("0");
                                    }
                                }
                            }
                        }
                        i++;
                    }
                    else if (q.TypeId == "8" || q.TypeId == "15")
                    {
                        var cr = results.Where(r => r.QuestionUid == q.Fid && r.UserUid == user.UserUid);
                        //多选
                        var qcs = choices.Where(c => c.QuestionUid == q.Fid).OrderBy(c => c.SortIndex);
                        foreach (var c in qcs)
                        {
                            ICell c03 = rowc.CreateCell(i);
                            if (cr != null)
                            {
                                var cresult = cr.Where(r => r.Answer == c.Fid);
                                if (cresult != null && cresult.Any())
                                {
                                    if (category == "1")
                                    {
                                        c03.SetCellValue(c.Content);
                                    }
                                    else
                                    {
                                        c03.SetCellValue(c.SortIndex);
                                    }
                                }
                                else
                                {
                                    if (category == "2")
                                    {
                                        c03.SetCellValue("0");
                                    }
                                }
                            }
                            i++;
                            if (c.IsOther == 1)
                            {
                                ICell c04 = row0.CreateCell(i);
                                if (cr != null)
                                {
                                    var cresult = cr.Where(r => r.Answer == c.Fid);
                                    if (cresult != null && cresult.Any())
                                    {
                                        c04.SetCellValue(cresult.First().AnswerOther);
                                    }
                                }

                                i++;
                            }
                        }
                    }
                    else if (q.TypeId == "9")
                    {//矩阵单选
                        var qct = titles.Where(t => t.QuestionUid == q.Fid).OrderBy(t => t.SortIndex);
                        foreach (var t in qct)
                        {
                            ICell c03 = rowc.CreateCell(i);
                            var cr = results.Where(r => r.QuestionUid == q.Fid && r.UserUid == user.UserUid && r.TitleUid == t.Fid);
                            if (cr != null)
                            {
                                var cc = choices.FirstOrDefault(c => c.Fid == cr.First().Answer);
                                if (cc != null)
                                {
                                    if (category == "1")
                                    {
                                        c03.SetCellValue(cc.Content);
                                    }
                                    else
                                    {
                                        c03.SetCellValue(cc.SortIndex);
                                    }
                                }
                                else
                                {
                                    if (category == "2")
                                    {
                                        c03.SetCellValue("0");
                                    }
                                }
                            }
                            i++;
                        }
                    }
                    else if (q.TypeId == "13")
                    {
                        //矩阵多选
                        var qct = titles.Where(t => t.QuestionUid == q.Fid).OrderBy(t => t.SortIndex);
                        var qcs = choices.Where(c => c.QuestionUid == q.Fid).OrderBy(c => c.SortIndex);
                        foreach (var t in qct)
                        {
                            var cr = results.Where(r => r.QuestionUid == q.Fid && r.UserUid == user.UserUid && r.TitleUid == t.Fid);
                            foreach (var c in qcs)
                            {
                                ICell c03 = rowc.CreateCell(i);
                                if (cr != null)
                                {
                                    var cresult = cr.Where(r => r.Answer == c.Fid);
                                    if (cresult != null && cresult.Any())
                                    {
                                        if (category == "1")
                                        {
                                            c03.SetCellValue(c.Content);
                                        }
                                        else
                                        {
                                            c03.SetCellValue(c.SortIndex);
                                        }
                                    }
                                    else
                                    {
                                        if (category == "2")
                                        {
                                            c03.SetCellValue("0");
                                        }
                                    }
                                }
                                else
                                {
                                    if (category == "2")
                                    {
                                        c03.SetCellValue("0");
                                    }
                                }
                                i++;
                            }

                        }
                    }
                }

            }

            // ...
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                //写入到excel文件中
                surveyStatBook.Write(fs);
            }
            surveyStatBook = null;
            return true;
        }

        private ICellStyle GetCellStyle(IWorkbook workbook)
        {
            ICellStyle style = workbook.CreateCellStyle();
            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            return style;
        }
        #endregion
        private string QuestionType(string typeid)
        {
            string typeName = string.Empty;
            switch (typeid)
            {
                case "8":
                    typeName = "复选框";
                    break;
                case "1":
                    typeName = "文本框";
                    break;
                case "7":
                    typeName = "下拉框";
                    break;
                case "2":
                    typeName = "大文本框";
                    break;
                case "9":
                    typeName = "单选矩阵";
                    break;
                case "13":
                    typeName = "多选矩阵";
                    break;
                case "10":
                    typeName = "章节分隔标题";
                    break;
                case "11":
                    typeName = "分页";
                    break;
                case "6":
                    typeName = "单选题";
                    break;
                case "15":
                    typeName = "图片多选";
                    break;
                case "14":
                    typeName = "图片单选";
                    break;
                default:
                    break;
            }
            return typeName;
        }
        private IEnumerable<JProperty> JObjectToDic(JObject jobj)
        {
            if (jobj != null)
            {
                IEnumerable<JProperty> properties = jobj.Properties();
                if (properties != null && properties.Any())
                {
                    return properties;
                }
            }
            return null;

        }
        [Transactional]
        public bool CopySurvey(string fid,string surName)
        {
            var survey = _dbContext.Get<Survey>(fid);
            if (survey != null)
            {
                Survey newSurvey = new Survey
                {
                    SurName = surName,
                    SurContent = survey.SurContent,
                    SurStatus = SurveyStatus.Creating,
                    CreateTime = DateTimeUtils.CurrentDateTimeStr,
                    UserUid = _applicationContext.UserUid,
                    EmpUid = _applicationContext.EmpUid
                };
                long id = _dbContext.Insert(newSurvey);
                JObject jContent = JObject.Parse(survey.JSONContent);
                jContent["survey_id"] = id;
                jContent["survey_name"] = surName;
                JArray arrContent = jContent["content"] as JArray;
                if (arrContent != null && arrContent.Any())
                {
                    foreach (JObject jc in arrContent)
                    {
                        jc["survey_id"] = id;
                    }
                }
                newSurvey.JSONContent = jContent.ToString();
                _dbContext.Update(newSurvey);
                return true;
            }
            return false;
        }
    }

}
