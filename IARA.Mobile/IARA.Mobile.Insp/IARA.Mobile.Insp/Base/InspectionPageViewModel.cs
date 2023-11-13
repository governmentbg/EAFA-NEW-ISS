using System;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace IARA.Mobile.Insp.Base
{
    public abstract class InspectionPageViewModel : PageViewModel, IDisposable
    {
        private InspectionState _inspectionState;

        protected InspectionPageViewModel()
        {
            _inspectionState = InspectionState.Draft;

            Print = CommandBuilder.CreateFrom(OnPrint);
        }
        protected InspectionEditDto ProtectedEdit { get; set; }

        public ViewActivityType ActivityType { get; set; }
        public SubmitType SubmitType { get; set; }
        public bool IsLocal { get; set; }

        public string LocalIdentifier { get; set; }

        public InspectionState InspectionState
        {
            get => _inspectionState;
            set => SetProperty(ref _inspectionState, value);
        }

        public ICommand SaveDraft { get; protected set; }
        public ICommand Finish { get; protected set; }
        public ICommand Print { get; }
        public ICommand ReturnForEdit { get; protected set; }

        private async Task OnPrint()
        {
            const string group = nameof(GroupResourceEnum.Common);

            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[group + "/PrintInspectionMessage"],
                TranslateExtension.Translator[group + "/Yes"],
                TranslateExtension.Translator[group + "/No"]
            );

            if (result)
            {
                await Downloader.DownloadFile(ProtectedEdit.ReportNum + ".pdf", "application/pdf", "Inspections/DownloadReport", new { inspectionId = ProtectedEdit.Id.Value });
            }
        }

        public virtual void Dispose()
        {
            InspectionHelper.Dispose(this);
        }
    }
}
