using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Essentials;

namespace HSESupport.TicketsTab
{
    public class TicketsTableSource : UITableViewSource
    {
        List<Ticket> TableItems;

        TicketsTabViewController View;

        UIStoryboard storyboard;

        string cellIdentifier = "TicketsTableCell";

        public TicketsTableSource(UIViewController view)
        {
            TableItems = RemoteService.Tickets;
            TableItems.Sort((x, y) =>
            {
                DateTime time1 = DateTime.ParseExact(x.LastMessageTime, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime time2 = DateTime.ParseExact(y.LastMessageTime, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                if (time1 < time2) return 1;
                if (time1 == time2) return 0;
                return -1;
            });
            storyboard = UIStoryboard.FromName("Main", null);
            View = (TicketsTabViewController)view;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return TableItems.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(cellIdentifier) as TicketsCell;
            if (cell == null)
                cell = new TicketsCell(cellIdentifier);
            cell.UpdateCell(TableItems[indexPath.Row]);
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
            View.PushChatView(TableItems[indexPath.Row]);
        }

        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            switch (editingStyle)
            {
                case UITableViewCellEditingStyle.Delete:
                    Ticket ticket = TableItems[indexPath.Row];
                    new Thread(new ThreadStart(async () =>
                    {
                        await RemoteService.DeleteTicket(ticket);
                        await RemoteService.DeleteMessagesConnectedWithTicket(ticket);
                    })).Start();
                    TableItems.RemoveAt(indexPath.Row);
                    tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                    break;
                case UITableViewCellEditingStyle.None:
                    Console.WriteLine("CommitEditingStyle:None called");
                    break;
            }
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return (Profile.Instance != null && Profile.Instance.Status == "Admin") ? true : false;
        }
    }
}
