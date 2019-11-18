using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTestFapCore
{
    public interface IUser
    {
        bool ModifyEmployee(string pinyin);
        bool ModifyUser(string pinyin);
        bool ModifyEmployeeDynamic(string pinyin);
        bool ModifyUserDynamic(string pinyin);
        bool DeleteTrace();
        bool DeleteLogic();
    }
    public interface IUser1
    {
        string Get1(string userName);
    }
}
