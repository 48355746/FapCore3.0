using Fap.Core.DataAccess;
using Fap.Core.DataAccess.Interceptor;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Infrastructure.Model;
using Fap.Core.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.Interceptor
{
    [Service]
    public class SurveyDataInterceptor : DataInterceptorBase
    {
        public SurveyDataInterceptor(IServiceProvider provider, IDbContext dbContext) : base(provider, dbContext)
        {
        }
        public override void AfterDynamicObjectInsert(FapDynamicObject fapDynamicData)
        {
            fapDynamicData.SetValue("UserUid", _applicationContext.UserUid);
            fapDynamicData.SetValue("EmpUid", _applicationContext.EmpUid);
            //fapDynamicData.SetValue("CreateTime", DateTimeUtils.CurrentDateTimeStr);
            fapDynamicData.SetValue("SurContent", "欢迎参加调查！答卷数据仅用于统计分析，请放心填写。题目选项无对错之分，按照实际情况选择即可。感谢您的帮助！");
            fapDynamicData.SetValue("Completed", "0/0");
            fapDynamicData.SetValue("SurStatus", SurveyStatus.Creating);

            base.AfterDynamicObjectInsert(fapDynamicData);
        }
    }
}
