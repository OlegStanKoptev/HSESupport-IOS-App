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
    [Register ("ChatTableCellUser")]
    partial class ChatTableCellUser
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView Bubble { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Content { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel Time { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (Bubble != null) {
                Bubble.Dispose ();
                Bubble = null;
            }

            if (Content != null) {
                Content.Dispose ();
                Content = null;
            }

            if (Time != null) {
                Time.Dispose ();
                Time = null;
            }
        }
    }
}