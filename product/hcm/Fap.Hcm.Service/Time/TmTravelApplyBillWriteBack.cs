using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Interface;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Time
{
    /// <summary>
    /// 出差回写出差统计
    /// </summary>
    [Service]
    public class TmTravelApplyBillWriteBack: IBillWritebackService
    {
        private readonly ITimeService _timeService;
        private readonly IDbContext _dbContext;
        public TmTravelApplyBillWriteBack(ITimeService timeService, IDbContext dbContext)
        {
            _timeService = timeService;
            _dbContext = dbContext;
        }
        [Transactional]
        public void Exec(FapDynamicObject billData, FapDynamicObject bizData)
        {
            string empUid = billData.Get("AppEmpUid").ToString();
            string startDateTime = billData.Get("StartTime").ToString();
            string endDateTime = billData.Get("EndTime").ToString();
            var appInfoList = _timeService.LeaveDaysInfo(empUid, startDateTime, endDateTime);
            string billUid = billData.Get("Fid").ToString();
            string travelType = billData.Get("TravelType").ToString();
            IList<TmTravelStat> stats = new List<TmTravelStat>();
            foreach (var appinfo in appInfoList)
            {
                TmTravelStat lstat = new TmTravelStat
                {
                    EmpUid = empUid,
                    BillUid = billUid,
                    WorkDate = appinfo.ApplyDate,
                    TravelTypeUid = travelType,
                    TravelDays = appinfo.Days,
                    TravelHours = appinfo.Hours
                };
                stats.Add(lstat);
            }
            _dbContext.InsertBatchSql(stats);
        }
    }
}
