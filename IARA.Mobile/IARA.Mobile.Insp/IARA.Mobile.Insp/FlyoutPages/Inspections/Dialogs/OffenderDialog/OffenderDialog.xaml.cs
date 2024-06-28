using IARA.Mobile.Insp.Models;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.OffenderDialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OffenderDialog : TLBaseDialog<OffenderDialogViewModel, OffenderModel>
    {
        public OffenderDialog()
        {
            InitializeComponent();
        }
    }
}