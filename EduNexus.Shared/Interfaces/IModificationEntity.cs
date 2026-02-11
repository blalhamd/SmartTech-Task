namespace EduNexus.Shared.Interfaces
{
    public interface IModificationEntity
    {
        DateTime? UpdatedAt { get; set; }
        Guid? UpdatedBy { get; set; }
    }
}
