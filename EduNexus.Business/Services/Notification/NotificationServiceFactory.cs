using EduNexus.Core.Constants;
using EduNexus.Core.IServices.Notification;
using Microsoft.Extensions.DependencyInjection;

namespace EduNexus.Business.Services.Notification
{
    public class NotificationServiceFactory : INotificationServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public INotificationService Create(string type)
        {
            switch(type.ToLower())
            {
                case NotificationType.Email:
                    return _serviceProvider.GetRequiredKeyedService<INotificationService>(ServiceKey.Email_Key);
                case NotificationType.SMS:
                    return _serviceProvider.GetRequiredKeyedService<INotificationService>(ServiceKey.SMS_Key);
                default:
                    throw new ArgumentException("UnKnown type");
            }
        }
    }
}
