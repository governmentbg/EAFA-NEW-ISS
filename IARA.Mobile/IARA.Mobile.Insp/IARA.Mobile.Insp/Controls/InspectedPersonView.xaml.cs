using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InspectedPersonView : StackLayout
    {
        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(InspectedPersonView));

        public static readonly BindableProperty InRegisterTextProperty =
            BindableProperty.Create(nameof(InRegisterText), typeof(string), typeof(InspectedPersonView));

        public InspectedPersonView()
        {
            InitializeComponent();
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string InRegisterText
        {
            get => (string)GetValue(InRegisterTextProperty);
            set => SetValue(InRegisterTextProperty, value);
        }
    }
}