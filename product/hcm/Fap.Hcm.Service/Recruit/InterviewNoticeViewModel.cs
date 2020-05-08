using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Fap.Hcm.Service.Recruit
{
    public class InterviewNoticeViewModel
    {
        public string DemandUid { get; set; }
        public string ResumeUid { get; set; }
        public string Rounds { get; set; }
        public string IvTime { get; set; }
        public string Locations { get; set; }
        public string MailBox { get; set; }
        public string MailContent { get; set; }
    }
    public class OfferNoticeViewModel
    {
        [Required]
        public string OfferUid { get; set; }
        [Required]
        public string MailBox { get; set; }
        [Required]
        public string MailContent { get; set; }
    }
}
