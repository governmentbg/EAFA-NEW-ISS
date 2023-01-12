using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using IARA.Mobile.Application.Interfaces.Transactions;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Shared.ResourceTranslator;
using IARA.Mobile.Shared.ViewModels.Base;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Base
{
    public abstract class PageViewModel : BasePageViewModel
    {
        protected ITranslationTransaction TranslationTransaction =>
            DependencyService.Resolve<ITranslationTransaction>();

        protected IProfileTransaction ProfileTransaction =>
            DependencyService.Resolve<IProfileTransaction>();

        protected INomenclatureTransaction NomenclaturesTransaction =>
            DependencyService.Resolve<INomenclatureTransaction>();

        protected IInspectionsTransaction InspectionsTransaction =>
            DependencyService.Resolve<IInspectionsTransaction>();

        protected IReportsTransaction ReportsTransaction =>
            DependencyService.Resolve<IReportsTransaction>();

        protected ICurrentUser CurrentUser =>
            DependencyService.Resolve<ICurrentUser>();

        protected IMapper Mapper =>
            DependencyService.Resolve<IMapper>();

        protected IDownloader Downloader =>
            DependencyService.Resolve<IDownloader>();

        public override IReadOnlyDictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>> GetPageResources(out GroupResourceEnum[] filtered)
        {
            filtered = Translator.Current.Filter(GetPageIndexes()).ToArray();
            return TranslationTransaction.GetPagesTranslations(filtered);
        }
    }
}
