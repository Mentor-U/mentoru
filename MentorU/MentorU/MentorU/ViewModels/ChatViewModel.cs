using MentorU.Models;
using MentorU.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Net.Http;

namespace MentorU.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        private string _textDraft;
        private Users _recipient;

        public ObservableCollection<Message> MessageList { get; }
        public string TextDraft { get => _textDraft; set { _textDraft = value; OnPropertyChanged(); } }
        public Command OnSendCommand { get; set; }
        public Command LoadPageData { get; set; }
        public Command ConnectChat { get; set; }

        private HubConnection hubConnection;
        private bool hubIsConnected = false;
        private string _groupName;

        public ChatViewModel(Users ChatRecipient)
        {
            Title = ChatRecipient.FirstName;
            _recipient = ChatRecipient;

            if (int.Parse(_recipient.id) < int.Parse(App.loggedUser.id))
                _groupName = _recipient.id + "-" + App.loggedUser.id;
            else
                _groupName = App.loggedUser.id + "-" + _recipient.id;

            MessageList = new ObservableCollection<Message>();
            OnSendCommand = new Command(async () => await ExecuteSend());
            LoadPageData = new Command(async () => { await ExecuteLoadPageData(); await Connect(); });
            ConnectChat = new Command(async () => await Connect());


            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{App.SignalRBackendUrl}"
                , // This is a work around to avoid SSL errors when run on localhost
                (opts) =>
                {
                    opts.HttpMessageHandlerFactory = (message) =>
                    {
                        if (message is HttpClientHandler clientHandler)

                            clientHandler.ServerCertificateCustomValidationCallback +=
                            (sender, certificate, chain, sslPolicyErrors) => { return true; };
                        return message;
                    };
                })
                .Build();

            hubConnection.On<string,string>("ReceiveMessage", (userID, message) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        if (userID == _recipient.id)
                            MessageList.Add(new Message() { UserID = _recipient.id, Mine = false, Theirs = true, Text = message });
                        else
                            MessageList.Add(new Message() { UserID = App.loggedUser.id, Mine = true, Theirs = false, Text = message });
                    }
                    catch(Exception ex)
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


        async Task ExecuteLoadPageData()
        {
            IsBusy = true;
            try
            {
                // Load message history from database
                var history = await App.client.GetTable<Messages>().OrderBy(u => u.TimeStamp).Where(u => u.GroupName == _groupName).ToListAsync();
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
            }
            catch(Exception ex)
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
                await App.client.GetTable<Messages>().InsertAsync(newMessage);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

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
