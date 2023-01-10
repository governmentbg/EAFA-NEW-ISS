using TechnoLogica.Xamarin.Commands;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Views
{
    [ContentProperty(nameof(InnerContent))]
    public class SectionView : Frame
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(SectionView), string.Empty);

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(SectionView), 18d);

        public static readonly BindableProperty ImageProperty =
            BindableProperty.Create(nameof(Image), typeof(ImageSource), typeof(SectionView));

        public static readonly BindableProperty IsExpandedProperty =
            BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(SectionView), true);

        public static readonly BindableProperty IsInvalidProperty =
            BindableProperty.Create(nameof(IsInvalid), typeof(bool), typeof(SectionView), false);

        private readonly StackLayout _stack;
        private bool _hasInnerContent;
        private View _header;

        public SectionView()
        {
            Padding = 0;
            Margin = 5;
            CornerRadius = 10;
            BackgroundColor = Color.White;
            _header = new SectionHeaderView
            {
                BindingContext = this
            };

            Content = _stack = new StackLayout
            {
                Spacing = 0,
                Padding = 0,
                Margin = 0,
                Children =
                {
                    new ContentView
                    {
                        GestureRecognizers =
                        {
                            new TapGestureRecognizer
                            {
                                Command = CommandBuilder.CreateFrom(() => IsExpanded = !IsExpanded)
                            }
                        },
                    }.Bind(ContentView.ContentProperty, nameof(Header), source: this)
                }
            };
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }

        public ImageSource Image
        {
            get => (ImageSource)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public View InnerContent
        {
            get => _hasInnerContent ? _stack.Children[1] : null;
            set
            {
                if (_hasInnerContent)
                {
                    _stack.Children.RemoveAt(1);
                }

                value.Bind(View.BindingContextProperty, BindingContextProperty.PropertyName, source: this);
                value.Bind(View.IsVisibleProperty, IsExpandedProperty.PropertyName, source: this);

                _stack.Children.Add(value);
                _hasInnerContent = true;
                OnPropertyChanged(nameof(InnerContent));
            }
        }

        public View Header
        {
            get => _header;
            set
            {
                _header = value;
                OnPropertyChanged(nameof(Header));
            }
        }

        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        public bool IsInvalid
        {
            get => (bool)GetValue(IsInvalidProperty);
            set => SetValue(IsInvalidProperty, value);
        }
    }
}
