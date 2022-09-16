using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using Point = Xamarin.Forms.Point;
using ScrollBarVisibility = Xamarin.Forms.ScrollBarVisibility;
using Size = Xamarin.Forms.Size;
using Thickness = Xamarin.Forms.Thickness;
using UwpScrollBarVisibility = Windows.UI.Xaml.Controls.ScrollBarVisibility;

namespace IARA.Mobile.Insp.UWP.Renderers
{
    public class CustomScrollViewRenderer : ViewRenderer<ScrollView, ScrollViewer>//, IDontGetFocus
    {
        private VisualElement _currentView;
        private bool _checkedForRtlScroll = false;

        public CustomScrollViewRenderer()
        {
            AutoPackage = false;
        }

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            SizeRequest result = base.GetDesiredSize(widthConstraint, heightConstraint);
            result.Minimum = new Size(40, 40);
            return result;
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            if (Element == null)
            {
                return finalSize;
            }

            Element.IsInNativeLayout = true;

            Control?.Arrange(new Windows.Foundation.Rect(0, 0, finalSize.Width, finalSize.Height));

            Element.IsInNativeLayout = false;

            return finalSize;
        }

        protected override void Dispose(bool disposing)
        {
            CleanUp(Element, Control);
            base.Dispose(disposing);
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            if (Element == null)
            {
                return new Windows.Foundation.Size(0, 0);
            }

            double width = Math.Max(0, Element.Width);
            double height = Math.Max(0, Element.Height);
            Windows.Foundation.Size result = new Windows.Foundation.Size(width, height);

            Control?.Measure(result);

            return result;
        }

        private void CleanUp(ScrollView scrollView, ScrollViewer scrollViewer)
        {
            if (scrollView != null)
            {
                scrollView.ScrollToRequested -= OnScrollToRequested;
            }

            if (scrollViewer != null)
            {
                scrollViewer.ViewChanged -= OnViewChanged;
                if (scrollViewer.Content is FrameworkElement element)
                {
                    element.LayoutUpdated -= SetInitialRtlPosition;
                }
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ScrollView> e)
        {
            base.OnElementChanged(e);
            CleanUp(e.OldElement, Control);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new ScrollViewer
                    {
                        HorizontalScrollBarVisibility = ScrollBarVisibilityToUwp(e.NewElement.HorizontalScrollBarVisibility),
                        VerticalScrollBarVisibility = ScrollBarVisibilityToUwp(e.NewElement.VerticalScrollBarVisibility)
                    });

                    Control.ViewChanged += OnViewChanged;
                }

                Element.ScrollToRequested += OnScrollToRequested;

                UpdateOrientation();

                UpdateContent();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "Content")
            {
                UpdateContent();
            }
            else if (e.PropertyName == Layout.PaddingProperty.PropertyName)
            {
                UpdateContentMargins();
            }
            else if (e.PropertyName == ScrollView.OrientationProperty.PropertyName)
            {
                UpdateOrientation();
            }
            else if (e.PropertyName == ScrollView.VerticalScrollBarVisibilityProperty.PropertyName)
            {
                UpdateVerticalScrollBarVisibility();
            }
            else if (e.PropertyName == ScrollView.HorizontalScrollBarVisibilityProperty.PropertyName)
            {
                UpdateHorizontalScrollBarVisibility();
            }
        }

        protected void OnContentElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == View.MarginProperty.PropertyName)
            {
                UpdateContentMargins();
            }
        }

        private void UpdateContent()
        {
            if (_currentView != null)
            {
                _currentView.Cleanup();
            }

            if (Control?.Content is FrameworkElement oldElement)
            {
                oldElement.LayoutUpdated -= SetInitialRtlPosition;

                if (oldElement is IVisualElementRenderer oldRenderer
                    && oldRenderer.Element is View oldContentView)
                {
                    oldContentView.PropertyChanged -= OnContentElementPropertyChanged;
                }
            }

            _currentView = Element.Content;

            IVisualElementRenderer renderer = null;
            if (_currentView != null)
            {
                renderer = _currentView.GetOrCreateRenderer();
            }

            Control.Content = renderer != null ? renderer.ContainerElement : null;

            UpdateContentMargins();
            if (renderer?.Element != null)
            {
                renderer.Element.PropertyChanged += OnContentElementPropertyChanged;
            }

            if (renderer?.ContainerElement != null)
            {
                renderer.ContainerElement.LayoutUpdated += SetInitialRtlPosition;
            }
        }

        private async void OnScrollToRequested(object sender, ScrollToRequestedEventArgs e)
        {
            ClearRtlScrollCheck();

            // Adding items into the view while scrolling to the end can cause it to fail, as
            // the items have not actually been laid out and return incorrect scroll position
            // values. The ScrollViewRenderer for Android does something similar by waiting up
            // to 10ms for layout to occur.
            int cycle = 0;
            while (Element != null && !Element.IsInNativeLayout)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1));
                cycle++;

                if (cycle >= 10)
                {
                    break;
                }
            }

            if (Element == null)
            {
                return;
            }

            double x = e.ScrollX, y = e.ScrollY;

            ScrollToMode mode = e.Mode;
            if (mode == ScrollToMode.Element)
            {
                Point pos = Element.GetScrollPositionForElement((VisualElement)e.Element, e.Position);
                x = pos.X;
                y = pos.Y;
                mode = ScrollToMode.Position;
            }

            if (mode == ScrollToMode.Position)
            {
                Control.ChangeView(x, y, null, !e.ShouldAnimate);
            }
            Element.SendScrollFinished();
        }

        private void SetInitialRtlPosition(object sender, object e)
        {
            if (Control == null)
            {
                return;
            }

            if (Control.ActualWidth <= 0 || _checkedForRtlScroll || Control.Content == null)
            {
                return;
            }

            if (Element is IVisualElementController controller && controller.EffectiveFlowDirection.IsLeftToRight())
            {
                ClearRtlScrollCheck();
                return;
            }

            FrameworkElement element = (Control.Content as FrameworkElement);
            if (element.ActualWidth == Control.ActualWidth)
            {
                return;
            }

            ClearRtlScrollCheck();
            Control.ChangeView(element.ActualWidth, 0, null, true);
        }

        private void ClearRtlScrollCheck()
        {
            _checkedForRtlScroll = true;
            if (Control.Content is FrameworkElement element)
            {
                element.LayoutUpdated -= SetInitialRtlPosition;
            }
        }

        private void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ClearRtlScrollCheck();
            Element.SetScrolledPosition(Control.HorizontalOffset, Control.VerticalOffset);

            if (!e.IsIntermediate)
            {
                Element.SendScrollFinished();
            }
        }

        private Windows.UI.Xaml.Thickness AddMargin(Thickness original, double left, double top, double right, double bottom)
        {
            return new Windows.UI.Xaml.Thickness(original.Left + left, original.Top + top, original.Right + right, original.Bottom + bottom);
        }

        // UAP ScrollView forces Content origin to be the same as the ScrollView origin.
        // This prevents Forms layout from emulating Padding and Margin by offsetting the origin.
        // So we must actually set the UAP Margin property instead of emulating it with an origin offset.
        // Not only that, but in UAP Padding and Margin are aliases with
        // the former living on the parent and the latter on the child.
        // So that's why the UAP Margin is set to the sum of the Forms Padding and Forms Margin.
        private void UpdateContentMargins()
        {
            if (!(Control.Content is FrameworkElement element
                && element is IVisualElementRenderer renderer
                && renderer.Element is View contentView))
            {
                return;
            }

            Thickness margin = contentView.Margin;
            Thickness padding = Element.Padding;
            switch (Element.Orientation)
            {
                case ScrollOrientation.Horizontal:
                    // need to add left/right margins
                    element.Margin = AddMargin(margin, padding.Left, 0, padding.Right, 0);
                    break;
                case ScrollOrientation.Vertical:
                    // need to add top/bottom margins
                    element.Margin = AddMargin(margin, 0, padding.Top, 0, padding.Bottom);
                    break;
                case ScrollOrientation.Both:
                    // need to add all margins
                    element.Margin = AddMargin(margin, padding.Left, padding.Top, padding.Right, padding.Bottom);
                    break;
            }
        }

        private void UpdateOrientation()
        {
            //Only update the horizontal scroll bar visibility if the user has not set a desired state.
            if (Element.HorizontalScrollBarVisibility != ScrollBarVisibility.Default)
            {
                return;
            }

            ScrollOrientation orientation = Element.Orientation;
            if (orientation == ScrollOrientation.Horizontal || orientation == ScrollOrientation.Both)
            {
                Control.HorizontalScrollBarVisibility = UwpScrollBarVisibility.Auto;
            }
            else
            {
                Control.HorizontalScrollBarVisibility = UwpScrollBarVisibility.Disabled;
            }
        }

        private UwpScrollBarVisibility ScrollBarVisibilityToUwp(ScrollBarVisibility visibility)
        {
            switch (visibility)
            {
                case ScrollBarVisibility.Always:
                    return UwpScrollBarVisibility.Visible;
                case ScrollBarVisibility.Default:
                    return UwpScrollBarVisibility.Auto;
                case ScrollBarVisibility.Never:
                    return UwpScrollBarVisibility.Hidden;
                default:
                    return UwpScrollBarVisibility.Auto;
            }
        }

        private void UpdateVerticalScrollBarVisibility()
        {
            Control.VerticalScrollBarVisibility = ScrollBarVisibilityToUwp(Element.VerticalScrollBarVisibility);
        }

        private void UpdateHorizontalScrollBarVisibility()
        {
            ScrollBarVisibility horizontalVisibility = Element.HorizontalScrollBarVisibility;

            if (horizontalVisibility == ScrollBarVisibility.Default)
            {
                UpdateOrientation();
                return;
            }

            ScrollOrientation orientation = Element.Orientation;
            if (orientation == ScrollOrientation.Horizontal || orientation == ScrollOrientation.Both)
            {
                Control.HorizontalScrollBarVisibility = ScrollBarVisibilityToUwp(horizontalVisibility);
            }
        }
    }

    public static class Extensions
    {
        internal static void Cleanup(this VisualElement self)
        {
            if (self == null)
            {
                throw new ArgumentNullException("self");
            }

            IVisualElementRenderer renderer = Platform.GetRenderer(self);

            foreach (Element element in self.Descendants())
            {
                VisualElement visual = element as VisualElement;
                if (visual == null)
                {
                    continue;
                }

                IVisualElementRenderer childRenderer = Platform.GetRenderer(visual);
                if (childRenderer != null)
                {
                    childRenderer.Dispose();
                    Platform.SetRenderer(visual, null);
                }
            }

            if (renderer != null)
            {
                renderer.Dispose();
                Platform.SetRenderer(self, null);
            }
        }
    }
}
