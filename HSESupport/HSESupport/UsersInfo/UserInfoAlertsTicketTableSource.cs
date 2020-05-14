using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Foundation;
using HSESupport.TicketsTab;
using UIKit;

namespace HSESupport
{
    public class UserInfoAlertsTicketsTableSource : UITableViewSource
    {
        public string AlertCellID { get => "UserInfoAlertCell"; }
        public string TicketCellID { get => "UserInfoTicketCell"; }

        List<Alert> Alerts;
        List<Ticket> Tickets;

        public UserInfoAlertsTicketsTableSource(List<Alert> items)
        {
            Alerts = items;
            Tickets = null;
        }

        public UserInfoAlertsTicketsTableSource(List<Ticket> items)
        {
            Alerts = null;
            Tickets = items;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (Alerts != null)
            {
                var cell = tableView.DequeueReusableCell(AlertCellID, indexPath) as UserInfoAlertsTicketsTableCell;
                Alert alert = Alerts[indexPath.Row];
                cell.Title.Text = alert.Title;
                cell.Content.Text = alert.Text;
                cell.Picture.Image = UIImage.FromFile(alert.Picture);
                cell.TimeStamp.Text = alert.TimeCreated;
                return cell;
            }
            else
            {
                var cell = tableView.DequeueReusableCell(TicketCellID, indexPath) as UserInfoAlertsTicketsTicketTableCell;
                Ticket ticket = Tickets[indexPath.Row];
                cell.Title.Text = ticket.Topic;
                cell.HeadLine.Text = $"{ImageFromStatus(ticket.Status)} {ticket.LastMessageText}";
                DateTime dtime = DateTime.ParseExact(ticket.LastMessageTime, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                if (DateTime.Now.Date == dtime.Date)
                {
                    cell.Time.Text = dtime.ToShortTimeString();
                }
                else
                {
                    cell.Time.Text = dtime.ToShortDateString();
                }
                cell.Picture.Image = UIImage.FromFile("Images/hse_round_logo.png");
                return cell;
            }
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return Alerts != null ? Alerts.Count : Tickets.Count;
        }

        private string ImageFromStatus(string status)
        {
            if (status == "Open") return "🟢";
            if (status == "Pending") return "🟡";
            return "🔴";
        }

        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            switch (editingStyle)
            {
                case UITableViewCellEditingStyle.Delete:
                    Alert alert = Alerts[indexPath.Row];
                    // remove the item from the underlying data source
                    new Thread(new ThreadStart(async () =>
                    {
                        await RemoteService.DeleteAlert(alert.Id);
                    })).Start();
                    Alerts.RemoveAt(indexPath.Row);
                    tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                    break;
                case UITableViewCellEditingStyle.None:
                    Console.WriteLine("CommitEditingStyle:None called");
                    break;
            }
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return Alerts != null ? true : false;
        }
    }
}
