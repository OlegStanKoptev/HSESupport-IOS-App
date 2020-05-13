using System;
using System.Collections.Generic;
using System.Threading;
using HtmlAgilityPack;

namespace HSESupport
{
    public static class NewsUpdate
    {
        internal static List<News> SavedNewsList = new List<News>();
        internal static int loadedPageOfNews;
        internal static HtmlWeb web = new HtmlWeb();
        public static List<News> InitialUpdateNews()
        {
            List<News> NewsList = new List<News>();
            List<string> htmls = new List<string> { @"https://www.hse.ru/news/" };
            loadedPageOfNews = 1;
            ParseNews(ref NewsList, htmls);
            return NewsList;
        }

        internal static List<News> UpdateNews()
        {
            List<News> NewsList = new List<News>();
            List<string> htmls = new List<string>();
            for (int i = loadedPageOfNews + 1; i <= loadedPageOfNews + 1; i++)
            {
                htmls.Add(string.Format(@"https://www.hse.ru/news/page{0}.html", i.ToString()));
            }
            ParseNews(ref NewsList, htmls);
            return NewsList;

        }
        private static void ParseNews(ref List<News> NewsList, List<string> htmls)
        {
            try
            {
                foreach (string html in htmls)
                {
                    var htmlDoc = web.Load(html);

                    string articles = "//body/div/div/div/div/div/div/div/div";
                    var articlesNodes = htmlDoc.DocumentNode.SelectNodes(articles);
                    for (int i = 6; i < 16; i++)
                    {
                        var currentArticleNode = articlesNodes[i];

                        string titlePath = "div/h2";
                        HtmlNode titleNode = currentArticleNode.SelectSingleNode(titlePath);
                        string title = titleNode.InnerText;
                        string link = titleNode.FirstChild.Attributes["href"].Value;
                        string picture;
                        try
                        {
                            string picturePath = "div/div/img";
                            HtmlNode pictureNode = currentArticleNode.SelectSingleNode(picturePath);
                            picture = pictureNode.Attributes["src"].Value;
                        }
                        catch (Exception)
                        {
                            string subtitlePath = "div/div/p[2]";
                            HtmlNode subtitleNode = currentArticleNode.SelectSingleNode(subtitlePath);
                            title += " " + subtitleNode.InnerText;
                            picture = "hse_logo.jpg";
                        }
                        NewsList.Add(new News(title, picture, link));
                    }
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
                NewsList = new List<News> { new News("A problem occured while connecting to hse news website", "hse_logo.jpg", " ") };
                Console.WriteLine("Problems while connecting to the website.");
            }
        }
    }
}
