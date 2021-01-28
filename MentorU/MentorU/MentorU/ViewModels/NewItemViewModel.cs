using MentorU.Models;
using MentorU.Services.DatabaseServices;
using System;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    public class NewItemViewModel : BaseViewModel
    {
        private string text;
        private string description;
        private Double itemPrice;

        public NewItemViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
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
                Owner = App.loggedUser.FirstName + " " + App.loggedUser.LastName
            };

            await DatabaseService.client.GetTable<Items>().InsertAsync(newItem);

            await Application.Current.MainPage.DisplayAlert("Success", "Item Added", "Ok");

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}
