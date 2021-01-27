using System;
using MentorU.Models;
using MentorU.Views;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;
using MentorU.Services.LogOn;

namespace MentorU.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public ObservableCollection<Users> Mentors { get; }
        public ObservableCollection<Items> MarketItems { get; }
        public Command LoadPageDataCommand { get; }
        public Command<Users> MentorTapped { get; }
        public Command<Items> ItemTapped { get; }
        private UserContext _user;
        public String UsersName { get; set; }
      
        public HomeViewModel()
        {
            _user = App.AADUser;
            UsersName = _user.GivenName;
            Title = "Home";
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
            _user = App.AADUser;

            IsBusy = true;

        }
    }
}