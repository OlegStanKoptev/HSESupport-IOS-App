using CoreGraphics;
using FFImageLoading;
using Foundation;
using HSESupport.TicketsTab;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace HSESupport.TicketsTab
{
    public partial class ChatViewController : UIViewController
    {
        UIImageView TypingBG;
        UIImageView Line;

        UITextField textField = new UITextField();
        UIButton sendButton = new UIButton();

        UIBarButtonItem moreButton;
        int ticketId;
        Ticket ticket;
        List<Message> messages;
        Random gnr = new Random();
        int minimumAmountOfMessagesToScroll = 3;


        public ChatViewController(IntPtr handle) : base(handle)
        {
        }


        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ParentViewController.TabBarController.TabBar.Hidden = true;
            View.AddSubviews(textField, sendButton);
            textField.Placeholder = "Enter your message here";
            textField.BorderStyle = UITextBorderStyle.RoundedRect;
            sendButton.SetTitle("Send", UIControlState.Normal);
            sendButton.SetTitleColor(UIColor.LinkColor, UIControlState.Normal);
            sendButton.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
            textField.Frame = new CGRect(8, View.Frame.Height - 75, View.Frame.Width - 16 - 60, 30);
            textField.Text = "";
            sendButton.Frame = new CGRect(View.Frame.Width - 60, View.Frame.Height - 75, 50, 30);
            sendButton.Enabled = false;
            View.BringSubviewToFront(textField);
            View.BringSubviewToFront(sendButton);
            sendButton.TouchUpInside += delegate (object sender, EventArgs args) { Send(); };
            textField.EditingChanged += (x, y) => { if (textField.Text.Length != 0) sendButton.Enabled = true; else sendButton.Enabled = false; };
            RemoteService.UpdateMessagesList += UpdateInfo;
            UpdateLocalMessages();
            UpdateEnteringPossibility();
            Constants.ChatIsPresented = true;
            MessagesTableView.Frame = new CGRect(0, 0, View.Frame.Width, View.Frame.Height - 83);
            MessagesTableView.Source = new MessagesTableSource(messages, textField, this);
            MessagesTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            MessagesTableView.RowHeight = UITableView.AutomaticDimension;
            MessagesTableView.EstimatedRowHeight = 50;
            MessagesTableView.ContentInset = new UIEdgeInsets(10, 0, 10, 0);
            if (messages.Count > minimumAmountOfMessagesToScroll)
                MessagesTableView.ScrollToRow(NSIndexPath.FromItemSection((nint)messages.Count - 1, 0), UITableViewScrollPosition.Bottom, false);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            ParentViewController.TabBarController.TabBar.Hidden = false;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            Constants.ChatIsPresented = false;
            RemoteService.UpdateMessagesList -= UpdateInfo;

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Setup();
            moreButton = new UIBarButtonItem("More", UIBarButtonItemStyle.Plain, (x, y) => { MoreActions(); });
            NavigationItem.RightBarButtonItem = moreButton;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (messages.Count > minimumAmountOfMessagesToScroll)
                MessagesTableView.ScrollToRow(NSIndexPath.FromItemSection((nint)messages.Count - 1, 0), UITableViewScrollPosition.Bottom, true);
        }

        private void UpdateEnteringPossibility()
        {
            if (ticket?.Status != "Closed")
            {
                textField.Placeholder = "Enter your message here";
                textField.Enabled = true;
            }
            else
            {
                textField.Placeholder = "Ticket is closed";
                textField.Enabled = false;
            }
        }

        private void MoreActions()
        {
            textField.ResignFirstResponder();
            DateTime dtime = DateTime.ParseExact(ticket.OpenTime, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            var alert = UIAlertController.Create("Ticket credentials", $"Created: {dtime}, Status: {ticket.Status}", UIAlertControllerStyle.ActionSheet);
            alert.AddAction(UIAlertAction.Create("Send a photo", UIAlertActionStyle.Default, (action) =>
            {
                ChoosePicture_Pressed();
            }));
            if (Profile.Instance.Status != "Student")
            {
                alert.AddAction(UIAlertAction.Create("Change ticket status", UIAlertActionStyle.Default, (action) => { ChangeTicketStatus(); }));
                alert.AddAction(UIAlertAction.Create("View profile information", UIAlertActionStyle.Default, (action) =>
                {
                    UIStoryboard storyboard = UIStoryboard.FromName("Main", null);
                    TempNavigationController vController = storyboard.InstantiateViewController("TempNavigationController") as TempNavigationController;
                    if (vController != null)
                    {
                        new Thread(new ThreadStart(async () =>
                        {
                            Profile profile = await RemoteService.FindProfileWithId(ticket.UserId);
                            InvokeOnMainThread(() =>
                            {
                                if (profile != null)
                                {
                                    vController.SetUser(profile);
                                    ((UINavigationController)ParentViewController).PresentViewController(vController, true, null);
                                }
                            });
                        })).Start();
                    }
                }));
            }
            alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, true, null);
        }

        UIImagePickerController picker;
        void ChoosePicture_Pressed()
        {
            picker = new UIImagePickerController();
            picker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            picker.MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary);
            picker.FinishedPickingMedia += Finished;
            picker.Canceled += Canceled;
            PresentViewController(picker, true, null);
        }

        public void Finished(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            bool isImage = false;
            switch (e.Info[UIImagePickerController.MediaType].ToString())
            {
                case "public.image":
                    isImage = true;
                    break;
                case "public.video":
                    break;
            }
            NSUrl referenceURL = e.Info[new NSString("UIImagePickerControllerReferenceUrl")] as NSUrl;
            if (referenceURL != null) Console.WriteLine("Url:" + referenceURL.ToString());
            if (isImage)
            {
                GetAndSendTheImage(e);
            }
            else
            {
                NSUrl mediaURL = e.Info[UIImagePickerController.MediaURL] as NSUrl;
                if (mediaURL != null)
                {
                    Console.WriteLine(mediaURL.ToString());
                }
            }
            picker.DismissModalViewController(true);
        }

        private void GetAndSendTheImage(UIImagePickerMediaPickedEventArgs e)
        {
            UIImage originalImage = e.Info[UIImagePickerController.OriginalImage] as UIImage;
            if (originalImage != null)
            {
                new Thread(new ThreadStart(async () =>
                {
                    StringBuilder fileName = new StringBuilder();
                    for (int i = 0; i <= 32; i++)
                    {
                        if (i == 8 || i == 16)
                        {
                            fileName.Append('-');
                            i++;
                        }
                        if (gnr.NextDouble() < 0.3)
                        {
                            fileName.Append(Math.Floor(gnr.NextDouble() * 10));
                        }
                        else
                        {
                            fileName.Append((char)Convert.ToInt32(Math.Floor(gnr.NextDouble() * 26) + 'a'));
                        }
                    }
                    Message message = new Message()
                    {
                        Text = fileName.ToString(),
                        Sender = Profile.Instance.Status,
                        SendTime = string.Format($"{DateTime.Now.Day:d2}.{DateTime.Now.Month:d2}.{DateTime.Now.Year} {DateTime.Now.Hour:d2}:{DateTime.Now.Minute:d2}:{DateTime.Now.Second:d2}"),
                        UserId = messages[0].UserId,
                        TicketId = messages[0].TicketId,
                        Type = "Picture"
                    };
                    originalImage.AsJPEG().Save(Constants.Images + fileName.ToString() + ".jpg", false);
                    UIImage image = await ImageService.Instance.LoadFile(Constants.Images + fileName.ToString() + ".jpg")
                                            .DownSample(width: 385)
                                            .AsUIImageAsync();
                    image.AsJPEG().Save(Constants.Images + fileName.ToString() + ".jpg", false);
                    Ticket ticket = RemoteService.Tickets.Find(x => x.Id == message.TicketId);
                    ticket.LastMessageText = "Picture";
                    ticket.LastMessageTime = message.SendTime;
                    await RemoteService.UpdateTicketInfo(ticket);
                    DateTime previousMessageTime = DateTime.ParseExact(messages[messages.Count - 1].SendTime, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    Message dateMessage = null;
                    if (previousMessageTime.Date != DateTime.Today)
                    {
                        dateMessage = new Message()
                        {
                            Text = $"{DateTime.Now.Day:d2}.{DateTime.Now.Month:d2}.{DateTime.Now.Year}",
                            Sender = "System",
                            SendTime = string.Format($"{DateTime.Now.Day:d2}.{DateTime.Now.Month:d2}.{DateTime.Now.Year} {DateTime.Now.Hour:d2}:{DateTime.Now.Minute:d2}:{DateTime.Now.Second:d2}"),
                            UserId = messages[0].UserId,
                            TicketId = messages[0].TicketId,
                            Type = "Text"
                        };
                        await RemoteService.SendMessage(dateMessage);
                    }
                    await RemoteService.SendPictureAsAttachment(message, Profile.Instance, Constants.Images + fileName.ToString() + ".jpg");
                    if (File.Exists(Constants.Images + fileName.ToString() + ".jpg"))
                    {
                        File.Delete(Constants.Images + fileName.ToString() + ".jpg");
                    }
                    await RemoteService.UpdateMessages();
                    UpdateLocalMessages();
                    InvokeOnMainThread(() =>
                    {
                        MessagesTableView.Source = new MessagesTableSource(messages, textField, this);
                        MessagesTableView.ReloadData();
                        if (messages.Count > minimumAmountOfMessagesToScroll)
                            MessagesTableView.ScrollToRow(NSIndexPath.FromItemSection((nint)messages.Count - 1, 0), UITableViewScrollPosition.Bottom, true);
                    });
                })).Start();
            }
        }

        void Canceled(object sender, EventArgs e)
        {
            picker.DismissModalViewController(true);
        }

        public void ShowPicture(string link)
        {
            UIStoryboard storyboard = UIStoryboard.FromName("Main", null);
            ImageViewController imageViewController = storyboard.InstantiateViewController("ImageViewController") as ImageViewController;
            if (imageViewController != null)
            {
                imageViewController.SetPicture(link);
                PresentModalViewController(imageViewController, true);
            }
        }
        private void ChangeTicketStatus()
        {
            var alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);
            if (ticket.Status != "Open")
            {
                alert.AddAction(UIAlertAction.Create("Open", UIAlertActionStyle.Default, async (action) =>
                {
                    await RemoteService.ChangeTicketStatus(ticket, "Open");
                    UpdateEnteringPossibility();
                }));
            }
            if (ticket.Status != "Pending")
            {
                alert.AddAction(UIAlertAction.Create("Pending", UIAlertActionStyle.Default, async (action) =>
                {
                    await RemoteService.ChangeTicketStatus(ticket, "Pending");
                    UpdateEnteringPossibility();
                }));
            }
            if (ticket.Status != "Closed")
            {
                alert.AddAction(UIAlertAction.Create("Closed", UIAlertActionStyle.Default, async (action) =>
                {
                    await RemoteService.ChangeTicketStatus(ticket, "Closed");
                    UpdateEnteringPossibility();
                }));
            }
            alert.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
            PresentViewController(alert, true, null);
        }

        private void UpdateLocalMessages()
        {
            messages = new List<Message>();
            foreach (var message in RemoteService.Messages)
            {
                if (message.TicketId == ticketId)
                {
                    messages.Add(message);
                }
            }
            ticket = RemoteService.Tickets.FirstOrDefault(x => x.Id == ticketId);
        }

        double delta = 0;
        void CallbackWill(object sender, UIKeyboardEventArgs args)
        {

            delta = args.FrameBegin.Top - args.FrameEnd.Top;
            if (delta > 0)
            {
                TextBoxBackgroundFrames(View.Frame.Height - 83 - delta + 38);
                textField.Frame = new CGRect(8, View.Frame.Height - 75 - delta + 37, View.Frame.Width - 16 - 60, 30);
                sendButton.Frame = new CGRect(View.Frame.Width - 60, View.Frame.Height - 75 - delta + 37, sendButton.Frame.Width, sendButton.Frame.Height);
            }
            else if (delta == 0) { }
            else
            {
                MessagesTableView.Frame = new CGRect(MessagesTableView.Frame.X, MessagesTableView.Frame.Y, View.Frame.Width, View.Frame.Height - 83);
                TextBoxBackgroundFrames(View.Frame.Height - 83);
                textField.Frame = new CGRect(8, View.Frame.Height - 75, View.Frame.Width - 16 - 60, 30);
                sendButton.Frame = new CGRect(View.Frame.Width - 60, View.Frame.Height - 75, sendButton.Frame.Width, sendButton.Frame.Height);
            }


        }

        void CorrectMessagesList(object sender, UIKeyboardEventArgs args)
        {
            if (delta > 0)
            {
                MessagesTableView.Frame = new CGRect(MessagesTableView.Frame.X, MessagesTableView.Frame.Y, View.Frame.Width, MessagesTableView.Frame.Height - delta + 38);
                if (messages.Count > minimumAmountOfMessagesToScroll)
                    MessagesTableView.ScrollToRow(NSIndexPath.FromItemSection((nint)messages.Count - 1, 0), UITableViewScrollPosition.Bottom, true);
            }
            else if (delta == 0) { }
            else
            {
            }

        }
#pragma warning disable IDE0052
        NSObject firstNotification;
        NSObject secondNotification;
#pragma warning restore IDE0052
        void Setup()
        {
            firstNotification = UIKeyboard.Notifications.ObserveWillChangeFrame(CallbackWill);
            secondNotification = UIKeyboard.Notifications.ObserveDidChangeFrame(CorrectMessagesList);
        }

        public void Info(int ticketId)
        {
            this.ticketId = ticketId;
            TextBoxBackground();
        }

        private void TextBoxBackground()
        {
            TypingBG = new UIImageView()
            {
                BackgroundColor = UIColor.SecondarySystemBackgroundColor,
                Alpha = (nfloat)0.75,
                Frame = new CGRect(0, View.Frame.Height - 83, View.Frame.Width, 85),
            };
            View.Add(TypingBG);

            blurView = new UIVisualEffectView(blur)
            {
                Frame = new CGRect(0, View.Frame.Height - 83, View.Frame.Width, 85)
            };
            View.Add(blurView);

            Line = new UIImageView()
            {
                BackgroundColor = UIColor.LightGray,
                Frame = new CGRect(0, View.Frame.Height - 83 - 1, View.Frame.Width, 0.5),
            };
            View.Add(Line);
        }

        readonly UIBlurEffect blur = UIBlurEffect.FromStyle(UIBlurEffectStyle.ExtraLight);
        UIVisualEffectView blurView;
        private void TextBoxBackgroundFrames(double height)
        {
            TypingBG.Frame = new CGRect(0, height, View.Frame.Width, 85);
            blurView.Frame = new CGRect(0, height, View.Frame.Width, 85);
            Line.Frame = new CGRect(0, height - 1, View.Frame.Width, 0.5);
        }

        void Send()
        {
            sendButton.Enabled = false;
            DateTime previousMessageTime = DateTime.ParseExact(messages[messages.Count - 1].SendTime, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            Message dateMessage = null;
            if (previousMessageTime.Date != DateTime.Today)
            {
                dateMessage = new Message()
                {
                    Text = $"{DateTime.Now.Day:d2}.{DateTime.Now.Month:d2}.{DateTime.Now.Year}",
                    Sender = "System",
                    SendTime = string.Format($"{DateTime.Now.Day:d2}.{DateTime.Now.Month:d2}.{DateTime.Now.Year} {DateTime.Now.Hour:d2}:{DateTime.Now.Minute:d2}:{DateTime.Now.Second:d2}"),
                    UserId = messages[0].UserId,
                    TicketId = messages[0].TicketId,
                    Type = "Text"
                };
            }
            Message message = new Message()
            {
                Text = textField.Text,
                Sender = Profile.Instance.Status,
                SendTime = string.Format($"{DateTime.Now.Day:d2}.{DateTime.Now.Month:d2}.{DateTime.Now.Year} {DateTime.Now.Hour:d2}:{DateTime.Now.Minute:d2}:{DateTime.Now.Second:d2}"),
                UserId = messages[0].UserId,
                TicketId = messages[0].TicketId,
                Type = "Text"
            };
            textField.Text = "";
            Ticket ticket = RemoteService.Tickets.Find(x => x.Id == message.TicketId);
            ticket.LastMessageText = message.Text;
            ticket.LastMessageTime = message.SendTime;
            new Thread(new ThreadStart(async () =>
            {
                if (dateMessage != null)
                    await RemoteService.SendMessage(dateMessage);
                await RemoteService.SendMessage(message);
                await RemoteService.UpdateTicketInfo(ticket);
                UpdateInfo();
                InvokeOnMainThread(() =>
                {
                    sendButton.Enabled = true;
                });
            }))
            .Start();
        }

        private void UpdateInfo()
        {
            new Thread(new ThreadStart(async () =>
            {
                await RemoteService.GetInfo();
                UpdateLocalMessages();
                InvokeOnMainThread(() =>
                {
                    MessagesTableView.Source = new MessagesTableSource(messages, textField, this);
                    MessagesTableView.ReloadData();
                    if (messages.Count > minimumAmountOfMessagesToScroll)
                        MessagesTableView.ScrollToRow(NSIndexPath.FromItemSection((nint)messages.Count - 1, 0), UITableViewScrollPosition.Bottom, true);
                });
            })).Start();
        }
    }
}