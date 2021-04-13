using System;
using MentorU.Models;
using MentorU.Views;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using MentorU.Services.DatabaseServices;
using MentorU.Services.Blob;

namespace MentorU.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public ObservableCollection<Users> Mentors { get; }
        public ObservableCollection<Items> MarketItems { get; }
        public Command LoadPageDataCommand { get; }
        public Command<Users> MentorTapped { get; }
        public Command<Items> ItemTapped { get; }
        private string _usersName;
        public string UsersName
        {
            get => _usersName;
            set
            {
                _usersName = value;
                OnPropertyChanged();
            }
        }

        // UI flags to control visibility
        public bool isMentor { get; set; }
        public bool isMentee { get; set; }

        public HomeViewModel()
        {
            Title = "Home";
            Mentors = new ObservableCollection<Users>();
            MarketItems = new ObservableCollection<Items>();
            LoadPageDataCommand = new Command(async () => await ExecuteLoadPageData());
            MentorTapped = new Command<Users>(OnMentorSelected);
            ItemTapped = new Command<Items>(OnItemSelected);
            UsersName = App.loggedUser.DisplayName;

            if (App.loggedUser.Role == "0")
            {
                isMentor = true;
                isMentee = false;
            }
            else
            {
                isMentor = false;
                isMentee = true;
            }
        }

        /// <summary>
        /// Load data from the data base to populate fields in the view
        /// </summary>
        /// <returns></returns>
        async Task ExecuteLoadPageData()
        {
            IsBusy = true;
            try
            {
                Mentors.Clear();
                List<Connection> mentors;

                mentors = await DatabaseService.Instance.client.GetTable<Connection>().Where(u => u.MenteeID == App.loggedUser.id).ToListAsync();
                foreach (var m in mentors)
                {
                    var temp = await DatabaseService.Instance.client.GetTable<Users>().Where(u => u.id == m.MentorID).ToListAsync();
                    var current = temp[0];
                    current.ProfileImage = await BlobService.Instance.TryDownloadImage("profile-images", current.id);
                    Mentors.Add(current);
                }

                mentors = await DatabaseService.Instance.client.GetTable<Connection>().Where(u => u.MentorID == App.loggedUser.id).ToListAsync();
                foreach (var m in mentors)
                {
                    var temp = await DatabaseService.Instance.client.GetTable<Users>().Where(u => u.id == m.MenteeID).ToListAsync();
                    var current = temp[0];
                    current.ProfileImage = await BlobService.Instance.TryDownloadImage("profile-images", current.id);
                    Mentors.Add(current);
                }

                if (Mentors.Count == 0)
                {
                    Mentors.Add(new Users() { DisplayName = "No current connections", Major = "Click to browse  list", Role = "-1", RoleSelector="" });
                }

                //Load marketplace items
                MarketItems.Clear();
                // List<Items> items = await DatabaseService.Instance.client.GetTable<Items>().Where(u => u.Owner != App.loggedUser.id).ToListAsync();
                List<Items> items = await App.assistU.GetRecommendations();
                foreach(var i in items)
                {
                    MarketItems.Add(i);
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


        /// <summary>
        /// Navigate to the selected users profile page.
        /// </summary>
        /// <param name="mentor"></param>
        async void OnMentorSelected(Users mentor)
        {
            if (mentor == null)
                return;
            else if(mentor.Role == "-1")
                await Shell.Current.Navigation.PushAsync(new SearchNewMentorPage());
            else
                await Shell.Current.Navigation.PushAsync(new ViewOnlyProfilePage(mentor, true));
        }


        /// <summary>
        /// Open the page of the item that was selected
        /// </summary>
        /// <param name="item"></param>
        async void OnItemSelected(Items item)
        {
            if (item == null)
                return;
            // This will push the ItemDetailPage onto the navigation stack
            //await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.id}");
            else
                await Shell.Current.Navigation.PushAsync(new ItemDetailPage(item));
        }


        public void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
