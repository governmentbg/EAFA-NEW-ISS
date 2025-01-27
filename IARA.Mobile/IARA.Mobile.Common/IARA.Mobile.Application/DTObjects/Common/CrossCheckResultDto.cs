using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.Mobile.Application.DTObjects.Common
{
    public class CrossCheckResultDto
    {
        public string PageCode { get; set; }
        public int TableId { get; set; }
        public string ErrorDescription { get; set; }


        public int Id { get; set; }

        public string CheckCode { get; set; }

        public string CheckName { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public int? AssignedUserId { get; set; }

        public string AssignedUser { get; set; }

        public string Resolution { get; set; }

        public bool IsActive { get; set; }

        public DateTime? AssignedOn { get; set; } // For mobile
    }
}
