using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Enums;

namespace Fap.Workflow.Engine.Xpdl
{
    /// <summary>
    /// 转移构造类
    /// </summary>
    public class TransitionBuilder
    {
        #region 静态方法
        /// <summary>
        /// 创建跳转Transition实体对象
        /// </summary>
        /// <param name="sourceActivity">来源节点</param>
        /// <param name="targetActivity">目标节点</param>
        /// <returns>转移实体</returns>
        public static TransitionEntity CreateJumpforwardEmptyTransition(ActivityEntity sourceActivity, ActivityEntity targetActivity)
        {
            TransitionEntity transition = new TransitionEntity();
            transition.TransitionID = string.Empty;
            transition.SourceActivity = sourceActivity;
            transition.SourceActivityID = sourceActivity.ActivityID;
            transition.TargetActivity = targetActivity;
            transition.TargetActivityID = targetActivity.ActivityID;
            transition.DirectionType = TransitionDirectionTypeEnum.Forward;

            return transition;
        }
        #endregion
    }
}
