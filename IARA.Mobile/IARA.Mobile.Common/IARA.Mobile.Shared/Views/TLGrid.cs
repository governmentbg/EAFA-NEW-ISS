using System.Collections;
using System.Collections.Specialized;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Views
{
    public class TLGrid : Grid
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IList), typeof(TLGrid), propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(TLGrid));

        public IList ItemsSource
        {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        private void CollectionChangedCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Children.Clear();
            RowDefinitions.Clear();
            DataTemplate template = ItemTemplate;
            IList items = ItemsSource;

            for (int i = 0; i < items.Count; i++)
            {
                RowDefinitions.Add(new RowDefinition
                {
                    Height = GridLength.Auto
                });

                View view = (View)template.CreateContent();
                view.BindingContext = items[i];
                SetRow(view, i);
                Children.Add(view);
            }

            Device.InvokeOnMainThreadAsync(InvalidateLayout);
        }

        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is TLGrid grid))
            {
                return;
            }

            if (oldValue is INotifyCollectionChanged collectionChanged1)
            {
                collectionChanged1.CollectionChanged -= grid.CollectionChangedCollectionChanged;
            }

            if (newValue is INotifyCollectionChanged collectionChanged)
            {
                collectionChanged.CollectionChanged += grid.CollectionChangedCollectionChanged;
            }
        }
    }
}
