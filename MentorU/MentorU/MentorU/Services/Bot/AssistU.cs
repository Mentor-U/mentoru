using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using MentorU.Services.DatabaseServices;
using Microsoft.AspNetCore.SignalR.Client;
using MentorU.Models;
using System.Text;
using System.Linq;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using MentorU.ViewModels;
using MentorU.Services.Bot;
using System.Threading;

namespace MentorU.Services.Bot
{
    public class AssistU : Users
    {
        private RecommendationGenerator recomendations;
        public ObservableCollection<Message> _chatHistory;
        public AssistU()
        {
            id = "AssistU";
            FirstName = "AssistU";
            _chatHistory = new ObservableCollection<Message>();
        }


        public AssistUChatViewModel StartChat()
        {
            //chat = new AssistUChat();
            return new AssistUChatViewModel();
        }

        public async Task<List<Items>> GetRecommendations()
        {
            recomendations = new RecommendationGenerator();
            return await recomendations.GetRecommendations();
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


        //-------------------------------------
        //        new and improved
        //------------------------------------
        public class AssistUChatViewModel : ChatViewModel
        { 
            BotService _botService;
            Task ReceiveTask;

            public AssistUChatViewModel() : base(App.assistU)
            {
                _botService = new BotService();
                _botService.BotMessageReceived += OnBotMessageReceived;
                LoadPageData = new Command(async () => await ExecuteLoadPageData());
                OnSendCommand = new Command(async () => await ExecuteSend());
            }


            public override Task ExecuteLoadPageData()
            {
                var t = new Task(() => { MessageList = App.assistU._chatHistory; });
                IsBusy = false;
                return t;
            }

            void OnBotMessageReceived(List<BotMessage> msgs)
            {
                ReceiveTask = new Task(() =>
                {
                    Thread.Sleep(1000);
                    foreach (var msg in msgs)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            var m = new Message() { Mine = false, Theirs = true, Text = msg.Content };
                            MessageList.Add(m);
                            App.assistU._chatHistory.Add(m);
                        });
                    }
                });
                ReceiveTask.Start();
            }

            public override async Task ExecuteSend()
            {
                try
                {
                    await _botService.SetUpAsync();
                    var msg = new Message() { Mine = true, Theirs = false, Text = TextDraft };
                    TextDraft = string.Empty;
                    MessageList.Add(msg);
                    await _botService.SendMessageAsync(msg.Text);
                    App.assistU._chatHistory.Add(msg);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Unable to send message. Exception => {e}");
                }
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

            }

            public async Task<List<Items>> GetRecommendations()
            {
                if (App.loggedUser.Role == "0")
                {
                    var allItems = await DatabaseService.Instance.client.GetTable<Items>().ToListAsync();
                    int count = allItems.Count < 5 ? allItems.Count : 5;
                    return allItems.GetRange(0, count);
                }

                var t = await DatabaseService.Instance.client.GetTable<Classes>()
                        .Where(u => u.UserId == App.loggedUser.id).ToListAsync();
                var h = new HashSet<string>(t.Select(u => u.ClassName));
                _classes = h.ToList();

                var items = await DatabaseService.Instance.client.GetTable<Items>()
                    .Where(u => u.id != App.loggedUser.id && _classes.Contains(u.ClassUsed)).ToListAsync();
                var maxLen = 5 < items.Count ? 5 : items.Count;
                return items.GetRange(0, maxLen); // return only the top five recommendations
                //TODO: come up with a more clever scheme, maybe one per class/cheapest
            }
        }
    }
}
