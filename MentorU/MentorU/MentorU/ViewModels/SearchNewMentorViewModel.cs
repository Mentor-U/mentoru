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
using MentorU.Services.Blob;

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

        private Tuple<string,string> _filterTuple;
        private HashSet<string> ExcludeIDs;

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
            Title = "Find Connections";
            Mentors = new ObservableCollection<Users>();
            Filters = new ObservableCollection<string>();

            LoadMentorsCommand = new Command(async () => await ExecuteLoadMentors());
            FilterCommand = new Command(async () => await ExecuteFilterMentors());
            MentorTapped = new Command<Users>(OnMentorSelected);
            ClearFilters = new Command(async () => { _filterTuple = null; Filters.Clear(); await ExecuteLoadMentors(); });
            ClosePopUp = new Command(async () => await ClosePopUpWindow());
            OpenAssistU = new Command(AssistUChat);
            SwitchAlumni = new Command(SwitchToAlumni);
        }


        /// <summary>
        /// Load all the users that are not currently connected with the active user.
        /// </summary>
        /// <returns></returns>
        async Task ExecuteLoadMentors()
        {
            try
            {
                Mentors.Clear();
                if (_filterTuple != null) // Condition for filters on the available users
                {
                    string _role = null;
                    List<Users> temp = null;
                    if (_filterTuple.Item2 != null)
                    {
                        _role = _filterTuple.Item2 == "Alumni" ? "0" : "1";
                        temp = await DatabaseService.Instance.client.GetTable<Users>().Where(user => user.Role == _role).ToListAsync();
                        Filters.Add(_filterTuple.Item2);
                    }
                    else
                        temp = await DatabaseService.Instance.client.GetTable<Users>().ToListAsync();
                    foreach (Users m in temp)
                    {
                        if (ExcludeIDs.Contains(m.id))
                            continue;
                        if (_filterTuple.Item1 != null) // Handles tuple AND in the boolean condition to further restrict
                        {
                            if (_filterTuple.Item1 == m.Major)
                            {
                                m.ProfileImage = await BlobService.Instance.TryDownloadImage("profile-images", m.id);
                                Mentors.Add(m);
                            }
                        }
                        else
                        {
                            m.ProfileImage = await BlobService.Instance.TryDownloadImage("profile-images", m.id);
                            Mentors.Add(m);
                        }
                    }
                    if(_filterTuple.Item1 != null)
                        Filters.Add(_filterTuple.Item1);
                    ShowFilters = string.Join(",", Filters);
                }
                else
                {
                    var connections = await DatabaseService.Instance.client.GetTable<Connection>()
                        .Where(u => u.MenteeID == App.loggedUser.id || u.MentorID == App.loggedUser.id).ToListAsync();
                    var available = await DatabaseService.Instance.client.GetTable<Users>().ToListAsync();
                    var excludedMentorIDs = new HashSet<string>(connections.Select(u => u.MentorID));
                    ExcludeIDs = new HashSet<string>(connections.Select(u => u.MenteeID));
                    ExcludeIDs.UnionWith(excludedMentorIDs);
                    var result = available.Where(p => !ExcludeIDs.Contains(p.id));

                    foreach (Users m in result)
                    {
                        m.ProfileImage = await BlobService.Instance.TryDownloadImage("profile-images", m.id);
                        Mentors.Add(m);
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


        /// <summary>
        /// Open the pop up dialog to allow for filters to be selected
        /// </summary>
        /// <returns></returns>
        async Task ExecuteFilterMentors()
        {
            await PopupNavigation.Instance.PushAsync(new PopUp(this));
        }

        /// <summary>
        /// Close pop up and load the users based on the filters
        /// </summary>
        /// <returns></returns>
        async Task ClosePopUpWindow()
        {
            string major = null;
            string role = null;
            if (!string.IsNullOrEmpty(FilterMajor))
            {
                major = FilterMajor;
                FilterYears = "";
            }
            if (WantsAlumni == Color.Red || WantsStudent == Color.Red)
                role = WantsAlumni == Color.Red ? "Alumni" : "Student";
            _filterTuple = new Tuple<string,string>(major, role);
            IsBusy = true; // triggers LoadPageData()
            await PopupNavigation.Instance.PopAllAsync();
        }

        /// <summary>
        /// Visual toggle for the pop up
        /// </summary>
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


        /// <summary>
        /// Navigate to the selected users profile
        /// </summary>
        /// <param name="mentor"></param>
        async void OnMentorSelected(Users mentor)
        {
            if (mentor == null)
                return;
            await Shell.Current.Navigation.PushAsync(new ViewOnlyProfilePage(mentor, false));
        }

        /// <summary>
        /// Open a chat with the chat bot to allow for more advance
        /// search queries using NLP
        /// </summary>
        async void AssistUChat()
        {
            await Shell.Current.Navigation.PushAsync(new ChatPage());
        }


        public void OnAppearing()
        {
            IsBusy = true;
        }

    }
}
