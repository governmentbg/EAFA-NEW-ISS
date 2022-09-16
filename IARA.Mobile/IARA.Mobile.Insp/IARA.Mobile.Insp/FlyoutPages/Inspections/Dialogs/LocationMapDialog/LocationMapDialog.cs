using IARA.Mobile.Domain.Enums;
using Rg.Plugins.Popup.Pages;
using System;
using System.Threading.Tasks;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.Extensions;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.LocationMapDialog
{
    public class LocationMapDialog : PopupPage, IPopupSelect<MapClickedEventArgs>
    {
        private readonly Map map;

        public LocationMapDialog(Xamarin.Essentials.Location location)
        {
            map = new Map(MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), Distance.FromKilometers(.5)))
            {
                Pins =
                {
                    new Pin
                    {
                        Position = new Position(location.Latitude, location.Longitude),
                        Label = string.Empty
                    }.BindTranslation(Pin.LabelProperty, "ChosenLocation", nameof(GroupResourceEnum.Common))
                }
            };

            map.MapClicked += (_, e) =>
            {
                map.Pins.Clear();
                map.Pins.Add(new Pin
                {
                    Position = e.Position,
                    Label = string.Empty
                }.BindTranslation(Pin.LabelProperty, "ChosenLocation", nameof(GroupResourceEnum.Common)));
            };

            const string group = nameof(GroupResourceEnum.Common);

            TLFillLayout layout = new TLFillLayout
            {
                Children =
                {
                    map
                }
            };

            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                {
                    layout.Children.Add(new ImageButton
                    {
                        BorderColor = Color.FromHex("#d7d7d8"),
                        BorderWidth = 1,
                        Opacity = .85,
                        Margin = new Thickness(0, 0, 10, 100),
                        CornerRadius = 3,
                        BackgroundColor = Color.White,
                        Source = new FontImageSource
                        {
                            FontFamily = "Material",
                            Color = Color.FromHex("#4b4b4b"),
                            Glyph = "\ue55c"
                        },
                        Command = CommandBuilder.CreateFrom(OnChosenLocationButtonClicked),
                        HorizontalOptions = LayoutOptions.End,
                        VerticalOptions = LayoutOptions.End,
                    });
                    layout.Children.Add(new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Margin = new Thickness(5, 0, 70, 5),
                        VerticalOptions = LayoutOptions.End,
                        HorizontalOptions = LayoutOptions.End,
                        Children =
                        {
                            new Button
                            {
                                BackgroundColor = App.GetResource<Color>("Secondary"),
                                Command = CommandBuilder.CreateFrom(OnCancelButtonClicked)
                            }.BindTranslation(Button.TextProperty, "Cancel", group),
                            new Button
                            {
                                Command = CommandBuilder.CreateFrom(OnChooseButtonClicked)
                            }.BindTranslation(Button.TextProperty, "Choose", group),
                        }
                    });
                    break;
                }
                case Device.UWP:
                {
                    layout.Children.Add(new ImageButton
                    {
                        BorderColor = Color.FromHex("#d7d7d6"),
                        BorderWidth = 2,
                        Margin = new Thickness(0, 185, 7, 0),
                        CornerRadius = 18,
                        HeightRequest = 36,
                        WidthRequest = 36,
                        BackgroundColor = Color.FromHex("#f2f2f2"),
                        Padding = new Thickness(5, 5, 2.5, 5),
                        Source = new FontImageSource
                        {
                            FontFamily = "Material",
                            Color = Color.FromHex("#4b4b4b"),
                            Glyph = "\ue55c",
                            Size = 30,
                        },
                        Command = CommandBuilder.CreateFrom(OnChosenLocationButtonClicked),
                        HorizontalOptions = LayoutOptions.End,
                        VerticalOptions = LayoutOptions.Center,
                    });
                    layout.Children.Add(new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Margin = new Thickness(5, 0, 7, 20),
                        VerticalOptions = LayoutOptions.End,
                        HorizontalOptions = LayoutOptions.End,
                        Children =
                        {
                            new Button
                            {
                                BackgroundColor = App.GetResource<Color>("Secondary"),
                                Command = CommandBuilder.CreateFrom(OnCancelButtonClicked)
                            }.BindTranslation(Button.TextProperty, "Cancel", group),
                            new Button
                            {
                                Command = CommandBuilder.CreateFrom(OnChooseButtonClicked)
                            }.BindTranslation(Button.TextProperty, "Choose", group),
                        }
                    });
                    break;
                }
                default:
                    throw new NotImplementedException("Map cannot be opened on the specified platform.");
            }

            Content = new Frame
            {
                Padding = 0,
                HasShadow = false,
                CornerRadius = 0,
                Content = layout
            };
        }

        public event EventHandler<MapClickedEventArgs> Selected;
        public event EventHandler Canceled;

        private void OnCancelButtonClicked()
        {
            Canceled?.Invoke(this, EventArgs.Empty);
        }

        private void OnChooseButtonClicked()
        {
            Selected?.Invoke(this, new MapClickedEventArgs(map.Pins[0].Position));
        }

        private async Task OnChosenLocationButtonClicked()
        {
            await TLLoadingHelper.ShowFullLoadingScreen();

            Xamarin.Essentials.Location geolocation = await Xamarin.Essentials.Geolocation.GetLocationAsync(new Xamarin.Essentials.GeolocationRequest { Timeout = TimeSpan.FromSeconds(5) });
            map.MoveToRegion(MapSpan.FromCenterAndRadius(
                new Position(geolocation.Latitude, geolocation.Longitude),
                Distance.FromKilometers(.5)
            ));
            map.Pins.Clear();
            map.Pins.Add(new Pin
            {
                Position = new Position(geolocation.Latitude, geolocation.Longitude),
                Label = string.Empty
            }.BindTranslation(Pin.LabelProperty, "ChosenLocation", nameof(GroupResourceEnum.Common)));

            await TLLoadingHelper.HideFullLoadingScreen();
        }

        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            return false;
        }

        protected override bool OnBackgroundClicked()
        {
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return false;
        }
    }
}
