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
    /// 请假回写类
    /// </summary>
    [Service]
    public class TmLeavelApplyBillWriteBack : IBillWritebackService
    {
        private readonly ITimeService _timeService;
        private readonly IDbContext _dbContext;
        public TmLeavelApplyBillWriteBack(ITimeService timeService,IDbContext dbContext)
        {
            _timeService = timeService;
            _dbContext = dbContext;
        }
        public void Exec(FapDynamicObject billData, FapDynamicObject bizData)
        {
            string empUid= billData.Get("AppEmpUid").ToString();
            string startDateTime = billData.Get("StartTime").ToString();
            string endDateTime = billData.Get("EndTime").ToString();
            var appInfoList = _timeService.LeaveDaysInfo(empUid, startDateTime, endDateTime);
            string billUid = billData.Get("Fid").ToString();
            string leaveType = billData.Get("LeaveType").ToString();
            IList<TmLeaveStat> stats = new List<TmLeaveStat>();
            foreach (var appinfo in appInfoList)
            {
                TmLeaveStat lstat = new TmLeaveStat
                {
                    EmpUid = empUid,
                    BillUid = billUid,
                    WorkDate = appinfo.ApplyDate,
                    LeaveTypeUid=leaveType,
                    LeaveDays = appinfo.Days,
                    LeaveHours = appinfo.Hours
                };
                stats.Add(lstat);
            }
            _dbContext.InsertBatchSql(stats);
        }
    }
}
