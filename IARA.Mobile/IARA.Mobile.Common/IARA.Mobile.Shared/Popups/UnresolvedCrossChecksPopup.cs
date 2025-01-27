using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Shared.Views;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Popups
{
    public class UnresolvedCrossChecksPopup : PopupPage
    {
        public UnresolvedCrossChecksPopup(CanMakeInspectionDto crossChecks)
        {
            string closeAction = "";
            if (crossChecks.CanMakeInspection)
            {
                closeAction = "Продължи";
            }
            else
            {
                closeAction = "Затвори";
            }

            var buttons = new Grid
            {
                ColumnSpacing = 12,
                ColumnDefinitions =
                {
                    new ColumnDefinition()
                    {
                        Width = GridLength.Star
                    },
                    new ColumnDefinition()
                    {
                        Width = new GridLength(2.2, GridUnitType.Star)
                    }
                },
                Margin = new Thickness(20, 10),
                HorizontalOptions = LayoutOptions.Fill,
                Children =
                {
                    new TLButton
                    {
                        Text = closeAction,
                        Command = CommandBuilder.CreateFrom(OnClose)
                    }.Column(0),
                    new TLButton
                    {
                        Text = "Резултати от кръстосани проверки",
                        Command = CommandBuilder.CreateFrom(OnView)
                    }.Column(1),
                }
            };

            var content = new StackLayout
            {
                Spacing = 0,
                Children =
                {
                    new Image
                    {
                        Source = ImageSourceConvert("iara_logo"),
                        HeightRequest = 150,
                        WidthRequest = 150,
                        Margin = new Thickness(0, 15, 0, 0),
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        FontSize = 24,
                        Padding = 5,
                        FontAttributes = FontAttributes.Bold,
                        Text = "Неразрешени кръстосани проверки!",
                        HorizontalOptions = LayoutOptions.Center,
                    },
                }
            };

            string text = "";

            if (crossChecks.UnresolvedCrossChecks > 0)
            {
                text += crossChecks.CanMakeInspection ? $"Имате назначени {crossChecks.UnresolvedCrossChecks} неразрешени кръстосани проверки. "
                            : "Има назначени на Вас неразрешени кръстосани проверки и не можете да добавите нова инспекция.";

            }

            if (crossChecks.CanMakeInspection)
            {
                DateTime dueDate = DateTime.Now + crossChecks.TimeRemainingUntilInspectorLock;
                text += $"Оставащо време до заключване на инспектора: {crossChecks.TimeRemainingUntilInspectorLock.Days} дни и {crossChecks.TimeRemainingUntilInspectorLock.Hours} часа. Моля разрешете конкретните проверки в срок до {dueDate.ToString(CommonConstants.DateTimeFormat)}! ";
            }

            content.Children.Add(new Label
            {
                Margin = new Thickness(20, 10),
                Text = text,
                FontSize = 18,
                LineBreakMode = LineBreakMode.WordWrap,
            });

            content.Children.Add(buttons);

            Content = new Frame
            {
                HasShadow = false,
                Margin = 50,
                Padding = 0,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Content = content
            };
        }

        private Task OnClose()
        {
            return PopupNavigation.Instance.PopAsync();
        }

        private Task OnView()
        {
            return Browser.OpenAsync($"https://www.microsoft.com/bg-bg/p/iara-inspectors/9PFGKH1JMSSL");
        }

        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            return true;
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return false;
        }

        public static FileImageSource ImageSourceConvert(string path)
        {
            if (Device.RuntimePlatform == Device.UWP)
            {
                path = System.IO.Path.Combine($"Images/{path}.png");
            }

            return (FileImageSource)ImageSource.FromFile(path);
        }
    }
}
