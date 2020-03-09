
using Fap.Workflow.Engine.Enums;

namespace Fap.Workflow.Engine.Xpdl.Entity
{
    /// <summary>
    /// participant entity 对应xml中的Participant
    /// </summary>
    public class Participant
    {
        /// <summary>
        /// 执行人类型：role角色、dynrole动态角色、user人员
        /// </summary>
        public ParticipantTypeEnum Type { get; set; }
        /// <summary>
        /// 执行人ID
        /// </summary>
        public string ID { get; set; } 
        /// <summary>
        /// 执行人名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 预留字段
        /// </summary>
        public string Code { get; set; }
        //public string OuterID { get; set; }
        /// <summary>
        /// 针对动态角色，自动绑定关系
        /// </summary>
        public string BindField { get; set; }
    }
}
