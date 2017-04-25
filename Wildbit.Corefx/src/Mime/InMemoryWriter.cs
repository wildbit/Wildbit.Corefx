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
            if (headers == null)
                throw new ArgumentNullException(nameof(headers));

            foreach (string key in headers)
            {
                string[] values = headers.GetValues(key);
                foreach (string value in values)
                {
                    WriteHeader(key, value, allowUnicode);
                }
            }
        }
    }
}