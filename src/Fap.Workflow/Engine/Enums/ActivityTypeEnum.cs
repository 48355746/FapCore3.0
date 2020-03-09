
namespace Fap.Workflow.Engine.Enums
{
    public enum ActivityTypeEnum : int
    {
        /// <summary>
        /// 开始节点
        /// </summary>
        StartNode = 0,

        /// <summary>
        /// 结束节点
        /// </summary>
        EndNode = 99,

        /// <summary>
        /// 普通任务节点
        /// </summary>
        TaskNode = 1,

        /// <summary>
        /// 子流程节点
        /// </summary>
        SubProcessNode =98,

        /// <summary>
        /// 会签节点
        /// </summary>
        SignNode = 2,
        /// <summary>
        /// 定时节点
        /// </summary>
        TimerNode=3,
        /// <summary>
        /// 网关节点
        /// </summary>
        GatewayNode = 8,

        /// <summary>
        /// 插件节点
        /// </summary>
        PluginNode = 16,

        /// <summary>
        /// 脚本节点
        /// </summary>
        ScriptNode = 32,

        /// <summary>
        /// 可执行节点
        /// </summary>
        SimpleWorkItem = 52
    }
}
