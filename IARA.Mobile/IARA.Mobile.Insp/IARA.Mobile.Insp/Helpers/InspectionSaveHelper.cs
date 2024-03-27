using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Models;
using IARA.Mobile.Shared.Menu;
using IARA.Mobile.Shared.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechnoLogica.Xamarin.Controls.Base;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Helpers
{
    public static class InspectionSaveHelper
    {
        public static async Task Finish(TLForwardSections sections, IViewModelValidation validation, Func<SubmitType, Task> save)
        {
            // Show all the ValidStates or the validation won't work
            List<SectionView> sectionList = sections.Children.OfType<SectionView>().ToList();

            foreach (SectionView section in sectionList)
            {
                section.IsExpanded = true;
            }

            await Task.Delay(100);

            validation.Force();

            if (!validation.IsValid)
            {
                PermitModel.ForceError();
                PermitLicenseModel.ForceError();
                SectionView firstInvalidSection = null;

                foreach (SectionView section in sectionList)
                {
                    bool isSectionValid = IsViewValid(section, sections.BindingContext);

                    section.IsExpanded = !isSectionValid;
                    section.IsInvalid = !isSectionValid;

                    if (firstInvalidSection == null && !isSectionValid)
                    {
                        firstInvalidSection = section;
                    }
                }

                await Task.Delay(100);

                if (firstInvalidSection != null)
                {
                    await sections.ScrollToSection(firstInvalidSection);
                }

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

        private static bool IsViewValid(View view, object parentContext)
        {
            if (view.BindingContext != parentContext && view.BindingContext is TLBaseViewModel viewModel)
            {
                return !viewModel.HasValidation || viewModel.Validation.IsValid;
            }

            Type type = view.GetType();

            if (type.GetProperties().Any(f => f.Name == nameof(TLBaseValidatableView<int>.ValidState)))
            {
                object stateObj = type
                    .GetProperty(nameof(TLBaseValidatableView<int>.ValidState))
                    .GetValue(view);

                if (stateObj is IValidState validState)
                {
                    return validState.IsValid;
                }
                else
                {
                    return true;
                }
            }

            if (!(Attribute.GetCustomAttribute(type, typeof(ContentPropertyAttribute)) is ContentPropertyAttribute attr))
            {
                return true;
            }

            object content = type.GetProperties().First(f => f.Name == attr.Name).GetValue(view);

            if (content == null)
            {
                return true;
            }

            if (content is ICollection viewCollection)
            {
                foreach (View contentView in viewCollection.OfType<View>())
                {
                    if (!IsViewValid(contentView, parentContext))
                    {
                        return false;
                    }
                }
            }
            else if (content is View contentView)
            {
                return IsViewValid(contentView, parentContext);
            }

            return true;
        }
    }
}
