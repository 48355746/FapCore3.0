using System;
using Microsoft.Extensions.DependencyInjection;
using Fap.Workflow.Service;
using Fap.Core.DataAccess.Interceptor;
using Fap.Core.DataAccess;
using Fap.Core.Utility;
using Fap.Core.Infrastructure.Metadata;
using Fap.Workflow.Model;
using Fap.Core.Extensions;

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
            string fid = fapDynamicData.Get("Fid").ToString();
            bool allow = _wfService.AllowDeleteProcessTemplate(fid);
            if (allow)
            {
                _dbContext.Execute($"delete from WfProcess where Fid='{fid}'");
            }
            else
            {
                WfProcess wft = _dbContext.Get<WfProcess>(fid);
                throw new Exception($"流程[{wft.ProcessName}]已关联业务，不能删除");
            }
        }
        public override void BeforeDynamicObjectInsert(FapDynamicObject fapDynamicData)
        {
            if (!fapDynamicData.ContainsKey("CollectionId"))
            {
                fapDynamicData.SetValue("CollectionId",UUIDUtils.Fid);
            }
            else if (fapDynamicData.ContainsKey("CollectionId") && (fapDynamicData.Get("CollectionId") == null || fapDynamicData.Get("CollectionId").ToString().IsMissing()))
            {
                fapDynamicData.SetValue("CollectionId", UUIDUtils.Fid);
            }
        }


    }
}
