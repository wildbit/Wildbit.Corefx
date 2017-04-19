// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Wildbit.Corefx.Mail
{
    [Serializable]
    public class SmtpException : Exception, ISerializable
    {
        private SmtpStatusCode _statusCode = SmtpStatusCode.GeneralFailure;

        private static string GetMessageForStatus(SmtpStatusCode statusCode, string serverResponse)
        {
            return GetMessageForStatus(statusCode) + " " + string.Format(Strings.MailServerResponse, serverResponse);
        }

        private static string GetMessageForStatus(SmtpStatusCode statusCode)
        {
            switch (statusCode)
            {
                default:
                case SmtpStatusCode.CommandUnrecognized:
                    return string.Format(Strings.SmtpCommandUnrecognized);
                case SmtpStatusCode.SyntaxError:
                    return string.Format(Strings.SmtpSyntaxError);
                case SmtpStatusCode.CommandNotImplemented:
                    return string.Format(Strings.SmtpCommandNotImplemented);
                case SmtpStatusCode.BadCommandSequence:
                    return string.Format(Strings.SmtpBadCommandSequence);
                case SmtpStatusCode.CommandParameterNotImplemented:
                    return string.Format(Strings.SmtpCommandParameterNotImplemented);
                case SmtpStatusCode.SystemStatus:
                    return string.Format(Strings.SmtpSystemStatus);
                case SmtpStatusCode.HelpMessage:
                    return string.Format(Strings.SmtpHelpMessage);
                case SmtpStatusCode.ServiceReady:
                    return string.Format(Strings.SmtpServiceReady);
                case SmtpStatusCode.ServiceClosingTransmissionChannel:
                    return string.Format(Strings.SmtpServiceClosingTransmissionChannel);
                case SmtpStatusCode.ServiceNotAvailable:
                    return string.Format(Strings.SmtpServiceNotAvailable);
                case SmtpStatusCode.Ok:
                    return string.Format(Strings.SmtpOK);
                case SmtpStatusCode.UserNotLocalWillForward:
                    return string.Format(Strings.SmtpUserNotLocalWillForward);
                case SmtpStatusCode.MailboxBusy:
                    return string.Format(Strings.SmtpMailboxBusy);
                case SmtpStatusCode.MailboxUnavailable:
                    return string.Format(Strings.SmtpMailboxUnavailable);
                case SmtpStatusCode.LocalErrorInProcessing:
                    return string.Format(Strings.SmtpLocalErrorInProcessing);
                case SmtpStatusCode.UserNotLocalTryAlternatePath:
                    return string.Format(Strings.SmtpUserNotLocalTryAlternatePath);
                case SmtpStatusCode.InsufficientStorage:
                    return string.Format(Strings.SmtpInsufficientStorage);
                case SmtpStatusCode.ExceededStorageAllocation:
                    return string.Format(Strings.SmtpExceededStorageAllocation);
                case SmtpStatusCode.MailboxNameNotAllowed:
                    return string.Format(Strings.SmtpMailboxNameNotAllowed);
                case SmtpStatusCode.StartMailInput:
                    return string.Format(Strings.SmtpStartMailInput);
                case SmtpStatusCode.TransactionFailed:
                    return string.Format(Strings.SmtpTransactionFailed);
                case SmtpStatusCode.ClientNotPermitted:
                    return string.Format(Strings.SmtpClientNotPermitted);
                case SmtpStatusCode.MustIssueStartTlsFirst:
                    return string.Format(Strings.SmtpMustIssueStartTlsFirst);
            }
        }

        public SmtpException(SmtpStatusCode statusCode) : base(GetMessageForStatus(statusCode))
        {
            _statusCode = statusCode;
        }

        public SmtpException(SmtpStatusCode statusCode, string message) : base(message)
        {
            _statusCode = statusCode;
        }

        public SmtpException() : this(SmtpStatusCode.GeneralFailure)
        {
        }

        public SmtpException(string message) : base(message)
        {
        }

        public SmtpException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SmtpException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
            _statusCode = (SmtpStatusCode)serializationInfo.GetInt32("Status");
        }


        internal SmtpException(SmtpStatusCode statusCode, string serverMessage, bool serverResponse) : base(GetMessageForStatus(statusCode, serverMessage))
        {
            _statusCode = statusCode;
        }

        internal SmtpException(string message, string serverResponse) : base(message + " " + string.Format(Strings.MailServerResponse, serverResponse))
        {
        }

        void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            GetObjectData(serializationInfo, streamingContext);
        }

        public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
            base.GetObjectData(serializationInfo, streamingContext);
            serializationInfo.AddValue("Status", (int)_statusCode, typeof(int));
        }

        public SmtpStatusCode StatusCode
        {
            get
            {
                return _statusCode;
            }
            set
            {
                _statusCode = value;
            }
        }
    }
}
