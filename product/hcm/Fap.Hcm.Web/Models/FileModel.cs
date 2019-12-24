using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fap.Hcm.Web.Models
{
    public class FileModel
    {
        public string TableName { get; set; }
        public IEnumerable<IFormFile> Files { get; set; }
    }
}
