using System.Windows.Input;
using TechnoLogica.Xamarin.ViewModels.Interfaces;

namespace IARA.Mobile.Shared.Helpers
{
    public interface IPagedCollection : IObservableList
    {
        int Page { get; set; }
        int PageCount { get; set; }

        ICommand GoToPage { get; }
    }
}
