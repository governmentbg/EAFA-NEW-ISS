using System.Collections.Generic;
using System.Windows.Input;
using IARA.Mobile.Shared.Views;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.Extensions;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.Views.Controls
{
    public class FilePickerView : Frame
    {
        public static readonly BindableProperty FileResultProperty =
            BindableProperty.Create(nameof(FileResult), typeof(TLFileResult), typeof(FilePickerView));

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(FilePickerView));

        public static readonly BindableProperty PickTextProperty =
            BindableProperty.Create(nameof(PickText), typeof(string), typeof(FilePickerView));

        public static readonly BindableProperty KilobytesTextProperty =
            BindableProperty.Create(nameof(KilobytesText), typeof(string), typeof(FilePickerView));

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(FilePickerView));

        public FilePickerView()
        {
            CornerRadius = 5;

            FuncConverter<TLFileResult, View> filePickedConverter = new FuncConverter<TLFileResult, View>(file =>
            {
                if (file == null)
                {
                    Padding = 0;
                    HasShadow = false;
                    BorderColor = Color.Transparent;

                    return new TLButton
                    {
                        BindingContext = this,
                        ImageSource = new FontImageSource
                        {
                            FontFamily = "FA",
                            Glyph = IconFont.Upload,
                            Color = Color.White
                        },
                        Command = CommandBuilder.CreateFrom(() =>
                        {
                            if (IsEnabled)
                            {
                                Command?.ExecuteCommand(null);
                            }
                        })
                    }.Bind(Button.TextProperty, PickTextProperty.PropertyName);
                }

                Padding = 10;
                HasShadow = true;
                BorderColor = Color.LightGray;

                return new Grid
                {
                    GestureRecognizers =
                    {
                        new TapGestureRecognizer
                        {
                            Command = CommandBuilder.CreateFrom(() => Command?.Execute(null)),
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
                            StyleClass = new List<string> { "BaseIcon" },
                            FontSize = 40,
                            Text = IconFont.Image
                        },
                        new StackLayout
                        {
                            Spacing = 0,
                            Children =
                            {
                                new Label
                                {
                                    TextColor = Color.White,
                                    FontAttributes = FontAttributes.Bold
                                }.Bind(Label.TextProperty, TitleProperty.PropertyName, source: this),
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
                                            Text = file.FullPath.Substring(file.FullPath.LastIndexOf('/') + 1)
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
                                                    .Bind(Span.TextProperty, KilobytesTextProperty.PropertyName, source: this)
                                            }
                                        }.Column(1)
                                    }
                                }
                            }
                        }.Column(1)
                    }
                }.Bind(Button.IsEnabledProperty, IsEnabledProperty.PropertyName, source: this);
            });

            this.Bind(Frame.ContentProperty, FileResultProperty.PropertyName, source: this, converter: filePickedConverter);
        }

        public TLFileResult FileResult
        {
            get => (TLFileResult)GetValue(FileResultProperty);
            set => SetValue(FileResultProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public string PickText
        {
            get => (string)GetValue(PickTextProperty);
            set => SetValue(PickTextProperty, value);
        }

        public string KilobytesText
        {
            get => (string)GetValue(KilobytesTextProperty);
            set => SetValue(KilobytesTextProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
    }
}
