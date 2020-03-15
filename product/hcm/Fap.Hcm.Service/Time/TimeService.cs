using Dapper;
using Fap.AspNetCore.ViewModel;
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
using Fap.Core.Infrastructure.Metadata;

namespace Fap.Hcm.Service.Time
{
    /// <summary>
    /// 时间管理服务类
    /// </summary>
    [Service]
    public class TimeService : ITimeService
    {
        private readonly IDbContext _dataAccessor;
        private readonly IFapApplicationContext _applicationContext;
        public TimeService(IDbContext dataAccessor, IFapApplicationContext applicationContext)
        {
            _dataAccessor = dataAccessor;
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
            _dataAccessor.DeleteExec("TmHoliday", "CaseUid=@CaseUid", param);
            if (list.Any())
            {
                _dataAccessor.InsertBatch<TmHoliday>(list);
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
        public ResponseViewModel InitSchedule(string empWhere, string shiftUid, string holidayUid, string startDate, string endDate)
        {
            //获取排班的员工
            var ecount = _dataAccessor.Count<Employee>(empWhere);
            if (ecount < 1)
            {
                return ResponseViewModelUtils.Failure("您设置的条件找不到员工");
            }
            string where = string.Empty;
            if (empWhere.IsPresent())
            {
                where = " where " + empWhere;
            }
            //查询已经排过班的员工
            string vstartDate = startDate + " 00:00:00";
            string vendDate = endDate + " 12:59:59";

            //对于之前排过班又包括的 先删除
            string schwhere = $"ShiftUid=@ShiftUid and EmpUid in (select fid from Employee {where}) and WorkDay>=@StartDate and WorkDay<=@EndDate";
            _dataAccessor.DeleteExec("TmSchedule", schwhere, new DynamicParameters(new { ShiftUid = shiftUid, StartDate = vstartDate, EndDate = vendDate }));


            //获取休息日
            DynamicParameters paramHolidy = new DynamicParameters();
            paramHolidy.Add("CaseUid", holidayUid);
            var listHolidys = _dataAccessor.QueryWhere<TmHoliday>("CaseUid=@CaseUid", paramHolidy);
            if (!listHolidys.Any())
            {
                return ResponseViewModelUtils.Failure("你还没有设置休息日，请先设置休息日再生成排班");
            }
            DynamicParameters paramShift = new DynamicParameters();
            paramShift.Add("Fid", shiftUid);
            var Shift = _dataAccessor.Get<TmShift>(shiftUid);
            if (Shift != null)
            {
                string currDate = DateTimeUtils.CurrentDateTimeStr;
                var TimeS = Convert.ToDateTime(endDate) - Convert.ToDateTime(startDate);
                var Days = Convert.ToInt32(TimeS.TotalDays);
                DateTime wDate = Convert.ToDateTime(startDate);
                string batchSql = "";
                for (int i = 0; i < Days; i++)
                {
                    string wd = wDate.AddDays(i).ToString("yyyy-MM-dd");
                    if (listHolidys.Any(w => w.Holiday == wd))
                    {
                        continue;
                    }
                    //打卡开始时间
                    string st = wd + " " + Shift.StartTime;
                    //迟到时间
                    string cd = Convert.ToDateTime(st).AddMinutes(Convert.ToDouble(Shift.ComeLate)).ToString("yyyy-MM-dd HH:mm:00");
                    //下班时间=上班时间+工作时长+休息时长
                    string ed = Convert.ToDateTime(st).AddHours(Convert.ToDouble(Shift.WorkHoursLength)).AddMinutes(Convert.ToDouble(Shift.RestMinutesLength)).ToString("yyyy-MM-dd HH:mm:00");
                    //排班休息开始时间
                    string rs = wd + " " + Shift.RestStartTime;
                    //休息结束时间
                    string re = Convert.ToDateTime(rs).AddMinutes(Convert.ToDouble(Shift.RestMinutesLength)).ToString("yyyy-MM-dd HH:mm:00");
                    if (Convert.ToDateTime(st) > Convert.ToDateTime(rs)) //如果排班跨天
                    {
                        //排班休息开始时间
                        rs = Convert.ToDateTime(wd).AddDays(1).ToString("yyyy-MM-dd") + " " + Shift.RestStartTime;
                        //休息结束时间
                        re = Convert.ToDateTime(rs).AddMinutes(Convert.ToDouble(Shift.RestMinutesLength)).ToString("yyyy-MM-dd HH:mm:00");
                    }
                    //早退时间
                    string zt = Convert.ToDateTime(ed).AddMinutes(Convert.ToDouble(-Shift.LeftEarly)).ToString("yyyy-MM-dd HH:mm:00");
                    //早打卡开始时间
                    string startcard = Convert.ToDateTime(st).AddHours(Convert.ToDouble(-Shift.EarlyCard)).ToString("yyyy-MM-dd HH:mm:00");
                    //最晚打卡时间
                    string endcard = Convert.ToDateTime(ed).AddHours(Convert.ToDouble(Shift.LateCard)).ToString("yyyy-MM-dd HH:mm:00");

                    batchSql = batchSql + "\r\n" + string.Format("insert into TmSchedule(Fid,EmpUid,ShiftUid,StartTime,EndTime,LateTime,LeaveTime,WorkDay,StartCardTime,EndCardTime,WorkHoursLength,RestMinutesLength,RestStartTime,RestEndTime,EnableDate,DisableDate,Dr,Ts,CreateBy,CreateName,CreateDate) select dbo.GetFid(),Fid, '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8},{9},'{10}','{11}','{12}','9999-12-31 23:59:59',0,dbo.GetTs(),'{13}','{14}','{15}' from Employee  {16}", shiftUid, st, ed, cd, zt, wd, startcard, endcard, Shift.WorkHoursLength, Shift.RestMinutesLength, rs, re, currDate, _applicationContext.EmpUid, _applicationContext.EmpName, currDate, empWhere);

                }
                _dataAccessor.Execute(batchSql);

            }

            return ResponseViewModelUtils.Sueecss();

        }

        /// <summary>
        /// 根据时间段获取员工在当前时间段里的工作天数
        /// </summary>
        /// <returns></returns>
        public double GetEmpWorkDaysByTime(string EmpUid, string StartDate, string EndDate, string LeaveType, out string msg, out double workhours, string Billcode = null)
        {
            msg = "";
            workhours = 0;
            var vStartDate = Convert.ToDateTime(StartDate);
            var vEndDate = Convert.ToDateTime(EndDate);

            DynamicParameters paramWork = new DynamicParameters();
            paramWork.Add("EmpUid", EmpUid);
            paramWork.Add("vStartDate", DateTimeUtils.DateTimeFormat(vStartDate));
            paramWork.Add("vEndDate", DateTimeUtils.DateTimeFormat(vEndDate));
            //获取该时间段是否有休假
            var tm = _dataAccessor.Count("TmLeaveApply", "AppEmpUid=@EmpUid and ((StartTime<=@vStartDate and EndTime>@vStartDate) or (StartTime<@vEndDate and EndTime>=@vEndDate) or (StartTime>=@vStartDate and EndTime<=@vEndDate)) and (BillStatus='PROCESSING' or BillStatus='PASSED' or BillStatus='SUSPENDED')", paramWork);
            if (tm > 0)
            {
                msg = "当前时间段内已存在请假申请！";
                return 0;
            }
            //获取当前时间段是否有出差
            var cc = _dataAccessor.Count("TmTravelApply", "AppEmpUid=@EmpUid and ((StartTime<=@vStartDate and EndTime>@vStartDate) or (StartTime<@vEndDate and EndTime>=@vEndDate) or (StartTime>=@vStartDate and EndTime<=@vEndDate)) and (BillStatus='PROCESSING' or BillStatus='PASSED' or BillStatus='SUSPENDED')", paramWork);
            if (cc > 0)
            {
                msg = "当前时间段内已存在出差申请！";
                return 0;
            }
            paramWork.Add("StartDate", vStartDate.ToString("yyyy-MM-dd"));
            paramWork.Add("EndDate", vEndDate.ToString("yyyy-MM-dd"));
            //获取当前时间段内排班天数
            double workDay = (double)_dataAccessor.Count("TmSchedule", "EmpUid=@EmpUid and WorkDay>=@StartDate and WorkDay<=@EndDate", paramWork);
            if (workDay <= 0)
            {
                msg = "您当前时段内未找到排班信息！";
                return 0;
            }
            workhours = (double)_dataAccessor.Sum("TmSchedule", "workhourslength", "EmpUid=@EmpUid and WorkDay>=@StartDate and WorkDay<=@EndDate", paramWork);
            //获取请假开始当天的排班
            var startSchedule = _dataAccessor.QueryFirstOrDefaultWhere<TmSchedule>("WorkDay=@StartDate", paramWork);
            if (startSchedule != null)
            {
                //获取开始请假当天的请假时长

                var startHours = (Convert.ToDateTime(startSchedule.EndTime) - vStartDate).TotalHours;

                //当天请假时长大于1小于排班数一半 按0.5天计算  超过请假时长一半按1天计算 小于1小时的不算当天请假
                if (1 <= startHours && startHours <= (startSchedule.WorkHoursLength / 2))
                {
                    workDay -= 0.5D;
                }
                else if (startHours < 1)
                {
                    workDay -= 1;
                }
                workhours = workhours - startSchedule.WorkHoursLength + startHours;
            }
            //获取请假结束当天的排班
            var endSchedule = _dataAccessor.QueryFirstOrDefaultWhere<TmSchedule>("WorkDay=@EndDate", paramWork);
            if (endSchedule != null)
            {
                //获取结束请假当天的请假时长
                var endHours = (vEndDate - Convert.ToDateTime(endSchedule.StartTime)).TotalHours;
                if (1 <= endHours && endHours <= (endSchedule.WorkHoursLength / 2))
                {
                    workDay -= 0.5;
                }
                else if (endHours < 1)
                {
                    workDay -= 1;
                }
                workhours = workhours - endSchedule.WorkHoursLength + endHours;
            }
            return workDay;
        }

        /// <summary>
        /// 判断某人在某个时间段内是否有排班
        /// </summary>
        /// <returns></returns>
        public bool ExistEmpWorkDaysByTime(string EmpUid, string StartDate, string EndDate)
        {
            var vStartDate = Convert.ToDateTime(StartDate);
            var vEndDate = Convert.ToDateTime(EndDate);

            DynamicParameters paramWork = new DynamicParameters();
            paramWork.Add("EmpUid", EmpUid);
            paramWork.Add("vStartDate", vStartDate.ToString("yyyy-MM-dd HH:mm"));
            paramWork.Add("vEndDate", vEndDate.ToString("yyyy-MM-dd HH:mm"));
            var tm = _dataAccessor.Count("TmSchedule", "EmpUid=@EmpUid and ((StartTime<=@vStartDate and EndTime>@vStartDate) or (StartTime<@vEndDate and EndTime>=@vEndDate) or (StartTime>=@vStartDate and EndTime<=@vEndDate))", paramWork);
            if (tm > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 根据获取人员当前年假天数
        /// </summary>
        /// <param name="EmpUid"></param>
        /// <returns></returns>
        public double GetAnnaulRemainderNum(string EmpUid)
        {

            //string sql = string.Format("select RemainderNum from TmAnnualLeave where EmpUid=@EmpUid and Annual=@Annual");
            DynamicParameters param = new DynamicParameters();
            //根据当前列获取值
            param.Add("EmpUid", EmpUid);
            param.Add("Annual", DateTime.Now.Year);
            double c = _dataAccessor.ExecuteScalar<double>("select RemainderNum from  TmAnnualLeave where EmpUid=@EmpUid and Annual=@Annual", param);

            return c;

        }



        /// <summary>
        /// 根据假期类型编码获取假期类型
        /// </summary>
        /// <returns></returns>
        public TmLeaveType GetLeaveType(string TypeCode)
        {

            DynamicParameters param = new DynamicParameters();
            param.Add("TypeCode", TypeCode);
            return _dataAccessor.QueryAll<TmLeaveType>().FirstOrDefault(t => t.TypeCode == TypeCode);
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
            var hours = _dataAccessor.ExecuteScalar<double>("select sum(ReHoursLength) from  TmOvertimeStat where EmpUid=@EmpUid and isFalse=0", param);

            return hours;
        }
        /// <summary>
        /// 假期初始化
        /// </summary>
        /// <param name="periodUid">假期区间</param>
        /// <param name="Annual">年度</param>
        [Transactional]
        public void InitLeaveData(string periodUid)
        {
            //string currDate = DateTimeUtils.CurrentDateTimeStr;
            //string empuid = _applicationContext.EmpUid;
            //string empName = _applicationContext.EmpName;

            ////先删除本区间
            //_dataAccessor.DeleteExec("TmAnnualLeave", "PeriodUid=@Period", new DynamicParameters(new { Period = periodUid }));
            //dynamic entity = _dataAccessor.Get("TmleavePeriod", periodUid, false);
            //string sql = "insert into TmAnnualLeave(Fid,EmpUid,EmpCode,EmpCategory,PeriodUid,Annual,EnableDate,DisableDate,Dr,Ts,CreateBy,CreateName,CreateDate) select dbo.GetFid(),Fid,EmpCode,EmpCategory,@Period,@Annual,@CurrDate,'9999-12-31 12:59:59',0,dbo.GetTs(),@CurrEmp,@CurrEmpName,@CurrDate from employee where DisableDate>getdate() and EnableDate<=getdate() and dr=0 and EmpStatus='Current' and IsMainJob=1";
            //_dataAccessor.Execute(sql, new DynamicParameters(new { Period = periodUid, Annual = entity.Annual, CurrDate = currDate, CurrEmp = empuid, CurrEmpName = empName }));
            //DynamicParameters param = new DynamicParameters();
            //param.Add("Period", periodUid);
            //IEnumerable<dynamic> list = _dataAccessor.Query("select * from TmLeaveInitRule where PeriodUid=@Period and IsEnabled=1", param, false);
            //if (list != null && list.Any())
            //{
            //    foreach (var item in list)
            //    {
            //        var days = item.DayNum;
            //        var filter = item.RuleSetting;
            //        if (filter != "")
            //        {
            //            JsonFilterToSql jfts = new JsonFilterToSql(_dataAccessor);
            //            string where = jfts.BuilderFilter("Employee", filter);
            //            string updateSql = $"update TmAnnualLeave set CurrYearNum={days} where PeriodUid=@Period and EmpUid in(select fid from Employee where  {where})";
            //            _dataAccessor.Execute(updateSql, new DynamicParameters(new { Period = periodUid }), session);
            //        }
            //    }
            //}

        }
        /// <summary>
        /// 上年结余假期
        /// </summary>
        /// <param name="periodUid"></param>
        [Transactional]
        public void BalanceLeaveData(string periodUid)
        {
            dynamic entity = _dataAccessor.Get("TmleavePeriod", periodUid, false);
            int annual = Convert.ToInt32(entity.Annual);//当前期间年
            int preAnnual = annual - 1;//上一年
            DynamicParameters parameters = new DynamicParameters(new { CurrAnnual = annual.ToString(), PreAnnual = preAnnual.ToString() });
            //求结余天数
            string sql = "update a set a.LastYearLeft=b.RemainderNum, a.IsHandle=1  from TmAnnualLeave a,TmAnnualLeave b where a.EmpUid=b.EmpUid and a.Annual=@CurrAnnual and a.IsHandle=0 and b.Annual=@PreAnnual";
            //此规则的sql 无法解析。所以用原始执行sql，需要的话手动添加有效条件
            _dataAccessor.Execute(sql, parameters);

            //求本年实际和剩余天数
            sql = "update TmAnnualLeave set RemainderNum=CurrYearNum+LastYearLeft,CurrRealNum=CurrYearNum+LastYearLeft,IsHandle=1 where Annual=@CurrAnnual ";
            _dataAccessor.Execute(sql, parameters);


        }
        /// <summary>
        /// 生成日结果补签数据
        /// </summary>
        /// <param name="fid">考勤周期Fid</param>
        /// <param name="employee">当前操作人</param>
        [Transactional]
        public void BuilderDayResultBq(string fid)
        {

            DynamicParameters param = new DynamicParameters();
            param.Add("Fid", fid);
            var tmmonth = _dataAccessor.QueryFirstOrDefault("select Code,StartDate,EndDate from  TmPeriod where Fid=@Fid", param);

            _dataAccessor.Execute(@"merge into TmReplenDay A
                                   using  (select * from  TmDayResult where dr=0 and CurrDate>=@StartDate and CurrDate<=@EndDate and (CalResult like '%旷工%' or CalResult like '%缺勤%' or CalResult like '%迟%' or CalResult like '%退%')) B on (A.EmpUid=B.EmpUid and A.CurrDate=B.CurrDate)
                                   when not matched then
                                   insert (Fid,EmpUid,TmMonthCode,StartTime,EndTime,CurrDate,ReCalResult,EnableDate,DisableDate,Ts,Dr,CreateBy,CreateName,CreateDate) values(dbo.GetFid(),B.EmpUid,@Code,B.CardStartTime,B.CardEndTime,B.CurrDate,B.CalResult,CONVERT(varchar(100), GETDATE(), 20),'9999-12-31 23:59:59',dbo.GetTs(),0,@UserFid,@UserName,CONVERT(varchar(100), GETDATE()));
                                ", new DynamicParameters(new { StartDate = tmmonth.StartDate, EndDate = tmmonth.EndDate, Code = tmmonth.Code, UserName = _applicationContext.EmpName, UserFid = _applicationContext.EmpUid }));
            //更新员工编码、邮箱、直属上级字段
            _dataAccessor.Execute($"update TmReplenDay  set EmpCode=Employee.EmpCode,MailBox=Employee.MailBox, LeaderShip=Employee.LeaderShip from Employee where TmReplenDay.EmpUid=Employee.Fid and TmReplenDay.TmMonthCode=@Code ", new DynamicParameters(new { Code = tmmonth.Code }));

            //发消息
            var UserName = _applicationContext.EmpName;
            var UserFid = _applicationContext.EmpUid;
            string baseUrl = _applicationContext.BaseUrl;

            string sql = string.Empty;
            if (_dataAccessor.DatabaseDialect == DatabaseDialectEnum.MSSQL)
            {
                sql = @"insert into FapMail (Fid,Recipient,RecipientEmailAddress,Subject,MailContent,EnableDate,DisableDate,Ts,Dr,CreateBy,CreateName,CreateDate) 
                            select dbo.GetFid(),(select top 1 EmpName from Employee where Fid=TmReplenDay.EmpUid and dr=0),
                            MailBox,
                            '考勤补签',
                            '您本月有'+cast(count(0) as varchar)+'条考勤异常需处理，详情进入系统查看。',
                             CONVERT(varchar(100), GETDATE(), 20),'9999-12-31 23:59:59',dbo.GetTs(),0,@UserFid,@UserName,CONVERT(varchar(100), GETDATE(),20) 
                            from  TmReplenDay where dr=0 and TmMonthCode=@Code group by EmpUid,MailBox;
                            insert into FapMail (Fid,Recipient,RecipientEmailAddress,Subject,MailContent,EnableDate,DisableDate,Ts,Dr,CreateBy,CreateName,CreateDate) 
                            select dbo.GetFid(),(select top 1 EmpName from Employee where Fid=TmReplenDay.LeaderShip and dr=0),
                            (select top 1 Mailbox from Employee where Fid=TmReplenDay.LeaderShip and dr=0),
                            '考勤补签',
                            '您的直属下级本月有'+cast(count(0) as varchar)+'条考勤异常需处理，详情进入系统查看。',
                             CONVERT(varchar(100), GETDATE(), 20),'9999-12-31 23:59:59',dbo.GetTs(),0,@UserFid,@UserName,CONVERT(varchar(100), GETDATE(),20) 
                            from  TmReplenDay where dr=0 and TmMonthCode=@Code and (LeaderShip<>'')  group by LeaderShip;
                            insert into FapMessage (Fid,SEmpUid, REmpUid,SendTime,URL,Title,MsgContent,MsgCategory,EnableDate,DisableDate,Ts,Dr,CreateBy,CreateName,CreateDate) 
                            select dbo.GetFid(),@SEmpUid, EmpUid,@SendTime,@PUrl,
                            '考勤补签',
                            '您本月有'+cast(count(0) as varchar)+'条考勤异常需处理，详情进入系统查看。',
                            'Notice',
                            CONVERT(varchar(100), GETDATE(), 20),'9999-12-31 23:59:59',dbo.GetTs(),0,@UserFid,@UserName,CONVERT(varchar(100), GETDATE(),20)
                            from   TmReplenDay where dr=0 and TmMonthCode=@Code group by EmpUid;
                            insert into FapMessage (Fid,SEmpUid, REmpUid,SendTime,URL,Title,MsgContent,MsgCategory,EnableDate,DisableDate,Ts,Dr,CreateBy,CreateName,CreateDate)
                            select dbo.GetFid(),@SEmpUid,LeaderShip,@SendTime,@MUrl,
                            '考勤补签',
                            '您的直属下级本月有'+cast(count(0) as varchar)+'条考勤异常需处理，详情进入系统查看。',
                            'Notice',
                            CONVERT(varchar(100), GETDATE(), 20),'9999-12-31 23:59:59',dbo.GetTs(),0,@UserFid,@UserName,CONVERT(varchar(100), GETDATE(),20)
                            from   TmReplenDay where dr=0 and TmMonthCode=@Code group by LeaderShip;";

                _dataAccessor.Execute(sql, new DynamicParameters(new { SEmpUid = UserFid, Code = tmmonth.Code, UserName = UserName, UserFid = UserFid, SendTime = DateTimeUtils.CurrentDateTimeStr, PUrl = baseUrl + "/Home/MainFrame#SelfService/Ess/TmMyReplenCard", MUrl = baseUrl + "/Home/MainFrame#Time/Time/TmReplenCard" }));

            }
            else if (_dataAccessor.DatabaseDialect == DatabaseDialectEnum.MYSQL)
            {
                sql = "";
            }


        }
        /// <summary>
        /// 补签打卡
        /// </summary>
        /// <param name="empUid">人员Fid</param>
        /// <param name="empCode">人员Code</param>
        /// <param name="startTime">打卡开始时间</param>
        /// <param name="endTime">打卡结束时间</param>
        [Transactional]
        public void ReCard(string empUid, string empCode, string startTime, string endTime)
        {

            dynamic fdo1 = new FapDynamicObject(_dataAccessor.Columns("TmCardRecord"));
            fdo1.EmpUid = empUid;
            fdo1.CardTime = startTime;
            fdo1.EmpCode = empCode;
            fdo1.DeviceName = "补签数据";

            dynamic fdo2 = new FapDynamicObject(_dataAccessor.Columns("TmCardRecord"));
            fdo2.EmpUid = empUid;
            fdo2.CardTime = endTime;
            fdo2.EmpCode = empCode;
            fdo2.DeviceName = "补签数据";

            _dataAccessor.InsertDynamicData(fdo1);
            _dataAccessor.InsertDynamicData(fdo2);
            string cardDate = startTime.Substring(0, 10);
            _dataAccessor.Execute("update TmReplenDay set ReplenStatus = 1 where EmpUid=@EmpUid and CurrDate=@CurrDate", new DynamicParameters(new { EmpUid = empUid, CurrDate = cardDate }));



        }
        /// <summary>
        ///  补签打卡不经过审批
        /// </summary>
        /// <param name="recard"></param>
        public void ReCardNoApply(ReCard recard)
        {
            //获取排班
            DynamicParameters param = new DynamicParameters();
            param.Add("StartDate", recard.StartDate);
            param.Add("EndDate", recard.EndDate);
            param.Add("DeptUids", recard.DeptUids);
            string sqlWhere = "WorkDay>=@StartDate and WorkDay<=@EndDate and EmpUid in(select Fid from Employee where DeptUid in @DeptUids)";
            IEnumerable<TmSchedule> schedules = _dataAccessor.QueryWhere<TmSchedule>(sqlWhere, param).OrderBy(c => c.WorkDay);
            if (schedules.Any())
            {

                foreach (var sch in schedules)
                {
                    string sql1 = $"insert into TmCardRecord(Fid,EmpUid,EmpCode,CardTime,DeviceName,EnableDate,DisableDate,Dr) select '{ UUIDUtils.Fid}',Fid,EmpCode,'{sch.StartTime}','管理员批量补签','{DateTimeUtils.CurrentDateTimeStr}','9999-12-31 23:59:59',0 from Employee where Fid =@EmpUid";
                    string sql2 = $"insert into TmCardRecord(Fid,EmpUid,EmpCode,CardTime,DeviceName,EnableDate,DisableDate,Dr) select '{ UUIDUtils.Fid}',Fid,EmpCode,'{sch.EndTime}','管理员批量补签','{DateTimeUtils.CurrentDateTimeStr}','9999-12-31 23:59:59',0 from Employee where Fid =@EmpUid";
                    _dataAccessor.Execute(sql1, new DynamicParameters(new { EmpUid = sch.EmpUid }));
                    _dataAccessor.Execute(sql2, new DynamicParameters(new { EmpUid = sch.EmpUid }));
                }

            }
        }
    }
}
