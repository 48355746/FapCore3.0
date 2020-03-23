using System.Collections.Generic;
using Fap.AspNetCore.ViewModel;

namespace Fap.Hcm.Service.Time
{
    public interface ITimeService
    {
        void AddHoliday(string caseUid, IEnumerable<TmHoliday> list);
        void BuilderDayResultBq(string fid);
        double GetAnnaulRemainderNum(string EmpUid);
        double GetEmpWorkDaysByTime(string EmpUid, string StartDate, string EndDate, string LeaveType, out string msg, out double workhours, string Billcode = null);
        TmLeaveType GetLeaveType(string TypeCode);
      
        double GetTuneoffHoursLength(string EmpUid);
        //年假初始化
        void AnnualLeaveInit(string year,string startDate,string endDate);
        //年假结余
        void AnnualLeaveSurplus(string year,string lastYear);
        /// <summary>
        /// 排班
        /// </summary>
        /// <param name="empList">员工</param>
        /// <param name="shiftUid">班次</param>
        /// <param name="holidayUid">休息日套</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        void Schedule(IEnumerable<Fap.Core.Rbac.Model.Employee> empList, string shiftUid, string holidayUid, string startDate, string endDate);
        bool ExistEmpWorkDaysByTime(string EmpUid, string StartDate, string EndDate);
        void ReCard(string empUid, string empCode, string startTime, string endTime);
        /// <summary>
        /// 部门批量打卡
        /// </summary>
        /// <param name="deptUids"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        void BatchPatchCard(IList<string> deptUids, string startDate, string endDate);
        /// <summary>
        /// 日结果计算(计算当前考勤期间)
        /// </summary>
        void DayResultCalculate();
    }
}