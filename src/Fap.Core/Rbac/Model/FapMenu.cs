using Dapper.Contrib.Extensions;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Model.Infrastructure
{
    /// <summary>
    /// 系统菜单表
    /// </summary>
    public class FapMenu :BaseModel
    {
        /// <summary>
        /// 菜单编码
        /// </summary>
        public string MenuCode { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }
        /// <summary>
        /// 菜单地址
        /// </summary>
        public string MenuUrl { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 所属模块
        /// </summary>
        public string ModuleUid { get; set; }
        /// <summary>
        /// 所属模块 的显性字段MC
        /// </summary>
        [Computed]
        public string ModuleUidMC { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int MenuOrder { get; set; }
        /// <summary>
        /// 父级菜单
        /// </summary>
        public string Pid { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public int ActiveFlag { get; set; }
        /// <summary>
        /// 徽章插件，实现IMenuBadge
        /// </summary>
        public string BadgePlusClass { get; set; }
       
        /// <summary>
        /// 所属产品
        /// </summary>
        public string ProductUid { get; set; }
        /// <summary>
        /// 所属产品 的显性字段MC
        /// </summary>
        [Computed]
        public string ProductUidMC { get; set; }


    }
}
