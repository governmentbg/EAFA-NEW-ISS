using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.FishermanInspection
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FishermanInspectionPage : BasePage<FishermanInspectionViewModel>
    {
        public FishermanInspectionPage(ViewActivityType activityType = ViewActivityType.Add, InspectionFisherDto dto = null, bool isLocal = false)
        {
            ViewModel.ActivityType = activityType;
            ViewModel.Edit = dto;
            ViewModel.IsLocal = isLocal;
            InitializeComponent();
        }
    }
}