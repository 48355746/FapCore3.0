using Fap.Core.Annex.Utility.Events;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Model;
using System.IO;

namespace Fap.Core.Annex.Database
{
    public class DatabaseService : IFileService
    {
        private IDbContext _dataAccessor;
        public DatabaseService(IDbContext dataAccessor)
        {
            _dataAccessor = dataAccessor;
        }
        public bool Upload(Stream stream, FapFileInfo fileInfo, FileUploadEventHandler updateEvent)
        {
            FapAttachment attachment = _dataAccessor.Get<FapAttachment>((int)fileInfo.FileId);
            if (attachment != null)
            {
                using (var inStream = stream)
                {
                    byte[] bytes = new byte[inStream.Length];
                    inStream.Read(bytes, 0, bytes.Length);
                    attachment.FileContent = bytes;
                }

                _dataAccessor.Update<FapAttachment>(attachment);
            }

            return true;
        }

        public Stream Download(FapFileInfo fileInfo)
        {
            FapAttachment attachment = _dataAccessor.Get<FapAttachment>((int)fileInfo.FileId);
            if (attachment != null && attachment.FileContent!=null)
            {
                return new MemoryStream(attachment.FileContent); 
            }
            return null;
        }

        public bool Delete(FapFileInfo fileInfo)
        {
            return true;
        }
    }
}
