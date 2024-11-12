using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.SendInspectionDialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SendInspectionDialog : TLBaseDialog<SendInspectionDialogViewModel>
    {
        public SendInspectionDialog(InspectionDto inspectionDto)
        {
            InitializeComponent();

            ViewModel.Inspection = inspectionDto;
        }
    }
}