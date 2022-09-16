using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.User
{
    public class ChangeUserDataDTO : RegixPersonDataDTO
    {
        public int Id { get; set; }//PersonId

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public List<AddressRegistrationDTO> UserAddresses { get; set; }
    }
}
