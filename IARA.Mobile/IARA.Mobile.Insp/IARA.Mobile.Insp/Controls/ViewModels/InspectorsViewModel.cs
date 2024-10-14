using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Attributes;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.InspectorDialog;
using IARA.Mobile.Insp.Models;
using IARA.Mobile.Shared.Popups;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms.Internals;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class InspectorsViewModel : ViewModel
    {
        private bool hasRecentInspectors;
        private List<MenuOption> _recentInspectors;

        public InspectorsViewModel(InspectionPageViewModel inspection, InspectionGeneralInfoViewModel generalInfo)
        {
            Inspection = inspection;
            GeneralInfo = generalInfo;

            RecentInspectorChosen = CommandBuilder.CreateFrom<MenuResult>(OnRecentInspectorChosen);
            Review = CommandBuilder.CreateFrom<InspectorModel>(OnReview);
            Add = CommandBuilder.CreateFrom(OnAdd);
            Edit = CommandBuilder.CreateFrom<InspectorModel>(OnEdit);
            Remove = CommandBuilder.CreateFrom<InspectorModel>(OnRemove);

            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; }
        public InspectionGeneralInfoViewModel GeneralInfo { get; }

        [ListMinLength(1)]
        public ValidStateTable<InspectorModel> Inspectors { get; set; }

        public bool HasRecentInspectors
        {
            get => hasRecentInspectors;
            set => SetProperty(ref hasRecentInspectors, value);
        }
        public List<MenuOption> RecentInspectors
        {
            get => _recentInspectors;
            set => SetProperty(ref _recentInspectors, value);
        }

        public ICommand RecentInspectorChosen { get; }
        public ICommand Review { get; }
        public ICommand Add { get; }
        public ICommand Edit { get; }
        public ICommand Remove { get; }

        public async Task Init()
        {
            int currentUserId = CurrentUser.Id;

            List<RecentInspectorDto> recentInspectors = InspectionsTransaction.GetRecentInspectors();

            if (recentInspectors.Count > 0)
            {
                RecentInspectors = recentInspectors.ConvertAll(f => new MenuOption
                {
                    Text = f.Inspectors,
                    Option = f,
                });
                HasRecentInspectors = true;
            }

            InspectorDuringInspectionDto currentInspector = InspectionsTransaction.GetInspectorByUserId(currentUserId);

            if (currentInspector != null)
            {
                currentInspector.IsInCharge = true;
                await OnEdit(new List<InspectorDuringInspectionDto> { currentInspector }, true);
            }
        }

        public async Task OnEdit(List<InspectorDuringInspectionDto> inspectors, bool changeNumber)
        {
            if (inspectors == null || inspectors.Count == 0)
            {
                return;
            }

            int currentUserId = CurrentUser.Id;

            List<SelectNomenclatureDto> insitutions = NomenclaturesTransaction.GetInstitutions();

            Inspectors.Value.ReplaceRange(inspectors.ConvertAll(f => new InspectorModel
            {
                Dto = f,
                Institution = insitutions.Find(s => s.Id == f.InstitutionId)?.Code,
                IsCurrentInspector = f.UserId == currentUserId,
                HasIdentified = f.HasIdentifiedHimself,
                IsInCharge = f.IsInCharge,
            }));

            if (changeNumber)
            {
                await ChangeReportNumber(Inspectors.Value.First(f => f.IsInCharge));
            }
        }

        private async Task OnRecentInspectorChosen(MenuResult result)
        {
            RecentInspectorDto dto = result.Option as RecentInspectorDto;
            List<InspectorDuringInspectionDto> inspectors = InspectionsTransaction.GetInspectorHistory(dto.Id);

            if (inspectors.Count == 0)
            {
                return;
            }

            int? inspectorInChargeId = inspectors.Where(f => f.IsInCharge).FirstOrDefault().InspectorId.Value;
            if (inspectorInChargeId != null)
            {
                var inspectorInCharge = inspectors.Where(f => f.InspectorId == inspectorInChargeId).FirstOrDefault();
                if (inspectorInCharge != null)
                {
                    inspectors.Where(x => x.IsInCharge).ForEach(x => x.IsInCharge = false);
                    inspectorInCharge.IsInCharge = true;
                }
                else
                {
                    inspectors[0].IsInCharge = true;
                }
            }
            else
            {
                inspectors[0].IsInCharge = true;
            }

            await OnEdit(inspectors, true);
        }

        private Task OnReview(InspectorModel model)
        {
            return TLDialogHelper.ShowDialog(new InspectorDialog(this, Inspection, ViewActivityType.Review, model));
        }

        private async Task OnAdd()
        {
            InspectorModel result = await TLDialogHelper.ShowDialog(new InspectorDialog(this, Inspection, ViewActivityType.Add));

            if (result != null && result.Dto != null)
            {
                if (result.IsInCharge)
                {
                    foreach (InspectorModel inspector in Inspectors)
                    {
                        inspector.IsInCharge = false;
                        inspector.Dto.IsInCharge = false;
                    }

                    await ChangeReportNumber(result);
                }

                Inspectors.Value.Add(result);

                if (!Inspectors.Any(f => f.IsInCharge))
                {
                    InspectorModel inspector = Inspectors.Value[0];
                    inspector.IsInCharge = true;
                    inspector.Dto.IsInCharge = true;

                    await ChangeReportNumber(inspector);
                }
            }
        }

        private async Task OnEdit(InspectorModel model)
        {
            InspectorModel result = await TLDialogHelper.ShowDialog(new InspectorDialog(this, Inspection, ViewActivityType.Edit, model));

            if (result != null && result.Dto != null)
            {
                if (result.IsInCharge)
                {
                    foreach (InspectorModel inspector in Inspectors)
                    {
                        if (inspector != model)
                        {
                            inspector.Dto.IsInCharge = false;
                            inspector.IsInCharge = false;
                        }
                    }

                    await ChangeReportNumber(result);
                }

                model.IsInCharge = result.IsInCharge;
                model.HasIdentified = result.HasIdentified;
                model.Institution = result.Institution;
                model.Dto = result.Dto;

                Inspectors.Value.Replace(model, result);

                if (!Inspectors.Any(f => f.IsInCharge))
                {
                    InspectorModel inspector = Inspectors.Value[0];
                    inspector.IsInCharge = true;
                    inspector.Dto.IsInCharge = true;

                    await ChangeReportNumber(inspector);
                }
            }
        }

        private async Task OnRemove(InspectorModel model)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DeleteMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
            );

            if (result)
            {
                Inspectors.Value.Remove(model);

                if (Inspectors.Value.Count > 0 && !Inspectors.Any(f => f.IsInCharge))
                {
                    InspectorModel inspector = Inspectors.Value[0];
                    inspector.IsInCharge = true;
                    inspector.Dto.IsInCharge = true;

                    await ChangeReportNumber(inspector);
                }
            }
        }

        private async Task ChangeReportNumber(InspectorModel inspector)
        {
            InspectorInfoDto info = InspectionsTransaction.GetInspectorInfo(inspector.Dto.InspectorId.Value);
            string nextReportNum = await InspectionsTransaction.GetNextReportNumber(inspector.Dto.UserId.Value);

            GeneralInfo.ChangeReportNum(info.TerritoryCode, info.CardNum, nextReportNum);
        }

        public static implicit operator List<InspectorDuringInspectionDto>(InspectorsViewModel viewModel)
        {
            return viewModel.Inspectors
                .Select(f => f.Dto)
                .ToList();
        }
    }
}
