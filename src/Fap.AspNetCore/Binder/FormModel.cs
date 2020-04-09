using Fap.AspNetCore.Serivce;
using Fap.Core.Infrastructure.Metadata;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fap.AspNetCore.Binder
{
    [ModelBinder(BinderType = typeof(FormModelBinder))]
    public class FormModel
    {
        /// <summary>
        /// 操作状态
        /// </summary>
        public FormOperEnum Oper { get; set; }
        /// <summary>
        /// 逻辑删除
        /// </summary>
        public bool LogicDelete { get; set; } = true;
        /// <summary>
        /// 批量处理Ids
        /// </summary>
        public string Ids { get; set; }
        /// <summary>
        /// 避免重复提交
        /// </summary>
        public string AvoidDuplicateKey { get; set; }      
        /// <summary>
        /// 主表
        /// </summary>
        [Required]
        public string TableName { get; set; }
        /// <summary>
        /// 主数据
        /// </summary>
        public FapDynamicObject MainData { get; set; }
        /// <summary>
        /// 子表数据
        /// </summary>
        public IDictionary<string, IEnumerable<FapDynamicObject>> ChildDataList { get; set; }
    }
}
