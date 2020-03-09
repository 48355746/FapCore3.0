
namespace Fap.Core.Scheduler
{
    /// <summary>
    /// 执行间隔
    /// 用于判断作业间隔多长时间执行
    /// </summary>
    public enum IntervalMethodEnum
    {
        Second = 1,
        Minute = 2,
        Hour = 3,
        Day = 4,
        Week = 5,
        Month = 6,
        Year = 7
    }

}
