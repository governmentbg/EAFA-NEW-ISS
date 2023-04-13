using System.Collections.Generic;
using System.Windows.Input;
using TechnoLogica.Xamarin.Helpers;

namespace IARA.Mobile.Shared.Helpers
{
    public class TLPagedCollection<T> : TLObservableCollection<T>, IPagedCollection
    {
        private int _page = 1;
        private int _pageCount;
        private ICommand _goToPage;

        public TLPagedCollection() { }

        public TLPagedCollection(List<T> list)
            : base(list) { }

        public TLPagedCollection(IEnumerable<T> collection)
            : base(collection) { }

        public int Page
        {
            get => _page;
            set
            {
                if (_page != value)
                {
                    _page = value;
                    OnPropertyChanged(nameof(Page));
                }
            }
        }

        public int PageCount
        {
            get => _pageCount;
            set
            {
                if (_pageCount != value)
                {
                    _pageCount = value;
                    OnPropertyChanged(nameof(PageCount));
                }
            }
        }

        public ICommand GoToPage
        {
            get => _goToPage;
            set
            {
                if (_goToPage != value)
                {
                    _goToPage = value;
                    OnPropertyChanged(nameof(PageCount));
                }
            }
        }
    }
}
