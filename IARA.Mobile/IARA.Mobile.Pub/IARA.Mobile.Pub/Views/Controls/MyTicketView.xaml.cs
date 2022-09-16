
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyTicketView : ContentView
    {
        public static readonly BindableProperty ModelProperty = BindableProperty.Create(nameof(Model), typeof(object), typeof(MyTicketView));

        public MyTicketView()
        {
            InitializeComponent();
        }

        public object Model
        {
            set => SetValue(ModelProperty, value);
            get => GetValue(ModelProperty);
        }
    }
}