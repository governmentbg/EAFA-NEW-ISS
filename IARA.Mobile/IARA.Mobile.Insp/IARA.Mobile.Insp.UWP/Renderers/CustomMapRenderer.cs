using IARA.Mobile.Insp.UWP.Renderers.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Platform.UWP;
using WEllipse = Windows.UI.Xaml.Shapes.Ellipse;

//[assembly: ExportRenderer(typeof(Map), typeof(MapRenderer))]

namespace IARA.Mobile.Insp.UWP.Renderers
{
    public class CustomMapRenderer : ViewRenderer<Map, MapControl>
    {
        protected override async void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                Map mapModel = e.OldElement;
                MessagingCenter.Unsubscribe<Map, MapSpan>(this, "MapMoveToRegion");
                ((ObservableCollection<Pin>)mapModel.Pins).CollectionChanged -= OnPinCollectionChanged;
                ((ObservableCollection<Xamarin.Forms.Maps.MapElement>)mapModel.MapElements).CollectionChanged -= OnMapElementCollectionChanged;
            }

            if (e.NewElement != null)
            {
                Map mapModel = e.NewElement;

                if (Control == null)
                {
                    SetNativeControl(new MapControl());
                    Control.MapServiceToken = "f63S9Xuo4vlTekpKKsys~E1ExlsOLaaukCEMWJju7-Q~AhInzM8O_uVccN0OT8yjzyAQpVudxvwzA8x4n42pWv1IyamjnzBINUL7rz4X-2Ap";
                    Control.ZoomLevelChanged += async (_, __) => await UpdateVisibleRegion();
                    Control.CenterChanged += async (_, __) => await UpdateVisibleRegion();
                    Control.MapTapped += OnMapTapped;
                    Control.LayoutUpdated += OnLayoutUpdated;
                }

                MessagingCenter.Subscribe<Map, MapSpan>(this, "MapMoveToRegion", async (_, a) => await MoveToRegion(a), mapModel);

                UpdateTrafficEnabled();
                UpdateMapType();
                UpdateHasScrollEnabled();
                UpdateHasZoomEnabled();

                ((ObservableCollection<Pin>)mapModel.Pins).CollectionChanged += OnPinCollectionChanged;
                if (mapModel.Pins.Count > 0)
                {
                    LoadPins();
                } ((ObservableCollection<Xamarin.Forms.Maps.MapElement>)mapModel.MapElements).CollectionChanged += OnMapElementCollectionChanged;
                if (mapModel.MapElements.Count > 0)
                {
                    LoadMapElements(mapModel.MapElements);
                }

                if (Control == null)
                {
                    return;
                }

                await Control.Dispatcher.RunIdleAsync(async (_) => await MoveToRegion(mapModel.LastMoveToRegion, MapAnimationKind.None));
                await UpdateIsShowingUser();
            }
        }

        private bool _isRegionUpdatePending;

        private async void OnLayoutUpdated(object sender, object e)
        {
            if (_isRegionUpdatePending)
            {
                // _isRegionUpdatePending is set to false when the update is successfull
                await MoveToRegion(Element.LastMoveToRegion, MapAnimationKind.None);
            }
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Map.MapTypeProperty.PropertyName)
            {
                UpdateMapType();
            }
            else if (e.PropertyName == Map.IsShowingUserProperty.PropertyName)
            {
                await UpdateIsShowingUser();
            }
            else if (e.PropertyName == Map.HasScrollEnabledProperty.PropertyName)
            {
                UpdateHasScrollEnabled();
            }
            else if (e.PropertyName == Map.HasZoomEnabledProperty.PropertyName)
            {
                UpdateHasZoomEnabled();
            }
            else if (e.PropertyName == Map.TrafficEnabledProperty.PropertyName)
            {
                UpdateTrafficEnabled();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;

                _timer?.Stop();
                _timer = null;

                MessagingCenter.Unsubscribe<Map, MapSpan>(this, "MapMoveToRegion");

                if (Element != null)
                {
                    ((ObservableCollection<Pin>)Element.Pins).CollectionChanged -= OnPinCollectionChanged;
                    ((ObservableCollection<Xamarin.Forms.Maps.MapElement>)Element.MapElements).CollectionChanged -= OnMapElementCollectionChanged;
                }

                if (Control != null)
                {
                    Control.LayoutUpdated -= OnLayoutUpdated;
                    Control.MapTapped -= OnMapTapped;
                }
            }

            base.Dispose(disposing);
        }

        private bool _disposed;
        private WEllipse _userPositionCircle;
        private DispatcherTimer _timer;

        private void OnPinCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Device.IsInvokeRequired)
            {
                Device.BeginInvokeOnMainThread(() => PinCollectionChanged(e));
            }
            else
            {
                PinCollectionChanged(e);
            }
        }

        private void PinCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Pin pin in e.NewItems)
                    {
                        LoadPin(pin);
                    }

                    break;
                case NotifyCollectionChangedAction.Move:
                    // no matter
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (Pin pin in e.OldItems)
                    {
                        RemovePin(pin);
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (Pin pin in e.OldItems)
                    {
                        RemovePin(pin);
                    }

                    foreach (Pin pin in e.NewItems)
                    {
                        LoadPin(pin);
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    ClearPins();
                    LoadPins();
                    break;
            }
        }

        private void LoadPins()
        {
            foreach (Pin pin in Element.Pins)
            {
                LoadPin(pin);
            }
        }

        private void ClearPins()
        {
            Control.Children.Clear();
#pragma warning disable 4014 // don't wanna block UI thread
            UpdateIsShowingUser();
#pragma warning restore
        }

        private void RemovePin(Pin pinToRemove)
        {
            DependencyObject pushPin = Control.Children.FirstOrDefault(c =>
            {
                PushPin pin = (c as PushPin);
                return (pin != null && pin.DataContext.Equals(pinToRemove));
            });

            if (pushPin != null)
            {
                Control.Children.Remove(pushPin);
            }
        }

        private void LoadPin(Pin pin)
        {
            Control.Children.Add(new PushPin(pin));
        }

        private void OnMapElementCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Device.IsInvokeRequired)
            {
                Device.BeginInvokeOnMainThread(() => MapElementCollectionChanged(e));
            }
            else
            {
                MapElementCollectionChanged(e);
            }
        }

        private void MapElementCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    LoadMapElements(e.NewItems.Cast<Xamarin.Forms.Maps.MapElement>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveMapElements(e.OldItems.Cast<Xamarin.Forms.Maps.MapElement>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    RemoveMapElements(e.OldItems.Cast<Xamarin.Forms.Maps.MapElement>());
                    LoadMapElements(e.NewItems.Cast<Xamarin.Forms.Maps.MapElement>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Control.MapElements.Clear();
                    LoadMapElements(Element.MapElements);
                    break;
            }
        }

        private void LoadMapElements(IEnumerable<Xamarin.Forms.Maps.MapElement> mapElements)
        {
            foreach (Xamarin.Forms.Maps.MapElement formsMapElement in mapElements)
            {
                Windows.UI.Xaml.Controls.Maps.MapElement nativeMapElement = null;

                switch (formsMapElement)
                {
                    case Polyline polyline:
                        nativeMapElement = LoadPolyline(polyline);
                        break;
                    case Polygon polygon:
                        nativeMapElement = LoadPolygon(polygon);
                        break;
                    case Circle circle:
                        nativeMapElement = LoadCircle(circle);
                        break;
                }

                Control.MapElements.Add(nativeMapElement);

                formsMapElement.PropertyChanged += MapElementPropertyChanged;
                formsMapElement.MapElementId = nativeMapElement;
            }
        }

        private void RemoveMapElements(IEnumerable<Xamarin.Forms.Maps.MapElement> mapElements)
        {
            foreach (Xamarin.Forms.Maps.MapElement mapElement in mapElements)
            {
                mapElement.PropertyChanged -= MapElementPropertyChanged;
                Control.MapElements.Remove((Windows.UI.Xaml.Controls.Maps.MapElement)mapElement.MapElementId);
            }
        }

        private void MapElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (sender)
            {
                case Polyline polyline:
                    OnPolylinePropertyChanged(polyline, e);
                    break;
                case Polygon polygon:
                    OnPolygonPropertyChanged(polygon, e);
                    break;
                case Circle circle:
                    OnCirclePropertyChanged(circle, e);
                    break;
            }
        }

        protected Geopath PositionsToGeopath(IList<Position> positions)
        {
            // Geopath constructor throws an exception on an empty list
            if (positions.Any())
            {
                return new Geopath(positions.Select(p => new BasicGeoposition
                {
                    Latitude = p.Latitude,
                    Longitude = p.Longitude
                }));
            }
            else
            {
                return new Geopath(new[]
                {
                    new BasicGeoposition(),
                });
            }
        }

        #region Polylines

        protected virtual MapPolyline LoadPolyline(Polyline polyline)
        {
            return new MapPolyline()
            {
                Path = PositionsToGeopath(polyline.Geopath),
                StrokeColor = polyline.StrokeColor.IsDefault ? Colors.Black : polyline.StrokeColor.ToWindowsColor(),
                StrokeThickness = polyline.StrokeWidth
            };
        }

        private void OnPolylinePropertyChanged(Polyline polyline, PropertyChangedEventArgs e)
        {
            MapPolyline mapPolyline = (MapPolyline)polyline.MapElementId;

            if (mapPolyline == null)
            {
                return;
            }

            if (e.PropertyName == Polyline.StrokeColorProperty.PropertyName)
            {
                mapPolyline.StrokeColor = polyline.StrokeColor.IsDefault ? Colors.Black : polyline.StrokeColor.ToWindowsColor();
            }
            else if (e.PropertyName == Polyline.StrokeWidthProperty.PropertyName)
            {
                mapPolyline.StrokeThickness = polyline.StrokeWidth;
            }
            else if (e.PropertyName == nameof(Polyline.Geopath))
            {
                mapPolyline.Path = PositionsToGeopath(polyline.Geopath);
            }
        }

        #endregion

        #region Polygons

        protected virtual MapPolygon LoadPolygon(Polygon polygon)
        {
            return new MapPolygon()
            {
                Path = PositionsToGeopath(polygon.Geopath),
                StrokeColor = polygon.StrokeColor.IsDefault ? Colors.Black : polygon.StrokeColor.ToWindowsColor(),
                StrokeThickness = polygon.StrokeWidth,
                FillColor = polygon.FillColor.ToWindowsColor()
            };
        }

        private void OnPolygonPropertyChanged(Polygon polygon, PropertyChangedEventArgs e)
        {
            MapPolygon mapPolygon = (MapPolygon)polygon.MapElementId;

            if (mapPolygon == null)
            {
                return;
            }

            if (e.PropertyName == Xamarin.Forms.Maps.MapElement.StrokeColorProperty.PropertyName)
            {
                mapPolygon.StrokeColor = polygon.StrokeColor.IsDefault ? Colors.Black : polygon.StrokeColor.ToWindowsColor();
            }
            else if (e.PropertyName == Xamarin.Forms.Maps.MapElement.StrokeWidthProperty.PropertyName)
            {
                mapPolygon.StrokeThickness = polygon.StrokeWidth;
            }
            else if (e.PropertyName == Polygon.FillColorProperty.PropertyName)
            {
                mapPolygon.FillColor = polygon.FillColor.ToWindowsColor();
            }
            else if (e.PropertyName == nameof(Polygon.Geopath))
            {
                mapPolygon.Path = PositionsToGeopath(polygon.Geopath);
            }
        }

        #endregion

        #region Circles

        protected virtual MapPolygon LoadCircle(Circle circle)
        {
            return new MapPolygon()
            {
                Path = PositionsToGeopath(circle.ToCircumferencePositions()),
                StrokeColor = circle.StrokeColor.IsDefault ? Colors.Black : circle.StrokeColor.ToWindowsColor(),
                StrokeThickness = circle.StrokeWidth,
                FillColor = circle.FillColor.ToWindowsColor()
            };
        }

        private void OnCirclePropertyChanged(Circle circle, PropertyChangedEventArgs e)
        {
            MapPolygon mapPolygon = (MapPolygon)circle.MapElementId;

            if (mapPolygon == null)
            {
                return;
            }

            if (e.PropertyName == Xamarin.Forms.Maps.MapElement.StrokeColorProperty.PropertyName)
            {
                mapPolygon.StrokeColor = circle.StrokeColor.IsDefault ? Colors.Black : circle.StrokeColor.ToWindowsColor();
            }
            else if (e.PropertyName == Xamarin.Forms.Maps.MapElement.StrokeWidthProperty.PropertyName)
            {
                mapPolygon.StrokeThickness = circle.StrokeWidth;
            }
            else if (e.PropertyName == Circle.FillColorProperty.PropertyName)
            {
                mapPolygon.FillColor = circle.FillColor.ToWindowsColor();
            }
            else if (e.PropertyName == Circle.CenterProperty.PropertyName ||
                     e.PropertyName == Circle.RadiusProperty.PropertyName)
            {
                mapPolygon.Path = PositionsToGeopath(circle.ToCircumferencePositions());
            }
        }

        #endregion

        private async Task UpdateIsShowingUser(bool moveToLocation = true)
        {
            if (Control == null || Element == null)
            {
                return;
            }

            if (Element.IsShowingUser)
            {
                Geolocator myGeolocator = new Geolocator();
                if (myGeolocator.LocationStatus != PositionStatus.NotAvailable &&
                    myGeolocator.LocationStatus != PositionStatus.Disabled)
                {
                    Geoposition userPosition = await myGeolocator.GetGeopositionAsync();
                    if (userPosition?.Coordinate != null)
                    {
                        LoadUserPosition(userPosition.Coordinate, moveToLocation);
                    }
                }

                if (Control == null || Element == null)
                {
                    return;
                }

                if (_timer == null)
                {
                    _timer = new DispatcherTimer();
                    _timer.Tick += async (s, o) => await UpdateIsShowingUser(moveToLocation: false);
                    _timer.Interval = TimeSpan.FromSeconds(15);
                }

                if (!_timer.IsEnabled)
                {
                    _timer.Start();
                }
            }
            else if (_userPositionCircle != null && Control.Children.Contains(_userPositionCircle))
            {
                _timer.Stop();
                Control.Children.Remove(_userPositionCircle);
            }
        }

        private async Task MoveToRegion(MapSpan span, MapAnimationKind animation = MapAnimationKind.Bow)
        {
            BasicGeoposition nw = new BasicGeoposition
            {
                Latitude = span.Center.Latitude + span.LatitudeDegrees / 2,
                Longitude = span.Center.Longitude - span.LongitudeDegrees / 2
            };
            BasicGeoposition se = new BasicGeoposition
            {
                Latitude = span.Center.Latitude - span.LatitudeDegrees / 2,
                Longitude = span.Center.Longitude + span.LongitudeDegrees / 2
            };
            GeoboundingBox boundingBox = new GeoboundingBox(nw, se);
            try
            {
                _isRegionUpdatePending = !await Control.TrySetViewBoundsAsync(boundingBox, null, animation);
            }
            catch
            {
                _isRegionUpdatePending = true;
            }
        }

        private async Task UpdateVisibleRegion()
        {
            if (Control == null || Element == null)
            {
                return;
            }

            try
            {
                Control.GetLocationFromOffset(new Windows.Foundation.Point(0, 0), out Geopoint nw);
                Control.GetLocationFromOffset(new Windows.Foundation.Point(Control.ActualWidth, Control.ActualHeight), out Geopoint se);

                if (nw != null && se != null)
                {
                    GeoboundingBox boundingBox = new GeoboundingBox(nw.Position, se.Position);
                    Position center = new Position(boundingBox.Center.Latitude, boundingBox.Center.Longitude);
                    double latitudeDelta = Math.Abs(nw.Position.Latitude - se.Position.Latitude);
                    double longitudeDelta = Math.Abs(nw.Position.Longitude - se.Position.Longitude);
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Element.SetVisibleRegion(new MapSpan(center, latitudeDelta, longitudeDelta));
                    });
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void LoadUserPosition(Geocoordinate userCoordinate, bool center)
        {
            if (Control == null || Element == null)
            {
                return;
            }

            BasicGeoposition userPosition = new BasicGeoposition
            {
                Latitude = userCoordinate.Point.Position.Latitude,
                Longitude = userCoordinate.Point.Position.Longitude
            };

            Geopoint point = new Geopoint(userPosition);

            if (_userPositionCircle == null)
            {
                _userPositionCircle = new WEllipse
                {
                    Stroke = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.White),
                    Fill = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Blue),
                    StrokeThickness = 2,
                    Height = 20,
                    Width = 20,
                    Opacity = 50
                };
            }

            if (Control.Children.Contains(_userPositionCircle))
            {
                Control.Children.Remove(_userPositionCircle);
            }

            MapControl.SetLocation(_userPositionCircle, point);
            MapControl.SetNormalizedAnchorPoint(_userPositionCircle, new Windows.Foundation.Point(0.5, 0.5));

            Control.Children.Add(_userPositionCircle);

            if (center)
            {
                Control.Center = point;
                Control.ZoomLevel = 13;
            }
        }

        private void UpdateTrafficEnabled()
        {
            Control.TrafficFlowVisible = Element.TrafficEnabled;
        }

        private void UpdateMapType()
        {
            switch (Element.MapType)
            {
                case MapType.Street:
                    Control.Style = MapStyle.Road;
                    break;
                case MapType.Satellite:
                    Control.Style = MapStyle.Aerial;
                    break;
                case MapType.Hybrid:
                    Control.Style = MapStyle.AerialWithRoads;
                    break;
            }
        }

        private void UpdateHasZoomEnabled()
        {
            Control.ZoomInteractionMode = Element.HasZoomEnabled
                ? MapInteractionMode.GestureAndControl
                : MapInteractionMode.ControlOnly;
        }

        private void UpdateHasScrollEnabled()
        {
            Control.PanInteractionMode = Element.HasScrollEnabled ? MapPanInteractionMode.Auto : MapPanInteractionMode.Disabled;
        }

        private void OnMapTapped(MapControl sender, MapInputEventArgs args)
        {
            Element?.SendMapClicked(new Position(args.Location.Position.Latitude, args.Location.Position.Longitude));
        }
    }
}
