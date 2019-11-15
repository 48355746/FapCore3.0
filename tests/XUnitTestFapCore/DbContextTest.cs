using Fap.Core.DataAccess;
using Fap.Hcm.Web;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Fap.Core.Exceptions;
using System.Linq;
using Fap.Core.Rbac.Model;
using Fap.Core.Metadata;

namespace XUnitTestFapCore
{
    public class DbContextTest : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        private readonly IDbContext _dbContext;
        public DbContextTest(TestWebApplicationFactory<Startup> factory)
        {
            _dbContext = factory.Services.GetService<IDbContext>();
        }

        [Fact]
        public void Query()
        {
            var c = _dbContext.Count("Employee");
            var s = _dbContext.Query("select * from Employee");
            var ss = _dbContext.QueryAll<Employee>();
            var sss = _dbContext.QueryWhere<Employee>("");
            var ssss = _dbContext.QueryWhere<Employee>("1=2");
            Assert.True(c == ss.Count());
            Assert.True(c == sss.Count());
            Assert.True(c == s.Count());
            Assert.NotNull(ssss);
            Assert.Equal(0,ssss?.Count());
        }
        [Fact]
        public void QueryFirst()
        {
            var s = _dbContext.QueryWhere<FapUser>("UserName=@Hr", new Dapper.DynamicParameters(new { Hr = "hr" }));

            Assert.Equal("hr", s.First().UserName);
        }
        [Fact]
        public void QuerySingleZero()
        {
            var ex = Assert.Throws<FapException>(() =>
            {
                var s = _dbContext.QuerySingle("select * from FapUser where 1=2");
            });
            Assert.Contains("Sequence contains no elements", ex.Message);
        }
        [Fact]
        public void QuerySingleMulti()
        {
            var ex = Assert.Throws<FapException>(() =>
            {
                var s = _dbContext.QuerySingle("select * from FapUser where Id>1");
            });
            Assert.Contains("Sequence contains more than one element", ex.Message);
        }
        [Fact]
        public void QuerySingleOrDefaultZero()
        {
            var s = _dbContext.QuerySingleOrDefault("select * from FapUser where 1=2");

            Assert.Null(s);
        }
        [Fact]
        public void QuerySingleOrDefaultMullti()
        {
            var ex = Assert.Throws<FapException>(() =>
            {
                var s = _dbContext.QuerySingleOrDefault("select * from FapUser where Id>1");
            });
            Assert.Contains("Sequence contains more than one element", ex.Message);
        }
        [Fact]
        public void QueryFirstMullti()
        {
            var s = _dbContext.QueryFirst("select * from FapUser where Id>1");
            Assert.NotNull(s);
        }
        [Fact]
        public void QueryFirstZero()
        {
            var ex = Assert.Throws<FapException>(() =>
            {
                var s = _dbContext.QueryFirst("select * from FapUser where 1=2");
            });
            //var s = _dbContext.QueryFirst("select * from FapUser where Id>1");
            Assert.Contains("Sequence contains no elements", ex.Message);
        }
        [Fact]
        public void QueryFirstOrDefaulttMullti()
        {
            var s = _dbContext.QueryFirstOrDefault("select * from FapUser where Id>1");
            Assert.NotNull(s);
        }
        [Fact]
        public void QueryFirstOrDefaulttZero()
        {
            var s = _dbContext.QueryFirstOrDefault("select * from FapUser where 1=2");
            Assert.Null(s);
        }
        [Fact]
        public void Delete()
        {
            _dbContext.BeginTransaction();
            long id= _dbContext.Insert<Employee>(new Employee { EmpName = "test" });
            _dbContext.Delete<Employee>(id);
            _dbContext.Commit();
            var employee= _dbContext.QueryWhere<Employee>($"Id={id}");
            Assert.NotNull(employee);
            Assert.Equal(0, employee?.Count());

        }
        [Fact]
        public void DeleteLogic()
        {
           
            var b= _dbContext.Delete<FapUser>(42);
            
            Assert.True(b);

        }
        [Fact]
        public void DeleteTraceDynamic()
        {
            dynamic obj = new FapDynamicObject();
            obj.TableName = "Employee";
            obj.Id = 89;
            long id= _dbContext.DeleteDynamicData(obj);
            Assert.NotEqual(89, id);

            dynamic obj1 = new FapDynamicObject();
            obj1.TableName = "FapUser";
            obj1.Id = 90;
            var uid = _dbContext.DeleteDynamicData(obj1);
            Assert.Equal(90, id);

        }
        [Fact]
        public void DeleteLogicDynamic()
        {
            dynamic obj1 = new FapDynamicObject();
            obj1.TableName = "FapUser";
            obj1.Id = 90;
            var uid = _dbContext.DeleteDynamicData(obj1);
            Assert.Equal(90, uid);

        }
        [Fact]
        public void UpdateTrace()
        {
            var emp= _dbContext.QueryFirst<Employee>("select * from employee where id=74");
            emp.EmpPinYin = "liyoujun";
            emp= _dbContext.Update<Employee>(emp);
            Assert.Equal("liyoujun", emp.EmpPinYin);
        }

    }
}
