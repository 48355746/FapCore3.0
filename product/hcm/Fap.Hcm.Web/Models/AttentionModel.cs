using Fap.Core.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fap.Hcm.Web.Models
{
    public class AttentionModel
    {
        /// <summary>
        /// 代理任务
        /// </summary>
        public int AgentCount { get; set; }
        /// <summary>
        /// 待办任务
        /// </summary>
        public int TodoCount { get; set; }
        /// <summary>
        /// 消息通知
        /// </summary>
        public IEnumerable<FapMessage> Messages { get; set; }

        public IEnumerable<Notifications> Notifications { get; set; }

    }
    public class Notifications
    {
        public string Icon { get; set; }
        public string Description { get; set; }
        public int NCount { get; set; }
        public string NavigateUrl { get; set; }
    }
}
