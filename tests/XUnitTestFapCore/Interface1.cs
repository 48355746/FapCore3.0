using System;
using System.Collections.Generic;
using System.Text;

namespace XUnitTestFapCore
{
    public interface IUser
    {
        bool ModifyEmployee(string pinyin);
        bool ModifyUser(string pinyin);
    }
    public interface IUser1
    {
        string Get1(string userName);
    }
}
