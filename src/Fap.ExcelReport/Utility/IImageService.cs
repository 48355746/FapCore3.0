using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Fap.ExcelReport.Utility
{
    public interface IImageService
    {
        Bitmap GetBitmap(string bid);
    }
}
