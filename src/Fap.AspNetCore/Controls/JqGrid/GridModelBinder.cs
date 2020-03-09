using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Fap.AspNetCore.Controls.JqGrid
{
    [ModelBinder(typeof(GridModelBinder))]
    public class GridSettings
    {
        public bool IsSearch { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }
        public Filter Where { get; set; }
    }

    [DataContract]
    public class Filter
    {
        [DataMember]
        public string groupOp { get; set; }
        [DataMember]
        public Rule[] rules { get; set; }

        public static Filter Create(string jsonData)
        {
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(Filter));
                //var ms = new System.IO.MemoryStream(Encoding.Default.GetBytes(jsonData));
                var ms = new System.IO.MemoryStream(
                    Encoding.Convert(
                        Encoding.Default,
                        Encoding.UTF8,
                        Encoding.Default.GetBytes(jsonData)));
                return serializer.ReadObject(ms) as Filter;
            }
            catch
            {
                return null;
            }
        }
    }

    [DataContract]
    public class Rule
    {
        [DataMember]
        public string field { get; set; }
        [DataMember]
        public string op { get; set; }
        [DataMember]
        public string data { get; set; }
    }

    public class GridModelBinder : IModelBinder
    {       

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            try
            {
                var request = bindingContext.HttpContext.Request;
                return Task.FromResult( new GridSettings
                {
                    IsSearch = bool.Parse(request.Form["_search"]),
                    PageIndex = int.Parse(request.Form["page"]),
                    PageSize = int.Parse(request.Form["rows"]),
                    SortColumn = request.Form["sidx"] ,
                    SortOrder = request.Form["sord"],
                    Where = Filter.Create(request.Form["filters"])
                });
            }
            catch
            {
                return null;
            }
        }
    }
}