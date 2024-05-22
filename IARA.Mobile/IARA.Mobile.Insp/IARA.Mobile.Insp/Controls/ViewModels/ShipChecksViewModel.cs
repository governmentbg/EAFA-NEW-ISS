using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.Extensions;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Dtos;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class ShipChecksViewModel : ViewModel
    {
        private string _preliminaryNoticeText;
        private string _opMembershipText;
        private List<ToggleOption> _preliminaryNoticeButtons;
        private List<ToggleOption> _opMembershipButtons;
        private List<SelectNomenclatureDto> _shipAssociations;

        public ShipChecksViewModel(InspectionPageViewModel inspection, ShipCatchesViewModel shipCatches = null)
        {
            Inspection = inspection;

            Permits = new PermitsViewModel(inspection);
            PermitLicenses = new PermitLicensesViewModel(inspection);
            LogBooks = new LogBooksViewModel(inspection, shipCatches);

            this.AddValidation(others: new IValidatableViewModel[]
            {
                Permits,
                PermitLicenses,
                LogBooks
            });

            ObservationsOrViolations.Category = InspectionObservationCategory.Check;
        }

        public InspectionPageViewModel Inspection { get; }

        public int PreliminaryNoticeCheckTypeId { get; private set; }
        public int OPMembershipCheckTypeId { get; private set; }

        public PermitsViewModel Permits { get; }
        public PermitLicensesViewModel PermitLicenses { get; }
        public LogBooksViewModel LogBooks { get; }

        public ValidStateValidatableTable<ToggleViewModel> Toggles { get; set; }

        public ValidStateMultiToggle OPMembership { get; set; }

        public ValidStateSelect<SelectNomenclatureDto> OPMembershipSelect { get; set; }

        public ValidStateMultiToggle PreliminaryNotice { get; set; }

        [MaxLength(50)]
        public ValidState PreliminaryNoticeNumber { get; set; }

        [MaxLength(400)]
        public ValidState PreliminaryNoticePurpose { get; set; }

        [MaxLength(4000)]
        public ValidStateObservation ObservationsOrViolations { get; set; }

        public string PreliminaryNoticeText
        {
            get => _preliminaryNoticeText;
            set => SetProperty(ref _preliminaryNoticeText, value);
        }
        public string OPMembershipText
        {
            get => _opMembershipText;
            set => SetProperty(ref _opMembershipText, value);
        }

        public List<ToggleOption> OPMembershipButtons
        {
            get => _opMembershipButtons;
            private set => SetProperty(ref _opMembershipButtons, value);
        }
        public List<ToggleOption> PreliminaryNoticeButtons
        {
            get => _preliminaryNoticeButtons;
            private set => SetProperty(ref _preliminaryNoticeButtons, value);
        }
        public List<SelectNomenclatureDto> ShipAssociations
        {
            get => _shipAssociations;
            private set => SetProperty(ref _shipAssociations, value);
        }

        public void Init(List<InspectionCheckTypeDto> checkTypes, List<SelectNomenclatureDto> shipAssociations)
        {
            ShipAssociations = shipAssociations;

            InspectionCheckTypeDto preliminaryNoticeCheck = checkTypes.Find(f => f.Code == Constants.PreliminaryNoticeType);
            InspectionCheckTypeDto opMembershipCheck = checkTypes.Find(f => f.Code == Constants.OPMembershipType);

            if (preliminaryNoticeCheck != null)
            {
                PreliminaryNoticeButtons = preliminaryNoticeCheck.Type == ToggleTypeEnum.Bool
                    ? InspectionTogglesHelper.YesNoMultiToggles
                    : InspectionTogglesHelper.YesNoNotApplicableMultiToggles;
                PreliminaryNoticeText = preliminaryNoticeCheck.Name;
                PreliminaryNoticeCheckTypeId = preliminaryNoticeCheck.Id;

                Toggles.Value.RemoveWhere(f => f.CheckTypeId == preliminaryNoticeCheck.Id);

                if (preliminaryNoticeCheck.IsMandatory)
                {
                    PreliminaryNotice.HasAsterisk = true;
                    PreliminaryNotice.Validations.Add(new TLValidator(new RequiredAttribute(), nameof(RequiredAttribute)));
                }
            }

            if (opMembershipCheck != null)
            {
                OPMembershipButtons = opMembershipCheck.Type == ToggleTypeEnum.Bool
                    ? InspectionTogglesHelper.YesNoMultiToggles
                    : InspectionTogglesHelper.YesNoNotApplicableMultiToggles;
                OPMembershipText = opMembershipCheck.Name;
                OPMembershipCheckTypeId = opMembershipCheck.Id;

                Toggles.Value.RemoveWhere(f => f.CheckTypeId == opMembershipCheck.Id);

                if (opMembershipCheck.IsMandatory)
                {
                    OPMembership.HasAsterisk = true;
                    OPMembership.Validations.Add(new TLValidator(new RequiredAttribute(), nameof(RequiredAttribute)));
                }
            }
        }

        public async Task OnEdit(IFishingShipInspection fishingShipInspection, List<InspectionCheckDto> checks)
        {
            Toggles.AssignFrom(fishingShipInspection.Checks);

            Permits.OnEdit(fishingShipInspection.Permits);
            PermitLicenses.OnEdit(fishingShipInspection.PermitLicenses);
            await LogBooks.OnEdit(fishingShipInspection.LogBooks);

            InspectionCheckDto preliminaryNotice = checks.Find(f => f.CheckTypeId == PreliminaryNoticeCheckTypeId);

            if (preliminaryNotice != null)
            {
                PreliminaryNotice.Value = preliminaryNotice.CheckValue.ToString();
                PreliminaryNoticeNumber.AssignFrom(preliminaryNotice.Number);
                PreliminaryNoticePurpose.AssignFrom(preliminaryNotice.Description);
            }

            InspectionCheckDto opMembership = checks.Find(f => f.CheckTypeId == OPMembershipCheckTypeId);

            if (opMembership != null)
            {
                OPMembership.Value = opMembership.CheckValue.ToString();
                OPMembershipSelect.AssignFrom(int.TryParse(opMembership.Number, out int result) ? (int?)result : null, ShipAssociations);
            }
        }

        public static implicit operator List<InspectionCheckDto>(ShipChecksViewModel viewModel)
        {
            if (viewModel == null || viewModel.PreliminaryNotice.Value == null)
            {
                return new List<InspectionCheckDto>();
            }

            List<InspectionCheckDto> list = new List<InspectionCheckDto>();

            if (viewModel.PreliminaryNotice.Value != null)
            {
                list.Add(new InspectionCheckDto
                {
                    CheckTypeId = viewModel.PreliminaryNoticeCheckTypeId,
                    CheckValue = Enum.TryParse(viewModel.PreliminaryNotice.Value, out CheckTypeEnum checkType)
                        ? checkType
                        : CheckTypeEnum.N,
                    Description = viewModel.PreliminaryNoticePurpose,
                    Number = viewModel.PreliminaryNoticeNumber,
                });
            }
            if (viewModel.OPMembership.Value != null)
            {
                list.Add(new InspectionCheckDto
                {
                    CheckTypeId = viewModel.OPMembershipCheckTypeId,
                    CheckValue = Enum.TryParse(viewModel.OPMembership.Value, out CheckTypeEnum checkType)
                        ? checkType
                        : CheckTypeEnum.N,
                    Number = viewModel.OPMembershipSelect.Value?.Id.ToString() ?? string.Empty,
                });
            }

            return list;
        }
    }
}
