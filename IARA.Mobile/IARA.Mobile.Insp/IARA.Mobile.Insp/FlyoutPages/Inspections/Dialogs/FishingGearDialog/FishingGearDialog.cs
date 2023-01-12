using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using IARA.Mobile.Shared.Views;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.Extensions;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FishingGearDialog
{
    public class FishingGearDialog : TLBaseDialog<FishingGearDialogViewModel, FishingGearModel>
    {
        public FishingGearDialog(InspectionPageViewModel inspection, ViewActivityType dialogType, bool hasPingers, FishingGearModel dto = null)
        {
            ViewModel.Inspection = inspection;
            ViewModel.Edit = dto;
            ViewModel.DialogType = dialogType;
            ViewModel.HasPingers = hasPingers;
            ViewModel.BeforeInit();

            this.BindTranslation(TitleProperty, "Title", nameof(GroupResourceEnum.FishingGear));
            TitleBackgroundColor = App.GetResource<Color>("Primary");
            TitleColor = Color.White;
            IconColor = Color.White;
            BackgroundColor = Color.White;

            StackLayout stack;

            if (dto?.IsAddedByInspector != false)
            {
                stack = new StackLayout
                {
                    Spacing = 5,
                    Padding = 10,
                    Children =
                    {
                        new FishingGearView(ViewModel.InspectedFishingGear)
                            .BindTranslation(SectionView.TextProperty, "InspectedFishingGear", nameof(GroupResourceEnum.FishingGear)),
                        new Button
                        {
                            Command = ViewModel.Save,
                            HorizontalOptions = LayoutOptions.End,
                            IsVisible = dialogType != ViewActivityType.Review,
                        }.BindTranslation(Button.TextProperty, "Save", nameof(GroupResourceEnum.Common))
                    }
                };
            }
            else
            {
                stack = new StackLayout
                {
                    Spacing = 5,
                    Padding = 10,
                    Children =
                    {
                        new FishingGearView(ViewModel.PermittedFishingGear)
                            .BindTranslation(FishingGearView.TextProperty, "FishingGear", nameof(GroupResourceEnum.FishingGear)),
                        new TLMultiToggleView
                        {
                            ValidState = ViewModel.Corresponds,
                            Buttons = InspectionTogglesHelper.YesNoUnavailableMultiToggles,
                            IsEnabled = dialogType != ViewActivityType.Review,
                        }.BindTranslation(TLMultiToggleView.TextProperty, "CorrespondsToRegistered", nameof(GroupResourceEnum.FishingGear)),
                        new TLMultiToggleView
                        {
                            ValidState = ViewModel.HasAttachedAppliances,
                            Buttons = InspectionTogglesHelper.YesNoMultiToggles,
                            IsEnabled = dialogType != ViewActivityType.Review,
                        }.BindTranslation(TLMultiToggleView.TextProperty, "HasAttachedAppliances", nameof(GroupResourceEnum.FishingGear)),
                        new FishingGearView(ViewModel.InspectedFishingGear)
                            .BindTranslation(SectionView.TextProperty, "InspectedFishingGear", nameof(GroupResourceEnum.FishingGear))
                            .Bind(SectionView.IsVisibleProperty, "Value", converter: App.GetResource<IValueConverter>("Equal"), converterParameter: nameof(CheckTypeEnum.N), source: ViewModel.Corresponds),
                        new Button
                        {
                            Command = ViewModel.Save,
                            HorizontalOptions = LayoutOptions.End,
                            IsVisible = dialogType != ViewActivityType.Review,
                        }.BindTranslation(Button.TextProperty, "Save", nameof(GroupResourceEnum.Common))
                    }
                };
            }

            Content = new ScrollView
            {
                Content = stack
            };
        }
    }
}
