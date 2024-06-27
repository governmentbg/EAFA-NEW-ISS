using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using System;
using TechnoLogica.Xamarin.ViewModels.Base.Models;

namespace IARA.Mobile.Insp.Models
{
    public class PingerModel : BaseModel
    {
        public int? Id { get; set; }
        public string Number { get; set; }
        public SelectNomenclatureDto Status { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }

        public static implicit operator FishingGearPingerDto(PingerModel model)
        {
            if (model.Status != null)
            {
                return new FishingGearPingerDto
                {
                    Id = model.Id,
                    Number = model.Number,
                    SelectedStatus = Enum.TryParse(model.Status.Code, out FishingGearPingerStatusesEnum markStatus)
                                ? markStatus
                                : FishingGearPingerStatusesEnum.NEW,
                    StatusId = model.Status.Id,
                    Model = model.Model,
                    Brand = model.Brand
                };
            }

            return null;
        }
    }
}
