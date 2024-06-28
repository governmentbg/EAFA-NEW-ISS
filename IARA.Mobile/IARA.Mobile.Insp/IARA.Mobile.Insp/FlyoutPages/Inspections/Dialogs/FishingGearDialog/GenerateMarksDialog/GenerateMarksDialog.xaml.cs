using IARA.Mobile.Insp.Models;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FishingGearDialog.GenerateMarksDialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GenerateMarksDialog : TLBaseDialog<GenerateMarksDialogViewModel, GenerateMarksModel>
    {
        public GenerateMarksDialog()
        {
            InitializeComponent();
        }
    }
}