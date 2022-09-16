using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Menu
{
    public interface IFlyoutPage
    {
        IReadOnlyList<NavigationItemView> NavigationItems { get; }
        IReadOnlyDictionary<string, Func<Page>> Routes { get; }
        Action CloseFlyout { get; set; }
    }
}
