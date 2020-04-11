using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Interface;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Fap.Core.Utility;

namespace Fap.Hcm.Service.Time
{
    /// <summary>
    /// 加班回写加班统计
    /// </summary>
    [Service]
    public class TmOvertimeApplyBillWriteBack : IBillWritebackService
    {
        private readonly IDbContext _dbContext;
        public TmOvertimeApplyBillWriteBack(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public void Exec(FapDynamicObject billData, FapDynamicObject bizData)
        {
            string empUid = billData.Get("AppEmpUid").ToString();
            string billUid = billData.Get("Fid").ToString();
            string startDateTime = billData.Get("StartTime").ToString();
            string endDateTime = billData.Get("EndTime").ToString();
            string restHours = billData.Get("RestHours").ToString();
            string overtimeType = billData.Get("OvertimeType").ToString();
            TmOvertimeStat overtimeStat = new TmOvertimeStat();
            overtimeStat.BillUid = billUid;
            overtimeStat.EmpUid = empUid;
            overtimeStat.WorkDate =DateTimeUtils.DateFormat(DateTimeUtils.ToDateTime(startDateTime));
            overtimeStat.DaysOffHours = 0.0;
            overtimeStat.HoursLength = DateTimeUtils.ToDateTime(endDateTime).Subtract(DateTimeUtils.ToDateTime(startDateTime)).TotalHours - (restHours.IsPresent() ? restHours.ToDouble() : 0.0);
            overtimeStat.OvertimeType = overtimeType;
            _dbContext.Insert(overtimeStat);
        }
    }
}
