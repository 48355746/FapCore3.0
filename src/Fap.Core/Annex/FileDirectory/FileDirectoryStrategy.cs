using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Annex.FileDirectory
{
    /// <summary>
    /// 文件目录策略
    /// 目录树节点等级:2  目录树等级位数:8
    /// 目录树节点等级:3  目录树等级位数:12
    /// 目录树节点等级:4  目录树等级位数:16
    /// 目录树节点等级:5  目录树等级位数:20
    /// 目录树节点等级:6  目录树等级位数:24
    /// </summary>
    public class FileDirectoryStrategy
    {
        /// <summary>
        /// 目录树节点等级
        /// </summary>
        public int Level
        {
            get;
            set;
        }

        /// <summary>
        /// 目录树等级位数
        /// </summary>
        public int Capacity
        {
            get
            {
                return Level * 4;
            }
        }

        /// <summary>
        /// 根据编码，获取完整路径
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetPath(string code)
        {
            int len = code.Length;

            string result = code;
            if (len < Capacity)
            {
                result = result.PadLeft(Capacity, '0');
            }
            return result;
        }
        /// <summary>
        /// 根据编码，生成文件目录编码路径
        /// </summary>
        /// <param name="code"></param>
        /// <param name="fileNameWithoutSuffix"></param>
        /// <returns></returns>
        public string GetFullPath(string code, out string fileNameWithoutSuffix)
        {
            string path = this.GetPath(code);
            List<string> builder = new List<string>();
            for (int i = 0; i < Level; i++)
            {
                builder.Add(path.Substring(i * 4, 4));
            }
            
            fileNameWithoutSuffix = path.Substring((Level - 1) * 4, 4);
            return Path.Combine(builder.ToArray()); 
        }


    }
}
