using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Domain.Enums;
using System;
using Xamarin.Essentials;

namespace IARA.Mobile.Insp.Utilities
{
    public class NomenclatureDatesUtility : INomenclatureDates, INomenclatureDatesClear
    {
        private const string SharedName = nameof(NomenclatureDatesUtility);

        public DateTime? this[NomenclatureEnum nomenclature]
        {
            get
            {
                string key = nomenclature.ToString();
                return Preferences.ContainsKey(key, SharedName)
                    ? new DateTime?(Preferences.Get(key, default(DateTime), SharedName))
                    : null;
            }
            set => Preferences.Set(nomenclature.ToString(), value ?? default, SharedName);
        }

        public void Clear()
        {
            Preferences.Clear(SharedName);
        }

        public void Remove(NomenclatureEnum nomenclatureEnum)
        {
            Preferences.Remove(nomenclatureEnum.ToString(), SharedName);
        }
    }
}
