using MentorU.Models;
using MentorU.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using MentorU.Views.ChatViews;
using System.Linq;
using MentorU.Services.DatabaseServices;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;

namespace MentorU.ViewModels
{
    public class SearchNewMentorViewModel : BaseViewModel
    {
        public Command LoadMentorsCommand { get; }
        public Command FilterCommand { get; }
        public Command ClearFilters { get; }
        public Command<Users> MentorTapped { get; }
        public Command OpenAssistU { get; }
        public Command ClosePopUp { get; set; }

        public ObservableCollection<Users> Mentors { get; }
        public ObservableCollection<string> Filters { get; }

        private string _filters;
        public string ShowFilters
        {
            get => _filters;
            set
            {
                _filters = value;
                OnPropertyChanged();
            }
        }

        private string _filterMajor { get; set; }
        public string FilterMajor
        {
            get => _filterMajor;
            set
            {
                _filterMajor = value;
                OnPropertyChanged();
            }
        }

        private string _filterYears { get; set; }
        public string FilterYears
        {
            get => _filterYears;
            set
            {
                _filterYears = value;
                OnPropertyChanged();
            }
        }

        private Color _wantsAlumni {get; set;}
        public Color WantsAlumni
        {
            get => _wantsAlumni;
            set
            {
                _wantsAlumni = value;
                OnPropertyChanged();
            }
        }

        private Color _wantsStudent { get; set; }
        public Color WantsStudent
        {
            get => _wantsStudent;
            set
            {
                _wantsStudent = value;
                OnPropertyChanged();
            }
        }

        public Command SwitchAlumni { get; set; }

        public SearchNewMentorViewModel()
        {
            Title = "Find New Mentors";
            Mentors = new ObservableCollection<Users>();
            Filters = new ObservableCollection<string>();

            LoadMentorsCommand = new Command(async () => await ExecuteLoadMentors());
            FilterCommand = new Command(async () => await ExecuteFilterMentors());
            MentorTapped = new Command<Users>(OnMentorSelected);
            ClearFilters = new Command(async () => { Filters.Clear(); await ExecuteLoadMentors(); });
            ClosePopUp = new Command(async () => await ClosePopUpWindow());
            OpenAssistU = new Command(AssistUChat);
            SwitchAlumni = new Command(SwitchToAlumni);
        }

        async Task ExecuteLoadMentors()
        {
            try
            {
                Mentors.Clear();
                if (Filters.Count != 0)
                {
                    var temp = await DatabaseService.Instance.client.GetTable<Users>().Where(user => user.Role == "0").ToListAsync();
                    foreach (Users m in temp)
                    {
                        if (Filters.Contains(m.Major))
                        {
                            Mentors.Add(m);
                            Debug.WriteLine("------- Filtering ------");
                        }
                    }
                    ShowFilters = string.Join(", ", Filters);
                }
                else
                {
                    var connections = await DatabaseService.Instance.client.GetTable<Connection>().Where(u => u.MenteeID == App.loggedUser.id).ToListAsync();
                    var available = await DatabaseService.Instance.client.GetTable<Users>().Where(u => u.Role == "0" ).ToListAsync();
                    var excludedIDs = new HashSet<string>(connections.Select(u => u.MentorID));
                    var result = available.Where(p => !excludedIDs.Contains(p.id) && p.id != App.loggedUser.id);

                    foreach (Users element in result)
                    {
                        Mentors.Add(element);
                    }
                    ShowFilters = "";
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
            await PopupNavigation.Instance.PushAsync(new PopUp(this));
        }

        async Task ClosePopUpWindow()
        {
            if (!string.IsNullOrEmpty(FilterMajor))
            {
                Filters.Add(FilterMajor); //FIXME: Make filters a dictionary mapping values to query
                FilterYears = "";
                //TODO: change to only execute on changes once users have been updated to have this information
                IsBusy = true;
            }
            if (!string.IsNullOrEmpty(FilterYears))
            {
                Filters.Add(FilterYears);
                FilterYears = "";
            }
            await PopupNavigation.Instance.PopAllAsync();
        }

        void SwitchToAlumni()
        {
            if (WantsAlumni == Color.Red)
            {
                WantsStudent = Color.Red;
                WantsAlumni = Color.Default;
            }
            else
            {
                WantsStudent = Color.Default;
                WantsAlumni = Color.Red;
            }
        }



        async void OnMentorSelected(Users mentor)
        {
            if (mentor == null)
                return;
            await Shell.Current.Navigation.PushAsync(new ViewOnlyProfilePage(mentor, false));
        }


        async void AssistUChat()
        {
            await Shell.Current.Navigation.PushAsync(new ChatPage());
            //await Shell.Current.Navigation.PushAsync(new Services.Bot.AssisUWebPage());

        }


        public void OnAppearing()
        {
            IsBusy = true;
        }

    }
}
