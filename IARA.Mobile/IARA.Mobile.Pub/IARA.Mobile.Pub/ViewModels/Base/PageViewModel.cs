using System.Collections.Generic;
using System.Linq;
using IARA.Mobile.Application.Interfaces.Transactions;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Shared.ResourceTranslator;
using IARA.Mobile.Shared.ViewModels.Base;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.ViewModels.Base
{
    public abstract class PageViewModel : BasePageViewModel
    {
        protected ITranslationTransaction TranslationTransaction =>
            DependencyService.Resolve<ITranslationTransaction>();

        protected IProfileTransaction ProfileTransaction =>
            DependencyService.Resolve<IProfileTransaction>();

        protected IScientificFishingTransaction ScientificFishingTransaction =>
            DependencyService.Resolve<IScientificFishingTransaction>();

        protected IFishingTicketsTransaction FishingTicketsTransaction =>
            DependencyService.Resolve<IFishingTicketsTransaction>();

        protected ICatchRecordsTransaction CatchRecordsTransaction =>
            DependencyService.Resolve<ICatchRecordsTransaction>();

        protected IReportViolationTransaction ReportViolationTransaction =>
            DependencyService.Resolve<IReportViolationTransaction>();

        protected INomenclatureTransaction NomenclaturesTransaction =>
            DependencyService.Resolve<INomenclatureTransaction>();

        protected IApplicationTransaction ApplicationTransaction =>
            DependencyService.Resolve<IApplicationTransaction>();

        protected IUserTransaction UserTransaction =>
            DependencyService.Resolve<IUserTransaction>();

        protected IPaymentTransaction PaymentTransaction =>
            DependencyService.Resolve<IPaymentTransaction>();

        protected IReportsTransaction ReportsTransaction =>
            DependencyService.Resolve<IReportsTransaction>();

        protected INewsTransaction NewsTransaction =>
            DependencyService.Resolve<INewsTransaction>();

        protected ICurrentUser CurrentUser =>
            DependencyService.Resolve<ICurrentUser>();

        protected IConnectivity CommonConnectivity =>
            DependencyService.Resolve<IConnectivity>();

        protected IAddressTransaction AddressTransaction =>
           DependencyService.Resolve<IAddressTransaction>();

        public override IReadOnlyDictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>> GetPageResources(out GroupResourceEnum[] filtered)
        {
            filtered = Translator.Current.Filter(GetPageIndexes()).ToArray();
            return TranslationTransaction.GetPagesTranslations(filtered);
        }
    }
}
