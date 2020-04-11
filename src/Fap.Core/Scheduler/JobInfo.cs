using Quartz;

namespace Fap.Core.Scheduler
{
    public class JobConstants
    {
        /// <summary>
        /// 任务调度执行sql时传递到上下文环境参数的key
        /// </summary>
        public const string JobExecSqlKey = "FAP::JOBEXECSQLKEY";
        /// <summary>
        /// Restful Api key
        /// </summary>
        public const string JobRestfulApiKey = "FAP::JOBRESTFULAPIKEY";
        /// <summary>
        /// 接口服务
        /// </summary>
        public const string JobIServiceProviderKey = "FAP::JOBISERVICEPROVIDERKEY";
        /// <summary>
        /// 系统配置
        /// </summary>
        //public const string JobFapOptionKey = "FAP::JOBFAPOPTIONKEY";
        /// <summary>
        /// 日志工厂
        /// </summary>
        //public const string JobIloggerFactoryKey = "FAP::JOBILOGGERFACTORYKEY";
        /// <summary>
        /// 数据访问
        /// </summary>
        //public const string JobDbSessionFactoryKey = "FAP::JOBDbSessionFACTORYKEY";
        /// <summary>
        /// 系统配置服务
        /// </summary>
        //public const string JobIFapConfigServiceKey = "FAP::JOBIFAPCONFIGSERVICEKEY";
        //public const string JobICacheServiceKey = "FAP::JOBICACHESERVICEKEY";
        
    }
    /// <summary>
    /// 调度任务信息（任务+触发器）
    /// </summary>
    public class JobInfo
    {
        /// <summary>
        /// 调度任务
        /// </summary>
        public IJobDetail Job { get; set; }
        /// <summary>
        /// 任务触发器
        /// </summary>
        public ITrigger Trigger { get; set; }
    }
}
