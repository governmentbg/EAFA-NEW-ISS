using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Shared.Views;
using Rg.Plugins.Popup.Animations;
using Rg.Plugins.Popup.Enums;
using Rg.Plugins.Popup.Services;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.Extensions;
using TechnoLogica.Xamarin.Helpers;
using Xamarin.CommunityToolkit.Effects;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.AddInspectionsPage
{
    public class AddInspectionsPopup : BasePopup<AddInspectionsViewModel>
    {
        private readonly ICommand command;

        public AddInspectionsPopup()
        {
            BackgroundColor = Color.Transparent;
            command = CommandBuilder.CreateFrom<string>(OnOptionChosen);

            Animation = new MoveAnimation
            {
                PositionIn = MoveAnimationOptions.Bottom,
                PositionOut = MoveAnimationOptions.Bottom,
                DurationIn = 400,
                DurationOut = 400,
                EasingIn = Easing.SinOut,
                EasingOut = Easing.SinIn
            };

            Content = new Frame
            {
                CornerRadius = 0,
                VerticalOptions = LayoutOptions.End,
                Content = new TLAutoGrid
                {
                    DefaultMinWidth = 500,
                    RowSpacing = 0,
                    ColumnSpacing = 0,
                    Children =
                    {
                        CreateInspectionTypeView(AddInspectionsViewModel.BoatOnOpenWater, FromFA(IconFont.Binoculars)),
                        CreateInspectionTypeView(AddInspectionsViewModel.InWaterOnBoardInspection, FromFA(IconFont.Ship)),
                        CreateInspectionTypeView(AddInspectionsViewModel.HarbourInspection, FromFA(IconFont.Anchor)),
                        CreateInspectionTypeView(AddInspectionsViewModel.TranshipmentInspection, FromFA(IconFont.RightLeft)),
                        CreateInspectionTypeView(AddInspectionsViewModel.VehicleInspection, FromFA(IconFont.TruckFast)),
                        CreateInspectionTypeView(AddInspectionsViewModel.FirstSaleInspection, FromFA(IconFont.Store)),
                        CreateInspectionTypeView(AddInspectionsViewModel.AquacultureFarmInspection, ImageExtension.Convert("fishbowl")),
                        CreateInspectionTypeView(AddInspectionsViewModel.FishermanInspection, FromFA(IconFont.Vest)),
                        CreateInspectionTypeView(AddInspectionsViewModel.InspectionWater, FromFA(IconFont.Water)),
                        CreateInspectionTypeView(AddInspectionsViewModel.FishingGearInspection, ImageExtension.Convert("hook"))
                    }
                }
            };
        }

        private Task OnOptionChosen(string option)
        {
            ViewModel.GoToAdd.Execute(option);
            return PopupNavigation.Instance.RemovePageAsync(this);
        }

        private ImageSource FromFA(string iconFont)
        {
            return new FontImageSource
            {
                FontFamily = "FA",
                Glyph = iconFont,
                Size = 100,
                Color = Color.White
            };
        }

        private View CreateInspectionTypeView(string parameter, ImageSource image)
        {
            const string group = nameof(GroupResourceEnum.AddInspections);

            const double height = 80d;
            const float cr = 40f;

            Frame frame = new Frame
            {
                Padding = 0,
                CornerRadius = cr,
                Margin = new Thickness(10, 5),
                HasShadow = false,
                HeightRequest = height,
                BackgroundColor = Color.Transparent,
                Content = new Grid
                {
                    ColumnSpacing = 0,
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = height - 10 },
                        new ColumnDefinition { Width = GridLength.Star }
                    },
                    Children =
                    {
                        new Frame
                        {
                            BackgroundColor = App.GetResource<Color>("Primary"),
                            HasShadow = false,
                            Margin = new Thickness(-height + 10, 0, 0, 0),
                            Padding = 0,
                            CornerRadius = cr,
                            Content = new Label
                            {
                                FontSize = 20,
                                LineBreakMode = LineBreakMode.WordWrap,
                                TextColor = Color.White,
                                VerticalOptions = LayoutOptions.Center,
                                Margin = new Thickness(height + 10, 0, 25, 0)
                            }.BindTranslation(Label.TextProperty, parameter, group)
                        }.Column(1),
                        new Frame
                        {
                            CornerRadius = cr,
                            HasShadow = false,
                            Padding = 0,
                            BackgroundColor = App.GetResource<Color>("PrimaryLight"),
                            Content = new Image
                            {
                                Source = image,
                                Aspect = Aspect.AspectFit,
                                HeightRequest = 45,
                                WidthRequest = 45,
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center,
                            }
                        }
                    }
                }
            };

            TouchEffect.SetNativeAnimation(frame, true);
            TouchEffect.SetCommand(frame, command);
            TouchEffect.SetCommandParameter(frame, parameter);

            return frame;
        }
    }
}
