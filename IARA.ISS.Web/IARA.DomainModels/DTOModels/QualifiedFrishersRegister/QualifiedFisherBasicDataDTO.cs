using System;
using System.Collections.Generic;
using System.Text;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.QualifiedFrishersRegister
{
    public class QualifiedFisherBasicDataDTO
    {
        public int? Id { get; set; }
        public EgnLncDTO Identifier { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }
    }
}
