using System;
using System.Collections.Generic;
using System.Linq;
using Fap.Core.Extensions;
using Fap.AspNetCore.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Fap.Core.Annex;
using Fap.Core.Infrastructure.Model;
using System.IO;

namespace Fap.Hcm.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class PublicController : FapController
    {
        private readonly IFapFileService _fapFileService;
        public PublicController(IServiceProvider serviceProvider, IFapFileService fapFileService) : base(serviceProvider)
        {
            _fapFileService = fapFileService;
        }
        [HttpGet("{fid}")]
        public FileResult Photo(string fid)
        {
            if (fid.IsMissing())
            {
                return File("~/Content/avatars/profile-pic.jpg", "image/png");
            }
            FapAttachment attachment = _fapFileService.DownloadOneFileByBid(fid, out Stream stream);
            if (attachment == null || stream == null)
            {
                return File("~/Content/avatars/profile-pic.jpg", "image/png");
            }
            else
            {
                return File(stream, attachment.FileType);
            }

        }
    }
}