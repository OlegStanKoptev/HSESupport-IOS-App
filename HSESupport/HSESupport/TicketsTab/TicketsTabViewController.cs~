using CoreGraphics;
using Foundation;
using HSESupport.TicketsTab;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace HSESupport.TicketsTab
{
    public partial class TicketsTabViewController : UIViewController
    {
        UIRefreshControl refreshControl;
        public TicketsTabViewController(IntPtr handle) : base(handle)
        {
        }

        UIStoryboard storyboard;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TicketsTable.Source = new TicketsTableSource(this);
            refreshControl = new UIRefreshControl();
            refreshControl.ValueChanged += refreshTable;
            TicketsTable.AddSubview(refreshControl);
            NewTicket.Enabled = false;
            storyboard = UIStoryboard.FromName("Main", null);

        }
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (Profile.Instance != null)
            {
                RemoteService.UpdateMessagesList += UpdateInfo;
                UpdateInfo();
                if (Profile.Instance.Status == "Student")
                {
                    NewTicket.Enabled = true;
                    NewTicket.Title = "New";
                }
                else
                {
                    NewTicket.Enabled = true;
                    NewTicket.Title = "Search";
                }
            }
            else
            {
                NewTicket.Enabled = false;
                RemoteService.Tickets = new List<Ticket>();
                TicketsTable.Source = new TicketsTableSource(this);
                TicketsTable.ReloadData();
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            RemoteService.UpdateMessagesList -= UpdateInfo;
        }
        private void UpdateInfo()
        {
            new Thread(new ThreadStart(async () =>
            {
                await RemoteService.GetInfo();
                InvokeOnMainThread(() =>
                {
                    TicketsTable.Source = new TicketsTableSource(this);
                    TicketsTable.ReloadData();
                });
            }))
                            .Start();
        }

        void refreshTable(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(async () =>
            {
                if (Profile.Instance != null)
                    await RemoteService.GetInfo();
                InvokeOnMainThread(() =>
                {
                    TicketsTable.Source = new TicketsTableSource(this);
                    TicketsTable.ReloadData();
                    refreshControl.EndRefreshing();
                });
            }))
            .Start();
        }

        partial void CreateNewTicket(UIBarButtonItem sender)
        {
            if (Profile.Instance != null && Profile.Instance.Status != "Student")
            {
                UIStoryboard storyboard = UIStoryboard.FromName("Main", null);
                NavigationControllerForAlertsTable vController = storyboard.InstantiateViewController("NavigationControllerForAlertsTable") as NavigationControllerForAlertsTable;
                if (vController != null)
                {
                    PresentViewController(vController, true, null);
                }
            }
            else
            {
                var alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);
                alert.AddAction(UIAlertAction.Create("Internet Connection", UIAlertActionStyle.Default, async (action) => { await CreateSpecificTicket(action); }));
                alert.AddAction(UIAlertAction.Create("Payment", UIAlertActionStyle.Default, async (action) => { await CreateSpecificTicket(action); }));
                alert.AddAction(UIAlertAction.Create("General Question", UIAlertActionStyle.Default, async (action) => { await CreateSpecificTicket(action); }));
                alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
                PresentViewController(alert, true, null);
            }
        }
        async Task CreateSpecificTicket(UIAlertAction action)
        {
            Console.WriteLine("Created new ticket");
            Console.WriteLine(action.Title);
            string currentTime = string.Format($"{DateTime.Now.Day:d2}.{DateTime.Now.Month:d2}.{DateTime.Now.Year} {DateTime.Now.Hour:d2}:{DateTime.Now.Minute:d2}:{DateTime.Now.Second:d2}");
            Ticket ticket = new Ticket()
            {
                UserId = Profile.Instance.UserId,
                Topic = action.Title,
                OpenTime = currentTime,
                Status = "Open",
                LastMessageText = "You created new ticket. Please share your issue!",
                LastMessageTime = currentTime,
                FullName = Profile.Instance.Name
            };
            Ticket updatedTicket = await RemoteService.CreateTicket(ticket);
            if (updatedTicket == null)
            {
                await RemoteService.DeleteTicket(ticket);
                return;
            }
            Message date = new Message()
            {
                Text = $"{DateTime.Now.Day:d2}.{DateTime.Now.Month:d2}.{DateTime.Now.Year}",
                Sender = "System",
                SendTime = currentTime,
                UserId = Profile.Instance.UserId,
                TicketId = updatedTicket.Id
            };
            Message message = new Message()
            {
                Text = "You created new ticket. Please share your issue!",
                Sender = "System",
                SendTime = currentTime,
                UserId = Profile.Instance.UserId,
                TicketId = updatedTicket.Id
            };
            new Thread(new ThreadStart(async () =>
            {
                await RemoteService.SendMessage(date);
                await RemoteService.SendMessage(message);
                InvokeOnMainThread(() =>
                {
                    refreshTable(new object(), new EventArgs());
                    TicketsTable.ReloadData();
                });
            })).Start();
        }

        public void PushChatView(Ticket ticket)
        {
            ChatViewController chatViewController = storyboard.InstantiateViewController("ChatViewController") as ChatViewController;
            if (chatViewController != null)
            {
                chatViewController.Info(ticket.Id);
                if (Profile.Instance != null && Profile.Instance.Status == "Student")
                {
                    chatViewController.NavigationItem.Title = ticket?.Topic;
                }
                else
                {
                    chatViewController.NavigationItem.Title = ticket?.FullName;
                }
                ((UINavigationController)ParentViewController).PushViewController(chatViewController, true);
            }
        }
    }
}