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
        /// <param name="message"></param>
        /// <param name="allowUnicode"></param>
        /// <returns></returns>
        public static string MessageDump(this MailMessage message, bool allowUnicode = true)
        {
            using (var ms = new MemoryStream())
            {
                var writer = new InMemoryWriter(ms, false);
                message.Send(writer, false, allowUnicode);

                var encoding = allowUnicode ? Encoding.UTF8 : Encoding.ASCII;
                ms.Position = 0;

                return encoding.GetString(ms.GetBuffer());
            }
        }
    }
}
