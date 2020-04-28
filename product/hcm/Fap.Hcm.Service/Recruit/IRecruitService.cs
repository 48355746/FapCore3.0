using Fap.Core.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Recruit
{
    public interface IRecruitService
    {
        void PublishWebsite(string demandUid, string websites);
        void DemandExecStatus(string demandUid, string status);
        void ResumeStatus(List<string> fids, string status);
        void ReceiveResume();
    }
}
