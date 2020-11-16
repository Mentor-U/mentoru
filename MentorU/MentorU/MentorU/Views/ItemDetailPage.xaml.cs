using System.ComponentModel;
using Xamarin.Forms;
using MentorU.ViewModels;

namespace MentorU.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}