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

namespace HSESupport.TicketsTab
{
    [Register ("TicketsTabViewController")]
    partial class TicketsTabViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem NewTicket { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView TicketsTable { get; set; }

        [Action ("CreateNewTicket:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void CreateNewTicket (UIKit.UIBarButtonItem sender);

        void ReleaseDesignerOutlets ()
        {
            if (NewTicket != null) {
                NewTicket.Dispose ();
                NewTicket = null;
            }

            if (TicketsTable != null) {
                TicketsTable.Dispose ();
                TicketsTable = null;
            }
        }
    }
}