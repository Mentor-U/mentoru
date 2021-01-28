using MentorU.Models;
using MentorU.Services.DatabaseServices;
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
        public string Id { get; set; }

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        public double ItemPrice
        {
            get => itemPrice;
            set => SetProperty(ref itemPrice, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
                LoadItemId(value);
            }
        }

        public async void LoadItemId(string itemId)
        {
            try
            {
                var item = await DatabaseService.client.GetTable<Items>().Where(u => u.id == itemId).ToListAsync();
                Id = item[0].id;
                Text = item[0].Text;
                Description = item[0].Description;
                ItemPrice = item[0].Price;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
