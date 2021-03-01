using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using MentorU.Services.DatabaseServices;
using Microsoft.AspNetCore.SignalR.Client;
using MentorU.Models;
using System.Text;
using System.Linq;

namespace MentorU.Services.Bot
{
    public class AssistU : Users
    {
        //public Dictionary<Users,List<MarketplaceItem>> MarketHistory;
        public Dictionary<Users,List<string>> CourseHistory;

        private AssistUChat chat;
        private RecommendationGenerator recomendations;

        public AssistU()
        {
            id = "AssistU";
            FirstName = "AssistU";

            //MarketHistory = new Dictionary<Users, List<MarketplaceItem>>();
            //CourseHistory = new Dictionary<Users, List<string>>();
        }


        public void StartChat()
        {

            chat = new AssistUChat();
        }

        public List<Items> GetRecommendations()
        {
            recomendations = new RecommendationGenerator();
            return recomendations.GetRecommendations().Result;
        }

        /**
         * Internal class definition for the automated chatting.
         */
        class AssistUChat
        {
            string groupName;
            HubConnection hubConnection;
            List<string> messages;

            const string WelcomeMessage = "Hello, I am AssistU. I can help you find mentors that " +
               "fit your needs. What type of topics would you like your mentor to be knowledgable on?";

            public AssistUChat()
            {
                messages = new List<string>();

                byte[] them = Encoding.ASCII.GetBytes(App.assistU.id);
                byte[] me = Encoding.ASCII.GetBytes(App.loggedUser.id);
                List<int> masked = new List<int>();
                for (int i = 0; i < them.Length; i++)
                    masked.Add(them[i] & me[i]);
                groupName = string.Join("", masked);

                hubConnection = new HubConnectionBuilder()
                    .WithUrl($"{App.SignalRBackendUrl}")
                    //,
                    //(opts) =>
                    //{
                    //    opts.HttpMessageHandlerFactory = (message) =>
                    //    {
                    //        if (message is HttpClientHandler clientHandler)

                    //            clientHandler.ServerCertificateCustomValidationCallback +=
                    //            (sender, certificate, chain, sslPolicyErrors) => { return true; };
                    //        return message;
                    //    };
                    //})
                    .Build();

                hubConnection.On<string, string>("ReceiveMessage", (userID, message) =>
                {
                    try
                    {
                        if (userID != App.assistU.id)
                            ReceiveNewMessage(message);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                });

                _ = hubConnection.StartAsync();
                _ = hubConnection.InvokeAsync("AddToGroup", groupName);
                _ = SendMessage(WelcomeMessage);
            }


            async void ReceiveNewMessage(string m)
            {
                messages.Add(m);
                // Do NLP on m to know what to qualities to look for

                await SendMessage("Okay! Let me see what I can find.");
            }

            async Task SendMessage(string m)
            {
                await hubConnection.InvokeAsync("SendMessage", groupName, App.assistU.id, m);
            }
        }



        /// <summary>
        /// Recommendations for marketplace based off of the classes the
        /// user is currently taking.
        /// </summary>
        class RecommendationGenerator
        {
            List<string> _classes;
            public RecommendationGenerator()
            {
                _classes = (List<string>)DatabaseService.Instance.client.GetTable<Classes>()
                    .Where(u => u.UserId == App.loggedUser.id).ToListAsync().Result
                    .Select(u => u.ClassName);
            }

            public async Task<List<Items>> GetRecommendations()
            {
                var items = await DatabaseService.Instance.client.GetTable<Items>()
                    .Where(u => u.id != App.loggedUser.id && _classes.Contains(u.ClassUsed)).ToListAsync();
                //FIXME: Items model on DB does notnot contain field ClassUsed yet
                return items.GetRange(0, 5); // return only the top five recommendations
                //TODO: come up with a more clever scheme, maybe one per class/cheapest
            }
        }
    }
}
