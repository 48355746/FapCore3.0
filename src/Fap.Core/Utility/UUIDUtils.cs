using IdGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fap.Core.Utility
{
    public class UUIDUtils
    {
        public static IdGenerator idGenerator = new IdGenerator(1);
        /// <summary>
        /// 时间戳
        /// </summary>
        public static long Ts => new IdGenerator(0).CreateId();
        /// <summary>
        /// 生成FID
        /// </summary>
        /// <returns></returns>
        public static string Fid
        {
            get
            {
                return idGenerator.CreateId().ToString();
              
            }
        }


        /// <summary>
        /// 获取多个Fid
        /// </summary>
        /// <param name="nums"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetFids(int nums) => idGenerator.Take(nums).Select(l => l.ToString());
    }
}
