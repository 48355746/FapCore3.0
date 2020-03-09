using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Extensions;
using Fap.Core.Infrastructure.Model;
using Polly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Tracker
{
    [Service]
    public class EventDataHandler : IEventDataHandler
    {
        private readonly IDbContext _dbContext;
        private readonly IHttpClientFactory _httpClientFactory;
        public EventDataHandler(IDbContext dbContext, IHttpClientFactory httpClientFactory)
        {
            _dbContext = dbContext;
            _httpClientFactory = httpClientFactory;
        }
        /// <summary>
        /// 保存事件数据
        /// </summary>
        /// <param name="eventData"></param>
        public void SaveEventData(EventData eventData)
        {
            FapRealtimeSynLog synLog = new FapRealtimeSynLog();
            synLog.SynType = eventData.EntityName;
            synLog.SynOper = eventData.ChangeDataType.ToString();
            synLog.SynData = eventData.ToJson(false);
            synLog.SynState = 0;
            //synLog.RemoteUrl = set.RemoteUrl;
            //synLog.RemoteSystem = set.RemoteSystem;

            synLog.Id = _dbContext.Insert<FapRealtimeSynLog>(synLog);
        }
        public async Task<SynchResult> PostEventData(string uri, string jsonData)
        {
            SynchResult synchResult = new SynchResult();
            var client = _httpClientFactory.CreateClient("Retry");
            var buffer = Encoding.UTF8.GetBytes(jsonData);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(uri, byteContent).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                synchResult.Success = true;
            }
            var context = response.RequestMessage?.GetPolicyExecutionContext(); // (if not already held in a local variable)
            if (context != null && context.TryGetValue("RetriesInvoked", out object retriesNeeded))
            {
                if (retriesNeeded.ToInt() == 3)
                {
                    synchResult.Success = false;
                    string result = await response.Content.ReadAsStringAsync();
                    synchResult.ErrMsg = $"GetAsync End, url:{uri}, HttpStatusCode:{response.StatusCode}, result:{result}";
                }
            }
            return synchResult;
        }
    }
}
