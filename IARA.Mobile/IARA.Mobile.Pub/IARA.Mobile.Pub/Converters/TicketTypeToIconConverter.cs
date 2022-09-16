using IARA.Mobile.Pub.Application.DTObjects.FishingTickets.LocalDb;
using IARA.Mobile.Pub.Domain.Enums;
using TechnoLogica.Xamarin.Converters.Base;
using TechnoLogica.Xamarin.Helpers;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.Converters
{
    public class TicketTypeToIconConverter : BaseValueConverter<ImageSource, TicketTypeDto>
    {
        public override ImageSource ConvertTo(TicketTypeDto value)
        {
            switch (value?.Code)
            {
                case nameof(TicketTypeEnum.ASSOCIATION):
                    return FromFA(IconFont.IdCard);
                case nameof(TicketTypeEnum.DISABILITY):
                    return FromFA(IconFont.Wheelchair);
                case nameof(TicketTypeEnum.ELDER):
                case nameof(TicketTypeEnum.ELDERASSOCIATION):
                    return FromFA(IconFont.BookOpenReader);
                case nameof(TicketTypeEnum.BETWEEN14AND18):
                case nameof(TicketTypeEnum.BETWEEN14AND18ASSOCIATION):
                    return FromFA(IconFont.Child);
                case nameof(TicketTypeEnum.STANDARD):
                    return FromFA(IconFont.User);
                case nameof(TicketTypeEnum.UNDER14):
                    return FromFA(IconFont.Baby);
                default:
                    return FromFA(IconFont.User);
            }
        }

        private ImageSource FromFA(string iconFont)
        {
            return new FontImageSource
            {
                FontFamily = "FA",
                Glyph = iconFont,
                Size = 100,
                Color = Color.White
            };
        }
    }
}
