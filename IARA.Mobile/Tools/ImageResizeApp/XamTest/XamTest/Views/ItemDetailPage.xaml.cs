using System.ComponentModel;
using Xamarin.Forms;
using XamTest.ViewModels;

namespace XamTest.Views
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