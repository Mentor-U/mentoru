using System;
using MentorU.Models;
using MentorU.Views;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Identity.Client;
using System.Collections.Generic;

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

        private AuthenticationResult authResult;

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
            UsersName = App.loggedUser.FirstName;

            if(App.loggedUser.Role == "0")
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

        async Task ExecuteLoadPageData()
        {
            IsBusy = true;
            try
            {
                Mentors.Clear();
                List<Connection> mentors;

                if(isMentee)
                {
                    mentors = await App.client.GetTable<Connection>().Where(u => u.MenteeID == App.loggedUser.id).ToListAsync();
                    foreach (var m in mentors)
                    {
                        var temp = await App.client.GetTable<Users>().Where(u => u.id == m.MentorID).ToListAsync();
                        Mentors.Add(temp[0]);
                    }
                }
                else
                {
                    mentors = await App.client.GetTable<Connection>().Where(u => u.MentorID == App.loggedUser.id).ToListAsync();
                    foreach (var m in mentors)
                    {
                        var temp = await App.client.GetTable<Users>().Where(u => u.id == m.MenteeID).ToListAsync();
                        Mentors.Add(temp[0]);
                    }
                }
                
                //var items = await DataStore.GetItemsAsync(true);
                //foreach(var i in items) // TODO: adding all? maybe limit to top three
                //{
                //    MarketItems.Add(i);
                //}
                
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


        async void OnMentorSelected(Users mentor)
        {
            if (mentor == null)
                return;
            await Shell.Current.Navigation.PushAsync(new ViewOnlyProfilePage(mentor, true));
        }



        async void OnItemSelected(Items item)
        {
            if (item == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.id}");
        }


        public void OnAppearing()
        {
            IsBusy = true;
        }
    }
}