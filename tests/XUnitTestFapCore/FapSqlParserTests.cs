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
        [Fact]
        public void TestJoinSql()
        {
            string sql = "select employee.EmpName,Employee.DeptUid,Fapuser.UserCode from employee,Fapuser where employee.Fid=FapUser.UserIdentity";
            FapSqlParser parser = new FapSqlParser(_appDomain, sql,true);
            string rv = parser.ParserSqlStatement();
            Assert.True(true);
        }
        [Fact]
        public void TestPageing()
        {
            string sql = "SELECT *, ROW_NUMBER() OVER( ORDER BY Id asc) AS RowNumber FROM Employee";
            FapSqlParser parser = new FapSqlParser(_appDomain, sql, true);
            string rv = parser.ParserSqlStatement();
            Assert.True(true);
        }

        [Fact]
        public void TestMysqlPageing()
        {
            string sql = "SELECT * FROM Employee where Id=1 order by id desc limit 1,10";
            FapSqlParser parser = new FapSqlParser(_appDomain, sql, true);
            string rv = parser.ParserSqlStatement();
            Assert.True(true);
        }
        [Fact]
        public void TestDDL()
        {
            string sql = "delete from  Employee";
            FapSqlParser parser = new FapSqlParser(_appDomain, sql, true);
            string rv= parser.ParserSqlStatement();
            Assert.True(rv.Length > 0);
        }
        [Fact]
        public void TestCount()
        {
            //string sql = "select count(1) from Employee";
            string sql = "select count(1) from fapuser where UserName=@UserName and Fid!='3534239123093061632'";
            FapSqlParser parser = new FapSqlParser(_appDomain, sql, true);
            string rv = parser.ParserSqlStatement();
            Assert.True(rv.Length > 0);
        }
    }
}
