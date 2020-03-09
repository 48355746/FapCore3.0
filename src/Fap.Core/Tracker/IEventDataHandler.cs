using Fap.Core.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Tracker
{
    public interface IEventDataHandler
    {
        void SaveEventData(EventData eventData);
        Task<SynchResult> PostEventData(string uri, string jsonData);
    }
}
