using System;
using System.Collections.Generic;
using System.Windows.Input;
using IARA.Mobile.Application;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.ViewModels.Models
{
    public class ValidStateInfiniteSelect<T> : ValidStateSelect<T>
        where T : class
    {
        private bool _wasChanged;

        public ValidStateInfiniteSelect(List<TLValidator> validations, List<string> groups, IViewModelValidation validation)
            : base(validations, groups, validation)
        {
            Closed = CommandBuilder.CreateFrom(OnClosed);
            EndReached = CommandBuilder.CreateFrom(OnEndReached);
            StoppedTyping = CommandBuilder.CreateFrom<string>(OnStoppedTyping);
        }

        public Func<int, int, string, List<T>> GetMore { get; set; }

        public TLObservableCollection<T> ItemsSource { get; set; }
        public int Page { get; private set; } = 0;
        public int PageSize { get; set; } = CommonGlobalVariables.PullItemsCount;
        public string Search { get; private set; } = string.Empty;

        public ICommand Closed { get; }
        public ICommand EndReached { get; }
        public ICommand StoppedTyping { get; }

        private void OnClosed()
        {
            ChackGetMore();

            if (_wasChanged)
            {
                Page = 0;
                Search = null;
                ItemsSource.ReplaceRange(GetMore(Page, PageSize, Search));
            }

            _wasChanged = false;
        }

        private void OnEndReached()
        {
            ChackGetMore();

            if (ItemsSource.Count >= PageSize)
            {
                Page++;
                ItemsSource.AddRange(GetMore(Page, PageSize, Search));
                _wasChanged = true;
            }
        }

        private void OnStoppedTyping(string search)
        {
            ChackGetMore();

            if (!string.IsNullOrEmpty(Search) || !string.IsNullOrEmpty(search))
            {
                Page = 0;
                Search = search;
                ItemsSource.ReplaceRange(GetMore(Page, PageSize, Search?.Length == 0 ? null : Search));
                _wasChanged = true;
            }
        }

        private void ChackGetMore()
        {
            if (GetMore == null)
            {
                throw new ArgumentNullException(nameof(GetMore));
            }
        }
    }
}
