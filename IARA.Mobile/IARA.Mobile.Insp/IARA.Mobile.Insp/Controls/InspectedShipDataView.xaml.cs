using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InspectedShipDataView : StackLayout
    {
        public static readonly BindableProperty ShipInRegisterLabelProperty =
            BindableProperty.Create(nameof(ShipInRegisterLabel), typeof(string), typeof(InspectedShipDataView));

        public InspectedShipDataView()
        {
            InitializeComponent();
        }

        public string ShipInRegisterLabel
        {
            get => (string)GetValue(ShipInRegisterLabelProperty);
            set => SetValue(ShipInRegisterLabelProperty, value);
        }
    }
}