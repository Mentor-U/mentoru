using MentorU.ViewModels;

namespace MentorU.Views
{
    public partial class PopUpGroupChat
    {
        public PopUpGroupChat(GroupMainChatViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
            Animation = new Rg.Plugins.Popup.Animations.ScaleAnimation();
        }
    }
}