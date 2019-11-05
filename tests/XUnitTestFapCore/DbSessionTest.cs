using Fap.Core.DataAccess;
using Fap.Hcm.Web;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Fap.Core.Exceptions;

namespace XUnitTestFapCore
{
    public class DbSessionTest: IClassFixture<TestWebApplicationFactory<Startup>>
    {
        private readonly IDbSession _dbSession;
        public DbSessionTest(TestWebApplicationFactory<Startup> factory)
        {
            _dbSession = factory.Services.GetService<IDbSession>();
        }

        [Fact]
        public void QueryFirstOrDefault()
        {

           var s=  _dbSession.QueryFirstOrDefault("select * from FapUser where 1=2");

            Assert.Null(s);
        }
        [Fact]
        public void QueryFirst()
        {

            var s = _dbSession.QueryFirst("select * from FapUser where 1=2");

            Assert.Throws<FapException>(()=>
            {

            });
        }
        [Fact]
        public void QuerySingle()
        {

            var s = _dbSession.QuerySingle("select * from FapUser where 1=2");

            Assert.Null(s);
        }
    }
}
