using System;
using System.Threading.Tasks;
using Fap.AspNetCore.Model;
using Fap.AspNetCore.ViewModel;
using Fap.Core.Infrastructure.Query;
using Fap.Core.Infrastructure.Metadata;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Fap.AspNetCore.Binder;

namespace Fap.AspNetCore.Serivce
{
    public interface IGridFormService
    {
        Task<ResponseViewModel> PersistenceAsync(FormModel formModel);
        PageDataResultView QueryPageDataResultView(JqGridPostData jqGridPostData);
        string ExportExcelData(JqGridPostData model);
        string ExportExcelTemplate(QuerySet querySet);
        bool ImportExcelData(string tableName);
        bool ImportWordTemplate(string tableName);
        string PrintWordTemplate(GridModel gridModel);
    }
}