using EduNexus.Core.IServices.Notification;

namespace EduNexus.Business.Services.Notification
{
    public class EmailNotificationService : INotificationService
    {
        public Task SendMessage(string message)
        {
            // logic email here
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}
