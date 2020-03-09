﻿using System;
using Fap.Workflow.Engine.Enums;

namespace Fap.Workflow.Engine.Xpdl
{
    /// <summary>
    /// 节点类型详细信息
    /// </summary>
    public class ActivityTypeDetail
    {
        /// <summary>
        /// 节点类型
        /// </summary>
        public ActivityTypeEnum ActivityType { get; set; }

        /// <summary>
        /// 会签加签类型
        /// </summary>
        public ComplexTypeEnum ComplexType { get; set; }

        /// <summary>
        /// 串行并行类型(会签加签)
        /// </summary>
        public MergeTypeEnum MergeType { get; set; }

        /// <summary>
        /// 会签加签节点的通过率设置类型
        /// </summary>
        public CompareTypeEnum CompareType { get; set; }

        /// <summary>
        /// 会签主节点记录的通过率
        /// </summary>
        public Nullable<float> CompleteOrder { get; set; }

        /// <summary>
        /// 加签类型
        /// </summary>
        public SignForwardTypeEnum SignForwardType { get; set; }

        /// <summary>
        /// 跳转信息
        /// </summary>
        public SkipInfo SkipInfo { get; set; }
    }

    /// <summary>
    /// 节点的其它附属类型
    /// </summary>
    public enum ComplexTypeEnum
    {
        /// <summary>
        /// 多实例-会签节点
        /// </summary>
        SignTogether = 1,

        /// <summary>
        /// 多实例-加签节点
        /// </summary>
        SignForward = 2
    }

    /// <summary>
    /// 会签节点合并类型
    /// </summary>
    public enum MergeTypeEnum
    {
        /// <summary>
        /// 串行
        /// </summary>
        Sequence = 1,

        /// <summary>
        /// 并行
        /// </summary>
        Parallel = 2
    }

    /// <summary>
    /// 会签节点的通过率设置类型
    /// </summary>
    public enum CompareTypeEnum
    {
        /// <summary>
        /// 个数
        /// </summary>
        Count = 1,

        /// <summary>
        /// 百分比
        /// </summary>
        Percentage = 2
    }

    /// <summary>
    /// 加签类型
    /// </summary>
    public enum SignForwardTypeEnum
    {
        /// <summary>
        /// 不加签
        /// </summary>
        None = 0,

        /// <summary>
        /// 前加签
        /// </summary>
        SignForwardBefore = 1,

        /// <summary>
        /// 后加签
        /// </summary>
        SignForwardBehind = 2,

        /// <summary>
        /// 并行加签
        /// </summary>
        SignForwardParallel = 3
    }

    /// <summary>
    /// 节点类型上描述的跳转信息
    /// </summary>
    public class SkipInfo
    {
        /// <summary>
        /// 是否跳转
        /// </summary>
        public Boolean IsSkip { get; set; }

        /// <summary>
        /// 跳转到的节点
        /// </summary>
        public string Skipto { get; set; }
    }
}
