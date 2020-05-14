using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Flurl.Http;
using HSESupport.TicketsTab;
using Microsoft.Identity.Client;
using System.Web;
using UIKit;
using UserNotifications;

namespace HSESupport
{
    public delegate void NewInformation();

    public class RemoteService
    {
        public static event NewInformation UpdateMessagesList;

        public static List<Ticket> Tickets { get; set; }
        public static List<Message> Messages { get; set; }
        public static List<Alert> Alerts { get; set; }
        public static List<News> News { get; set; }

        public static int NeededProfilePageNum { get; set; }
        static IPublicClientApplication app;
        static AuthenticationResult result;
        static string[] scopes;

        public static void TriggerUpdateEvent()
        {
            UpdateMessagesList?.Invoke();
        }
        public static async Task GetInfo()
        {
            await UpdateTickets();
            await UpdateMessages();
        }
        static RemoteService()
        {
            scopes = new string[] { "" };
            string clientId = "";
            app = PublicClientApplicationBuilder.Create(clientId)
                .WithB2CAuthority("")
                .WithIosKeychainSecurityGroup("com.koptev.HSE-Support")
                .WithRedirectUri($"msal{clientId}://auth")
                .Build();

            Tickets = new List<Ticket>();
            Messages = new List<Message>();
            Alerts = new List<Alert>();
            News = new List<News>();
            NeededProfilePageNum = 0;
        }

        #region Authorization
        public static async Task<bool> LogInTheUser()
        {
            string firstName = "", secondName = "", objectId = "", emailAdress = "";
            try
            {
                var jwtHandler = new JwtSecurityTokenHandler();
                var jwtInput = result.IdToken;
                var readableToken = jwtHandler.CanReadToken(jwtInput);
                if (readableToken)
                {
                    var token = jwtHandler.ReadJwtToken(jwtInput);
                    var claims = token.Claims;
                    foreach (Claim c in claims)
                    {
                        switch (c.Type)
                        {
                            case "given_name":
                                firstName = c.Value;
                                break;
                            case "family_name":
                                secondName = c.Value;
                                break;
                            case "oid":
                                objectId = c.Value;
                                break;
                            case "emails":
                                emailAdress = c.Value;
                                break;
                            default:
                                break;
                        }
                    }

                    await FindProfile(objectId, firstName + ' ' + secondName, emailAdress);
                    await GetInfo();
                    NeededProfilePageNum = 1;
                    return true;
                }

            }
            catch (Exception)
            {
                Console.WriteLine("Probably 'result' is null");
            }
            return false;
        }
        public static async Task NonSilentLogIn()
        {
            try
            {
                result = await app.AcquireTokenInteractive(scopes)
                                .ExecuteAsync();
            }
            catch (MsalException)
            {
                Console.WriteLine("You cancelled logging in process");
            }
        }
        public static async Task<bool> SilentLogInPossible()
        {
            try
            {
                var accounts = await app.GetAccountsAsync();
                result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                    .ExecuteAsync();
                return true;
            }
            catch (MsalException)
            {
                return false;
            }
        }
        public static async Task LogOut()
        {
            IEnumerable<IAccount> accounts = await app.GetAccountsAsync();

            while (accounts.Any())
            {
                await app.RemoveAsync(accounts.First());
                accounts = await app.GetAccountsAsync();
            }
            result = null;
            Profile.Instance = null;
        }
        #endregion
        #region Profile Search
        public static async Task FindProfile(string id, string name, string email)
        {
            try
            {
                Profile.Instance = await (Constants.APIAdress + "/api/users/" + id).GetJsonAsync<Profile>();
            }
            catch (FlurlHttpException ex)
            {
                if (ex.Call.HttpStatus.Value == HttpStatusCode.NotFound)
                {
                    Profile.Instance = await (Constants.APIAdress + "/api/users/")
                        .PostJsonAsync(new { Name = name, UserId = id, Status = "Student", Email = email })
                        .ReceiveJson<Profile>();
                }
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine(Profile.Instance != null ? "Profile found" : "Profile not found");
        }

        public static async Task<List<Profile>> FindProfilesWithNameOrEmail(string name)
        {
            try
            {
                List<Profile> result = await (Constants.APIAdress + "/api/users/search/" + name).GetJsonAsync<List<Profile>>();
                return result;
            }
            catch (FlurlHttpException) { return null; }
        }

        public static async Task<Profile> FindProfileWithId(string userId)
        {
            try
            {
                return await (Constants.APIAdress + "/api/users/" + userId).GetJsonAsync<Profile>();
            }
            catch (FlurlHttpException) { return null; }
        }
        #endregion
        #region Tickets
        public static async Task UpdateTicketInfo(Ticket ticket)
        {
            try
            {
                var result = await (Constants.APIAdress + "/api/tickets/").PutJsonAsync(ticket).ReceiveString();
                Console.WriteLine("Result of PUT request: " + result);
            }
            catch (FlurlHttpException) { }
        }

        public static async Task<Ticket> CreateTicket(Ticket ticket)
        {
            try
            {
                Ticket obj = await (Constants.APIAdress + "/api/tickets/")
                    .PostJsonAsync(new
                    {
                        userId = ticket.UserId,
                        topic = ticket.Topic,
                        openTime = ticket.OpenTime,
                        status = ticket.Status,
                        lastMessageText = ticket.LastMessageText,
                        lastMessageTime = ticket.LastMessageTime,
                        fullName = ticket.FullName
                    })
                    .ReceiveJson<Ticket>();
                Console.WriteLine("Your ticket topic: \"" + obj.Topic + "\"");
                return obj;
            }
            catch (FlurlHttpException) { return null; }
        }

        public static async Task DeleteTicket(Ticket ticket)
        {
            try
            {
                Ticket obj = await (Constants.APIAdress + "/api/tickets/" + ticket.Id).DeleteAsync().ReceiveJson<Ticket>();
                Console.WriteLine("Was deleted ticket about: " + obj.Topic);
            }
            catch (FlurlHttpException) { }
        }

        public static async Task ChangeTicketStatus(Ticket ticket, string status)
        {
            try
            {
                ticket.Status = status;
                Ticket obj = await (Constants.APIAdress + "/api/tickets/").PutJsonAsync(ticket).ReceiveJson<Ticket>();
                Console.WriteLine("Result of changing status: " + obj.Status);
            }
            catch (FlurlHttpException) { }
        }

        public static async Task<List<Ticket>> GetTickets(string userId)
        {
            try
            {
                List<Ticket> result = await (Constants.APIAdress + "/api/tickets/" + userId).GetJsonAsync<List<Ticket>>();
                return result;
            }
            catch (FlurlHttpException) { return null; }
        }

        public static async Task UpdateTickets()
        {
            try
            {
                if (Profile.Instance.Status == "Student")
                {
                    Tickets = await (Constants.APIAdress + "/api/tickets/" + Profile.Instance.UserId).GetJsonAsync<List<Ticket>>();
                }
                else
                {
                    Tickets = await (Constants.APIAdress + "/api/tickets/").GetJsonAsync<List<Ticket>>();
                }
            }
            catch (FlurlHttpException) { }
        }
        #endregion
        #region Messages
        public static async Task UpdateMessages()
        {
            try
            {
                if (Profile.Instance.Status == "Student")
                {
                    Messages = await (Constants.APIAdress + "/api/messages/" + Profile.Instance.UserId).GetJsonAsync<List<Message>>();
                }
                else
                {
                    Messages = await (Constants.APIAdress + "/api/messages/").GetJsonAsync<List<Message>>();
                }
            }
            catch (FlurlHttpException) { }
        }

        public static async Task SendMessage(Message message)
        {
            try
            {
                Message obj = await (Constants.APIAdress + "/api/messages/")
                .PostJsonAsync(new
                {
                    text = message.Text,
                    sender = message.Sender,
                    sendTime = message.SendTime,
                    userId = message.UserId,
                    ticketId = message.TicketId,
                    type = message.Type
                })
                .ReceiveJson<Message>();
                Console.WriteLine("Your message text: \"" + obj.Text + "\"");
            }
            catch (FlurlHttpException) { }
        }

        public static async Task DeleteMessagesConnectedWithTicket(Ticket ticket)
        {
            try
            {
                string obj = await (Constants.APIAdress + "/api/messages/ticket/" + ticket.Id).DeleteAsync().ReceiveString();
                Console.WriteLine("Was deleted: " + obj);
            }
            catch (FlurlHttpException) { }
        }
        #endregion
        #region Alerts
        public static async Task UpdateAlerts(string email)
        {
            try
            {
                Alerts = await (Constants.APIAdress + "/api/alerts/" + email).GetJsonAsync<List<Alert>>();
            }
            catch (FlurlHttpException) { }
        }

        public static async Task PostAlert(Alert alert)
        {
            try
            {
                Alert receivedAlert = await (Constants.APIAdress + "/api/alerts/")
                    .PostJsonAsync(new
                    {
                        userEmail = alert.UserEmail,
                        title = alert.Title,
                        text = alert.Text,
                        timeCreated = alert.TimeCreated,
                        picture = alert.Picture
                    })
                    .ReceiveJson<Alert>();
                Console.WriteLine(receivedAlert.Title);
            }
            catch (FlurlHttpException) { }
        }

        public static async Task DeleteAlert(int id)
        {
            try
            {
                Alert obj = await (Constants.APIAdress + "/api/alerts/" + id).DeleteAsync().ReceiveJson<Alert>();
            }
            catch (FlurlHttpException) { }
        }

        public static async Task<List<Alert>> GetAlerts(string email)
        {
            try
            {
                List<Alert> alerts = await (Constants.APIAdress + "/api/alerts/" + email).GetJsonAsync<List<Alert>>();
                return alerts;
            }
            catch (FlurlHttpException) { return null; }
        }
        #endregion
        #region Pictures
        public static async Task SetUserPicture(Profile profile)
        {
            try
            {
                if (!File.Exists(Constants.Images + profile.UserId + ".jpg")) Console.WriteLine("There is no such image");
                else
                {
                    if (profile.HasPicture == 0)
                    {
                        profile.HasPicture = 1;
                        var obj = await (Constants.APIAdress + "/api/users/").PutJsonAsync(profile).ReceiveString();
                        Console.WriteLine(obj);
                    }
                    var obj2 = await (Constants.APIAdress + "/api/pictures/user/").PostMultipartAsync(mp => mp.AddFile("uploadedFile", Constants.Images + profile.UserId + ".jpg")).ReceiveString();
                    Console.WriteLine(obj2);
                }
            }
            catch (FlurlHttpException) { }
        }

        public static async Task GetUserPicture(Profile profile, string path)
        {
            try
            {
                var obj = await (Constants.APIAdress + "/api/pictures/" + profile.UserId).DownloadFileAsync(path);
                Console.WriteLine(obj);
            }
            catch (FlurlHttpException) { }
        }

        public static async Task DeleteUserPicture(Profile profile)
        {
            try
            {
                if (profile.HasPicture == 1)
                {
                    profile.HasPicture = 0;
                    var obj1 = await (Constants.APIAdress + "/api/users/").PutJsonAsync(profile).ReceiveString();
                    Console.WriteLine(obj1);
                }
                var obj2 = await (Constants.APIAdress + "/api/pictures/" + profile.UserId + ".jpg").DeleteAsync().ReceiveString();
                Console.WriteLine(obj2);
            }
            catch (FlurlHttpException) { }
        }

        public static async Task SendPictureAsAttachment(Message message, Profile profile, string path)
        {
            try
            {
                //Send a message
                Message obj = await (Constants.APIAdress + "/api/messages/")
                    .PostJsonAsync(new
                    {
                        text = message.Text,
                        sender = message.Sender,
                        sendTime = message.SendTime,
                        userId = message.UserId,
                        ticketId = message.TicketId,
                        type = message.Type
                    })
                    .ReceiveJson<Message>();

                //Send a picture
                var obj2 = await (Constants.APIAdress + "/api/pictures/message/" + profile.UserId + "/" + obj.Id)
                    .PostMultipartAsync(mp => mp.AddFile("uploadedFile", path)).ReceiveString();
                Console.WriteLine(obj2);
            }
            catch (FlurlHttpException) { }
        }

        public static async Task GetPictureWithName(string name, string path)
        {
            try
            {
                var obj = await (Constants.APIAdress + "/api/pictures/attachment/" + name).DownloadFileAsync(path);
                Console.WriteLine(obj);
            }
            catch (FlurlHttpException) { }
        }
        #endregion
    }
}
