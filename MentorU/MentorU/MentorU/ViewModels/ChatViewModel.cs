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

        public ObservableCollection<Message> Messages { get; }
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

            Messages = new ObservableCollection<Message>();
            OnSendCommand = new Command(async () => await ExecuteSend());
            LoadPageData = new Command(async () => { await ExecuteLoadPageData(); await Connect(); });
            ConnectChat = new Command(async () => await Connect());


            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{App.SignalRBackendUrl}")
                //, // This is a work around to avoid SSL errors when run on localhost
                //(opts) =>
                //{
                //    opts.HttpMessageHandlerFactory = (message) =>
                //    {
                //        if (message is HttpClientHandler clientHandler)
                //                
                //                clientHandler.ServerCertificateCustomValidationCallback +=
                //                (sender, certificate, chain, sslPolicyErrors) => { return true; };
                //        return message;
                //    };
                //})
                .Build();

            hubConnection.On<string,string>("ReceiveMessage", (userID, message) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        if (userID == _recipient.id)
                            Messages.Add(new Message() { User = _recipient, Mine = false, Theirs = true, Text = message });
                        else
                            Messages.Add(new Message() { User = App.loggedUser, Mine = true, Theirs = false, Text = message });
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
                // TODO: load message history from database
                List<Message> messages = new List<Message>(); //REMOVE ME: (placeholder)
                foreach (var m in messages)
                {
                    Messages.Add(m);
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
                TextDraft = "";
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }


        public void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
