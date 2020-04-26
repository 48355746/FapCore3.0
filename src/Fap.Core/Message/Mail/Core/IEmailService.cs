using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MailKit.Search;
using MimeKit;
using MimeKit.Text;

namespace Fap.Core.Message.Mail.Core
{
    public interface IEmailService
    {
        Task<List<MimeMessage>> RecieveEmailByImapAsync(IEnumerable<string> readedUids, SearchQuery query = null);
        Task<List<MimeMessage>> RecieveEmailByPop3Async(IEnumerable<string> readedUids);
        void Send(string mailTo, string subject, string message, TextFormat textFormat = TextFormat.Plain, IEnumerable<AttachmentInfo> attachments = null);
        void Send(string mailTo, string subject, string message, Encoding encoding, TextFormat textFormat = TextFormat.Plain, IEnumerable<AttachmentInfo> attachments = null);
        void Send(string mailTo, string mailCc, string mailBcc, string subject, string message, TextFormat textFormat = TextFormat.Plain, IEnumerable<AttachmentInfo> attachments = null);
        void Send(string mailTo, string mailCc, string mailBcc, string subject, string message, Encoding encoding, TextFormat textFormat = TextFormat.Plain, IEnumerable<AttachmentInfo> attachments = null);
        Task SendAsync(string mailTo, string subject, string message, TextFormat textFormat = TextFormat.Plain, IEnumerable<AttachmentInfo> attachments = null);
        Task SendAsync(string mailTo, string subject, string message, Encoding encoding, TextFormat textFormat = TextFormat.Plain, IEnumerable<AttachmentInfo> attachments = null);
        Task SendAsync(string mailTo, string mailCc, string mailBcc, string subject, string message, TextFormat textFormat = TextFormat.Plain, IEnumerable<AttachmentInfo> attachments = null);
        Task SendAsync(string mailTo, string mailCc, string mailBcc, string subject, string message, Encoding encoding, TextFormat textFormat = TextFormat.Plain, IEnumerable<AttachmentInfo> attachments = null);
    }
}