using Fap.Core.DataAccess;
using Fap.Hcm.Web;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.DataAccess.SqlParser;

namespace XUnitTestFapCore
{
    public class FapSqlParserTests : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        private readonly IDbContext _dbContext;
        private readonly IFapPlatformDomain _appDomain;
        public FapSqlParserTests(TestWebApplicationFactory<Startup> factory)
        {
            _dbContext = factory.Services.GetService<IDbContext>();
            _appDomain = factory.Services.GetService<IFapPlatformDomain>();
        }
        [Fact]
        public void TestSelect()
        {
            string sql = "select * from FapUser where Id=1";
            FapSqlParser parser = new FapSqlParser(_appDomain,sql,true);
            string rv= parser.ParserSelectSqlNoWhere();
           
            Assert.Equal("", rv);
        }
    }
}
