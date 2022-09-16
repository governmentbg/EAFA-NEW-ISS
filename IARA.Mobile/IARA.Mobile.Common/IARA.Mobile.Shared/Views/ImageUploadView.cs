using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Shared.Helpers;
using IARA.Mobile.Shared.Popups;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.Extensions;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Views
{
    public class ImageUploadView : TLFillLayout
    {
        private enum ImageOption
        {
            Take,
            Pick
        }

        public static readonly BindableProperty SizeProperty =
            BindableProperty.Create(nameof(Size), typeof(double), typeof(ImageUploadView), 150d);

        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(nameof(Source), typeof(ImageSource), typeof(ImageUploadView));

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ImageUploadView));

        public ImageUploadView()
        {
            WidthRequest = 150d;
            HeightRequest = 150d;
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;

            List<MenuOption> choices = new List<MenuOption>
            {
                new MenuOption
                {
                    Icon = IconFont.FileImage,
                    Text = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/PickPhoto"],
                    Option = ImageOption.Pick
                },
                new MenuOption
                {
                    Icon = IconFont.Camera,
                    Text = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/CapturePhoto"],
                    Option = ImageOption.Take
                },
            };

            this.Bind(WidthRequestProperty, SizeProperty.PropertyName, source: this);
            this.Bind(HeightRequestProperty, SizeProperty.PropertyName, source: this);

            FuncConverter<double, double> iconBGSizeConverter = new FuncConverter<double, double>((value) => value / 3);
            FuncConverter<double, double> frameRadiusConverter = new FuncConverter<double, double>((value) => value / 2);
            FuncConverter<double, double> iconFrameRadiusConverter = new FuncConverter<double, double>((value) => value / 3 / 2);
            FuncConverter<ImageSource, bool> imageVisibleConverter = new FuncConverter<ImageSource, bool>((value) => value != null);
            FuncConverter<ImageSource, bool> imageNotVisibleConverter = new FuncConverter<ImageSource, bool>((value) => value == null);

            Children.Add(new Grid
            {
                Children =
                {
                    new TLMenuButton
                    {
                        BindingContext = this,
                        Command = CommandBuilder.CreateFrom<MenuResult>(OnResultChosen),
                        Choices = choices,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        BackgroundColor = (Color)Xamarin.Forms.Application.Current.Resources["PrimaryLight"],
                    }.Bind(TLMenuButton.CornerRadiusProperty, SizeProperty.PropertyName, converter: frameRadiusConverter)
                        .Bind(TLMenuButton.IsVisibleProperty, SourceProperty.PropertyName, converter: imageNotVisibleConverter)
                        .Bind(TLMenuButton.IsEnabledProperty, IsEnabledProperty.PropertyName),
                    new Frame
                    {
                        HasShadow = false,
                        BindingContext = this,
                        Padding = 0,
                        Content = new CachedImage
                        {
                            DownsampleToViewSize = true,
                            BindingContext = this,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            Transformations =
                            {
                                new RoundedTransformation { Radius = 250 }
                            }
                        }.Bind(CachedImage.SourceProperty, SourceProperty.PropertyName)
                            .Bind(CachedImage.IsVisibleProperty, SourceProperty.PropertyName, converter: imageVisibleConverter)
                    }.Bind(Frame.CornerRadiusProperty, SizeProperty.PropertyName, converter: frameRadiusConverter)
                        .Bind(Frame.WidthRequestProperty, SizeProperty.PropertyName)
                        .Bind(Frame.HeightRequestProperty, SizeProperty.PropertyName)
                }
            });

            Children.Add(new TLMenuButton
            {
                BindingContext = this,
                Command = CommandBuilder.CreateFrom<MenuResult>(OnResultChosen),
                Choices = choices,
                Opacity = .8,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End,
                Padding = 10,
                BackgroundColor = (Color)Xamarin.Forms.Application.Current.Resources["GrayColor"],
                Source = new FontImageSource
                {
                    Color = Color.Black,
                    FontFamily = "FA",
                    Glyph = IconFont.Camera,
                }
            }.Bind(TLMenuButton.WidthRequestProperty, SizeProperty.PropertyName, converter: iconBGSizeConverter)
                .Bind(TLMenuButton.HeightRequestProperty, SizeProperty.PropertyName, converter: iconBGSizeConverter)
                .Bind(TLMenuButton.CornerRadiusProperty, SizeProperty.PropertyName, converter: iconFrameRadiusConverter)
                .Bind(TLMenuButton.IsVisibleProperty, IsEnabledProperty.PropertyName));
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public double Size
        {
            get => (double)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private async Task OnResultChosen(MenuResult option)
        {
            TLFileResult result;

            if (Device.RuntimePlatform == Device.iOS)
            {
                await Task.Delay(1000);
            }

            switch ((ImageOption)option.Option)
            {
                case ImageOption.Take:
                    result = await TLMediaPicker.CapturePhotoAsync();
                    break;
                case ImageOption.Pick:
                    result = await TLMediaPicker.PickPhotoAsync();
                    break;
                default:
                    throw new NotImplementedException($"Internal error: {nameof(ImageOption)} not implemented inside {nameof(OnResultChosen)}");
            }

            if (result != null)
            {
                result = result.ImageResize(ImageResizeConstants.MAX_WIDTH, ImageResizeConstants.MAX_HEIGTH, ImageResizeConstants.COMPRESSION_RATE);
                Command?.ExecuteCommand(result);
            }
        }
    }
}