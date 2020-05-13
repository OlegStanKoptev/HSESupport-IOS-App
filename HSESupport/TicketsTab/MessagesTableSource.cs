using System;
using System.Collections.Generic;
using FFImageLoading;
using Foundation;
using UIKit;

namespace HSESupport.TicketsTab
{
    public class MessagesTableSource : UITableViewSource
    {
        List<Message> TableItems;

        UITextField textField;

        UIViewController parentController;

        public MessagesTableSource(List<Message> messages, UITextField textField, UIViewController controller)
        {
            TableItems = messages;
            parentController = controller;
            this.textField = textField;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return TableItems.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            ChatTableCell cell;
            if (TableItems[indexPath.Row].Sender == Profile.Instance.Status)
            {
                cell = tableView.DequeueReusableCell("ChatTableCellUser") as ChatTableCellUser;
                if (cell == null)
                    cell = new ChatTableCellUser("ChatTableCellUser");
            }
            else if (TableItems[indexPath.Row].Sender == "System")
            {
                cell = tableView.DequeueReusableCell("ChatTableCellSystem") as ChatTableCellSystem;
                if (cell == null)
                    cell = new ChatTableCellSystem("ChatTableCellSystem");
            }
            else
            {
                cell = tableView.DequeueReusableCell("ChatTableCellOther") as ChatTableCellOther;
                if (cell == null)
                    cell = new ChatTableCellOther("ChatTableCellOther");
            }
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            cell.SetInfo(TableItems[indexPath.Row].Text,
                    TableItems[indexPath.Row].SendTime, TableItems[indexPath.Row].Type == "Picture");
            return cell;
        }
        
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            textField.ResignFirstResponder();
            var message = TableItems[indexPath.Row];
            if (message.Type == "Picture")
            {
                ((ChatViewController)parentController).ShowPicture(message.Text);
            }
            tableView.DeselectRow(indexPath, true);
        }
        
    }
}
