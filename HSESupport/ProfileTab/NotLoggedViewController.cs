using Foundation;
using System;
using System.Threading.Tasks;
using UIKit;
using UserNotifications;

namespace HSESupport
{
    public partial class NotLoggedViewController : UIViewController
    {
        public NotLoggedViewController (IntPtr handle) : base (handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            LogInButton.TouchUpInside += async (x, y) =>
            {
                await RemoteService.NonSilentLogIn();
                if (await RemoteService.LogInTheUser())
                {
                    RemoteService.NeededProfilePageNum = 1;
                    ((ProfileViewController)(ParentViewController.ParentViewController)).PresentContainerView(1);
                    if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                    {
                        UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                            (granted, error) =>
                            {
                                if (granted)
                                    InvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications);
                            });
                    }
                    else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                    {
                        var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                                UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                                new NSSet());

                        UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                        UIApplication.SharedApplication.RegisterForRemoteNotifications();
                    }
                }
            };
        }
    }
}