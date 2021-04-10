using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MentorU.Models;
using MentorU.Services.Blob;
using MentorU.Services.DatabaseServices;
using MentorU.Views;
using MentorU.Views.ChatViews;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    [QueryProperty(nameof(JobId), nameof(JobId))]
    public class JobDetailViewModel : BaseViewModel
    {
        private string jobId;
        private string text;
        private string companyName;
        private string description;
        private string location;
        private string date;
        private ImageSource companyLogoSource;
        private Jobs _job;

        public string JobType { get; set; }
        public string Level { get; set; }

        public JobDetailViewModel(Jobs job)
        {
            _job = job;
            _ = loadJobAsync(job);
        }

        public string Id { get; set; }

        public string Text
        {
            get => _job.Text;
            set => SetProperty(ref text, value);
        }

        public string CompanyName
        {
            get => _job.CompanyName;
            set => SetProperty(ref companyName, value);
        }

        public string Description
        {
            get => _job.Description;
            set => SetProperty(ref description, value);
        }

        public string Location
        {
            get => _job.Location;
            set => SetProperty(ref location, value);
        }

        public string Date
        {
            get => _job.Date;
            set => SetProperty(ref date, value);
        }

        public ImageSource CompanyLogoSource
        {
            get => companyLogoSource;
            set
            {
                companyLogoSource = value;
                OnPropertyChanged();
            }
        }

        public string JobId
        {
            get => _job.id;
            set => SetProperty(ref jobId, value);
        }

        async System.Threading.Tasks.Task loadJobAsync(Jobs job)
        {
            Id = job.id;
            Text = job.Text;
            Description = job.Description;
            Location = job.Location;
            JobType = job.JobType;
            Level = job.Level;

            CompanyLogoSource = await BlobService.Instance.TryDownloadImage(Id, "Image0");
        }
        public async void StartApply(object obj)
        {
            await Shell.Current.Navigation.PopToRootAsync(false); // false -> disables navigation animation
            var job = await DatabaseService.Instance.client.GetTable<Jobs>().Where(u => u.id == _job.id).ToListAsync();
            await Shell.Current.Navigation.PushAsync(new ApplicationPage(job[0]));
        }

        public async void DeleteJob(object obj)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Confirm Delete", "Are you sure you want to remove this job from Job Board?", "Accept", "Cancel");
            if (confirm)
            {
                try
                {
                    _job.jobImage = null;
                    await DatabaseService.Instance.client.GetTable<Jobs>().DeleteAsync(_job);
                    await BlobService.Instance.BlobServiceClient.DeleteBlobContainerAsync(_job.id);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    // Deletion error... Must have already been deleted or somehow never existed
                }
                await Shell.Current.Navigation.PopToRootAsync(false); // false -> disables navigation animation

            }
            else return;
        }
    }
}
