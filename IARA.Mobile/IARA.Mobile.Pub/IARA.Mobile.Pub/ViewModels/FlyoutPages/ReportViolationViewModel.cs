using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.DTObjects.ReportViolations;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Pub.Views.FlyoutPages;
using IARA.Mobile.Shared.Menu;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages
{
    public class ReportViolationViewModel : PageViewModel
    {
        private Position? _location;

        public ReportViolationViewModel()
        {
            SignalTypes = new TLObservableCollection<NomenclatureDto>();
            Send = CommandBuilder.CreateFrom(OnSend);

            this.AddValidation();

            Date.Value = DateTime.Now;
        }

        [Required]
        public ValidStateSelect<NomenclatureDto> SignalType { get; set; }

        [Required]
        [StringLength(1000)]
        public ValidState Description { get; set; }

        [Required]
        [TLPhone]
        public ValidState Phone { get; set; }

        [Required]
        public ValidStateDateTime Date { get; set; }

        public Position? Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public ICommand Send { get; }

        public TLObservableCollection<NomenclatureDto> SignalTypes { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] { GroupResourceEnum.ReportViolation, GroupResourceEnum.Validation };
        }

        public override Task Initialize(object sender)
        {
            List<NomenclatureDto> signalTypes = NomenclaturesTransaction.GetViolationSignalTypes();

            SignalTypes.AddRange(signalTypes);

            return Task.CompletedTask;
        }

        private async Task OnSend()
        {
            Validation.Force();

            if (!Validation.IsValid)
            {
                return;
            }
            await TLLoadingHelper.ShowFullLoadingScreen();
            bool success = await ReportViolationTransaction.ReportViolation(new ReportViolationDto
            {
                Date = Date.Value,
                Description = Description.Value,
                Phone = Phone.Value,
                SignalType = SignalType.Value,
                Location = Location.HasValue ? new LocationDto
                {
                    Latitude = Location.Value.Latitude,
                    Longitude = Location.Value.Longitude
                } : null
            });
            await TLLoadingHelper.HideFullLoadingScreen();
            if (success)
            {
                string successMessage = TranslateExtension.Translator[nameof(GroupResourceEnum.ReportViolation) + "/SuccessfulViolationSignal"];//load resource before leave the page, bc it'll be offloaded
                await MainNavigator.Current.GoToPageAsync(nameof(HomePage));
                await TLSnackbar.Show(successMessage, Color.Green);
            }
            else
            {
                await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.ReportViolation) + "/UnsuccessfulViolationSignal"], App.GetResource<Color>("ErrorColor"));
            }
        }
    }
}
