using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Attributes;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Helpers;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Models
{
    public class LogBookModel : ViewModel
    {
        private bool _addedByInspector;

        public LogBookModel()
        {
            AddPage = CommandBuilder.CreateFrom<string>(OnAddPage);

            Pages = new List<LogBookPageDto>();

            this.AddValidation();

            AddedByInspectorState.IsValid = true;
            EndPage.IsValid = true;

            Number.AddFakeValidation();
            PageText.AddFakeValidation();
            AddedByInspectorState.AddFakeValidation();
            EndPage.AddFakeValidation();

            EndPage.Title = TranslateExtension.Translator[nameof(GroupResourceEnum.ShipChecks) + "/EndPage"];

            PageText.HasAsterisk = true;
            PageSelect.HasAsterisk = true;
        }

        public bool AddedByInspector
        {
            get => _addedByInspector;
            set
            {
                AddedByInspectorState.Value = value;
                _addedByInspector = value;
            }
        }

        public ValidStateBool AddedByInspectorState { get; set; }
        public ValidState EndPage { get; set; }

        public InspectionLogBookDto Dto { get; set; }

        [Required]
        [MaxLength(20)]
        public ValidState Number { get; set; }

        [RequiredPageSelect(true, ErrorMessageResourceName = "Required")]
        [TLRange(1, long.MaxValue)]
        public ValidState PageText { get; set; }

        [RequiredPageSelect(false, ErrorMessageResourceName = "Required")]
        [SelectLessThanOrEqualToInt(nameof(EndPage), nameof(LogBookPageDto.PageNum), ErrorMessageResourceName = "LessThanOrEqualTo")]
        public ValidStateSelect<LogBookPageDto> PageSelect { get; set; }

        [Required]
        public ValidStateMultiToggle Corresponds { get; set; }

        [MaxLength(200)]
        public ValidState Description { get; set; }

        public List<LogBookPageDto> Pages { get; set; }

        public ICommand AddPage { get; set; }

        private void OnAddPage(string text)
        {
            PageSelect.Value = new LogBookPageDto
            {
                Id = -1,
                PageNum = text
            };
        }
    }
}
