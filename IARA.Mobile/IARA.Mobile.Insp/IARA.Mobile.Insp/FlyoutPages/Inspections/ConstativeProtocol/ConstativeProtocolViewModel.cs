using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechnoLogica.Xamarin.Helpers;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.ConstativeProtocol
{
    public class ConstativeProtocolViewModel : PageViewModel
    {
        public InspectionConstativeProtocolDto Edit { get; set; }

        public TLObservableCollection<CPFishingGearModel> FishingGears { get; set; }
        public TLObservableCollection<CPCatchModel> Catches { get; set; }

        public void BeforeInit()
        {
            INomenclatureTransaction nomTransaction = NomenclaturesTransaction;

            List<SelectNomenclatureDto> fishingGearTypes = nomTransaction.GetFishingGears();
            List<SelectNomenclatureDto> fishes = nomTransaction.GetFishes();

            FishingGears = new TLObservableCollection<CPFishingGearModel>(Edit.FishingGears.ConvertAll(f => new CPFishingGearModel
            {
                Dto = f,
                FishingGear = fishingGearTypes.Find(s => s.Id == f.FishingGearId)
            }));

            Catches = new TLObservableCollection<CPCatchModel>(Edit.Catches.ConvertAll(f => new CPCatchModel
            {
                Dto = f,
                Fish = fishes.Find(s => s.Id == f.FishId)
            }));
        }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] { GroupResourceEnum.ConstativeProtocol };
        }

        public override Task Initialize(object sender)
        {
            return TLLoadingHelper.HideFullLoadingScreen();
        }
    }
}
