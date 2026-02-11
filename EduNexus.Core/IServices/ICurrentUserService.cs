namespace EduNexus.Core.IServices
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
    }
}
