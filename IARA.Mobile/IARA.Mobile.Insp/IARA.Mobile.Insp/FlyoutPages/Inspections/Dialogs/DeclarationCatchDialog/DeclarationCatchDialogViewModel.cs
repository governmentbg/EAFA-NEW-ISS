using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
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
        private List<SelectNomenclatureDto> _catchTypes;
        private List<CatchZoneNomenclatureDto> _catchZones;
        private List<SelectNomenclatureDto> _declarationTypes;
        private List<DeclarationLogBookPageDto> _logBookPages;
        private List<SelectNomenclatureDto> _fishTypes;
        private List<SelectNomenclatureDto> _presentations;
        private InspectedLogBookPageDataDto _connectedDeclarations;

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
            AddCatch = CommandBuilder.CreateFrom(OnAddCatch);
            ViewCatch = CommandBuilder.CreateFrom<InspectedDeclarationCatchDto>(OnViewCatch);
            EditCatch = CommandBuilder.CreateFrom<InspectedDeclarationCatchDto>(OnEditCatch);
            RemoveCatch = CommandBuilder.CreateFrom<InspectedDeclarationCatchDto>(OnRemoveCatch);
            GeneratePageProducts = CommandBuilder.CreateFrom(OnGeneratePageProducts);
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
        public ValidStateSelect<SelectNomenclatureDto> DeclarationType { get; set; }

        public ValidStateSelect<DeclarationLogBookPageDto> LogBookPage { get; set; }

        public ValidState LogBookNum { get; set; }

        public ValidStateDate LogBookPageDate { get; set; }

        [MaxLength(4000)]
        public ValidState InvoiceInformation { get; set; }

        public ValidStateTable<InspectedDeclarationCatchDto> DeclarationCatches { get; set; }

        public ValidState DocumentOriginNumber { get; set; }

        public ValidStateDate LogBookPageOriginDate { get; set; }

        public ValidState DocumentTransportNumber { get; set; }

        public ValidStateDate DocumentTransportDate { get; set; }

        public ValidState DocumentAdmissionNumber { get; set; }

        public ValidStateDate DeclarationAdmissionDate { get; set; }

        public ValidState DocumentFirstSaleNumber { get; set; }

        public ValidStateDate DocumentFirstSaleDate { get; set; }

        public ValidState DocumentAquacultureNumber { get; set; }

        public ValidStateDate DocumentAquacultureDate { get; set; }

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

        public List<SelectNomenclatureDto> CatchTypes
        {
            get => _catchTypes;
            set => SetProperty(ref _catchTypes, value);
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
        public List<SelectNomenclatureDto> FishTypes
        {
            get => _fishTypes;
            set => SetProperty(ref _fishTypes, value);
        }
        public List<SelectNomenclatureDto> Presentations
        {
            get => _presentations;
            set => SetProperty(ref _presentations, value);
        }

        public InspectedLogBookPageDataDto ConnectedDeclarations
        {
            get => _connectedDeclarations;
            set => SetProperty(ref _connectedDeclarations, value);
        }


        public ICommand Save { get; }
        public ICommand DeclarationSelected { get; }
        public ICommand AquacultureChosen { get; }
        public ICommand PageSelected { get; }
        public ICommand AddPage { get; set; }
        public ICommand AddCatch { get; set; }
        public ICommand ViewCatch { get; set; }
        public ICommand EditCatch { get; set; }
        public ICommand RemoveCatch { get; set; }
        public ICommand GeneratePageProducts { get; set; }

        public void OnInit()
        {
            InspectedShip = new InspectedShipDataViewModel(new EmptyCopy(DialogType), null, false)
            {
                ShipSelected = CommandBuilder.CreateFrom<ShipSelectNomenclatureDto>(OnShipSelected)
            };

            this.AddValidation(others: new IValidatableViewModel[]
            {
                InspectedShip,
            }, groups: new Dictionary<string, Func<bool>>
            {
                { "IsAquaculture", () => SubjectType == DeclarationLogBookType.AquacultureLogBook },
                { "IsShip", () => SubjectType == DeclarationLogBookType.ShipLogBook
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

            CatchTypes = nomTransaction.GetCatchInspectionTypes();
            FishTypes = nomTransaction.GetFishes();
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
                DeclarationCatches.Value.AddRange(Edit.InspectionLogBookPage.InspectionCatchMeasures);
                DeclarationType.AssignFrom(Edit.InspectionLogBookPage.LogBookType.ToString(), DeclarationTypes);
                await OnDeclarationSelected(DeclarationType.Value);
                SubjectType = Edit.InspectionLogBookPage.LogBookType.Value;

                if (Edit.InspectionLogBookPage.OriginShip != null)
                {
                    InspectedShip.OnEdit(Edit.InspectionLogBookPage.OriginShip);

                    if (Edit.InspectionLogBookPage.OriginShip.IsRegistered == true
                        && Edit.InspectionLogBookPage.OriginShip.ShipId != null)
                    {
                        ShipSelectNomenclatureDto ship = nomTransaction.GetShipNomenclature(Edit.InspectionLogBookPage.OriginShip.ShipId.Value);

                        if (ship != null)
                        {
                            await OnShipSelected(ship);
                        }
                    }
                }
                else if (Edit.InspectionLogBookPage.AquacultureId != null)
                {
                    IsAquacultureInRegistered.AssignFrom(true);
                    SelectNomenclatureDto aquaculture = nomTransaction.GetAquaculture(Edit.InspectionLogBookPage.AquacultureId.Value);
                    Aquaculture.Value = aquaculture;
                    await OnAquacultureChosen(aquaculture);
                }
                else if (Edit.InspectionLogBookPage.LogBookType == DeclarationLogBookType.AquacultureLogBook)
                {
                    IsAquacultureInRegistered.AssignFrom(false);
                    UnregisteredAquaculture.AssignFrom(Edit.InspectionLogBookPage.UnregisteredEntityData);
                }
                else
                {
                    InvoiceInformation.AssignFrom(Edit.InspectionLogBookPage.UnregisteredEntityData);
                }

                if (LogBookPages != null && LogBookPages.Count() > 0)
                {
                    int? logBookId = null;
                    switch (Edit.InspectionLogBookPage.LogBookType.Value)
                    {
                        case DeclarationLogBookType.FirstSaleLogBook:
                            logBookId = Edit.InspectionLogBookPage.FirstSaleLogBookPageId;
                            break;
                        case DeclarationLogBookType.TransportationLogBook:
                            logBookId = Edit.InspectionLogBookPage.TransportationLogBookPageId;
                            break;
                        case DeclarationLogBookType.AdmissionLogBook:
                            logBookId = Edit.InspectionLogBookPage.AdmissionLogBookPageId;
                            break;
                        case DeclarationLogBookType.ShipLogBook:
                            logBookId = Edit.InspectionLogBookPage.ShipLogBookPageId;
                            break;
                        case DeclarationLogBookType.AquacultureLogBook:
                            logBookId = Edit.InspectionLogBookPage.AquacultureLogBookPageId;
                            break;
                        default:
                            break;
                    }

                    if (logBookId != null)
                    {
                        DeclarationLogBookPageDto declaration = LogBookPages.Find(f => f.Id == logBookId.Value);
                        LogBookPage.Value = declaration;
                        await OnPageSelected(declaration);
                    }
                }
                else
                {
                    OnAddPage(Edit.InspectionLogBookPage.UnregisteredPageNum);
                    LogBookNum.AssignFrom(Edit.InspectionLogBookPage.UnregisteredLogBookNum);
                    LogBookPageDate.AssignFrom(Edit.InspectionLogBookPage.UnregisteredPageDate);

                    DeclarationCatches.Value.AddRange(Edit.InspectionLogBookPage.InspectionCatchMeasures);
                }

                if (DialogType == ViewActivityType.Edit)
                {
                    Validation.Force();
                }
            }
        }
        private async Task OnRemoveCatch(InspectedDeclarationCatchDto dto)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DeleteMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
            );

            if (result)
            {
                DeclarationCatches.Value.Remove(dto);
            }
        }

        private async Task OnEditCatch(InspectedDeclarationCatchDto dto)
        {
            InspectedDeclarationCatchDto declarationCatchDto = await TLDialogHelper.ShowDialog(new CatchDialog.CatchDialog(ViewActivityType.Add, dto, FishTypes, Presentations));
            if (declarationCatchDto != null)
            {
                dto.Id = declarationCatchDto.Id;
                dto.InspectionLogBookPageId = declarationCatchDto.InspectionLogBookPageId;
                dto.CatchTypeId = declarationCatchDto.CatchTypeId;
                dto.FishTypeId = declarationCatchDto.FishTypeId;
                dto.CatchCount = declarationCatchDto.CatchCount;
                dto.CatchQuantity = declarationCatchDto.CatchQuantity;
                dto.UnloadedQuantity = declarationCatchDto.UnloadedQuantity;
                dto.PresentationId = declarationCatchDto.PresentationId;
                dto.Undersized = declarationCatchDto.Undersized;
                dto.CatchZoneId = declarationCatchDto.CatchZoneId;
                dto.TurbotSizeGroupId = declarationCatchDto.TurbotSizeGroupId;
                dto.OriginShip = declarationCatchDto.OriginShip;
                dto.AquacultureId = declarationCatchDto.AquacultureId;
                dto.UnregisteredEntityData = declarationCatchDto.UnregisteredEntityData;
                dto.LogBookPageId = declarationCatchDto.LogBookPageId;
                dto.LogBookType = declarationCatchDto.LogBookType;
                dto.UnregisteredPageNum = declarationCatchDto.UnregisteredPageNum;
                dto.UnregisteredLogBookNum = declarationCatchDto.UnregisteredLogBookNum;
                dto.UnregisteredPageDate = declarationCatchDto.UnregisteredPageDate;

                DeclarationCatches.Value.Replace(dto, dto);
            }
        }

        private Task OnViewCatch(InspectedDeclarationCatchDto dto)
        {
            return TLDialogHelper.ShowDialog(new CatchDialog.CatchDialog(ViewActivityType.Review, dto, _fishTypes, _presentations));
        }

        private void OnGeneratePageProducts()
        {
            if (ConnectedDeclarations != null && ConnectedDeclarations.InspectionCatchMeasures != null)
            {
                DeclarationCatches.Value.Clear();
                DeclarationCatches.Value.AddRange(ConnectedDeclarations.InspectionCatchMeasures);
            }
        }

        private async Task OnAddCatch()
        {
            InspectedDeclarationCatchDto dto = await TLDialogHelper.ShowDialog(new CatchDialog.CatchDialog(ViewActivityType.Add, null, _fishTypes, _presentations));

            if (dto != null)
            {
                DeclarationCatches.Value.Add(dto);
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

            LogBookNum.AssignFrom("");
            LogBookPageDate.AssignFrom(null);
            //IsLogBookPageSelected = false;
            DeclarationCatches.Value.Clear();
            ConnectedDeclarations = null;
            DocumentOriginNumber.AssignFrom("");
            LogBookPageOriginDate.AssignFrom(null);
            DocumentTransportNumber.AssignFrom("");
            DocumentTransportDate.AssignFrom(null);
            DocumentAdmissionNumber.AssignFrom("");
            DeclarationAdmissionDate.AssignFrom(null);
            DocumentFirstSaleNumber.AssignFrom("");
            DocumentFirstSaleDate.AssignFrom(null);
            DocumentAquacultureNumber.AssignFrom("");
            DocumentAquacultureDate.AssignFrom(null);
        }

        private async Task OnPageSelected(DeclarationLogBookPageDto dto)
        {
            if (dto != null)
            {
                if (dto.Id != null)
                {
                    if (dto.LogBookType != DeclarationLogBookType.AquacultureLogBook)
                    {
                        IsLogBookPageSelected = true;
                    }
                    LogBookNum.AssignFrom(dto.LogBookNumber);
                    LogBookPageDate.AssignFrom(dto.Date);

                    int logBookPageId = dto.Id.Value;
                    DeclarationLogBookType type = dto.LogBookType;
                    HttpResult<InspectedLogBookPageDataDto> result = await DependencyService.Resolve<IRestClient>().GetAsync<InspectedLogBookPageDataDto>("InspectionData/GetInspectedLogBookPageData", new { logBookPageId, type });
                    if (result.IsSuccessful)
                    {
                        ConnectedDeclarations = result.Content;

                        DocumentOriginNumber.AssignFrom(ConnectedDeclarations.ShipLogBookPageNumber);
                        LogBookPageOriginDate.AssignFrom(ConnectedDeclarations.ShipPageFillDate);

                        if (dto.LogBookType != DeclarationLogBookType.TransportationLogBook && ConnectedDeclarations.TransportationLogBookPageId != null)
                        {
                            DocumentTransportNumber.AssignFrom(ConnectedDeclarations.TransportationLogBookPageNumber);
                            DocumentTransportDate.AssignFrom(ConnectedDeclarations.TransportationPageLoadingDate);
                        }

                        if (dto.LogBookType != DeclarationLogBookType.AdmissionLogBook && ConnectedDeclarations.AdmissionLogBookPageId != null)
                        {
                            DocumentAdmissionNumber.AssignFrom(ConnectedDeclarations.AdmissionLogBookPageNumber);
                            DeclarationAdmissionDate.AssignFrom(ConnectedDeclarations.AdmissionPageHandoverDate);
                        }

                        if (dto.LogBookType != DeclarationLogBookType.FirstSaleLogBook && ConnectedDeclarations.FirstSaleLogBookPageId != null)
                        {
                            DocumentFirstSaleNumber.AssignFrom(ConnectedDeclarations.FirstSaleLogBookPageNumber);
                            DocumentFirstSaleDate.AssignFrom(ConnectedDeclarations.FirstSalePageSaleDate);
                        }

                        if (dto.LogBookType != DeclarationLogBookType.AquacultureLogBook && ConnectedDeclarations.AquacultureLogBookPageId != null)
                        {
                            DocumentAquacultureNumber.AssignFrom(ConnectedDeclarations.AquacultureLogBookPageNumber);
                            DocumentAquacultureDate.AssignFrom(ConnectedDeclarations.AquaculturePageFillingDate);
                        }
                    }
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
            if (Validation.IsValid)
            {
                DeclarationLogBookType type = ParseHelper.ParseEnum<DeclarationLogBookType>(DeclarationType.Value.Code).Value;

                string name = _declarationTypes.Where(x => x.Code == type.ToString()).FirstOrDefault()?.Name;
                return HideDialog(new DeclarationCatchModel
                {
                    DocumentType = name,
                    PageNumber = LogBookPage.Value?.Num,
                    PageDate = LogBookPageDate.Value,
                    Information = string.Join(", ", DeclarationCatches.Value.Select(x =>
                    {
                        return $"{FishTypes.Where(f => f.Id == x.FishTypeId).First().DisplayValue} - {x.CatchQuantity:f2}кг";
                    }).ToArray()),
                    InspectionLogBookPage = new InspectionLogBookPageDto
                    {
                        Id = Edit?.InspectionLogBookPage.Id,
                        OriginShip = InspectedShip,
                        LogBookType = type,
                        FirstSaleLogBookPageId = type == DeclarationLogBookType.FirstSaleLogBook ? LogBookPage.Value?.Id : null,
                        TransportationLogBookPageId = type == DeclarationLogBookType.TransportationLogBook ? LogBookPage.Value?.Id : null,
                        AdmissionLogBookPageId = type == DeclarationLogBookType.AdmissionLogBook ? LogBookPage.Value?.Id : null,
                        ShipLogBookPageId = type == DeclarationLogBookType.ShipLogBook ? LogBookPage.Value?.Id : null,
                        AquacultureLogBookPageId = type == DeclarationLogBookType.AquacultureLogBook ? LogBookPage.Value?.Id : null,
                        UnregisteredPageNum = LogBookPage.Value?.Num,
                        UnregisteredPageDate = LogBookPageDate.Value,
                        UnregisteredLogBookNum = LogBookNum.Value,
                        AquacultureId = Aquaculture.Value?.Id,
                        UnregisteredEntityData = SubjectType == DeclarationLogBookType.AquacultureLogBook ? UnregisteredAquaculture.Value : InvoiceInformation.Value,
                        InspectionCatchMeasures = DeclarationCatches.Value.ToList(),
                    }
                });
            }

            return Task.CompletedTask;
        }
    }
}
