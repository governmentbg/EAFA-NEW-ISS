using System.Collections.Generic;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Shared.ResourceTranslator;
using IARA.Mobile.Shared.ViewModels.Base;
using IARA.Mobile.Shared.ViewModels.Intrerfaces;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Views
{
    public abstract class BasePage<TViewModel> : TLBasePage<TViewModel>, IBasePage
        where TViewModel : class, IPageViewModel
    {
        public static readonly BindableProperty PageInfoProperty =
            BindableProperty.Create(nameof(PageInfo), typeof(string), typeof(BasePage<>));

        protected BasePage()
        {
            Translator.Current.Add(ViewModel.GetPageResources(out GroupResourceEnum[] filtered));
            AddedResources = filtered;
        }

        public GroupResourceEnum[] AddedResources { get; }

        public string PageInfo
        {
            get => (string)GetValue(PageInfoProperty);
            set => SetValue(PageInfoProperty, value);
        }

        public List<View> TitleViews { get; } = new List<View>();

        public GroupResourceEnum[] GetPageIndexes()
        {
            return ViewModel.GetPageIndexes();
        }
    }
}
