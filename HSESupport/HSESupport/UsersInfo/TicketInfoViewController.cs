using CoreGraphics;
using Foundation;
using HSESupport.TicketsTab;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UIKit;

namespace HSESupport
{
    public partial class TicketInfoViewController : UIViewController
    {
        UILabel UserInitials;
        Profile User;
        UINavigationController navigationController;
        public TicketInfoViewController(IntPtr handle) : base(handle)
        {
        }
        public void SetUser(Profile user, UINavigationController controller)
        {
            User = user;
            navigationController = controller;
            NavigationItem.Title = "Profile";
        }
        public void PrepareIndependantNav()
        {
            Title = "User Info";
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (x, y) =>
            {
                DismissModalViewController(true);
            });
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            UserInitials = new UILabel()
            {
                TextAlignment = UITextAlignment.Center,
                Frame = new CGRect(0, 0, UserPicture.Frame.Width, UserPicture.Frame.Height),
                TextColor = UIColor.White,
                Font = UIFont.FromDescriptor(UIFont.PreferredTitle1.FontDescriptor, 48f)
            };
            UserPicture.AddSubview(UserInitials);

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (User != null)
            {
                NameLabel.Text = User.Name;
                StatusLabel.Text = User.Status;
                EmailLabel.Text = User.Email;
                string[] name = User.Name.Split(' ');
                UserInitials.Text = char.ToUpper(name[0][0]).ToString() + char.ToUpper(name[1][0]).ToString();
                if (File.Exists(Constants.Images + User.UserId + ".jpg"))
                {
                    UserInitials.Text = string.Empty;
                    UserPicture.Image = UIImage.FromFile(Constants.Images + User.UserId + ".jpg");
                }
                else if (User.HasPicture == 1)
                {
                    new Thread(async () =>
                    {
                        await RemoteService.GetUserPicture(User, Constants.Images);
                        InvokeOnMainThread(() =>
                        {
                            if (File.Exists(Constants.Images + User.UserId + ".jpg"))
                            {
                                UserInitials.Text = string.Empty;
                                UserPicture.Image = UIImage.FromFile(Constants.Images + User.UserId + ".jpg");
                            }
                        });
                    }).Start();
                }
                else
                {
                    UserInitials.Text = char.ToUpper(name[0][0]).ToString() + char.ToUpper(name[1][0]).ToString();
                    UserPicture.Image = UIImage.FromFile("Images/PicBG@3x.png");
                }
            }
            AlertsTicketsSegmentedControl.SelectedSegment = 0;
            ContentTable.RowHeight = UITableView.AutomaticDimension;
            ContentTable.EstimatedRowHeight = 113f;

            new Thread(new ThreadStart(async () =>
            {
                List<Alert> alerts = await RemoteService.GetAlerts(User.Email);
                InvokeOnMainThread(() =>
                {
                    if (alerts != null)
                    {
                        ContentTable.Source = new UserInfoAlertsTicketsTableSource(alerts);
                        ContentTable.ReloadData();
                    }
                });

            })).Start();

        }
        partial void ValueChanged(UISegmentedControl scontrol)
        {
            if (scontrol.SelectedSegment == 0)
            {
                Console.WriteLine("You selected the first item");
                ContentTable.EstimatedRowHeight = 113f;
                new Thread(new ThreadStart(async () =>
                {
                    List<Alert> alerts = await RemoteService.GetAlerts(User.Email);
                    InvokeOnMainThread(() =>
                    {
                        if (alerts != null)
                        {
                            ContentTable.Source = new UserInfoAlertsTicketsTableSource(alerts);
                            ContentTable.ReloadData();
                        }
                    });
                })).Start();
            }
            else
            {
                Console.WriteLine("You selected the second item");
                ContentTable.EstimatedRowHeight = 76f;

                new Thread(new ThreadStart(async () =>
                {
                    List<Ticket> tickets = await RemoteService.GetTickets(User.UserId);
                    InvokeOnMainThread(() =>
                    {
                        if (tickets != null)
                        {
                            ContentTable.Source = new UserInfoAlertsTicketsTableSource(tickets);
                            ContentTable.ReloadData();
                        }
                    });
                })).Start();
            }
        }

        partial void UIButton113674_TouchUpInside(UIButton sender)
        {
            UIStoryboard storyboard = UIStoryboard.FromName("Main", null);
            AlertCreateViewController alertController = storyboard.InstantiateViewController("AlertCreateViewController") as AlertCreateViewController;
            if (alertController != null)
            {
                alertController.SetUser(User, navigationController);
                navigationController.PushViewController(alertController, true);
            }
        }
    }
}