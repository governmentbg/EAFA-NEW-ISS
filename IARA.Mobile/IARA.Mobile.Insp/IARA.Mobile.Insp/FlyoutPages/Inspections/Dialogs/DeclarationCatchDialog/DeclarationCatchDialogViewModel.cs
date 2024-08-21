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
using System.Linq;
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
        private bool _isLogBookPageSelected;
        private bool _doDeclarationRequireCatchZone;
        private bool _isDeclarationSelected;
        private bool _hasUndersizedFishControl;
        private DeclarationLogBookType _subjectType;

        public DeclarationCatchDialogViewModel()
        {
            Save = CommandBuilder.CreateFrom(OnSave);
            DeclarationSelected = CommandBuilder.CreateFrom<SelectNomenclatureDto>(OnDeclarationSelected);
            PageSelected = CommandBuilder.CreateFrom<DeclarationLogBookPageDto>(OnPageSelected);
            AquacultureChosen = CommandBuilder.CreateFrom<SelectNomenclatureDto>(OnAquacultureChosen);
            AddPage = CommandBuilder.CreateFrom<string>(OnAddPage);
            SubjectType = DeclarationLogBookType.NNN;
            _onlyUnregisteredDeclaration = true;
        }

        public DeclarationCatchModel Edit { get; set; }
        public int? ShipUid { get; set; }
        public bool HasCatchType { get; set; }
        public bool HasUndersizedCheck { get; set; }
        public bool HasUnloadedQuantity { get; set; }
        public bool IsCatchZoneKnown { get; set; }
        public DeclarationCatchesViewModel Catches { get; set; }
        public InspectionPageViewModel Inspection { get; set; }
        public ViewActivityType DialogType { get; set; }

        public InspectedShipDataViewModel InspectedShip { get; set; }

        [Required]
        [ValidGroup("IsAquaculture")]
        [ValidGroup("IsAquacultureRegistered")]
        public ValidStateInfiniteSelect<SelectNomenclatureDto> Aquaculture { get; set; }

        [Required]
        [ValidGroup("IsAquaculture")]
        public ValidState UnregisteredAquaculture { get; set; }

        public ValidStateBool IsAquacultureInRegistered { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> FishType { get; set; }

        [TLRange(1, 10000, true)]
        public ValidState CatchCount { get; set; }

        public ValidStateBool UndersizedFish { get; set; }

        [Required]
        [TLRange(1, 10000, true)]
        public ValidState CatchQuantity { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> Presentation { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> DeclarationType { get; set; }

        public ValidStateSelect<DeclarationLogBookPageDto> LogBookPage { get; set; }

        public ValidState LogBookNum { get; set; }

        public ValidStateDate LogBookPageDate { get; set; }

        public ValidState DocumentOriginNumber { get; set; }

        public ValidStateDate LogBookPageOriginDate { get; set; }

        [MaxLength(4000)]
        public ValidState InvoiceInformation { get; set; }

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
        public bool DoDeclarationRequireCatchZone
        {
            get => _doDeclarationRequireCatchZone;
            set => SetProperty(ref _doDeclarationRequireCatchZone, value);
        }
        public bool IsDeclarationSelected
        {
            get => _isDeclarationSelected;
            set => SetProperty(ref _isDeclarationSelected, value);
        }
        public bool IsLogBookPageSelected
        {
            get => _isLogBookPageSelected;
            set => SetProperty(ref _isLogBookPageSelected, value);
        }
        public bool HasUndersizedFishControl
        {
            get => _hasUndersizedFishControl;
            set => SetProperty(ref _hasUndersizedFishControl, value);
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
        public ICommand PageSelected { get; }
        public ICommand AddPage { get; set; }

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
                { "IsNNN", () => SubjectType == DeclarationLogBookType.NNN },
                { "IsAquacultureRegistered", () => IsAquacultureInRegistered.Value == true }
            });

            InspectedShip.Validation.GlobalGroups = new List<string> { "IsShip" };

            Aquaculture.ItemsSource = new TLObservableCollection<SelectNomenclatureDto>();
            LogBookPages = new List<DeclarationLogBookPageDto>();
            Aquaculture.GetMore = (int page, int pageSize, string search) =>
                DependencyService.Resolve<INomenclatureTransaction>().GetAquacultures(page, pageSize, search);
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
            IsAquacultureInRegistered.AssignFrom(true);

            if (Edit != null)
            {
                CatchCount.AssignFrom(Edit.Dto.CatchCount);
                CatchQuantity.AssignFrom(Edit.Dto.CatchQuantity);
                FishType.AssignFrom(Edit.Dto.FishTypeId, FishTypes);
                Presentation.AssignFrom(Edit.Dto.PresentationId, Presentations);
                UndersizedFish.AssignFrom(Edit.Dto.Undersized);
                DeclarationType.AssignFrom(Edit.Dto.LogBookType.ToString(), DeclarationTypes);
                await OnDeclarationSelected(DeclarationType.Value);
                SubjectType = Edit.Dto.LogBookType.Value;

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
                        }
                    }
                }
                else if (Edit.Dto.AquacultureId != null)
                {
                    IsAquacultureInRegistered.AssignFrom(true);
                    SelectNomenclatureDto aquaculture = nomTransaction.GetAquaculture(Edit.Dto.AquacultureId.Value);
                    Aquaculture.Value = aquaculture;
                    await OnAquacultureChosen(aquaculture);
                }
                else if (Edit.Dto.LogBookType == DeclarationLogBookType.AquacultureLogBook)
                {
                    IsAquacultureInRegistered.AssignFrom(false);
                    UnregisteredAquaculture.AssignFrom(Edit.Dto.UnregisteredEntityData);
                }
                else
                {
                    InvoiceInformation.AssignFrom(Edit.Dto.UnregisteredEntityData);
                }

                if (LogBookPages != null && Edit.Dto.LogBookPageId != null)
                {
                    DeclarationLogBookPageDto declaration = LogBookPages.Find(f => f.Id == Edit.Dto.LogBookPageId.Value);
                    LogBookPage.Value = declaration;
                    OnPageSelected(declaration);
                }
                else
                {
                    OnAddPage(Edit.Dto.UnregisteredPageNum);
                    LogBookNum.AssignFrom(Edit.Dto.UnregisteredLogBookNum);
                    LogBookPageDate.AssignFrom(Edit.Dto.UnregisteredPageDate);
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


        private void OnAddPage(string text)
        {
            var newPage = new DeclarationLogBookPageDto
            {
                Num = text
            };
            LogBookPages.Add(newPage);
            LogBookPage.Value = newPage;
        }


        private void OnPageSelected(DeclarationLogBookPageDto dto)
        {
            if (dto != null)
            {
                if (dto.Id != null)
                {
                    IsLogBookPageSelected = true;
                    LogBookNum.AssignFrom(dto.LogBookNumber);
                    LogBookPageDate.AssignFrom(dto.Date);
                    DocumentOriginNumber.AssignFrom(dto.LogBookPageOrigin);
                    LogBookPageOriginDate.AssignFrom(dto.LogBookPageOriginDate);
                }
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
            LogBookNum.Value = null;

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
            IsDeclarationSelected = true;
            IsLogBookPageSelected = false;
            LogBookNum.AssignFrom("");
            LogBookPageDate.AssignFrom(null);
            DocumentOriginNumber.AssignFrom("");
            LogBookPageOriginDate.AssignFrom(null);
            LogBookPage.Value = null;
            LogBookPages.Clear();

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
                        DoDeclarationRequireCatchZone = true;
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
                        DoDeclarationRequireCatchZone = false;
                        break;
                    }
                case DeclarationLogBookType.Invoice:
                case DeclarationLogBookType.NNN:
                    {
                        OnlyUnregisteredDeclaration = true;
                        IsDeclarationRegistered = false;
                        DoDeclarationRequireCatchZone = false;
                        break;
                    }
            }
        }

        private async Task OnAquacultureChosen(SelectNomenclatureDto dto)
        {
            LogBookPage.Value = null;
            LogBookNum.Value = null;

            if (DeclarationType.Value != null)
            {
                IInspectionsTransaction inspTransaction = DependencyService.Resolve<IInspectionsTransaction>();

                LogBookPages = await inspTransaction.GetDeclarationLogBookPages(ParseHelper.ParseEnum<DeclarationLogBookType>(DeclarationType.Value.Code).Value, dto.Id)
                    ?? new List<DeclarationLogBookPageDto>();

                OnlyUnregisteredDeclaration = false;
                IsDeclarationRegistered = LogBookPages.Count > 0;

                UnregisteredAquaculture.AssignFrom(dto.DisplayValue);
            }
        }

        private Task OnSave()
        {
            Validation.Force();
            var a = Validation.ValidStates.Where(x => x.Value.IsValid == false);
            if (Validation.IsValid)
            {
                return HideDialog(new DeclarationCatchModel
                {
                    Type = FishType.Value?.DisplayValue,
                    Presentation = Presentation.Value?.Name,
                    Dto = new InspectedDeclarationCatchDto
                    {
                        Id = Edit?.Dto.Id,
                        CatchQuantity = ParseHelper.ParseDecimal(CatchQuantity.Value),
                        CatchCount = ParseHelper.ParseInteger(CatchCount.Value),
                        FishTypeId = FishType.Value,
                        PresentationId = Presentation.Value,
                        OriginShip = InspectedShip,
                        LogBookType = ParseHelper.ParseEnum<DeclarationLogBookType>(DeclarationType.Value?.Code),
                        LogBookPageId = LogBookPage.Value?.Id,
                        UnregisteredPageNum = LogBookPage.Value?.Num,
                        UnregisteredPageDate = LogBookPageDate.Value,
                        UnregisteredLogBookNum = LogBookNum.Value,
                        AquacultureId = Aquaculture.Value?.Id,
                        UnregisteredEntityData = SubjectType == DeclarationLogBookType.AquacultureLogBook ? UnregisteredAquaculture.Value : InvoiceInformation.Value,
                        Undersized = UndersizedFish,
                    }
                });
            }

            return Task.CompletedTask;
        }
    }
}
