using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.ViewModels.Models;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class InspectionFileViewModel : ViewModel
    {
        public InspectionFileViewModel(FileResultModel file, List<SelectNomenclatureDto> fileTypes, int? fileTypeId = null)
        {
            File = file;

            this.AddValidation();

            if (fileTypeId.HasValue)
            {
                FileType.Value = fileTypes.Find(f => f.Id == fileTypeId.Value);
            }
            else
            {
                FileType.Value = File.ContentType.StartsWith("image")
                    ? fileTypes.Find(f => f.Code == CommonConstants.PhotoFileType)
                    : fileTypes.Find(f => f.Code == CommonConstants.NomenclatureOther);
            }
        }

        public int? Id { get; set; }
        public bool WasDeleted { get; set; }
        public bool WasAddedNow { get; set; }
        public bool IsFileSavedLocally { get; set; }

        public FileResultModel File { get; }

        [MaxLength(4000)]
        public ValidState Description { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> FileType { get; set; }
    }
}
