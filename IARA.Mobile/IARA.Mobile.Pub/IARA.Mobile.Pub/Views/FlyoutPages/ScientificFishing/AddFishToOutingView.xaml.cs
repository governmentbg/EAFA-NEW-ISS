using System.Collections.Generic;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.LocalDb;
using IARA.Mobile.Pub.ViewModels.FlyoutPages.ScientificFishing;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages.ScientificFishing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddFishToOutingView : TLBaseDialog<AddFishToOutingViewModel, SFCatchDto>
    {
        public AddFishToOutingView(List<SelectNomenclatureDto> fishTypes, SFCatchDto sFCatch)
        {
            ViewModel.SFCatch = sFCatch;
            ViewModel.FishTypes = fishTypes;
            InitializeComponent();
        }
    }
}