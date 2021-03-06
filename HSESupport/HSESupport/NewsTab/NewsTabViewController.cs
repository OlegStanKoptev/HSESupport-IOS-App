using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Threading;
using UIKit;

namespace HSESupport
{
    public partial class NewsTabViewController : UITableViewController
    {
        UIRefreshControl refreshControl;
        public NewsTabViewController(IntPtr handle) : base(handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TableSetup();
            NavigationItem.TitleView = SetTitle("News", "from «hse.ru»");
            List<News> emptyNews = new List<News>();
            emptyNews.AddRange(new List<News> { new News("", "launchCellInside.png", " "),
                new News("", "launchCellInside.png", " "),
                new News("", "launchCellInside.png", " "),
                new News("", "launchCellInside.png", " ")});
            NewsTable.Source = new NewsTableSource(emptyNews, false);
            NewsTable.ScrollEnabled = false;
            new Thread(new ThreadStart(() =>
            {
                NewsUpdate.SavedNewsList = NewsUpdate.InitialUpdateNews();
                InvokeOnMainThread(() => {
                    NewsTable.Source = new NewsTableSource(NewsUpdate.SavedNewsList, true);
                    NewsTable.ReloadData();
                    NewsTable.ScrollEnabled = true;
                });
            })).Start();
            SubmenuButton.Title = "";
            SubmenuButton.Enabled = false;
        }
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (Profile.Instance != null)
            {
                SubmenuButton.Title = "Alerts";
                SubmenuButton.Enabled = true;
            }
            else
            {
                SubmenuButton.Title = "";
                SubmenuButton.Enabled = false;
            }

        }
        internal void TableSetup()
        {
            refreshControl = new UIRefreshControl();
            refreshControl.ValueChanged += refreshTable;
            NewsTable.AddSubview(refreshControl);
            NewsTable.ContentInset = new UIEdgeInsets(10, 0, 0, 0);
        }
        private void refreshTable(object sender, EventArgs e)
        {
            refreshControl.BeginRefreshing();
            new Thread(new ThreadStart(() => {
                NewsUpdate.SavedNewsList = NewsUpdate.InitialUpdateNews();
                InvokeOnMainThread(() => {
                    NewsTable.Source = new NewsTableSource(NewsUpdate.SavedNewsList);
                    refreshControl.EndRefreshing();
                });
            })).Start();
        }

        UIView SetTitle(string title, string subtitle)
        {
            var titleLabel = new UILabel();
            titleLabel.TextColor = UIColor.Black;
            titleLabel.Font = UIFont.BoldSystemFontOfSize(17);
            titleLabel.Text = title;

            var subtitleLabel = new UILabel();
            subtitleLabel.TextColor = UIColor.Gray;
            subtitleLabel.Font = UIFont.SystemFontOfSize(12);
            subtitleLabel.Text = subtitle;
            
            var stackView = new UIStackView(new UIView[] { titleLabel, subtitleLabel });
            stackView.Distribution = UIStackViewDistribution.EqualCentering;
            stackView.Axis = UILayoutConstraintAxis.Vertical;
            stackView.Alignment = UIStackViewAlignment.Center;

            var width = Math.Max(titleLabel.Frame.Size.Width, subtitleLabel.Frame.Size.Width);
            stackView.Frame = new CGRect(x: 0, y: 0, width: width, height: 35);

            titleLabel.SizeToFit();
            subtitleLabel.SizeToFit();

            return stackView;
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "FromNews")
            {
                NavigationControllerForAlertsTable viewController;
                if ((viewController = segue.DestinationViewController as NavigationControllerForAlertsTable) != null)
                {
                    viewController.SetIfFromNews(true);
                }
            }
        }
    }
}