using System.Collections;
using IARA.Mobile.Insp.DataTemplateSelectors;
using TechnoLogica.Xamarin.Controls.Base;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Controls
{
    public class TLGeneratedToggles : TLValidatableView<IList>
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(TLGeneratedToggles));

        public TLGeneratedToggles()
        {
            Content = new StackLayout()
                .Bind(BindableLayout.ItemsSourceProperty, ItemsSourceProperty.PropertyName, source: this)
                .Bind(StackLayout.IsEnabledProperty, IsEnabledProperty.PropertyName, source: this);
            BindableLayout.SetItemTemplateSelector(Content, new ToggleTemplateSelector());

            ValidStateValueProperty = ItemsSourceProperty;
        }

        public IList ItemsSource
        {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
    }
}
