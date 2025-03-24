using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Domain.Enums;
using System;
using Xamarin.Essentials;

namespace IARA.Mobile.Pub.Utilities
{
    public class NomenclatureDatesUtility : INomenclatureDates, INomenclatureDatesClear
    {
        private const string SharedName = nameof(NomenclatureDatesUtility);

        public DateTime? this[NomenclatureEnum nomenclature]
        {
            get => Preferences.Get(nomenclature.ToString(), default(DateTime), SharedName);
            set => Preferences.Set(nomenclature.ToString(), value ?? default, SharedName);
        }

        public void Clear()
        {
            Preferences.Clear(SharedName);
        }

        public void Remove(string nomenclatureEnumString)
        {
            Preferences.Remove(nomenclatureEnumString, SharedName);
        }
    }
}
