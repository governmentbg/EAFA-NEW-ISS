using TechnoLogica.Xamarin.ViewModels.Interfaces;

namespace IARA.Mobile.Shared.ViewModels.Models.ReportParameters
{
    public interface IReportValidation : IValidatableViewModel
    {
        IValidState ValidState { get; }

        string GetValue();
    }
}
