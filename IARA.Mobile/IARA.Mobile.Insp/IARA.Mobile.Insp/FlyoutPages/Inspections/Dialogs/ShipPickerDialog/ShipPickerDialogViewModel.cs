using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Application.Interfaces.Transactions;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Shared.ResourceTranslator;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.ShipPickerDialog
{
    public class ShipPickerDialogViewModel : TLBaseDialogViewModel<(string, VesselDuringInspectionDto)>
    {
        public ShipPickerDialogViewModel()
        {
            Cancel = CommandBuilder.CreateFrom(OnCancel);
            Pick = CommandBuilder.CreateFrom(OnPick);
        }

        public InspectedShipDataViewModel Ship { get; private set; }

        public VesselDuringInspectionDto SelectedShip { get; set; }

        public ICommand Cancel { get; set; }
        public ICommand Pick { get; set; }

        public void OnInit(InspectionPageViewModel inspection)
        {
            GroupResourceEnum[] filtered = Translator.Current
                .Filter(new[] { GroupResourceEnum.InspectedShipData })
                .ToArray();

            IReadOnlyDictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>> resources =
                DependencyService.Resolve<ITranslationTransaction>().GetPagesTranslations(filtered);

            Translator.Current.Add(resources);

            Ship = new InspectedShipDataViewModel(inspection, false);

            this.AddValidation(others: new[] { Ship });

            Ship.ShipSelected = CommandBuilder.CreateFrom<ShipSelectNomenclatureDto>(OnShipSelected);
        }

        public override Task Initialize(object sender)
        {
            INomenclatureTransaction nomTransaction = DependencyService.Resolve<INomenclatureTransaction>();

            Ship.Init(nomTransaction.GetCountries(), nomTransaction.GetVesselTypes(), nomTransaction.GetCatchZones());
            Ship.OnEdit(SelectedShip);

            return Task.CompletedTask;
        }

        private Task OnCancel()
        {
            return HideDialog((null, null));
        }

        private Task OnPick()
        {
            return HideDialog((Ship.GetShortName(), Ship));
        }

        private void OnShipSelected(ShipSelectNomenclatureDto ship)
        {
            INomenclatureTransaction nomTransaction = DependencyService.Resolve<INomenclatureTransaction>();

            ShipDto chosenShip = nomTransaction.GetShip(ship.Id);

            if (chosenShip == null)
            {
                return;
            }

            Ship.InspectedShip = chosenShip;

            Ship.CallSign.AssignFrom(chosenShip.CallSign);
            Ship.MMSI.AssignFrom(chosenShip.MMSI);
            Ship.CFR.AssignFrom(chosenShip.CFR);
            Ship.ExternalMarkings.AssignFrom(chosenShip.ExtMarkings);
            Ship.Name.AssignFrom(chosenShip.Name);
            Ship.UVI.AssignFrom(chosenShip.UVI);
            Ship.Flag.AssignFrom(chosenShip.FlagId, Ship.Flags);
            Ship.ShipType.AssignFrom(chosenShip.ShipTypeId, Ship.ShipTypes);
        }
    }
}
