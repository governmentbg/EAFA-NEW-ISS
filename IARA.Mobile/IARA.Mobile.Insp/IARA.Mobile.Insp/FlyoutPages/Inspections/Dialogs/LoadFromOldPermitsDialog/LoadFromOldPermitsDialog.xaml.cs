using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Models;
using System.Collections.Generic;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.LoadFromOldPermitsDialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadFromOldPermitsDialog : TLBaseDialog<LoadFromOldPermitsDialogViewModel, PermitNomenclatureDto>
    {
        public LoadFromOldPermitsDialog(int? poundNetId, int? shipId)
        {
            ViewModel.PoundNetId = poundNetId;
            ViewModel.ShipId = shipId;
            InitializeComponent();
        }
    }
}