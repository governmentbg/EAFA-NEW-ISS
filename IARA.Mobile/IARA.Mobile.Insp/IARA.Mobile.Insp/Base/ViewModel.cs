using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using TechnoLogica.Xamarin.ViewModels.Base;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Base
{
    public abstract class ViewModel : TLBaseViewModel
    {
        protected INomenclatureTransaction NomenclaturesTransaction =>
            DependencyService.Resolve<INomenclatureTransaction>();

        protected IInspectionsTransaction InspectionsTransaction =>
            DependencyService.Resolve<IInspectionsTransaction>();

        protected ICurrentUser CurrentUser =>
            DependencyService.Resolve<ICurrentUser>();
    }
}
