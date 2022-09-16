using System;
using System.Collections.Generic;
using System.Text;
using TechnoLogica.Xamarin.ViewModels.Base.Models;

namespace IARA.Mobile.Shared.ViewModels.Models
{
    public class ReportNodeModel : BaseModel
    {
        public int Id { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }

        public ReportModel Parent { get; set; }
    }
}
