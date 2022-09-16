using IARA.Mobile.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportsPage : BasePage<ReportsViewModel>
    {
        public ReportsPage()
        {
            InitializeComponent();
        }
    }
}