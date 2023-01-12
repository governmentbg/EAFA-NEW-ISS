using System.Windows.Input;
using IARA.Mobile.Shared.Helpers;
using TechnoLogica.Xamarin.Helpers;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Views
{
    public class TLPagination : StackLayout
    {
        public static readonly BindableProperty PagedItemsSourceProperty =
            BindableProperty.Create(nameof(PagedItemsSource), typeof(IPagedCollection), typeof(TLPagination), propertyChanged: OnPagedItemsSourceChanged);

        public static readonly BindableProperty PageProperty =
            BindableProperty.Create(nameof(Page), typeof(int), typeof(TLPagination), propertyChanged: OnPagnationPropertyChanged);

        public static readonly BindableProperty PageCountProperty =
            BindableProperty.Create(nameof(PageCount), typeof(int), typeof(TLPagination), propertyChanged: OnPagnationPropertyChanged);

        public static readonly BindableProperty GoToPageProperty =
            BindableProperty.Create(nameof(GoToPage), typeof(ICommand), typeof(TLPagination));

        public TLPagination()
        {
            Orientation = StackOrientation.Horizontal;

            BuildPages();

            // Only visible when there are more then 0 pages
            this.Bind(IsVisibleProperty, PageCountProperty.PropertyName, convert: (int pc) => pc != 0, source: this);
        }

        public IPagedCollection PagedItemsSource
        {
            get => (IPagedCollection)GetValue(PagedItemsSourceProperty);
            set => SetValue(PagedItemsSourceProperty, value);
        }

        public int Page
        {
            get => (int)GetValue(PageProperty);
            set => SetValue(PageProperty, value);
        }

        public int PageCount
        {
            get => (int)GetValue(PageCountProperty);
            set => SetValue(PageCountProperty, value);
        }

        public ICommand GoToPage
        {
            get => (ICommand)GetValue(GoToPageProperty);
            set => SetValue(GoToPageProperty, value);
        }

        private void BuildPages()
        {
            Children.Clear();

            int page = Page;
            int pageCount = PageCount;

            if (pageCount == 0 || page == 0 || page > pageCount)
            {
                return;
            }

            AddAngle(page > 1, true, page);

            AddPages(page, pageCount);

            AddAngle(pageCount > page, false, page);
        }

        private void AddPages(int page, int pageCount)
        {
            (int startPageRange, int endPageRange) = CalculatePageRange(page, pageCount);

            if (startPageRange > 1)
            {
                AddExtraPage(1);

                if (startPageRange > 3)
                {
                    AddDots();
                }
                else if (startPageRange > 2)
                {
                    AddExtraPage(2);
                }
            }

            for (int currentPage = startPageRange; currentPage <= endPageRange; currentPage++)
            {
                bool isSelected = currentPage == page;

                Color textColor = isSelected
                    ? Color.White
                    : Color.Gray;

                Color backgroundColor = isSelected
                    ? Color.FromHex("#0078c2")
                    : Color.White;

                View pageView = CreatePage(textColor, backgroundColor, currentPage.ToString());

                if (!isSelected)
                {
                    pageView.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        CommandParameter = currentPage
                    }.Bind(TapGestureRecognizer.CommandProperty, GoToPageProperty.PropertyName, source: this));
                }

                Children.Add(pageView);
            }

            if (endPageRange < pageCount)
            {
                if (endPageRange < pageCount - 2)
                {
                    AddDots();
                }
                else if (endPageRange < pageCount - 1)
                {
                    AddExtraPage(pageCount - 1);
                }

                AddExtraPage(pageCount);
            }
        }

        private void AddExtraPage(int page)
        {
            View pageView = CreatePage(Color.Gray, Color.White, page.ToString());

            pageView.GestureRecognizers.Add(new TapGestureRecognizer
            {
                CommandParameter = page
            }.Bind(TapGestureRecognizer.CommandProperty, GoToPageProperty.PropertyName, source: this));

            Children.Add(pageView);
        }

        private void AddDots()
        {
            Children.Add(new Frame
            {
                HasShadow = false,
                CornerRadius = 3,
                Content = new Label
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    TextColor = Color.Gray,
                    Text = "...",
                    FontSize = 20,
                },
                Padding = 0,
                WidthRequest = 35,
                HeightRequest = 35,
                BackgroundColor = Color.White,
            });
        }

        private void AddAngle(bool isEnabled, bool isLeft, int selectedPage)
        {
            View angle = CreatePage(
                isEnabled ? Color.Gray : Color.LightGray,
                Color.White,
                isLeft ? IconFont.AngleLeft : IconFont.AngleRight,
                "FA"
            );

            if (isEnabled)
            {
                angle.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    CommandParameter = selectedPage + (isLeft ? -1 : 1)
                }.Bind(TapGestureRecognizer.CommandProperty, GoToPageProperty.PropertyName, source: this));
            }
            else
            {
                angle.IsEnabled = false;
            }

            Children.Add(angle);
        }

        private View CreatePage(Color textColor, Color backgroundColor, string text, string fontFamily = null)
        {
            Label label = new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TextColor = textColor,
                Text = text,
                FontSize = 20,
            };

            if (fontFamily != null)
            {
                label.FontFamily = fontFamily;
            }

            return new Frame
            {
                HasShadow = false,
                CornerRadius = 3,
                Content = label,
                Padding = 0,
                WidthRequest = 35,
                HeightRequest = 35,
                BackgroundColor = backgroundColor,
            };
        }

        private (int, int) CalculatePageRange(int page, int pageCount)
        {
            int startPageRange = page - 1;
            int endPageRange = page + 1;

            bool startPageOutOfRange = startPageRange == 0;
            bool endPageOutOfRange = endPageRange >= pageCount;

            if (startPageOutOfRange && endPageOutOfRange)
            {
                startPageRange = 1;
                endPageRange = 1;
            }
            else if (startPageOutOfRange)
            {
                startPageRange = 1;
                endPageRange++;

                if (endPageRange >= pageCount)
                {
                    endPageRange = pageCount;
                }
            }
            else if (endPageOutOfRange)
            {
                startPageRange--;
                endPageRange = pageCount;

                if (startPageRange == 0)
                {
                    startPageRange = 1;
                }
            }

            return (startPageRange, endPageRange);
        }

        private static void OnPagedItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is TLPagination pagnation))
            {
                return;
            }

            if (newValue is IPagedCollection collection)
            {
                pagnation.Bind(PageProperty, nameof(IPagedCollection.Page), source: collection);
                pagnation.Bind(PageCountProperty, nameof(IPagedCollection.PageCount), source: collection);
                pagnation.Bind(GoToPageProperty, nameof(IPagedCollection.GoToPage), source: collection);
            }
        }

        private static void OnPagnationPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is TLPagination pagnation)
            {
                pagnation.BuildPages();
            }
        }
    }
}
