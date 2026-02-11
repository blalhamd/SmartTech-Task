using System.Runtime.Serialization;
namespace EduNexus.Shared.Exceptions
{
    /// <summary>
    /// Thrown when an attempt is made to create an item that already exists.
    /// </summary>
    [Serializable]
    public class ItemAlreadyExistsException : BaseException
    {
        public ItemAlreadyExistsException()
            : base("This item already exists.")
        {
        }

        public ItemAlreadyExistsException(string message)
            : base(message)
        {
        }

        public ItemAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// A helper constructor to create a standardized "already exists" message.
        /// </summary>
        /// <param name="itemName">The name of the entity type (e.g., "User", "Email").</param>
        /// <param name="key">The value that already exists.</param>
        public ItemAlreadyExistsException(string itemName, object key)
            : base($"The item '{itemName}' with value '{key}' already exists.")
        {
        }

        protected ItemAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
