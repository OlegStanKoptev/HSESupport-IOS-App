using Foundation;
using System;
using System.Threading;
using UIKit;

namespace HSESupport
{
    public partial class AlertCreateViewController : UIViewController
    {
        public AlertCreateViewController (IntPtr handle) : base (handle)
        {
        }

        Profile User;
        UINavigationController navigationController;
        public void SetUser(Profile user, UINavigationController nav)
        {
            User = user;
            navigationController = nav;
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationItem.Title = "New Alert";
            AlertTitle.Text = "Alert Title";
            AlertTitle.TextColor = UIColor.LightGray;
            Content.Text = "Please enter all information here";
            Content.TextColor = UIColor.LightGray;
            NSNotificationCenter.DefaultCenter.AddObserver(UITextView.TextDidBeginEditingNotification, TextDidBeginEditing);
            NSNotificationCenter.DefaultCenter.AddObserver(UITextView.TextDidEndEditingNotification, TextDidEndEditing);

            if (NavigationItem.LeftBarButtonItem == null)
            {
                NavigationItem.LeftBarButtonItem = new UIBarButtonItem("Close", UIBarButtonItemStyle.Plain, (x, y) =>
                {
                    DismissModalViewController(true);
                });
            }

            NavigationItem.RightBarButtonItem = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (x, y) =>
            {
                string currentTime = string.Format($"{DateTime.Now.Day:d2}.{DateTime.Now.Month:d2}.{DateTime.Now.Year} {DateTime.Now.Hour:d2}:{DateTime.Now.Minute:d2}:{DateTime.Now.Second:d2}");
                string receiver = "all";
                Action<bool> popViewController = DismissModalViewController;
                if (User != null)
                {
                    receiver = User.Email;
                    popViewController = (bool anim) => { navigationController?.PopViewController(anim); };
                }
                if (AlertTitle.Text != "Alert Title" && Content.Text != "Please enter all information here")
                {
                    Alert alert = new Alert()
                    {
                        UserEmail = receiver,
                        Title = AlertTitle.Text,
                        Text = Content.Text,
                        TimeCreated = currentTime,
                        Picture = "Images/hse_round_logo.png"
                    };
                    new Thread(new ThreadStart(async () =>
                    {
                        await RemoteService.PostAlert(alert);
                        InvokeOnMainThread(() =>
                        {
                            popViewController(true);
                        });
                    })).Start();
                }
                else
                {
                    popViewController(true);
                }
            });
        }

        void TextDidBeginEditing(NSNotification notification)
        {
            if (notification.Description.Contains("Alert Title"))
            {
                AlertTitle.Text = string.Empty;
                AlertTitle.TextColor = UIColor.Black;
            }
            else if (notification.Description.Contains("Please enter all informat..."))
            {
                Content.Text = string.Empty;
                Content.TextColor = UIColor.Black;
            }
        }

        void TextDidEndEditing(NSNotification notification)
        {
            if (AlertTitle.Text == string.Empty)
            {
                AlertTitle.Text = "Alert Title";
                AlertTitle.TextColor = UIColor.LightGray;
            }
            if (Content.Text == string.Empty)
            {
                Content.Text = "Please enter all information here";
                Content.TextColor = UIColor.LightGray;
            }
        }
    }
}