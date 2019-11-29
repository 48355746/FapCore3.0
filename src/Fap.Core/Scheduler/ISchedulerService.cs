using Fap.Model.Infrastructure;
using System.Threading.Tasks;

namespace Fap.Core.Scheduler
{
    public interface ISchedulerService
    {
        Task Run();
        Task AddJob(FapJob fapJob);
        Task PauseJob(FapJob fapJob);
        Task RemoveJob(FapJob fapJob);
        Task ResumeJob(FapJob fapJob);
        Task Shutdown();
        Task StartAllJobs();
    }
}