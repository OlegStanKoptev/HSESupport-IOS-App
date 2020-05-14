using System;
using Microsoft.Azure.NotificationHubs;

namespace HSESupportAPI.Models
{
    public class Notifications
    {
        public static Notifications Instance = new Notifications();

        public NotificationHubClient Hub { get; set; }

        private Notifications()
        {
            string notificationHubConnection = "Endpoint=sb://namespaceformyprojects.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=JieudvKc+VSCDeTjfjyit6HcWBEZW7jDn7Ev0vDDkHU=";
            string notificationHubName = "HSESupportAppNotifHub";
            Hub = NotificationHubClient.CreateClientFromConnectionString(notificationHubConnection, notificationHubName);
        }
    }
}
