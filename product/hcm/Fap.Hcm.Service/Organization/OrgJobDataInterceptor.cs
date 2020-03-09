using Fap.Core.DataAccess;
using Fap.Core.DataAccess.Interceptor;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Fap.Hcm.Service.Organization
{
    [Service]
    public class OrgJobDataInterceptor : DataInterceptorBase
    {
        public OrgJobDataInterceptor(IServiceProvider provider, IDbContext dbContext) : base(provider, dbContext)
        {
        }
        public override void BeforeDynamicObjectInsert(FapDynamicObject fapDynamicData)
        {
            HandleJob(fapDynamicData);
        }

        private void HandleJob(FapDynamicObject fapDynamicData)
        {
            string pid = fapDynamicData.Get("Pid").ToString();
            if (pid.IsMissing())
            {
                int c = _dbContext.Count<OrgJob>("Pid=''") + 1;
                string jobCode = c.ToString().PadLeft(2, '0');
                fapDynamicData.SetValue("JobCode", jobCode);
                fapDynamicData.SetValue("TreeLevel", 0);
                fapDynamicData.SetValue("JobOrder", c);
            }
            else
            {
                var param = new Dapper.DynamicParameters(new { Pid = pid });
                var parentJob = _dbContext.Get<OrgJob>(pid);
                if (parentJob.IsFinal == 1)
                {
                    parentJob.IsFinal = 0;
                    _dbContext.Update(parentJob);
                }
                var jobs = _dbContext.QueryWhere<OrgJob>("Pid=@Pid", param);
                fapDynamicData.SetValue("TreeLevel", parentJob.TreeLevel + 1);
                fapDynamicData.SetValue("JobCode", jobs.Any() ? (jobs.Max(d => d.JobCode).ToInt() + 1).ToString() : $"{parentJob.JobCode}01");
            }
        }

        public override void BeforeDynamicObjectUpdate(FapDynamicObject fapDynamicData)
        {
            string fid = fapDynamicData.Get("Fid").ToString();
            OrgJob orgJob = _dbContext.Get<OrgJob>(fid);
            string pid = fapDynamicData.Get("Pid").ToString();
            //父部门没变化
            if (pid != orgJob.Pid)
            {
                HandleJob(fapDynamicData);
            }           
        }

    }
}
