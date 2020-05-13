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
    [Register ("LoggedViewController")]
    partial class LoggedViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ChoosePicture { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CreateAlertForAll { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel EmailLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel FullNameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton LogOutButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel StatusLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView UserPicture { get; set; }

        [Action ("ChoosePicture_Pressed:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ChoosePicture_Pressed (UIKit.UIButton sender);

        [Action ("CreateAlertForAll_Pressed:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CreateAlertForAll_Pressed (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ChoosePicture != null) {
                ChoosePicture.Dispose ();
                ChoosePicture = null;
            }

            if (CreateAlertForAll != null) {
                CreateAlertForAll.Dispose ();
                CreateAlertForAll = null;
            }

            if (EmailLabel != null) {
                EmailLabel.Dispose ();
                EmailLabel = null;
            }

            if (FullNameLabel != null) {
                FullNameLabel.Dispose ();
                FullNameLabel = null;
            }

            if (LogOutButton != null) {
                LogOutButton.Dispose ();
                LogOutButton = null;
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