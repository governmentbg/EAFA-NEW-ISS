using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Shared.Views;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.Extensions;
using TechnoLogica.Xamarin.Helpers;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Controls
{
    [ContentProperty(nameof(Children))]
    public class TLForwardSections : TLScrollView
    {
        private readonly StackLayout _stack;

        public TLForwardSections()
        {
            InnerContent = _stack = new StackLayout
            {
                Padding = 5,
                Spacing = 0,
            };

            TLObservableCollection<View> children = new TLObservableCollection<View>();
            children.CollectionChanged += OnChildrenCollectionChanged;
            Children = children;
        }

        public new IList<View> Children { get; }

        public Task ScrollToSection(SectionView section)
        {
            return InnerScrollView.ScrollToAsync(section, ScrollToPosition.Start, false);
        }

        private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_stack.Children.Count == 0)
            {
                _stack.Children.Insert(e.NewStartingIndex, (View)e.NewItems[0]);
                DummyPropHelper.SetTLDummy(Children[0], "0");
                return;
            }

            if (Children[Children.Count - 2] is SectionView lastSection
                && lastSection.InnerContent is StackLayout stack)
            {
                StackLayout buttonStack = new StackLayout
                {
                    HorizontalOptions = LayoutOptions.End,
                    Orientation = StackOrientation.Horizontal
                }.Bind(StackLayout.IsVisibleProperty, nameof(BindingContext) + "." + nameof(InspectionPageViewModel.ActivityType), converter: App.GetResource<IValueConverter>("IsNotReview"), source: this);

                if (_stack.Children.Count != 1)
                {
                    buttonStack.Children.Add(new Button
                    {
                        ImageSource = new FontImageSource
                        {
                            FontFamily = "FA",
                            Color = Color.White,
                            Glyph = IconFont.ArrowUp,
                            Size = 24,
                        },
                        Command = CommandBuilder.CreateFrom<SectionView>(ScrollToPreviousSection),
                        CommandParameter = lastSection
                    }.BindTranslation(Button.TextProperty, "Previous", nameof(GroupResourceEnum.Common)));
                }

                if (e.NewItems[0] is SectionView nextSection)
                {
                    buttonStack.Children.Add(new Button
                    {
                        ImageSource = new FontImageSource
                        {
                            FontFamily = "FA",
                            Color = Color.White,
                            Glyph = IconFont.ArrowDown,
                            Size = 24,
                        },
                        Command = CommandBuilder.CreateFrom<SectionView>(ScrollToNextSection),
                        CommandParameter = lastSection
                    }.BindTranslation(Button.TextProperty, "Next", nameof(GroupResourceEnum.Common))
                        .Bind(Button.IsVisibleProperty, SectionView.IsVisibleProperty.PropertyName, source: nextSection));
                }

                stack.Children.Add(buttonStack);
            }

            View currentView = (View)e.NewItems[0];

            _stack.Children.Insert(e.NewStartingIndex, currentView);

            DummyPropHelper.SetTLDummy(currentView, (Children.Count - 1).ToString());
        }

        private Task ScrollToNextSection(SectionView section)
        {
            int index = int.Parse(DummyPropHelper.GetTLDummy(section)) + 1;

            section.IsExpanded = false;

            for (; index < Children.Count; index++)
            {
                if (Children[index].IsVisible && Children[index] is SectionView nextSection)
                {
                    nextSection.IsExpanded = true;
                    return InnerScrollView.ScrollToAsync(nextSection, ScrollToPosition.Start, false);
                }
            }

            return Task.CompletedTask;
        }

        private Task ScrollToPreviousSection(SectionView section)
        {
            int index = int.Parse(DummyPropHelper.GetTLDummy(section)) - 1;

            section.IsExpanded = false;

            for (; index >= 0; index--)
            {
                if (Children[index].IsVisible && Children[index] is SectionView previousSection)
                {
                    previousSection.IsExpanded = true;
                    return InnerScrollView.ScrollToAsync(previousSection, ScrollToPosition.End, false);
                }
            }

            return Task.CompletedTask;
        }
    }
}
