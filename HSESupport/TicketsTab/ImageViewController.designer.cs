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
    [Register ("ImageViewController")]
    partial class ImageViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView Activity { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ImageView { get; set; }

        [Action ("DoneButton_Pressed:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void DoneButton_Pressed (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (Activity != null) {
                Activity.Dispose ();
                Activity = null;
            }

            if (ImageView != null) {
                ImageView.Dispose ();
                ImageView = null;
            }
        }
    }
}