namespace EduNexus.Core.IServices.Notification
{
    public interface INotificationService
    {
        Task SendMessage(string message);
    }
}
