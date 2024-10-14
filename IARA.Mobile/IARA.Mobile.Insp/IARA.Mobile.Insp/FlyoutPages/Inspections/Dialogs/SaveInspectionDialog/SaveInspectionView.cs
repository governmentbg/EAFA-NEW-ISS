using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.ViewModels.Models;
using SignaturePad.Forms;
using System.Collections.Generic;
using System.IO;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.Extensions;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FloppyDiskInspectionDialog
{
    public class SaveInspectionView : TLBaseDialog<SaveInspectionViewModel, bool>
    {
        public SaveInspectionView(List<SignatureSaveModel> signatures)
        {
            ViewModel.Signatures = signatures;

            const string groupGF = nameof(GroupResourceEnum.GeneralInfo);
            const string groupC = nameof(GroupResourceEnum.Common);

            this.BindTranslation(TitleProperty, "SignInspection", groupGF);

            TitleBackgroundColor = App.GetResource<Color>("Primary");
            TitleColor = Color.White;
            IconColor = Color.White;
            BackgroundColor = Color.White;
            HasMaxWidth = false;

            IValueConverter stackOrientationConverter = new FuncConverter<double, StackOrientation>(
                (width) => width > 900 ? StackOrientation.Horizontal : StackOrientation.Vertical
            );

            StackLayout checkStack = new StackLayout();

            StackLayout stack = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
            }.Bind(StackLayout.OrientationProperty, Page.WidthProperty.PropertyName, converter: stackOrientationConverter, source: App.Current.MainPage);

            foreach (SignatureSaveModel signature in signatures)
            {
                TLCheckView check = null;

                if (!string.IsNullOrEmpty(signature.DoesNotWantToSignMessage))
                {
                    check = new TLCheckView
                    {
                        Text = signature.DoesNotWantToSignMessage,
                    };
                    checkStack.Children.Add(check);
                }

                SignaturePadView signatureView = new SignaturePadView
                {
                    BindingContext = this,
                    StrokeWidth = 3,
                    StrokeColor = Color.Black,
                    BackgroundColor = Color.Transparent,
                    PromptText = string.Empty,
                }.BindTranslation(SignaturePadView.ClearTextProperty, "Clear", groupC)
                    .Bind(SignaturePadView.CaptionTextProperty, nameof(SignatureSaveModel.Caption), source: signature);

                ViewModel.SignatureBytes.Add(async () =>
                {
                    if (signatureView.IsBlank)
                    {
                        return null;
                    }

                    Stream image = await signatureView.GetImageStreamAsync(SignatureImageFormat.Png);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        return memoryStream.ToArray();
                    }
                });

                stack.Children.Add(new Frame
                {
                    HasShadow = false,
                    Padding = 5,
                    BorderColor = Color.LightGray,
                    CornerRadius = 5,
                    HeightRequest = 300,
                    WidthRequest = 400,
                    Content = signatureView
                }.Bind(Frame.IsVisibleProperty, TLCheckView.IsCheckedProperty.PropertyName, fallbackValue: true, converter: App.GetResource<IValueConverter>("OppositeBool"), source: check));
            }

            checkStack.IsVisible = checkStack.Children.Count > 0;

            Content = new ScrollView
            {
                Margin = 10,
                Content = new StackLayout
                {
                    Spacing = 10,
                    Padding = 10,
                    Children =
                    {
                        stack,
                        new Label
                        {
                            TextColor = App.GetResource<Color>("ErrorColor"),
                            LineBreakMode = LineBreakMode.WordWrap,
                            HorizontalOptions = LayoutOptions.Center,
                            HorizontalTextAlignment = TextAlignment.Center,
                        }.BindTranslation(Label.TextProperty, "NeedsSignatures", groupGF)
                            .Bind(Label.IsVisibleProperty, nameof(SaveInspectionViewModel.NeedsSignatures)),
                        checkStack,
                        new Button
                        {
                            HorizontalOptions = LayoutOptions.End,
                            Command = ViewModel.Save
                        }.BindTranslation(Button.TextProperty, "Sign", groupC)
                    }
                }
            };
        }
    }
}
