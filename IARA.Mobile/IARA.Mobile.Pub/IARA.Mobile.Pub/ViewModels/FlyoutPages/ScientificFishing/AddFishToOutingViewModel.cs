using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.LocalDb;
using IARA.Mobile.Pub.Attributes;
using IARA.Mobile.Pub.ViewModels.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages.ScientificFishing
{
    public class AddFishToOutingViewModel : BaseDialogViewModel<SFCatchDto>
    {
        private const int MaxCoughtFish = 1000;

        public AddFishToOutingViewModel()
        {
            Save = CommandBuilder.CreateFrom(OnSave);
            CatchChanged = CommandBuilder.CreateFrom(OnCatchChanged);

            this.AddValidation();
        }

        public SFCatchDto SFCatch { get; set; }

        public List<SelectNomenclatureDto> FishTypes { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> FishType { get; set; }

        [Required]
        [TLRange(0, MaxCoughtFish)]
        public ValidState CatchUnder100 { get; set; }

        [Required]
        [TLRange(0, MaxCoughtFish)]
        public ValidState Catch100To500 { get; set; }

        [Required]
        [TLRange(0, MaxCoughtFish)]
        public ValidState Catch500To1000 { get; set; }

        [Required]
        [TLRange(0, MaxCoughtFish)]
        public ValidState CatchOver1000 { get; set; }

        [Required]
        [TLRange(0, MaxCoughtFish)]
        [LessThanOrEqualToInt(nameof(TotalCatch))]
        public ValidState TotalKeptCount { get; set; }

        [Required]
        [TLRange(0, MaxCoughtFish)]
        public ValidState TotalCatch { get; set; }

        public ICommand CatchChanged { get; }
        public ICommand Save { get; }

        public override Task Initialize(object sender)
        {
            if (SFCatch == null)
            {
                return Task.CompletedTask;
            }

            FishType.Value = new SelectNomenclatureDto
            {
                Id = SFCatch.FishType.Value,
                Name = SFCatch.FishType.DisplayName
            };
            CatchUnder100.Value = SFCatch.CatchUnder100.ToString();
            Catch100To500.Value = SFCatch.Catch100To500.ToString();
            Catch500To1000.Value = SFCatch.Catch500To1000.ToString();
            CatchOver1000.Value = SFCatch.CatchOver1000.ToString();
            TotalKeptCount.Value = SFCatch.TotalKeptCount.ToString();
            TotalCatch.Value = SFCatch.TotalCatch.ToString();

            return Task.CompletedTask;
        }

        private void OnCatchChanged()
        {
            TotalCatch.Value = (
                TryParse(Catch100To500)
                + TryParse(Catch500To1000)
                + TryParse(CatchOver1000)
                + TryParse(CatchUnder100)
            ).ToString();
        }

        private Task OnSave()
        {
            Validation.Force();

            if (!Validation.IsValid)
            {
                return Task.CompletedTask;
            }

            if (SFCatch != null)
            {
                SFCatch.FishType = new NomenclatureDto
                {
                    DisplayName = FishType.Value.Name,
                    Value = FishType.Value.Id
                };
                SFCatch.Catch100To500 = int.Parse(Catch100To500.Value);
                SFCatch.Catch500To1000 = int.Parse(Catch500To1000.Value);
                SFCatch.CatchOver1000 = int.Parse(CatchOver1000.Value);
                SFCatch.CatchUnder100 = int.Parse(CatchUnder100.Value);
                SFCatch.TotalCatch = int.Parse(TotalCatch.Value);
                SFCatch.TotalKeptCount = int.Parse(TotalKeptCount.Value);
                if (SFCatch.Result != DtoResultEnum.Added)
                {
                    SFCatch.Result = DtoResultEnum.Updated;
                }
                return HideDialog(null);
            }

            return HideDialog(new SFCatchDto
            {
                FishType = new NomenclatureDto
                {
                    DisplayName = FishType.Value.Name,
                    Value = FishType.Value.Id
                },
                Catch100To500 = int.Parse(Catch100To500.Value),
                Catch500To1000 = int.Parse(Catch500To1000.Value),
                CatchOver1000 = int.Parse(CatchOver1000.Value),
                CatchUnder100 = int.Parse(CatchUnder100.Value),
                TotalCatch = int.Parse(TotalCatch.Value),
                TotalKeptCount = int.Parse(TotalKeptCount.Value),
                Result = DtoResultEnum.Added
            });
        }

        private int TryParse(ValidState validState)
        {
            return int.TryParse(validState.Value, out int value)
                ? value
                : 0;
        }
    }
}
