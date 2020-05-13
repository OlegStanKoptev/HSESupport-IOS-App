using System;
using UserNotifications;

namespace HSESupport
{
    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {
        public UserNotificationCenterDelegate()
        {
        }

        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification,
            Action<UNNotificationPresentationOptions> completionHandler)
        {
            RemoteService.TriggerUpdateEvent();
            completionHandler(Constants.ChatIsPresented ? UNNotificationPresentationOptions.None : UNNotificationPresentationOptions.Alert);
            //completionHandler(UNNotificationPresentationOptions.Alert);
        }
    }
}