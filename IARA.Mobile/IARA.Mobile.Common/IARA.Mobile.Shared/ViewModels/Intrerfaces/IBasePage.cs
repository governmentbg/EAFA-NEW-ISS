using System.Collections.Generic;
using IARA.Mobile.Domain.Enums;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.ViewModels.Intrerfaces
{
    public interface IBasePage
    {
        List<View> TitleViews { get; }
        string Title { get; set; }
        string PageInfo { get; set; }
        GroupResourceEnum[] AddedResources { get; }
        GroupResourceEnum[] GetPageIndexes();
    }
}
