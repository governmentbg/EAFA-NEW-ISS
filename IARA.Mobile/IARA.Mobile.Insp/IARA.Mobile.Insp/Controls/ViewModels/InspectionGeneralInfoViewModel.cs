using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Attributes;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class InspectionGeneralInfoViewModel : ViewModel
    {
        private string _reportNrStart;
        public InspectionGeneralInfoViewModel(InspectionPageViewModel inspection)
        {
            Inspection = inspection;
            Inspectors = new InspectorsViewModel(inspection, this);

            this.AddValidation(others: new[] { Inspectors });

            StartDate.Value = DateTime.Now;
            EndDate.Value = DateTime.Now.AddHours(1);

            CanEditReportNumber.Value = DependencyService.Resolve<IProfileTransaction>().HasPermission("InspectionEditNumber");
            CanEditReportNumber.AddFakeValidation();

        }
        public InspectionPageViewModel Inspection { get; }

        public InspectorsViewModel Inspectors { get; }

        [Required]
        [ReportNumber(nameof(CanEditReportNumber), ErrorMessageResourceName = "ReportNumber")]
        public ValidState ReportNr { get; set; }

        public string ReportNrStart
        {
            get => _reportNrStart;
            set => SetProperty(ref _reportNrStart, value);
        }

        [Required]
        [UpdateFrom(nameof(EndDate))]
        [LessThanOrEqualTo(nameof(EndDate))]
        public ValidStateDateTime StartDate { get; set; }

        [Required]
        [UpdateFrom(nameof(StartDate))]
        [BiggerThanOrEqualTo(nameof(StartDate))]
        public ValidStateDateTime EndDate { get; set; }

        public ValidStateBool ByEmergencySignal { get; set; }
        public ValidStateBool CanEditReportNumber { get; set; }

        public Task Init()
        {
            return Inspectors.Init();
        }

        public void ChangeReportNum(string territoryCode, string cardNum, string nextReportNum)
        {
            if (Inspection.ActivityType == ViewActivityType.Review)
            {
                return;
            }

            if (CanEditReportNumber.Value)
            {
                ReportNrStart = "";
                ReportNr.Value = $"{HandleNumber(territoryCode)}-{HandleNumber(cardNum)}-" + nextReportNum ?? "001";
            }
            else
            {
                ReportNrStart = $"{HandleNumber(territoryCode)}-{HandleNumber(cardNum)}-";
                ReportNr.Value = nextReportNum ?? "001";
            }
        }

        public Task OnEdit(InspectionEditDto dto)
        {
            if (!string.IsNullOrEmpty(dto.ReportNum))
            {

                if (CanEditReportNumber.Value)
                {
                    ReportNrStart = "";
                    ReportNr.Value = dto.ReportNum;
                }
                else
                {
                    string[] numSplit = dto.ReportNum.Split('-');

                    ReportNrStart = string.Join("-", numSplit.Take(2)) + "-";

                    if (numSplit.Length == 3)
                    {
                        string num = numSplit[2];

                        ReportNr.Value = num.Length > 3
                            ? num.Substring(0, 3)
                            : num.PadLeft(3, '0');
                    }
                }
            }

            EndDate.Value = dto.EndDate ?? DateTime.Now;
            StartDate.Value = dto.StartDate ?? DateTime.Now;
            ByEmergencySignal.Value = dto.ByEmergencySignal ?? false;

            return Inspectors.OnEdit(dto.Inspectors, false);
        }

        public string BuildReportNum()
        {
            return ReportNrStart + ReportNr.Value;
        }

        private string HandleNumber(string num)
        {
            return num.Length > 3
                ? num.Substring(0, 3)
                : num.PadRight(3, '0');
        }
    }
}
