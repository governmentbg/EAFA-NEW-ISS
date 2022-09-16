using Rg.Plugins.Popup.Pages;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Popups
{
    public class MenuDimension
    {
        public MenuDimension(double rawX, double rawY, double width, double height)
        {
            RawX = rawX;
            RawY = rawY;
            Width = width;
            Height = height;
        }

        public double Height { get; }
        public double RawX { get; }
        public double RawY { get; }
        public double Width { get; }
    }

    public class MenuOption
    {
        public object Option { get; set; }
        public string Icon { get; set; }
        public string Text { get; set; }
    }

    public class MenuResult
    {
        public object Option { get; set; }
        public object Parameter { get; set; }
    }

    public class MenuListPopup : PopupPage
    {
        private readonly MenuDimension _dimension;

        public MenuListPopup(IReadOnlyList<MenuOption> options, ICommand tapped, MenuDimension dimension)
        {
            _dimension = dimension;
            BackgroundColor = Color.Transparent;
            Color primaryColor = (Color)Xamarin.Forms.Application.Current.Resources["Primary"];

            StackLayout optionsList = new StackLayout();

            for (int i = 0; i < options.Count; i++)
            {
                MenuOption option = options[i];

                if (string.IsNullOrEmpty(option.Icon))
                {
                    optionsList.Children.Add(new Label
                    {
                        FontSize = 18,
                        Text = option.Text,
                        Padding = 10,
                        LineBreakMode = LineBreakMode.WordWrap,
                        GestureRecognizers =
                        {
                            new TapGestureRecognizer
                            {
                                Command = tapped,
                                CommandParameter = (option, this)
                            }
                        }
                    });
                }
                else
                {
                    optionsList.Children.Add(new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = 25 },
                            new ColumnDefinition { Width = GridLength.Auto }
                        },
                        Children =
                        {
                            new Label
                            {
                                StyleClass = new List<string> { "BaseIcon" },
                                FontSize = 24,
                                Text = option.Icon,
                                TextColor = primaryColor,
                                VerticalOptions = LayoutOptions.Center,
                            },
                            new Label
                            {
                                FontSize = 18,
                                Text = option.Text,
                                Padding = 10,
                                LineBreakMode = LineBreakMode.WordWrap,
                                VerticalOptions = LayoutOptions.Center,
                            }.Column(1)
                        },
                        GestureRecognizers =
                        {
                            new TapGestureRecognizer
                            {
                                Command = tapped,
                                CommandParameter = (option, this)
                            }
                        }
                    });
                }
            }

            Content = new Frame
            {
                BorderColor = Device.RuntimePlatform == Device.UWP
                    ? Color.LightGray
                    : Color.Transparent,
                IsClippedToBounds = true,
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.Start,
                Margin = 0,
                Padding = new Thickness(15, 0, 5, 0),
                Content = optionsList
            };
        }

        protected override async void OnAppearing()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.UWP:
                    while (Width < 0)
                    {
                        await Task.Delay(10);
                    }
                    break;
                default:
                    await Task.Yield();
                    break;
            }

            double newX = _dimension.RawX;
            double newY = _dimension.RawY;
            double width = Width;
            double height = Height;

            if (newX + Content.Width >= width)
            {
                newX -= newX + Content.Width - width + 5;
            }

            if (newY + Content.Height >= height)
            {
                newY -= Content.Height;
            }

            Content.TranslationX = newX;
            Content.TranslationY = newY;
        }
    }
}
