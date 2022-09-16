using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using System;
using System.ComponentModel.DataAnnotations;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class InspectionGeneralInfoViewModel : ViewModel
    {
        public InspectionGeneralInfoViewModel(InspectionPageViewModel inspection)
        {
            Inspection = inspection;
            Inspectors = new InspectorsViewModel(inspection);

            this.AddValidation(others: new[] { Inspectors });

            StartDate.Value = DateTime.Now;
            EndDate.Value = DateTime.Now.AddHours(1);
        }

        public InspectionPageViewModel Inspection { get; }

        public InspectorsViewModel Inspectors { get; }

        public ValidState ReportNr { get; set; }

        [Required]
        [UpdateFrom(nameof(EndDate))]
        [LessThanOrEqualTo(nameof(EndDate))]
        public ValidStateDateTime StartDate { get; set; }

        [Required]
        [UpdateFrom(nameof(StartDate))]
        [BiggerThanOrEqualTo(nameof(StartDate))]
        public ValidStateDateTime EndDate { get; set; }

        public ValidStateBool ByEmergencySignal { get; set; }

        public void Init()
        {
            ReportNr.Value = TranslateExtension.Translator[nameof(GroupResourceEnum.GeneralInfo) + "/GeneratedOnSave"];
            Inspectors.Init();
        }

        public void OnEdit(InspectionEditDto dto)
        {
            ReportNr.Value = string.IsNullOrEmpty(dto.ReportNum)
                ? TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/NoReportNumber"]
                : dto.ReportNum;
            EndDate.Value = dto.EndDate ?? DateTime.Now;
            StartDate.Value = dto.StartDate ?? DateTime.Now;
            ByEmergencySignal.Value = dto.ByEmergencySignal ?? false;

            Inspectors.OnEdit(dto.Inspectors);
        }
    }
}
