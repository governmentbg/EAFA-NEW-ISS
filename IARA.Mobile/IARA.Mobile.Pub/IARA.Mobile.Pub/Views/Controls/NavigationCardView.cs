using System.Collections.Generic;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.Extensions;
using Xamarin.CommunityToolkit.Effects;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.Views.Controls
{
    public class NavigationCardView : Frame
    {
        public static readonly BindableProperty IconProperty =
            BindableProperty.Create(nameof(Icon), typeof(string), typeof(NavigationCardView));

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(NavigationCardView), string.Empty);

        public static readonly BindableProperty NavigateCommandProperty =
            BindableProperty.Create(nameof(NavigateCommand), typeof(ICommand), typeof(NavigationCardView));

        public static readonly BindableProperty NavigateCommandParameterProperty =
            BindableProperty.Create(nameof(NavigateCommandParameter), typeof(object), typeof(NavigationCardView));

        public NavigationCardView()
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {
                TLFlex.SetGrow(this, 1);
                HeightRequest = 100;
            }
            else
            {
                TLFlex.SetGrow(this, 2);
                HeightRequest = 175;
            }

            CornerRadius = 10;
            Padding = 0;

            Color primaryColor = App.GetResource<Color>("Primary");

            Content = new Grid
            {
                RowSpacing = 0,
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(50, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(50, GridUnitType.Star) }
                },
                Padding = new Thickness(15, 0),
                Children =
                {
                    new Label
                    {
                        VerticalOptions = LayoutOptions.End,
                        BindingContext = this,
                        StyleClass = new List<string> { "BaseIcon" },
                        FontSize = 35,
                        TextColor = primaryColor,
                    }.Bind(Label.TextProperty, IconProperty.PropertyName),
                    new Label
                    {
                        VerticalOptions = LayoutOptions.Start,
                        BindingContext = this,
                        HorizontalTextAlignment = TextAlignment.Center,
                        LineBreakMode = LineBreakMode.WordWrap,
                        TextColor = primaryColor
                    }.Row(1)
                        .Bind(Label.TextProperty, TextProperty.PropertyName)
                }
            };

            TouchEffect.SetCommand(this, CommandBuilder.CreateFrom(() => NavigateCommand?.ExecuteCommand(NavigateCommandParameter)));
            TouchEffect.SetNormalBackgroundColor(this, Color.White);

            if (Device.RuntimePlatform == Device.Android)
            {
                TouchEffect.SetNativeAnimation(this, true);
            }
            else
            {
                TouchEffect.SetPressedBackgroundColor(this, Color.Gray);
                TouchEffect.SetAnimationEasing(this, Easing.CubicOut);
                TouchEffect.SetAnimationDuration(this, 100);
            }
        }

        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public ICommand NavigateCommand
        {
            get => (ICommand)GetValue(NavigateCommandProperty);
            set => SetValue(NavigateCommandProperty, value);
        }

        public object NavigateCommandParameter
        {
            get => GetValue(NavigateCommandParameterProperty);
            set => SetValue(NavigateCommandParameterProperty, value);
        }
    }
}
