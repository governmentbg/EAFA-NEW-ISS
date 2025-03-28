﻿using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.FlyoutPages.InspectionsPage;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Shared.Menu;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Base
{
    public abstract class InspectionPageViewModel : PageViewModel, IDisposable
    {
        private InspectionState _inspectionState;

        protected InspectionPageViewModel()
        {
            _inspectionState = InspectionState.Draft;

            Print = CommandBuilder.CreateFrom(OnPrint);
            ReturnForEdit = CommandBuilder.CreateFrom(OnReturnForEdit);
        }
        protected InspectionEditDto ProtectedEdit { get; set; }

        public DateTime CreatedOn { get; set; }
        public ViewActivityType ActivityType { get; set; }
        public SubmitType SubmitType { get; set; }
        public InspectionType InspectionType { get; set; }
        public bool IsLocal { get; set; }
        public bool CreatedByCurrentUser { get; set; }
        public string LocalIdentifier { get; set; }

        public InspectionState InspectionState
        {
            get => _inspectionState;
            set => SetProperty(ref _inspectionState, value);
        }

        public List<InspectionSubjectPersonnelDto> DefaultInspecterPerson => new List<InspectionSubjectPersonnelDto>
        {
            new InspectionSubjectPersonnelDto
            {
                FirstName = "",
                MiddleName = "",
                LastName = "",
            }
        };

        public ICommand SaveDraft { get; protected set; }
        public ICommand Finish { get; protected set; }
        public ICommand Print { get; }
        public ICommand ReturnForEdit { get; protected set; }


        private async Task OnReturnForEdit()
        {
            bool hasPermission = DependencyService.Resolve<IProfileTransaction>().HasPermission("InspectionLockedEdit");
            if (hasPermission || CreatedOn.AddHours(48) > DateTime.Now)
            {
                ProtectedEdit.InspectionState = InspectionState.Draft;
                string json = GetInspectionJson();

                if (ProtectedEdit.Id.Value <= 0 || string.IsNullOrEmpty(json))
                {
                    await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/SavedFailed"], App.GetResource<Color>("ErrorColor"));
                    return;
                }
                HttpResult result = await DependencyService.Resolve<IRestClient>().PostAsFormDataAsync("Inspections/SendForFurtherCorrections", new InspectionDraftDto()
                {
                    Id = ProtectedEdit.Id.Value,
                    Json = json
                });

                if (result.IsSuccessful)
                {
                    await MainNavigator.Current.GoToPageAsync(nameof(InspectionsPage));
                }
                else
                {
                    await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/InspectionLocked"], App.GetResource<Color>("ErrorColor"));
                }
            }
            else
            {
                await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/InspectionLocked"], App.GetResource<Color>("ErrorColor"));
            }
        }

        protected abstract string GetInspectionJson();

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

        protected async Task OnGetStartupData()
        {
            await TLLoadingHelper.ShowFullLoadingScreen();
            IStartupTransaction starup = DependencyService.Resolve<IStartupTransaction>();
            await starup.GetInitialData(false, null, null);
            //await OnReload();
            await TLLoadingHelper.HideFullLoadingScreen();
        }
    }

    public class EmptyCopy : InspectionPageViewModel
    {
        public EmptyCopy(ViewActivityType viewActivityType)
        {
            ActivityType = viewActivityType;
        }
        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[]
            {
                GroupResourceEnum.ShipChecks,
                GroupResourceEnum.FishingShip,
                GroupResourceEnum.CatchInspection,
                GroupResourceEnum.FishingGear,
                GroupResourceEnum.InspectedPerson,
                GroupResourceEnum.InspectedShipData,
                GroupResourceEnum.PatrolVehicle,
                GroupResourceEnum.HarbourInspection,
                GroupResourceEnum.Validation,
                GroupResourceEnum.DeclarationCatch,
            };
        }

        public override Task Initialize(object sender)
        {
            return Task.CompletedTask;
        }

        protected override string GetInspectionJson()
        {
            return string.Empty;
        }
    }
}
