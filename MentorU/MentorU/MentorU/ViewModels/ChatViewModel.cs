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
            _recipient = ChatRecipient;
            Title = ChatRecipient.FirstName;
            Messages = new ObservableCollection<Message>();
            OnSendCommand = new Command(async () => await ExecuteSend());
            LoadPageData = new Command(async () => await ExecuteLoadPageData());
            ConnectChat = new Command(async () => await Connect());

            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{App.SignalRBackendUrl}/messages",
                (opts) =>
                {
                    opts.HttpMessageHandlerFactory = (message) =>
                    {
                        if (message is HttpClientHandler clientHandler)
                            // FIXME: This is a TEMPORARY work around to avoid SSL errors and pulled from ->
                            // https://stackoverflow.com/questions/60794798/signalr-with-xamarin-client-getting-certificate-verify-failed-on-calling-start
                            clientHandler.ServerCertificateCustomValidationCallback +=
                                (sender, certificate, chain, sslPolicyErrors) => { return true; };
                        return message;
                    };
                })
                .Build();

            _groupName = _recipient.FirstName + App.loggedUser.FirstName;
           //hubConnection.InvokeAsync("AddToGroup", _groupName);

            hubConnection.On<string>("ReceiveMessage", (message) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        Messages.Add(new Message() { User = _recipient, Mine = false, Theirs = true, Text = message });
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
            hubIsConnected = true;
        }


        async Task ExecuteLoadPageData()
        {
            IsBusy = true;
            try
            {
                // TODO: load message history
                List<Message> messages = new List<Message>(); //REMOVE ME: (placeholder)
                messages.Add(new Message() { User = App.loggedUser, Mine = true, Theirs = false, Text = "Hello There" });
                messages.Add(new Message { User = _recipient, Mine = false, Theirs = true, Text = "Hi, how are you today? What can I help you with? " });
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
                    await hubConnection.StartAsync();
                    hubIsConnected = true;
                }
                Message m = new Message() { User = App.loggedUser, Mine = true, Theirs = false, Text = TextDraft };
                await hubConnection.InvokeAsync("SendMessage", _groupName, m.Text);
                Messages.Add(m);
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
