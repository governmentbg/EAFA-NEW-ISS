using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Shared.Views;
using System;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.HarbourInspection
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HarbourInspectionPage : BasePage<HarbourInspectionViewModel>
    {
        public HarbourInspectionPage(DateTime CreatedOn, SubmitType submitType = SubmitType.Draft, ViewActivityType activityType = ViewActivityType.Add, InspectionTransboardingDto dto = null, bool isLocal = false, bool createdByCurrentUser = true)
        {
            ViewModel.SubmitType = submitType;
            ViewModel.ActivityType = activityType;
            ViewModel.Edit = dto;
            ViewModel.IsLocal = isLocal;
            ViewModel.CreatedByCurrentUser = createdByCurrentUser;
            ViewModel.CreatedOn = CreatedOn;
            InitializeComponent();
            ViewModel.Sections = forwardSections;
        }
    }
}