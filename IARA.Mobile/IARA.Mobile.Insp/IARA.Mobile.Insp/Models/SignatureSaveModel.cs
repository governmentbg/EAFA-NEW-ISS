using TechnoLogica.Xamarin.ViewModels.Base.Models;

namespace IARA.Mobile.Insp.ViewModels.Models
{
    public class SignatureSaveModel : BaseModel
    {
        public string Caption { get; set; }
        public string DoesNotWantToSignMessage { get; set; }
        public int FileTypeId { get; set; }
        public byte[] Signature { get; set; }
    }
}
