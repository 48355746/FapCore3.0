using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Rbac.Model;
/* ==============================================================================
* 功能描述：  
* 创 建 者：wyf
* 创建日期：2016/5/17 17:20:33
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using Fap.Core.Extensions;
using Fap.Core.Utility;
using Ardalis.GuardClauses;
using Fap.Core.Exceptions;
using Fap.Core.Infrastructure.Config;
using Fap.Core.Infrastructure.Model;

namespace Fap.Hcm.Service.Time
{
    /// <summary>
    /// 时间管理服务类
    /// </summary>
    [Service]
    public class TimeService : ITimeService
    {
        private readonly IDbContext _dbContext;
        private readonly IFapApplicationContext _applicationContext;
        private readonly IFapConfigService _configService;
        public TimeService(IDbContext dataAccessor, IFapConfigService configService, IFapApplicationContext applicationContext)
        {
            _dbContext = dataAccessor;
            _configService = configService;
            _applicationContext = applicationContext;
        }
        /// <summary>
        /// 添加假日
        /// </summary>
        /// <param name="caseUid">假日套</param>
        /// <param name="list">假日集合</param>
        [Transactional]
        public void AddHoliday(string caseUid, IEnumerable<TmHoliday> list)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("CaseUid", caseUid);
            _dbContext.DeleteExec("TmHoliday", "CaseUid=@CaseUid", param);
            if (list.Any())
            {
                _dbContext.InsertBatchSql<TmHoliday>(list);
            }
        }

        /// <summary>
        /// 初始化排班计划
        /// </summary>
        /// <param name="empWhere">员工条件</param>
        /// <param name="shiftUid">班次</param>
        /// <param name="holidayUid">休息日套</param>
        /// <param name="shiftYear">年份</param>
        [Transactional]
        public void Schedule(IEnumerable<Fap.Core.Rbac.Model.Employee> empList, string shiftUid, string holidayUid, string startDate, string endDate)
        {
            Guard.Against.Null(empList, nameof(empList));
            string sheduleUid = UUIDUtils.Fid;
            ScheduleEmployee();
            ScheduleShift();
            void ScheduleShift()
            {
                //获取休息日
                DynamicParameters paramHolidy = new DynamicParameters();
                paramHolidy.Add("CaseUid", holidayUid);
                var listHolidys = _dbContext.QueryWhere<TmHoliday>("CaseUid=@CaseUid", paramHolidy);
                //获取班次
                var Shift = _dbContext.Get<TmShift>(shiftUid);
                if (Shift != null)
                {
                    startDate += " 00:00:00";
                    endDate += " 23:59:59";
                    string currDate = DateTimeUtils.CurrentDateTimeStr;
                    var TimeS = DateTimeUtils.ToDateTime(endDate) - DateTimeUtils.ToDateTime(startDate);
                    var Days = Convert.ToInt32(TimeS.TotalDays);
                    DateTime wDate = DateTimeUtils.ToDateTime(startDate);

                    IList<TmSchedule> listSchedule = new List<TmSchedule>();
                    for (int i = 0; i < Days; i++)
                    {
                        TmSchedule tmSchedule = new TmSchedule();
                        string wd = DateTimeUtils.DateFormat(wDate.AddDays(i));
                        if (listHolidys.Any(w => w.Holiday == wd))
                        {
                            continue;
                        }
                        tmSchedule.ScheduleUid = sheduleUid;
                        tmSchedule.ShiftUid = shiftUid;
                        tmSchedule.WorkDay = wd;
                        //上班时间
                        tmSchedule.StartTime = wd + " " + Shift.StartTime;
                        //下班时间，可能存在跨天=上班时间+工作时长+休息时长
                        tmSchedule.EndTime = DateTimeUtils.DateTimeFormat(DateTimeUtils.ToDateTime(tmSchedule.StartTime).AddHours(Shift.WorkHoursLength).AddMinutes(Shift.RestMinutesLength));
                        //迟到时间
                        tmSchedule.LateTime = DateTimeUtils.DateTimeFormat(DateTimeUtils.ToDateTime(tmSchedule.StartTime).AddMinutes(Convert.ToDouble(Shift.ComeLate)));

                        //排班休息开始时间
                        tmSchedule.RestStartTime = wd + " " + Shift.RestStartTime;
                        //休息时间跨天处理
                        if (DateTimeUtils.ToDateTime(tmSchedule.StartTime) > DateTimeUtils.ToDateTime(tmSchedule.RestStartTime)) //如果排班跨天
                        {
                            //排班休息开始时间
                            tmSchedule.RestStartTime = DateTimeUtils.DateFormat(DateTimeUtils.ToDateTime(wd).AddDays(1)) + " " + Shift.RestStartTime;
                        }
                        //休息结束时间
                        tmSchedule.RestEndTime = DateTimeUtils.DateTimeFormat(DateTimeUtils.ToDateTime(tmSchedule.RestStartTime).AddMinutes(Convert.ToDouble(Shift.RestMinutesLength)));
                        //早退时间
                        tmSchedule.LeaveTime = DateTimeUtils.DateTimeFormat(DateTimeUtils.ToDateTime(tmSchedule.EndTime).AddMinutes(Convert.ToDouble(-Shift.LeftEarly)));
                        //早打卡开始时间
                        tmSchedule.StartCardTime = DateTimeUtils.DateTimeFormat(DateTimeUtils.ToDateTime(tmSchedule.StartTime).AddHours(Convert.ToDouble(-Shift.EarlyCard)));
                        //最晚打卡时间
                        tmSchedule.EndCardTime = DateTimeUtils.DateTimeFormat(DateTimeUtils.ToDateTime(tmSchedule.EndTime).AddHours(Convert.ToDouble(Shift.LateCard)));

                        tmSchedule.WorkHoursLength = Shift.WorkHoursLength;
                        tmSchedule.RestMinutesLength = Shift.RestMinutesLength;

                        listSchedule.Add(tmSchedule);
                    }
                    _dbContext.InsertBatchSql(listSchedule);

                }
            }
            void ScheduleEmployee()
            {
                //删除存在此区间排班的员工
                string sql = "delete from TmScheduleEmployee where EmpUid in @EmpList and StartDate<@EndDate and EndDate>@StartDate";
                _dbContext.Execute(sql, new DynamicParameters(new { EmpList = empList.Select(e => e.Fid), StartDate = startDate, EndDate = endDate }));
                //add tmshceduleemployee

                IList<TmScheduleEmployee> scheduleEmployeeList = new List<TmScheduleEmployee>();
                foreach (var emp in empList)
                {
                    TmScheduleEmployee schemp = new TmScheduleEmployee();
                    schemp.EmpUid = emp.Fid;
                    schemp.DeptUid = emp.DeptUid;
                    schemp.StartDate = startDate;
                    schemp.EndDate = endDate;
                    schemp.ScheduleUid = sheduleUid;
                    scheduleEmployeeList.Add(schemp);
                }
                _dbContext.InsertBatchSql(scheduleEmployeeList);
            }
        }
        [Transactional]
        public void DayResultCalculate()
        {
            //获取当前月考勤周期的排班
            var currPeriod = GetCurrentPeriod();
            string startDate = currPeriod.StartDate;
            string endDate = currPeriod.EndDate;
            //初始化当月日结果数据通过排班
            InitDayResultBySchedule();

            //此时间段日结果
            string sWhere = $"{nameof(TmDayResult.CurrDate)}>=@StartDate and {nameof(TmDayResult.CurrDate)}<=@EndDate";
            var param = new DynamicParameters(new { StartDate = startDate, EndDate = DateTimeUtils.CurrentDateStr });
            var dayResults = _dbContext.QueryWhere<TmDayResult>(sWhere, param);
            //更新打卡数据到日结果
            UpdateCardRecordToDayResult();
            UpdateTravelAndLeaveToDayResult();
            //计算日结果
            foreach (var dayResult in dayResults)
            {
                //无请假出差
                if (dayResult.TravelType.IsMissing() && dayResult.LeavelType.IsMissing())
                {
                    //计算打卡
                    CalculateCardRecord(dayResult);
                }
                else
                {
                    //计算出差和请假
                    CalculateTravelAndLeavel(dayResult);
                }
            }
            _dbContext.UpdateBatchSql(dayResults);
            //日结果计算后事件
            string afterSql = _configService.GetSysParamValue("AfterDayResultCalculate");
            if (afterSql.IsPresent())
            {
                List<string> sqls = afterSql.SplitSemicolon();
                foreach (var sql in sqls)
                {
                    if (sql.IsPresent())
                    {
                        _dbContext.Execute(sql + " and " + sWhere, param);
                    }
                }
            }
            void CalculateTravelAndLeavel(TmDayResult dayResult)
            {
                string calResult = string.Empty;
                if (dayResult.TravelType.IsPresent())
                {
                    calResult = DayResultEnum.Travel.Description();
                    if (dayResult.TravelHours != dayResult.WorkHoursLength)
                    {
                        calResult += $"{dayResult.TravelHours}小时";
                    }
                }
                if (dayResult.LeavelType.IsPresent())
                {
                    calResult = DayResultEnum.TakeLeave.Description();
                    if (dayResult.LeavelHours != dayResult.WorkHoursLength)
                    {
                        calResult += $"{dayResult.LeavelHours}小时";
                    }
                }
                double abs = dayResult.WorkHoursLength - dayResult.LeavelHours - dayResult.TravelHours - dayResult.StWorkHourLength;
                if (abs > 0)
                {
                    calResult += (DayResultEnum.Absence.Description() + $"{Math.Round(abs, 2)}小时");
                }
            }
            void CalculateCardRecord(TmDayResult dayResult)
            {
                //存在实际工作时长
                if (dayResult.StWorkHourLength > 0)
                {
                    if (DateTimeUtils.ToDateTime(dayResult.CardStartTime) > DateTimeUtils.ToDateTime(dayResult.LateTime)
                        && DateTimeUtils.ToDateTime(dayResult.CardEndTime) > DateTimeUtils.ToDateTime(dayResult.EndTime))
                    {
                        //早打卡迟到且晚打卡大于下班时间
                        dayResult.CalResult = DayResultEnum.ComeLate.Description();
                    }
                    if (DateTimeUtils.ToDateTime(dayResult.CardEndTime) < DateTimeUtils.ToDateTime(dayResult.LeaveTime) &&
                        DateTimeUtils.ToDateTime(dayResult.CardStartTime) < DateTimeUtils.ToDateTime(dayResult.StartTime))
                    {
                        //早打卡正常，晚打卡早于早退时间
                        dayResult.CalResult = DayResultEnum.LeaveEarly.Description();
                    }
                    if (DateTimeUtils.ToDateTime(dayResult.CardStartTime) > DateTimeUtils.ToDateTime(dayResult.LateTime)
                        && DateTimeUtils.ToDateTime(dayResult.CardEndTime) < DateTimeUtils.ToDateTime(dayResult.LeaveTime))
                    {
                        //迟到早退
                        dayResult.CalResult = DayResultEnum.ComeLate.Description() + DayResultEnum.LeaveEarly.Description();
                    }
                    //实际工作时长>工作时长,且无迟到早退
                    if (dayResult.StWorkHourLength >= dayResult.WorkHoursLength)
                    {
                        if (dayResult.CalResult.IsMissing())
                        {
                            dayResult.CalResult = DayResultEnum.Normal.Description();
                        }
                    }
                    else if (dayResult.StWorkHourLength < dayResult.WorkHoursLength)
                    {
                        dayResult.CalResult = DayResultEnum.Absence.Description() + $"{Math.Round(dayResult.WorkHoursLength - dayResult.StWorkHourLength, 2)}小时";
                    }
                }
                else
                {
                    dayResult.CalResult = DayResultEnum.Absence.Description();
                }

            }

            void UpdateCardRecordToDayResult()
            {
                //此时间段(+-1天)所有打卡记录，存在跨天情况
                var cardRecords = _dbContext.QueryWhere<TmCardRecord>($"{nameof(TmCardRecord.CardTime)}>=@StartDate and {nameof(TmCardRecord.CardTime)}<=@EndDate"
                    , new DynamicParameters(new { StartDate = DateTimeUtils.DateTimeFormat(DateTimeUtils.ToDateTime(startDate).AddDays(-1)), EndDate = DateTimeUtils.DateTimeFormat(DateTimeUtils.ToDateTime(endDate).AddDays(1)) }));
                if (!cardRecords.Any())
                {
                    return;
                }
                foreach (var dayResult in dayResults)
                {
                    //当前日结果中考勤打卡集合
                    var cardTimes = cardRecords.Where(c => c.EmpUid == dayResult.EmpUid && DateTimeUtils.ToDateTime(c.CardTime) >= DateTimeUtils.ToDateTime(dayResult.StartCardTime) && DateTimeUtils.ToDateTime(c.CardTime) <= DateTimeUtils.ToDateTime(dayResult.EndCardTime));
                    if (cardTimes.Any())
                    {
                        dayResult.CardStartTime = cardTimes.Min(c => c.CardTime);
                        dayResult.CardEndTime = cardTimes.Max(c => c.CardTime);
                        if (dayResult.CardStartTime == dayResult.CardEndTime)
                        {
                            dayResult.StWorkHourLength = 0;
                        }
                        else
                        {
                            dayResult.StWorkHourLength = Math.Round(DateTimeUtils.ToDateTime(dayResult.CardEndTime).Subtract(DateTimeUtils.ToDateTime(dayResult.CardStartTime)).TotalHours - Math.Round(dayResult.RestMinutesLength / 60.0, 2), 2);
                        }
                    }
                }
            }
            void UpdateTravelAndLeaveToDayResult()
            {
                var travelStatList = _dbContext.QueryWhere<TmTravelStat>($"{nameof(TmTravelStat.WorkDate)}>=@StartDate and {nameof(TmTravelStat.WorkDate)}<=@EndDate"
                    , new DynamicParameters(new { StartDate = DateTimeUtils.DateTimeFormat(DateTimeUtils.ToDateTime(startDate).AddDays(-1)), EndDate = DateTimeUtils.DateTimeFormat(DateTimeUtils.ToDateTime(endDate).AddDays(1)) }));

                var leaveStatList = _dbContext.QueryWhere<TmLeaveStat>($"{nameof(TmLeaveStat.WorkDate)}>=@StartDate and {nameof(TmLeaveStat.WorkDate)}<=@EndDate"
                    , new DynamicParameters(new { StartDate = DateTimeUtils.DateTimeFormat(DateTimeUtils.ToDateTime(startDate).AddDays(-1)), EndDate = DateTimeUtils.DateTimeFormat(DateTimeUtils.ToDateTime(endDate).AddDays(1)) }));
                if (!travelStatList.Any() && !leaveStatList.Any())
                {
                    return;
                }
                foreach (var dayResult in dayResults)
                {
                    if (travelStatList.Any())
                    {
                        var travel = travelStatList.FirstOrDefault(t => t.WorkDate == dayResult.CurrDate&&t.EmpUid==dayResult.EmpUid);
                        if (travel != null)
                        {
                            dayResult.TravelType = travel.TravelTypeUid;
                            dayResult.TravelDays = travel.TravelDays;
                            dayResult.TravelHours = travel.TravelHours;
                        }

                    }
                    if (leaveStatList.Any())
                    {
                        var leave = leaveStatList.FirstOrDefault(t => t.WorkDate == dayResult.CurrDate&&t.EmpUid==dayResult.EmpUid);
                        if (leave != null)
                        {
                            dayResult.LeavelType = leave.LeaveTypeUid;
                            dayResult.LeaveDays = leave.LeaveDays;
                            dayResult.LeavelHours = leave.LeaveHours;
                        }
                    }
                }
            }
            void InitDayResultBySchedule()
            {

                int isExist = _dbContext.Count(nameof(TmDayResult), $"{nameof(TmDayResult.CurrDate)}>='{currPeriod.StartDate}' and {nameof(TmDayResult.CurrDate)}<='{currPeriod.EndDate}'");
                if (isExist > 0)
                {
                    return;
                }
                var schedules = _dbContext.QueryWhere<TmSchedule>($"{nameof(TmSchedule.WorkDay)}>= '{currPeriod.StartDate}' and {nameof(TmSchedule.WorkDay)}<='{currPeriod.EndDate}'");
                if (!schedules.Any())
                {
                    Guard.Against.FapBusiness("未找到当月排班，请设置排班");
                }
                //排班员工
                var scheduleEmployees = _dbContext.QueryWhere<TmScheduleEmployee>($"{nameof(TmScheduleEmployee.ScheduleUid)} in @ScheduleUids", new DynamicParameters(new { ScheduleUids = schedules.Select(s => s.ScheduleUid) }));
                if (!scheduleEmployees.Any())
                {
                    Guard.Against.FapBusiness("未找到次考勤周期的排班员工");
                }
                _dbContext.InsertBatchSql(InitDayResult());
                IEnumerable<TmDayResult> InitDayResult()
                {
                    foreach (var schedule in schedules)
                    {
                        foreach (var scheduleEmployee in scheduleEmployees)
                        {
                            TmDayResult dayResult = new TmDayResult();
                            dayResult.EmpUid = scheduleEmployee.EmpUid;
                            dayResult.DeptUid = scheduleEmployee.DeptUid;
                            dayResult.ShiftUid = schedule.ShiftUid;
                            dayResult.WorkHoursLength = schedule.WorkHoursLength;
                            dayResult.RestMinutesLength = schedule.RestMinutesLength;
                            dayResult.CurrDate = schedule.WorkDay;
                            dayResult.StartTime = schedule.StartTime;
                            dayResult.EndTime = schedule.EndTime;
                            dayResult.LateTime = schedule.LateTime;
                            dayResult.LeaveTime = schedule.LeaveTime;
                            dayResult.StartCardTime = schedule.StartCardTime;
                            dayResult.EndCardTime = schedule.EndCardTime;
                            dayResult.RestStartTime = schedule.RestStartTime;
                            dayResult.RestEndTime = schedule.RestEndTime;
                            yield return dayResult;
                        }
                    }
                }
            }
        }
        private TmPeriod GetCurrentPeriod()
        {
            //获取当前月考勤周期的排班
            var currPeriod = _dbContext.QueryFirstOrDefaultWhere<TmPeriod>($"{nameof(TmPeriod.IsPeriod)}=1");
            if (currPeriod == null)
            {
                Guard.Against.FapBusiness("无可计算考勤周期，请设置考勤周期[基础设置-考勤周期]");
            }
            return currPeriod;
        }
        /// <summary>
        /// 月考勤结果计算
        /// </summary>
        [Transactional]
        public void MonthResultCalculate()
        {
            var currPeriod = GetCurrentPeriod();
            string startDate = currPeriod.StartDate;
            string endDate = currPeriod.EndDate;
            //此时间段日结果
            var dayResults = _dbContext.QueryWhere<TmDayResult>($"{nameof(TmDayResult.CurrDate)}>=@StartDate and {nameof(TmDayResult.CurrDate)}<=@EndDate",
                new DynamicParameters(new { StartDate = startDate, EndDate = endDate }));
            _dbContext.ExecuteOriginal($"delete from {nameof(TmMonthResult)} where {nameof(TmMonthResult.CurrMonth)}='{currPeriod.CurrMonth}'");
            var empGrp = dayResults.GroupBy(d => d.EmpUid);
            IList<TmMonthResult> monthResults = new List<TmMonthResult>();
            foreach (var empDay in empGrp)
            {
                var firstDayRv = empDay.FirstOrDefault();
                TmMonthResult mr = new TmMonthResult();
                mr.EmpUid = firstDayRv.EmpUid;
                mr.DeptUid = firstDayRv.DeptUid;
                mr.CurrMonth = currPeriod.CurrMonth;
                mr.TravelNum = empDay.Sum(d => d.TravelDays);
                mr.AbsenceNum = empDay.Count(d => d.CalResult.Contains(DayResultEnum.Absence.Description(), StringComparison.OrdinalIgnoreCase));
                mr.AnnualNum = empDay.Where(d => d.LeavelType.EqualsWithIgnoreCase("Annaul")).Sum(d => d.LeaveDays);
                mr.BizLeaveNum = empDay.Where(d => d.LeavelType.EqualsWithIgnoreCase("Business")).Sum(d => d.LeaveDays);
                mr.SickLeaveNum = empDay.Where(d => d.LeavelType.EqualsWithIgnoreCase("Sick")).Sum(d => d.LeaveDays);
                mr.WedLeaveNum = empDay.Where(d => d.LeavelType.EqualsWithIgnoreCase("Wed")).Sum(d => d.LeaveDays);
                mr.MaternityLeaveNum = empDay.Where(d => d.LeavelType.EqualsWithIgnoreCase("Maternity ")).Sum(d => d.LeaveDays);
                mr.FuneralLeaveNum = empDay.Where(d => d.LeavelType.EqualsWithIgnoreCase("Funeral ")).Sum(d => d.LeaveDays);
                mr.LateNum = empDay.Count(d => d.CalResult.Contains(DayResultEnum.ComeLate.Description(), StringComparison.OrdinalIgnoreCase));
                mr.LeftEarlyNum = empDay.Count(d => d.CalResult.Contains(DayResultEnum.LeaveEarly.Description(), StringComparison.OrdinalIgnoreCase));
                mr.WorkDayNum = empDay.Count();
                monthResults.Add(mr);
            }
            _dbContext.InsertBatchSql(monthResults);

            //月结果计算后事件
            string afterSql = _configService.GetSysParamValue("AfterMonthResultCalculate");
            if (afterSql.IsPresent())
            {
                List<string> sqls = afterSql.SplitSemicolon();
                foreach (var sql in sqls)
                {
                    if (sql.IsPresent())
                    {
                        _dbContext.Execute(sql + $" and CurrMonth='{currPeriod.CurrMonth}'");
                    }
                }
            }
        }
        public void ExceptionNotice(string[] options)
        {
            var currPeriod = GetCurrentPeriod();
            string startDate = currPeriod.StartDate;
            string endDate = currPeriod.EndDate;
            //此时间段日结果
            var dayResults = _dbContext.QueryWhere<TmDayResult>($"{nameof(TmDayResult.CurrDate)}>=@StartDate and {nameof(TmDayResult.CurrDate)}<=@EndDate and {nameof(TmDayResult.CalResult)} like '{DayResultEnum.Absence.Description()}%'",
                new DynamicParameters(new { StartDate = startDate, EndDate = endDate }));
            if (!dayResults.Any())
            {
                return;
            }
            IList<FapMail> mailList = new List<FapMail>();
            if (options.Contains("emp", new FapStringEqualityComparer()))
            {
                var employees = _dbContext.QueryWhere<Employee>("Fid in @EmpUids", new DynamicParameters(new { EmpUids = dayResults.Select(d => d.EmpUid).Distinct() }));
                foreach (var empResult in dayResults.GroupBy(d => d.EmpUid))
                {
                    var employee = employees.FirstOrDefault(e => e.Fid == empResult.Key);
                    if (employee == null || employee.Mailbox.IsMissing())
                    {
                        continue;
                    }
                    FapMail mail = new FapMail();
                    mail.Recipient = employee.EmpName;
                    mail.RecipientEmailAddress = $"{employee.EmpName}<{employee.Mailbox}>";
                    mail.Subject = "考勤异常通知";
                    mail.MailContent = $"您有{empResult.Count()}条考勤异常未处理，请抓紧补签。";
                    mail.SendCount = 0;
                    mail.MailCategory = "考勤异常";
                    mailList.Add(mail);
                }
            }
            if (options.Contains("mgr", new FapStringEqualityComparer()))
            {
                var deptList = _dbContext.QueryWhere<OrgDept>("Fid in @DeptUids", new DynamicParameters(new { DeptUids = dayResults.Select(d => d.DeptUid).Distinct() }));
                if (deptList.Any())
                {
                    var employees = _dbContext.QueryWhere<Employee>("Fid in @EmpUids", new DynamicParameters(new { EmpUids = deptList.Select(d => d.DeptManager).Union(deptList.Select(d => d.Director)).Distinct() }));
                    foreach (var deptResult in dayResults.GroupBy(d => d.DeptUid))
                    {
                        var dept = deptList.FirstOrDefault(d => d.Fid == deptResult.Key);
                        if (dept != null && (dept.DeptManager.IsPresent() || dept.Director.IsPresent()))
                        {
                            if (dept.DeptManager.EqualsWithIgnoreCase(dept.Director))
                            {
                                var employee = employees.FirstOrDefault(e => e.Fid == dept.DeptManager);
                                if (employee == null)
                                {
                                    continue;
                                }
                                SendDeptMail(employee, deptResult.Count());
                            }
                            if (dept.DeptManager.IsPresent() && dept.Director.IsPresent() && !dept.DeptManager.EqualsWithIgnoreCase(dept.Director))
                            {
                                string[] empUids = new string[] { dept.DeptManager, dept.Director };
                                foreach (var employee in employees.Where(e => empUids.Contains(e.Fid)))
                                {
                                    SendDeptMail(employee, deptResult.Count());
                                }
                            }
                        }
                    }
                }

            }
            if (mailList.Count > 0)
            {
                _dbContext.InsertBatchSql(mailList);
            }
            void SendDeptMail(Employee employee, int count)
            {
                if (employee.Mailbox.IsMissing())
                {
                    return;
                }
                FapMail mail = new FapMail();
                mail.Recipient = employee.EmpName;
                mail.RecipientEmailAddress = $"{employee.EmpName}<{employee.Mailbox}>";
                mail.Subject = "部门考勤异常通知";
                mail.MailContent = $"您部门还有{count}条考勤异常未处理，请抓紧催促补签。";
                mail.SendCount = 0;
                mail.MailCategory = "考勤异常";
                mailList.Add(mail);
            }
        }
        /// <summary>
        ///请假天数时长
        /// </summary>
        /// <returns></returns>
        public (double, double) LeavelDays(string empUid, string startDateTime, string endDateTime)
        {
            double days = 0.0;
            var scheduleEmployees = _dbContext.QueryWhere<TmScheduleEmployee>("EmpUid=@EmpUid and StartDate<@EndDate and EndDate>@StartDate"
                , new DynamicParameters(new { EmpUid = empUid, EndDate = endDateTime, StartDate = startDateTime }));
            if (!scheduleEmployees.Any())
            {
                Guard.Against.FapBusiness("系统还没有排班，暂时不能请假！");
            }
            var scheEmp = scheduleEmployees.First();
            var schedules = _dbContext.QueryWhere<TmSchedule>("ScheduleUid=@ScheduleUid and EndTime>=@StartTime and StartTime<=@EndTime"
                , new DynamicParameters(new { ScheduleUid = scheEmp.ScheduleUid, StartTime = startDateTime, EndTime = endDateTime }));
            if (!schedules.Any())
            {
                Guard.Against.FapBusiness("选择的时间周期内无排班，不需要请假！");
            }
            var firstSche = schedules.First();
            var lastSche = schedules.Last();
            if (schedules.Count() == 1)
            {
                double totalHours = DateTimeUtils.ToDateTime(endDateTime).Subtract(DateTimeUtils.ToDateTime(startDateTime)).TotalHours;
                if (totalHours >= firstSche.WorkHoursLength)
                {
                    days = 1.0;
                }
                else
                {
                    if (totalHours > firstSche.WorkHoursLength / 2)
                    {
                        //减去休息时长
                        totalHours -= firstSche.RestMinutesLength / 60.0;
                    }
                    days = (totalHours / firstSche.WorkHoursLength > 0.5) ? 1 : 0.5;
                }
            }
            else
            {
                bool firstDiff = false;
                bool lastDiff = false;
                if (DateTimeUtils.ToDateTime(firstSche.StartTime) < DateTimeUtils.ToDateTime(startDateTime))
                {
                    //第一天请假时长
                    double firstHour = DateTimeUtils.ToDateTime(firstSche.EndTime).Subtract(DateTimeUtils.ToDateTime(startDateTime)).TotalHours;
                    if (firstHour > firstSche.WorkHoursLength / 2)
                    {
                        //减去休息时长
                        firstHour -= firstSche.RestMinutesLength / 60.0;
                    }
                    double firstDay = (firstHour / firstSche.WorkHoursLength > 0.5) ? 1 : 0.5;
                    days += firstDay;
                    firstDiff = true;
                }
                if (DateTimeUtils.ToDateTime(endDateTime) < DateTimeUtils.ToDateTime(lastSche.EndTime))
                {
                    //最后一天请假时长
                    double lastHour = DateTimeUtils.ToDateTime(endDateTime).Subtract(DateTimeUtils.ToDateTime(lastSche.StartTime)).TotalHours;
                    if (lastHour > lastSche.WorkHoursLength / 2)
                    {
                        //减去休息时长
                        lastHour -= firstSche.RestMinutesLength / 60.0;
                    }
                    double lastDay = (lastHour / lastSche.WorkHoursLength > 0.5) ? 1 : 0.5;
                    days += lastDay;
                    lastDiff = true;
                }
                if (firstDiff && lastDiff)
                {
                    days += (schedules.Count() - 2);
                }
                else if ((firstDiff && !lastDiff) || (!firstDiff && lastDiff))
                {
                    days += (schedules.Count() - 1);
                }
                else
                {
                    days = schedules.Count();
                }

            }
            return (days, Math.Round(days * firstSche.WorkHoursLength, 2));
        }
        /// <summary>
        /// 获取请假或出差的日结果详情
        /// </summary>
        /// <param name="empUid"></param>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns></returns>
        public IEnumerable<ApplyInfo> LeaveDaysInfo(string empUid, string startDateTime, string endDateTime)
        {
            double days = 0.0;
            var scheduleEmployees = _dbContext.QueryWhere<TmScheduleEmployee>("EmpUid=@EmpUid and StartDate<@EndDate and EndDate>@StartDate"
                , new DynamicParameters(new { EmpUid = empUid, EndDate = endDateTime, StartDate = startDateTime }));
            var scheEmp = scheduleEmployees.First();
            var schedules = _dbContext.QueryWhere<TmSchedule>("ScheduleUid=@ScheduleUid and EndTime>=@StartTime and StartTime<=@EndTime"
                , new DynamicParameters(new { ScheduleUid = scheEmp.ScheduleUid, StartTime = startDateTime, EndTime = endDateTime }));

            if (schedules.Count() == 1)
            {
                var firstSche = schedules.First();
                var lastSche = schedules.Last();
                double totalHours = DateTimeUtils.ToDateTime(endDateTime).Subtract(DateTimeUtils.ToDateTime(startDateTime)).TotalHours;
                if (totalHours >= firstSche.WorkHoursLength)
                {
                    days = 1.0;
                }
                else
                {
                    if (totalHours > firstSche.WorkHoursLength / 2)
                    {
                        //减去休息时长
                        totalHours -= firstSche.RestMinutesLength / 60.0;
                    }
                    days = (totalHours / firstSche.WorkHoursLength > 0.5) ? 1 : 0.5;
                }
                yield return new ApplyInfo { ApplyDate = firstSche.WorkDay, Days = days, Hours = Math.Round(days * firstSche.WorkHoursLength, 2) };
            }
            else
            {
                int first = 0;
                int last = schedules.Count();
                foreach (var schedule in schedules)
                {
                    first++;
                    if (first == 1)
                    {
                        if (DateTimeUtils.ToDateTime(schedule.StartTime) < DateTimeUtils.ToDateTime(startDateTime))
                        {
                            //第一天请假时长
                            double firstHour = DateTimeUtils.ToDateTime(schedule.EndTime).Subtract(DateTimeUtils.ToDateTime(startDateTime)).TotalHours;
                            if (firstHour > schedule.WorkHoursLength / 2)
                            {
                                //减去休息时长
                                firstHour -= schedule.RestMinutesLength / 60.0;
                            }
                            double firstDay = (firstHour / schedule.WorkHoursLength > 0.5) ? 1 : 0.5;

                            yield return new ApplyInfo { ApplyDate = schedule.WorkDay, Days = firstDay, Hours = Math.Round(firstDay * schedule.WorkHoursLength, 2) };
                        }
                        else
                        {
                            yield return new ApplyInfo { ApplyDate = schedule.WorkDay, Days = 1.0, Hours = Math.Round(1.0 * schedule.WorkHoursLength, 2) };
                        }
                    }
                    else if (first == last)
                    {
                        if (DateTimeUtils.ToDateTime(endDateTime) < DateTimeUtils.ToDateTime(schedule.EndTime))
                        {
                            //最后一天请假时长
                            double lastHour = DateTimeUtils.ToDateTime(endDateTime).Subtract(DateTimeUtils.ToDateTime(schedule.StartTime)).TotalHours;
                            if (lastHour > schedule.WorkHoursLength / 2)
                            {
                                //减去休息时长
                                lastHour -= schedule.RestMinutesLength / 60.0;
                            }
                            double lastDay = (lastHour / schedule.WorkHoursLength > 0.5) ? 1 : 0.5;
                            yield return new ApplyInfo { ApplyDate = schedule.WorkDay, Days = lastDay, Hours = Math.Round(lastDay * schedule.WorkHoursLength, 2) };

                        }
                        else
                        {
                            yield return new ApplyInfo { ApplyDate = schedule.WorkDay, Days = 1.0, Hours = Math.Round(1.0 * schedule.WorkHoursLength, 2) };

                        }
                    }
                    else
                    {
                        yield return new ApplyInfo { ApplyDate = schedule.WorkDay, Days = 1.0, Hours = Math.Round(1.0 * schedule.WorkHoursLength, 2) };
                    }
                }
            }
        }

        //public void 

        /// <summary>
        /// 根据获取人员当前有效年假天数
        /// </summary>
        /// <param name="EmpUid"></param>
        /// <returns></returns>
        public double ValidAnnualLeaveDays(string empUid,string startTime)
        {
            int annual = DateTime.Now.Year;
            if (startTime.IsPresent())
            {
                annual = DateTimeUtils.ToDateTime(startTime).Year;
            }
            DynamicParameters param = new DynamicParameters();
            //根据当前列获取值
            param.Add("EmpUid", empUid);
            param.Add("Annual", annual);
            double c = _dbContext.ExecuteScalar<double>("select RemainderNum from  TmAnnualLeave where EmpUid=@EmpUid and Annual=@Annual", param);
            //检查审批中是否有年假
            double cc= _dbContext.ExecuteScalar<double>("select IntervalDay from TmLeaveApply where LeaveType='Annaul' and AppEmpUid=@EmpUid  and BillStatus='PROCESSING'",param);
            return c+cc;

        }
        /// <summary>
        /// 获取有效的加班小时数
        /// </summary>
        /// <param name="empUid"></param>
        /// <returns></returns>
        public double OvertimeValidHours(string empUid)
        {
            var param = new DynamicParameters(new { EmpUid = empUid });
            //审批通过的加班数据
            string sql = "select HoursLength-DaysoffHours from TmOvertimeStat where EmpUid=@EmpUid and Invalid=0";
            double c= _dbContext.ExecuteScalar<double>(sql, param);
            //流程中的调休小时数
            double cc = _dbContext.ExecuteScalar<double>("select IntervalHour from TmLeaveApply where LeaveType='Tuneoff' and AppEmpUid=@EmpUid  and BillStatus='PROCESSING'", param);
            return Math.Round((c + cc),2);
        }


        /// <summary>
        /// 根据假期类型编码获取假期类型
        /// </summary>
        /// <returns></returns>
        public TmLeaveType GetLeaveType(string TypeCode)
        {

            DynamicParameters param = new DynamicParameters();
            param.Add("TypeCode", TypeCode);
            return _dbContext.QueryAll<TmLeaveType>().FirstOrDefault(t => t.TypeCode == TypeCode);
        }
        /// <summary>
        /// 获取人员可调休时长
        /// </summary>
        /// <returns></returns>
        public double GetTuneoffHoursLength(string EmpUid)
        {

            DynamicParameters param = new DynamicParameters();
            param.Add("EmpUid", EmpUid);
            //缺少计算条件
            var hours = _dbContext.ExecuteScalar<double>("select sum(ReHoursLength) from  TmOvertimeStat where EmpUid=@EmpUid and isFalse=0", param);

            return hours;
        }
        /// <summary>
        /// 假期初始化
        /// </summary>
        /// <param name="periodUid">假期区间</param>
        /// <param name="Annual">年度</param>
        [Transactional]
        public void AnnualLeaveInit(string year, string startDate, string endDate)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("Year", year);
            param.Add("StartDate", startDate);
            param.Add("EndDate", endDate);
            int isExist = _dbContext.Count(nameof(TmAnnualLeave), $"{nameof(TmAnnualLeave.Annual)}=@Year", param);
            if (isExist > 0)
            {
                Guard.Against.FapBusiness("已存在该年度年假，不能再生成！");
            }
            //查找年假规则
            var rules = _dbContext.QueryAll<TmAnnualLeaveRule>();
            if (!rules.Any())
            {
                Guard.Against.FapBusiness("没有找到年假生成规则，请在菜单[基础设置-年假规则]中设置年假规则");
            }
            var cols = _dbContext.Columns("Employee");
            IList<TmAnnualLeave> annualLeaveList = new List<TmAnnualLeave>();
            foreach (var rule in rules)
            {
                if (rule.EmpConditionDesc.IsMissing())
                {
                    continue;
                }
                string sql = $"select {rule.Days} Days,Fid,DeptUid from Employee where {SqlUtils.ParsingSql(cols, rule.EmpConditionDesc, _dbContext.DatabaseDialect)}";
                var emps = _dbContext.Query(sql);
                foreach (var emp in emps)
                {
                    TmAnnualLeave annualLeave = new TmAnnualLeave()
                    {
                        Annual = year,
                        EmpUid = emp.Fid,
                        DeptUid = emp.DeptUid,
                        StartDate = startDate,
                        EndDate = endDate,
                        CurrYearNum = emp.Days,
                        LastYearLeft = 0,
                        CurrRealNum = emp.Days,
                        UsedNum = 0,
                        RemainderNum = emp.Days,
                        IsHandle = 0
                    };
                    annualLeaveList.Add(annualLeave);
                }
            }
            _dbContext.InsertBatchSql(annualLeaveList);

        }
        /// <summary>
        /// 上年结余假期
        /// </summary>
        /// <param name="year"></param>
        /// <param name="lastYear">上一年</param>
        [Transactional]
        public void AnnualLeaveSurplus(string year, string lastYear)
        {
            DynamicParameters parameters = new DynamicParameters(new { CurrAnnual = year, PreAnnual = lastYear });
            //求结余天数
            string sql = "update a set a.LastYearLeft=b.RemainderNum, a.IsHandle=1  from TmAnnualLeave as a,TmAnnualLeave as b where a.EmpUid=b.EmpUid and a.Annual=@CurrAnnual and a.IsHandle=0 and b.Annual=@PreAnnual";
            //此规则的sql 无法解析。所以用原始执行sql，需要的话手动添加有效条件
            _dbContext.ExecuteOriginal(sql, parameters);

            //求本年实际和剩余天数
            sql = "update TmAnnualLeave set RemainderNum=CurrYearNum+LastYearLeft,CurrRealNum=CurrYearNum+LastYearLeft,IsHandle=1 where Annual=@CurrAnnual";
            _dbContext.Execute(sql, parameters);
        }
       
        /// <summary>
        /// 补签打卡
        /// </summary>
        /// <param name="empUid">人员Fid</param>
        /// <param name="empCode">人员Code</param>
        /// <param name="startTime">打卡开始时间</param>
        /// <param name="endTime">打卡结束时间</param>
        [Transactional]
        public void ReCard(string empUid, string startDate, string endDate)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("StartDate", startDate);
            param.Add("EndDate", endDate);
            param.Add("EmpUid", empUid);
            string sql = "select * from TmScheduleEmployee where EmpUid =@EmpUid and EndDate>@StartDate and StartDate<@EndDate";
            var scheduleEmployes = _dbContext.Query<TmScheduleEmployee>(sql, param);
            var scheduleUids = scheduleEmployes.Select(s => s.ScheduleUid).Distinct();
            if (!scheduleUids.Any())
            {
                return;
            }
            param.Add("ScheduleUids", scheduleUids);
            string sqlWhere = "WorkDay>=@StartDate and WorkDay<=@EndDate and ScheduleUid in @ScheduleUids";
            var schedules = _dbContext.QueryWhere<TmSchedule>(sqlWhere, param).GroupBy(s => s.ScheduleUid);
            ReCard(scheduleEmployes, schedules,"个人补签");

        }
        /// <summary>
        ///  批量补签打卡
        /// </summary>
        /// <param name="recard"></param>
        [Transactional]
        public void BatchPatchCard(IList<string> deptUids, string startDate, string endDate)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("StartDate", startDate);
            param.Add("EndDate", endDate);
            param.Add("DeptUids", deptUids);
            string sql = "select * from TmScheduleEmployee where DeptUid in @DeptUids and EndDate>@StartDate and StartDate<@EndDate";
            var scheduleEmployes = _dbContext.Query<TmScheduleEmployee>(sql, param);
            var scheduleUids = scheduleEmployes.Select(s => s.ScheduleUid).Distinct();
            if (!scheduleUids.Any())
            {
                throw new FapException("未找到排班信息");
            }
            param.Add("ScheduleUids", scheduleUids);
            string sqlWhere = "WorkDay>=@StartDate and WorkDay<=@EndDate and ScheduleUid in @ScheduleUids";
            var schedules = _dbContext.QueryWhere<TmSchedule>(sqlWhere, param).GroupBy(s => s.ScheduleUid);
            ReCard(scheduleEmployes, schedules, "批量补签");

        }

        private void ReCard(IEnumerable<TmScheduleEmployee> scheduleEmployes, IEnumerable<IGrouping<string, TmSchedule>> schedules,string desc)
        {
            IList<TmCardRecord> cardList = new List<TmCardRecord>();
            foreach (var schedule in schedules)
            {
                foreach (var emp in scheduleEmployes.Where(s => s.ScheduleUid == schedule.Key))
                {
                    foreach (var sch in schedule)
                    {
                        TmCardRecord cdr = new TmCardRecord();
                        cdr.CardTime = sch.StartTime;
                        cdr.EmpUid = emp.EmpUid;
                        cdr.DeptUid = emp.DeptUid;
                        cdr.DeviceName = desc;
                        cdr.DeviceNumber = "-";
                        cdr.IpAddress = "0.0.0.0";
                        cardList.Add(cdr);
                        TmCardRecord cdr1 = new TmCardRecord();
                        cdr1.CardTime = sch.EndTime;
                        cdr1.EmpUid = emp.EmpUid;
                        cdr1.DeptUid = emp.DeptUid;
                        cdr1.DeviceName = desc;
                        cdr1.DeviceNumber = "-";
                        cdr1.IpAddress = "0.0.0.0";
                        cardList.Add(cdr1);
                    }
                }
            }
            _dbContext.InsertBatchSql(cardList);
        }
    }
}
