using Fap.Workflow.Engine.Enums;
using Fap.Workflow.Engine.Utility;
using Fap.Workflow.Engine.Xpdl.Entity;
using Fap.Workflow.Engine.Xpdl.Exceptions;
using Fap.Workflow.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Fap.Core.Extensions;
using Microsoft.Extensions.Logging;
using Dapper;
using System.Text.RegularExpressions;
using Fap.Workflow.Engine.Entity;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Rbac.Model;

namespace Fap.Workflow.Engine.Xpdl
{
    /// <summary>
    /// 流程定义模型类
    /// </summary>
    public class ProcessModel : IProcessModel
    {
        private readonly IDbContext _dataAccessor;
        private readonly ILogger<ProcessModel> _logger;
        #region 属性和构造函数
        /// <summary>
        /// 流程定义实体
        /// </summary>
        public ProcessEntity ProcessEntity { get; set; }

        public ProcessModel(IDbContext dataAccessor, ILoggerFactory loggerFactory, string processId, string billUid)
        {
            _dataAccessor = dataAccessor;
            _logger = loggerFactory.CreateLogger<ProcessModel>();
            ProcessEntity = GetProcessEntity(processId, billUid);
        }
        private ProcessEntity GetProcessEntity(string processUid, string billUid)
        {
            ProcessEntity entity = null;
            WfProcess wfProcess = _dataAccessor.Get<WfProcess>(processUid);
            if (wfProcess != null)
            {
                entity = new ProcessEntity();
                dynamic bizData = _dataAccessor.Get(wfProcess.BillTable, billUid);
                var processInstance = _dataAccessor.QueryFirstOrDefaultWhere<WfProcessInstance>("ProcessUid=@ProcessUid and BillUid=@BillUid", new DynamicParameters(new { ProcessUid = processUid, BillUid = billUid }));
                if (processInstance == null)
                {
                    WfDiagram diagram = _dataAccessor.Get<WfDiagram>(wfProcess.DiagramId);
                    entity.XmlContent = diagram.XmlContent;
                }
                else
                {
                    var diagramInstance = _dataAccessor.QueryFirstOrDefaultWhere<WfDiagramInstance>("ProcessInsUid=@ProcessInsUid", new DynamicParameters(new { ProcessInsUid = processInstance.Fid }));
                    entity.XmlContent = diagramInstance.XmlContent;
                }
                entity.ProcessUid = wfProcess.Fid;
                entity.ProcessName = wfProcess.ProcessName;
                entity.AppType = wfProcess.ProcessGroupUid;
                entity.IsUsing = wfProcess.Status == WfProcessState.Using;
                entity.Version = wfProcess.Version;
                entity.BizData = bizData;
            }
            else
            {
                _logger.LogError($"数据库没有对应的流程定义记录,模板{processUid}");
            }
            return entity;
        }
        /// <summary>
        /// 流程xml文档
        /// </summary>
        public XmlDocument XmlProcessDefinition
        {
            get
            {
                if (CachedHelper.GetXpdlCache(ProcessEntity.ProcessUid, ProcessEntity.Version) == null)
                {
                    XmlDocument xmlDoc = new XmlDocument();

                    xmlDoc.LoadXml(ProcessEntity.XmlContent);

                    CachedHelper.SetXpdlCache(ProcessEntity.ProcessUid, ProcessEntity.Version, xmlDoc);
                }
                return CachedHelper.GetXpdlCache(ProcessEntity.ProcessUid, ProcessEntity.Version);
            }
        }


        #endregion
        #region 回退
        /// <summary>
        /// 获取与合并节点相对应的分支节点
        /// </summary>
        /// <param name="fromActivity">合并节点</param>
        /// <param name="joinCount">经过的合并节点个数</param>
        /// <param name="splitCount">经过的分支节点个数</param>
        /// <returns>对应的分支节点</returns>
        public ActivityEntity GetBackwardGatewayActivity(ActivityEntity fromActivity, ref int joinCount, ref int splitCount)
        {
            if (fromActivity.ActivityType == ActivityTypeEnum.GatewayNode && fromActivity.GatewaySplitJoinType == GatewaySplitJoinTypeEnum.Join)
            {
                joinCount++;
                IList<TransitionEntity> backwardTrans = GetBackwardTransitionList(fromActivity.ActivityID);
                if (backwardTrans.Count > 0)
                {
                    return GetBackwardGatewayActivity(backwardTrans[0].SourceActivity, ref joinCount, ref splitCount);
                }
            }
            else if (fromActivity.ActivityType == ActivityTypeEnum.GatewayNode && fromActivity.GatewaySplitJoinType == GatewaySplitJoinTypeEnum.Split)
            {
                splitCount++;
                if (splitCount == joinCount)
                {
                    return fromActivity;
                }
                else
                {
                    IList<TransitionEntity> backwardTrans = GetBackwardTransitionList(fromActivity.ActivityID);
                    if (backwardTrans.Count > 0)
                    {
                        return GetBackwardGatewayActivity(backwardTrans[0].SourceActivity, ref joinCount, ref splitCount);
                    }
                }
            }
            else if (fromActivity.ActivityType == ActivityTypeEnum.StartNode || fromActivity.ActivityType == ActivityTypeEnum.EndNode)
                return null;
            else
            {
                IList<TransitionEntity> backwardTrans = GetBackwardTransitionList(fromActivity.ActivityID);
                if (backwardTrans.Count > 0)
                {
                    return GetBackwardGatewayActivity(backwardTrans[0].SourceActivity, ref joinCount, ref splitCount);
                }
            }
            return null;
        }
        /// <summary>
        /// 获取节点前驱连线的数目
        /// </summary>
        /// <param name="toActivityGUID"></param>
        /// <returns></returns>
        public int GetBackwardTransitionListCount(string toActivityGUID)
        {
            IList<TransitionEntity> backwardList = GetBackwardTransitionList(toActivityGUID);
            return backwardList.Count;
        }

        #endregion

        #region 活动节点基本方法和流转规则处理

        #region 活动节点基本方法
        /// <summary>
        /// 获取开始节点信息
        /// </summary>
        /// <returns></returns>
        public ActivityEntity GetStartActivity()
        {
            int nodeType = (int)ActivityTypeEnum.StartNode;

            XmlNode startTypeNode = GetXmlActivityTypeNodeFromXmlFile(nodeType);
            if (startTypeNode != null)
                return ConvertXmlActivityNodeToActivityEntity(startTypeNode);// ConvertXmlActivityNodeToActivityEntity(startTypeNode);
            throw new Exception("未检测到流程配置节点信息，请先设计流程后在进行办理！");
        }

        /// <summary>
        /// 获取结束节点
        /// </summary>
        /// <returns></returns>
        public ActivityEntity GetEndActivity()
        {
            int nodeType = (int)ActivityTypeEnum.EndNode;
            XmlNode endTypeNode = GetXmlActivityTypeNodeFromXmlFile(nodeType);
            return ConvertXmlActivityNodeToActivityEntity(endTypeNode);
        }

        /// <summary>
        /// 获取任务类型的节点(包含普通节点，会签节点，定时节点，和子流程节点)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ActivityEntity> GetAllTaskActivityList()
        {
            List<ActivityEntity> activityList = new List<ActivityEntity>();
            var nodeTypes = new int[] { (int)ActivityTypeEnum.TaskNode, (int)ActivityTypeEnum.SignNode, (int)ActivityTypeEnum.TimerNode, (int)ActivityTypeEnum.SubProcessNode };
            foreach (var item in nodeTypes)
            {
                XmlNodeList nodeList = GetXmlActivityListByTypeFromXmlFile(item);

                ActivityEntity entity = null;

                foreach (XmlNode node in nodeList)
                {
                    entity = ConvertXmlActivityNodeToActivityEntity(node);
                    activityList.Add(entity);
                }
            }

            return activityList;
        }

        /// <summary>
        /// 获取流程的第一个可办理节点
        /// </summary>
        /// <returns></returns>
        public ActivityEntity GetFirstActivity()
        {
            string startActivityId = GetStartActivity().ActivityID;
            ActivityEntity firstActivity = GetNextActivity(startActivityId);
            return firstActivity;
        }

        /// <summary>
        /// 获取当前节点的下一个可用节点信息
        /// </summary>
        /// <param name="activityGUID"></param>
        /// <returns></returns>
        public ActivityEntity GetNextActivity(string activityId)
        {
            XmlNode transitionNode = GetForwardXmlTransitionNode(activityId);

            return GetActivityFromTransitionTo(transitionNode);
        }

        /// <summary>
        /// 获取流程起始的活动节点列表(开始节点之后，可能有多个节点)
        /// </summary>
        /// <param name="conditionKeyValuePair">条件表达式的参数名称-参数值的集合</param>
        /// <returns></returns>
        public NextActivityMatchedResult GetFirstActivityList()
        {
            try
            {
                string startActivityId = GetStartActivity().ActivityID;
                return GetNextActivityList(startActivityId);
            }
            catch (System.Exception e)
            {
                throw new WfXpdlException(string.Format("解析流程定义文件发生异常，异常描述：{0}", e.Message), e);
            }
        }
        #endregion

        #region 流程流转解析，处理流程下一步流转条件等规则
        /// <summary>
        /// 获取下一步活动节点树，供流转界面使用
        /// </summary>
        /// <param name="currentActivityID"></param>
        /// <returns></returns>
        public IList<NodeView> GetNextActivityTree(string currentActivityID)
        {
            var nextSteps = GetNextActivityList(currentActivityID);
            var treeNodeList = new List<NodeView>();

            foreach (var child in nextSteps.Root)
            {
                if (child.HasChildren)
                {
                    Tranverse(child, treeNodeList);
                }
                else
                {
                    treeNodeList.Add(new NodeView
                    {
                        ActivityId = child.Activity.ActivityID,
                        ActivityName = child.Activity.ActivityName,
                        Participants = GetActivityParticipants(child.Activity.ActivityID)

                    });
                }
            }
            return treeNodeList;
        }

        /// <summary>
        /// 获取下一步活动节点树，供流转界面使用
        /// </summary>
        /// <param name="currentActivityGUID"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        //public IList<NodeView> GetNextActivityTree(int processInstanceID,
        //    string currentActivityID)
        //{
        //    var treeNodeList = new List<NodeView>();
        //    var activity = GetActivity(currentActivityID);

        //    //判断有没有指定的跳转节点信息
        //    if (activity.ActivityTypeDetail.SkipInfo != null
        //        && activity.ActivityTypeDetail.SkipInfo.IsSkip == true)
        //    {
        //        //获取跳转节点信息
        //        var skipto = activity.ActivityTypeDetail.SkipInfo.Skipto;
        //        var skiptoActivity = GetActivity(skipto);

        //        treeNodeList.Add(new NodeView
        //        {
        //            ActivityId = skiptoActivity.ActivityID,
        //            ActivityName = skiptoActivity.ActivityName,
        //            ActivityCode = skiptoActivity.ActivityCode,

        //            Participants = GetActivityParticipants(skiptoActivity.ActivityID),
        //            //ReceiverType = skiptoActivity.Transition.Receiver.ReceiverType
        //            IsSkipTo = true
        //        });
        //    }
        //    else
        //    {
        //        //Transiton方式的流转定义
        //        var nextSteps = GetNextActivityList(activity.ActivityID);

        //        foreach (var child in nextSteps.Root)
        //        {
        //            if (child.HasChildren)
        //            {
        //                Tranverse(child, treeNodeList);
        //            }
        //            else
        //            {
        //                treeNodeList.Add(new NodeView
        //                {
        //                    ActivityId = child.Activity.ActivityID,
        //                    ActivityName = child.Activity.ActivityName,
        //                    ActivityCode = child.Activity.ActivityCode,

        //                    Participants = GetActivityParticipants(child.Activity.ActivityID),

        //                });
        //            }
        //        }
        //    }

        //    return treeNodeList;
        //}

        /// <summary>
        /// 迭代遍历
        /// </summary>
        /// <param name="root"></param>
        /// <param name="treeNodeList"></param>
        private void Tranverse(NextActivityComponent root, IList<NodeView> treeNodeList)
        {
            foreach (var child in root)
            {
                if (child.HasChildren)
                {
                    Tranverse(child, treeNodeList);
                }
                else
                {
                    treeNodeList.Add(new NodeView
                    {
                        ActivityId = child.Activity.ActivityID,
                        ActivityName = child.Activity.ActivityName,
                        ActivityCode = child.Activity.ActivityCode,

                        Participants = GetActivityParticipants(child.Activity.ActivityID),
                    });
                }
            }
        }

        /// <summary>
        /// 获取下一步节点列表，满足运行时条件信息
        /// </summary>
        /// <param name="currentActivityGUID"></param>
        /// <returns></returns>
        public NextActivityMatchedResult GetNextActivityList(string currentActivityID)
        {
            try
            {
                NextActivityMatchedResult result = null;
                NextActivityMatchedType resultType = NextActivityMatchedType.Unknown;

                //创建“下一步节点”的根节点
                NextActivityComponent root = NextActivityComponentFactory.CreateNextActivityComponent();
                NextActivityComponent child = null;
                List<TransitionEntity> transitionList = GetForwardTransitionList(currentActivityID).ToList();

                if (transitionList.Count > 0)
                {
                    //遍历连线，获取下一步节点的列表
                    foreach (TransitionEntity transition in transitionList)
                    {
                        if (XPDLHelper.IsWorkItem(transition.TargetActivity.ActivityType))        //可流转简单类型节点 || 子流程节点
                        {
                            child = NextActivityComponentFactory.CreateNextActivityComponent(transition, transition.TargetActivity);
                        }
                        else if (transition.TargetActivity.ActivityType == ActivityTypeEnum.GatewayNode)
                        {
                            NextActivityScheduleBase activitySchedule = NextActivityScheduleFactory.CreateActivitySchedule(this as IProcessModel,
                                transition.TargetActivity.GatewaySplitJoinType);

                            child = activitySchedule.GetNextActivityListFromGateway(transition,
                                transition.TargetActivity,
                                out resultType);
                        }
                        else
                        {
                            throw new XmlDefinitionException(string.Format("未知的节点类型：{0}", transition.TargetActivity.ActivityType.ToString()));
                        }

                        if (child != null)
                        {
                            root.Add(child);
                            resultType = NextActivityMatchedType.Successed;
                        }
                    }
                }
                else
                {
                    resultType = NextActivityMatchedType.NoneTransitionFilteredByCondition;
                }
                result = NextActivityMatchedResult.CreateNextActivityMatchedResultObject(resultType, root);
                return result;
            }
            catch (System.Exception e)
            {
                throw new WfXpdlException(string.Format("解析流程定义文件发生异常，异常描述：{0}", e.Message), e);
            }
        }
        /// <summary>
        /// 添加子节点到网关节点
        /// </summary>
        /// <param name="newRoot">新的根节点</param>
        /// <param name="root">根节点</param>
        /// <param name="child">子节点</param>
        /// <returns>下一步活动节点</returns>
        private NextActivityComponent AddChildToNewGatewayComponent(NextActivityComponent newRoot,
            NextActivityComponent root,
            NextActivityComponent child)
        {
            if ((newRoot == null) && (child != null))
                newRoot = NextActivityComponentFactory.CreateNextActivityComponent(root);

            if ((newRoot != null) && (child != null))
                newRoot.Add(child);
            return newRoot;
        }
        #endregion

        #endregion

        #region Xml活动节点读取操作
        /// <summary>
        /// 获取XML的节点信息
        /// </summary>
        /// <param name="activityGUID"></param>
        /// <returns></returns>
        private XmlNode GetXmlActivityNodeFromXmlFile(string activityID)
        {
            XmlNode xmlNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                    string.Format("{0}[@id='" + activityID + "']", XPDLDefinition.StrXmlActivityPath));
            return xmlNode;
        }

        /// <summary>
        /// 获取活动节点的类型信息
        /// </summary>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        private XmlNode GetXmlActivityTypeNodeFromXmlFile(int nodeType)
        {
            XmlNode typeNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                string.Format("{0}[@nodeType='" + nodeType + "']", XPDLDefinition.StrXmlActivityPath));
            return typeNode;
        }

        /// <summary>
        /// 获取特定类型的活动节点
        /// </summary>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        private XmlNodeList GetXmlActivityListByTypeFromXmlFile(int nodeType)
        {
            XmlNodeList nodeList = XMLHelper.GetXmlNodeListByXpath(XmlProcessDefinition,
                string.Format("{0}[@nodeType='" + nodeType + "']", XPDLDefinition.StrXmlActivityPath));
            return nodeList;
        }

        /// <summary>
        /// 获取参与者信息
        /// </summary>
        /// <param name="participantGUID"></param>
        /// <returns></returns>
        private XmlNode GetXmlParticipantNodeFromXmlFile(string participantGUID)
        {
            XmlNode participantNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                string.Format("{0}[@id='" + participantGUID + "']", XPDLDefinition.StrXmlSingleParticipantPath));
            return participantNode;
        }

        /// <summary>
        /// 获取当前节点信息
        /// </summary>
        /// <returns></returns>
        public ActivityEntity GetActivity(string activityGUID)
        {
            XmlNode activityNode = GetXmlActivityNodeFromXmlFile(activityGUID);

            ActivityEntity entity = ConvertXmlActivityNodeToActivityEntity(activityNode);
            return entity;
        }

        /// <summary>
        /// 获取转移上的target节点的对象
        /// </summary>
        /// <param name="transitionNode">转移的xml节点</param>
        /// <returns></returns>
        private ActivityEntity GetActivityFromTransitionTo(XmlNode transitionNode)
        {
            string activityId = XMLHelper.GetXmlAttribute(transitionNode.FirstChild, "target");
            XmlNode activityNode = GetXmlActivityNodeFromXmlFile(activityId);
            var nodeType = (ActivityTypeEnum)Enum.Parse(typeof(ActivityTypeEnum), XMLHelper.GetXmlAttribute(activityNode, "nodeType"));
            if (nodeType == ActivityTypeEnum.GatewayNode)
            {
                //网关的话继续往下找
                string gateWayId = XMLHelper.GetXmlAttribute(activityNode, "id");
                var nextTransitionNode = GetForwardXmlTransitionNode(gateWayId);
                return GetActivityFromTransitionTo(nextTransitionNode);
            }
            ActivityEntity entity = ConvertXmlActivityNodeToActivityEntity(activityNode);
            return entity;
        }
        #endregion

        #region 获取节点上的角色信息    

        /// <summary>
        /// 获取节点上定义的角色及人员集合
        /// </summary>
        /// <param name="activityGUID"></param>
        /// <returns></returns>
        public IList<Participant> GetActivityParticipants(string activityId)
        {
            IList<Participant> participants = new List<Participant>();
            XmlNode xmlNode = GetXmlActivityNodeFromXmlFile(activityId);
            XmlNode participantsNode = xmlNode.SelectSingleNode("participants");
            if (participantsNode != null)
            {
                foreach (XmlNode participantNode in participantsNode.ChildNodes)
                {
                    Participant participant = new Participant();
                    participant.Type = (ParticipantTypeEnum)Enum.Parse(typeof(ParticipantTypeEnum), XMLHelper.GetXmlAttribute(participantNode, "pType"));

                    participant.Name = XMLHelper.GetXmlAttribute(participantNode, "pName");
                    participant.ID = XMLHelper.GetXmlAttribute(participantNode, "pValue");
                    participant.BindField = XMLHelper.GetXmlAttribute(participantNode, "pColumn");
                    participants.Add(participant);
                }
            }
            return participants;
        }
        /// <summary>
        /// 获取任务节点审批人
        /// </summary>
        /// <param name="activityId"></param>
        /// <returns></returns>
        public PerformerList GetActivityPerformers(string activityId)
        {
            IList<Participant> participants = GetActivityParticipants(activityId);
            PerformerList performers = new PerformerList();
            if (participants.Any())
            {
                foreach (var pp in participants)
                {
                    if (pp.Type == ParticipantTypeEnum.User)
                    {
                        Performer pf = new Performer();
                        pf.UserId = pp.ID;
                        pf.UserName = pp.Name;
                        performers.Add(pf);
                    }
                    else if (pp.Type == ParticipantTypeEnum.Role)
                    {
                        var ps = GetPerformerByRole(pp.ID);
                        if (ps.Any())
                        {
                            performers.AddRange(ps);
                        }
                    }
                    else if (pp.Type == ParticipantTypeEnum.DynRole)
                    {
                        IDictionary<string, object> bizData = ProcessEntity.BizData as IDictionary<string, object>;
                        var ps = GetPerformerByDynRole(pp.ID, pp.BindField, bizData);
                        if (ps.Any())
                        {
                            performers.AddRange(ps);
                        }
                    }
                    else if (pp.Type == ParticipantTypeEnum.Other)
                    {
                        //同其他节点审批人
                        PerformerList others = GetActivityPerformers(pp.ID);
                        performers.AddRange(others);
                    }
                }
            }
            return performers;
        }
        #endregion       

        #region Xml节点转换信息
        /// <summary>
        /// 把XML节点转换为ActivityEntity实体对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal ActivityEntity ConvertXmlActivityNodeToActivityEntity(XmlNode node)
        {
            var nodeType = (ActivityTypeEnum)Enum.Parse(typeof(ActivityTypeEnum), XMLHelper.GetXmlAttribute(node, "nodeType"));

            ActivityEntity entity = new ActivityEntity();
            entity.ActivityName = XMLHelper.GetXmlAttribute(node, "label");

            entity.ActivityCode = XMLHelper.GetXmlAttribute(node, "nodeName");
            if (entity.ActivityName.IsMissing())
            {
                //开始结束节点label为“”
                entity.ActivityName = entity.ActivityCode;
            }
            entity.ActivityID = XMLHelper.GetXmlAttribute(node, "id");
            entity.ProcessUid = ProcessEntity.ProcessUid;
            entity.IsAppoint = XMLHelper.GetXmlAttribute(node, "isAppoint").ToBool();
            entity.ActivityType = nodeType;
            //单据模板
            entity.BillTemplate = XMLHelper.GetXmlAttribute(node, "billTemplate");

            if (entity.ActivityType == ActivityTypeEnum.SubProcessNode)             //sub process node
            {
                //子流程节点
                string subProcess = XMLHelper.GetXmlAttribute(node, "subProcess");
                entity.SubProcess = subProcess;
            }
            else if (entity.ActivityType == ActivityTypeEnum.TaskNode)
            {
                entity.ApproverMethod = (ApproverMethodEnum)Enum.Parse(typeof(ApproverMethodEnum), XMLHelper.GetXmlAttribute(node, "approverMethod"));
            }
            else if (entity.ActivityType == ActivityTypeEnum.SignNode)      //multiple instance node
            {
                string passRate = XMLHelper.GetXmlAttribute(node, "passRate");
                entity.PassRate = Convert.ToInt16(passRate);
            }
            else if (entity.ActivityType == ActivityTypeEnum.TimerNode)
            {
                entity.Expiration = Convert.ToInt32(XMLHelper.GetXmlAttribute(node, "expiration"));
            }
            else if (entity.ActivityType == ActivityTypeEnum.GatewayNode)
            {
                string gateway = XMLHelper.GetXmlAttribute(node, "gateway");
                string direction = XMLHelper.GetXmlAttribute(node, "direction");
                entity.GatewaySplitJoinType = (GatewaySplitJoinTypeEnum)Enum.Parse(typeof(GatewaySplitJoinTypeEnum), gateway);
                entity.GatewayDirectionType = (GatewayDirectionEnum)Enum.Parse(typeof(GatewayDirectionEnum), direction);
            }
            //字段权限以及审批人
            if (nodeType != ActivityTypeEnum.StartNode && nodeType != ActivityTypeEnum.EndNode && nodeType != ActivityTypeEnum.GatewayNode)
            {
                //获取可操作字段
                entity.FieldItems = GetActivityDataItems(entity.ActivityID);
                //获取审批人
                entity.Performers = GetActivityPerformers(entity.ActivityID);
            }
            entity.NoticeApplicant = XMLHelper.GetXmlAttribute(node, "noticeApplicant").ToBool();
            entity.NoticeApprover = XMLHelper.GetXmlAttribute(node, "noticeApprover").ToBool();

            entity.IsMail = XMLHelper.GetXmlAttribute(node, "isMail").ToBool();
            entity.IsMessage = XMLHelper.GetXmlAttribute(node, "isMessage").ToBool();

            //获取节点的操作列表
            //XmlNode actionsNode = node.SelectSingleNode("Actions");
            //if (actionsNode != null)
            //{
            //    XmlNodeList xmlActionList = actionsNode.ChildNodes;
            //    List<ActionEntity> actionList = new List<ActionEntity>();
            //    foreach (XmlNode element in xmlActionList)
            //    {
            //        actionList.Add(ConvertXmlActionNodeToActionEntity(element));
            //    }
            //    entity.ActionList = actionList;
            //}

            return entity;
        }

        /// <summary>
        /// 把Xml节点转换为ActivityTypeDetail 类（用于会签等复杂类型）
        /// </summary>
        /// <param name="typeNode"></param>
        /// <returns></returns>
        private ActivityTypeDetail ConvertXmlNodeToActivityTypeDetail(XmlNode typeNode)
        {
            ActivityTypeDetail entity = new ActivityTypeDetail();
            entity.ActivityType = (ActivityTypeEnum)Enum.Parse(typeof(ActivityTypeEnum), XMLHelper.GetXmlAttribute(typeNode, "type"));
            if (XMLHelper.GetXmlAttribute(typeNode, "complexType").IsPresent())
            {
                entity.ComplexType = XMLHelper.GetXmlAttribute(typeNode, "complexType").ParseEnum<ComplexTypeEnum>();
            }

            if (XMLHelper.GetXmlAttribute(typeNode, "mergeType").IsPresent())
            {
                entity.MergeType = XMLHelper.GetXmlAttribute(typeNode, "mergeType").ParseEnum<MergeTypeEnum>();
            }

            if (XMLHelper.GetXmlAttribute(typeNode, "compareType").IsPresent())
            {
                entity.CompareType = XMLHelper.GetXmlAttribute(typeNode, "compareType").ParseEnum<CompareTypeEnum>();
            }

            if (XMLHelper.GetXmlAttribute(typeNode, "completeOrder").IsPresent())
            {
                entity.CompleteOrder = float.Parse(XMLHelper.GetXmlAttribute(typeNode, "completeOrder"));
            }

            if (XMLHelper.GetXmlAttribute(typeNode, "skip").IsPresent())
            {
                var skip = Boolean.Parse(XMLHelper.GetXmlAttribute(typeNode, "skip"));
                var skipto = XMLHelper.GetXmlAttribute(typeNode, "to");

                if (skip)
                {
                    entity.SkipInfo = new SkipInfo { IsSkip = skip, Skipto = skipto };
                }
            }

            return entity;
        }

        /// <summary>
        /// 将Action的XML节点
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private ActionEntity ConvertXmlActionNodeToActionEntity(XmlNode node)
        {
            ActionEntity action = new ActionEntity();
            var actionType = XMLHelper.GetXmlAttribute(node, "type");
            action.ActionType = (ActionTypeEnum)Enum.Parse(typeof(ActionTypeEnum), actionType);
            if (action.ActionType == ActionTypeEnum.ExternalMethod)
            {
                action.ActionName = XMLHelper.GetXmlAttribute(node, "name");
                action.AssemblyFullName = XMLHelper.GetXmlAttribute(node, "assembly");
                action.InterfaceFullName = XMLHelper.GetXmlAttribute(node, "interface");
                action.MethodName = XMLHelper.GetXmlAttribute(node, "method");
            }
            else if (action.ActionType == ActionTypeEnum.WebApi)
            {
                throw new ApplicationException("WebApi接口，暂时未实现！");
            }
            return action;
        }
        #endregion

        #region 转移连线的获取方法
        internal XmlNode GetXmlTransitionNode(string transitionId)
        {
            XmlNode transitionNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                string.Format("{0}[@id='" + transitionId + "']", XPDLDefinition.StrXmlTransitionPath));

            return transitionNode;
        }

        /// <summary>
        /// 获取活动转移的To节点信息
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <returns></returns>
        internal XmlNode GetForwardXmlTransitionNode(string fromActivityID)
        {
            XmlNodeList transitionNodes = XMLHelper.GetXmlNodeListByXpath(XmlProcessDefinition,
                string.Format("{0}/mxCell[@source='" + fromActivityID + "']", XPDLDefinition.StrXmlTransitionPath));
            if (transitionNodes.Count > 0)
            {
                foreach (XmlNode node in transitionNodes)
                {
                    var tansitionNode = node.ParentNode;
                    string condition = XMLHelper.GetXmlAttribute(tansitionNode, "condition");
                    if (condition.IsMissing())
                    {
                        //根据tansitionNode获取toActivityNode
                        return tansitionNode;
                    }
                    else if (condition.IsPresent() && CheckTansitionCondition(ProcessEntity.BizData, condition))
                    {
                        //返回满足条件的流转节点
                        return tansitionNode;
                    }
                }
            }
            return null; ;
        }

        /// <summary>
        /// 获取活动转移的To节点列表
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <returns></returns>
        internal XmlNodeList GetForwardXmlTransitionNodeList(string fromActivityID)
        {
            XmlNodeList transitionNodeList = XMLHelper.GetXmlNodeListByXpath(XmlProcessDefinition,
                string.Format("{0}/mxCell[@source='" + fromActivityID + "']", XPDLDefinition.StrXmlTransitionPath));

            return transitionNodeList;
        }

        /// <summary>
        /// 根据两个节点，查看是否有连线
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <param name="toActivityGUID"></param>
        /// <returns></returns>
        internal TransitionEntity GetForwardTransition(string fromActivityGUID, string toActivityGUID)
        {
            XmlNode xmlTransitionNode = GetForwardXmlTransitionNode(fromActivityGUID, toActivityGUID);
            TransitionEntity transition = xmlTransitionNode != null ?
                ConvertXmlTransitionNodeToTransitionEntity(xmlTransitionNode) : null;

            return transition;
        }

        /// <summary>
        /// 获取当前节点的后续连线的集合
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <returns></returns>
        public IList<TransitionEntity> GetForwardTransitionList(string sourceActivityID)
        {
            IList<TransitionEntity> transitionList = new List<TransitionEntity>();
            XmlNodeList transitionNodeList = GetForwardXmlTransitionNodeList(sourceActivityID);
            foreach (XmlNode transitionNode in transitionNodeList)
            {
                string condition = XMLHelper.GetXmlAttribute(transitionNode.ParentNode, "condition");
                if (condition.IsPresent())
                {
                    if (!CheckTansitionCondition(ProcessEntity.BizData, condition))
                    {
                        continue;
                    }
                }
                TransitionEntity entity = ConvertXmlTransitionNodeToTransitionEntity(transitionNode);
                transitionList.Add(entity);
            }
            return transitionList;
        }



        /// <summary>
        /// 获取活动转移的节点信息
        /// </summary>
        /// <param name="fromActivityGUID"></param>
        /// <returns></returns>
        private XmlNode GetForwardXmlTransitionNode(string sourceActivityId,
            string targetActivityId)
        {
            XmlNode transitionNode = XMLHelper.GetXmlNodeByXpath(XmlProcessDefinition,
                string.Format("{0}/mxCell[@source='" + sourceActivityId + "' and target='" + targetActivityId + "']", XPDLDefinition.StrXmlTransitionPath));
            return transitionNode;
        }

        /// <summary>
        /// 获取前驱节点的列表
        /// </summary>
        /// <param name="toActivityGUID"></param>
        /// <returns></returns>
        internal XmlNodeList GetXmlBackwardTransitonNodeList(string targetActivityId)
        {
            XmlNodeList transtionNodeList = XMLHelper.GetXmlNodeListByXpath(XmlProcessDefinition,
                string.Format("{0}/mxCell[@target='" + targetActivityId + "']", XPDLDefinition.StrXmlTransitionPath));
            return transtionNodeList;
        }

        /// <summary>
        /// 获取节点的前驱连线
        /// </summary>
        /// <param name="toActivityGUID"></param>
        /// <returns></returns>
        internal IList<TransitionEntity> GetBackwardTransitionList(string targetActivityId)
        {
            XmlNodeList transitionNodeList = GetXmlBackwardTransitonNodeList(targetActivityId);
            IList<TransitionEntity> transitionList = new List<TransitionEntity>();
            foreach (XmlNode transitionNode in transitionNodeList)
            {
                TransitionEntity transition = ConvertXmlTransitionNodeToTransitionEntity(transitionNode);
                transitionList.Add(transition);
            }
            return transitionList;
        }


        #region 解析条件表达式
        /// <summary>
        /// 用LINQ解析条件表达式
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="conditionKeyValuePair"></param>
        /// <returns></returns>
        //private bool ParseCondition(TransitionEntity transition, IDictionary<string, string> conditionKeyValuePair)
        //{
        //    string expression = transition.Condition.ConditionText;
        //    string expressionReplaced = ReplaceParameterToValue(expression, conditionKeyValuePair);

        //    Expression e = System.Linq.Dynamic.DynamicExpression.Parse(typeof(Boolean), expressionReplaced);
        //    LambdaExpression LE = Expression.Lambda(e);
        //    Func<bool> testMe = (Func<bool>)LE.Compile();
        //    bool result = testMe();

        //    return result;
        //}       
        #endregion

        #region Xml节点转换信息
        /// <summary>
        /// 把XML节点转换为ActivityEntity实体对象
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal TransitionEntity ConvertXmlTransitionNodeToTransitionEntity(XmlNode node)
        {
            var parentNode = node.ParentNode;
            //构造转移的基本属性
            TransitionEntity entity = new TransitionEntity();
            entity.TransitionID = XMLHelper.GetXmlAttribute(parentNode, "id");
            entity.SourceActivityID = XMLHelper.GetXmlAttribute(node, "source");
            entity.TargetActivityID = XMLHelper.GetXmlAttribute(node, "target");


            //构造活动节点的实体对象
            entity.SourceActivity = GetActivity(entity.SourceActivityID);
            entity.TargetActivity = GetActivity(entity.TargetActivityID);

            //构造转移的条件节点
            entity.Condition = XMLHelper.GetXmlAttribute(parentNode, "condition");


            return entity;
        }
        #endregion
        #endregion

        #region 资源数据内容操作
        /// <summary>
        /// 获取任务节点上可以编辑的数据项列表
        /// </summary>
        /// <param name="activityId">任务节点</param>
        /// <returns></returns>
        internal IList<FieldEntity> GetActivityDataItems(string activityId)
        {
            IList<FieldEntity> fieldEntitys = new List<FieldEntity>();
            XmlNode activityNode = XMLHelper.GetXmlNodeByXpath(this.XmlProcessDefinition, $"{XPDLDefinition.StrXmlActivityPath}[@id='{activityId}']");
            XmlNode fieldsNode = activityNode.SelectSingleNode("fields");
            if (fieldsNode != null)
            {
                foreach (XmlNode fieldNode in fieldsNode.ChildNodes)
                {
                    FieldEntity entity = new FieldEntity();
                    entity.FieldName = XMLHelper.GetXmlAttribute(fieldNode, "name");
                    entity.Property = (FieldPropertyEnum)Enum.Parse(typeof(FieldPropertyEnum), XMLHelper.GetXmlAttribute(fieldNode, "property"));
                    fieldEntitys.Add(entity);
                }
            }

            return fieldEntitys;
        }
        #endregion

        #region 校验流转条件
        private bool CheckTansitionCondition(dynamic bizData, string condition)
        {
            IDictionary<string, object> dapperRow = bizData as IDictionary<string, object>;
            Regex rgx = new Regex(FapPlatformConstants.VariablePattern);
            MatchCollection matchs = rgx.Matches(condition);
            foreach (Match item in matchs)
            {
                int length = item.ToString().Length - 3;
                string fn = item.ToString().Substring(2, length);

                condition = condition.Replace(item.ToString(), dapperRow[fn].ToStringOrEmpty());
            }
            string sql = $"select 1 where {condition}";
            if (_dataAccessor.DatabaseDialect != DatabaseDialectEnum.MSSQL)
            {
                sql = $"select 1 from dual where {condition}";

            }
            object o = null;
            _dataAccessor.ExecuteScalar(sql);
            if (o != null && DBNull.Value != o)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取审批人通过业务角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private PerformerList GetPerformerByRole(string roleId)
        {
            PerformerList plist = new PerformerList();
            DynamicParameters param = new DynamicParameters();
            param.Add("RoleUid", roleId);
            IEnumerable<FapBizRoleEmployee> employees = _dataAccessor.QueryWhere<FapBizRoleEmployee>("BizRoleUid=@RoleUid", param, true);
            if (employees != null && employees.Count() > 0)
            {
                foreach (var employee in employees)
                {
                    Performer pf = new Performer();
                    pf.UserId = employee.EmpUid;
                    pf.UserName = employee.EmpUidMC;
                    plist.Add(pf);
                }
            }
            return plist;
        }
        /// <summary>
        /// 获取审批人通过动态角色
        /// </summary>
        /// <param name="roleId">动态角色</param>
        /// <param name="colName">绑定列</param>
        /// <param name="bizData">业务数据</param>
        /// <returns></returns>
        private PerformerList GetPerformerByDynRole(string roleId, string colName, IDictionary<string, object> bizData)
        {
            PerformerList plist = new PerformerList();
            DynamicParameters param = new DynamicParameters();
            param.Add("RoleUid", roleId);
            IEnumerable<FapBizDynRole> bizDynRole = _dataAccessor.QueryWhere<FapBizDynRole>("Fid=@RoleUid", param);
            foreach (var item in bizDynRole)
            {
                string sql = item.CustomSql;
                if (string.IsNullOrWhiteSpace(sql)) continue;
                if (sql.Contains("{申请人}")) //申请人
                {
                    if (bizData.TryGetValue("AppEmpUid", out object appEmpUid))
                    {
                        sql = sql.Replace("{申请人}", appEmpUid?.ToString());
                    }
                    else
                    {
                        sql = sql.Replace("{申请人}", "未知");
                    }
                }
                //这个要去掉，存在代理人的情况下 无法确定处理人
                //if (sql.Contains("{处理人}")) //处理人
                //{
                //    sql = sql.Replace("{处理人}", _session.EmpUid);
                //}

                if (sql.Contains("{指定字段}")) //指定字段
                {
                    if (bizData.TryGetValue(colName, out object colValue))
                    {
                        sql = sql.Replace("{指定字段}", colValue?.ToString());
                    }
                    else
                    {
                        sql = sql.Replace("{指定字段}", "未知");
                    }
                }
                if (sql.IsSafeSQL())
                {
                    IEnumerable<Employee> list = _dataAccessor.Query<Employee>("SELECT Fid,EmpName FROM Employee where Fid IN (" + sql + ")");
                    foreach (var employee in list)
                    {
                        plist.Add(new Performer { UserId = employee.Fid, UserName = employee.EmpName });
                    }
                }
            }
            return plist;
        }
        #endregion

    }
}
