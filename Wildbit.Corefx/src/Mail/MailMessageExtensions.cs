using System.IO;
using System.Text;
using Wildbit.Corefx.Mime;

namespace Wildbit.Corefx.Mail
{
    public static class MailMessageExtensions
    {
        /// <summary>
        /// Produce the message dump from a MailMessage.
        /// </summary>
        /// <param name="message">The message for which you'd like content.</param>
        /// <param name="escapeUnicode">Depending on the transport, the message may not require unicode encoding (by the use of "Q" Encoding). 
        /// If you would like a less irritating dump, you may set this to false, and the dump will produce the UTF-8 string with the unicode
        /// characters unescaped.</param>
        /// <returns></returns>
        public static string MessageDump(this MailMessage message, bool escapeUnicode = true)
        {
            using (var ms = new MemoryStream())
            {
                var writer = new InMemoryWriter(ms, false);
                message.Send(writer, false, !escapeUnicode);

                var encoding = !escapeUnicode ? Encoding.UTF8 : Encoding.ASCII;

                var count = (int)ms.Position;
                ms.Position = 0;
                return encoding.GetString(ms.GetBuffer(), 0, count);
            }
        }
    }
}
