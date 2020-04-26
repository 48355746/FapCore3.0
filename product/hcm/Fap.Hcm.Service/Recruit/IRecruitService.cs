﻿using Fap.Core.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Recruit
{
    public interface IRecruitService
    {
        void ReceiveResume(string host, int port, string account, string pwd, bool useSSL, MailProtocolEnum protocol);
    }
}
