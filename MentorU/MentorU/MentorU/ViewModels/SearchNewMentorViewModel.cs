﻿using MentorU.Models;
using MentorU.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using MentorU.Views.ChatViews;

namespace MentorU.ViewModels
{
    class SearchNewMentorViewModel : BaseViewModel
    {
        public Command LoadMentorsCommand { get; }
        public Command FilterCommand { get; }
        public Command ClearFilters { get; }
        public Command<Users> MentorTapped { get; }
        public Command OpenAssistU { get; }

        public ObservableCollection<Users> Mentors { get; }
        public ObservableCollection<string> Filters { get; }

        private string _filters;
        public string ShowFilters { get => _filters; set { _filters = value; OnPropertyChanged(); } }

        public SearchNewMentorViewModel()
        {
            Title = "Find New Mentors";
            Mentors = new ObservableCollection<Users>();
            Filters = new ObservableCollection<string>();

            LoadMentorsCommand = new Command(async () => await ExecuteLoadMentors());
            FilterCommand = new Command(async () => await ExecuteFilterMentors());
            MentorTapped = new Command<Users>(OnMentorSelected);
            ClearFilters = new Command(async () => { Filters.Clear(); await ExecuteLoadMentors(); });
            OpenAssistU = new Command(AssistUChat);
        }

        async Task ExecuteLoadMentors()
        {
            IsBusy = true;
            try
            {
                Mentors.Clear();
                if (Filters.Count != 0)
                {
                    var temp = await App.client.GetTable<Users>().Where(user => user.Role == "0").ToListAsync();
                    foreach (Users m in temp)
                    {
                        if (Filters.Contains(m.Major))
                            Mentors.Add(m);
                    }
                    ShowFilters = string.Join(", ", Filters);
                }
                else
                {
                    var temp = await App.client.GetTable<Users>().Where(user => user.Role == "0").ToListAsync();
                    foreach (Users element in temp)
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
            var filters = await Application.Current.MainPage.DisplayPromptAsync("Filter", "Enter a Filter");
            if (filters != null)
            {
                Filters.Add(filters.ToString());
                await ExecuteLoadMentors();
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
            await Shell.Current.Navigation.PushAsync(new ChatPage(App.assistU));
            App.assistU.StartChat();
        }


        public void OnAppearing()
        {
            IsBusy = true;
        }

    }
}
