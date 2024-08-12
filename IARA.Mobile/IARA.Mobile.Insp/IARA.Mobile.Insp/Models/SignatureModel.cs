using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Base;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Base.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.ViewModels.Models
{
    public class SignatureModel : BaseModel
    {
        private ImageSource _image;

        public SignatureModel(InspectionPageViewModel inspection)
        {
            Inspection = inspection;
        }

        public InspectionPageViewModel Inspection { get; set; }

        public ImageSource Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public string SignatureType { get; set; } = TranslateExtension.Translator[nameof(GroupResourceEnum.GeneralInfo) + "/InspectorSignature"];
    }
}
