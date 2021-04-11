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
        /// Bot chat viewmodel that interfaces with the bot hosted on
        /// Azure to handle all NLP.
        /// </summary>
        public class AssistUChatViewModel : ChatViewModel
        { 
            BotService _botService;
            Task ReceiveTask;
            Regex _query;
            string _foundMentor = "I found {0} who is knowledgable with {1} and skilled at {2}!";
            string _notFound = "I wasn't able to find a mentor who works in {0} and is skilled at {1}. " +
                "If I wasn't able to understand your request, please try rephrasing.";
            Users FoundUser;


            public AssistUChatViewModel() : base(App.assistU)
            {
                _botService = new BotService();
                _query = new Regex(@"<QUERY>.*");
                _botService.BotMessageReceived += OnBotMessageReceived;
                LoadPageData = new Command(async () => await ExecuteLoadPageData());
                OnSendCommand = new Command(async () => await ExecuteSend());
                ViewProfileCommand = new Command(ViewProfile);
            }


            public async override Task ExecuteLoadPageData()
            {
                IsBusy = false;
                await _botService.SetUpAsync(App.loggedUser.id);
                var t = new Task(() => { MessageList = App.assistU._chatHistory; });
                //t.Start();

                if(MessageList.Count > 0)
                    _messageListView.ScrollTo(MessageList[MessageList.Count - 1], ScrollToPosition.MakeVisible, true);
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
                            _messageListView.ScrollTo(MessageList[MessageList.Count - 1], ScrollToPosition.MakeVisible, true);
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

                if (entities.Length < 3)
                    return;
                // Query the DB based on the parameters sent from the bot
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


                // Filter the response based on what mentors where found.
                var msg = new Message(){Mine = false,Theirs = true,};
                foreach (var m in choices)
                {
                    if (m.id == App.loggedUser.id)
                        continue;
                    FoundUser = m;
                    msg.Text = string.Format(_foundMentor, FoundUser.DisplayName, entities[1], entities[2]);
                    goto MentorFound;
                }

                var mentorList = mentors.Where(userID => userID.id == mSet.ToList()[0]).ToList();
                foreach (var m in mentorList)
                {
                    if (m.id == App.loggedUser.id)
                        continue;
                    FoundUser = m;
                    msg.Text = $"I was able to find {FoundUser.DisplayName}, who works in {entities[1]}.";
                    goto MentorFound;
                }

                foreach (var m in skills.ToList())
                {
                    if (m.UserId == App.loggedUser.id)
                        continue;
                    var uList = await DatabaseService.Instance.client.GetTable<Users>().Where(u => u.id == m.UserId).ToListAsync();
                    FoundUser = uList[0];
                    msg.Text = $"I was able to find {FoundUser.DisplayName}, who is skilled with {entities[2]}.";
                    goto MentorFound;
                }

                // Unsuccessful mentor query
                msg.Text = string.Format(_notFound, entities[1], entities[2]);
                goto Finish;

                MentorFound:
                    msg.MentorSearch = true;
                    msg.Name = FoundUser.DisplayName;
                    msg.searchID = FoundUser.id;
                    msg.ProfileImage = await BlobService.Instance.TryDownloadImage("profile-images", msg.searchID);


                Finish:
                    MessageList.Add(msg);
                    App.assistU._chatHistory.Add(msg);

                await _botService.SendMessageAsync(""); // Signal continuation of dialog
            }


            /// <summary>
            /// Sends the message to the bot and updates the UI
            /// </summary>
            /// <returns></returns>
            public override async Task ExecuteSend()
            {
                try
                {
                    var msg = new Message() { Mine = true, Theirs = false, Text = TextDraft };
                    TextDraft = string.Empty;
                    MessageList.Add(msg);
                    await _botService.SendMessageAsync(msg.Text);
                    App.assistU._chatHistory.Add(msg);
                    _messageListView.ScrollTo(MessageList[MessageList.Count - 1], ScrollToPosition.MakeVisible, true);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Unable to send message. Exception => {e}");
                }
            }

            async void ViewProfile(object source)
            {
                if (FoundUser == null)
                    return;

                string ID = (string)source;
                if (FoundUser.id == ID)
                {
                    await Shell.Current.Navigation.PushAsync(new ViewOnlyProfilePage(FoundUser, false));
                }
                else
                {
                    var user = await DatabaseService.Instance.client.GetTable<Users>().Where(u => u.id == ID).ToListAsync();
                    await Shell.Current.Navigation.PushAsync(new ViewOnlyProfilePage(user[0], false));
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
            }
        }
    }
}
