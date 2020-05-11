using Foundation;
using System;
using UIKit;

namespace HSESupport
{
    public partial class TempNavigationController : UINavigationController
    {
        public TempNavigationController (IntPtr handle) : base (handle)
        {
        }

        public void SetUser(Profile user)
        {
            ((TicketInfoViewController)ViewControllers[0]).SetUser(user, this);
            ((TicketInfoViewController)ViewControllers[0]).PrepareIndependantNav();
        }
    }
}