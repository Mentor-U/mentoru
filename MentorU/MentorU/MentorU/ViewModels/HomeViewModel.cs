using System;
using MentorU.Models;
using MentorU.Views;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Identity.Client;

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
        public string UsersName { get => authResult.UniqueId; }
        private Users _user;

        private AuthenticationResult authResult;

        public HomeViewModel()
        {
            Title = "Home";
            _user = DataStore.GetUser().Result;
            Mentors = new ObservableCollection<Users>();
            MarketItems = new ObservableCollection<Items>();
            LoadPageDataCommand = new Command(async () => await ExecuteLoadPageData());
            MentorTapped = new Command<Users>(OnMentorSelected);
            ItemTapped = new Command<Items>(OnItemSelected);
        }

        async Task ExecuteLoadPageData()
        {
            IsBusy = true;
            try
            {
                //TODO: pull mentor list data and market place data here

                //REMOVE: once above task has be done
                var mentors = await DataStore.GetMentorsAsync(true);
                foreach(var m in mentors) // TODO: adding all? maybe limit to top three
                {
                    Mentors.Add(m);
                }
                var items = await DataStore.GetItemsAsync(true);
                foreach(var i in items) // TODO: adding all? maybe limit to top three
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