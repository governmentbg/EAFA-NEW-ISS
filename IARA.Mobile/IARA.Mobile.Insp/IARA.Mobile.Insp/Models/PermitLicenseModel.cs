using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Models
{
    public class PermitLicenseModel : ViewModel, IDisposable
    {
        private bool isDisposed = false;
        public static List<PermitLicenseModel> Instances { get; set; } = new List<PermitLicenseModel>();
        public PermitLicenseModel()
        {
            this.AddValidation();

            LicenseNumber.AddFakeValidation();

            Instances.Add(this);
            Corresponds.PropertyChanged += UpdateShowError;
        }
        ~PermitLicenseModel()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                Instances.Remove(this);
                Corresponds.PropertyChanged -= UpdateShowError;
            }
        }

        private void UpdateShowError(object sender, PropertyChangedEventArgs e)
        {
            ShowError = Validation.WasForced && !Corresponds.IsValid;
        }

        private bool showError;
        public bool ShowError
        {
            get => showError ? !Corresponds.IsValid : false;
            set
            {
                SetProperty(ref showError, value, nameof(ShowError));
            }
        }

        public bool AddedByInspector { get; set; }

        public InspectionPermitDto Dto { get; set; }

        [Required]
        [MaxLength(20)]
        public ValidState LicenseNumber { get; set; }

        [Required]
        public ValidStateMultiToggle Corresponds { get; set; }

        [MaxLength(200)]
        public ValidState Description { get; set; }


        public static void ForceError()
        {
            foreach (var instance in Instances)
            {
                instance.ShowError = instance.Validation.WasForced && !instance.Corresponds.IsValid;
            }
        }
    }
}
