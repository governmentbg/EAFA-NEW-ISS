using System.Collections.Generic;
using System.Globalization;
using System.Windows.Input;
using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Shared.Popups;
using IARA.Mobile.Shared.Views;
using TechnoLogica.Xamarin.Effects;
using TechnoLogica.Xamarin.Extensions;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.CommunityToolkit.Converters;
using Xamarin.CommunityToolkit.Helpers;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace IARA.Mobile.Insp.Controls
{
    public class LocationHeaderView : Grid
    {
        public static readonly BindableProperty IsPickingProperty =
            BindableProperty.Create(nameof(IsPicking), typeof(bool), typeof(LocationHeaderView), false);

        public static readonly BindableProperty OptionsCommandProperty =
            BindableProperty.Create(nameof(OptionsCommand), typeof(ICommand), typeof(LocationHeaderView));

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(LocationHeaderView));

        private readonly Entry[] _longEntries;
        private readonly Entry[] _latEntries;
        private ExecutablePlan executablePlan;
        private bool _isFromUpdate;

        public LocationHeaderView()
        {
            RowSpacing = 0;
            ColumnSpacing = 0;
            HorizontalOptions = LayoutOptions.FillAndExpand;

            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = 1 },
                new ColumnDefinition { Width = GridLength.Star },
            };

            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition { Height = 40 },
                new RowDefinition { Height = 40 },
                new RowDefinition { Height = 1 },
            };

            _longEntries = new Entry[4];
            _latEntries = new Entry[4];

            for (int i = 0; i < 4; i++)
            {
                int maxLength = i == 3 ? 5 : 2;

                _longEntries[i] = CreateEntry(maxLength);
                _latEntries[i] = CreateEntry(maxLength);
            }

            Children.Add(new Label
            {
                FontSize = 16,
                TextColor = Color.White,
                Padding = 8,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = App.GetResource<Color>("Primary"),
            }.BindTranslation(Label.TextProperty, "Latitude", nameof(GroupResourceEnum.Common))
                .ColumnSpan(3));

            Children.Add(new Label
            {
                FontSize = 16,
                TextColor = Color.White,
                Padding = 8,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = App.GetResource<Color>("Primary"),
            }.BindTranslation(Label.TextProperty, "Longitude", nameof(GroupResourceEnum.Common))
                .Column(2));

            Children.Add(new BoxView
            {
                BackgroundColor = Color.LightGray
            }.RowSpan(2).Column(1));

            Children.Add(CreateCoordinatesEntry(_latEntries).Row(1).Column(0));
            Children.Add(CreateCoordinatesEntry(_longEntries).Row(1).Column(2));

            Children.Add(new TLMenuButton
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.End,
                Source = new FontImageSource
                {
                    FontFamily = "FA",
                    Glyph = IconFont.MagnifyingGlassLocation,
                    Color = Color.White,
                    Size = 25
                },
                Margin = new Thickness(0, 0, 5, 0),
                Padding = 5,
                HeightRequest = 30,
                WidthRequest = 30,
                Choices = new List<MenuOption>
                {
                    new MenuOption
                    {
                        Icon = IconFont.DiamondTurnRight,
                        Option = "Pick",
                        Text = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/ChooseLocation"]
                    },
                    new MenuOption
                    {
                        Icon = IconFont.LocationCrosshairs,
                        Option = "Current",
                        Text = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/ChooseCurrentLocation"]
                    }
                }
            }.ColumnSpan(3)
                .Bind(TLMenuButton.IsEnabledProperty, IsPickingProperty.PropertyName, converter: App.GetResource<IValueConverter>("OppositeBool"), source: this)
                .Bind(TLMenuButton.IsVisibleProperty, IsEnabledProperty.PropertyName, source: this)
                .Bind(TLMenuButton.CommandProperty, OptionsCommandProperty.PropertyName, source: this));

            Children.Add(new BoxView
            {
                BackgroundColor = Color.LightGray
            }.Row(2).ColumnSpan(3));

            for (int i = 0; i < _longEntries.Length; i++)
            {
                _longEntries[i].TextChanged += OnTextChanged;
            }
        }

        public bool IsPicking
        {
            get => (bool)GetValue(IsPickingProperty);
            set => SetValue(IsPickingProperty, value);
        }

        public ICommand OptionsCommand
        {
            get => (ICommand)GetValue(OptionsCommandProperty);
            set => SetValue(OptionsCommandProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public void UpdateLocation(LocationDto location)
        {
            DMSType dmsLong = DMSType.Parse(location.DMSLongitude);
            DMSType dmsLat = DMSType.Parse(location.DMSLatitude);

            _isFromUpdate = true;

            _longEntries[0].Text = dmsLong.Degrees.ToString().PadLeft(2, '0');
            _longEntries[1].Text = dmsLong.Minutes.ToString().PadLeft(2, '0');
            _longEntries[2].Text = dmsLong.Seconds.ToString().PadLeft(2, '0');
            _longEntries[3].Text = dmsLong.Milliseconds.ToString();

            _latEntries[0].Text = dmsLat.Degrees.ToString().PadLeft(2, '0');
            _latEntries[1].Text = dmsLat.Minutes.ToString().PadLeft(2, '0');
            _latEntries[2].Text = dmsLat.Seconds.ToString().PadLeft(2, '0');
            _latEntries[3].Text = dmsLat.Milliseconds.ToString();

            _isFromUpdate = false;
        }

        private Entry CreateEntry(int maxLength)
        {
            Entry entry = new Entry
            {
                WidthRequest = maxLength * (Device.RuntimePlatform == Device.Android ? 12.5 : 8.8),
                MaxLength = maxLength,
                FontSize = 16,
                Keyboard = Keyboard.Numeric,
                Effects =
                {
                    new BorderlessEntryEffect()
                }
            }.Bind(Entry.IsReadOnlyProperty, new MultiBinding
            {
                Bindings =
                {
                    new Binding(IsEnabledProperty.PropertyName, source: this),
                    new Binding(IsPickingProperty.PropertyName, converter: App.GetResource<IValueConverter>("OppositeBool"), source: this)
                },
                Converter = new VariableMultiValueConverter { ConditionType = MultiBindingCondition.None }
            });

            entry.TextChanged += OnTextChanged;

            return entry;
        }

        private View CreateCoordinatesEntry(Entry[] entries)
        {
            Thickness labelMargin = new Thickness(0, 6, 0, 0);

            return new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 3,
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    entries[0],
                    new Label
                    {
                        FontSize = 16,
                        Text = "°",
                        Margin = labelMargin,
                    },
                    entries[1],
                    new Label
                    {
                        FontSize = 16,
                        Text = "'",
                        Margin = labelMargin,
                    },
                    entries[2],
                    new Label
                    {
                        FontSize = 16,
                        Text = ".",
                        Margin = labelMargin,
                    },
                    entries[3],
                    new Label
                    {
                        FontSize = 16,
                        Text = "\"",
                        Margin = labelMargin,
                    },
                }
            };
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isFromUpdate)
            {
                return;
            }

            executablePlan?.Dispose();

            executablePlan = ExecutablePlan.Delay(3000, () =>
            {
                executablePlan.Dispose();
                if (int.TryParse(_longEntries[0].Text, out int longDegrees)
                    && int.TryParse(_longEntries[1].Text, out int longMinutes)
                    && double.TryParse($"{_longEntries[2].Text}.{_longEntries[3].Text}", NumberStyles.Float, CultureInfo.InvariantCulture, out double longSeconds)
                    && int.TryParse(_latEntries[0].Text, out int latDegrees)
                    && int.TryParse(_latEntries[1].Text, out int latMinutes)
                    && double.TryParse($"{_latEntries[2].Text}.{_latEntries[3].Text}", NumberStyles.Float, CultureInfo.InvariantCulture, out double latSeconds))
                {
                    double longitude = DMSType.ConvertDegreeAngleToDouble(longDegrees, longMinutes, longSeconds);
                    double latitude = DMSType.ConvertDegreeAngleToDouble(latDegrees, latMinutes, latSeconds);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        for (int x = 0; x < _longEntries.Length; x++)
                        {
                            _longEntries[x].Unfocus();
                        }

                        Command?.ExecuteCommand(new LocationDto
                        {
                            DMSLatitude = DMSType.FromDouble(latitude).ToString(),
                            DMSLongitude = DMSType.FromDouble(longitude).ToString(),
                        });
                    });
                }
            });
        }
    }
}
