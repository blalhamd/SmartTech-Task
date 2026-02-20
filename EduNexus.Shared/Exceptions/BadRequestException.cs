using System.Runtime.Serialization;
namespace EduNexus.Shared.Exceptions
{
    /// <summary>
    /// Thrown when a request is malformed, invalid, or cannot be processed.
    /// Corresponds to an HTTP 400 Bad Request.
    /// </summary>
    [Serializable]
    public class BadRequestException : BaseException
    {
        public BadRequestException()
            : base("The request was invalid or malformed.")
        {
        }

        public BadRequestException(string message)
            : base(message)
        {
        }

        public BadRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

    }
}
