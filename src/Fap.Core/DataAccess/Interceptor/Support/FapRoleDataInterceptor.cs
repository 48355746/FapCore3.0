using Fap.Core.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fap.Core.DataAccess.DbContext;
using Fap.Core.Rbac;
using Fap.Core.Platform.Domain;
using Microsoft.Extensions.Logging;
using Fap.Core.Events;
using Fap.Core.DataAccess.BaseAccess;
using Fap.Model.MetaData;

namespace Fap.Core.Infrastructure.Support.Interceptor
{
    public class FapRoleDataInterceptor : DataInterceptorBase
    {
        ILogger _logger;

        public FapRoleDataInterceptor(IServiceProvider provider, IDbContext dataAccessor, DbSession dbSession) : base(provider, dataAccessor, dbSession)
        {
            _logger = _loggerFactory.CreateLogger<FapRoleDataInterceptor>();
        }

        public override void AfterDynamicObjectDelete(dynamic dynamicData)
        {
            _appDomain.RoleSet.Refresh();
        }
        public override void AfterDynamicObjectInsert(dynamic dynamicData)
        {
            _appDomain.RoleSet.Refresh();
        }
        public override void AfterDynamicObjectUpdate(dynamic dynamicData)
        {
            _appDomain.RoleSet.Refresh();
        }

       
    }
}
