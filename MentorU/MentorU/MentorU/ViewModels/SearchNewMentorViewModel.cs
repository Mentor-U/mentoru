using MentorU.Models;
using MentorU.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    class SearchNewMentorViewModel : BaseViewModel
    {
        public Command LoadMentorsCommand { get; }
        public Command FilterCommand { get; }
        public Command<Users> MentorTapped { get; }
        public ObservableCollection<Users> Mentors { get; }

        public SearchNewMentorViewModel()
        {
            Title = "Find New Mentors";
            Mentors = new ObservableCollection<Users>();
            LoadMentorsCommand = new Command(async () => await ExecuteLoadMentors());
            FilterCommand = new Command(async () => await ExecuteFilterMentors());
            MentorTapped = new Command<Users>(OnMentorSelected);
        }

        async Task ExecuteLoadMentors(object filters = null)
        {
            IsBusy = true;
            try
            {
                Mentors.Clear();

                // FIXME: determine correct DB parameters for getting available mentors and filtering

                //if (filters != null)
                //{
                //    var temp = await App.client.GetTable<Users>().Select(user => user.Role == 0).ToListAsync();
                //}
                //else()
                //{
                //    await App.clientt.getTable<User>().Where(m => m.isMentor == true).Read();
                //}

                var temp = await App.client.GetTable<Users>().Where(user => user.Role == 0).ToListAsync();

                foreach(Users element in temp)
                {
                    Mentors.Add(element);
                }

                //// REMOVE: once above is implemented)
                //Users u1 = new Users() { FirstName = "Bob", Major = "Art", Bio = "Pottery is my favorite" , id = "10"};
                //Users u2 = new Users() { FirstName = "Jerry", Major = "Comedy", Bio = "I love to make people laugh" , id = "11"};
                //Users u3 = new Users() { FirstName = "Jonny", Major = "Computer Science", Bio = "I love Machine Learning" , id = "12"};
                //Mentors.Add(u1);
                //Mentors.Add(u2);
                //Mentors.Add(u3);
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

        async Task ExecuteFilterMentors()
        {
            //TODO: determine the structure of filtering options. right now does nothing
            var filters = await Application.Current.MainPage.DisplayPromptAsync("Filter", "Select Filters");
            await ExecuteLoadMentors(filters);
        }


        async void OnMentorSelected(Users mentor)
        {
            if (mentor == null)
                return;
            await Shell.Current.GoToAsync($"{nameof(ViewOnlyProfilePage)}?{nameof(ViewOnlyProfileViewModel.UserID)}={mentor.id}");
            //TODO: Inherit from the viewonly? and have page button request connection
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
