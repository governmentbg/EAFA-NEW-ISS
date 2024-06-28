using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Insp.Attributes;
using IARA.Mobile.Insp.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FishingGearDialog.GenerateMarksDialog
{
    public class GenerateMarksDialogViewModel : TLBaseDialogViewModel<GenerateMarksModel>
    {
        public GenerateMarksDialogViewModel()
        {
            Save = CommandBuilder.CreateFrom(OnSave);
            this.AddValidation();
        }

        [Required]
        [TLRange(1, 100000000)]
        public ValidState From { get; set; }

        [Required]
        [TLRange(1, 100000000)]
        [BiggerThanOrEqualToInt(nameof(From), ErrorMessageResourceName = "FromMustBeSmallerThanTo")]
        [DifferenceBetweenTwoIntAttributesLowerThan(nameof(From), 500, ErrorMessageResourceName = "DifferenceBetweenFromAndTo")]
        public ValidState To { get; set; }
        public ICommand Save { get; set; }

        public override Task Initialize(object sender)
        {
            return Task.CompletedTask;
        }
        private void OnSave()
        {
            Validation.Force();
            if (!Validation.IsValid)
            {
                return;
            }

            HideDialog(new GenerateMarksModel()
            {
                From = int.Parse(From.Value),
                To = int.Parse(To.Value)
            });
        }
    }
}
