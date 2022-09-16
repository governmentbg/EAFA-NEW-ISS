using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.LocationMapDialog;
using IARA.Mobile.Shared.Popups;
using System;
using System.Threading.Tasks;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.Controls.Base;
using TechnoLogica.Xamarin.Extensions;
using TechnoLogica.Xamarin.Helpers;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace IARA.Mobile.Insp.Controls
{
    public class ExtendedLocationView : TLValidatableView<Position?>
    {
        public static readonly BindableProperty LocationProperty =
            BindableProperty.Create(nameof(Location), typeof(Position?), typeof(ExtendedLocationView), null, BindingMode.TwoWay,
                propertyChanged: OnLocationPropertyChanged);

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(ExtendedLocationView));

        public static readonly BindableProperty IsTitleVisibleProperty =
            BindableProperty.Create(nameof(IsTitleVisible), typeof(bool), typeof(ExtendedLocationView), true);

        public static readonly BindableProperty TitleFontSizeProperty =
            BindableProperty.Create(nameof(TitleFontSize), typeof(double), typeof(ExtendedLocationView), 16d);

        public static readonly BindableProperty IsPickingProperty =
            BindableProperty.Create(nameof(IsPicking), typeof(bool), typeof(ExtendedLocationView), false);

        private readonly ContentView _innerContent;
        private readonly StackLayout _stack;
        private readonly TLInputTitle _titleLabel;
        private readonly LocationHeaderView _header;
        private Xamarin.Forms.Maps.Map _map;
        private bool _changedFromEntry;

        public ExtendedLocationView()
        {
            _stack = new StackLayout();
            _titleLabel = new TLInputTitle
            {
                BindingContext = this,
                TextColor = Color.Black
            }.Bind(TLInputTitle.HasAsteriskProperty, HasAsteriskProperty.PropertyName)
                .Bind(TLInputTitle.FontSizeProperty, TitleFontSizeProperty.PropertyName)
                .Bind(TLInputTitle.TitleProperty, TitleProperty.PropertyName)
                .Bind(TLInputTitle.IsVisibleProperty, IsTitleVisibleProperty.PropertyName);

            _innerContent = new ContentView
            {
                Content = new Grid
                {
                    Padding = 5,
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = GridLength.Star },
                    },
                    Children =
                    {
                        new Button
                        {
                            BindingContext = this,
                            ImageSource = new FontImageSource
                            {
                                FontFamily = "FA",
                                Color = Color.White,
                                Glyph = IconFont.DiamondTurnRight,
                                Size = 25,
                            },
                            Command = CommandBuilder.CreateFrom(OnChooseLocation)
                        }.BindTranslation(Button.TextProperty, "ChooseLocation", nameof(GroupResourceEnum.Common))
                            .Bind(Button.IsVisibleProperty, IsEnabledProperty.PropertyName)
                            .Bind(Button.IsEnabledProperty, IsPickingProperty.PropertyName, converter: App.GetResource<IValueConverter>("OppositeBool")),
                        new Button
                        {
                            BindingContext = this,
                            ImageSource = new FontImageSource
                            {
                                FontFamily = "FA",
                                Color = Color.White,
                                Glyph = IconFont.LocationCrosshairs,
                                Size = 25,
                            },
                            Command = CommandBuilder.CreateFrom(OnChooseCurrentLocation)
                        }.BindTranslation(Button.TextProperty, "ChooseCurrentLocation", nameof(GroupResourceEnum.Common))
                            .Bind(Button.IsVisibleProperty, IsEnabledProperty.PropertyName)
                            .Bind(Button.IsEnabledProperty, IsPickingProperty.PropertyName, converter: App.GetResource<IValueConverter>("OppositeBool"))
                            .Column(1)
                    }
                }
            };

            _header = new LocationHeaderView
            {
                Command = CommandBuilder.CreateFrom<Position>(OnHeaderLocationChanged),
                OptionsCommand = CommandBuilder.CreateFrom<MenuResult>(OnMenuChosen),
            }.Bind(LocationHeaderView.IsPickingProperty, IsPickingProperty.PropertyName, source: this);

            _stack.Children.Add(_titleLabel);
            _stack.Children.Add(new Frame
            {
                HasShadow = false,
                BorderColor = Color.LightGray,
                CornerRadius = 5,
                HorizontalOptions = LayoutOptions.Start,
                Padding = 0,
                Content = new StackLayout
                {
                    Spacing = 0,
                    Children =
                    {
                        _header,
                        _innerContent,
                        new BoxView
                        {
                            BackgroundColor = Color.LightGray,
                            HeightRequest = 1,
                        },
                        new Grid
                        {
                            Padding = 5,
                            ColumnDefinitions =
                            {
                                new ColumnDefinition { Width = GridLength.Star },
                                new ColumnDefinition { Width = GridLength.Star },
                            },
                            Children =
                            {
                                new TLPicker()
                                    .BindTranslation(TLPicker.TitleProperty, "Quadrant", nameof(GroupResourceEnum.GeneralInfo))
                                    .Bind(TLPicker.ItemsSourceProperty, nameof(ExtendedLocationViewModel.Quadrants))
                                    .Bind(TLPicker.ValidStateProperty, nameof(ExtendedLocationViewModel.Quadrant))
                                    .Bind(TLPicker.IsEnabledProperty, IsEnabledProperty.PropertyName, source: this),
                                new TLEntry()
                                    .Column(1)
                                    .BindTranslation(TLEntry.TitleProperty, "Description", nameof(GroupResourceEnum.GeneralInfo))
                                    .Bind(TLEntry.ValidStateProperty, nameof(ExtendedLocationViewModel.Description))
                                    .Bind(TLEntry.IsEnabledProperty, IsEnabledProperty.PropertyName, source: this),
                            }
                        },
                    }
                }
            });
            _stack.Children.Add(ErrorStack);

            Content = _stack;

            this.Bind(ValidStateProperty, nameof(ExtendedLocationViewModel.Location));
            this.Bind(CommandProperty, nameof(ExtendedLocationViewModel.LocationChosen));

            ComponentTitleProperty = TitleProperty;
            ValidStateValueProperty = LocationProperty;
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public bool IsTitleVisible
        {
            get => (bool)GetValue(IsTitleVisibleProperty);
            set => SetValue(IsTitleVisibleProperty, value);
        }

        public double TitleFontSize
        {
            get => (double)GetValue(TitleFontSizeProperty);
            set => SetValue(TitleFontSizeProperty, value);
        }

        public Position? Location
        {
            get => (Position?)GetValue(LocationProperty);
            set => SetValue(LocationProperty, value);
        }

        public bool IsPicking
        {
            get => (bool)GetValue(IsPickingProperty);
            private set => SetValue(IsPickingProperty, value);
        }

        private async Task OnChooseLocation()
        {
            IsPicking = true;
            await TLLoadingHelper.ShowFullLoadingScreen();
            Location geolocation = await Geolocation.GetLocationAsync(new GeolocationRequest { Timeout = TimeSpan.FromSeconds(5) });

            if (geolocation == null)
            {
                geolocation = await Geolocation.GetLastKnownLocationAsync();

                if (geolocation == null)
                {
                    IsPicking = false;
                    await TLLoadingHelper.HideFullLoadingScreen();
                    return;
                }
            }

            MapClickedEventArgs result = await TLPopupHelper.OpenPopup<MapClickedEventArgs, LocationMapDialog>(new LocationMapDialog(geolocation), hideLoading: true);

            IsPicking = false;

            if (result != null)
            {
                Location = result.Position;
                ValueUpdated(result.Position);
            }
        }

        private async Task OnChooseCurrentLocation()
        {
            IsPicking = true;
            Location geolocation = await Geolocation.GetLocationAsync(new GeolocationRequest { Timeout = TimeSpan.FromSeconds(5) });

            if (geolocation?.IsFromMockProvider == true)
            {
                IsPicking = false;
                return;
            }

            Position location = new Position(geolocation.Latitude, geolocation.Longitude);

            IsPicking = false;
            Location = location;
            ValueUpdated(location);
        }

        private void LayoutMap(Position location)
        {
            if (!_changedFromEntry)
            {
                _header.UpdateLocation(location);
            }
            else
            {
                _changedFromEntry = false;
            }

            if (_map != null)
            {
                _map.Pins.Clear();

                _map.Pins.Add(new Pin
                {
                    Position = location,
                    Label = string.Empty,
                }.BindTranslation(Pin.LabelProperty, "ChosenLocation", nameof(GroupResourceEnum.Common)));

                _map.MoveToRegion(MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(.5)));

                return;
            }

            _map = new Xamarin.Forms.Maps.Map(MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(.5)))
            {
                Margin = new Thickness(2, 0),
                HasScrollEnabled = false,
                Pins =
                {
                    new Pin
                    {
                        Position = location,
                        Label = string.Empty,
                    }.BindTranslation(Pin.LabelProperty, "ChosenLocation", nameof(GroupResourceEnum.Common))
                }
            };

            _innerContent.Content = new TLFillLayout
            {
                BindingContext = this,
                HeightRequest = 200,
                Children =
                {
                    _map,
                    new Button
                    {
                        BindingContext = this,
                        ImageSource = new FontImageSource
                        {
                            FontFamily = "FA",
                            Color = Color.White,
                            Glyph = IconFont.DiamondTurnRight,
                            Size = 25,
                        },
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.End,
                        Command = CommandBuilder.CreateFrom(OnChooseLocation)
                    }.BindTranslation(Button.TextProperty, "ChooseLocation", nameof(GroupResourceEnum.Common))
                        .Bind(Button.IsVisibleProperty, IsEnabledProperty.PropertyName)
                }
            };
        }

        private Task OnMenuChosen(MenuResult result)
        {
            switch ((string)result.Option)
            {
                case "Pick":
                    return OnChooseLocation();
                case "Current":
                    return OnChooseCurrentLocation();
                default:
                    return Task.CompletedTask;
            }
        }

        private void OnHeaderLocationChanged(Position location)
        {
            _changedFromEntry = true;
            Location = location;
        }

        private static void OnLocationPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is ExtendedLocationView locationView))
            {
                return;
            }

            if (newValue is Position position)
            {
                locationView.LayoutMap(position);
            }
        }
    }
}
