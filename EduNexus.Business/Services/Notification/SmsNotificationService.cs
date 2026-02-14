using EduNexus.Core.IServices.Notification;

namespace EduNexus.Business.Services.Notification
{
    public class SmsNotificationService : INotificationService
    {
        public Task SendMessage(string message)
        {
            // logic sms here
            Console.WriteLine(message);
            return Task.CompletedTask;
        }
    }
}
