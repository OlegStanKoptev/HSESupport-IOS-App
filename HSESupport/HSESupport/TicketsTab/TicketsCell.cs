﻿using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace HSESupport.TicketsTab
{
    public class TicketsCell : UITableViewCell
    {
        protected UILabel Title;
        protected UILabel Message;
        protected UILabel Time;
        protected UILabel UserInitials;
        protected UIImageView imageView;

        public TicketsCell(string cellId) : base(UITableViewCellStyle.Default, cellId)
        {
            SelectionStyle = UITableViewCellSelectionStyle.Gray;
            ContentView.BackgroundColor = UIColor.SystemBackgroundColor;

            imageView = new UIImageView();
            imageView.Layer.CornerRadius = 54 / 2;
            imageView.ClipsToBounds = true;
            imageView.ContentMode = UIViewContentMode.ScaleAspectFill;

            Title = new UILabel()
            {
                Font = UIFont.PreferredHeadline,
                TextColor = UIColor.LabelColor,
                Lines = 3
            };

            Message = new UILabel()
            {
                Font = UIFont.PreferredHeadline,
                TextColor = UIColor.SecondaryLabelColor
            };

            Time = new UILabel()
            {
                Font = UIFont.PreferredBody,
                TextAlignment = UITextAlignment.Right,
                TextColor = UIColor.SecondaryLabelColor
            };

            UserInitials = new UILabel()
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Font = UIFont.FromDescriptor(UIFont.PreferredTitle1.FontDescriptor, 24f)
            };
            imageView.AddSubview(UserInitials);
            ContentView.AddSubviews(new UIView[] { imageView, Title, Message, Time });

        }
        public void UpdateCell(Ticket ticket)
        {
            if (Profile.Instance != null && Profile.Instance.Status != "Student")
            {
                try
                {
                    if (File.Exists(Constants.Images + ticket.UserId + ".jpg"))
                    {
                        UserInitials.Text = "";
                        imageView.Image = UIImage.FromFile(Constants.Images + ticket.UserId + ".jpg");
                    }
                    else
                    {
                        string[] names = ticket.FullName.Split(' ');
                        UserInitials.Text = char.ToUpper(names[0][0]).ToString() + char.ToUpper(names[1][0]);
                        imageView.Image = UIImage.FromFile("Images/PicBG@3x.png");
                    }
                    Title.Text = ticket.FullName;
                    Message.Text = $"{ImageFromStatus(ticket.Status)} {ticket.Topic}";
                }
                catch (Exception)
                {
                    string[] names = ticket.FullName.Split(' ');
                    UserInitials.Text = char.ToUpper(names[0][0]).ToString() + char.ToUpper(names[1][0]);
                    imageView.Image = UIImage.FromFile("Images/PicBG@3x.png");
                    Title.Text = ticket.FullName;
                    Message.Text = $"{ImageFromStatus(ticket.Status)} {ticket.Topic}";
                }
            }
            else
            {
                UserInitials.Text = "";
                imageView.Image = UIImage.FromFile("Images/hse_round_logo.png");
                Title.Text = ticket.Topic;
                Message.Text = $"{ImageFromStatus(ticket.Status)} {ticket.LastMessageText}";

            }
            DateTime dtime = DateTime.ParseExact(ticket.LastMessageTime, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            if (DateTime.Now.Date == dtime.Date)
            {
                Time.Text = dtime.ToShortTimeString();
            }
            else
            {
                Time.Text = dtime.ToShortDateString();
            }
        }
        private string ImageFromStatus(string status)
        {
            if (status == "Open") return "🟢";
            if (status == "Pending") return "🟡";
            return"🔴";
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            imageView.Frame = new CGRect(16, 10, 54, 54);
            UserInitials.Frame = new CGRect(0, 0, imageView.Frame.Width, imageView.Frame.Height);
            Title.Frame = new CGRect(83, ContentView.Bounds.Height / 4 - 6, ContentView.Bounds.Width - 160, 22);
            Message.Frame = new CGRect(83, ContentView.Bounds.Height / 2, ContentView.Bounds.Width - 120, 22);
            Time.Frame = new CGRect(ContentView.Bounds.Width - 120, ContentView.Bounds.Height / 4 - 6, 100, 22);
        }
    }
}
