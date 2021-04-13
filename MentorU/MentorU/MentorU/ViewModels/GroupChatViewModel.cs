using MentorU.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Microsoft.AspNetCore.SignalR.Client;
using System.Text;
using MentorU.Services.DatabaseServices;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using MentorU.Views.ChatViews;

namespace MentorU.ViewModels
{
    public class GroupChatViewModel : BaseViewModel
    {

        private string _textDraft;
        private ObservableCollection<Message> _messageList;

        public ObservableCollection<Message> MessageList { get => _messageList; set { _messageList = value; OnPropertyChanged(); } }
        public string TextDraft { get => _textDraft; set { _textDraft = value; OnPropertyChanged(); } }
        public Command OnSendCommand { get; set; }
        public Command LoadPageData { get; set; }
        public Command ConnectChat { get; set; }
        public Command DisconnectChat { get; set; }
        public Command RefreshChatCommand { get; set; }
        public Command ViewProfileCommand { get; set; }

        private HubConnection hubConnection;
        private bool hubIsConnected = false;
        private string _groupName;
        private bool _useCache;

        public ListView _messageListView { get; set; }

        public GroupChatViewModel(string GroupName)
        {

            Title = GroupName;
            _useCache = true;

            _messageListView = null;
            _groupName = GroupName;

            MessageList = new ObservableCollection<Message>();
            OnSendCommand = new Command(async () => await ExecuteSend());
            LoadPageData = new Command(async () => { await ExecuteLoadPageData(); await Connect(); });
            ConnectChat = new Command(async () => await Connect());
            DisconnectChat = new Command(async () => await Disconnect());
            RefreshChatCommand = new Command(() => { _useCache = false; IsBusy = true; });
            ViewProfileCommand = null; // used by assistU

            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{App.SignalRBackendUrl}")
                .Build();

            // Receiving messages callback
            hubConnection.On<string, string>("ReceiveMessage", (userID, message) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    try
                    {

                        if (userID != App.loggedUser.id)
                            MessageList.Add(new Message() { UserID = userID, Mine = false, Theirs = true, Text = message });
                        else
                            MessageList.Add(new Message() { UserID = App.loggedUser.id, Mine = true, Theirs = false, Text = message });
                        App._cache.Set(_groupName, MessageList, new TimeSpan(24, 0, 0));
                        _messageListView.ScrollTo(MessageList[MessageList.Count - 1], ScrollToPosition.MakeVisible, true);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                });
            });

        }


        async Task Connect()
        {
            if (!hubIsConnected)
                await hubConnection.StartAsync();
            await hubConnection.InvokeAsync("AddToGroup", _groupName);
            hubIsConnected = true;
        }

        public async Task Disconnect()
        {
            await hubConnection.InvokeAsync("RemoveFromGroup", _groupName);
            hubIsConnected = false;
        }


        public virtual async Task ExecuteLoadPageData()
        {
            try
            {
                object his;
                List<Messages> history;
                if (_useCache && App._cache.TryGetValue(_groupName, out his))
                {
                    MessageList = (ObservableCollection<Message>)his;
                }
                else
                {
                    MessageList.Clear();
                    history = await DatabaseService.Instance.client.GetTable<Messages>()
                    .OrderBy(u => u.TimeStamp).Where(u => u.GroupName == _groupName).ToListAsync();
                    foreach (var m in history)
                    {
                        MessageList.Add(new Message()
                        {
                            Text = m.Text,
                            UserID = m.UserID,
                            Mine = m.UserID != App.loggedUser.id ? false : true,
                            Theirs = m.UserID != App.loggedUser.id ? true : false
                        });
                    }
                    App._cache.Set(_groupName, MessageList, new TimeSpan(24, 0, 0));
                    _useCache = true;
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
                if (MessageList.Count > 0)
                    _messageListView.ScrollTo(MessageList[MessageList.Count - 1], ScrollToPosition.MakeVisible, true);
            }
        }

        public virtual async Task ExecuteSend()
        {
            try
            {
                if (!hubIsConnected)
                {
                    await Connect();
                }

                await hubConnection.InvokeAsync("SendMessage", _groupName, App.loggedUser.id, TextDraft);
                Messages newMessage = new Messages
                {
                    Text = TextDraft,
                    UserID = App.loggedUser.id,
                    GroupName = _groupName,
                    TimeStamp = DateTime.Now
                };
                TextDraft = "";
                await DatabaseService.Instance.client.GetTable<Messages>().InsertAsync(newMessage);
                _messageListView.ScrollTo(MessageList[MessageList.Count - 1], ScrollToPosition.MakeVisible, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        // Model for the database which excludes UI specific info
        public class Messages
        {
            public string id { get; set; }
            public string Text { get; set; }
            public string UserID { get; set; }
            public string GroupName { get; set; }
            public DateTime TimeStamp { get; set; }
        }


        public virtual void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
