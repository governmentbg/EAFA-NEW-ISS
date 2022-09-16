using IARA.Mobile.Pub.Application.DTObjects.CatchRecords;
using IARA.Mobile.Pub.ViewModels.FlyoutPages.CatchRecords;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages.CatchRecords
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddCatchRecordPage : BasePage<AddCatchRecordViewModel>
    {
        public AddCatchRecordPage(CatchRecordDto dto = null, bool canEdit = false, bool isAdd = false)
        {
            ViewModel.CatchRecord = dto;
            ViewModel.CanEdit = canEdit;
            ViewModel.IsAdd = isAdd;
            InitializeComponent();
        }
    }
}