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
    [Register ("UserInfoAlertsTicketsTicketTableCell")]
    partial class UserInfoAlertsTicketsTicketTableCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UILabel HeadLine { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UIImageView Picture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UILabel Time { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UILabel Title { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (HeadLine != null) {
                HeadLine.Dispose ();
                HeadLine = null;
            }

            if (Picture != null) {
                Picture.Dispose ();
                Picture = null;
            }

            if (Time != null) {
                Time.Dispose ();
                Time = null;
            }

            if (Title != null) {
                Title.Dispose ();
                Title = null;
            }
        }
    }
}