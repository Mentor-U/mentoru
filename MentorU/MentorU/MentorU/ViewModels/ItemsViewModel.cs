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
        public ObservableCollection<string> AllConditions { get; set; }
        private string _condition;
        private string _classUsed;


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

        private string _filterClassUsed;
        public string FilterClassUsed
        {
            get => _filterClassUsed;
            set
            {
                _filterClassUsed = value;
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

        public string condition
        {
            get => _condition;
            set
            {
                _condition = value;
                OnPropertyChanged();
            }
        }

        public string ClassUsed
        {
            get => _classUsed;
            set
            {
                _classUsed = value;
                OnPropertyChanged();
            }
        }

        public ItemsViewModel()
        {
            Title = "Marketplace";
            Items = new ObservableCollection<Items>();
            Filters = new ObservableCollection<string>();

            AllConditions = new ObservableCollection<string>()
            {
                "New",
                "Like New",
                "Good",
                "Decent"
            };

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
                if (Filters.Count != 0)
                {
                    var items = await DatabaseService.Instance.client.GetTable<Items>().ToListAsync();

                    //List<Items> its = new List<Items>();
                    // TODO: will need to implement multiple filters.
                    foreach (var item in items)
                    {
                        if(Filters.Contains(item.Condition) && Filters.Contains(item.ClassUsed))
                        {
                            Items.Add(item);
                        }
                        else if (Filters.Contains(item.Condition) && Filters.Count == 1)
                        {
                            Items.Add(item);
                        }
                        else if (Filters.Contains(item.ClassUsed) && Filters.Count == 1)
                        {
                            Items.Add(item);
                        }
                    }

                    //foreach (var i in its)
                    //{
                    //    if(Filters.Contains(i.Condition))
                    //    {
                    //        Items.Add(i);
                    //    }
                    //}

                    ShowFilters = String.Join(", ", Filters);
                }
                else
                {
                    var items = await DatabaseService.Instance.client.GetTable<Items>().ToListAsync();

                    foreach (var item in items)
                    {
                        item.itemImage = await BlobService.Instance.TryDownloadImage(item.id, "Image0");

                        Items.Add(item);
                    }
                    ShowFilters = "";
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
            if (!string.IsNullOrEmpty(condition))
            {
                Filters.Add(condition);
                condition = "";
            }
            if (!string.IsNullOrEmpty(FilterClassUsed))
            {
                Filters.Add(FilterClassUsed);
                FilterClassUsed = "";
            }
            IsBusy = true;
            await PopupNavigation.Instance.PopAllAsync();
        }
    }
}