using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.ShipPickerDialog
{
    public class ShipPickerDialog : TLBaseDialog<ShipPickerDialogViewModel, (string, VesselDuringInspectionDto)>
    {
        public ShipPickerDialog(InspectionPageViewModel inspection, VesselDuringInspectionDto ship)
        {
            ViewModel.OnInit(inspection);
            ViewModel.SelectedShip = ship;

            IconColor = Color.White;
            TitleColor = Color.White;
            TitleBackgroundColor = App.GetResource<Color>("Primary");
            Title = TranslateExtension.Translator[nameof(GroupResourceEnum.CatchInspection) + "/PickShipDialog"];
            BackgroundColor = Color.White;

            Padding = 10;
            Content = new StackLayout
            {
                Children =
                {
                    new InspectedShipDataView
                    {
                        ShipInRegisterLabel = TranslateExtension.Translator[nameof(GroupResourceEnum.InspectedShipData) + "/ShipInRegister"],
                    }.Bind(InspectedShipDataView.BindingContextProperty, nameof(ViewModel.Ship), source: ViewModel),
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.End,
                        Children =
                        {
                            new Button
                            {
                                BackgroundColor = App.GetResource<Color>("Secondary"),
                                Text = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Cancel"],
                                Command = ViewModel.Cancel
                            },
                            new Button
                            {
                                BackgroundColor = App.GetResource<Color>("Primary"),
                                Text = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Okay"],
                                Command = ViewModel.Pick
                            }
                        }
                    }
                }
            };
        }
    }
}
