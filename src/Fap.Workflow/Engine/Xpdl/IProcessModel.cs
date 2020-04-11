using System.Collections.Generic;
using System.Xml;
using Fap.Workflow.Model;
using Fap.Workflow.Engine.Xpdl.Entity;
using Fap.Workflow.Engine.Entity;

namespace Fap.Workflow.Engine.Xpdl
{
    public interface IProcessModel
    {
        ProcessEntity ProcessEntity { get; set; }
        XmlDocument XmlProcessDefinition { get; }
        ActivityEntity GetBackwardGatewayActivity(ActivityEntity fromActivity, ref int joinCount, ref int splitCount);
        int GetBackwardTransitionListCount(string toActivityGUID);
        ActivityEntity GetActivity(string activityGUID);
        IEnumerable<ActivityEntity> GetAllTaskActivityList();
        ActivityEntity GetEndActivity();
        ActivityEntity GetFirstActivity();
        NextActivityMatchedResult GetFirstActivityList();
        IList<TransitionEntity> GetForwardTransitionList(string sourceActivityID);
        ActivityEntity GetNextActivity(string activityId);
        NextActivityMatchedResult GetNextActivityList(string currentActivityID);
        //IList<NodeView> GetNextActivityTree(int processInstanceID, string currentActivityID);
        IList<NodeView> GetNextActivityTree(string currentActivityID);
        ActivityEntity GetStartActivity();
        IList<Participant> GetActivityParticipants(string activityId);
        PerformerList GetActivityPerformers(string activityId);
    }
}