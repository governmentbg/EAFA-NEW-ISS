using System;
using System.Threading.Tasks;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.FlyoutPages.InspectionsPage;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Converters;
using TechnoLogica.Xamarin.DataTemplates.Base;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace IARA.Mobile.Insp.DataTemplateSelectors
{
    public class InspectionDataTemplateSelector : BaseDataTemplateSelector<InspectionDto, InspectionsViewModel>
    {
        public override Func<object> FromTemplate(InspectionDto item, BindableObject container, InspectionsViewModel bindingContext)
        {
            StackLayout buttonsStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
            };

            DateTimeToStringConverter dateTimeConverter = App.GetResource<DateTimeToStringConverter>("DateTimeToString");

            if (item.SubmitType == SubmitType.Finish)
            {
                if (item.InspectionState == InspectionState.Submitted && item.CreatedByCurrentUser)
                {
                    buttonsStack.Children.Add(new ImageButton
                    {
                        Source = new FontImageSource
                        {
                            Color = Color.White,
                            FontFamily = "FA",
                            Glyph = IconFont.Signature,
                            Size = 25
                        },
                        Padding = 10,
                        Command = bindingContext.SignInspection,
                        CommandParameter = item,
                    });
                }

                buttonsStack.Children.Add(new ImageButton
                {
                    Source = new FontImageSource
                    {
                        Color = Color.White,
                        FontFamily = "FA",
                        Glyph = IconFont.Eye,
                        Size = 25
                    },
                    Padding = 10,
                    Command = bindingContext.OpenInspection,
                    CommandParameter = item,
                });
            }
            else if (item.CreatedByCurrentUser)
            {
                buttonsStack.Children.Add(new ImageButton
                {
                    Source = new FontImageSource
                    {
                        Color = Color.White,
                        FontFamily = "FA",
                        Glyph = IconFont.Pen,
                        Size = 25
                    },
                    Padding = 10,
                    Command = bindingContext.OpenInspection,
                    CommandParameter = item,
                });

                buttonsStack.Children.Add(new ImageButton
                {
                    Source = new FontImageSource
                    {
                        Color = Color.White,
                        FontFamily = "FA",
                        Glyph = IconFont.Trash,
                        Size = 25
                    },
                    Padding = 10,
                    Command = bindingContext.DeleteInspection,
                    CommandParameter = item,
                });
            }

            Grid content = new Grid
            {
                GestureRecognizers =
                {
                    new TapGestureRecognizer
                    {
                        Command = bindingContext.OpenInspection,
                        CommandParameter = item,
                    }
                },
                BackgroundColor = bindingContext.Inspections.IndexOf(f => f.Id == item.Id) % 2 == 0
                    ? Color.Transparent
                    : Color.FromRgb(232, 232, 232),
                Padding = new Thickness(20, 10),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = 150 },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = 175 },
                    new ColumnDefinition { Width = 100 },
                },
                Children =
                {
                    HandleReportNumber(item),
                    CreateLabel(item.Description).Column(1),
                    CreateLabel(item.Inspectors).Column(2),
                    CreateLabel(item.InspectionSubjects).Column(3),
                    CreateLabel(dateTimeConverter.ConvertTo(item.StartDate)).Column(4),
                    buttonsStack.Column(5)
                }
            };

            if (Device.RuntimePlatform == Device.UWP)
            {
                return () => new ViewCell
                {
                    View = content
                };
            }

            return () => content;
        }

        public static async Task OpenOfflineInfo()
        {
            const string group = nameof(GroupResourceEnum.Common);

            await App.Current.MainPage.DisplayAlert(
                TranslateExtension.Translator[group + "/InspectionOfflineTitle"],
                TranslateExtension.Translator[group + "/InspectionOfflineMessage"],
                TranslateExtension.Translator[group + "/Okay"]
            );
        }

        public static async Task OpenContentSavedLocallyInfo()
        {
            const string group = nameof(GroupResourceEnum.Common);

            await App.Current.MainPage.DisplayAlert(
                TranslateExtension.Translator[group + "/HasContentLocallyTitle"],
                TranslateExtension.Translator[group + "/HasContentLocallyMessage"],
                TranslateExtension.Translator[group + "/Okay"]
            );
        }

        public static View CreateLabel(string text)
        {
            Label label = new Label
            {
                Text = text,
                LineBreakMode = LineBreakMode.WordWrap,
                VerticalOptions = LayoutOptions.Center
            };
            return label;
        }

        public static View HandleReportNumber(InspectionDto item)
        {
            if (item.IsLocal)
            {
                return new StackLayout
                {
                    GestureRecognizers =
                    {
                        new TapGestureRecognizer
                        {
                            Command = CommandBuilder.CreateFrom(OpenOfflineInfo)
                        }
                    },
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        new Label
                        {
                            VerticalOptions = LayoutOptions.Center,
                            FontFamily = "FA",
                            FontSize = 24,
                            Text = IconFont.CircleExclamation,
                            TextColor = Color.Gold
                        },
                        new Label
                        {
                            Text = FormatNumber(item.Number, "NotUploaded"),
                            LineBreakMode = LineBreakMode.WordWrap,
                            VerticalOptions = LayoutOptions.Center
                        },
                    }
                };
            }
            else if (item.HasContentLocally)
            {
                return new StackLayout
                {
                    GestureRecognizers =
                    {
                        new TapGestureRecognizer
                        {
                            Command = CommandBuilder.CreateFrom(OpenContentSavedLocallyInfo)
                        }
                    },
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        new Label
                        {
                            VerticalOptions = LayoutOptions.Center,
                            FontFamily = "FA",
                            FontSize = 24,
                            Text = IconFont.FloppyDisk,
                            TextColor = App.GetResource<Color>("Primary")
                        },
                        new Label
                        {
                            Text = FormatNumber(item.Number, item.InspectionState == InspectionState.Draft ? "Draft" : "NoReportNumber"),
                            LineBreakMode = LineBreakMode.WordWrap,
                            VerticalOptions = LayoutOptions.Center
                        },
                    }
                };
            }
            else
            {
                return new Label
                {
                    Text = FormatNumber(item.Number, item.InspectionState == InspectionState.Draft ? "Draft" : "NoReportNumber"),
                    LineBreakMode = LineBreakMode.WordWrap,
                    VerticalOptions = LayoutOptions.Center
                };
            }
        }

        public static string FormatNumber(string number, string emptyResource)
        {
            return string.IsNullOrEmpty(number)
                ? TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + '/' + emptyResource]
                : number;
        }
    }
}
