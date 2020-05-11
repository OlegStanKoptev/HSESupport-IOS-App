using Foundation;
using System;
using System.Collections.Generic;
using System.Threading;
using UIKit;

namespace HSESupport
{
    public partial class AlertsTableViewController : UITableViewController
    {
        public AlertsTableViewController(IntPtr handle) : base(handle)
        {
            searchBar = new UISearchBar()
            {
                Placeholder = "Enter user's name",
                Frame = new CoreGraphics.CGRect(0, 0, View.Frame.Width, 54),
                ShowsCancelButton = true
            };
        }
        UISearchBar searchBar;
        int index = 1;
        public bool IfFromNews { get; set; } = false;
        partial void CloseAlertsModule(UIBarButtonItem sender)
        {
            DismissModalViewController(true);
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (Profile.Instance != null && (Profile.Instance.Status == "Student" || IfFromNews))
            {
                NavigationItem.Title = "Alerts";
                TableView.AllowsSelection = false;
            }
            else
            {
                NavigationItem.Title = "Search";
                TableView.AllowsSelection = true;
            }
            TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            Console.WriteLine(ParentViewController);


            if (Profile.Instance != null && (Profile.Instance.Status == "Student" || IfFromNews))
            {
                TableView.RowHeight = UITableView.AutomaticDimension;
                TableView.EstimatedRowHeight = 113f;
                TableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
                new Thread(new ThreadStart(async () =>
                {
                    List<Alert> alerts = await RemoteService.GetAlerts(Profile.Instance.Email);
                    InvokeOnMainThread(() =>
                    {
                        if (alerts != null)
                        {
                            TableView.Source = new AlertsTableSource(alerts);
                        }
                    });
                })).Start();
            }
            else
            {
                TableView.TableHeaderView = searchBar;
                searchBar.SearchButtonClicked += (sender, e) =>
                {
                    Console.WriteLine("You clicked on a search button " + index++);
                    TableView.RowHeight = 76;
                    TableView.SeparatorStyle = UITableViewCellSeparatorStyle.SingleLine;
                    string request = searchBar.Text;
                    new Thread(new ThreadStart(async () =>
                    {
                        List<Profile> users = await RemoteService.FindProfilesWithNameOrEmail(request);
                        InvokeOnMainThread(() =>
                        {
                            if (users != null)
                            {
                                TableView.Source = new UsersTableSource(users, this);
                                TableView.ReloadData();
                            }
                        });
                    })).Start();
                };
                searchBar.CancelButtonClicked += (sender, e) =>
                {
                    searchBar.Text = "";
                    Console.WriteLine("You pressed cancel button " + index++);
                    searchBar.ResignFirstResponder();
                    TableView.Source = new UsersTableSource(new List<Profile>(), this);
                    TableView.ReloadData();
                    TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
                };
            }
        }
        public void PushUserView(Profile profile)
        {
            UIStoryboard storyboard = UIStoryboard.FromName("Main", null);
            TicketInfoViewController ticketInfoViewController = storyboard.InstantiateViewController("TicketInfoViewController") as TicketInfoViewController;
            if (ticketInfoViewController != null)
            {
                ticketInfoViewController.SetUser(profile, (UINavigationController)ParentViewController);
                ((UINavigationController)ParentViewController).PushViewController(ticketInfoViewController, true);
            }
        }
    }
}