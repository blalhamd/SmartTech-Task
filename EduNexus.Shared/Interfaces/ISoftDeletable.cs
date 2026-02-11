namespace EduNexus.Shared.Interfaces
{
    public interface ISoftDeletable
    {
        DateTime? DeletedAt { get; set; }
        Guid? DeletedBy { get; set; }
        bool IsDeleted { get; set; }
    }
}
