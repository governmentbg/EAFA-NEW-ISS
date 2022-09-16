using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.ViewModels.Base;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages.News
{
    public class NewsDetailsViewModel : PageViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishStart { get; set; }
        public string MainPhotoUrl { get; set; }
        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] { GroupResourceEnum.Common };
        }

        public override Task Initialize(object sender)
        {
            return Task.CompletedTask;
        }
    }
}
