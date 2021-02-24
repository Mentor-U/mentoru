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

namespace MentorU.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        private string _textDraft;
        private Users _recipient;
        private ObservableCollection<Message> _messageList;
        
        public ObservableCollection<Message> MessageList { get => _messageList; set { _messageList = value; OnPropertyChanged(); } }
        public string TextDraft { get => _textDraft; set { _textDraft = value; OnPropertyChanged(); } }
        public Command OnSendCommand { get; set; }
        public Command LoadPageData { get; set; }
        public Command ConnectChat { get; set; }
        public Command DisconnectChat { get; set; }

        private HubConnection hubConnection;
        private bool hubIsConnected = false;
        private string _groupName;

        public ChatViewModel(Users ChatRecipient)
        {
            Title = ChatRecipient.FirstName;
            _recipient = ChatRecipient;

            // Use bit mask of user id's to generate group name 
            byte[] them = Encoding.ASCII.GetBytes(_recipient.id);
            byte[] me = Encoding.ASCII.GetBytes(App.loggedUser.id);
            List<int> masked = new List<int>();
            for (int i = 0; i < them.Length; i++)
                masked.Add(them[i] & me[i]);

            _groupName = string.Join("", masked);

            MessageList = new ObservableCollection<Message>();
            OnSendCommand = new Command(async () => await ExecuteSend());
            LoadPageData = new Command(async () => { await ExecuteLoadPageData(); await Connect(); });
            ConnectChat = new Command(async () => await Connect());
            DisconnectChat = new Command(async () => await Disconnect());


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
                         
                         if (userID == _recipient.id)
                             MessageList.Add(new Message() { UserID = _recipient.id, Mine = false, Theirs = true, Text = message });
                         else
                             MessageList.Add(new Message() { UserID = App.loggedUser.id, Mine = true, Theirs = false, Text = message });
                         App._cache.Set(_groupName, MessageList, new TimeSpan(0,2,0));

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


        async Task ExecuteLoadPageData()
        {
            //IsBusy = true;
            try
            {   
                object his;
                List<Messages> history;
                if (App._cache.TryGetValue(_groupName, out his))
                {
                    MessageList = (ObservableCollection<Message>)his;
                    Debug.WriteLine("Pull from cache");
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
                            Mine = m.UserID == _recipient.id ? false : true,
                            Theirs = m.UserID == _recipient.id ? true : false
                        });
                    }
                    App._cache.Set(_groupName, MessageList, new TimeSpan(0,2,0));
                }
                
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }


        async Task ExecuteSend()
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


        public void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
