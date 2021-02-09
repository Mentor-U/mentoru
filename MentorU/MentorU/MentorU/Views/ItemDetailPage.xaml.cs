using MentorU.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MentorU.Models;

namespace MentorU.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemDetailPage : ContentPage
    {
        ItemDetailViewModel _vm;
        public ItemDetailPage(Items i)
        {
            InitializeComponent();
            BindingContext = _vm = new ItemDetailViewModel(i);

            ToolbarItem chatButton = new ToolbarItem
            {
                Text = "Chat",
                Command = new Command(_vm.StartChat)
            };
            ToolbarItems.Add(chatButton);
        }
    }
}