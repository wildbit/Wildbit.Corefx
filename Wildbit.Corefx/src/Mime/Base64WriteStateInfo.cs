// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;

namespace Wildbit.Corefx.Mime
{
    internal class Base64WriteStateInfo : WriteStateInfoBase
    {
        internal Base64WriteStateInfo() { }

        internal Base64WriteStateInfo(int bufferSize, byte[] header, byte[] footer, int maxLineLength, int mimeHeaderLength, Encoding textEncoding = null) :
            base(bufferSize, header, footer, maxLineLength, mimeHeaderLength, textEncoding)
        {
        }

        internal int Padding { get; set; }
        internal byte LastBits { get; set; }
    }
}
