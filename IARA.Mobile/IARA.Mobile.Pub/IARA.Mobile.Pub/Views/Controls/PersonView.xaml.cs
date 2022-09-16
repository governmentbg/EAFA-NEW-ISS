using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersonView : ContentView
    {
        public static readonly BindableProperty IsEditEnabledProperty =
           BindableProperty.Create(nameof(IsEditEnabled), typeof(bool), typeof(PersonView), defaultValue: true);
        public PersonView()
        {
            InitializeComponent();
        }

        public bool IsEditEnabled
        {
            get => (bool)GetValue(IsEditEnabledProperty);
            set => SetValue(IsEditEnabledProperty, value);
        }
    }
}