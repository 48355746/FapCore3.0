using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace Fap.AspNetCore.MvcResult
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class FormatJsonResult :ActionResult
    {
        /// <summary>
        /// 
        /// </summary>
        public Object Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public FormatJsonResult()
        {
        }
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void ExecuteResult(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            HttpResponse response = context.HttpContext.Response;
            response.ContentType = "application/json";

            var sw = new StringWriter();
            var settings = new JsonSerializerSettings
            {
                Converters = new JsonConverter[] { 
                    new IsoDateTimeConverter() },
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Include
            };
            JsonSerializer serializer = JsonSerializer.Create(settings);

            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                serializer.Serialize(jsonWriter, Data);
            }
            JObject jResult= JObject.Parse(sw.ToString());
            response.WriteAsync(jResult.GetValue("msg").ToString());
        }
    }
}
