﻿using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Converters
{
    public class IdToPresentationNameConverter : IValueConverter
    {
        public static List<SelectNomenclatureDto> Presentations { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Presentations == null)
            {
                Presentations = DependencyService.Resolve<INomenclatureTransaction>().GetFishPresentations();
            }

            if (value is int id)
            {
                SelectNomenclatureDto nomenclature = Presentations.Where(x => x.Id == id).FirstOrDefault();
                if (nomenclature != null)
                {
                    return nomenclature.DisplayValue;
                }
            }
            return "not found";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}