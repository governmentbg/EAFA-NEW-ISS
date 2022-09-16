using IARA.Mobile.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using TechnoLogica.Xamarin.ViewModels.Base;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.ViewModels.Base
{
    public abstract class ViewModel : TLBaseViewModel
    {
        protected INomenclatureTransaction NomenclaturesTransaction =>
            DependencyService.Resolve<INomenclatureTransaction>();
        protected IFishingTicketsTransaction FishingTicketsTransaction =>
            DependencyService.Resolve<IFishingTicketsTransaction>();
    }
}
