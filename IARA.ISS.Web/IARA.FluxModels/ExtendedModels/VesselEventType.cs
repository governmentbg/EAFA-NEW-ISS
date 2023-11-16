using System;
using System.Linq;

namespace IARA.Flux.Models
{
    public partial class VesselEventType
    {
        public override int GetHashCode()
        {
            return HashCode.Combine(
                RelatedVesselTransportMeans.ID.Where(x => x.schemeID == "CFR").First().Value.GetHashCode(),
                ((DateTime)OccurrenceDateTime).Date.GetHashCode()
            );
        }

        public override bool Equals(object other)
        {
            if (other == null)
            {
                return false;
            }

            if (other is VesselEventType rhs)
            {
                return RelatedVesselTransportMeans.ID.Where(x => x.schemeID == "CFR").First().Value == rhs.RelatedVesselTransportMeans.ID.Where(x => x.schemeID == "CFR").First().Value
                    && ((DateTime)OccurrenceDateTime).Date == ((DateTime)rhs.OccurrenceDateTime).Date;
            }

            return false;
        }
    }
}
