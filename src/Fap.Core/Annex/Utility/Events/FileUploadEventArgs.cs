using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fap.Core.Annex.Utility.Events
{
    /// <summary>
    /// 文件上传委托
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <param name="args"></param>
    public delegate void FileUploadEventHandler(FapFileInfo fileInfo, FileUploadEventArgs args);

    public class FileUploadEventArgs : EventArgs
    {
        public long Total
        {
            get;
            set;
        }

        public long Uploaded
        {
            get;
            set;
        }

        public string FileFullName
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }

        public string FileDirectory
        {
            get;
            set;
        }
    }
}
