
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamTest.ViewModels;

namespace XamTest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageCompressionPage : ContentPage
    {
        public ImageCompressionPage()
        {
            InitializeComponent();

            BindingContext = new ImageCompressionViewModel();
        }
    }
}