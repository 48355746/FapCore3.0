
namespace Fap.Workflow.Engine.Entity
{
    /// <summary>
    /// 流程模板实体类
    /// </summary>
    public class ProcessTemplateEntity
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 模板Fid
        /// </summary>
        public string TemplateId { get; set; }
        /// <summary>
        /// 模板类型
        /// </summary>
        public string TemplateType { get; set; }
        /// <summary>
        /// 表单类型(外挂表单,无表单)
        /// </summary>
        public string FormType { get; set; }
        /// <summary>
        /// 表单外挂类型（标准表单、自由表单） ，针对外挂表单
        /// </summary>
        public string FormAddonType { get; set; }
        /// <summary>
        /// 表单外挂链接
        /// </summary>
        public string FormUrl { get; set; }
        /// <summary>
        /// 模板名称
        /// </summary>
        //public string TemplateName { get; set; }
        /// <summary>
        /// 流程图实体
        /// </summary>
        public ProcessFileEntity Flow { get; set; }
       
    }
}
