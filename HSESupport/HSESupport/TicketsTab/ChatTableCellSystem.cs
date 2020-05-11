using Foundation;
using System;
using System.Globalization;
using UIKit;

namespace HSESupport
{
    public partial class ChatTableCellSystem : ChatTableCell
    {
        public ChatTableCellSystem (IntPtr handle) : base (handle)
        {
        }

        public ChatTableCellSystem(string cellId) : base(cellId)
        {
        }

        public override void SetInfo(string text, string time, bool picture)
        {
            try
            {
                DateTime date = DateTime.ParseExact(text, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                Content.Text = date.ToLongDateString();
            } catch (FormatException)
            {
                Content.Text = text;
            }
        }
    }
}