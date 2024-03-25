using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using TechnoLogica.Xamarin.Extensions;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Controls
{
    public class SaveButtonsLayout : StackLayout
    {
        public SaveButtonsLayout()
        {
            const string group = nameof(GroupResourceEnum.GeneralInfo);

            Orientation = StackOrientation.Horizontal;

            Margin = new Thickness(10, 10, 10, 0);
            Spacing = 15;
            HorizontalOptions = LayoutOptions.End;

            Children.Add(new Button
            {
                BackgroundColor = App.GetResource<Color>("Secondary"),
                Padding = new Thickness(15, 0),
            }.BindTranslation(Button.TextProperty, "Print", group)
                .Bind(Button.CommandProperty, nameof(InspectionPageViewModel.Print))
                .Bind(Button.IsVisibleProperty, nameof(InspectionPageViewModel.InspectionState), convert:
                (InspectionState state) =>
                {
                    return state == InspectionState.Submitted || state == InspectionState.Signed;
                }));

            Children.Add(new Button
            {
                BackgroundColor = App.GetResource<Color>("Secondary"),
                Padding = new Thickness(15, 0),
            }.BindTranslation(Button.TextProperty, "ReturnForEdit", group)
                .Bind(Button.CommandProperty, nameof(InspectionPageViewModel.ReturnForEdit))
                .Bind(Button.IsVisibleProperty, nameof(InspectionPageViewModel.InspectionState), convert: (InspectionState state) => state == InspectionState.Submitted));

            Children.Add(new Button
            {
                BackgroundColor = App.GetResource<Color>("Secondary"),
                Padding = new Thickness(15, 0),
            }.BindTranslation(Button.TextProperty, "SaveDraft", group)
                .Bind(Button.CommandProperty, nameof(InspectionPageViewModel.SaveDraft))
                .Bind(SaveButtonsLayout.IsVisibleProperty, nameof(InspectionPageViewModel.ActivityType), converter: App.GetResource<IValueConverter>("IsNotReview")));

            Children.Add(new Button
            {
                Padding = new Thickness(15, 0),
            }.BindTranslation(Button.TextProperty, "Finish", nameof(GroupResourceEnum.Common))
                .Bind(Button.CommandProperty, nameof(InspectionPageViewModel.Finish))
                .Bind(SaveButtonsLayout.IsVisibleProperty, nameof(InspectionPageViewModel.ActivityType), converter: App.GetResource<IValueConverter>("IsNotReview")));
        }
    }
}
