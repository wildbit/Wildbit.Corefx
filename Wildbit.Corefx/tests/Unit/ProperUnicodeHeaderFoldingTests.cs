using Wildbit.Corefx.Mail;
using Xunit;

namespace Wildbit.Corefx.UnitTests
{
    public class ProperUnicodeHeaderFoldingTests
    {
        [Fact]
        public void UnicodeHeadersAreFoldedOnCharacterBoundaries()
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress("asdf@example.com"));
            message.From = new MailAddress("asdf@example.com");

            message.Headers.Add("Feedback-ID", "210165:67109:редовна ознака:postmark");

            var messageDump = message.MessageDump();
        }
    }
}
