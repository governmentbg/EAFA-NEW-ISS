using System.Collections.Generic;
using IARA.Mobile.Domain.Enums;
using TechnoLogica.Xamarin.ViewModels.Interfaces;

namespace IARA.Mobile.Shared.ViewModels.Base
{
    public interface IPageViewModel : ITLPageViewModel
    {
        /// <summary>
        /// The resources for the current page
        /// </summary>
        IReadOnlyDictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>> GetPageResources(out GroupResourceEnum[] filtered);

        /// <summary>
        /// Returns the <see cref="GroupResourceEnum"/> for the current page
        /// </summary>
        GroupResourceEnum[] GetPageIndexes();
    }
}
