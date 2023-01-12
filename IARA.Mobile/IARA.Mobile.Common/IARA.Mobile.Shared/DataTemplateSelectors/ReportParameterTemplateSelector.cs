using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.Extensions;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Shared.ViewModels;
using IARA.Mobile.Shared.ViewModels.Models;
using IARA.Mobile.Shared.ViewModels.Models.ReportParameters;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.DataTemplates.Base;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.DataTemplateSelectors
{
    public class ReportParameterTemplateSelector : BaseDataTemplateSelector<ReportParameterModel, ReportViewModel>
    {
        public override Func<object> FromTemplate(ReportParameterModel item, BindableObject container, ReportViewModel bindingContext)
        {
            return () =>
            {
                switch (item.Type)
                {
                    case ReportParameterType.Month:
                        return new TLNativePicker
                        {
                            Title = item.Name,
                            ItemsSource = GenerateMonths(),
                            ValidState = item.ValidParam.ValidState as IValidState<object>
                        };
                    case ReportParameterType.Year:
                    case ReportParameterType.Int:
                    case ReportParameterType.Decimal:
                        return new TLEntry
                        {
                            Title = item.Name,
                            Keyboard = Keyboard.Numeric,
                            ValidState = item.ValidParam.ValidState as IValidState<string>
                        };
                    case ReportParameterType.Date:
                        return new TLDatePicker
                        {
                            Title = item.Name,
                            ValidState = item.ValidParam.ValidState as IValidState<DateTime?>
                        };
                    case ReportParameterType.Time:
                        return new TLTimePicker
                        {
                            Title = item.Name,
                            ValidState = item.ValidParam.ValidState as IValidState<TimeSpan?>
                        };
                    case ReportParameterType.DateTime:
                        return new TLDateTimePicker
                        {
                            Title = item.Name,
                            ValidState = item.ValidParam.ValidState as IValidState<DateTime?>
                        };
                    case ReportParameterType.String:
                        return new TLEntry
                        {
                            Title = item.Name,
                            ValidState = item.ValidParam.ValidState as IValidState<string>
                        };
                    case ReportParameterType.Nomenclature:
                        NomenclatureReportValidation nomReportParam = item.ValidParam as NomenclatureReportValidation;
                        return new TLPicker
                        {
                            Title = item.Name,
                            ValidState = nomReportParam.ValidState,
                            ItemsSource = nomReportParam.ItemsSource,
                        };
                    case ReportParameterType.NomenclatureMultiSelect:
                        NomenclatureMultiSelectReportValidation nomMultiReportParam = item.ValidParam as NomenclatureMultiSelectReportValidation;
                        return new TLMultiPicker
                        {
                            Title = item.Name,
                            ValidState = nomMultiReportParam.ValidState,
                            ItemsSource = nomMultiReportParam.ItemsSource,
                        };
                    default:
                        return null;
                }
            };
        }

        private IList GenerateMonths()
        {
            List<SelectNomenclatureDto> list = new List<SelectNomenclatureDto>();

            DateTime date = new DateTime(2000, 1, 1);

            for (int i = 1; i <= 12; i++)
            {
                list.Add(new SelectNomenclatureDto
                {
                    Id = i,
                    Name = date.AddMonths(i - 1).ToString("MMMM", CultureInfo.CurrentUICulture).CapitalizeFirstLetter()
                });
            }

            return list;
        }
    }
}
