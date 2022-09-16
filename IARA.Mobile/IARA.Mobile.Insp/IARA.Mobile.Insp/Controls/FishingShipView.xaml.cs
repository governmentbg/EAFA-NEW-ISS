using IARA.Mobile.Shared.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FishingShipView : SectionView
    {
        public static readonly BindableProperty ShipInRegisterLabelProperty =
            BindableProperty.Create(nameof(ShipInRegisterLabel), typeof(string), typeof(FishingShipView));

        public FishingShipView()
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