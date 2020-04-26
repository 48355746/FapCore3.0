using MimeKit;
using MimeKit.Text;
using Fap.Core.Message.Mail.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit;
using MailKit.Search;
using System.IO;

namespace Fap.Core.Message.Mail.Core
{
    public class EmailService : IEmailService
    {
        private readonly IMailKitProvider _MailKitProvider;

        public EmailService(IMailKitProvider provider)
        {
            _MailKitProvider = provider;
        }

        /// <summary>
        /// send email with UTF-8
        /// </summary>
        /// <param name="mailTo">consignee email,multi split with ","</param>
        /// <param name="subject">subject</param>
        /// <param name="message">email message</param>
        /// <param name="textFormat">邮件格式</param>
        public void Send(string mailTo, string subject, string message, TextFormat textFormat = TextFormat.Text, IEnumerable<AttachmentInfo> attachments = null)
        {
            SendEmail(mailTo, null, null, subject, message, Encoding.UTF8, textFormat, attachments);
        }

        /// <summary>
        /// send email
        /// </summary>
        /// <param name="mailTo">consignee email,multi split with ","</param>
        /// <param name="subject">subject</param>
        /// <param name="message">email message</param>
        /// <param name="encoding">email message encoding</param>
        /// <param name="textFormat">邮件格式</param>
        public void Send(string mailTo, string subject, string message, Encoding encoding, TextFormat textFormat = TextFormat.Text, IEnumerable<AttachmentInfo> attachments = null)
        {
            SendEmail(mailTo, null, null, subject, message, encoding, textFormat, attachments);
        }

        /// <summary>
        /// send email with UTF-8
        /// </summary>
        /// <param name="mailTo">consignee email,multi split with ","</param>
        /// <param name="mailCc">send cc,multi split with ","</param>
        /// <param name="mailBcc">send bcc,multi split with ","</param>
        /// <param name="subject">subject</param>
        /// <param name="message">email message</param>
        /// <param name="textFormat">邮件格式</param>
        public void Send(string mailTo, string mailCc, string mailBcc, string subject, string message, TextFormat textFormat = TextFormat.Text, IEnumerable<AttachmentInfo> attachments = null)
        {
            SendEmail(mailTo, mailCc, mailBcc, subject, message, Encoding.UTF8, textFormat, attachments);
        }

        /// <summary>
        /// send email
        /// </summary>
        /// <param name="mailTo">consignee email,multi split with ","</param>
        /// <param name="mailCc">send cc,multi split with ","</param>
        /// <param name="mailBcc">send bcc,multi split with ","</param>
        /// <param name="subject">subject</param>
        /// <param name="message">email message</param>
        /// <param name="encoding">email message encoding</param>
        /// <param name="textFormat">邮件格式</param>
        public void Send(string mailTo, string mailCc, string mailBcc, string subject, string message, Encoding encoding, TextFormat textFormat = TextFormat.Text, IEnumerable<AttachmentInfo> attachments = null)
        {
            SendEmail(mailTo, mailCc, mailBcc, subject, message, encoding, textFormat, attachments);
        }


        /// <summary>
        /// send email with UTF-8 async
        /// </summary>
        /// <param name="mailTo">consignee email,multi split with ","</param>
        /// <param name="subject">subject</param>
        /// <param name="message">email message</param>
        /// <param name="textFormat">邮件格式</param>
        public Task SendAsync(string mailTo, string subject, string message, TextFormat textFormat = TextFormat.Text, IEnumerable<AttachmentInfo> attachments = null)
        {
            return Task.Factory.StartNew(() =>
            {
                SendEmail(mailTo, null, null, subject, message, Encoding.UTF8, textFormat, attachments);
            });
        }

        /// <summary>
        /// send email async
        /// </summary>
        /// <param name="mailTo">consignee email,multi split with ","</param>
        /// <param name="subject">subject</param>
        /// <param name="message">email message</param>
        /// <param name="encoding">email message encoding</param>
        /// <param name="textFormat">邮件格式</param>

        public Task SendAsync(string mailTo, string subject, string message, Encoding encoding, TextFormat textFormat = TextFormat.Text, IEnumerable<AttachmentInfo> attachments = null)
        {
            return Task.Factory.StartNew(() =>
            {
                SendEmail(mailTo, null, null, subject, message, encoding, textFormat, attachments);
            });
        }

        /// <summary>
        /// send email with UTF-8 async
        /// </summary>
        /// <param name="mailTo">consignee email,multi split with ","</param>
        /// <param name="mailCc">send cc,multi split with ","</param>
        /// <param name="mailBcc">send bcc,multi split with ","</param>
        /// <param name="subject">subject</param>
        /// <param name="message">email message</param>
        /// <param name="textFormat">邮件格式</param>
        public Task SendAsync(string mailTo, string mailCc, string mailBcc, string subject, string message, TextFormat textFormat = TextFormat.Text, IEnumerable<AttachmentInfo> attachments = null)
        {
            return Task.Factory.StartNew(() =>
            {
                SendEmail(mailTo, mailCc, mailBcc, subject, message, Encoding.UTF8, textFormat, attachments);
            });
        }

        /// <summary>
        /// send email async
        /// </summary>
        /// <param name="mailTo">consignee email,multi split with ","</param>
        /// <param name="mailCc">send cc,multi split with ","</param>
        /// <param name="mailBcc">send bcc,multi split with ","</param>
        /// <param name="subject">subject</param>
        /// <param name="message">email message</param>
        /// <param name="encoding">email message encoding</param>
        /// <param name="textFormat">邮件格式</param>
        public Task SendAsync(string mailTo, string mailCc, string mailBcc, string subject, string message, Encoding encoding, TextFormat textFormat = TextFormat.Text, IEnumerable<AttachmentInfo> attachments = null)
        {
            return Task.Factory.StartNew(() =>
            {
                SendEmail(mailTo, mailCc, mailBcc, subject, message, encoding, textFormat, attachments);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mailTo"></param>
        /// <param name="mailCc"></param>
        /// <param name="mailBcc"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="encoding"></param>
        /// <param name="textFormat"></param>
        private void SendEmail(string mailTo, string mailCc, string mailBcc, string subject, string message, Encoding encoding, TextFormat textFormat = TextFormat.Text, IEnumerable<AttachmentInfo> attachments = null)
        {
            var _to = new string[0];
            var _cc = new string[0];
            var _bcc = new string[0];
            if (!string.IsNullOrEmpty(mailTo))
                _to = mailTo.Split(';').Select(x => x.Trim()).ToArray();
            if (!string.IsNullOrEmpty(mailCc))
                _cc = mailCc.Split(';').Select(x => x.Trim()).ToArray();
            if (!string.IsNullOrEmpty(mailBcc))
                _bcc = mailBcc.Split(';').Select(x => x.Trim()).ToArray();

            Check.Argument.IsNotEmpty(_to, nameof(mailTo));
            Check.Argument.IsNotEmpty(message, nameof(message));

            var mimeMessage = new MimeMessage();

            //add mail from
            mimeMessage.From.Add(new MailboxAddress(_MailKitProvider.Options.SenderName, _MailKitProvider.Options.SenderEmail));

            //add mail to 
            foreach (var to in _to)
            {
                mimeMessage.To.Add(MailboxAddress.Parse(to));
            }

            //add mail cc
            foreach (var cc in _cc)
            {
                mimeMessage.Cc.Add(MailboxAddress.Parse(cc));
            }

            //add mail bcc 
            foreach (var bcc in _bcc)
            {
                mimeMessage.Bcc.Add(MailboxAddress.Parse(bcc));
            }

            //add subject
            mimeMessage.Subject = subject;

            TextPart txtPart = new TextPart(textFormat);
            txtPart.SetText(encoding, message);

            var alternative = new Multipart("alternative");
            alternative.Add(txtPart);
            if (attachments != null)
            {
                var multipart = new Multipart("mixed");
                multipart.Add(alternative);

                foreach (var att in attachments)
                {
                    if (att.Stream != null)
                    {
                        var attachment = string.IsNullOrWhiteSpace(att.ContentType) ? new MimePart() : new MimePart(att.ContentType);
                        attachment.Content = new MimeContent(att.Stream);
                        attachment.ContentDisposition = new ContentDisposition(ContentDisposition.Attachment);
                        attachment.ContentTransferEncoding = att.ContentTransferEncoding;
                        attachment.FileName = ConvertHeaderToBase64(att.FileName, Encoding.UTF8);//解决附件中文名问题
                        attachment.ContentId = att.UniqueId;
                        multipart.Add(attachment);

                    }
                }
                mimeMessage.Body = multipart;
            }
            else
            {
                mimeMessage.Body = alternative;
            }
            
            using (var client = _MailKitProvider.SmtpClient)
            {
                client.Send(mimeMessage);
                client.Disconnect(true);
            }
        }
        /// <summary>
        /// pop3收取邮件
        /// </summary>
        /// <param name="uids">已读uid</param>
        /// <returns></returns>
        public async Task<List<MimeMessage>> RecieveEmailByPop3Async(IEnumerable<string> readedUids)
        {
            try
            {
                List<MimeMessage> listMsg = new List<MimeMessage>();
                using (var client = _MailKitProvider.Pop3Client)
                {
                    for (int i = 0; i < client.Count; i++)
                    {
                        string uid = client.GetMessageUid(i);
                        if (readedUids.Any() && readedUids.Contains(uid))
                        {
                            continue;
                        }
                        var message = await client.GetMessageAsync(i);
                        message.MessageId = uid;
                        listMsg.Add(message);
                    }

                    client.Disconnect(true);
                    return listMsg;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Imap收邮件
        /// </summary>
        /// <param name="readedUids">已读列表</param>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public async Task<List<MimeMessage>> RecieveEmailByImapAsync(IEnumerable<string> readedUids, SearchQuery query = null)
        {
            try
            {

                if (query == null)
                {
                    query = SearchQuery.DeliveredAfter(DateTime.Now.AddDays(-8));
                }
                using (var client = _MailKitProvider.ImapClient)
                {
                    //获取所有的文件夹
                    //List<IMailFolder> mailFolderList = client.GetFolders(client.PersonalNamespaces[0]).ToList();
                    //只获取收件箱文件夹
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);
                    //获取大于2016-9-1时间的所有邮件的唯一Id
                    var uidss = inbox.Search(query);
                    //获取邮件头
                    //IList<IMessageSummary> summarys = folder.Fetch(uidss, MessageSummaryItems.UniqueId | MessageSummaryItems.Full);
                    List<MimeMessage> listMsg = new List<MimeMessage>();
                    //获取完整邮件
                    foreach (var item in uidss)
                    {
                        string uid = item.Id.ToString();
                        if (readedUids.Any() && readedUids.Contains(uid))
                        {
                            continue;
                        }
                        MimeMessage message = await inbox.GetMessageAsync(new UniqueId(item.Id));
                        message.MessageId = item.Id.ToString();
                        listMsg.Add(message);
                    }
                    inbox.Close();
                    client.Disconnect(true);
                    return listMsg;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static string ConvertToBase64(string inputStr, Encoding encoding)
        {
            return Convert.ToBase64String(encoding.GetBytes(inputStr));
        }
        private static string ConvertHeaderToBase64(string inputStr, Encoding encoding)
        {
            var encode = !string.IsNullOrEmpty(inputStr) && inputStr.Any(c => c > 127);
            if (encode)
            {
                return "=?" + encoding.WebName + "?B?" + ConvertToBase64(inputStr, encoding) + "?=";
            }
            return inputStr;
        }
        //public List<MimeEntity> GetPart(SearchQuery query)
        //{
        //    try
        //    {
        //        if (query == null)
        //        {
        //            query = SearchQuery.DeliveredAfter(DateTime.Now.AddDays(-1));
        //        }
        //        using (var client = _MailKitProvider.ImapClient)
        //        {
        //            //获取所有的文件夹
        //            //List<IMailFolder> mailFolderList = client.GetFolders(client.PersonalNamespaces[0]).ToList();
        //            //只获取收件箱文件夹
        //            var inbox = client.Inbox;
        //            //打开文件夹并设置为读的方式
        //            inbox.Open(FolderAccess.ReadOnly);

        //            //获取大于2016-9-1时间的所有邮件的唯一Id
        //            var uidss = inbox.Search(query);
        //            //获取邮件头
        //            //IList<IMessageSummary> summarya = inbox.Fetch(uidss, MessageSummaryItems.UniqueId | MessageSummaryItems.Full);
        //            List<MimeMessage> listMsg = new List<MimeMessage>();
        //            foreach (var summary in inbox.Fetch(uidss, MessageSummaryItems.UniqueId | MessageSummaryItems.BodyStructure))
        //            {
        //                if (summary.TextBody != null)
        //                {
        //                    // this will download *just* the text/plain part
        //                    var text = inbox.GetBodyPart(summary.UniqueId, summary.TextBody);
        //                }

        //                if (summary.HtmlBody != null)
        //                {
        //                    // this will download *just* the text/html part
        //                    var html = inbox.GetBodyPart(summary.UniqueId, summary.HtmlBody);
        //                }

        //                // if you'd rather grab, say, an image attachment... it might look something like this:
        //                if (summary.Body is BodyPartMultipart)
        //                {
        //                    var multipart = (BodyPartMultipart)summary.Body;

        //                    var attachment = multipart.BodyParts.OfType<BodyPartBasic>().FirstOrDefault(x => x.FileName == "logo.jpg");
        //                    if (attachment != null)
        //                    {
        //                        // this will download *just* the attachment
        //                        var part = inbox.GetBodyPart(summary.UniqueId, attachment);
        //                    }
        //                }
        //            }

        //            inbox.Close();
        //            client.Disconnect(true);                   
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
    /// <summary>
    /// 附件信息
    /// </summary>
    public class AttachmentInfo : IDisposable
    {
        /// <summary>
        /// 附件类型，比如application/pdf
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 唯一ID
        /// </summary>
        public string UniqueId { get; set; }
        /// <summary>
        /// 文件传输编码方式，默认ContentEncoding.Default
        /// </summary>
        public ContentEncoding ContentTransferEncoding { get; set; } = ContentEncoding.Default;
        /// <summary>
        /// 文件数组
        /// </summary>
        public byte[] Data { get; set; }
        private Stream stream;
        /// <summary>
        /// 文件数据流，获取数据时优先采用此部分
        /// </summary>
        public Stream Stream
        {
            get
            {
                if (this.stream == null && this.Data != null)
                {
                    stream = new MemoryStream(this.Data);
                }
                return this.stream;
            }
            set { this.stream = value; }
        }
        /// <summary>
        /// 释放Stream
        /// </summary>
        public void Dispose()
        {
            if (this.stream != null)
            {
                this.stream.Dispose();
            }
        }
    }
}
