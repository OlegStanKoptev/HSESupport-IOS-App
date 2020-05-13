using Foundation;
using System;
using System.Globalization;
using UIKit;

namespace HSESupport
{
    public partial class ChatTableCellOther : ChatTableCell
    {
        public ChatTableCellOther (IntPtr handle) : base (handle)
        {
        }

        public ChatTableCellOther(string cellId) : base(cellId)
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