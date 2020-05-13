using System;
namespace HSESupport
{
    public class News
    {
        internal string Title { get; set; }
        internal string Image { get; set; }
        internal string Link { get; set; }
        public News(string title, string image, string link)
        {
            string SiteAdress = @"https://hse.ru";
            Title = title;
            Image = image[0] == '/' ? SiteAdress + image : image;
            Link = link[0] == '/' ? SiteAdress + link : link;
        }
    }
}
