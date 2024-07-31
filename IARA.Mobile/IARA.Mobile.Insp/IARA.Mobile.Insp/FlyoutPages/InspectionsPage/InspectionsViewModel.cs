using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Application;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.Filters;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.FlyoutPages.Inspections.AquacultureFarmInspection;
using IARA.Mobile.Insp.FlyoutPages.Inspections.BoatOnOpenWater;
using IARA.Mobile.Insp.FlyoutPages.Inspections.ConstativeProtocol;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FloppyDiskInspectionDialog;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FloppyDiskInspectionUWPDialog;
using IARA.Mobile.Insp.FlyoutPages.Inspections.FirstSaleInspection;
using IARA.Mobile.Insp.FlyoutPages.Inspections.FishermanInspection;
using IARA.Mobile.Insp.FlyoutPages.Inspections.FishingGearInspection;
using IARA.Mobile.Insp.FlyoutPages.Inspections.HarbourInspection;
using IARA.Mobile.Insp.FlyoutPages.Inspections.InspectionWater;
using IARA.Mobile.Insp.FlyoutPages.Inspections.InWaterOnBoard;
using IARA.Mobile.Insp.FlyoutPages.Inspections.TranshipmentInspection;
using IARA.Mobile.Insp.FlyoutPages.Inspections.VehicleInspection;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.ViewModels.Models;
using IARA.Mobile.Shared.Helpers;
using IARA.Mobile.Shared.Menu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.InspectionsPage
{
    public class InspectionsViewModel : MainPageViewModel
    {
        private InspectionsFilters _inspectionsFilters = null;
        public InspectionsViewModel()
        {
            IsBusy = false;
            Inspections = new TLPagedCollection<InspectionDto>();

            SignInspection = CommandBuilder.CreateFrom<InspectionDto>(OnSignInspection);
            GetStartupData = CommandBuilder.CreateFrom(OnGetStartupData);
            Reload = CommandBuilder.CreateFrom(OnReload);
            GoToAddInspection = CommandBuilder.CreateFrom(OnGoToAddInspection);
            OpenInspection = CommandBuilder.CreateFrom<InspectionDto>(OnOpenInspection);
            DeleteInspection = CommandBuilder.CreateFrom<InspectionDto>(OnDeleteInspection);
            Inspections.GoToPage = CommandBuilder.CreateFrom<int>(OnGoToPage);

            Filter = CommandBuilder.CreateFrom(OnFilter);
            ClearFilters = CommandBuilder.CreateFrom(OnClearFilters);

            this.AddValidation();
        }

        public TLPagedCollection<InspectionDto> Inspections { get; }

        public ICommand SignInspection { get; }
        public ICommand GoToAddInspection { get; }
        public ICommand OpenInspection { get; }
        public ICommand DeleteInspection { get; }
        public ICommand Reload { get; }
        public ICommand GetStartupData { get; }
        public ICommand Filter { get; }
        public ICommand ClearFilters { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] { GroupResourceEnum.Inspections, GroupResourceEnum.GeneralInfo };
        }

        public ValidStateDate StartDate { get; set; }
        public ValidStateDate EndDate { get; set; }

        public override async void OnAppearing()
        {
            await OnReload();
        }

        public override Task Initialize(object sender)
        {
            DependencyService.Resolve<IConnectivity>().OfflineDataPosted += OnConnectivityOfflineDataPosted;
            return Task.CompletedTask;
        }

        private async void OnConnectivityOfflineDataPosted(object sender, EventArgs e)
        {
            await OnReload();
        }

        private async Task OnSignInspection(InspectionDto dto)
        {
            bool result;

            if (Device.RuntimePlatform == Device.UWP)
            {
                FileModel fileResult = await TLDialogHelper.ShowDialog(new SaveInspectionUWPDialog());

                if (fileResult == null)
                {
                    result = false;
                }
                else
                {
                    fileResult.FileTypeId = NomenclaturesTransaction.GetFileType(Constants.SignedReport);

                    await TLLoadingHelper.ShowFullLoadingScreen();
                    result = await InspectionsTransaction.SignInspection(dto.Id, new List<FileModel> { fileResult });
                }
            }
            else
            {
                result = await MobileSign(dto);
            }

            if (result)
            {
                await OnReload();
            }

            await TLLoadingHelper.HideFullLoadingScreen();
        }

        private async Task OnGetStartupData()
        {
            await TLLoadingHelper.ShowFullLoadingScreen();
            IStartupTransaction starup = DependencyService.Resolve<IStartupTransaction>();
            await starup.GetInitialData(false, null, null);
            await OnReload();
            await TLLoadingHelper.HideFullLoadingScreen();
        }

        private Task OnReload()
        {
            return OnGoToPage(Inspections.Page);
        }

        private async Task OnGoToPage(int page)
        {
            if (IsBusy)
            {
                return;
            }

            IsBusy = true;
            List<InspectionDto> inspections = null;
            try
            {
                inspections = await InspectionsTransaction.GetAll(page, _inspectionsFilters);
            }
            catch (Exception ex)
            {
                await TLSnackbar.Show(ex.Message, App.GetResource<Color>("ErrorColor"));
            }

            if (inspections == null)
            {
                return;
            }

            int pageCount = InspectionsTransaction.GetPageCount(_inspectionsFilters);

            Inspections.PageCount = pageCount;
            Inspections.Page = pageCount >= page ? page : pageCount;

            Inspections.ReplaceRange(inspections);
            IsBusy = false;
        }

        private Task OnGoToAddInspection()
        {
            return LocalPopupHelper.OpenAddInspectionsDrawer();
        }

        private async Task OnDeleteInspection(InspectionDto dto)
        {
            bool result = await InspectionsTransaction.DeleteInspection(dto.Id);

            if (result)
            {
                Inspections.Remove(dto);
            }
        }

        private Task OnFilter()
        {
            _inspectionsFilters = new InspectionsFilters();
            if (StartDate.Value != null)
            {
                _inspectionsFilters.DateFrom = StartDate.Value.Value;
            }
            if (EndDate.Value != null)
            {
                _inspectionsFilters.DateTo = EndDate.Value.Value;
            }
            Inspections.Page = 1;

            return OnReload();
        }

        private Task OnClearFilters()
        {
            _inspectionsFilters = null;
            StartDate.Value = null;
            EndDate.Value = null;
            Inspections.Page = 1;

            return OnReload();
        }

        private async Task OnOpenInspection(InspectionDto dto)
        {
            if (!dto.HasContentLocally && CommonGlobalVariables.InternetStatus == InternetStatus.Disconnected)
            {
                await TLSnackbar.Show(
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/WarningNoInternetTitle"],
                    App.GetResource<Color>("ErrorColor")
                );
                return;
            }

            await TLLoadingHelper.ShowFullLoadingScreen();

            if (dto.SubmitType != SubmitType.Finish)
            {
                GlobalVariables.IsAddingInspection = true;
            }

            ViewActivityType viewActivity = dto.SubmitType == SubmitType.Finish || !dto.CreatedByCurrentUser
                ? ViewActivityType.Review
                : ViewActivityType.Edit;

            await ForInspectionType(dto, viewActivity);
        }

        private Task ForInspectionType(InspectionDto dto, ViewActivityType viewActivity)
        {
            switch (dto.Type)
            {
                case InspectionType.OTH:
                    return HandleInspection(
                        InspectionsTransaction.GetOTH(dto.Id, dto.IsLocal),
                        (oth) => new ConstativeProtocolPage(oth)
                    );
                case InspectionType.OFS:
                    return HandleInspection(
                        InspectionsTransaction.GetOFS(dto.Id, dto.IsLocal),
                        (ofs) => new BoatOnOpenWaterPage(dto.SubmitType, viewActivity, ofs, dto.IsLocal, dto.CreatedByCurrentUser)
                    );
                case InspectionType.IBS:
                    return HandleInspection(
                        InspectionsTransaction.GetIBS(dto.Id, dto.IsLocal),
                        (ibs) => new InWaterOnBoardInspectionPage(dto.SubmitType, viewActivity, ibs, dto.IsLocal, dto.CreatedByCurrentUser)
                    );
                case InspectionType.IBP:
                    return HandleInspection(
                        InspectionsTransaction.GetIBP(dto.Id, dto.IsLocal),
                        (ibp) => new HarbourInspectionPage(dto.SubmitType, viewActivity, ibp, dto.IsLocal, dto.CreatedByCurrentUser)
                    );
                case InspectionType.ITB:
                    return HandleInspection(
                        InspectionsTransaction.GetITB(dto.Id, dto.IsLocal),
                        (itb) => new TranshipmentInspectionPage(dto.SubmitType, viewActivity, itb, dto.IsLocal, dto.CreatedByCurrentUser)
                    );
                case InspectionType.IVH:
                    return HandleInspection(
                        InspectionsTransaction.GetIVH(dto.Id, dto.IsLocal),
                        (ivh) => new VehicleInspectionPage(dto.SubmitType, viewActivity, ivh, dto.IsLocal, dto.CreatedByCurrentUser)
                    );
                case InspectionType.IFS:
                    return HandleInspection(
                        InspectionsTransaction.GetIFS(dto.Id, dto.IsLocal),
                        (ifs) => new FirstSaleInspectionPage(dto.SubmitType, viewActivity, ifs, dto.IsLocal, dto.CreatedByCurrentUser)
                    );
                case InspectionType.IAQ:
                    return HandleInspection(
                        InspectionsTransaction.GetIAQ(dto.Id, dto.IsLocal),
                        (iaq) => new AquacultureFarmInspectionPage(dto.SubmitType, viewActivity, iaq, dto.IsLocal, dto.CreatedByCurrentUser)
                    );
                case InspectionType.IFP:
                    return HandleInspection(
                        InspectionsTransaction.GetIFP(dto.Id, dto.IsLocal),
                        (ifp) => new FishermanInspectionPage(dto.SubmitType, viewActivity, ifp, dto.IsLocal, dto.CreatedByCurrentUser)
                    );
                case InspectionType.CWO:
                    return HandleInspection(
                        InspectionsTransaction.GetCWO(dto.Id, dto.IsLocal),
                        (cwo) => new InspectionWaterPage(dto.SubmitType, viewActivity, cwo, dto.IsLocal, dto.CreatedByCurrentUser)
                    );
                case InspectionType.IGM:
                    return HandleInspection(
                        InspectionsTransaction.GetIGM(dto.Id, dto.IsLocal),
                        (cmu) => new FishingGearInspectionPage(dto.SubmitType, viewActivity, cmu, dto.IsLocal, dto.CreatedByCurrentUser)
                    );
                default:
                    return Task.CompletedTask;
            }
        }

        private async Task HandleInspection<TDto>(Task<TDto> getDto, Func<TDto, Page> getPage)
            where TDto : class
        {
            TDto dto = await getDto;

            if (dto == null)
            {
                await TLLoadingHelper.HideFullLoadingScreen();
                await TLSnackbar.Show(
                    TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/WarningNoInternetTitle"],
                    App.GetResource<Color>("ErrorColor")
                );
                return;
            }

            await MainNavigator.Current.GoToPageAsync(getPage(dto));
        }

        private async Task<bool> MobileSign(InspectionDto dto)
        {
            List<SelectNomenclatureDto> fileTypes = DependencyService.Resolve<INomenclatureTransaction>().GetFileTypes();

            List<SignatureSaveModel> signatures = new List<SignatureSaveModel>(2)
            {
                new SignatureSaveModel
                {
                    Caption = TranslateExtension.Translator[nameof(GroupResourceEnum.GeneralInfo) + "/InspectorSignature"],
                    FileTypeId = fileTypes.Find(f => f.Code == Constants.InspectorSignature).Id
                },
            };

            if (dto.Type != InspectionType.OFS)
            {
                signatures.Add(new SignatureSaveModel
                {
                    Caption = TranslateExtension.Translator[nameof(GroupResourceEnum.GeneralInfo) + "/InspectedPersonSignature"],
                    FileTypeId = fileTypes.Find(f => f.Code == Constants.InspectedPersonSignature).Id
                });
            }

            bool finish = await TLDialogHelper.ShowDialog(new SaveInspectionView(signatures));

            if (!finish)
            {
                return false;
            }

            List<FileModel> files = new List<FileModel>(signatures.Count);

            for (int i = 0; i < signatures.Count; i++)
            {
                byte[] signatureBytes = signatures[i].Signature;

                string fileName = Guid.NewGuid().ToString("N") + ".png";
                string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
                long size = signatureBytes.Length;

                File.WriteAllBytes(filePath, signatureBytes);

                files.Add(new FileModel
                {
                    ContentType = "image/png",
                    FullPath = filePath,
                    Name = fileName,
                    Size = size,
                    FileTypeId = signatures[i].FileTypeId,
                    UploadedOn = DateTime.Now,
                    StoreOriginal = true,
                });
            }

            await TLLoadingHelper.ShowFullLoadingScreen();
            return await InspectionsTransaction.SignInspection(dto.Id, files);
        }
    }
}
