using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.ViewModels.Models;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.Extensions;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Controls
{
    public class SignatureView : Frame
    {
        public static readonly BindableProperty CaptionTextProperty =
            BindableProperty.Create(nameof(CaptionText), typeof(string), typeof(SignatureView));

        public static readonly BindableProperty ModelProperty =
            BindableProperty.Create(nameof(Model), typeof(SignatureModel), typeof(SignatureView));

        public SignatureView()
        {
            HasShadow = false;
            Padding = 5;
            BorderColor = Color.LightGray;
            CornerRadius = 5;
            HeightRequest = 300;

            Content = new TLFillLayout
            {
                Children =
                {
                    new Label
                    {
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    }.BindTranslation(Label.TextProperty, "NoInternet", nameof(GroupResourceEnum.GeneralInfo)),
                    new ContentView
                    {
                        BackgroundColor = Color.White,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Content = new Image
                        {
                            BindingContext = this,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center
                        }.Bind(Image.SourceProperty, ModelProperty.PropertyName + "." + nameof(SignatureModel.Image), source: this)
                    }.Bind(ContentView.IsVisibleProperty, ModelProperty.PropertyName + "." + nameof(SignatureModel.Image), converter: App.GetResource<IValueConverter>("IsNotNull"), source: this),
                    new Label
                    {
                        BindingContext = this,
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.End
                    }.Bind(Label.TextProperty, CaptionTextProperty.PropertyName),
                }
            };
        }

        public string CaptionText
        {
            get => (string)GetValue(CaptionTextProperty);
            set => SetValue(CaptionTextProperty, value);
        }

        public SignatureModel Model
        {
            get => (SignatureModel)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }
    }
}
