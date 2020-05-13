using CoreGraphics;
using Foundation;
using HSESupport.TicketsTab;
using System;
using System.IO;
using UIKit;

namespace HSESupport
{
    public partial class MainTabViewController : UITabBarController
    {
        public MainTabViewController (IntPtr handle) : base (handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }
    }
}