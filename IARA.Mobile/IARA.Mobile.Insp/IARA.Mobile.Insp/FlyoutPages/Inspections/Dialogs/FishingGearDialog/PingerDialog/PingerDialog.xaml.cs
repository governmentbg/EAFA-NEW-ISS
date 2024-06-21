using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Models;
using System.Collections.Generic;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FishingGearDialog.PingerDialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PingerDialog : TLBaseDialog<PingerDialogViewModel, PingerModel>
    {
        public PingerDialog(PingerModel pingerModel, ViewActivityType viewActivityType, List<SelectNomenclatureDto> pingerStatuses)
        {
            ViewModel.Pinger = pingerModel;
            ViewModel.ViewActivityType = viewActivityType;
            ViewModel.IsEditable = viewActivityType != ViewActivityType.Review;
            ViewModel.PingerStatuses = pingerStatuses;

            InitializeComponent();
        }
    }
}