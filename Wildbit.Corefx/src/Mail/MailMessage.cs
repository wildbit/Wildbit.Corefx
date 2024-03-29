// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Specialized;
using Wildbit.Corefx.Mime;
using System.Text;

namespace Wildbit.Corefx.Mail
{
    [Flags]
    public enum DeliveryNotificationOptions
    {
        None = 0, OnSuccess = 1, OnFailure = 2, Delay = 4, Never = (int)0x08000000
    }

    public class MailMessage : IDisposable
    {
        private AlternateViewCollection _views;
        private AttachmentCollection _attachments;
        private AlternateView _bodyView = null;
        private string _body = string.Empty;
        private Encoding _bodyEncoding;
        private TransferEncoding _bodyTransferEncoding = TransferEncoding.Unknown;
        private bool _isBodyHtml = false;
        private bool _disposed = false;
        private Message _message;
        private DeliveryNotificationOptions _deliveryStatusNotification = DeliveryNotificationOptions.None;

        public MailMessage()
        {
            _message = new Message();
            if (NetEventSource.IsEnabled)
                NetEventSource.Associate(this, _message);
        }

        public MailMessage(string from, string to)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from));

            if (to == null)
                throw new ArgumentNullException(nameof(to));

            if (from == string.Empty)
                throw new ArgumentException(string.Format(Strings.net_emptystringcall, nameof(from)), nameof(from));

            if (to == string.Empty)
                throw new ArgumentException(string.Format(Strings.net_emptystringcall, nameof(to)), nameof(to));

            _message = new Message(from, to);
            if (NetEventSource.IsEnabled)
                NetEventSource.Associate(this, _message);
        }


        public MailMessage(string from, string to, string subject, string body) : this(from, to)
        {
            Subject = subject;
            Body = body;
        }


        public MailMessage(MailAddress from, MailAddress to)
        {
            if (from == null)
                throw new ArgumentNullException(nameof(from));

            if (to == null)
                throw new ArgumentNullException(nameof(to));

            _message = new Message(from, to);
        }


        public MailAddress From
        {
            get
            {
                return _message.From;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _message.From = value;
            }
        }

        public MailAddress Sender
        {
            get
            {
                return _message.Sender;
            }
            set
            {
                _message.Sender = value;
            }
        }

        /// <summary>
        /// Should this message emit a Sender header? This is potentially useful for IIS to forward SMTP mail, but
        /// confuses the issue when Return-Path is involved. You can disable the "Sender" header by setting this to false.
        /// </summary>
        public bool EmitSenderHeader
        {
            get
            {
                return _message.EmitSenderHeader;
            }
            set
            {
                _message.EmitSenderHeader = value;
            }
        }

        [Obsolete("ReplyTo is obsoleted for this type.  Please use ReplyToList instead which can accept multiple addresses. http://go.microsoft.com/fwlink/?linkid=14202")]
        public MailAddress ReplyTo
        {
            get
            {
                return _message.ReplyTo;
            }
            set
            {
                _message.ReplyTo = value;
            }
        }

        public MailAddressCollection ReplyToList
        {
            get
            {
                return _message.ReplyToList;
            }
        }

        public MailAddressCollection To
        {
            get
            {
                return _message.To;
            }
        }

        public MailAddressCollection Bcc
        {
            get
            {
                return _message.Bcc;
            }
        }

        public MailAddressCollection CC
        {
            get
            {
                return _message.CC;
            }
        }

        public MailPriority Priority
        {
            get
            {
                return _message.Priority;
            }
            set
            {
                _message.Priority = value;
            }
        }
        
        /// <summary>
        /// Sets a custom Date header. By default, DateTime.Now will be used.
        /// </summary>
        public DateTime Date
        {
            get
            {
                return _message.Date;
            }
            set
            {
                _message.Date = value;
            }
        }

        public DeliveryNotificationOptions DeliveryNotificationOptions
        {
            get
            {
                return _deliveryStatusNotification;
            }
            set
            {
                if (7 < (uint)value && value != DeliveryNotificationOptions.Never)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }
                _deliveryStatusNotification = value;
            }
        }

        public string Subject
        {
            get
            {
                return (_message.Subject != null ? _message.Subject : string.Empty);
            }
            set
            {
                _message.Subject = value;
            }
        }

        public Encoding SubjectEncoding
        {
            get
            {
                return _message.SubjectEncoding;
            }
            set
            {
                _message.SubjectEncoding = value;
            }
        }

        public NameValueCollection Headers
        {
            get
            {
                return _message.Headers;
            }
        }

        public Encoding HeadersEncoding
        {
            get
            {
                return _message.HeadersEncoding;
            }
            set
            {
                _message.HeadersEncoding = value;
            }
        }

        /// <summary>
        /// The raw body of the message.
        /// Ignored when using Alternative Views, or MimeBody.
        /// </summary>
        public string Body
        {
            get
            {
                return (_body != null ? _body : string.Empty);
            }

            set
            {
                _body = value;

                if (_bodyEncoding == null && _body != null)
                {
                    if (MimeBasePart.IsAscii(_body, true))
                    {
                        _bodyEncoding = Encoding.ASCII;
                    }
                    else
                    {
                        _bodyEncoding = Encoding.GetEncoding(MimeBasePart.DefaultCharSet);
                    }
                }
            }
        }

        public Encoding BodyEncoding
        {
            get
            {
                return _bodyEncoding;
            }
            set
            {
                _bodyEncoding = value;
            }
        }

        public TransferEncoding BodyTransferEncoding
        {
            get
            {
                return _bodyTransferEncoding;
            }
            set
            {
                _bodyTransferEncoding = value;
            }
        }

        /// <summary>
        /// Indicates that the Body property is HTML (or Text).
        /// Ignored when using Alternative Views, or MimeBody.
        /// </summary>
        public bool IsBodyHtml
        {
            get
            {
                return _isBodyHtml;
            }
            set
            {
                _isBodyHtml = value;
            }
        }

        /// <summary>
        /// Indicate a preference for QEncoding when encoding non-ascii headers.
        /// </summary>
        /// <remarks>
        /// The preference of the original System.Net.Mail library was to use base64, but we believe that
        /// QEncoding (QP, instead of base64) is the way to go because some MUAs may percieve it as
        /// "less spammy". This was a valid hueristic in 2012-ish, but may have changed.
        /// </remarks>
        public bool PreferQEncodeForHeaders
        {
            get
            {
                return _message.PreferQEncodeForHeaders;
            }
            set
            {
                _message.PreferQEncodeForHeaders = value;
            }
        }

        /// <summary>
        /// Should we include the Bcc header in the message content?
        /// </summary>
        public bool DisplayBccHeader
        {
            get
            {
                return _message.DisplayBccHeader;
            }
            set
            {
                _message.DisplayBccHeader = value;
            }
        }

        /// <summary>
        /// Determine whether To, Cc, Bcc headers should be appended/replace 
        /// those same headers if they have been set manually.
        /// 
        /// Defaults to true.
        /// </summary>
        /// <remarks>This allows finer-grained control over how headers can be set.</remarks>
        public bool AutomaticallyBuildAndApplyRecipientHeaders
        {
            get { return _message.AutomaticallyBuildAndApplyRecipientHeaders; }
            set { _message.AutomaticallyBuildAndApplyRecipientHeaders = value; }
        }

        /// <summary>
        /// Associated attachments. Ignored when using MimeBody
        /// </summary>
        public AttachmentCollection Attachments
        {
            get
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }

                if (_attachments == null)
                {
                    _attachments = new AttachmentCollection();
                }
                return _attachments;
            }
        }

        /// <summary>
        /// The multipart/related parts of the message. Ignored when using MimeBody.
        /// </summary>
        public AlternateViewCollection AlternateViews
        {
            get
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }

                if (_views == null)
                {
                    _views = new AlternateViewCollection();
                }

                return _views;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;

                if (_views != null)
                {
                    _views.Dispose();
                }
                if (_attachments != null)
                {
                    _attachments.Dispose();
                }
                if (_bodyView != null)
                {
                    _bodyView.Dispose();
                }
            }
        }

        /// <summary>
        /// The root mime part for the message. 
        /// This will override all other body parts and alternative views.
        /// </summary>
        public MimeBasePart MimeBody { get; set; }

        private void SetContent(bool allowUnicode)
        {
            //the attachments may have changed, so we need to reset the message
            if (_bodyView != null)
            {
                _bodyView.Dispose();
                _bodyView = null;
            }

            if (MimeBody != null)
            {
                _message.Content = MimeBody;
            }
            else if (AlternateViews.Count == 0 && Attachments.Count == 0)
            {
                if (!string.IsNullOrEmpty(_body))
                {
                    _bodyView = AlternateView.CreateAlternateViewFromString(_body, _bodyEncoding, (_isBodyHtml ? MediaTypeNames.Text.Html : null));
                    _message.Content = _bodyView.MimePart;
                }
            }
            else if (AlternateViews.Count == 0 && Attachments.Count > 0)
            {
                MimeMultiPart part = new MimeMultiPart(MimeMultiPartType.Mixed);

                if (!string.IsNullOrEmpty(_body))
                {
                    _bodyView = AlternateView.CreateAlternateViewFromString(_body, _bodyEncoding, (_isBodyHtml ? MediaTypeNames.Text.Html : null));
                }
                else
                {
                    _bodyView = AlternateView.CreateAlternateViewFromString(string.Empty);
                }

                part.Parts.Add(_bodyView.MimePart);

                foreach (Attachment attachment in Attachments)
                {
                    if (attachment != null)
                    {
                        //ensure we can read from the stream.
                        attachment.PrepareForSending(allowUnicode);
                        part.Parts.Add(attachment.MimePart);
                    }
                }
                _message.Content = part;
            }
            else
            {
                // we should not unnecessarily use Multipart/Mixed
                // When there is no attachement and all the alternative views are of "Alternative" types.
                MimeMultiPart part = null;
                MimeMultiPart viewsPart = new MimeMultiPart(MimeMultiPartType.Alternative);

                if (!string.IsNullOrEmpty(_body))
                {
                    _bodyView = AlternateView.CreateAlternateViewFromString(_body, _bodyEncoding, null);
                    viewsPart.Parts.Add(_bodyView.MimePart);
                }

                foreach (AlternateView view in AlternateViews)
                {
                    //ensure we can read from the stream.
                    if (view != null)
                    {
                        view.PrepareForSending(allowUnicode);
                        if (view.LinkedResources.Count > 0)
                        {
                            MimeMultiPart wholeView = new MimeMultiPart(MimeMultiPartType.Related);
                            wholeView.ContentType.Parameters["type"] = view.ContentType.MediaType;
                            wholeView.ContentLocation = view.MimePart.ContentLocation;
                            wholeView.Parts.Add(view.MimePart);

                            foreach (LinkedResource resource in view.LinkedResources)
                            {
                                //ensure we can read from the stream.
                                resource.PrepareForSending(allowUnicode);

                                wholeView.Parts.Add(resource.MimePart);
                            }
                            viewsPart.Parts.Add(wholeView);
                        }
                        else
                        {
                            viewsPart.Parts.Add(view.MimePart);
                        }
                    }
                }

                if (Attachments.Count > 0)
                {
                    part = new MimeMultiPart(MimeMultiPartType.Mixed);
                    part.Parts.Add(viewsPart);

                    MimeMultiPart attachmentsPart = new MimeMultiPart(MimeMultiPartType.Mixed);
                    foreach (Attachment attachment in Attachments)
                    {
                        if (attachment != null)
                        {
                            //ensure we can read from the stream.
                            attachment.PrepareForSending(allowUnicode);
                            attachmentsPart.Parts.Add(attachment.MimePart);
                        }
                    }
                    part.Parts.Add(attachmentsPart);
                    _message.Content = part;
                }
                // If there is no Attachement, AND only "1" Alternate View AND !!no body!!
                // then in fact, this is NOT a multipart region.
                else if (viewsPart.Parts.Count == 1 && string.IsNullOrEmpty(_body))
                {
                    _message.Content = viewsPart.Parts[0];
                }
                else
                {
                    _message.Content = viewsPart;
                }
            }

            if (_bodyView != null && _bodyTransferEncoding != TransferEncoding.Unknown)
            {
                _bodyView.TransferEncoding = _bodyTransferEncoding;
            }
        }

        internal void Send(BaseWriter writer, bool sendEnvelope, bool allowUnicode)
        {
            SetContent(allowUnicode);
            _message.Send(writer, sendEnvelope, allowUnicode);
        }

        internal IAsyncResult BeginSend(BaseWriter writer, bool sendEnvelope, bool allowUnicode,
            AsyncCallback callback, object state)
        {
            SetContent(allowUnicode);
            return _message.BeginSend(writer, sendEnvelope, allowUnicode, callback, state);
        }

        internal void EndSend(IAsyncResult asyncResult)
        {
            _message.EndSend(asyncResult);
        }

        internal string BuildDeliveryStatusNotificationString()
        {
            if (_deliveryStatusNotification != DeliveryNotificationOptions.None)
            {
                StringBuilder s = new StringBuilder(" NOTIFY=");

                bool oneSet = false;

                //none
                if (_deliveryStatusNotification == DeliveryNotificationOptions.Never)
                {
                    s.Append("NEVER");
                    return s.ToString();
                }

                if ((((int)_deliveryStatusNotification) & (int)DeliveryNotificationOptions.OnSuccess) > 0)
                {
                    s.Append("SUCCESS");
                    oneSet = true;
                }
                if ((((int)_deliveryStatusNotification) & (int)DeliveryNotificationOptions.OnFailure) > 0)
                {
                    if (oneSet)
                    {
                        s.Append(",");
                    }
                    s.Append("FAILURE");
                    oneSet = true;
                }
                if ((((int)_deliveryStatusNotification) & (int)DeliveryNotificationOptions.Delay) > 0)
                {
                    if (oneSet)
                    {
                        s.Append(",");
                    }
                    s.Append("DELAY");
                }
                return s.ToString();
            }
            return string.Empty;
        }
    }
}
