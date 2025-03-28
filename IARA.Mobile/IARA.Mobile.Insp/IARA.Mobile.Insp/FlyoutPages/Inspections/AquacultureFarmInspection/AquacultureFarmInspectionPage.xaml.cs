﻿using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Shared.Views;
using System;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.AquacultureFarmInspection
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AquacultureFarmInspectionPage : BasePage<AquacultureFarmInspectionViewModel>
    {
        public AquacultureFarmInspectionPage(DateTime CreatedOn, SubmitType sumbitType = SubmitType.Draft, ViewActivityType activityType = ViewActivityType.Add, InspectionAquacultureDto dto = null, bool isLocal = false, bool createdByCurrentUser = true)
        {
            ViewModel.SubmitType = sumbitType;
            ViewModel.ActivityType = activityType;
            ViewModel.Edit = dto;
            ViewModel.IsLocal = isLocal;
            ViewModel.CreatedByCurrentUser = createdByCurrentUser;
            ViewModel.CreatedOn = CreatedOn;
            ViewModel.InspectionType = InspectionType.IAQ;
            InitializeComponent();
            ViewModel.Sections = forwardSections;
        }
    }
}