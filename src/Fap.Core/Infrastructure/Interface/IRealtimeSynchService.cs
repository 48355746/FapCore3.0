namespace Fap.Core.Infrastructure.Interface
{
    /// <summary>
    /// 实时同步数据
    /// </summary>
    public interface IRealtimeSynchService
    {
        /// <summary>
        /// 同步方法
        /// </summary>
        /// <param name="content">实时同步数据</param>
        void Synch(RealtimeData data);
        SynchResult PostWebRequest(string postUrl, string paramData, int tryNumber);
    }

    /// <summary>
    /// 实时同步数据类
    /// </summary>
    public class RealtimeData
    {
        public readonly static string OPER_ADD = "add";
        public readonly static string OPER_UPDATE = "update";
        public readonly static string OPER_DELETE = "delete";
        /// <summary>
        /// 同步数据类型，比如employee人员同步、dept部门同步、position岗位同步
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 同步操作类型：add新增、update更新、delete删除
        /// </summary>
        public string Oper { get; set; }
        /// <summary>
        /// 要同步的数据对象
        /// </summary>
        public object Data { get; set; }
    }
    public class SynchResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 是否异常的远程地址
        /// </summary>
        public bool IsExceptionRemoteAddress { get; set; }
        /// <summary>
        /// 尝试次数
        /// </summary>
        public int TryNumber { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrMsg { get; set; }
    }
}
