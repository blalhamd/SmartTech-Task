using System.Runtime.Serialization;
namespace EduNexus.Shared.Exceptions
{
    /// <summary>
    /// Thrown when one or more model validation rules fail.
    /// </summary>
    [Serializable]
    public class ValidationException : BaseException
    {
        /// <summary>
        /// A dictionary where the key is the property name and the value
        /// is an array of error messages for that property.
        /// </summary>
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(string message)
            : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
            Errors = new Dictionary<string, string[]>();
        }

        /// <summary>
        /// Creates a new validation exception with a collection of errors.
        /// </summary>
        public ValidationException(IDictionary<string, string[]> errors)
            : this() // Calls the default constructor to set the default message
        {
            Errors = errors;
        }

    }
}
