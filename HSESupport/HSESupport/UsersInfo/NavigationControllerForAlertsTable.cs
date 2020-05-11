using Foundation;
using System;
using UIKit;

namespace HSESupport
{
    public partial class NavigationControllerForAlertsTable : UINavigationController
    {
        public NavigationControllerForAlertsTable (IntPtr handle) : base (handle)
        {
        }
        public void SetIfFromNews(bool ifFromNews)
        {
            var rootController = ChildViewControllers[0] as AlertsTableViewController;
            rootController.IfFromNews = ifFromNews;
        }
    }
}