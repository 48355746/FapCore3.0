using IdGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fap.Core.Utility
{
    public class UUIDUtils
    {
        /// <summary>
        /// 生成FID
        /// </summary>
        /// <returns></returns>
        public static string Fid => new IdGenerator(0).CreateId().ToString();
        /// <summary>
        /// 获取多个Fid
        /// </summary>
        /// <param name="nums"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetFids(int nums) => new IdGenerator(0).Take(nums).Select(l=>l.ToString());
    }
}
