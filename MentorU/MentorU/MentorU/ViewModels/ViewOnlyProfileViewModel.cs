using MentorU.Models;
using MentorU.Services.Blob;
using MentorU.Services.DatabaseServices;
using MentorU.Views.ChatViews;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Rg.Plugins.Popup.Services;
using MentorU.Views;

namespace MentorU.ViewModels
{
    public class ViewOnlyProfileViewModel : BaseViewModel
    {
        private string name;
        private string field;
        private string bio;
        private TimeSpan _selectedTime;
        private Users _user;
        public string _scheduleMessage;
        private ImageSource _profileImage;
        private string _email;
        private bool _showEmail;

        public string Name { get => name; set => SetProperty(ref name, value); }
        public string Field { get => field; set => SetProperty(ref field, value); }
        public string Bio { get => bio; set => SetProperty(ref bio, value); }
        
        public TimeSpan SelectedTime
        {
            get => _selectedTime;
            set
            {
                _selectedTime = value;
                OnPropertyChanged();
            }
        }

        public ImageSource ProfileImage
        {
            get => _profileImage;
            set
            {
                _profileImage = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public bool showEmail
        {
            get => _showEmail;
            set
            {
                _showEmail = value; OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Classes { get; set; }
        public string Role { get; set; }

        public bool FromNotification { get; set; }

        public Command AcceptCommand {get; set;}
        public Command DeclineCommand { get; set; }
        public Command CancelClicked { get; set; }
        public Command ConfirmClicked { get; set; }
        public Command ChatCommand { get; set; }
        public Command ScheduleCommand { get; set; }

        public bool Standardview { get; set; }
        public bool IsConnected { get; set; }

        public ViewOnlyProfileViewModel(Users user, bool isConnected=false, bool fromNotification=false)
        {
            _user = user;
            Name = _user.FirstName;
            Field = _user.Major;
            Bio = _user.Bio;
            FromNotification = fromNotification;
            Role = _user.Role == "0" ? "Skills:" : "Classes:";

            Standardview = !fromNotification;
            IsConnected = isConnected;

            Classes = new ObservableCollection<string>();
            LoadData();

            AcceptCommand = new Command(async () => await Accept());
            DeclineCommand = new Command(async () => await Decline());
            CancelClicked = new Command(async() => await OnCancel());
            ConfirmClicked = new Command(async () => await OnConfirm());

            ChatCommand = new Command(StartChat);
            ScheduleCommand = new Command(ScheduleMeeting);
        }


        async void LoadData()
        {
            List<Classes> c = await DatabaseService.Instance.client.GetTable<Classes>()
                .Where(u => u.UserId == _user.id)
                .ToListAsync();

            foreach (Classes val in c)
            {
                Classes.Add(val.ClassName);
            }
        }

        /** ------------------------------------------------------
        * Below is the view for users that have not connected 
        * and are looking to connect with them
        * --------------------------------------------------------
        */

        ///<summary>
        /// Sends a request to the mentor requesting a connection be made.
        ///</summary>
        public async void OnRequestMentor()
        {
            try {
                await DatabaseService.Instance.client.GetTable<Notification>().InsertAsync(new Notification()
                {
                    MentorID = _user.id,
                    MenteeID = App.loggedUser.id,
                    Message = $"{App.loggedUser.FirstName} wants to connect!",
                    Seen = false,
                    Unseen = true
                });
                await Application.Current.MainPage.DisplayAlert(
                    "You have sent a request to " + _user.FirstName,
                    "We'll let you know if they accept your request!",
                    "OK");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
         }



        /** --------------------------------------------
        * Below is the view for users that are connected 
        * ----------------------------------------------
        */


        ///<summary>
        /// Opens the chat window with the associated user.
        ///</summary>
        public async void StartChat(object obj)
        {
            await Shell.Current.Navigation.PopToRootAsync(false); // false -> disables navigation animation
            await Shell.Current.Navigation.PushAsync(new ChatPage(_user));
        }

        /// <summary>
        /// Options pane allows users to remove connections that they have.
        /// ~ Left as Options in case we want additional options and can do a selection alert ~
        /// </summary>
        public async void OpenOptions()
        {
            bool remove = await App.Current.MainPage.DisplayAlert("Options", $"Remove {_user.DisplayName} from your connections?", "Yes", "No");
            if (remove)
            {
                // Mentor removes mentee
                if (App.loggedUser.Role == "0") { 
                    var con = await DatabaseService.Instance.client.GetTable<Connection>()
                        .Where(u => u.MenteeID == _user.id && u.MentorID == App.loggedUser.id).ToListAsync();
                    await DatabaseService.Instance.client.GetTable<Connection>().DeleteAsync(con[0]);
                    
                }
                // Mentee removes mentor
                else
                {
                    var con = await DatabaseService.Instance.client.GetTable<Connection>()
                        .Where(u => u.MentorID == _user.id && u.MenteeID == App.loggedUser.id).ToListAsync();
                    await DatabaseService.Instance.client.GetTable<Connection>().DeleteAsync(con[0]);
                }

                //// Delete message history
                //byte[] them = Encoding.ASCII.GetBytes(_user.id);
                //byte[] me = Encoding.ASCII.GetBytes(App.loggedUser.id);
                //List<int> masked = new List<int>();

                //for (int i = 0; i < them.Length; i++)
                //    masked.Add(them[i] & me[i]);

                //var groupName = string.Join("", masked);

                //var messages = await DatabaseService.Instance.client.GetTable<ChatViewModel.Messages>()
                //    .Where(m => m.GroupName == groupName).ToListAsync();

                //foreach (var m in messages)
                //    await DatabaseService.Instance.client.GetTable<ChatViewModel.Messages>().DeleteAsync(m);

                await App.Current.MainPage.Navigation.PopToRootAsync(); // after removal exit window
            }
        }

        /// <summary>
        /// Schedule a meeting with mentor/mentee so that later on we will have a push notification to remind the meeting to users.
        /// </summary>
        public async void ScheduleMeeting()
        {
            await PopupNavigation.Instance.PushAsync(new PopUpSchedule(this));
            

        }

        async Task OnCancel()
        {
            await PopupNavigation.Instance.PopAllAsync();
        }

        async Task OnConfirm()
        {
            byte[] them = Encoding.ASCII.GetBytes(_user.id);
            byte[] me = Encoding.ASCII.GetBytes(App.loggedUser.id);
            List<int> masked = new List<int>();
            for (int i = 0; i < them.Length; i++)
                masked.Add(them[i] & me[i]);

            string _groupName = string.Join("", masked);

            ChatViewModel.Messages newMessage = new ChatViewModel.Messages
            {
                Text = _scheduleMessage + " " + SelectedTime.ToString(@"h\:mm"),
                UserID = App.loggedUser.id,
                GroupName = _groupName,
                TimeStamp = DateTime.Now
            };
            await DatabaseService.Instance.client.GetTable<ChatViewModel.Messages>().InsertAsync(newMessage);
            await PopupNavigation.Instance.PopAllAsync();
        }

        /** ------------------------------------------------------------------------------------
         * Below is the view for users that have not connected and have received a request to
         * from another user 
         * -------------------------------------------------------------------------------------
         */


        async Task Accept()
        {
            bool confirm = await Application.Current.MainPage
                .DisplayAlert("Confirm", $"Do you want to connect with {_user.FirstName}", "Accept", "Cancel");
            if (confirm)
            {
                await DatabaseService.Instance.client.GetTable<Connection>().InsertAsync(new Connection()
                {
                    MentorID = App.loggedUser.id,
                    MenteeID = _user.id
                });

                await App.Current.MainPage.DisplayAlert("Connected!", $"You have connected with {_user.FirstName}", "OK");
                var notification = await DatabaseService.Instance.client.GetTable<Notification>()
                    .Where(u => u.MentorID == App.loggedUser.id && u.MenteeID == _user.id).ToListAsync();
                await DatabaseService.Instance.client.GetTable<Notification>().DeleteAsync(notification[0]);
                await App.Current.MainPage.Navigation.PopAsync();
            }
        }

        async Task Decline()
        {
            bool confirm = await Application.Current.MainPage
                .DisplayAlert("Confirm", $"Do you want to decline connection with {_user.FirstName}", "Accept", "Cancel");
            if (confirm)
            {
                var notification = await DatabaseService.Instance.client.GetTable<Notification>()
                    .Where(u => u.MentorID == App.loggedUser.id && u.MenteeID == _user.id).ToListAsync();
                await DatabaseService.Instance.client.GetTable<Notification>().DeleteAsync(notification[0]);
                await App.Current.MainPage
                    .DisplayAlert("Declining Connection.", $"You have declined the connection with {_user.FirstName}", "OK");
                await App.Current.MainPage.Navigation.PopAsync();
            }
        }


        public async Task OnAppearing()
        {
            IsBusy = true;
            ProfileImage = await BlobService.Instance.TryDownloadImage("profile-images", _user.id);
        }

    }
}
