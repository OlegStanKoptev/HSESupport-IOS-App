using FFImageLoading;
using Foundation;
using System;
using System.IO;
using System.Threading;
using UIKit;

namespace HSESupport
{
    public partial class ImageViewController : UIViewController
    {
        public ImageViewController (IntPtr handle) : base (handle)
        {
        }
        string Name;
        public void SetPicture(string name)
        {
            Name = name;
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Activity.StartAnimating();
            new Thread(new ThreadStart(async () =>
            {
                await RemoteService.GetPictureWithName(Name, Constants.Images);
                InvokeOnMainThread(() =>
                {
                    if (File.Exists(Constants.Images + Name + ".jpg"))
                    {
                        Activity.StopAnimating();
                        ImageView.Image = UIImage.FromFile(Constants.Images + Name + ".jpg");
                    }
                });
            })).Start();
        }
        partial void DoneButton_Pressed(UIBarButtonItem sender)
        {
            DismissModalViewController(true);
        }
    }
}