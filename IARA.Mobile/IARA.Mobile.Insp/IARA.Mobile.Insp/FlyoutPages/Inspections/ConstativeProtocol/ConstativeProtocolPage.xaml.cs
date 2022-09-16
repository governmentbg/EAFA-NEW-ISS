using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.ConstativeProtocol
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConstativeProtocolPage : BasePage<ConstativeProtocolViewModel>
    {
        public ConstativeProtocolPage(InspectionConstativeProtocolDto dto)
        {
            ViewModel.Edit = dto;
            ViewModel.BeforeInit();
            InitializeComponent();
        }
    }
}