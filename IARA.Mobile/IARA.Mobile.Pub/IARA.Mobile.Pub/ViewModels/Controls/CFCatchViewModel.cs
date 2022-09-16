using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Pub.Application.DTObjects.CatchRecords;
using IARA.Mobile.Pub.ViewModels.Base;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Pub.ViewModels.Controls
{
    public class CFCatchViewModel : ViewModel
    {
        public CFCatchViewModel(List<NomenclatureDto> fishTypes)
        {
            FishTypes = fishTypes;
            this.AddValidation();
        }

        public CatchRecordFishDto Catch
        {
            get => Validation.IsValid
                ? new CatchRecordFishDto
                {
                    FishType = FishType.Value,
                    Count = int.Parse(Count.Value),
                    Quantity = double.Parse(Quantity.Value)
                } : null;
            set
            {
                FishType.Value = value.FishType;
                Count.Value = value.Count.ToString();
                Quantity.Value = value.Quantity.ToString();
            }
        }

        [Required]
        public ValidStateSelect<NomenclatureDto> FishType { get; set; }

        [Required]
        [TLRange(1, 1000)]
        public ValidState Count { get; set; }

        [Required]
        [TLRange(1d, 1000d)]
        public ValidState Quantity { get; set; }

        public List<NomenclatureDto> FishTypes { get; }
    }
}
