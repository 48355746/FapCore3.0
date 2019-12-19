using System;
using Microsoft.Extensions.DependencyInjection;
using Fap.Workflow.Service;
using Fap.Core.DataAccess.Interceptor;
using Fap.Core.DataAccess;
using Fap.Core.Utility;
using Fap.Core.Infrastructure.Metadata;

namespace Fap.Workflow.Interceptor
{
    public class WfTemplateDataInterceptor : DataInterceptorBase
    {
        private readonly IWorkflowService _wfService;

        public WfTemplateDataInterceptor(IServiceProvider provider, IDbContext dataAccessor) : base(provider, dataAccessor)
        {
            _wfService = provider.GetService<IWorkflowService>();
        }


        /// <summary>
        /// 删除之前判断是否关联了业务
        /// </summary>
        /// <param name="dynamicData"></param>
        public override void BeforeDynamicObjectDelete(FapDynamicObject fapDynamicData)
        {
            dynamic dynamicData = fapDynamicData;
            bool allow = _wfService.AllowDeleteProcessTemplate(dynamicData.Fid);
            if (allow)
            {
                _dbContext.Execute($"delete from WfProcess where Fid={dynamicData.Fid}'");
            }
            else
            {
                var wft = _dbContext.Get("WfProcess", dynamicData.Fid);
                throw new Exception($"流程[{wft.TemplateName}]已关联业务，不能删除");
            }
        }
        public override void BeforeDynamicObjectInsert(FapDynamicObject fapDynamicData)
        {
            dynamic dynamicData = fapDynamicData;
            if (!dynamicData.ContainsKey("CollectionId"))
            {
                dynamicData.CollectionId = UUIDUtils.Fid;
            }
            else if (dynamicData.ContainsKey("CollectionId") && (dynamicData.CollectionId == null || dynamicData.CollectionId == ""))
            {
                dynamicData.CollectionId = UUIDUtils.Fid;
            }
        }


    }
}
