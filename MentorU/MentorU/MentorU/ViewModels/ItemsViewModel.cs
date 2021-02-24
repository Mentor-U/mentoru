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
    public class ItemsViewModel : BaseViewModel
    {
        private Items _selectedItem;

        public ObservableCollection<Items> Items { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command<Items> ItemTapped { get; }
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

        private string _filterYear;
        public string FilterYear
        {
            get => _filterYear;
            set
            {
                _filterYear = value;
                OnPropertyChanged();
            }
        }

        private string _filterCondition;
        public string FilterCondition
        {
            get => _filterCondition;
            set
            {
                _filterCondition = value;
                OnPropertyChanged();
            }
        }

        public ItemsViewModel()
        {
            Title = "Marketplace";
            Items = new ObservableCollection<Items>();
            Filters = new ObservableCollection<string>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<Items>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem);

            FilterCommand = new Command(async () => await ExecuteFilterItems());
            ClearFilters = new Command(async () => { Filters.Clear(); await ExecuteLoadItemsCommand(); });
            ClosePopUp = new Command(async () => await ClosePopUpWindow());
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DatabaseService.Instance.client.GetTable<Items>().ToListAsync();
 
                foreach (var item in items)
                {
                    item.itemImage = await BlobService.Instance.TryDownloadImage(item.id, "Image0");

                    Items.Add(item);

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        public Items SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        async void OnItemSelected(Items item)
        {
            if (item == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            //await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.id}");
            await Application.Current.MainPage.Navigation.PushAsync(new ItemDetailPage(item));
        }

        async Task ExecuteFilterItems()
        {
            await PopupNavigation.Instance.PushAsync(new PopUpItems(this));
        }

        async Task ClosePopUpWindow()
        {
            if (!string.IsNullOrEmpty(FilterCondition))
            {
                Filters.Add(FilterCondition); //FIXME: Make filters a dictionary mapping values to query
                FilterYear = "";
                //TODO: change to only execute on changes once users have been updated to have this information
                IsBusy = true;
            }
            if (!string.IsNullOrEmpty(FilterYear))
            {
                Filters.Add(FilterYear);
                FilterYear = "";
            }
            await PopupNavigation.Instance.PopAllAsync();
        }
    }
}