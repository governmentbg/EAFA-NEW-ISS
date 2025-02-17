using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using IARA.Mobile.Insp.ViewModels.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Attributes;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.LoadFromOldPermitsDialog
{
    public class LoadFromOldPermitsDialogViewModel : TLBaseDialogViewModel<PermitNomenclatureDto>
    {
        public LoadFromOldPermitsDialogViewModel()
        {
            this.AddValidation();
            Permit.ItemsSource = new TLObservableCollection<PermitNomenclatureDto>();
            Save = CommandBuilder.CreateFrom(OnSave);
        }

        [Required]
        public ValidStateInfiniteSelect<PermitNomenclatureDto> Permit { get; set; }
        public ICommand Save { get; set; }

        public int? PoundNetId { get; set; }
        public int? ShipId { get; set; }

        public override Task Initialize(object sender)
        {
            INomenclatureTransaction nomenclatureTransaction = DependencyService.Resolve<INomenclatureTransaction>();
            if (PoundNetId != null)
            {
                Permit.GetMore = (int page, int pageSize, string search) =>
                    nomenclatureTransaction.GetPoundNetPermits(PoundNetId.Value, page, pageSize, search);
                Permit.ItemsSource.ReplaceRange(
                    nomenclatureTransaction.GetPoundNetPermits(PoundNetId.Value, 0, CommonGlobalVariables.PullItemsCount)
                );
            }

            if (ShipId != null)
            {
                Permit.GetMore = (int page, int pageSize, string search) =>
                    nomenclatureTransaction.GetPermits(ShipId.Value, page, pageSize, search);
                Permit.ItemsSource.ReplaceRange(
                    nomenclatureTransaction.GetPermits(ShipId.Value, 0, CommonGlobalVariables.PullItemsCount)
                );
            }
            return Task.CompletedTask;
        }

        private Task OnSave()
        {
            Validation.Force();
            if (!Validation.IsValid)
            {
                return Task.CompletedTask;
            }

            return HideDialog(Permit.Value);
        }
    }
}
