// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace HSESupport
{
    [Register ("TicketInfoViewController")]
    partial class TicketInfoViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl AlertsTicketsSegmentedControl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        HSESupport.UserInfoAlertsTicketsTableView ContentTable { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel EmailLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel NameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel StatusLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView UserPicture { get; set; }

        [Action ("UIButton113674_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UIButton113674_TouchUpInside (UIKit.UIButton sender);

        [Action ("ValueChanged:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ValueChanged (UIKit.UISegmentedControl sender);

        void ReleaseDesignerOutlets ()
        {
            if (AlertsTicketsSegmentedControl != null) {
                AlertsTicketsSegmentedControl.Dispose ();
                AlertsTicketsSegmentedControl = null;
            }

            if (ContentTable != null) {
                ContentTable.Dispose ();
                ContentTable = null;
            }

            if (EmailLabel != null) {
                EmailLabel.Dispose ();
                EmailLabel = null;
            }

            if (NameLabel != null) {
                NameLabel.Dispose ();
                NameLabel = null;
            }

            if (StatusLabel != null) {
                StatusLabel.Dispose ();
                StatusLabel = null;
            }

            if (UserPicture != null) {
                UserPicture.Dispose ();
                UserPicture = null;
            }
        }
    }
}