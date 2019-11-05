using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core.Infrastructure.License
{
    //项目名称+申请日期+结束日期+硬盘序列号/CPU序列号+随机数+整个MD5
    public class RequestCodeInfo
    {
        public string ProjectName { get; set; }
        public string RequestDate { get; set; }
        public string EndDate { get; set; }

        public string UniqueId { get; set; }
    }
}
