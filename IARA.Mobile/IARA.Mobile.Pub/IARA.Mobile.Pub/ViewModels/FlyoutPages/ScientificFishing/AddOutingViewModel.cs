using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.LocalDb;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Pub.Views.FlyoutPages.ScientificFishing;
using IARA.Mobile.Shared.Menu;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.CommunityToolkit.ObjectModel;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages.ScientificFishing
{
    public class AddOutingViewModel : PageViewModel
    {
        private readonly List<SFCatchDto> _catches;
        private List<SelectNomenclatureDto> _fishTypes;

        public AddOutingViewModel()
        {
            _catches = new List<SFCatchDto>();

            AddCatch = CommandBuilder.CreateFrom(OnAddCatch);
            Catches = new ObservableRangeCollection<SFCatchDto>();
            Add = CommandBuilder.CreateFrom(OnAdd);
            RemoveCatch = CommandBuilder.CreateFrom<SFCatchDto>(OnRemoveCatch);
            EditCatch = CommandBuilder.CreateFrom<SFCatchDto>(OnEditCatch);

            this.AddValidation();

            DateOfOuting.Value = DateTime.Today;
        }

        public int Id { get; set; }
        public SFOutingDto Edit { get; set; }
        public SFPermitDto Permit { get; set; }
        public bool CanEdit { get; set; }
        public ObservableRangeCollection<SFCatchDto> Catches { get; }

        [Required]
        public ValidStateDate DateOfOuting { get; set; }

        [Required]
        [StringLength(4000)]
        public ValidState WaterArea { get; set; }

        public ICommand AddCatch { get; }
        public ICommand EditCatch { get; }
        public ICommand RemoveCatch { get; }
        public ICommand Add { get; }

        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] { GroupResourceEnum.AddOuting, GroupResourceEnum.AddFishToOuting, GroupResourceEnum.Validation };
        }

        public override Task Initialize(object sender)
        {
            if (Edit != null)
            {
                DateOfOuting.Value = Edit.DateOfOuting;
                WaterArea.Value = Edit.WaterArea;
                Catches.AddRange(Edit.Catches);
                _catches.AddRange(Edit.Catches);
            }

            _fishTypes = NomenclaturesTransaction.GetFishTypes()
                .ConvertAll(f => new SelectNomenclatureDto
                {
                    Id = f.Value,
                    Name = f.DisplayName
                });

            return Task.CompletedTask;
        }

        private async Task OnAddCatch()
        {
            if (_fishTypes == null)
            {
                return;
            }

            SFCatchDto sfCatch = await TLDialogHelper.ShowDialog(new AddFishToOutingView(_fishTypes, null)
            {
                Title = TranslateExtension.Translator[nameof(GroupResourceEnum.AddFishToOuting) + "/Title"]
            });

            if (sfCatch == null)
            {
                return;
            }

            _catches.Add(sfCatch);
            Catches.Add(sfCatch);
        }

        private Task OnEditCatch(SFCatchDto sfCatch)
        {
            if (_fishTypes == null || !CanEdit)
            {
                return Task.CompletedTask;
            }

            return TLDialogHelper.ShowDialog(new AddFishToOutingView(_fishTypes, sfCatch)
            {
                Title = TranslateExtension.Translator[nameof(GroupResourceEnum.AddFishToOuting) + "/TitleEdit"]
            });
        }

        private async Task OnRemoveCatch(SFCatchDto sfCatch)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DeleteConfirm"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Delete"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Cancel"]
            );
            if (result)
            {
                Catches.Remove(sfCatch);
                sfCatch.Result = DtoResultEnum.Deleted;
            }
        }

        private async Task OnAdd()
        {
            Validation.Force();

            if (Catches.Count == 0 || !Validation.IsValid)
            {
                return;
            }

            if (Edit != null)
            {
                await ScientificFishingTransaction.EditOuting(new SFOutingDto
                {
                    WaterArea = WaterArea.Value,
                    DateOfOuting = DateOfOuting.Value.Value,
                    PermitId = Id,
                    Catches = _catches,
                    Id = Edit.Id
                });
            }
            else
            {
                await ScientificFishingTransaction.AddOuting(new SFOutingDto
                {
                    WaterArea = WaterArea.Value,
                    DateOfOuting = DateOfOuting.Value.Value,
                    PermitId = Id,
                    Catches = _catches
                });
            }
            await MainNavigator.Current.PopPageAsync();
        }
    }
}
