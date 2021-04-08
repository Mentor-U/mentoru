using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using MentorU.Services.DatabaseServices;
using MentorU.Models;
using System.Linq;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using MentorU.ViewModels;
using System.Threading;
using System.Text.RegularExpressions;
using MentorU.Views;
using MentorU.Services.Blob;

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
            return new AssistUChatViewModel();
        }

        public async Task<List<Items>> GetRecommendations()
        {
            recomendations = new RecommendationGenerator();
            return await recomendations.GetRecommendations();
        }


        /// <summary>
        /// Bot chat viewmodel that interfaces with the bot service
        /// to handle all NLP.
        /// </summary>
        public class AssistUChatViewModel : ChatViewModel
        { 
            BotService _botService;
            Task ReceiveTask;
            Regex _query;
            string _foundMentor = "I found {0} who is knowledgable with {1} and skilled at {2}!";
            string _notFound = "I wasn't able to find a mentor who works in {0} and is skilled at {1}.";
            Users FoundUser;

            public AssistUChatViewModel() : base(App.assistU)
            {
                _botService = new BotService();
                _query = new Regex(@"<QUERY>.*");
                _botService.BotMessageReceived += OnBotMessageReceived;
                LoadPageData = new Command(() => { IsBusy = false; });
                OnSendCommand = new Command(async () => await ExecuteSend());
                ViewProfileCommand = new Command(ViewProfile);
            }


            public override Task ExecuteLoadPageData()
            {
                var t = new Task(() => { MessageList = App.assistU._chatHistory; });
                //t.Start();
                IsBusy = false;
                return t;
            }

            /// <summary>
            /// Handler for receiving messages from the bot
            /// </summary>
            /// <param name="msgs"></param>
            void OnBotMessageReceived(List<BotMessage> msgs)
            {
                ReceiveTask = new Task(() =>
                {
                    Thread.Sleep(500);
                    foreach (var msg in msgs)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            string message = msg.Content;
                            if (message != null && _query.IsMatch(message))
                            {
                                var queryMsg = message.Split('#');
                                message = queryMsg[1];
                                DBQuery(queryMsg[0]); // if concurrency issues, create task
                            }
                            var finalMsg = new Message() { Mine = false, Theirs = true, Text = message };
                            MessageList.Add(finalMsg);
                            App.assistU._chatHistory.Add(finalMsg);
                        });
                    }
                });
                ReceiveTask.Start();
            }

            /// <summary>
            /// Entities should have been extracted by the model and
            /// the database can be queried for relevant mentors.
            /// </summary>
            /// <param name="m"></param>
            private async void DBQuery(string query)
            {
                string[] entities = query.Split(':');
                if (entities.Length >= 3) // gaurunteed unless blank message? TODO: test bot's edge cases
                {
                    var mentors = await DatabaseService.Instance.client
                        .GetTable<Users>()
                        .Where(u => u.Role == "0" && u.Major.ToLower() == entities[1])
                        .ToListAsync();
                    var skills = await DatabaseService.Instance.client
                        .GetTable<Classes>()
                        .Where(s => s.ClassName == entities[2])
                        .ToListAsync();
                    var mSet = new HashSet<string>(mentors.Select(mntr => mntr.id));
                    var sSet = new HashSet<string>(skills.Select(s => s.UserId));
                    var available = mSet.Intersect(sSet);
                    var choices = mentors.Where(mntr => available.Contains(mntr.id)).ToList();
                    
                    var msg = new Message(){Mine = false,Theirs = true,};
                    if (choices.Count > 0)
                    {
                        FoundUser = choices[0];
                        msg.Text = string.Format(_foundMentor, FoundUser.DisplayName, entities[1], entities[2]);
                    }
                    else if (mSet.Count > 0)
                    {
                        FoundUser = mentors.Where(userID => userID.id == mSet.ToList()[0]).ToList()[0];
                        msg.Text = $"I was able to find {FoundUser.DisplayName}, who is works in {entities[1]}.";
                    }
                    else if (sSet.Count > 0)
                    {
                        string usrID = skills.ToList()[0].UserId;
                        var uList = await DatabaseService.Instance.client.GetTable<Users>().Where(u => u.id == usrID).ToListAsync();
                        FoundUser = uList[0];
                        msg.Text = $"I was able to find {FoundUser.DisplayName}, who is skilled with {entities[2]}.";
                    }
                    else
                    {
                        msg.Text = string.Format(_notFound, entities[1], entities[2]);
                        goto Finish;
                    }

                    msg.MentorSearch = true;
                    msg.Name = FoundUser.DisplayName;
                    msg.ProfileImage = await BlobService.Instance.TryDownloadImage("profile-images", FoundUser.id);

                    Finish:
                        MessageList.Add(msg);
                        App.assistU._chatHistory.Add(msg);

                    await _botService.SendMessageAsync(""); // Signal continuation of dialog
                }
            }


            /// <summary>
            /// Sends the message to the bot and updates the UI
            /// </summary>
            /// <returns></returns>
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

            async void ViewProfile()
            {
                if (FoundUser != null)
                    await Shell.Current.Navigation.PushAsync(new ViewOnlyProfilePage(FoundUser, false));
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
