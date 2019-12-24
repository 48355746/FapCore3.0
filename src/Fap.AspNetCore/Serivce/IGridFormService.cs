using System;
using System.Threading.Tasks;
using Fap.AspNetCore.Model;
using Fap.AspNetCore.ViewModel;
using Fap.Core.Infrastructure.Query;
using Fap.Core.Infrastructure.Metadata;
using Microsoft.AspNetCore.Http;

namespace Fap.AspNetCore.Serivce
{
    public interface IGridFormService
    {
        Task<ResponseViewModel> PersistenceAsync(IFormCollection formCollection);
        ResponseViewModel BatchUpdate(IFormCollection frmCollection);
        JqGridData QueryPageDataResultView(JqGridPostData jqGridPostData);
        ResponseViewModel SaveChange(OperEnum oper, FapDynamicObject mainDataKeyValues, System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<dynamic>> childDataList = null);
        string ExportExcelData(JqGridPostData model);
        string ExportExcelTemplate(QuerySet querySet);
        bool ImportExcelData(string tableName);
    }
}