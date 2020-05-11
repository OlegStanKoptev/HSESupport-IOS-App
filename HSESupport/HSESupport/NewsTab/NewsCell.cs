using System;
using CoreGraphics;
using Foundation;
using UIKit;
using FFImageLoading;
using CoreAnimation;

namespace HSESupport
{
    public class NewsCell : UITableViewCell
    {
        protected UILabel Title;
        protected UIImageView imageView;
        CAGradientLayer gradientLayer;
        const int cellOffset = 20;

        public NewsCell(string cellId, bool gradient) : base(UITableViewCellStyle.Default, cellId)
        {
            SelectionStyle = UITableViewCellSelectionStyle.Gray;
            ContentView.BackgroundColor = UIColor.SystemBackgroundColor;
            imageView = new UIImageView();
            imageView.Layer.CornerRadius = 14;
            imageView.ClipsToBounds = true;
            imageView.ContentMode = UIViewContentMode.ScaleAspectFill;

            gradientLayer = new CAGradientLayer();
            gradientLayer.Colors = new CGColor[] { UIColor.Clear.CGColor, UIColor.Black.CGColor};
            gradientLayer.Locations = new NSNumber[] { 0.0, 1.0 };

            if (gradient)
            {
                imageView.Layer.AddSublayer(gradientLayer);
            }

            Title = new UILabel()
            {
                Font = UIFont.PreferredHeadline,
                TextColor = UIColor.FromRGB(255, 255, 255),
                Lines = 3
            };
            ContentView.AddSubviews(new UIView[] { imageView, Title });

        }
        public void UpdateCell(string caption, string imageName)
        {
            if (imageName.Contains("http"))
            {
                try
                {
                    ImageService.Instance.LoadUrl(imageName).DownSample(height: 200).Into(imageView);
                } catch (Exception)
                {
                    imageView.Image = UIImage.FromFile("Images/hse_logo.jpg");
                }
            }
            else
            {
                imageView.Image = UIImage.FromFile("Images/" + imageName);
            }
            Title.Text = caption;
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            imageView.Frame = new CGRect(cellOffset, cellOffset / 2, ContentView.Bounds.Width - cellOffset*2, 200);
            gradientLayer.Frame = new CGRect(0, 0, ContentView.Bounds.Width, ContentView.Bounds.Height);
            Title.Frame = new CGRect(15 + cellOffset, 80 + cellOffset / 2, ContentView.Bounds.Width - 30 - cellOffset*2, 150);
        }
    }
    public class NewsCellEmpty : UITableViewCell
    {
        UIImageView imageView;
        public NewsCellEmpty(string cellId) : base(UITableViewCellStyle.Default, cellId)
        {
            SelectionStyle = UITableViewCellSelectionStyle.Gray;
            ContentView.BackgroundColor = UIColor.SystemBackgroundColor;
            imageView = new UIImageView();
            imageView.Hidden = true;
        }
        public void UpdateCell()
        {
        }
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            imageView.Frame = new CGRect(0, 0, ContentView.Bounds.Width, ContentView.Bounds.Height);
        }
    }
}
