using Fap.Core.Annex;
using Fap.Core.DI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Fap.ExcelReport.Utility
{
    [Service]
    public class ImageService : IImageService
    {
        private readonly IFapFileService _fileService;
        public ImageService(IFapFileService fileService)
        {
            _fileService = fileService;
        } 
        public Bitmap GetBitmap(string bid)
        {
            var annex= _fileService.DownloadOneFileByBid(bid, out Stream stream);
            if (annex == null)
            {           
                var img= Image.FromFile(Path.Combine(Environment.CurrentDirectory, "wwwroot", "Content", "avatars", "profile-pic.jpg"));
                return new Bitmap(img);
            }
            return new Bitmap(Image.FromStream(stream));
        }    
    }
}
