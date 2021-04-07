using Azure.Storage.Blobs;
using MentorU.Models;
using MentorU.Services;
using MentorU.Services.Blob;
using MentorU.Services.DatabaseServices;
using System;
using System.IO;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace MentorU.ViewModels
{
    public class NewJobViewModel : BaseViewModel
    {
        private string companyName;
        private string text;
        private string description;
        private string responsibilities;
        private string qualifications;
        private string location;

        public Command AddLogoPictureCommand { get; set; }
        private string jobImageFilePath;
        private ImageSource _companyLogo;

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        public NewJobViewModel()
        {
            AddLogoPictureCommand = new Command(AddLogo);
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();

            AllLevels = new ObservableCollection<string>()
            {
                "Associate",
                "Entry Level",
                "Mid-Senior level",
                "Executive",
                "Director",
                "Internship"
            };

            AllJobTypes = new ObservableCollection<string>()
            {
                "Full-time",
                "Part-time",
                "Internship",
                "Contract",
                "Other"
            };
        }

        public ImageSource CompanyLogo
        {
            get => _companyLogo;
            set
            {
                _companyLogo = value;
                OnPropertyChanged();
            }
        }

        private async void AddLogo()
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await AppShell.Current.DisplayAlert("Not supported", "Your device does not currently support this functionality", "Ok");
                return;
            }

            var mediaOption = new PickMediaOptions()
            {
                PhotoSize = PhotoSize.Medium
            };

            string fileName = "temp-job-logo";

            var selectedImageFile = await CrossMedia.Current.PickPhotoAsync(mediaOption);

            jobImageFilePath = DependencyService.Get<IFileService>().SavePicture(fileName, selectedImageFile.GetStream());

            CompanyLogo = jobImageFilePath;
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(text)
                && !String.IsNullOrWhiteSpace(description);
        }

        public string CompanyName
        {
            get => companyName;
            set => SetProperty(ref companyName, value);
        }

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public string Responsibilities
        {
            get => responsibilities;
            set => SetProperty(ref responsibilities, value);
        }

        public string Qualifications
        {
            get => qualifications;
            set => SetProperty(ref qualifications, value);
        }

        public string Location
        {
            get => location;
            set => SetProperty(ref location, value);
        }

        public ObservableCollection<string> AllLevels { get; set; }
        public ObservableCollection<string> AllJobTypes { get; set; }

        private string _level;
        public string Level
        {
            get => _level;
            set
            {
                _level = value;
                OnPropertyChanged();
            }
        }

        private string _jobType;
        public string JobType
        {
            get => _jobType;
            set
            {
                _jobType = value;
                OnPropertyChanged();
            }
        }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            Jobs newJob = new Jobs()
            {
                CompanyName = CompanyName,
                Text = Text,
                Description = Description,
                Location = Location,
                Owner = App.loggedUser.id,
                JobType = JobType,
                Level = Level
            };

            await DatabaseService.Instance.client.GetTable<Jobs>().InsertAsync(newJob);

            string jobId = newJob.id;

            if (jobImageFilePath != null)
            {
                BlobContainerClient containerClient = BlobService.Instance.BlobServiceClient.GetBlobContainerClient(jobId);
                await containerClient.CreateIfNotExistsAsync();

                string fileName = "Image0";

                //deletes the blob file if it exists and uploads an image
                await BlobService.Instance.TryUploadImage(containerClient, fileName, jobImageFilePath);

                File.Delete(jobImageFilePath);
            }

            await Application.Current.MainPage.DisplayAlert("Success", "Job Added", "Ok");

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}
