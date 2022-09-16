using System;
using TechnoLogica.Xamarin.ViewModels.Base.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.ViewModels.Models
{
    public class CatchImageModel : BaseModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string FullPath { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadedOn { get; set; }
        public int FileTypeId { get; set; }

        public bool WasAddedNow { get; set; }
        public bool WasDeleted { get; set; }
        public bool IsFileSavedLocally { get; set; }

        public ImageSource Image { get; set; }
    }
}
