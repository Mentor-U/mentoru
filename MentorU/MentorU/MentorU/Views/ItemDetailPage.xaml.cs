using MentorU.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

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