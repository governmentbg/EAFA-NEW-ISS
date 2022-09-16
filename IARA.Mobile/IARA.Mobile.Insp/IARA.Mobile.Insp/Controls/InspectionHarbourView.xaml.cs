using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InspectionHarbourView : StackLayout
    {
        public static readonly BindableProperty InRegisterLabelProperty =
            BindableProperty.Create(nameof(InRegisterLabel), typeof(string), typeof(InspectionHarbourView));

        public InspectionHarbourView()
        {
            InitializeComponent();
        }

        public string InRegisterLabel
        {
            get => (string)GetValue(InRegisterLabelProperty);
            set => SetValue(InRegisterLabelProperty, value);
        }
    }
}