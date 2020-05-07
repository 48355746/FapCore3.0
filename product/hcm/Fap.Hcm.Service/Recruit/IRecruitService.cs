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
        void ReceiveResume(IEnumerable<RcrtMail> mails);
        /// <summary>
        /// 简历推送给部门
        /// </summary>
        /// <param name="resumeUids"></param>
        /// <param name="demandUid"></param>
        void SendResumeToDept(List<string> resumeUids, string demandUid);
        /// <summary>
        /// 面试邀约
        /// </summary>
        /// <param name="interviewNotice"></param>
        void InterviewNotice(InterviewNoticeViewModel interviewNotice);
        /// <summary>
        /// 发送Offer
        /// </summary>
        /// <param name="offerNotice"></param>
        void OfferNotice(OfferNoticeViewModel offerNotice);
    }
}
