using System.Collections.Generic;
using Fap.AspNetCore.ViewModel;

namespace Fap.Hcm.Service.Time
{
    public interface ITimeService
    {
        void AddHoliday(string caseUid, IEnumerable<TmHoliday> list);
        void BalanceLeaveData(string periodUid);
        void BuilderDayResultBq(string fid);
        double GetAnnaulRemainderNum(string EmpUid);
        double GetEmpWorkDaysByTime(string EmpUid, string StartDate, string EndDate, string LeaveType, out string msg, out double workhours, string Billcode = null);
        TmLeaveType GetLeaveType(string TypeCode);
      
        double GetTuneoffHoursLength(string EmpUid);
        void InitLeaveData(string periodUid);
        ResponseViewModel InitSchedule(string empWhere, string shiftUid, string holidayUid, string startDate, string endDate);
        bool ExistEmpWorkDaysByTime(string EmpUid, string StartDate, string EndDate);
        void ReCard(string empUid, string empCode, string startTime, string endTime);
        void ReCardNoApply(ReCard recard);
    }
}