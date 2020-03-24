using System.Collections.Generic;
using Fap.AspNetCore.ViewModel;

namespace Fap.Hcm.Service.Time
{
    public interface ITimeService
    {
        /// <summary>
        /// 添加休息日
        /// </summary>
        /// <param name="caseUid"></param>
        /// <param name="list"></param>
        void AddHoliday(string caseUid, IEnumerable<TmHoliday> list);
        void BuilderDayResultBq(string fid);
        /// <summary>
        /// 获取有效年假天数
        /// </summary>
        /// <param name="EmpUid"></param>
        /// <returns></returns>
        double ValidAnnualLeaveDays(string EmpUid);
        /// <summary>
        /// 请假天数时长
        /// </summary>
        /// <param name="empUid"></param>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns></returns>
        (double, double) LeavelDays(string empUid, string startDateTime, string endDateTime);
        /// <summary>
        /// 获取请假类型
        /// </summary>
        /// <param name="TypeCode"></param>
        /// <returns></returns>
        TmLeaveType GetLeaveType(string TypeCode);
      
        double GetTuneoffHoursLength(string EmpUid);
        /// <summary>
        /// 年假初始化
        /// </summary>
        /// <param name="year"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        void AnnualLeaveInit(string year,string startDate,string endDate);
        /// <summary>
        /// 年假结余
        /// </summary>
        /// <param name="year"></param>
        /// <param name="lastYear"></param>
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
        void ReCard(string empUid, string empCode, string startTime, string endTime);
        /// <summary>
        /// 部门批量打卡
        /// </summary>
        /// <param name="deptUids"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        void BatchPatchCard(IList<string> deptUids, string startDate, string endDate);
        /// <summary>
        /// 日结果计算(计算当前考勤周期)
        /// </summary>
        void DayResultCalculate();
        /// <summary>
        /// 月结果计算（计算当前考勤周期）
        /// </summary>
        void MonthResultCalculate();
        /// <summary>
        /// 考勤异常通知
        /// </summary>
        /// <param name="options">emp,mgr</param>
        void ExceptionNotice(string[] options);
    }
}