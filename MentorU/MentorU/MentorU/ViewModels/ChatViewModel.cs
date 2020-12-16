using MentorU.Models;
using MentorU.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        private string _textDraft;

        public ObservableCollection<Message> Messages { get; }
        public string TextDraft { get => _textDraft; set { _textDraft = value; OnPropertyChanged(); } }
        public Command OnSendCommand { get; set; }
        public Command LoadPageData { get; set; }


        public ChatViewModel(Users ChatRecipient)
        {
            Title = ChatRecipient.FirstName;
            Messages = new ObservableCollection<Message>();
            OnSendCommand = new Command(async () => await ExecuteSend());
            LoadPageData = new Command(async () => await ExecuteLoadPageData());
        }


        async Task ExecuteLoadPageData()
        {
            IsBusy = true;
            try
            {
                // TODO: load message history
                List<Message> messages = new List<Message>(); //REMOVE ME: (placeholder)
                messages.Add(new Message() { User = App.loggedUser, Mine = true, Theirs = false, Text = "Hello There" });
                messages.Add(new Message { User = new Users { FirstName = "Bob"}, Mine = false, Theirs = true, Text = "Hi, how are you today? What can I help you with? " });
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
                // TODO: add signalR stuffs

                Messages.Add(new Message() { User = App.loggedUser, Mine = true, Theirs = false, Text = TextDraft });
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
