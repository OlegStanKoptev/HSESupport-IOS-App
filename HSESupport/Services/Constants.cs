using System;
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

        public const string APIAdress = //"http://127.0.0.1:5000";
        "https://hsesupportapi.azurewebsites.net";

        private static string _documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string Library = Path.Combine(_documents, "..", "Library");
        public static string Images = Path.Combine(Library, "Images/");

        //public static NSData DeviceToken { get; set; }

        // Azure app-specific connection string and hub path
        public const string ListenConnectionString = "Endpoint=sb://namespaceformyprojects.servicebus.windows.net/" +
            ";SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=8vDtcvm4o7/" +
            "viny07UvLuTX9jGbDdaPrpD2HYOTihZw=";
        public const string NotificationHubName = "HSESupportAppNotifHub";

        public static bool ChatIsPresented { get; set; } = false;
    }
}
