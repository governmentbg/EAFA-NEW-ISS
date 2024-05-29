using IARA.Mobile.Application;
using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using IARA.Mobile.Insp.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.DeclarationCatchDialog
{
    public class DeclarationCatchDialogViewModel : TLBaseDialogViewModel<DeclarationCatchModel>
    {
        private List<SelectNomenclatureDto> _fishTypes;
        private List<SelectNomenclatureDto> _catchTypes;
        private List<SelectNomenclatureDto> _presentations;
        private List<CatchZoneNomenclatureDto> _catchZones;
        private List<SelectNomenclatureDto> _declarationTypes;
        private List<DeclarationLogBookPageDto> _logBookPages;
        private bool _isDeclarationRegistered;
        private bool _onlyUnregisteredDeclaration;
        private DeclarationLogBookType _subjectType;

        public DeclarationCatchDialogViewModel()
        {
            Save = CommandBuilder.CreateFrom(OnSave);
            DeclarationSelected = CommandBuilder.CreateFrom<SelectNomenclatureDto>(OnDeclarationSelected);
            AquacultureChosen = CommandBuilder.CreateFrom(OnAquacultureChosen);
            SubjectType = DeclarationLogBookType.NNN;
            _onlyUnregisteredDeclaration = true;
        }

        public DeclarationCatchModel Edit { get; set; }
        public int? ShipUid { get; set; }
        public bool HasCatchType { get; set; }
        public bool HasUndersizedCheck { get; set; }
        public bool HasUnloadedQuantity { get; set; }

        public DeclarationCatchesViewModel Catches { get; set; }
        public InspectionPageViewModel Inspection { get; set; }
        public ViewActivityType DialogType { get; set; }

        public InspectedShipDataViewModel InspectedShip { get; set; }

        [Required]
        [ValidGroup("IsAquaculture")]
        public ValidStateInfiniteSelect<SelectNomenclatureDto> Aquaculture { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> FishType { get; set; }

        [TLRange(1, 10000, true)]
        public ValidState CatchCount { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> CatchType { get; set; }

        public ValidStateBool UndersizedFish { get; set; }

        [Required]
        [TLRange(1, 10000, true)]
        [ValidGroup("IsNNN")]
        public ValidState CatchQuantity { get; set; }

        [Required]
        [TLRange(0, 10000, true)]
        public ValidState UnloadedQuantity { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> Presentation { get; set; }

        [Required]
        [ValidGroup("IsNNN")]
        public ValidStateSelect<CatchZoneNomenclatureDto> CatchZone { get; set; }

        public ValidStateSelect<SelectNomenclatureDto> DeclarationType { get; set; }

        public ValidStateSelect<DeclarationLogBookPageDto> LogBookPage { get; set; }

        public ValidState LogBookPageNum { get; set; }

        public ValidStateDate LogBookPageDate { get; set; }

        public bool IsDeclarationRegistered
        {
            get => _isDeclarationRegistered;
            set => SetProperty(ref _isDeclarationRegistered, value);
        }
        public bool OnlyUnregisteredDeclaration
        {
            get => _onlyUnregisteredDeclaration;
            set => SetProperty(ref _onlyUnregisteredDeclaration, value);
        }
        public DeclarationLogBookType SubjectType
        {
            get => _subjectType;
            set => SetProperty(ref _subjectType, value);
        }

        public List<SelectNomenclatureDto> FishTypes
        {
            get => _fishTypes;
            set => SetProperty(ref _fishTypes, value);
        }
        public List<SelectNomenclatureDto> CatchTypes
        {
            get => _catchTypes;
            set => SetProperty(ref _catchTypes, value);
        }
        public List<SelectNomenclatureDto> Presentations
        {
            get => _presentations;
            set => SetProperty(ref _presentations, value);
        }
        public List<CatchZoneNomenclatureDto> CatchZones
        {
            get => _catchZones;
            set => SetProperty(ref _catchZones, value);
        }
        public List<SelectNomenclatureDto> DeclarationTypes
        {
            get => _declarationTypes;
            set => SetProperty(ref _declarationTypes, value);
        }
        public List<DeclarationLogBookPageDto> LogBookPages
        {
            get => _logBookPages;
            set => SetProperty(ref _logBookPages, value);
        }

        public ICommand Save { get; }
        public ICommand DeclarationSelected { get; }
        public ICommand AquacultureChosen { get; }

        public void OnInit()
        {
            InspectedShip = new InspectedShipDataViewModel(Inspection, false)
            {
                ShipSelected = CommandBuilder.CreateFrom<ShipSelectNomenclatureDto>(OnShipSelected)
            };

            this.AddValidation(others: new IValidatableViewModel[]
            {
                InspectedShip,
            }, groups: new Dictionary<string, Func<bool>>
            {
                { "IsAquaculture", () => SubjectType == DeclarationLogBookType.AquacultureLogBook },
                {
                    "IsShip",
                    () => SubjectType == DeclarationLogBookType.ShipLogBook
                        || SubjectType == DeclarationLogBookType.FirstSaleLogBook
                        || SubjectType == DeclarationLogBookType.TransportationLogBook
                        || SubjectType == DeclarationLogBookType.AdmissionLogBook
                },
                { "IsNNN", () => SubjectType == DeclarationLogBookType.NNN }
            });

            InspectedShip.Validation.GlobalGroups = new List<string> { "IsShip" };

            if (!HasCatchType)
            {
                CatchType.HasAsterisk = false;
                CatchType.Validations.Clear();
            }

            Aquaculture.ItemsSource = new TLObservableCollection<SelectNomenclatureDto>();
            Aquaculture.GetMore = (int page, int pageSize, string search) =>
                DependencyService.Get<INomenclatureTransaction>().GetAquacultures(page, pageSize, search);
        }

        public override async Task Initialize(object sender)
        {
            INomenclatureTransaction nomTransaction = DependencyService.Resolve<INomenclatureTransaction>();

            List<CatchZoneNomenclatureDto> catchZones = nomTransaction.GetCatchZones();

            FishTypes = nomTransaction.GetFishes();
            CatchTypes = nomTransaction.GetCatchInspectionTypes();
            Presentations = nomTransaction.GetFishPresentations();
            CatchZones = catchZones;

            InspectedShip.Init(nomTransaction.GetCountries(), nomTransaction.GetVesselTypes(), catchZones);

            Aquaculture.ItemsSource.AddRange(nomTransaction.GetAquacultures(0, CommonGlobalVariables.PullItemsCount));

            Array logBooksTypes = Enum.GetValues(typeof(DeclarationLogBookType));

            List<SelectNomenclatureDto> declarationTypes = new List<SelectNomenclatureDto>(logBooksTypes.Length);

            for (int i = 0; i < logBooksTypes.Length; i++)
            {
                string logBookType = logBooksTypes.GetValue(i).ToString();

                declarationTypes.Add(new SelectNomenclatureDto
                {
                    Id = i + 1,
                    Code = logBookType,
                    Name = TranslateExtension.Translator[nameof(GroupResourceEnum.DeclarationCatch) + "/" + logBookType]
                });
            }

            DeclarationTypes = declarationTypes;

            if (Edit != null)
            {
                CatchCount.AssignFrom(Edit.Dto.CatchCount);
                CatchQuantity.AssignFrom(Edit.Dto.CatchQuantity);
                CatchType.AssignFrom(Edit.Dto.CatchTypeId, CatchTypes);
                CatchZone.AssignFrom(Edit.Dto.CatchZoneId, CatchZones);
                FishType.AssignFrom(Edit.Dto.FishTypeId, FishTypes);
                Presentation.AssignFrom(Edit.Dto.PresentationId, Presentations);
                UnloadedQuantity.AssignFrom(Edit.Dto.UnloadedQuantity);
                UndersizedFish.AssignFrom(Edit.Dto.Undersized);

                if (Edit.Dto.OriginShip != null)
                {
                    InspectedShip.OnEdit(Edit.Dto.OriginShip);

                    if (Edit.Dto.OriginShip.IsRegistered == true
                        && Edit.Dto.OriginShip.ShipId != null)
                    {
                        ShipSelectNomenclatureDto ship = nomTransaction.GetShipNomenclature(Edit.Dto.OriginShip.ShipId.Value);

                        if (ship != null)
                        {
                            await OnShipSelected(ship);

                            DeclarationType.AssignFrom(Edit.Dto.LogBookType?.ToString(), DeclarationTypes);

                            await OnDeclarationSelected(DeclarationType.Value);

                            LogBookPageNum.AssignFrom(Edit.Dto.UnregisteredPageNum);
                            LogBookPageDate.AssignFrom(Edit.Dto.UnregisteredPageDate ?? DateTime.Now);

                            if (LogBookPages != null && Edit.Dto.LogBookPageId != null)
                            {
                                LogBookPage.Value = LogBookPages.Find(f => f.Id == Edit.Dto.LogBookPageId.Value);
                            }
                        }
                    }
                    else
                    {
                        DeclarationType.AssignFrom(Edit.Dto.LogBookType?.ToString(), DeclarationTypes);
                        LogBookPageNum.AssignFrom(Edit.Dto.UnregisteredPageNum);
                        LogBookPageDate.AssignFrom(Edit.Dto.UnregisteredPageDate ?? DateTime.Now);
                    }
                }
                else if (Edit.Dto.AquacultureId != null)
                {
                    Aquaculture.Value = nomTransaction.GetAquaculture(Edit.Dto.AquacultureId.Value);
                    DeclarationType.AssignFrom(Edit.Dto.LogBookType?.ToString(), DeclarationTypes);
                    LogBookPageNum.AssignFrom(Edit.Dto.UnregisteredPageNum);
                    LogBookPageDate.AssignFrom(Edit.Dto.UnregisteredPageDate ?? DateTime.Now);

                    SubjectType = Edit.Dto.LogBookType.Value;
                }

                if (Edit.Dto.LogBookType != null)
                {
                    SubjectType = Edit.Dto.LogBookType.Value;
                    //OnlyUnregisteredDeclaration = false;
                    //IsDeclarationRegistered = LogBookPages.Count > 0;
                }

                if (DialogType == ViewActivityType.Edit)
                {
                    Validation.Force();
                }
            }
            else
            {
                Presentation.Value = Presentations.Find(f => f.Code == "WHL");
            }
        }

        private async Task OnShipSelected(ShipSelectNomenclatureDto ship)
        {
            INomenclatureTransaction nomTransaction = DependencyService.Resolve<INomenclatureTransaction>();

            ShipDto chosenShip = nomTransaction.GetShip(ship.Id);

            if (chosenShip == null)
            {
                return;
            }

            ShipUid = ship.Uid;

            InspectedShip.InspectedShip = chosenShip;

            InspectedShip.CallSign.AssignFrom(chosenShip.CallSign);
            InspectedShip.MMSI.AssignFrom(chosenShip.MMSI);
            InspectedShip.CFR.AssignFrom(chosenShip.CFR);
            InspectedShip.ExternalMarkings.AssignFrom(chosenShip.ExtMarkings);
            InspectedShip.Name.AssignFrom(chosenShip.Name);
            InspectedShip.UVI.AssignFrom(chosenShip.UVI);
            InspectedShip.Flag.AssignFrom(chosenShip.FlagId, InspectedShip.Flags);
            InspectedShip.ShipType.AssignFrom(chosenShip.ShipTypeId, InspectedShip.ShipTypes);

            LogBookPage.Value = null;
            LogBookPageNum.Value = null;
            LogBookPageDate.Value = DateTime.Now;

            if (DeclarationType.Value != null)
            {
                IInspectionsTransaction inspTransaction = DependencyService.Resolve<IInspectionsTransaction>();

                LogBookPages = await inspTransaction.GetDeclarationLogBookPages(ParseHelper.ParseEnum<DeclarationLogBookType>(DeclarationType.Value.Code).Value, ShipUid.Value)
                    ?? new List<DeclarationLogBookPageDto>();

                OnlyUnregisteredDeclaration = false;
                IsDeclarationRegistered = LogBookPages.Count > 0;
            }
        }

        private async Task OnDeclarationSelected(SelectNomenclatureDto declarationType)
        {
            SubjectType = ParseHelper.ParseEnum<DeclarationLogBookType>(declarationType.Code).Value;

            switch (SubjectType)
            {
                case DeclarationLogBookType.FirstSaleLogBook:
                case DeclarationLogBookType.TransportationLogBook:
                case DeclarationLogBookType.AdmissionLogBook:
                case DeclarationLogBookType.ShipLogBook:
                    {
                        if (!ShipUid.HasValue)
                        {
                            break;
                        }

                        IInspectionsTransaction inspTransaction = DependencyService.Resolve<IInspectionsTransaction>();

                        LogBookPages = await inspTransaction.GetDeclarationLogBookPages(SubjectType, ShipUid.Value)
                            ?? new List<DeclarationLogBookPageDto>();

                        OnlyUnregisteredDeclaration = false;
                        IsDeclarationRegistered = LogBookPages.Count > 0;
                        break;
                    }
                case DeclarationLogBookType.AquacultureLogBook:
                    {
                        if (Aquaculture.Value == null)
                        {
                            break;
                        }

                        IInspectionsTransaction inspTransaction = DependencyService.Resolve<IInspectionsTransaction>();

                        LogBookPages = await inspTransaction.GetDeclarationLogBookPages(SubjectType, Aquaculture.Value.Id)
                            ?? new List<DeclarationLogBookPageDto>();

                        OnlyUnregisteredDeclaration = false;
                        IsDeclarationRegistered = LogBookPages.Count > 0;
                        break;
                    }
                case DeclarationLogBookType.Invoice:
                case DeclarationLogBookType.NNN:
                    {
                        OnlyUnregisteredDeclaration = true;
                        IsDeclarationRegistered = false;
                        break;
                    }
            }
        }

        private async Task OnAquacultureChosen()
        {
            LogBookPage.Value = null;
            LogBookPageNum.Value = null;
            LogBookPageDate.Value = DateTime.Now;

            if (DeclarationType.Value != null)
            {
                IInspectionsTransaction inspTransaction = DependencyService.Resolve<IInspectionsTransaction>();

                LogBookPages = await inspTransaction.GetDeclarationLogBookPages(ParseHelper.ParseEnum<DeclarationLogBookType>(DeclarationType.Value.Code).Value, ShipUid.Value)
                    ?? new List<DeclarationLogBookPageDto>();

                OnlyUnregisteredDeclaration = false;
                IsDeclarationRegistered = LogBookPages.Count > 0;
            }
        }

        private Task OnSave()
        {
            Validation.Force();

            if (Validation.IsValid)
            {
                return HideDialog(new DeclarationCatchModel
                {
                    Type = FishType.Value?.DisplayValue,
                    CatchType = CatchType.Value?.DisplayValue,
                    CatchZone = CatchZone.Value?.Name,
                    Presentation = Presentation.Value?.Name,
                    Dto = new InspectedDeclarationCatchDto
                    {
                        Id = Edit?.Dto.Id,
                        CatchQuantity = ParseHelper.ParseDecimal(CatchQuantity.Value),
                        CatchCount = ParseHelper.ParseInteger(CatchCount.Value),
                        CatchTypeId = HasUndersizedCheck
                            ? (UndersizedFish ? CatchTypes.Find(f => f.Code == nameof(CatchSizeCodesEnum.BMS))?.Id : CatchTypes.Find(f => f.Code == nameof(CatchSizeCodesEnum.LSC))?.Id)
                            : CatchType.Value,
                        CatchZoneId = CatchZone.Value,
                        FishTypeId = FishType.Value,
                        PresentationId = Presentation.Value,
                        UnloadedQuantity = ParseHelper.ParseDecimal(UnloadedQuantity.Value),
                        AquacultureId = Aquaculture.Value?.Id,
                        OriginShip = InspectedShip,
                        LogBookType = ParseHelper.ParseEnum<DeclarationLogBookType>(DeclarationType.Value?.Code),
                        LogBookPageId = LogBookPage.Value?.Id,
                        UnregisteredPageNum = LogBookPageNum.Value,
                        UnregisteredPageDate = LogBookPageDate.Value,
                        Undersized = UndersizedFish,
                    }
                });
            }

            return Task.CompletedTask;
        }
    }
}
