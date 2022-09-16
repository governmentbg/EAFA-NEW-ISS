using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Shared.Views;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.Extensions;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FloppyDiskInspectionUWPDialog
{
    public class SaveInspectionUWPDialog : TLBaseDialog<SaveInspectionUWPViewModel, FileModel>
    {
        public SaveInspectionUWPDialog()
        {
            const string groupGF = nameof(GroupResourceEnum.GeneralInfo);

            this.BindTranslation(TitleProperty, "SignInspection", groupGF);

            Color primaryColor = App.GetResource<Color>("Primary");

            TitleBackgroundColor = primaryColor;
            TitleColor = Color.White;
            IconColor = Color.White;
            BackgroundColor = Color.White;
            HasMaxWidth = false;

            Frame frame = new Frame
            {
                CornerRadius = 5,
                HorizontalOptions = LayoutOptions.Center,
                WidthRequest = 350,
                BackgroundColor = primaryColor
            };

            FuncConverter<TLFileResult, View> filePickedConverter = new FuncConverter<TLFileResult, View>(file =>
            {
                if (file == null)
                {
                    frame.Padding = 0;
                    frame.HasShadow = false;
                    frame.BorderColor = Color.Transparent;

                    return new TLButton
                    {
                        BindingContext = this,
                        ImageSource = new FontImageSource
                        {
                            FontFamily = "FA",
                            Glyph = IconFont.Upload,
                            Color = Color.White
                        },
                        Command = ViewModel.PickFile
                    }.BindTranslation(Span.TextProperty, "PickFile", groupGF);
                }

                frame.Padding = 10;
                frame.HasShadow = true;
                frame.BorderColor = Color.LightGray;

                return new Grid
                {
                    GestureRecognizers =
                    {
                        new TapGestureRecognizer
                        {
                            Command = ViewModel.PickFile,
                        }
                    },
                    ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition() { Width = GridLength.Auto },
                        new ColumnDefinition() { Width = GridLength.Star }
                    },
                    ColumnSpacing = 15,
                    Children =
                    {
                        new Label
                        {
                            TextColor = Color.White,
                            FontFamily = "FA",
                            FontSize = 40,
                            Text = IconFont.Image,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                        },
                        new StackLayout
                        {
                            Spacing = 0,
                            Children =
                            {
                                new Label
                                {
                                    TextColor = Color.White,
                                    FontAttributes = FontAttributes.Bold,
                                    Text = file.FileName,
                                },
                                new Grid
                                {
                                    ColumnDefinitions = new ColumnDefinitionCollection
                                    {
                                        new ColumnDefinition() { Width = GridLength.Star },
                                        new ColumnDefinition() { Width = GridLength.Auto }
                                    },
                                    Children =
                                    {
                                        new Label
                                        {
                                            TextColor = Color.White,
                                            Text = file.ContentType
                                        },
                                        new TLRichLabel
                                        {
                                            TextColor = Color.White,
                                            Spans =
                                            {
                                                new Span
                                                {
                                                    Text = (file.FileSize / 1000).ToString()
                                                },
                                                new Span
                                                {
                                                    Text = " "
                                                },
                                                new Span()
                                                    .BindTranslation(Span.TextProperty, "Kilobytes", groupGF)
                                            }
                                        }.Column(1)
                                    }
                                }
                            }
                        }.Column(1)
                    }
                };
            });

            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Spacing = 10,
                    Padding = 10,
                    Children =
                    {
                        frame.Bind(Frame.ContentProperty, nameof(ViewModel.FileResult), converter: filePickedConverter, source: ViewModel),
                        new Label
                        {
                            TextColor = App.GetResource<Color>("ErrorColor"),
                            LineBreakMode = LineBreakMode.WordWrap,
                            HorizontalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                        }.BindTranslation(Label.TextProperty, "NeedsSignature", groupGF)
                            .Bind(Label.IsVisibleProperty, nameof(SaveInspectionUWPViewModel.NeedsSignature)),
                        new Button
                        {
                            HorizontalOptions = LayoutOptions.End,
                            Command = ViewModel.Save
                        }.BindTranslation(Button.TextProperty, "Sign", groupGF)
                    }
                }
            };
        }
    }
}
