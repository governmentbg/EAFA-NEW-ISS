using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.Extensions;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.WaterCatchDialog
{
    public class WaterCatchViewModel : TLBaseDialogViewModel<WaterCatchModel>
    {
        private bool _isTaken;
        private SelectNomenclatureDto _action;
        private List<SelectNomenclatureDto> _fishes;
        private bool _showErrorText;

        public WaterCatchViewModel()
        {

            Save = CommandBuilder.CreateFrom(OnSave);

            Actions = new List<SelectNomenclatureDto>
            {
                new SelectNomenclatureDto
                {
                    Id = 1,
                    Code = nameof(CatchActionEnum.Stored),
                    Name = TranslateExtension.Translator[nameof(GroupResourceEnum.WaterCatch) + "/Stored"],
                },
                new SelectNomenclatureDto
                {
                    Id = 2,
                    Code = nameof(CatchActionEnum.Returned),
                    Name = TranslateExtension.Translator[nameof(GroupResourceEnum.WaterCatch) + "/Returned"],
                },
                new SelectNomenclatureDto
                {
                    Id = 3,
                    Code = nameof(CatchActionEnum.Donated),
                    Name = TranslateExtension.Translator[nameof(GroupResourceEnum.WaterCatch) + "/Donated"],
                },
                new SelectNomenclatureDto
                {
                    Id = 4,
                    Code = nameof(CatchActionEnum.Destroyed),
                    Name = TranslateExtension.Translator[nameof(GroupResourceEnum.WaterCatch) + "/Destroyed"],
                },
            };

            Action = Actions.First();

            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; set; }

        public WaterCatchesViewModel WaterCatches { get; set; }

        public WaterCatchModel Edit { get; set; }

        public ViewActivityType DialogType { get; set; }

        public int? Id { get; set; }

        public bool IsTaken
        {
            get => _isTaken;
            set => SetProperty(ref _isTaken, value);
        }
        public bool ShowErrorText
        {
            get { return _showErrorText; }
            set { _showErrorText = value; }
        }

        public SelectNomenclatureDto Action
        {
            get => _action;
            set => SetProperty(ref _action, value);
        }

        public ValidStateSelect<SelectNomenclatureDto> Fish { get; set; }

        [TLRange(0, 1000, true)]
        public ValidState Quantity { get; set; }

        [MaxLength(500)]
        public ValidState Location { get; set; }
        public List<SelectNomenclatureDto> Actions { get; set; }

        public List<SelectNomenclatureDto> Fishes
        {
            get => _fishes;
            set => SetProperty(ref _fishes, value);
        }

        public ICommand Save { get; }

        public override Task Initialize(object sender)
        {
            INomenclatureTransaction nomTransaction = DependencyService.Resolve<INomenclatureTransaction>();

            Fishes = nomTransaction.GetFishes();

            if (Edit != null)
            {
                Id = Edit.Dto.Id;
                IsTaken = Edit.Dto.IsTaken ?? false;
                Fish.AssignFrom(Edit.Dto.FishId, Fishes);
                Quantity.AssignFrom(Edit.Dto.CatchQuantity);
                Location.AssignFrom(Edit.Dto.StorageLocation);

                Action = Edit.Dto.Action.HasValue
                    ? Actions.Find(f => f.Code == Edit.Dto.Action.Value.ToString())
                    : null;

                if (DialogType == ViewActivityType.Edit)
                {
                    Validation.Force();
                }
            }

            return Task.CompletedTask;
        }

        private Task OnSave()
        {
            Validation.Force();

            if (!Validation.IsValid)
            {
                return Task.CompletedTask;
            }


            return HideDialog(new WaterCatchModel
            {
                FishName = Fish.Value == null ? "" : Fish.Value.DisplayValue,
                Dto = new InspectionCatchMeasureDto
                {
                    Id = Id,
                    FishId = Fish.Value,
                    IsTaken = IsTaken,
                    CatchQuantity = ParseHelper.ParseDecimal(Quantity.Value),
                    Action = EnumExtensions.Parse<CatchActionEnum>(Action?.Code),
                    StorageLocation = Location,
                }
            });
        }
    }
}
