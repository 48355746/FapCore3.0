using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Fap.Core.Infrastructure.Enums
{
    public enum OperEnum
    {
        /// <summary>
        /// 查找
        /// </summary>
        [Description("查询")]
        Search = 1,
        /// <summary>
        /// 刷新
        /// </summary>
        [Description("刷新")]
        Refresh = 2,
        /// <summary>
        /// 增加
        /// </summary>
        [Description("新增")]
        Add = 4,
        /// <summary>
        /// 修改
        /// </summary>
        [Description("修改")]
        Update = 8,
        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Delete = 16,
        /// <summary>
        /// 导出 excel
        /// </summary>
        [Description("导出Excel")]
        ExportExcel = 32,
        /// <summary>
        /// 导入
        /// </summary>
        [Description("导入")]
        Import = 64,
        /// <summary>
        /// 批量编辑
        /// </summary>
        [Description("批量编辑")]
        BatchUpdate = 128,
        /// <summary>
        /// 查看
        /// </summary>
        [Description("查看")]
        View = 256,   
        /// <summary>
        /// 导出word
        /// </summary>
        [Description("导出word")]
        ExportWord = 512,
        /// <summary>
        /// 查询方案
        /// </summary>
        [Description("查询方案")]
        QueryProgram = 1024,
        /// <summary>
        /// 图表
        /// </summary>
        [Description("图表")]
        Chart=2048



    }
}
