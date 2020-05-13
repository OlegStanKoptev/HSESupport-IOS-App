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
    [Register ("AlertsTableViewController")]
    partial class AlertsTableViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView AlertsTable { get; set; }

        [Action ("CloseAlertsModule:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CloseAlertsModule (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (AlertsTable != null) {
                AlertsTable.Dispose ();
                AlertsTable = null;
            }
        }
    }
}