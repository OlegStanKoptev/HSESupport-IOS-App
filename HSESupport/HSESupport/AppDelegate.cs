﻿using System;
using Foundation;
using UIKit;
using WindowsAzure.Messaging;
using UserNotifications;
using AC.Components.Util;
using System.Threading.Tasks;

namespace HSESupport
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIResponder, IUIApplicationDelegate
    {

        [Export("window")]
        public UIWindow Window { get; set; }

        private SBNotificationHub Hub { get; set; }

        private async Task LogInProcessing()
        {
            if (await RemoteService.SilentLogInPossible().ConfigureAwait(false))
            {
                await RemoteService.LogInTheUser().ConfigureAwait(false);

                if (Profile.Instance.HasPicture == 1)
                {
                    await RemoteService.GetUserPicture(Profile.Instance, Constants.Images).ConfigureAwait(false);
                }

                await RemoteService.GetInfo();
            }
        }

        [Export("application:didFinishLaunchingWithOptions:")]
        public bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert |
                    UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                    (granted, error) =>
                    {
                        if (granted)
                            InvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications);
                    });
            }

            UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();

            AsyncUtil.RunSync(() => LogInProcessing());

            return true;
        }

        [Export("application:didRegisterForRemoteNotificationsWithDeviceToken:")]
        public void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {

            Hub = new SBNotificationHub(Constants.ListenConnectionString, Constants.NotificationHubName);
            Hub.UnregisterAll(deviceToken, (error) => {
                if (error != null)
                {
                    Console.WriteLine("Error calling Unregister: {0}", error.ToString());
                    return;
                }
                NSSet tags = null;
                if (Profile.Instance != null)
                {
                    if (Profile.Instance.Status == "Student")
                    {
                        tags = new NSSet("username:" + Profile.Instance.UserId);
                    }
                    else
                    {
                        tags = new NSSet("usertype:admin");
                    }
                    Hub.RegisterNativeAsync(deviceToken, tags);
                }
            });
        }

        [Export("application:didReceiveRemoteNotification:fetchCompletionHandler:")]
        public void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            RemoteService.TriggerUpdateEvent();
        }

        // UISceneSession Lifecycle
        [Export("application:configurationForConnectingSceneSession:options:")]
        public UISceneConfiguration GetConfiguration(UIApplication application, UISceneSession connectingSceneSession, UISceneConnectionOptions options)
        {
            // Called when a new scene session is being created.
            // Use this method to select a configuration to create the new scene with.
            return UISceneConfiguration.Create("Default Configuration", connectingSceneSession.Role);
        }

        [Export("application:didDiscardSceneSessions:")]
        public void DidDiscardSceneSessions(UIApplication application, NSSet<UISceneSession> sceneSessions)
        {
            // Called when the user discards a scene session.
            // If any sessions were discarded while the application was not running, this will be called shortly after `FinishedLaunching`.
            // Use this method to release any resources that were specific to the discarded scenes, as they will not return.
        }
    }
}

