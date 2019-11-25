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
            string rvsql = rv.Substring(rv.LastIndexOf("where".ToUpper()));
            string sqlw = sql.Substring(sql.LastIndexOf("where"));
            Assert.Equal(sqlw.ToUpper(), rvsql.ToUpper());
        }
        [Fact]
        public void TestSelectInParams()
        {
            string sql = "select * from FapUser where Id in @Ins";
            string sql1 = "select *from FapUser where id in (1,2,3)";
            string sql2 = "select *from Fapuser where id in(select 2)";
            string sql3 = "select * from Fapuser where id in(select * from emp)";
            FapSqlParser parser = new FapSqlParser(_appDomain,sql);
            string rv= parser.ParserSqlStatement();
            FapSqlParser parser1 = new FapSqlParser(_appDomain, sql1);
            string rv1 = parser1.ParserSqlStatement();
            FapSqlParser parser2 = new FapSqlParser(_appDomain, sql2);
            string rv2 = parser2.ParserSqlStatement();
            FapSqlParser parser3 = new FapSqlParser(_appDomain, sql3);
            string rv3 = parser3.ParserSqlStatement();
            Assert.Equal(sql.Length, rv.Length);
        }
    }
}
