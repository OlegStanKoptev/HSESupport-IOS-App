using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using Xamarin.Essentials;

namespace HSESupport
{
    public class NewsTableSource : UITableViewSource
    {
        List<News> tableItems;

        bool isLoading;
        bool hasGradient;

        string cellIdentifierWithGradient = "NewsTableCellWithGradient";
        string cellIdentifierNoGradient = "NewsTableCellNoGradient";

        public NewsTableSource(List<News> news, params bool[] gradient)
        {
            tableItems = news;
            isLoading = false;
            if (gradient.Length != 0)
                hasGradient = gradient[0];
            else
                hasGradient = true;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return tableItems.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row >= NewsUpdate.loadedPageOfNews * 10 - 10 && !isLoading)
            {
                isLoading = true;
                new System.Threading.Thread(new System.Threading.ThreadStart(() => {
                    List<News> tempList = NewsUpdate.UpdateNews();
                    if (tempList.Count > 1)
                    {
                        NewsUpdate.loadedPageOfNews += 1;
                        for (int i = 0; i < 10; i++)
                        {
                            NewsUpdate.SavedNewsList.Add(tempList[i]);
                        }
                        InvokeOnMainThread(() =>
                        {
                            tableView.Source = new NewsTableSource(NewsUpdate.SavedNewsList);
                            isLoading = false;
                        });
                    }
                    else
                    {
                        InvokeOnMainThread(() => { tableView.Source = new NewsTableSource(tempList); }); 
                    }
                })).Start();
            }
            if (hasGradient)
            {
                var cell = tableView.DequeueReusableCell(cellIdentifierWithGradient) as NewsCell;
                if (cell == null)
                    cell = new NewsCell(cellIdentifierWithGradient, true);
                cell.UpdateCell(tableItems[indexPath.Row].Title
                        , tableItems[indexPath.Row].Image);
                return cell;
            }
            else
            {
                var cell = tableView.DequeueReusableCell(cellIdentifierNoGradient) as NewsCell;
                if (cell == null)
                    cell = new NewsCell(cellIdentifierNoGradient, false);
                cell.UpdateCell(tableItems[indexPath.Row].Title
                        , tableItems[indexPath.Row].Image);
                return cell;
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);
            if (tableItems[indexPath.Row].Link != " ")
            { Browser.OpenAsync(new NSUrl(NewsUpdate.SavedNewsList[indexPath.Row].Link), BrowserLaunchMode.SystemPreferred); }
        }
    }
}
