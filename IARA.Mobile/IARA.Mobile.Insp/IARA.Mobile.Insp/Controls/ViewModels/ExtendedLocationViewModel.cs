using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;
using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.ViewModels.Models;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms.Maps;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class ExtendedLocationViewModel : ViewModel
    {
        private List<CatchZoneNomenclatureDto> _quadrants;

        public ExtendedLocationViewModel(InspectionPageViewModel inspection)
        {
            Inspection = inspection;

            LocationChosen = CommandBuilder.CreateFrom(OnLocationChosen);

            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; }

        public ValidStateLocation Location { get; set; }

        public ValidStateSelect<CatchZoneNomenclatureDto> Quadrant { get; set; }

        public ValidState Description { get; set; }

        public List<CatchZoneNomenclatureDto> Quadrants
        {
            get => _quadrants;
            private set => SetProperty(ref _quadrants, value);
        }

        public ICommand LocationChosen { get; set; }

        public void Init(List<CatchZoneNomenclatureDto> quadrants)
        {
            Quadrants = quadrants;
        }

        public void OnEdit(LocationDto location, int? catchZoneId, string description)
        {
            Location.AssignFrom(location);
            Quadrant.AssignFrom(catchZoneId, Quadrants);
            Description.AssignFrom(description);
        }

        private void OnLocationChosen()
        {
            if (Location.Value == null)
            {
                return;
            }

            Position location = new Position(DMSType.Parse(Location.Value.DMSLatitude).ToDecimal(), DMSType.Parse(Location.Value.DMSLongitude).ToDecimal());
            PointF point = new PointF(Convert.ToSingle(location.Longitude), Convert.ToSingle(location.Latitude));

            foreach (CatchZoneNomenclatureDto quadrant in Quadrants)
            {
                if (quadrant.Block.Contains(point))
                {
                    Quadrant.Value = quadrant;
                    return;
                }
            }
        }
    }
}
