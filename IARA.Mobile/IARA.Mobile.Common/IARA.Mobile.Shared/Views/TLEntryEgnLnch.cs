using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Shared.Popups;
using IARA.Mobile.Shared.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.Extensions;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Views
{
    public class TLEntryEgnLnch : TLEntry
    {
        public static readonly BindableProperty IdentifierTypeProperty =
            BindableProperty.Create(nameof(IdentifierType), typeof(IdentifierTypeEnum), typeof(TLEntryEgnLnch), IdentifierTypeEnum.EGN, BindingMode.TwoWay);

        public static readonly BindableProperty EGNLabelProperty =
            BindableProperty.Create(nameof(EGNLabel), typeof(string), typeof(TLEntryEgnLnch), nameof(IdentifierTypeEnum.EGN));

        public static readonly BindableProperty LNCHLabelProperty =
            BindableProperty.Create(nameof(LNCHLabel), typeof(string), typeof(TLEntryEgnLnch), nameof(IdentifierTypeEnum.LNC));

        public TLEntryEgnLnch()
        {
            FuncConverter<IdentifierTypeEnum, string> converter = new FuncConverter<IdentifierTypeEnum, string>(
                (IdentifierTypeEnum selected) =>
                {
                    return selected switch
                    {
                        IdentifierTypeEnum.EGN => EGNLabel,
                        IdentifierTypeEnum.LNC => LNCHLabel,
                        _ => throw new NotImplementedException($"{nameof(IdentifierTypeEnum)} not implemented inside {nameof(TLEntryEgnLnch)}"),
                    };
                }
            );

            FuncConverter<IdentifierTypeEnum, double> widthConverter = new FuncConverter<IdentifierTypeEnum, double>(
                (IdentifierTypeEnum selected) =>
                {
                    string label = selected switch
                    {
                        IdentifierTypeEnum.EGN => EGNLabel,
                        IdentifierTypeEnum.LNC => LNCHLabel,
                        _ => throw new NotImplementedException($"{nameof(IdentifierTypeEnum)} not implemented inside {nameof(TLEntryEgnLnch)}"),
                    };

                    return (label.Length * (FontSize - 4)) + 18;
                }
            );

            FuncConverter<List<MenuOption>> optionsConverter = new FuncConverter<List<MenuOption>>((_) => new List<MenuOption>
            {
                new MenuOption
                {
                    Text = EGNLabel,
                    Option = IdentifierTypeEnum.EGN,
                },
                new MenuOption
                {
                    Text = LNCHLabel,
                    Option = IdentifierTypeEnum.LNC,
                },
            });

            MultiBinding optionsMultiBinding = new MultiBinding
            {
                Bindings =
                {
                    new Binding(EGNLabelProperty.PropertyName, source: this),
                    new Binding(LNCHLabelProperty.PropertyName, source: this),
                },
                Converter = new FuncMultiConverter<string, string, List<MenuOption>>((labels) =>
                {
                    return new List<MenuOption>
                    {
                        new MenuOption
                        {
                            Text = labels.Item1,
                            Option = IdentifierTypeEnum.EGN,
                        },
                        new MenuOption
                        {
                            Text = labels.Item2,
                            Option = IdentifierTypeEnum.LNC,
                        },
                    };
                }),
            };

            InnerContent = new StackLayout
            {
                BindingContext = this,
                Orientation = StackOrientation.Horizontal,
                Spacing = 0,
                Children =
                {
                    new TLInnerInput(),
                    new BoxView
                    {
                        WidthRequest = 1,
                        Triggers =
                        {
                            new DataTrigger(typeof(BoxView))
                            {
                                BindingContext = this,
                                Binding = new Binding(ValidAfterFirstTouchProperty.PropertyName),
                                Value = false,
                                Setters =
                                {
                                    new Setter
                                    {
                                        Property = BoxView.ColorProperty,
                                        Value = ErrorColor
                                    }
                                }
                            }
                        }
                    }.Bind(BoxView.ColorProperty, BorderColorProperty.PropertyName, source: this),
                    new TLMenuTextButton
                    {
                        HeightRequest = Device.RuntimePlatform switch
                        {
                            Device.UWP => 40,
                            Device.iOS => 40,
                            Device.Android => 20,
                            _ => 0,
                        },
                        Padding = 0,
                        Margin = 0,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        CornerRadius = 0,
                        BorderColor = Color.LightGray,
                        BackgroundColor = Color.Transparent,
                        TextColor = Color.Black,
                        Command = CommandBuilder.CreateFrom<MenuResult>(OnTypeChosen),
                    }.Bind(TLMenuTextButton.TextProperty, IdentifierTypeProperty.PropertyName, converter: converter, source: this)
                        .Bind(TLMenuTextButton.ChoicesProperty, optionsMultiBinding)
                        .Bind(TLMenuTextButton.WidthRequestProperty, IdentifierTypeProperty.PropertyName, converter: widthConverter, source: this),
                    new Label
                    {
                        FontFamily = "FA",
                        TextColor = Color.Black,
                        Text = IconFont.CaretDown,
                        FontSize = 15,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(0, 0, 10, 0)
                    }
                }
            };

            FrameWrapper.Padding = Device.RuntimePlatform switch
            {
                Device.UWP => new Thickness(7, 1, 0, 1),
                Device.Android => new Thickness(5, 0, 0, 0),
                Device.iOS => new Thickness(5, 0.5, 0, 0.5),
                _ => new Thickness(0),
            };

            this.Bind(
                TitleProperty,
                IdentifierTypeProperty.PropertyName,
                converter: converter,
                source: this
            );
            InnerEntry.Bind(
                Entry.MaxLengthProperty,
                IdentifierTypeProperty.PropertyName,
                convert: (IdentifierTypeEnum selected) => selected == IdentifierTypeEnum.EGN ? 10 : 20,
                source: this
            );

            ComponentTitleProperty = null;
        }

        public IdentifierTypeEnum IdentifierType
        {
            get => (IdentifierTypeEnum)GetValue(IdentifierTypeProperty);
            set => SetValue(IdentifierTypeProperty, value);
        }

        public string EGNLabel
        {
            get => (string)GetValue(EGNLabelProperty);
            set => SetValue(EGNLabelProperty, value);
        }

        public string LNCHLabel
        {
            get => (string)GetValue(LNCHLabelProperty);
            set => SetValue(LNCHLabelProperty, value);
        }

        protected override void OnValidStateValueBindingPropertyChanged(IValidState<string> validState)
        {
            base.OnValidStateValueBindingPropertyChanged(validState);

            if (ValidState is EgnLncValidState egnLncValidState)
            {
                this.Bind(IdentifierTypeProperty, nameof(EgnLncValidState.IdentifierType), source: egnLncValidState);
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == EGNLabelProperty.PropertyName)
            {
                OnPropertyChanged(TitleProperty.PropertyName);
                OnPropertyChanged(IdentifierTypeProperty.PropertyName);
            }
        }

        private void OnTypeChosen(MenuResult result)
        {
            IdentifierType = (IdentifierTypeEnum)result.Option;
        }
    }
}
