using Fap.Core.Configuration;
using Fap.Core.DataAccess.BaseAccess;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Platform.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

namespace Fap.Core.Rbac
{
    /// <summary>
    /// Fap权限中心
    /// </summary>
    [Serializable]
    public class FapDomain : FapRbacDomain
    {
        private static object obj = new object();      
        public FapDomain(IOptions<FapOption> option,ILoggerFactory logger,ISessionFactory sessionFactory, string product="HCM")
            : base(option, logger, sessionFactory, product)
        {
            Init();
        }
      

        /// <summary>
        /// 初始化Fap全局域
        /// </summary>     
        public override PlatformDomainBase Init()
        {          
            return base.Init();
        }
     
        public override void Configure()
        {
            base.Configure();
        }
       
    }
}