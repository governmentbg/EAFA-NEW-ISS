using IARA.Mobile.Domain.Models;

namespace IARA.Mobile.Application.DTObjects.Profile.API
{
    public class EditProfileApiDto : BaseProfileApiDto
    {
        public FileModel Photo { get; set; }
    }
}
