using Fap.Core.DataAccess;
using Fap.Core.DataAccess.Interceptor;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.SignalR;
using Microsoft.AspNetCore.SignalR;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;
using Fap.Core.Infrastructure.Model;

namespace Fap.Core.Infrastructure.Interceptor
{
    [Service]
    public class EssPartnerDataInterceptor : DataInterceptorBase
    {
        private readonly IHubContext<OnlineUserHub> _hubContext;
        public EssPartnerDataInterceptor(IServiceProvider provider, IDbContext dbContext) : base(provider, dbContext)
        {
            _hubContext = provider.GetService<IHubContext<OnlineUserHub>>();
        }
        public override void AfterDynamicObjectInsert(FapDynamicObject fapDynamicData)
        {
            string userId = fapDynamicData.Get(nameof(EssPartner.PartnerUid)).ToString();
            string applier =_applicationContext.EmpName;
            _hubContext.Clients.User(userId).SendAsync("PartnerApplyNotifications", applier);
            base.AfterDynamicObjectInsert(fapDynamicData);
        }
    }
}
