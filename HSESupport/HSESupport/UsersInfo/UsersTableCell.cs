using System;
using System.IO;
using System.Threading;
using CoreGraphics;
using UIKit;

namespace HSESupport
{
    public class UsersTableCell : UITableViewCell
    {
        protected UILabel Title;
        protected UILabel Email;
        protected UILabel UserInitials;
        protected UIImageView imageView;

        public UsersTableCell(string cellId) : base(UITableViewCellStyle.Default, cellId)
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

            Email = new UILabel()
            {
                Font = UIFont.PreferredHeadline,
                TextColor = UIColor.SecondaryLabelColor
            };

            UserInitials = new UILabel()
            {
                TextAlignment = UITextAlignment.Center,
                TextColor = UIColor.White,
                Font = UIFont.FromDescriptor(UIFont.PreferredTitle1.FontDescriptor, 24f)
            };
            imageView.AddSubview(UserInitials);
            ContentView.AddSubviews(new UIView[] { imageView, Title, Email });

        }
        public void UpdateCell(Profile profile)
        {
            string[] names = profile.Name.Split(' ');
            UserInitials.Text = char.ToUpper(names[0][0]).ToString() + char.ToUpper(names[1][0]);
            imageView.Image = UIImage.FromFile("Images/PicBG@3x.png");
            Title.Text = profile.Name;
            Email.Text = profile.Email;
            if (File.Exists(Constants.Images + profile.UserId + ".jpg"))
            {
                UserInitials.Text = string.Empty;
                imageView.Image = UIImage.FromFile(Constants.Images + profile.UserId + ".jpg");
            }
            else if (profile.HasPicture == 1)
            {
                new Thread(async () =>
                {
                    await RemoteService.GetUserPicture(profile, Constants.Images);
                    InvokeOnMainThread(() =>
                    {
                        UserInitials.Text = string.Empty;
                        imageView.Image = UIImage.FromFile(Constants.Images + profile.UserId + ".jpg");
                    });
                }).Start();
            }
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            imageView.Frame = new CGRect(16, 10, 54, 54);
            UserInitials.Frame = new CGRect(0, 0, imageView.Frame.Width, imageView.Frame.Height);
            Title.Frame = new CGRect(83, ContentView.Bounds.Height / 4 - 6, ContentView.Bounds.Width - 160, 22);
            Email.Frame = new CGRect(83, ContentView.Bounds.Height / 2, ContentView.Bounds.Width - 120, 22);
        }
    }
}
