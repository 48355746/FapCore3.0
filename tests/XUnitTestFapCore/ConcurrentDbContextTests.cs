using Fap.Core.Rbac;
using Fap.Hcm.Web;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Fap.Core;
using System.Threading.Tasks;

namespace XUnitTestFapCore
{
    public class ConcurrentDbContextTests : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        private readonly ILoginService _loginService;
        private readonly IUser _userService;
        public ConcurrentDbContextTests(TestWebApplicationFactory<Startup> factory)
        {
            _loginService = factory.Services.GetService<ILoginService>();
            _userService = factory.Services.GetService<IUser>();
        }
        [Fact]
        public void Login()
        {

            var user = _loginService.Login("hr");

            Assert.Equal("hr",user.UserName);
        }
        [Fact]
        public void ConcurrentUpdateEntityTrace()
        {
            Action<string> a1 = (s) =>
            {
                _userService.ModifyEmployee(s);
            };
            Parallel.Invoke(()=>a1("gaoya1"),()=>
             a1("gaoya2"), () => a1("gaoya3"),
             () => a1("gaoya4"),
              () => a1("gaoya5"),
             () => a1("gaoya6"),
              () => a1("gaoya7"),
              () => a1("gaoya8"),
              () => a1("gaoya9"));

           
            Assert.True(true);
        }
        [Fact]
        public void UpdateEntityTrace()
        {
            var s= _userService.ModifyEmployee("zhangsan");
            Assert.True(s);
        }

        [Fact]
        public void ConcurrentUpdateDynamicTrace()
        {
            Action<string> a1 = (s) =>
            {
                _userService.ModifyEmployeeDynamic(s);
            };
            Parallel.Invoke(() => a1("gaoya1")
            , () =>a1("gaoya2"), () => a1("gaoya3"),
             () => a1("gaoya4"),
              () => a1("gaoya5"),
             () => a1("gaoya6"),
              () => a1("gaoya7"),
              () => a1("gaoya8"),
              () => a1("gaoya9")
              );
            Assert.True(true);
        }
        [Fact]
        public void UpdateDynamicTrace()
        {
            var b= _userService.ModifyEmployeeDynamic("lisi");
            Assert.True(b);
        }
        [Fact]
        public void UpdateDynamicLogic()
        {
            var b = _userService.ModifyUserDynamic("111-222");
            Assert.True(b);
        }
        [Fact]
        public void ConcurrentUpdateDynamicLogic()
        {
            Action<string> a1 = (s) =>
            {
                _userService.ModifyUserDynamic(s);
            };
            Parallel.Invoke(() => a1("gaoya1")
            , () => a1("gaoya2"), () => a1("gaoya3"),
             () => a1("gaoya4"),
              () => a1("gaoya5"),
             () => a1("gaoya6"),
              () => a1("gaoya7"),
              () => a1("gaoya8"),
              () => a1("gaoya9")
              );
            Assert.True(true);
        }

        [Fact]
        public void DeleteLogic()
        {
            var b = _userService.DeleteLogic();
            Assert.True(b);
        }
        [Fact]
        public void ConcurrentDeleteEntityLogic()
        {
            Action<string> a1 = (s) =>
            {
                _userService.DeleteLogic();
            };
            Parallel.Invoke(() => a1("gaoya1")
            , () => a1("gaoya2"), () => a1("gaoya3"),
             () => a1("gaoya4"),
              () => a1("gaoya5"),
             () => a1("gaoya6"),
              () => a1("gaoya7"),
              () => a1("gaoya8"),
              () => a1("gaoya9")
              );
            Assert.True(true);
        }
        [Fact]
        public void DeleteTrace()
        {
            var b = _userService.DeleteTrace();
            Assert.True(b);
        }
        [Fact]
        public void ConcurrentDeleteEntityTrace()
        {
            Action<string> a1 = (s) =>
            {
                _userService.DeleteTrace();
            };
            Parallel.Invoke(() => a1("gaoya1")
            , () => a1("gaoya2"), () => a1("gaoya3"),
             () => a1("gaoya4"),
              () => a1("gaoya5"),
             () => a1("gaoya6"),
              () => a1("gaoya7"),
              () => a1("gaoya8"),
              () => a1("gaoya9")
              );
            Assert.True(true);
        }
        [Fact]
        public void DeleteDynamicLogic()
        {
            var b = _userService.DeleteDynamicLogic();
            Assert.True(b);
        }
        [Fact]
        public void DeleteDynamicTrace()
        {
            var b = _userService.DeleteDynamicTrace();
            Assert.True(b);
        }
        [Fact]
        public void ConcurrentDeleteDynamicTrace()
        {
            Action<string> a1 = (s) =>
            {
                _userService.DeleteDynamicTrace();
            };
            Parallel.Invoke(() => a1("gaoya1")
            , () => a1("gaoya2"), () => a1("gaoya3"),
             () => a1("gaoya4"),
              () => a1("gaoya5"),
             () => a1("gaoya6"),
              () => a1("gaoya7"),
              () => a1("gaoya8"),
              () => a1("gaoya9")
              );
            Assert.True(true);
        }
        [Fact]
        public void ConcurrentDeleteDynamicLogic()
        {
            Action<string> a1 = (s) =>
            {
                _userService.DeleteDynamicLogic();
            };
            Parallel.Invoke(() => a1("gaoya1")
            , () => a1("gaoya2"), () => a1("gaoya3"),
             () => a1("gaoya4"),
              () => a1("gaoya5"),
             () => a1("gaoya6"),
              () => a1("gaoya7"),
              () => a1("gaoya8"),
              () => a1("gaoya9")
              );
            Assert.True(true);
        }
    }
}
