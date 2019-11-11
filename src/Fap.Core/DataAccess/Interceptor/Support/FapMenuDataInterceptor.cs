using System;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Events;
using Fap.Core.Platform.Domain;
/* ==============================================================================
 * 功能描述：  
 * 创 建 者：wyf
 * 创建日期：2017-05-17 19:03:06
 * ==============================================================================*/
using Fap.Core.Rbac;
using Microsoft.Extensions.Logging;
using Fap.Core.DataAccess.BaseAccess;
using Fap.Model.MetaData;
using Microsoft.Extensions.DependencyInjection;

namespace Fap.Core.Infrastructure.Support.Interceptor
{
    public class FapMenuDataInterceptor : DataInterceptorBase
    {
        ILogger _logger;

        public FapMenuDataInterceptor(IServiceProvider provider, IDbContext dataAccessor, DbSession dbSession) : base(provider, dataAccessor, dbSession)
        {
            _logger = provider.GetService<ILoggerFactory>().CreateLogger<FapMenuDataInterceptor>();
        }

        public override void AfterDynamicObjectDelete(dynamic dynamicData)
        {
            _appDomain.MenuSet.Refresh();
        }
        public override void AfterDynamicObjectInsert(dynamic dynamicData)
        {
            _appDomain.MenuSet.Refresh();
        }
        public override void AfterDynamicObjectUpdate(dynamic dynamicData)
        {
            _appDomain.MenuSet.Refresh();
        }

    }
}
