namespace EduNexus.Shared.Interfaces
{
    public interface IEntityCreation
    {
        DateTime CreatedAt { get; set; }
        Guid? CreatedBy { get; set; } // Nullable for system-created entities
    }
}
