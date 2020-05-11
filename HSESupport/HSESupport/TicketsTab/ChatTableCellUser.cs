using Foundation;
using System;
using System.Globalization;
using UIKit;

namespace HSESupport
{
    public partial class ChatTableCellUser : ChatTableCell
    {
        public ChatTableCellUser (IntPtr handle) : base (handle)
        {
        }

        public ChatTableCellUser(string cellId) : base(cellId)
        {
        }

        public override void SetInfo(string text, string time, bool picture)
        {
            if (picture)
            {
                Content.Text = "Press here to open a picture";
                Content.TextColor = UIColor.LinkColor;
            }
            else
            {
                Content.Text = text;
                Content.TextColor = UIColor.LabelColor;
            }
            DateTime dTime = DateTime.ParseExact(time, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            Time.Text = dTime.ToShortTimeString();
        }
    }
}