using System;
using UIKit;

namespace HSESupport
{
    public abstract class ChatTableCell : UITableViewCell
    {
        public ChatTableCell(IntPtr intPtr) : base(intPtr)
        {
        }

        public ChatTableCell(string cellId) : base(UITableViewCellStyle.Default, cellId)
        {
        }

        public abstract void SetInfo(string text, string time, bool picture);
    }
}
