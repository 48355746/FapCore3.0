using Fap.Workflow.Engine.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Workflow.Engine.Xpdl
{
    public class FieldEntity
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// 属性
        /// </summary>
        public FieldPropertyEnum Property { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string FieldNameMC{get;set;}
    }
   
}
