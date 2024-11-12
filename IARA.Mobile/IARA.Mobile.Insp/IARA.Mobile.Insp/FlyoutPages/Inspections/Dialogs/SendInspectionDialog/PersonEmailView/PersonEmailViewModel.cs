using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.SendInspectionDialog.PersonEmailView
{
    public class PersonEmailViewModel : ViewModel
    {
        private InspectedEntityEmailDTO personData;
        public InspectedEntityEmailDTO PersonData
        {
            get => personData;
            set => SetProperty(ref personData, value);
        }
        public PersonEmailViewModel(InspectedEntityEmailDTO personData)
        {
            this.AddValidation();
            PersonData = personData;

            PersonType.Value = personData.Name;

            if (!string.IsNullOrEmpty(personData.Email))
            {
                Email.Value = personData.Email;
            }
            SendEmail.Value = personData.SendEmail;
        }
        public ValidState PersonType { get; set; }
        [EmailAddress]
        public ValidState Email { get; set; }
        public ValidStateBool SendEmail { get; set; }

        public bool IsValid()
        {
            Validation.Force();

            return Validation.IsValid;
        }

        public InspectedEntityEmailDTO GetPersonData()
        {
            PersonData.SendEmail = SendEmail.Value;
            PersonData.Email = Email.Value;
            return PersonData;
        }
    }
}
