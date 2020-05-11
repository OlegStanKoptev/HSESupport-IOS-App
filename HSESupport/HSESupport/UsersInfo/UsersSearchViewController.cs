using Foundation;
using System;
using UIKit;

namespace HSESupport
{
    public partial class UsersSearchViewController : UIViewController
    {
        public UsersSearchViewController (IntPtr handle) : base (handle)
        {
            searchBar = new UISearchBar();
        }
        UISearchBar searchBar;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //NavigationItem.TitleView = searchBar;
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            //UsersTable.Source = new UsersTableSource(); 
        }
    }
}