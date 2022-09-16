using IARA.Mobile.Domain.Enums;
using Rg.Plugins.Popup.Pages;
using System;
using System.Threading.Tasks;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.Extensions;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace IARA.Mobile.Shared.Popups
{
    public class SelectLocationOnMapPopup : PopupPage, IPopupSelect<MapClickedEventArgs>
    {
        private readonly Map map;

        public SelectLocationOnMapPopup(Xamarin.Essentials.Location location)
        {
            map = new Map
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
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(location.Latitude, location.Longitude), Distance.FromKilometers(.5)));

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

            Content = new Frame
            {
                Padding = 0,
                HasShadow = false,
                CornerRadius = 0,
                Content = new TLFillLayout
                {
                    Children =
                    {
                        map,
                        new TLWrapLayout
                        {
                            ColumnSpacing = 5,
                            RowSpacing = 5,
                            Margin = new Thickness(5, 0, 50, 5),
                            VerticalOptions = LayoutOptions.End,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            Children =
                            {
                                new Button
                                {
                                    BackgroundColor = (Color)Xamarin.Forms.Application.Current.Resources["ErrorColor"],
                                    Command = CommandBuilder.CreateFrom(OnCancelButtonClicked)
                                }.BindTranslation(Button.TextProperty, "Cancel", group),
                                new Button
                                {
                                    Command = CommandBuilder.CreateFrom(OnChooseButtonClicked)
                                }.BindTranslation(Button.TextProperty, "Choose", group),
                                new Button
                                {
                                    BackgroundColor = Color.Green,
                                    Command = CommandBuilder.CreateFrom(OnChosenLocationButtonClicked)
                                }.BindTranslation(Button.TextProperty, "CurrentLocation", group),
                            }
                        }
                    }
                }
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
            Xamarin.Essentials.Location geolocation = await Xamarin.Essentials.Geolocation.GetLocationAsync();
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
