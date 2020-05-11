using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace HSESupport
{
    public class UsersTableSource : UITableViewSource
    {
        public string CellId { get => "UsersCell"; }
        List<Profile> tableItems;
        AlertsTableViewController View;
        public UsersTableSource(List<Profile> profiles, UIViewController view)
        {
            tableItems = profiles;
            View = (AlertsTableViewController)view;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellId) as UsersTableCell;
            if (cell == null) cell = new UsersTableCell(CellId);
            var item = tableItems[indexPath.Row];
            cell.UpdateCell(item);
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return tableItems.Count;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            View.PushUserView(tableItems[indexPath.Row]);
        }
    }
}
