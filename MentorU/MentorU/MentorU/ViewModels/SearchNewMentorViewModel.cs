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
        public ObservableCollection<string> Filters { get; }

        public SearchNewMentorViewModel()
        {
            Title = "Find New Mentors";
            Mentors = new ObservableCollection<Users>();
            Filters = new ObservableCollection<string>();
            LoadMentorsCommand = new Command(async () => await ExecuteLoadMentors());
            FilterCommand = new Command(async () => await ExecuteFilterMentors());
            MentorTapped = new Command<Users>(OnMentorSelected);
        }

        async Task ExecuteLoadMentors(string filter = null)
        {
            IsBusy = true;
            try
            {
                Mentors.Clear();

                // FIXME: determine correct DB parameters for getting available mentors and filtering

                if (filter != null)
                {
                    //await App.clientt.getTable<User>().Where(filters).Read();
                    var mentors = await DataStore.GetAvailableMentors();
                    foreach (var m in mentors)
                    {
                        if (filter == "All" || Filters.Contains(m.Major))
                            Mentors.Add(m);
                    }
                }
                else
                {
                    //await App.clientt.getTable<User>().Where(m => m.isMentor == true).Read();

                    // REMOVE: once above is implemented
                    var mentors = await DataStore.GetAvailableMentors();
                    foreach (var m in mentors)
                    {
                        Mentors.Add(m);
                    }
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

        async Task ExecuteFilterMentors()
        {
            //TODO: determine the structure of filtering options. right now does nothing
            var filters = await Application.Current.MainPage.DisplayPromptAsync("Filter", "Enter a Filter");
            Filters.Add(filters.ToString());
            await ExecuteLoadMentors(filters);
        }


        async void OnMentorSelected(Users mentor)
        {
            if (mentor == null)
                return;
            await Shell.Current.Navigation.PushAsync(new ViewOnlyProfilePage(mentor, false));
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }
    }
}
