﻿using System;
using System.IO;

namespace HSESupport
{
    public static class Constants
    {
        static Constants()
        {
            Console.WriteLine(Library);
            Console.WriteLine(Images);
        }

        public const string APIAdress = "http://127.0.0.1:5000";

        private static string _documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string Library = Path.Combine(_documents, "..", "Library");
        public static string Images = Path.Combine(Library, "Images/");

        //public static NSData DeviceToken { get; set; }

        // Azure app-specific connection string and hub path
        public const string ListenConnectionString = "";
        public const string NotificationHubName = "";

        public static bool ChatIsPresented { get; set; } = false;
    }
}
