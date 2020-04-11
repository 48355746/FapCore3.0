using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Interface;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Time
{
    [Service]
    public class TmReCardApplyBillWriteBack : IBillWritebackService
    {
        private readonly ITimeService _timeService;
        public TmReCardApplyBillWriteBack(ITimeService timeService)
        {
            _timeService = timeService;
        }
        public void Exec(FapDynamicObject billData, FapDynamicObject bizData)
        {
            string empUid = billData.Get("AppEmpUid").ToString();
            //string billUid = billData.Get("Fid").ToString();
            string startDate = billData.Get("CardStartDate").ToString();
            string endDate = billData.Get("CardEndDate").ToString();
            _timeService.ReCard(empUid, startDate, endDate);            
        }
    }
}
