﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Text;

namespace Wildbit.Corefx.Mime
{
    internal interface IEncodableStream
    {
        int EncodeHeaderBytes(byte[] buffer, Encoding textEncoding);
        int DecodeBytes(byte[] buffer, int offset, int count);
        int EncodeBytes(byte[] buffer, int offset, int count);
        string GetEncodedString();
        Stream GetStream();
    }
}
