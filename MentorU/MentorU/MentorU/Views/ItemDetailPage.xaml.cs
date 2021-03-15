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

            ToolbarItem interactButton;

            if (i.Owner.Equals(App.loggedUser.id))
            {
                interactButton = new ToolbarItem
                {
                    Text = "Delete",
                    Command = new Command(_vm.DeleteItem)
                };
            }
            else
            {
                interactButton = new ToolbarItem
                {
                    Text = "Chat",
                    Command = new Command(_vm.StartChat)
                };
            }

            ToolbarItems.Add(interactButton);
        }
    }
}