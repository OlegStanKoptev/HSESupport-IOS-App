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
            string notificationHubConnection = "";
            string notificationHubName = "";
            Hub = NotificationHubClient.CreateClientFromConnectionString(notificationHubConnection, notificationHubName);
        }
    }
}
