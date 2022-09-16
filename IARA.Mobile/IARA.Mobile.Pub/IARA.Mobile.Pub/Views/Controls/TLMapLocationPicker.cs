using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Shared.Helpers;
using System.Threading.Tasks;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.Controls.Base;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace IARA.Mobile.Pub.Views.Controls
{
    public class TLMapLocationPicker : TLValidatableView<Position?>
    {
        public static readonly BindableProperty LocationProperty =
            BindableProperty.Create(nameof(Location), typeof(Position?), typeof(TLMapLocationPicker), null, BindingMode.TwoWay,
                propertyChanged: OnLocationChanged);

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(TLMapLocationPicker));

        private readonly StackLayout _stack;
        private readonly TLInputTitle _titleLabel;

        public TLMapLocationPicker()
        {
            _stack = new StackLayout();
            _titleLabel = new TLInputTitle
            {
                BindingContext = this,
                TextColor = Color.Black
            }.Bind(TLInputTitle.HasAsteriskProperty, HasAsteriskProperty.PropertyName)
                .Bind(TLInputTitle.TitleProperty, TitleProperty.PropertyName);

            _stack.Children.Add(_titleLabel);
            _stack.Children.Add(new Button
            {
                ImageSource = new FontImageSource
                {
                    Color = Color.White,
                    FontFamily = "FA",
                    Glyph = IconFont.DiamondTurnRight,
                },
                Text = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/ChooseLocation"],
                Command = CommandBuilder.CreateFrom(OnChooseLocation)
            });
            _stack.Children.Add(ErrorStack);

            Content = _stack;

            ComponentTitleProperty = TitleProperty;
            ValidStateValueProperty = LocationProperty;
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public Position? Location
        {
            get => (Position?)GetValue(LocationProperty);
            set => SetValue(LocationProperty, value);
        }

        private async Task OnChooseLocation()
        {
            MapClickedEventArgs result = await PopupHelper.OpenLocationPicker();

            if (result != null)
            {
                Location = result.Position;
                ValueUpdated(result.Position);
            }
        }

        private void LayoutMap(Position location)
        {
            _stack.Children.Clear();

            Map map = new Map
            {
                Pins =
                {
                    new Pin
                    {
                        Position = location,
                        Label = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/ChosenLocation"]
                    }
                }
            };
            map.MoveToRegion(MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(.5)));

            _stack.Children.Add(_titleLabel);
            _stack.Children.Add(new TLFillLayout
            {
                HeightRequest = 200,
                Children =
                {
                    map,
                    new Button
                    {
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.End,
                        ImageSource = new FontImageSource
                        {
                            Color = Color.White,
                            FontFamily = "FA",
                            Glyph = IconFont.DiamondTurnRight,
                        },
                        Text = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/ChooseLocation"],
                        Command = CommandBuilder.CreateFrom(OnChooseLocation)
                    }
                }
            });
            _stack.Children.Add(ErrorStack);
        }

        private static void OnLocationChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is TLMapLocationPicker mapLocationPicker) || !(newValue is Position position))
            {
                return;
            }

            mapLocationPicker.LayoutMap(position);
        }
    }
}
