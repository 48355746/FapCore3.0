﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Enums;

namespace Fap.Workflow.Engine.Xpdl
{
    /// <summary>
    /// 连线上的行为属性实体
    /// </summary>
    public class GroupBehaviourEntity
    {
        /// <summary>
        /// 优先级
        /// </summary>
        public short Priority
        {
            get;
            set;
        }

        /// <summary>
        /// 并行选项
        /// </summary>
        public  ParallelOptionEnum ParallelOption
        {
            get;
            set;
        }
    }
}
