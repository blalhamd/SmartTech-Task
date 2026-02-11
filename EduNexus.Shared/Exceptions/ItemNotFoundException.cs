using System.Runtime.Serialization;
namespace EduNexus.Shared.Exceptions
{
    /// <summary>
    /// Thrown when a specific requested item (e.g., a user, a course) cannot be found.
    /// </summary>
    [Serializable]
    public class ItemNotFoundException : BaseException
    {
        public ItemNotFoundException()
            : base("The requested item was not found.")
        {
        }

        public ItemNotFoundException(string message)
            : base(message)
        {
        }

        public ItemNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// A helper constructor to create a standardized "not found" message.
        /// </summary>
        /// <param name="itemName">The name of the entity type (e.g., "Student", "Course").</param>
        /// <param name="key">The ID or key used for the lookup.</param>
        public ItemNotFoundException(string itemName, object key)
            : base($"The item '{itemName}' with key '{key}' was not found.")
        {
        }

        protected ItemNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
