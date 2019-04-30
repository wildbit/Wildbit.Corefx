using System;
using System.Linq;
using Wildbit.Corefx.Mail;
using Xunit;

namespace Wildbit.Corefx.UnitTests.MailMessageTests
{
    public class CustomDateHeaderTest
    {
        [Fact]
        public void SetCustomDate_ShouldBeUsedWhenSending()
        {
            var expected = new DateTime(2001, 1, 1);
            var mailMessage = new MailMessage("from@test.com", "to@test.com", "Test subject", "Test body")
            {
                Date = expected
            };

            var dump = mailMessage.MessageDump();
            var splitLines = dump.Split(new[] { "\r\n" }, StringSplitOptions.None);
            var dateHeader = splitLines.First(s => s.StartsWith("Date: "));
            var dateToParse = dateHeader.Substring(dateHeader.IndexOf(" ", StringComparison.Ordinal) + 1);
            var actual = DateTime.Parse(dateToParse);

            Assert.Equal(expected, actual);
        }
    }
}
