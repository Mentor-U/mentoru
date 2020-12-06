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
        public Command<User> MentorTapped { get; }
        public ObservableCollection<User> Mentors { get; }

        public SearchNewMentorViewModel()
        {
            Title = "Find New Mentors";
            Mentors = new ObservableCollection<User>();
            LoadMentorsCommand = new Command(async () => await ExecuteLoadMentors());
            FilterCommand = new Command(async () => await ExecuteFilterMentors());
            MentorTapped = new Command<User>(OnMentorSelected);
        }

        async Task ExecuteLoadMentors(object filters = null)
        {
            IsBusy = true;
            try
            {
                Mentors.Clear();

                // FIXME: determine correct DB parameters for getting available mentors and filtering

                if (filters != null)
                {
                    //await App.clientt.getTable<User>().Where(filters).Read();
                }
                else
                {
                    //await App.clientt.getTable<User>().Where(m => m.isMentor == true).Read();
                }

                // REMOVE: once above is implemented
                User u1 = new User() { Name = "Bob", Major = "Art", Bio = "Pottery is my favorite" , UserID = 10};
                User u2 = new User() { Name = "Jerry", Major = "Comedy", Bio = "I love to make people laugh" , UserID = 11};
                User u3 = new User() { Name = "Jonny", Major = "Computer Science", Bio = "I love Machine Learning" , UserID = 12};
                Mentors.Add(u1);
                Mentors.Add(u2);
                Mentors.Add(u3);
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


        async void OnMentorSelected(User mentor)
        {
            if (mentor == null)
                return;
            await Shell.Current.GoToAsync($"{nameof(ViewOnlyProfilePage)}?{nameof(ViewOnlyProfileViewModel.UserID)}={mentor.UserID}");
            //TODO: Inherit from the viewonly? and have page button request connection
            //TODO: after merge convert from above to the below code

            //await Shell.Current.Navigation.PushModalAsync(new ViewOnlyProfilePage(mentor));
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
