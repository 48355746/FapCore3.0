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
using Fap.Core.Utility;

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
            Assert.Equal(0, ssss?.Count());
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
            long id = _dbContext.Insert<Employee>(new Employee { EmpName = "test" });
            _dbContext.Delete<Employee>(id);
            _dbContext.Commit();
            var employee = _dbContext.QueryWhere<Employee>($"Id={id}");
            Assert.NotNull(employee);
            Assert.Equal(0, employee?.Count());

        }
        [Fact]
        public void DeleteLogic()
        {

            var b = _dbContext.Delete<FapUser>(42);

            Assert.True(b);

        }
        [Fact]
        public void DeleteTraceDynamic()
        {
            //dynamic obj = new FapDynamicObject("Employee", 89,ts:);
            ////obj.TableName = "Employee";
            ////obj.Id = 89;
            //long id = _dbContext.DeleteDynamicData(obj);
            //Assert.NotEqual(89, id);

            //dynamic obj1 = new FapDynamicObject("FapUser", 90);
            ////obj1.TableName = "FapUser";
            ////obj1.Id = 90;
            //var uid = _dbContext.DeleteDynamicData(obj1);
            //Assert.Equal(90, id);

        }
        [Fact]
        public void DeleteLogicDynamic()
        {
            //dynamic obj1 = new FapDynamicObject("FapUser", 90);
            ////obj1.TableName = "FapUser";
            ////obj1.Id = 90;
            //var uid = _dbContext.DeleteDynamicData(obj1);
            //Assert.Equal(90, uid);

        }
        [Fact]
        public void UpdateTrace()
        {
            var emp = _dbContext.QueryFirst<Employee>("select * from employee where id=2474");
            emp.EmpPinYin = "gaoyuan1";
            var rv = _dbContext.Update<Employee>(emp);
            Assert.True(rv);
        }
        [Fact]
        public void UpdateDyncTrace()
        {
            //dynamic emp = new FapDynamicObject("Employee", 2475);
            ////emp.Id = 2475;
            //emp.EmpPinYin = "gaoyuan2";
            //var boo = _dbContext.UpdateDynamicData(emp);
            //Assert.True(boo);
        }
        [Fact]
        public void Update()
        {
            var emp = _dbContext.QueryFirst<FapUser>("select * from FapUser where id=35");
            emp.UserEmail = "gaoyuan1@fda.com";
            var rv = _dbContext.Update<FapUser>(emp);
            Assert.True(rv);
        }
        [Fact]
        public void UpdateDync()
        {
            //dynamic emp = new FapDynamicObject("FapUser", 35);
            ////emp.Id = 2475;
            //emp.UserEmail = "gaoyuan2@fda.com";
            //var boo = _dbContext.UpdateDynamicData(emp);
            //Assert.True(boo);
        }
        [Fact]
        public void InsertEntity()
        {
            Employee emp = new Employee();
            emp.EmpName = "jeke";
            emp.EmpCode = "jeke zhang";
            emp.Age = 20;
            long id = _dbContext.Insert<Employee>(emp);
            Assert.Equal(id, emp.Id);
        }
        [Fact]
        public void InsertDynamic()
        {
            dynamic emp = new FapDynamicObject("FapUser", 0,UUIDUtils.Ts);
            emp.UserName = "jeke";
            emp.UserCode = "jeke zhang";
            emp.UserEmail = "jeke@leo.com";
            emp.UserPassword = "AQAAAAEAAAPoAAAAEPyzGOp9bSEKLsUrTKsxb/dYCuil0xALUPFogrbMCTuTfDb/w3YuWxhqlFTCYhGUow==";
            long id = _dbContext.InsertDynamicData(emp);
            Assert.Equal(id, emp.Id);
        }
    }
}
