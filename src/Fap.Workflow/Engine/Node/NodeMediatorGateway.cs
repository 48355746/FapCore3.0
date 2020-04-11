using Fap.Core.DataAccess;
using Fap.Workflow.Engine.Common;
using Fap.Workflow.Engine.Enums;
using Fap.Workflow.Engine.Manager;
using Fap.Workflow.Engine.Xpdl;
using Fap.Workflow.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Fap.Wrokflow.Engine.Node
{
    /// <summary>
    /// 逻辑控制节点执行器
    /// </summary>
    internal class NodeMediatorGateway
    {
        private ActivityEntity _gatewayActivity;
        internal ActivityEntity GatewayActivity
        {
            get { return _gatewayActivity; }
        }

        private IProcessModel _processModel;
        internal IProcessModel ProcessModel
        {
            get { return _processModel; }
        }
        public readonly WfAppRunner _runner;

        protected readonly IDbContext _dataAccessor;
        protected readonly ILoggerFactory _loggerFactory;
        protected readonly IServiceProvider _serviceProvider;

        internal WfActivityInstance GatewayActivityInstance
        {
            get;
            set;
        }

        private ActivityInstanceManager activityInstanceManager;
        internal ActivityInstanceManager ActivityInstanceManager
        {
            get
            {
                if (activityInstanceManager == null)
                {
                    activityInstanceManager = new ActivityInstanceManager(_serviceProvider);
                }
                return activityInstanceManager;
            }
        }
        
        internal NodeMediatorGateway(ActivityEntity gActivity, IProcessModel processModel, WfAppRunner runner, IServiceProvider serviceProvider)
        {
            _gatewayActivity = gActivity;
            _processModel = processModel;
            _serviceProvider = serviceProvider;
            _dataAccessor =serviceProvider.GetService<IDbContext>();
            _loggerFactory =serviceProvider.GetService<ILoggerFactory>();
            _runner = runner;
        }

        /// <summary>
        /// 创建节点对象
        /// </summary>
        /// <param name="activity">活动</param>
        /// <param name="processInstance">流程实例</param>
        /// <param name="runner">运行者</param>
        protected WfActivityInstance CreateActivityInstanceObject(ActivityEntity activity,
            WfProcessInstance processInstance)
        {
            ActivityInstanceManager aim = new ActivityInstanceManager(_serviceProvider);
            this.GatewayActivityInstance = aim.CreateActivityInstanceObject(processInstance,
                activity,
                _runner);

            return this.GatewayActivityInstance;
        }

        /// <summary>
        /// 插入实例数据
        /// </summary>
        /// <param name="activityInstance">活动资源</param>
        /// <param name="session">会话</param>
        internal virtual void InsertActivityInstance(WfActivityInstance activityInstance)
        {
            ActivityInstanceManager.Insert(activityInstance);
        }

        /// <summary>
        /// 插入连线实例的方法
        /// </summary>
        /// <param name="processInstance">流程实例</param>
        /// <param name="transitionGUID">转移GUID</param>
        /// <param name="fromActivityInstance">来源活动实例</param>
        /// <param name="toActivityInstance">目的活动实例</param>
        /// <param name="transitionType">转移类型</param>
        /// <param name="flyingType">飞跃类型</param>
        /// <param name="runner">运行者</param>
        /// <param name="session">会话</param>
        /// <returns></returns>
        internal virtual void InsertTransitionInstance(WfProcessInstance processInstance,
            string transitionGUID,
            WfActivityInstance fromActivityInstance,
            WfActivityInstance toActivityInstance,
            TransitionTypeEnum transitionType,
            TransitionFlyingTypeEnum flyingType)
        {
            var tim = new TransitionInstanceManager(_serviceProvider);
            var transitionInstanceObject = tim.CreateTransitionInstanceObject(processInstance,
                transitionGUID,
                fromActivityInstance,
                toActivityInstance,
                transitionType,
                flyingType,
                _runner,
                (byte)ConditionParseResultEnum.Passed);

            tim.Insert( transitionInstanceObject);
        }

        /// <summary>
        /// 节点对象的完成方法
        /// </summary>
        /// <param name="activityInsUid">活动实例ID</param>
        /// <param name="runner"></param>
        internal virtual void CompleteActivityInstance(string  activityInsUid)
        {
            //设置完成状态
            ActivityInstanceManager.Complete(activityInsUid, _runner);
        }
        /// <summary>
        /// 获取分支实例的个数
        /// </summary>
        /// <param name="splitActivityInstanceUID"></param>
        /// <param name="processInstanceUid"></param>
        /// <returns></returns>
        protected int GetInstanceGatewayCount(string splitActivityInstanceUID,string processInstanceUid)
        {
            ActivityInstanceManager aim = new ActivityInstanceManager(_serviceProvider);
            return aim.GetInstanceGatewayCount(splitActivityInstanceUID, processInstanceUid);
        }
    }
}
