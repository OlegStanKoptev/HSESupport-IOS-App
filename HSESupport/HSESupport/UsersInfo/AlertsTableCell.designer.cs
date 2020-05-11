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
    [Register ("AlertsTableCell")]
    partial class AlertsTableCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UILabel Content { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UIImageView Picture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UILabel TimeStamp { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        public UIKit.UILabel Title { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (Content != null) {
                Content.Dispose ();
                Content = null;
            }

            if (Picture != null) {
                Picture.Dispose ();
                Picture = null;
            }

            if (TimeStamp != null) {
                TimeStamp.Dispose ();
                TimeStamp = null;
            }

            if (Title != null) {
                Title.Dispose ();
                Title = null;
            }
        }
    }
}