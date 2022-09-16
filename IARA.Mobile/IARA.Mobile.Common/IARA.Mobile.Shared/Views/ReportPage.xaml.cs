using IARA.Mobile.Application.DTObjects.Reports;
using IARA.Mobile.Shared.ViewModels;
using System.Collections.Generic;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportPage : BasePage<ReportViewModel>
    {
        public ReportPage(ReportDto report, List<(string, object)> defaultValues = null)
        {
            ViewModel.Report = report;
            ViewModel.Init(defaultValues);
            InitializeComponent();
        }
    }
}