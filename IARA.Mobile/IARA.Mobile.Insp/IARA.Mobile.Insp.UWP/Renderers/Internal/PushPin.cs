using System;
using System.ComponentModel;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Xamarin.Forms.Maps;

namespace IARA.Mobile.Insp.UWP.Renderers.Internal
{
    internal class PushPin : ContentControl
    {
        private readonly Pin _pin;

        internal PushPin(Pin pin)
        {
            if (pin == null)
            {
                throw new ArgumentNullException();
            }

            ContentTemplate = Windows.UI.Xaml.Application.Current.Resources["PushPinTemplate"] as Windows.UI.Xaml.DataTemplate;
            DataContext = Content = _pin = pin;

            UpdateLocation();

            Loaded += PushPinLoaded;
            Unloaded += PushPinUnloaded;
            Tapped += PushPinTapped;
        }

        private void PushPinLoaded(object sender, RoutedEventArgs e)
        {
            _pin.PropertyChanged += PinPropertyChanged;
        }

        private void PushPinUnloaded(object sender, RoutedEventArgs e)
        {
            _pin.PropertyChanged -= PinPropertyChanged;
            Tapped -= PushPinTapped;
        }

        private void PinPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Pin.PositionProperty.PropertyName)
            {
                UpdateLocation();
            }
        }

        private void PushPinTapped(object sender, TappedRoutedEventArgs e)
        {
#pragma warning disable CS0618
            _pin.SendTap();
#pragma warning restore CS0618
            _pin.SendMarkerClick();
        }

        private void UpdateLocation()
        {
            Windows.Foundation.Point anchor = new Windows.Foundation.Point(0.65, 1);
            Geopoint location = new Geopoint(new BasicGeoposition
            {
                Latitude = _pin.Position.Latitude,
                Longitude = _pin.Position.Longitude
            });
            MapControl.SetLocation(this, location);
            MapControl.SetNormalizedAnchorPoint(this, anchor);
        }
    }
}
