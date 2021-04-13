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
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;
using System.Net;

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
        private string _ErrorMessage;
        private bool _showError;

        public string Name { get => name; set => SetProperty(ref name, value); }
        public string Field { get => field; set => SetProperty(ref field, value); }
        public string Bio { get => bio; set => SetProperty(ref bio, value); }

         public bool isMentor { get; set; }
        public bool isMentee { get; set; }
        
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

        public bool showError
        {
            get => _showError;
            set
            {
                _showError = value; OnPropertyChanged();
            }
        }

        private AddressInfo _selectedAddress;
        public AddressInfo selectedAddress
        {
            get => _selectedAddress;
            set
            {
                _selectedAddress = value; OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _ErrorMessage;
            set
            {
                _ErrorMessage = value;
                OnPropertyChanged();
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

        public Command AddressCommand { get; set; }

        public bool Standardview { get; set; }
        public bool IsConnected { get; set; }

        public const string GooglePlacesApiAutoCompletePath = "https://maps.googleapis.com/maps/api/place/autocomplete/json?key={0}&input={1}&components=country:us"; //Adding country:us limits results to us

        public const string GooglePlacesApiKey = "AIzaSyCYu_YWRQpx4H0LrQftSboewaW70mtq8EA";

        private static HttpClient _httpClientInstance;
        public static HttpClient HttpClientInstance => _httpClientInstance ?? (_httpClientInstance = new HttpClient());

        private ObservableCollection<AddressInfo> _addresses;
        public ObservableCollection<AddressInfo> Addresses
        {
            get => _addresses ?? (_addresses = new ObservableCollection<AddressInfo>());
            set
            {
                if (_addresses != value)
                {
                    _addresses = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _addressText;
        public string AddressText
        {
            get => _addressText;
            set
            {
                if (_addressText != value)
                {
                    _addressText = value;
                    OnPropertyChanged();
                }
            }
        }

        public async Task GetPlacesPredictionsAsync()
        {

            // TODO: Add throttle logic, Google begins denying requests if too many are made in a short amount of time

            CancellationToken cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(2)).Token;

            using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, string.Format(GooglePlacesApiAutoCompletePath, GooglePlacesApiKey, WebUtility.UrlEncode(_addressText))))
            { //Be sure to UrlEncode the search term they enter

                using (HttpResponseMessage message = await HttpClientInstance.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false))
                {
                    if (message.IsSuccessStatusCode)
                    {
                        string json = await message.Content.ReadAsStringAsync().ConfigureAwait(false);

                        PlacesLocationPredictions predictionList = await Task.Run(() => JsonConvert.DeserializeObject<PlacesLocationPredictions>(json)).ConfigureAwait(false);

                        if (predictionList.Status == "OK")
                        {

                            Addresses.Clear();

                            if (predictionList.Predictions.Count > 0)
                            {
                                showError = false;

                                foreach (Prediction prediction in predictionList.Predictions)
                                {
                                    Addresses.Add(new AddressInfo
                                    {
                                        Address = prediction.Description
                                    });
                                }
                            }
                        }
                        else
                        {
                            showError = true;
                            ErrorMessage = predictionList.Status;
                            Addresses.Clear();
                               
                            //throw new Exception(predictionList.Status);
                        }
                    }
                }
            }
        }


        public ViewOnlyProfileViewModel(Users user, bool isConnected=false, bool fromNotification=false)
        {
            _user = user;
            Name = _user.FirstName;
            Field = _user.Major;
            Bio = _user.Bio;
            FromNotification = fromNotification;
            //Role = _user.Role == "0" ? "Skills:" : "Classes:";

            if (_user.Role == "0")
            {
                isMentor = true;
                isMentee = false;
            }
            else
            {
                isMentor = false;
                isMentee = true;
            }

            Standardview = !fromNotification;
            IsConnected = isConnected;

            Classes = new ObservableCollection<string>();
            LoadData();

            AcceptCommand = new Command(async () => await Accept());
            DeclineCommand = new Command(async () => await Decline());
            CancelClicked = new Command(async() => await OnCancel());
            ConfirmClicked = new Command(async () => await OnConfirm());

            AddressCommand = new Command(() => { AddressText = selectedAddress.Address;});

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


            var settingsList = await DatabaseService.Instance.client.GetTable<Settings>().Where(u => u.UserID == _user.id).ToListAsync();
            Email = settingsList.Count > 0 ? _user.Email : "";

            if (settingsList.Count > 0)
            {
                if (settingsList[0].AllEmailSettings == true )
                {
                    showEmail = true;
                }
                if (settingsList[0].ConnectionEmailSettings == true)
                {
                    var connectionsList1 = await DatabaseService.Instance.client.GetTable<Connection>().Where((u => u.MenteeID == _user.id && u.MentorID == App.loggedUser.id)).ToListAsync();
                    var connectionsList2 = await DatabaseService.Instance.client.GetTable<Connection>().Where((u => u.MentorID == _user.id && u.MenteeID == App.loggedUser.id)).ToListAsync();

                    if(connectionsList1.Count > 0 || connectionsList2.Count > 0)
                    {
                        showEmail = true;
                    }

                }

            }
            else
            {
                showEmail = false;
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

                var c = await DatabaseService.Instance.client.GetTable<Connection>()
                    .Where(u => (u.MenteeID == _user.id && u.MentorID == App.loggedUser.id) || (u.MentorID == _user.id && u.MenteeID == App.loggedUser.id))
                    .ToListAsync();
                await DatabaseService.Instance.client.GetTable<Connection>().DeleteAsync(c[0]);

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
                Text = _scheduleMessage + " " + SelectedTime.ToString(@"h\:mm") + " @ " + AddressText,
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
