using System;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.FlyoutPages.InspectionsPage;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Converters;
using TechnoLogica.Xamarin.DataTemplates.Base;
using TechnoLogica.Xamarin.Extensions;
using TechnoLogica.Xamarin.Helpers;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.DataTemplateSelectors
{
    public class InspectionOnPhoneDataTemplateSelector : BaseDataTemplateSelector<InspectionDto, InspectionsViewModel>
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
                if (item.InspectionState == InspectionState.Signed && item.CreatedByCurrentUser)
                {
                    buttonsStack.Children.Add(new ImageButton
                    {
                        Source = new FontImageSource
                        {
                            Color = Color.White,
                            FontFamily = "FA",
                            Glyph = IconFont.Envelope,
                            Size = 25
                        },
                        Padding = 10,
                        Command = bindingContext.SendInspection,
                        CommandParameter = item,
                    });
                }

                if (item.InspectionState == InspectionState.Submitted)
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
            }
            else
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
                Command = CommandBuilder.CreateFrom(() =>
                {
                    item.IsReview = true;
                    bindingContext.OpenInspection.Execute(item);
                }),
            });

            const string group = nameof(GroupResourceEnum.Inspections);

            ContentView content = new ContentView
            {
                Content = new Frame
                {
                    HasShadow = false,
                    Margin = 5,
                    BorderColor = App.GetResource<Color>("GrayColor"),
                    Content = new Grid
                    {
                        RowSpacing = 5,
                        ColumnSpacing = 20,
                        GestureRecognizers =
                        {
                            new TapGestureRecognizer
                            {
                                Command = bindingContext.OpenInspection,
                                CommandParameter = item,
                            }
                        },
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = 150 },
                            new ColumnDefinition { Width = GridLength.Star },
                        },
                        RowDefinitions =
                        {
                            new RowDefinition { Height = GridLength.Auto },
                            new RowDefinition { Height = GridLength.Auto },
                            new RowDefinition { Height = GridLength.Auto },
                            new RowDefinition { Height = GridLength.Auto },
                            new RowDefinition { Height = GridLength.Auto },
                            new RowDefinition { Height = 40 },
                        },
                        Children =
                        {
                            new Label()
                                .BindTranslation(Label.TextProperty, "Number", group),
                            new Label()
                                .BindTranslation(Label.TextProperty, "Description", group)
                                .Row(1),
                            new Label()
                                .BindTranslation(Label.TextProperty, "Inspectors", group)
                                .Row(2),
                            new Label()
                                .BindTranslation(Label.TextProperty, "InspectionSubjects", group)
                                .Row(3),
                            new Label()
                                .BindTranslation(Label.TextProperty, "StartDate", group)
                                .Row(4),
                            InspectionDataTemplateSelector.HandleReportNumber(item).Column(1),
                            InspectionDataTemplateSelector.CreateLabel(item.Description).Row(1).Column(1),
                            InspectionDataTemplateSelector.CreateLabel(item.Inspectors).Row(2).Column(1),
                            InspectionDataTemplateSelector.CreateLabel(item.InspectionSubjects).Row(3).Column(1),
                            InspectionDataTemplateSelector.CreateLabel(dateTimeConverter.ConvertTo(item.StartDate)).Row(4).Column(1),
                            buttonsStack.Row(5).ColumnSpan(2)
                        }
                    }
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
    }
}
