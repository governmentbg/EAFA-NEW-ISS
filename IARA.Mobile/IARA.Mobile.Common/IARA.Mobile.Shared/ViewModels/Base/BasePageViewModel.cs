using System.Collections.Generic;
using IARA.Mobile.Domain.Enums;
using TechnoLogica.Xamarin.ViewModels.Base;

namespace IARA.Mobile.Shared.ViewModels.Base
{
    public abstract class BasePageViewModel : TLBasePageViewModel, IPageViewModel
    {
        public abstract GroupResourceEnum[] GetPageIndexes();

        public abstract IReadOnlyDictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>> GetPageResources(out GroupResourceEnum[] filtered);
    }
}
