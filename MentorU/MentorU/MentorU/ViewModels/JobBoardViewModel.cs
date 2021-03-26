using MentorU.Models;
using MentorU.Services.Blob;
using MentorU.Services.DatabaseServices;
using MentorU.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace MentorU.ViewModels
{
    public class JobBoardViewModel : BaseViewModel
    {
        private Jobs _selectedJob;
        public ObservableCollection<string> AllLevel { get; set; }
        private string _level;
        private string _jobType;

        public ObservableCollection<Jobs> Jobs { get; }
        public Command LoadJobsCommand { get; }
        public Command AddJobCommand { get; }
        public Command<Jobs> JobTapped { get; }
        public Command FilterCommand { get; }
        public Command ClearFilters { get; }
        public Command ClosePopUp { get; set; }

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
    }
}
