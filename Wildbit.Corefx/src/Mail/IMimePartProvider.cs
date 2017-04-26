using Wildbit.Corefx.Mime;

namespace Wildbit.Corefx.Mail
{
    /// <summary>
    /// A mechanism for exposing the underlying Mime structure to construct a mail message.
    /// </summary>
    public interface IMimePartProvider
    {
        /// <summary>
        /// The MimePart that this class provides.
        /// </summary>
        MimeBasePart Part { get; }
    }
}
