using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Workflow.Service
{
    /// <summary>
    /// 流程回写接口
    /// </summary>
    public interface IWriteBackService
    {
        /// <summary>
        /// 流程成功时回写业务表
        /// </summary>
        /// <param name="tableName">单据表</param>
        /// <param name="fid">单据fid</param>
        void WriteBackToBusiness(string tableName, string fid);
        /// <summary>
        /// 单据审核通过
        /// </summary>
        /// <param name="billTable"></param>
        /// <param name="billUid"></param>
        void Approved(string billTable, string billUid);
        /// <summary>
        /// 单据被驳回
        /// </summary>
        /// <param name="billTable"></param>
        /// <param name="billUid"></param>
        void Rejected(string billTable, string billUid);
        /// <summary>
        /// 流程失败时处理
        /// </summary>
        void HandleWhenError();

    }

    /// <summary>
    /// 处理单据时流程处于的状态
    /// </summary>
    public enum EnumProcessState
    {
        [Description("发起时")]
        Startup,
        [Description("处理中")]
        Processing,
        [Description("完成后")]
        Completed
    }
}
