using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MentorU.Models;
using MentorU.Services.Blob;
using MentorU.Services.DatabaseServices;
using MentorU.Views.ChatViews;
using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace MentorU.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class ItemDetailViewModel : BaseViewModel
    {
        private string itemId;
        private string text;
        private string description;
        private double itemPrice;
        private ImageSource itemImageSource;
        private Items _item;

        public ItemDetailViewModel(Items item)
        {
            _item = item;
            _ = loadItemAsync(item);
        }

        public string Id { get; set; }

        public string Text
        {
            get => _item.Text;
            set => SetProperty(ref text, value);
        }

        public ImageSource ItemImageSource
        {
            get => itemImageSource;
            set
            {
                itemImageSource = value;
                OnPropertyChanged();
            }
        }

        public double ItemPrice
        {
            get => _item.Price;
            set => SetProperty(ref itemPrice, value);
        }

        public string Description
        {
            get => _item.Description;
            set => SetProperty(ref description, value);
        }

        public string ClassUsed { get; set; }
        public string Condition { get; set; }

        public string ItemId
        {
            get
            {
                return _item.id;
            }
            set
            {
                itemId = value;
                //LoadItemId(value);
            }
        }

        //public async void LoadItemId(string itemId)
        //{
        //    try
        //    {
        //        var item = await DatabaseService.Instance.client.GetTable<Items>().Where(u => u.id == _item.id).ToListAsync();
        //        Id = item[0].id;
        //        Text = item[0].Text;
        //        Description = item[0].Description;
        //        ItemPrice = item[0].Price;

        //        ItemImageSource = await BlobService.Instance.TryDownloadImage(Id, "Image0");

        //    }
        //    catch (Exception)
        //    {
        //        Debug.WriteLine("Failed to Load Item");
        //    }
        //}

        async System.Threading.Tasks.Task loadItemAsync(Items item)
        {
            Id = item.id;
            Text = item.Text;
            Description = item.Description;
            ItemPrice = item.Price;
            ClassUsed = item.ClassUsed;
            Condition = item.Condition;

            ItemImageSource = await BlobService.Instance.TryDownloadImage(Id, "Image0");

        }

        ///<summary>
        /// Opens the chat window with the associated user.
        ///</summary>
        public async void StartChat(object obj)
        {
            await Shell.Current.Navigation.PopToRootAsync(false); // false -> disables navigation animation
            var user = await DatabaseService.Instance.client.GetTable<Users>().Where(u => u.id == _item.Owner).ToListAsync();
            await Shell.Current.Navigation.PushAsync(new ChatPage(user[0]));
        }


        ///<summary>
        /// Delete the item from DB
        ///</summary>
        public async void DeleteItem(object obj)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Confirm Delete", "Are you sure you want to remove this listing from the marketplace?", "Accept", "Cancel");
            if(confirm)
            {
                await Shell.Current.Navigation.PopToRootAsync(false); // false -> disables navigation animation
                await DatabaseService.Instance.client.GetTable<Items>().DeleteAsync(_item);
            }
           
        }
    }
}
