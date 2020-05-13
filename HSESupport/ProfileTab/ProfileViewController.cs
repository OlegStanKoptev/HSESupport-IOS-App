using Foundation;
using System;
using System.Threading.Tasks;
using UIKit;

namespace HSESupport
{
    public partial class ProfileViewController : UIViewController
    {

        private ContainerViewController containerViewController;

        public ProfileViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            PresentContainerView(RemoteService.NeededProfilePageNum);
        }
        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "embedContainer")
            {
                containerViewController =
                    segue.DestinationViewController as ContainerViewController;
            }
        }

        public async void PresentContainerView(nint selectedId)
        {
            if (selectedId == 0)
            {
                await containerViewController.PresentFirstViewAsync();
            }
            else if (selectedId == 1)
            {
                await containerViewController.PresentSecondViewAsync();
            }
        }

    }
}