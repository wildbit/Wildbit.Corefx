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

            message.Headers.Add("Feedback-ID", 
                @"210165:67109:редовна ознака:postmark 210165:67109:редовна ознака:postmark1 
210165:67109:редовна ознака:postmark2 210165:67109:редовна ознака:postmark3");
            
            var messageDump = message.MessageDump();

            Assert.Contains(@"Feedback-ID: =?utf-8?B?MjEwMTY1OjY3MTA5OtGA0LXQtNC+0LLQvdCwINC+0Lc=?=
 =?utf-8?B?0L3QsNC60LA6cG9zdG1hcmsgMjEwMTY1OjY3MTA5OtGA0LXQtNC+0LI=?=
 =?utf-8?B?0L3QsCDQvtC30L3QsNC60LA6cG9zdG1hcmsxIA0KMjEwMTY1OjY3MTA5?=
 =?utf-8?B?OtGA0LXQtNC+0LLQvdCwINC+0LfQvdCw0LrQsDpwb3N0bWFyazIgMjEw?=
 =?utf-8?B?MTY1OjY3MTA5OtGA0LXQtNC+0LLQvdCwINC+0LfQvdCw0LrQsDpwb3N0?=
 =?utf-8?B?bWFyazM=?=", messageDump);
        }

        [Fact]
        public void ASCIIHeadersDoNotNeedEncodedWordTreatmentButShouldBeFolded()
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress("asdf@example.com"));
            message.From = new MailAddress("asdf@example.com");

            var header = @"ASCII Chars all the way across, should never be set to encoded word, just include it with correct folding.";
            var foldedHeader = @"Feedback-ID: ASCII Chars all the way across, should never be set to encoded
 word, just include it with correct folding.";

            message.Headers.Add("Feedback-ID", header);

            var messageDump = message.MessageDump();

            Assert.Contains(foldedHeader, messageDump);
        }
    }
}
