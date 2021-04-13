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
using Plugin.FilePicker.Abstractions;
using Plugin.FilePicker;
using Xamarin.Essentials;
using System.Diagnostics;
using Plugin.Media;

namespace MentorU.ViewModels
{
    public class ApplicationViewModel : BaseViewModel
    {
        private Jobs _job;
        private string firstName;
        private string lastName;
        private string phoneNumber;
        private string email;
        private string _legal;
        private string _h1B;

        public ObservableCollection<string> AllLegals { get; set; }
        public ObservableCollection<string> AllH1B { get; set; }

        public Command AddResumeCommand { get; set; }
        private string resumeFilePath;
        private ImageSource _resumeFile;

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        public ApplicationViewModel(Jobs job)
        {
            _job = job;
            AddResumeCommand = new Command(AddResume);
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();

            AllLegals = new ObservableCollection<string>()
            {
                "Yes",
                "No"
            };

            AllH1B = new ObservableCollection<string>()
            {
                "Yes",
                "No"
            };
        }

        public string FirstName
        {
            get => firstName;
            set => SetProperty(ref firstName, value);
        }

        public string LastName
        {
            get => lastName;
            set => SetProperty(ref lastName, value);
        }

        public string PhoneNumber
        {
            get => phoneNumber;
            set => SetProperty(ref phoneNumber, value);
        }

        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public string Legal
        {
            get => _legal;
            set
            {
                _legal = value;
                OnPropertyChanged();
            }
        }

        public string H1B
        {
            get => _h1B;
            set
            {
                _h1B = value;
                OnPropertyChanged();
            }
        }

        public ImageSource ResumeFile
        {
            get => _resumeFile;
            set
            {
                _resumeFile = value;
                OnPropertyChanged();
            }
        }

        private async void AddResume()
        {
            //var result = await FilePicker.PickAsync(new PickOptions
            //{
            //    PickerTitle = "Select your resume",
            //    FileTypes = FilePickerFileType.Pdf
            //});

            //if (result != null)
            //{
            //    var stream = await result.OpenReadAsync();
            //    ResumeFile.DataArray = FilePicker.PickAsync(result);
            //}
            try
            {
                var pdfFileType =
                    new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        {DevicePlatform.iOS, new[] { "com.adobe.pdf" } },
                        {DevicePlatform.Android, new[] { "application/pdf" } },
                    });

                var pickResult = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = pdfFileType,
                    PickerTitle = "Select your Resume"
                });

                if (pickResult != null)
                {
                    string fileName = "temp-job-resume";
                    resumeFilePath = DependencyService.Get<IFileService>().SavePicture(fileName, pickResult.OpenReadAsync().Result);
                    ResumeFile = resumeFilePath;
                }
                else return;


                //ResumeFile = await CrossFilePicker.Current.PickFile();
                //if (ResumeFile == null) return;
                //string fileName = ResumeFile.FileName;
                //resumeFilePath = ResumeFile.FilePath;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Error choosing the file, please try again..");
            }

            
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(FirstName)
                && !String.IsNullOrWhiteSpace(LastName)
                && !String.IsNullOrWhiteSpace(PhoneNumber)
                && !String.IsNullOrWhiteSpace(Email);
        }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            Applications newApplication = new Applications()
            {
                ApplicantId = App.loggedUser.id,
                JobId = _job.id,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                PhoneNumber = PhoneNumber,
                Legal = Legal,
                H1B = H1B
            };

            await DatabaseService.Instance.client.GetTable<Applications>().InsertAsync(newApplication);

            string applicationId = newApplication.id;

            if(resumeFilePath != null)
            {
                BlobContainerClient containerClient = BlobService.Instance.BlobServiceClient.GetBlobContainerClient("resume");
                //BlobClient blobClient = containerClient.GetBlobClient(ResumeFile.FileName);
                //await blobClient.UploadAsync(resumeFilePath);
                await containerClient.CreateIfNotExistsAsync();

                //string fileName = "Resume0";

                await BlobService.Instance.TryUploadImage(containerClient, applicationId, resumeFilePath);

                File.Delete(resumeFilePath);
            }

            await Application.Current.MainPage.DisplayAlert("Success", "Application Submitted", "Ok");

            await Shell.Current.GoToAsync("..");
        }
    }
}
