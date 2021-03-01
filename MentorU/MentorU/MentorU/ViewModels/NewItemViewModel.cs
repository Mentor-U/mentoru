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

namespace MentorU.ViewModels
{
    public class NewItemViewModel : BaseViewModel
    {
        private string text;
        private string description;
        private Double itemPrice;

        public Command AddItemPictureCommand { get; set; }
        private string itemImageFilePath;
        private ImageSource _firstItemImage;

        public NewItemViewModel()
        {
            AddItemPictureCommand = new Command(AddPicture);
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
            AllDepartments = new List<string>(DatabaseService.Instance.ClassList.classList);
            Department = AllDepartments[0];

            AllConditions = new ObservableCollection<string>()
            {
                "New",
                "Like New",
                "Good",
                "Decent"
            };
        }

        public ImageSource ItemFirstImage
        {
            get => _firstItemImage;
            set
            {
                _firstItemImage = value;
                OnPropertyChanged();
            }
        }

        private async void AddPicture()
        {

            Stream itemImageStream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
            if (itemImageStream != null)
            {
                string fileName = "temp-market-image";
                itemImageFilePath = DependencyService.Get<IFileService>().SavePicture(fileName, itemImageStream);

                ItemFirstImage = itemImageFilePath;
            }

        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(text)
                && !String.IsNullOrWhiteSpace(description);
        }

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        public Double ItemPrice
        {
            get => itemPrice;
            set => SetProperty(ref itemPrice, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }


        public List<string> AllDepartments { get; set; }

        private string _department;
        public string Department
        {
            get => _department;
            set
            {
                _department = value;
                OnPropertyChanged();
            }
        }

        private string _classNumber;
        public string ClassNumber
        {
            get => _classNumber;
            set
            {
                _classNumber = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> AllConditions { get; set; }

        private string _conditions;
        public string condition
        {
            get => _conditions;
            set
            {
                _conditions = value;
                OnPropertyChanged();
            }
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            Items newItem = new Items()
            {
                Text = Text,
                Description = Description,
                Price = ItemPrice,
                Owner = App.loggedUser.id,
                ClassUsed = Department + " " + ClassNumber,
                Condition = condition
            };

            await DatabaseService.Instance.client.GetTable<Items>().InsertAsync(newItem);

            string containeritemid = newItem.id;


            BlobContainerClient containerClient = BlobService.Instance.BlobServiceClient.GetBlobContainerClient(containeritemid);
            await containerClient.CreateIfNotExistsAsync();

            string fileName = "Image0";

            //deletes the blob file if it exists and uploads an image
            await BlobService.Instance.TryUploadImage(containerClient, fileName, itemImageFilePath);


            File.Delete(itemImageFilePath);

            await Application.Current.MainPage.DisplayAlert("Success", "Item Added", "Ok");

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}
