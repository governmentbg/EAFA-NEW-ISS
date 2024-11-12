using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.SendInspectionDialog.PersonEmailView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Base;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.SendInspectionDialog
{
    public class SendInspectionDialogViewModel : TLBaseDialogViewModel
    {
        public InspectionDto Inspection { get; set; }
        public SendInspectionDialogViewModel()
        {
            this.AddValidation();

            People = new TLObservableCollection<PersonEmailViewModel>();
            ChooseAll = CommandBuilder.CreateFrom(OnChooseAll);
            RemoveAll = CommandBuilder.CreateFrom(OnRemoveAll);
            Close = CommandBuilder.CreateFrom(OnClose);
            SendEmails = CommandBuilder.CreateFrom(OnSendEmails);
        }

        public override async Task Initialize(object sender)
        {
            HttpResult<List<InspectedEntityEmailDTO>> result = await DependencyService.Resolve<IRestClient>()
                .GetAsync<List<InspectedEntityEmailDTO>>("Inspections/GetInspectedEntityEmails", "Administrative", new { inspectionId = Inspection.Id });
            if (result.IsSuccessful)
            {
                People.AddRange(result.Content.Select(x => new PersonEmailViewModel(x)).ToList());
            }
        }

        public TLObservableCollection<PersonEmailViewModel> People { get; set; }

        public ICommand ChooseAll { get; set; }
        public ICommand RemoveAll { get; set; }
        public ICommand Close { get; set; }
        public ICommand SendEmails { get; set; }


        private async Task OnSendEmails()
        {
            InspectionEmailDTO inspectionEmailToSend = new InspectionEmailDTO()
            {
                InspectionId = Inspection.Id,
                InspectedEntityEmails = new List<InspectedEntityEmailDTO>()
            };
            foreach (var person in People.Items)
            {
                if (person.IsValid())
                {
                    inspectionEmailToSend.InspectedEntityEmails.Add(person.GetPersonData());
                }
                else
                {
                    return;
                }
            }
            HttpResult<Unit> result = await DependencyService.Resolve<IRestClient>()
                .PostAsFormDataAsync<Unit>(url: "Inspections/SendInspectedEntityEmailNotification", urlExtension: "Administrative", content: inspectionEmailToSend, needsAuthentication: true);

            if (result.IsSuccessful)
            {
                await HideDialog();

                await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.Inspections) + "/EmailsSend"], Color.Green);
            }
            else
            {
                await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.Inspections) + "/EmailNotSend"], App.GetResource<Color>("ErrorColor"));
            }
        }

        private Task OnClose()
        {
            return HideDialog();
        }

        private void OnRemoveAll()
        {
            foreach (var person in People.Items)
            {
                person.SendEmail.Value = false;
            }
        }

        private void OnChooseAll()
        {
            foreach (var person in People.Items)
            {
                person.SendEmail.Value = true;
            }
        }
    }
}
