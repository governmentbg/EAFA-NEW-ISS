using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Shared.Menu;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Helpers
{
    public static class InspectionSaveHelper
    {
        public static async Task Finish(Action expandAll, IViewModelValidation validation, Func<SubmitType, Task> save)
        {
            // Show all the ValidStates or the validation won't work
            expandAll?.Invoke();
            await Task.Delay(100);

            validation.Force();

            if (!validation.IsValid)
            {
                MessageOptions options = new MessageOptions
                {
                    Message = TranslateExtension.Translator[nameof(GroupResourceEnum.GeneralInfo) + "/InvalidValidation"]
                };

                if (Device.RuntimePlatform == Device.UWP)
                {
                    options.Padding = 10;
                }

                await TLSnackbar.Show(new SnackBarOptions
                {
                    MessageOptions = options,
                    BackgroundColor = App.GetResource<Color>("ErrorColor"),
                    Duration = TimeSpan.FromSeconds(5)
                });
                return;
            }

            if (Device.RuntimePlatform == Device.UWP)
            {
                await save(SubmitType.Finish);
            }
            else
            {
                await save(SubmitType.Finish);
            }
        }

        public static async Task Save(this InspectionPageViewModel inspectionViewModel, InspectionEditDto edit, InspectionFilesViewModel filesViewModel, Func<string, List<FileModel>, Task<PostEnum>> post)
        {
            await TLLoadingHelper.ShowFullLoadingScreen();

            string inspectionIdentifier = edit?.LocalIdentifier ?? inspectionViewModel.LocalIdentifier;

            if (inspectionIdentifier != null)
            {
                InspectionFilesHelper.DeleteFiles(inspectionIdentifier);
            }
            else
            {
                inspectionIdentifier = Guid.NewGuid().ToString("N");
            }

            inspectionViewModel.LocalIdentifier = inspectionIdentifier;

            List<FileModel> files = await InspectionFilesHelper.HandleAllFiles(
                filesViewModel.ListOfFiles,
                inspectionIdentifier
            );

            PostEnum postResult = await post(inspectionIdentifier, files);

            if (postResult == PostEnum.Offline)
            {
                await MainNavigator.Current.PopPageAsync();
                await TLLoadingHelper.HideFullLoadingScreen();
                await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/SavedOffline"], Color.Green);
            }
            else if (postResult == PostEnum.Success)
            {
                InspectionFilesHelper.DeleteFiles(inspectionIdentifier);
                await MainNavigator.Current.PopPageAsync();
                await TLLoadingHelper.HideFullLoadingScreen();
                await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/SavedSuccessfully"], Color.Green);
            }
            else
            {
                inspectionViewModel.Validation.Force();
                await TLLoadingHelper.HideFullLoadingScreen();
                await TLSnackbar.Show(TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/SavedFailed"], App.GetResource<Color>("ErrorColor"));
            }
        }
    }
}
