using Fap.Model.Infrastructure;
using System.Threading.Tasks;

namespace Fap.Core.Scheduler
{
    public interface ISchedulerService
    {
        void Run();
        void AddJob(FapJob fapJob);
        void PauseJob(FapJob fapJob);
        void RemoveJob(FapJob fapJob);
        void ResumeJob(FapJob fapJob);
        void ShutdownJobs();
        void StartAllJobs();
    }
}