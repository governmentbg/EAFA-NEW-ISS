using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Shared.ViewModels.Models.ReportParameters;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;

namespace IARA.Mobile.Shared.ViewModels.Models
{
    public class ReportParameterModel : TLBaseViewModel
    {
        public ReportParameterModel(IReportValidation validParam)
        {
            ValidParam = validParam;

            this.AddValidation(others: new[] { validParam });
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public ReportParameterType Type { get; set; }

        public IReportValidation ValidParam { get; }
    }
}
