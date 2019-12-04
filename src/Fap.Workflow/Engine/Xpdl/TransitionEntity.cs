
using Fap.Workflow.Engine.Enums;
using System;
using System.Collections.Generic;

namespace Fap.Workflow.Engine.Xpdl
{
    /// <summary>
    /// 转移定义
    /// </summary>
    public class TransitionEntity
    {
        /// <summary>
        /// 转移GUID
        /// </summary>
        public String TransitionID
        {
            get;
            set;
        }

        /// <summary>
        /// 源活动
        /// </summary>
        public String SourceActivityID
        {
            get;
            set;
        }

        /// <summary>
        /// 目标活动
        /// </summary>
        public String TargetActivityID
        {
            get;
            set;
        }

        /// <summary>
        /// 方向类型
        /// </summary>
        public TransitionDirectionTypeEnum DirectionType
        {
            get;
            set;
        }

        /// <summary>
        /// 接收者类型
        /// </summary>
        public Receiver Receiver
        {
            get;
            set;
        }

        /// <summary>
        /// 条件
        /// </summary>
        public string Condition
        {
            get;
            set;
        }

        /// <summary>
        /// 群体行为类型
        /// </summary>
        public GroupBehaviourEntity GroupBehaviour
        {
            get;
            set;
        }

        /// <summary>
        /// 起始活动
        /// </summary>
        public ActivityEntity SourceActivity
        {
            get;
            set;
        }

        /// <summary>
        /// 到达活动
        /// </summary>
        public ActivityEntity TargetActivity
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 转移列表类
    /// </summary>
    public class TransitonList : List<TransitionEntity>
    {

    }
}
