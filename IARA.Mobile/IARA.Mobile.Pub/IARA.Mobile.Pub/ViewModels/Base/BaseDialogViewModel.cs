using IARA.Mobile.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using TechnoLogica.Xamarin.ViewModels.Base;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.ViewModels.Base
{
    public abstract class BaseDialogViewModel : TLBaseDialogViewModel
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
    }

    public abstract class BaseDialogViewModel<TResult> : TLBaseDialogViewModel<TResult>
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
    }
}
