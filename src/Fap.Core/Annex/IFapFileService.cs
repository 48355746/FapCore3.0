using System.Collections.Generic;
using System.IO;
using Fap.Core.Infrastructure.Model;

namespace Fap.Core.Annex
{
    public interface IFapFileService
    {
        bool DeleteFileByFid(string fid);
        FapAttachment DownloadFileByFid(string fid, out Stream stream);
        FapAttachment DownloadOneFileByBid(string bid, out Stream stream);
        void DownloadZip(IEnumerable<FapAttachment> attachments, FapFileService.DownloadZipEventHandler handler);
        void DownloadZip(List<string> attachmentFids, FapFileService.DownloadZipEventHandler handler);
        void DownloadZip(string bid, FapFileService.DownloadZipEventHandler handler);
        
        void StreamToFile(Stream stream, string fileName);
        string UploadFile(Stream stream, FapAttachment attachement);
        string UploadFile(Stream stream, string fileName, string bid);
        string UploadFile(string filePath, string bid);
        IEnumerable<FapAttachment> FileList(string bid);
        Stream GetFileStream(FapAttachment attachment);
    }
}