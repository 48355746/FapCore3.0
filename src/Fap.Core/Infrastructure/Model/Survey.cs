using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Model
{
    /// <summary>
    /// 调查问卷
    /// </summary>
    public class Survey : BaseModel
    {
        /// <summary>
        /// 问卷名称
        /// </summary>
        public string SurName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string SurContent { get; set; }
        /// <summary>
        /// 共享
        /// </summary>
        public int IsShare { get; set; }
        /// <summary>
        /// 发布方式
        /// </summary>
        public string FilterModel { get; set; }
        /// <summary>
        /// 发布方式 的显性字段MC
        /// </summary>
        [Computed]
        public string FilterModelMC { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 完成情况
        /// </summary>
        public string Completed { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string SurStatus { get; set; }
        /// <summary>
        /// 状态 的显性字段MC
        /// </summary>
        [Computed]
        public string SurStatusMC { get; set; }
        /// <summary>
        /// 收集量
        /// </summary>
        public int CollectionAmount { get; set; }
        /// <summary>
        /// 已收集数量
        /// </summary>
        public int Amounted { get; set; }
        /// <summary>
        /// 目标用户
        /// </summary>
        public string TargetUser { get; set; }
        /// <summary>
        /// 质量控制
        /// </summary>
        public int QualityControl { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        public string UserUid { get; set; }
        /// <summary>
        /// 创建用户 的显性字段MC
        /// </summary>
        [Computed]
        public string UserUidMC { get; set; }
        /// <summary>
        /// 创建员工
        /// </summary>
        public string EmpUid { get; set; }
        /// <summary>
        /// 创建员工 的显性字段MC
        /// </summary>
        [Computed]
        public string EmpUidMC { get; set; }
        /// <summary>
        /// 调研结果
        /// </summary>
        public string SurResult { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string JSONContent { get; set; }
        /// <summary>
        /// 预览JSON
        /// </summary>
        public string JSONPreview { get; set; }
        /// <summary>
        /// 发布JSON
        /// </summary>
        public string JSONPublish { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public string PublishTime { get; set; }
        /// <summary>
        /// 收集开始时间
        /// </summary>
        public string SurStartDate { get; set; }
        /// <summary>
        /// 收集结束时间
        /// </summary>
        public string SurEndDate { get; set; }
        /// <summary>
        /// 未锁定
        /// </summary>
        public int IsUnlocked { get; set; }
        /// <summary>
        /// 在线
        /// </summary>
        public int IsOnline { get; set; }
        /// <summary>
        /// 投票类型
        /// </summary>
        public int VoteType { get; set; }

    }
    /// <summary>
    /// 问卷问题
    /// </summary>
    public class SurQuestion :BaseModel
    {
        /// <summary>
        /// 关联调查
        /// </summary>
        public string SurveyUid { get; set; }
        /// <summary>
        /// 关联调查 的显性字段MC
        /// </summary>
        [Computed]
        public string SurveyUidMC { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 问题类型
        /// </summary>
        public string TypeId { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortIndex { get; set; }
        /// <summary>
        /// 其它
        /// </summary>
        public int HasOther { get; set; }
        /// <summary>
        /// 必填
        /// </summary>
        public int Required { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageNum { get; set; }
        /// <summary>
        /// 索引
        /// </summary>
        public int QIndex { get; set; }
        /// <summary>
        /// 绝对编号
        /// </summary>
        public int AbsoluteId { get; set; }
        /// <summary>
        /// 末尾绝对编号
        /// </summary>
        public int LastAbsoluteId { get; set; }
        /// <summary>
        /// 逻辑隐藏
        /// </summary>
        public int LogicHide { get; set; }
        /// <summary>
        /// 互斥选项
        /// </summary>
        public string ExclusiveOptions { get; set; }
        /// <summary>
        /// 标题引用
        /// </summary>
        public int TitleQuote { get; set; }
        /// <summary>
        /// 选择项引用
        /// </summary>
        public string ChoiceQuote { get; set; }

        /// <summary>
        /// 最大值
        /// </summary>
        public string MaxValue { get; set; }
        /// <summary>
        /// 最小值
        /// </summary>
        public string MinValue { get; set; }
        /// <summary>
        /// JSON内容
        /// </summary>
        public string JSONContent { get; set; }
        /// <summary>
        /// 跳转关系
        /// </summary>
        public string RedirectRelation { get; set; }

    }
    /// <summary>
    /// 矩阵标题
    /// </summary>
    public class SurArrayTitle : BaseModel
    {
        /// <summary>
        /// 问题
        /// </summary>
        public string QuestionUid { get; set; }
        /// <summary>
        /// 问题ID
        /// </summary>
        public int QuestionID { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortIndex { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 问卷
        /// </summary>
        public string SurveyUid { get; set; }

    }
    /// <summary>
    /// 收集设置
    /// </summary>
    public class SurFilter : BaseModel
    {
        /// <summary>
        /// 问卷
        /// </summary>
        public string SurveyUid { get; set; }
        /// <summary>
        /// 问卷 的显性字段MC
        /// </summary>
        [Computed]
        public string SurveyUidMC { get; set; }
        /// <summary>
        /// 收集目标条件
        /// </summary>
        public string FilterCondition { get; set; }
        /// <summary>
        /// 总收集量
        /// </summary>
        public int Amounted { get; set; }
        /// <summary>
        /// 开始收集时间
        /// </summary>
        public string SurStartDate { get; set; }
        /// <summary>
        /// 结束收集时间
        /// </summary>
        public string SurEndDate { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public string PublishTime { get; set; }
        /// <summary>
        /// 收集方式
        /// </summary>
        public string FilterModel { get; set; }

    }
    /// <summary>
    /// 问题选项
    /// </summary>
    public class SurQuestionChoice : BaseModel
    {
        /// <summary>
        /// 关联问题
        /// </summary>
        public string QuestionUid { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortIndex { get; set; }
        /// <summary>
        /// 选项绝对编号
        /// </summary>
        public int ChoiceAbsoluteId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 是否含文本框
        /// </summary>
        public int IsOther { get; set; }
        /// <summary>
        /// 必填
        /// </summary>
        public int Required { get; set; }
        /// <summary>
        /// 问题ID
        /// </summary>
        public int QuestionID { get; set; }
        /// <summary>
        /// 问卷
        /// </summary>
        public string SurveyUid { get; set; }

    }
    /// <summary>
    /// 报表过滤条件
    /// </summary>
    public class SurReportFilter : BaseModel
    {
        /// <summary>
        /// 问卷
        /// </summary>
        public string SurveyUid { get; set; }
        /// <summary>
        /// 问卷 的显性字段MC
        /// </summary>
        [Computed]
        public string SurveyUidMC { get; set; }
        /// <summary>
        /// 问题
        /// </summary>
        public string QuestionUid { get; set; }
        /// <summary>
        /// 问题 的显性字段MC
        /// </summary>
        [Computed]
        public string QuestionUidMC { get; set; }
        /// <summary>
        /// 选项
        /// </summary>
        public string ChoiceUid { get; set; }
        /// <summary>
        /// 选项 的显性字段MC
        /// </summary>
        [Computed]
        public string ChoiceUidMC { get; set; }
        /// <summary>
        /// 矩阵标题
        /// </summary>
        public string TitleUid { get; set; }
        /// <summary>
        /// 矩阵标题 的显性字段MC
        /// </summary>
        [Computed]
        public string TitleUidMC { get; set; }
        /// <summary>
        /// 条件
        /// </summary>
        public string ConditionId { get; set; }
        /// <summary>
        /// 条件 的显性字段MC
        /// </summary>
        [Computed]
        public string ConditionIdMC { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int TypeId { get; set; }


    }
    /// <summary>
    /// 用户问卷情况
    /// </summary>
    public class SurResponseList : BaseModel
    {
        /// <summary>
        /// 问卷
        /// </summary>
        public string SurveyUid { get; set; }
        /// <summary>
        /// 问卷 的显性字段MC
        /// </summary>
        [Computed]
        public string SurveyUidMC { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        public string UserUid { get; set; }
        /// <summary>
        /// 用户 的显性字段MC
        /// </summary>
        [Computed]
        public string UserUidMC { get; set; }
        /// <summary>
        /// 员工
        /// </summary>
        public string EmpUid { get; set; }
        /// <summary>
        /// 员工 的显性字段MC
        /// </summary>
        [Computed]
        public string EmpUidMC { get; set; }
        /// <summary>
        /// 答题时长
        /// </summary>
        public double TimeLength { get; set; }
        /// <summary>
        /// 答题时长
        /// </summary>
        public string TimeLenDesc { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        public string SubmitTime { get; set; }
        /// <summary>
        /// 开始答卷时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortIndex { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string ResponseStatus { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress { get; set; }

    }
    /// <summary>
    /// 调查结果
    /// </summary>
    [Serializable]
    public class SurResult : BaseModel
    {
        /// <summary>
        /// 调查问卷
        /// </summary>
        public string SurveyUid { get; set; }
        /// <summary>
        /// 调查问卷 的显性字段MC
        /// </summary>
        [Computed]
        public string SurveyUidMC { get; set; }
        /// <summary>
        /// 问题
        /// </summary>
        public string QuestionUid { get; set; }
        /// <summary>
        /// 问题 的显性字段MC
        /// </summary>
        [Computed]
        public string QuestionUidMC { get; set; }
        /// <summary>
        /// 矩阵标题
        /// </summary>
        public string TitleUid { get; set; }
        /// <summary>
        /// 答案
        /// </summary>
        public string Answer { get; set; }
        /// <summary>
        /// 其它答案
        /// </summary>
        public string AnswerOther { get; set; }
        /// <summary>
        /// 填写时间
        /// </summary>
        public string FillDate { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        public string UserUid { get; set; }
        /// <summary>
        /// 用户 的显性字段MC
        /// </summary>
        [Computed]
        public string UserUidMC { get; set; }
        /// <summary>
        /// 员工
        /// </summary>
        public string EmpUid { get; set; }
        /// <summary>
        /// 员工 的显性字段MC
        /// </summary>
        [Computed]
        public string EmpUidMC { get; set; }
        /// <summary>
        /// 多选答案
        /// </summary>
        public string Answers { get; set; }
        /// <summary>
        /// 问卷人
        /// </summary>
        public string ResponseUid { get; set; }

    }
    /// <summary>
    /// 字典表的常量 - 调查问卷状态(SurveyStatus)
    /// 注意：最好由FAP工具生成，不需要手动修改
    /// 只针对属于系统编码的字典
    /// </summary>
    public class SurveyStatus
    {
        /// <summary>
        /// 创建中
        /// </summary>
        public static string Creating = "0";
        /// <summary>
        /// 编辑中
        /// </summary>
        public static string Editing = "2";
        /// <summary>
        /// 审核中
        /// </summary>
        public static string Checking = "3";
        /// <summary>
        /// 审核不通过
        /// </summary>
        public static string NoChecked = "1";
        /// <summary>
        /// 发布中
        /// </summary>
        public static string Publishing = "4";
        /// <summary>
        /// 已完成
        /// </summary>
        public static string Completed = "5";

    }
}
