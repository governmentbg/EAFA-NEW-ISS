﻿using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using TechnoLogica.Xamarin.ViewModels.Base.Models;

namespace IARA.Mobile.Insp.Models
{
    public class WaterCatchModel : BaseAssignableModel<WaterCatchModel>
    {
        public string FishName { get; set; }

        public InspectionCatchMeasureDto Dto { get; set; }
    }
}
