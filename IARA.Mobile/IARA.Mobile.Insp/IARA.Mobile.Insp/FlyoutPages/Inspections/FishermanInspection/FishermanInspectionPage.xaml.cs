using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Shared.Views;
using System;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.FishermanInspection
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FishermanInspectionPage : BasePage<FishermanInspectionViewModel>
    {
        public FishermanInspectionPage(DateTime CreatedOn, SubmitType submitType = SubmitType.Draft, ViewActivityType activityType = ViewActivityType.Add, InspectionFisherDto dto = null, bool isLocal = false, bool createdByCurrentUser = true)
        {
            ViewModel.SubmitType = submitType;
            ViewModel.ActivityType = activityType;
            ViewModel.Edit = dto;
            ViewModel.IsLocal = isLocal;
            ViewModel.CreatedByCurrentUser = createdByCurrentUser;
            ViewModel.CreatedOn = CreatedOn;
            ViewModel.InspectionType = InspectionType.IFP;
            InitializeComponent();
            ViewModel.Sections = forwardSections;
        }
    }
}