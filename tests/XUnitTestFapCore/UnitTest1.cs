using Fap.Core.Utility;
using System;
using Xunit;

namespace XUnitTestFapCore
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            PasswordHasher hasher = new PasswordHasher();
            var pwd = hasher.HashPassword("1");
            bool result = hasher.VerifyHashedPassword(pwd, "1");
            Assert.True(result);
        }
    }
}
