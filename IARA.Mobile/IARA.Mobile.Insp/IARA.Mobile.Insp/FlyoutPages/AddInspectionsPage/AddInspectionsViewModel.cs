using System;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.FlyoutPages.Inspections.AquacultureFarmInspection;
using IARA.Mobile.Insp.FlyoutPages.Inspections.BoatOnOpenWater;
using IARA.Mobile.Insp.FlyoutPages.Inspections.FirstSaleInspection;
using IARA.Mobile.Insp.FlyoutPages.Inspections.FishermanInspection;
using IARA.Mobile.Insp.FlyoutPages.Inspections.FishingGearInspection;
using IARA.Mobile.Insp.FlyoutPages.Inspections.HarbourInspection;
using IARA.Mobile.Insp.FlyoutPages.Inspections.InspectionWater;
using IARA.Mobile.Insp.FlyoutPages.Inspections.InWaterOnBoard;
using IARA.Mobile.Insp.FlyoutPages.Inspections.TranshipmentInspection;
using IARA.Mobile.Insp.FlyoutPages.Inspections.VehicleInspection;
using IARA.Mobile.Shared.Menu;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.AddInspectionsPage
{
    public class AddInspectionsViewModel : MainPageViewModel
    {
        public const string BoatOnOpenWater = nameof(BoatOnOpenWater);
        public const string InWaterOnBoardInspection = nameof(InWaterOnBoardInspection);
        public const string HarbourInspection = nameof(HarbourInspection);
        public const string TranshipmentInspection = nameof(TranshipmentInspection);
        public const string VehicleInspection = nameof(VehicleInspection);
        public const string FirstSaleInspection = nameof(FirstSaleInspection);
        public const string AquacultureFarmInspection = nameof(AquacultureFarmInspection);
        public const string FishermanInspection = nameof(FishermanInspection);
        public const string InspectionWater = nameof(InspectionWater);
        public const string FishingGearInspection = nameof(FishingGearInspection);

        public AddInspectionsViewModel()
        {
            GoToAdd = CommandBuilder.CreateFrom<string>(OnGoToAdd);
        }

        public ICommand GoToAdd { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] { GroupResourceEnum.AddInspections };
        }

        public override Task Initialize(object sender)
        {
            return Task.CompletedTask;
        }

        public async Task OnGoToAdd(string type)
        {
            await TLLoadingHelper.ShowFullLoadingScreen();
            GlobalVariables.IsAddingInspection = true;
            await MainNavigator.Current.GoToPageAsync(GetPage(type));
        }

        private Page GetPage(string type)
        {
            switch (type)
            {
                case BoatOnOpenWater:
                    return new BoatOnOpenWaterPage();
                case InWaterOnBoardInspection:
                    return new InWaterOnBoardInspectionPage();
                case HarbourInspection:
                    return new HarbourInspectionPage();
                case TranshipmentInspection:
                    return new TranshipmentInspectionPage();
                case VehicleInspection:
                    return new VehicleInspectionPage();
                case FirstSaleInspection:
                    return new FirstSaleInspectionPage();
                case AquacultureFarmInspection:
                    return new AquacultureFarmInspectionPage();
                case FishermanInspection:
                    return new FishermanInspectionPage();
                case InspectionWater:
                    return new InspectionWaterPage();
                case FishingGearInspection:
                    return new FishingGearInspectionPage();
                default:
                    throw new ArgumentException($"Provided type '{type}' to {nameof(OnGoToAdd)} was invalid.");
            }
        }
    }
}
