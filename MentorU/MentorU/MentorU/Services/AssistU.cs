using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Essentials;
using Microsoft.AspNetCore.SignalR.Client;
using MentorU.ViewModels;
using System.Net.Http;
using MentorU.Models;
using System.Text;

namespace MentorU.Services
{
    public class AssistU : Users
    {
        //public Dictionary<Users,List<MarketplaceItem>> MarketHistory;
        public Dictionary<Users,List<string>> CourseHistory;

        private AssistUChat chat;


        public AssistU()
        {
            id = "0";
            FirstName = "AssistU";

            //MarketHistory = new Dictionary<Users, List<MarketplaceItem>>();
            CourseHistory = new Dictionary<Users, List<string>>();
        }


        public void StartChat()
        {
            chat = new AssistUChat();
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

                byte[] them = Encoding.ASCII.GetBytes("0");
                byte[] me = Encoding.ASCII.GetBytes(App.loggedUser.id);
                List<int> masked = new List<int>();
                for (int i = 0; i < them.Length; i++)
                    masked.Add(them[i] & me[i]);

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
                        if (userID != "0")
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
                await hubConnection.InvokeAsync("SendMessage", groupName, "0", m);
            }
        }
    }
}
