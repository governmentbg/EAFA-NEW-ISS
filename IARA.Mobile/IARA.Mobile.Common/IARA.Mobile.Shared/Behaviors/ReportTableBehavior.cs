using IARA.Mobile.Application.DTObjects.Reports;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using TechnoLogica.Xamarin.Behaviors.Base;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.Helpers;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Behaviors
{
    public class ReportTableBehavior : BaseBindableBehavior<TLResponsiveTable>
    {
        public static readonly BindableProperty ColumnNamesProperty =
            BindableProperty.Create(nameof(ColumnNames), typeof(TLObservableCollection<ReportColumnNameDto>), typeof(ReportTableBehavior),
                propertyChanged: OnColumnNamesPropertyChanged);

        public TLObservableCollection<ReportColumnNameDto> ColumnNames
        {
            get => (TLObservableCollection<ReportColumnNameDto>)GetValue(ColumnNamesProperty);
            set => SetValue(ColumnNamesProperty, value);
        }

        private void OnColumnNamesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            TLObservableCollection<ReportColumnNameDto> columnNames = ColumnNames;

            AssociatedObject.TableColumns = new TLObservableCollection<TLTableColumn>(columnNames
                .Select(f => new TLTableColumn
                {
                    Text = f.PropertyDisplayName,
                    Width = GridLength.Star
                })
                .ToList());

            IValueConverter converter = new FuncConverter<Dictionary<string, object>, string, string>(
                (dict, parameter) => dict?[parameter]?.ToString()
            );

            AssociatedObject.DefaultItemTemplate = new DataTemplate(() =>
            {
                TLTableRow row = new TLTableRow();

                for (int i = 0; i < columnNames.Count; i++)
                {
                    row.Columns.Add(new Label
                    {
                        LineBreakMode = LineBreakMode.WordWrap
                    }.Bind(Label.TextProperty, converter: converter, converterParameter: columnNames[i].PropertyName));
                }

                return row;
            });

            AssociatedObject.DefaultSectionItemTemplate = new DataTemplate(() =>
            {
                TLAutoGrid autoGrid = new TLAutoGrid
                {
                    UnfilledColumns = TLAutoGrid.UnfilledColumnsEnum.LeaveEmpty,
                    DefaultMinWidth = 500,
                    ColumnSpacing = 20,
                };

                for (int i = 0; i < columnNames.Count; i++)
                {
                    ReportColumnNameDto column = columnNames[i];

                    autoGrid.Children.Add(new TLRichLabel
                    {
                        Spans =
                        {
                            new Span
                            {
                                Text = column.PropertyDisplayName,
                            },
                            new Span
                            {
                                Text = ": ",
                            },
                            new Span()
                                .Bind(Span.TextProperty, converter: converter, converterParameter: column.PropertyName)
                        },
                        LineBreakMode = LineBreakMode.WordWrap
                    });
                }

                return new Frame
                {
                    BorderColor = Color.LightGray,
                    BackgroundColor = Color.Transparent,
                    Padding = 10,
                    HasShadow = false,
                    Content = autoGrid,
                };
            });
        }

        private static void OnColumnNamesPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is ReportTableBehavior reportTable) || !(newValue is TLObservableCollection<ReportColumnNameDto> reportColumns))
            {
                return;
            }

            reportColumns.CollectionChanged += reportTable.OnColumnNamesCollectionChanged;
        }
    }
}
