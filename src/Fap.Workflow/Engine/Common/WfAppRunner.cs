using Fap.Workflow.Engine.Xpdl;
using Fap.Workflow.Engine.Xpdl.Entity;

namespace Fap.Workflow.Engine.Common
{
    /// <summary>
    /// 流程执行人(业务应用的办理者)
    /// </summary>
    public class WfAppRunner
    {        
        public WfAppRunner()
        {
        }
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// 应用实例数据ID
        /// </summary>
        public string BizUid { get; set; }
        /// <summary>
        /// 应用实例表名
        /// </summary>
        public string BillTableName { get; set; }
        ///// <summary>
        ///// 业务数据
        ///// </summary>
        public dynamic BizData { get; set; }
        /// <summary>
        /// 业务分类ID
        /// </summary>
        public string BizTypeId { get; set; }
        /// <summary>
        /// 流程模板ID
        /// </summary>
        public string DiagramUid { get; set; }
        /// <summary>
        /// 流程模板版本号
        /// </summary>
        public int DiagramVersion { get; set; }
        /// <summary>
        /// 流程ID
        /// </summary>
        public string ProcessId { get; set; }
        /// <summary>
        /// 当前流程实例
        /// </summary>
        public string CurrProcessInsUid { get; set; }
        /// <summary>
        /// 当前活动实例
        /// </summary>
        public string CurrActivityInsUid { get; set; }
        public string FlowStatus { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 申请人名称
        /// </summary>
        public string UserName { get; set; }

        #region 下一步可用审批节点
        public ActivityEntity NextActivity { get; set; }

        #endregion
        
        /// <summary>
        /// 当前任务Uid
        /// </summary>
        public string CurrWfTaskUid { get; set; }   
        /// <summary>
        /// 当前Node
        /// </summary>
        public string CurrNodeId { get; set; }
        /// <summary>
        /// 回跳的节点GUID
        /// </summary>
        public string JumpbackActivityId { get; set; }   
        public string ApproveState { get; set; } //审批结论
        public string Comment { get; set; } //批注

        public PerformerList Turners;  //转办人

        public PerformerList Agents; //代办人
    }

    /// <summary>
    /// 流程返签、撤销和退回接收人的实体对象
    /// </summary>
    public class WfBackwardTaskReciever
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string ActivityName { get; set; }

        /// <summary>
        /// 构造WfBackwardReciever实例
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="backwardToActivityName"></param>
        /// <returns></returns>
        public static WfBackwardTaskReciever Instance(string backwardToActivityName,
            string userID,
            string userName)
        {
            var instance = new WfBackwardTaskReciever();
            instance.ActivityName = backwardToActivityName;
            instance.UserID = userID;
            instance.UserName = userName;

            return instance;
        }
    }
}
