using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Core
{
    public interface IUser
    {
        bool ModifyEmployee(string pinyin);
    }
    public interface IUser1
    {
        string Get1(string userName);
    }
}
