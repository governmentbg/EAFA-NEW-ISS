using System;
using System.Collections.Generic;
using System.Text;
using TechnoLogica.Xamarin.Helpers;

namespace IARA.Mobile.Shared.ViewModels.Models
{
    public class ReportModel : TLObservableCollection<ReportNodeModel>
    {
        private bool _isExpanded = true;

        public int Id { get; set; }
        public string Name { get; set; }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                OnPropertyChanged(nameof(IsExpanded));
            }
        }

        public List<ReportNodeModel> HiddenChildren { get; set; }
    }
}
