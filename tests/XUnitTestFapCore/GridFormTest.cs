using Fap.AspNetCore.Serivce;
using Fap.Hcm.Web;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Fap.AspNetCore.ViewModel;

namespace XUnitTestFapCore
{
    public class GridFormTest : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        private readonly IGridFormService _gridFormService;
        public GridFormTest(TestWebApplicationFactory<Startup> factory)
        {
            _gridFormService = factory.Services.GetService<IGridFormService>();
        }
        [Fact]
        public void JqgridQueryOption()
        {
            JqGridPostData jqGridPostData = new JqGridPostData()
            {
                QuerySet = new Fap.Core.Infrastructure.Query.QuerySet { TableName = "Employee", QueryCols = "*" }
                ,Rows=10, Page=2, Sidx="Id", Sord="asc" 
            };
            var rv = _gridFormService.QueryPageDataResultView(jqGridPostData, null);

            Assert.Equal(10, rv.Records);

            
        }
    }
}
