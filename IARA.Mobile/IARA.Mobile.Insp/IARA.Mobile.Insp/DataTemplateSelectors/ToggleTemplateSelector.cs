using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.Extensions;
using Xamarin.CommunityToolkit.Converters;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.DataTemplateSelectors
{
    public class ToggleTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            ToggleViewModel toggle = (ToggleViewModel)item;

            if (toggle.HasDescription)
            {
                return new DataTemplate(() =>
                {
                    TLMultiToggleView multiToggle = new TLMultiToggleView
                    {
                        Buttons = toggle.Type == ToggleTypeEnum.Bool
                            ? InspectionTogglesHelper.YesNoMultiToggles
                            : InspectionTogglesHelper.YesNoNotApplicableMultiToggles,
                        Text = toggle.Text,
                        ValidState = toggle.Value,
                    }.Bind(TLMultiToggleView.IsEnabledProperty, StackLayout.IsEnabledProperty.PropertyName, source: container);

                    IValueConverter equalConveter = new FuncConverter<string, bool>((string selected) => selected == nameof(CheckTypeEnum.Y));

                    TLEntry entry = new TLEntry
                    {
                        Title = toggle.DescriptionLabel,
                        ValidState = toggle.Description,
                    }.Bind(TLEntry.IsEnabledProperty, new MultiBinding
                    {
                        Bindings =
                        {
                            new Binding(TLMultiToggleView.SelectedValueProperty.PropertyName, converter: equalConveter, source: multiToggle),
                            new Binding(StackLayout.IsEnabledProperty.PropertyName, source: container),
                        },
                        Converter = new VariableMultiValueConverter
                        {
                            ConditionType = MultiBindingCondition.All
                        }
                    });

                    return new StackLayout
                    {
                        Children =
                        {
                            multiToggle,
                            entry,
                        }
                    };
                });
            }
            else
            {
                return new DataTemplate(() => new TLMultiToggleView
                {
                    Buttons = toggle.Type == ToggleTypeEnum.Bool
                        ? InspectionTogglesHelper.YesNoMultiToggles
                        : InspectionTogglesHelper.YesNoNotApplicableMultiToggles,
                    Text = toggle.Text,
                    ValidState = toggle.Value,
                }.Bind(TLMultiToggleView.IsEnabledProperty, StackLayout.IsEnabledProperty.PropertyName, source: container));
            }
        }
    }
}
