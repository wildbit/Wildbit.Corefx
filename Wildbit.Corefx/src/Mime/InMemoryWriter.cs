using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace Wildbit.Corefx.Mime
{
    internal class InMemoryWriter : BaseWriter
    {
        public InMemoryWriter(Stream stream, bool shouldEncodeLeadingDots) : base(stream, shouldEncodeLeadingDots)
        {
        }

        protected override void OnClose(object sender, EventArgs args)
        {
            //Don't do anything!
        }

        internal override void Close()
        {
            //No need to do anything!
        }

        internal override void WriteHeaders(NameValueCollection headers, bool allowUnicode)
        {
            var encoding = allowUnicode ? Encoding.UTF8 : Encoding.ASCII;
            //TODO: This should probably throw if a header includes a unicode character, but
            // the 'allowUnicode' parameter is false.
            // This implementation might be totally wrong.. 
            // Perhaps need to use QP or EW, or Base64 to fix things up.
            var encodedHeaders = "";
            foreach (var a in headers.AllKeys)
            {
                encodedHeaders += $"{a}: { headers[a] }\r\n";
            }

            if (!allowUnicode && !MimeBasePart.IsAscii(encodedHeaders, true))
            {
                throw new FormatException("The message's headers contain unicode characters, but unicode is not permitted for these headers.");
            }
            var headerBytes = encoding.GetBytes(encodedHeaders);
            _stream.Write(headerBytes, 0, headerBytes.Length);
        }
    }
}