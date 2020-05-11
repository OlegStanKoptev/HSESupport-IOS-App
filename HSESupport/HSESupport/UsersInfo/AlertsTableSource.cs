using System;
using System.Collections.Generic;
using System.Threading;
using Foundation;
using UIKit;

namespace HSESupport
{
    public class AlertsTableSource : UITableViewSource
    {
        public string CellId { get => "AlertCell"; }
        List<Alert> tableItems;
        public AlertsTableSource(List<Alert> alerts)
        {
            tableItems = alerts;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellId, indexPath) as AlertsTableCell;
            var alert = tableItems[indexPath.Row];

            cell.Title.Text = alert.Title;
            cell.Picture.Image = UIImage.FromFile(alert.Picture);
            cell.Content.Text = alert.Text;
            cell.TimeStamp.Text = alert.TimeCreated;

            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return tableItems.Count;
        }
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
        }

        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            switch (editingStyle)
            {
                case UITableViewCellEditingStyle.Delete:
                    Alert alert = tableItems[indexPath.Row];
                    new Thread(new ThreadStart(async () =>
                    {
                        await RemoteService.DeleteAlert(alert.Id);
                    })).Start();
                    tableItems.RemoveAt(indexPath.Row);
                    tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                    break;
                case UITableViewCellEditingStyle.None:
                    Console.WriteLine("CommitEditingStyle:None called");
                    break;
            }
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return (Profile.Instance != null && Profile.Instance.Status != "Student") ? true : false;
        }
    }
}
