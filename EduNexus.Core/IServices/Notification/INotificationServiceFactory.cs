namespace EduNexus.Core.IServices.Notification
{
    public interface INotificationServiceFactory
    {
        INotificationService Create(string type);
    }
}
