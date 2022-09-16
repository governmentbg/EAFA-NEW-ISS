using System;
using IARA.FluxModels;

namespace IARA.Flux.Models
{
    public partial class IDType 
    {
        public static IDType GenerateGuid()
        {
            return new IDType
            {
                schemeID = IDTypes.UUID,
                Value = Guid.NewGuid().ToString()
            };
        }

        public static IDType CreateCFR(string cfr)
        {
            return new IDType
            {
                schemeID = IDTypes.CFR,
                Value = cfr
            };
        }

        public static IDType CreateUVI(string uvi)
        {
            return new IDType
            {
                schemeID = IDTypes.UVI,
                Value = uvi
            };
        }

        public static IDType CreateFromGuid(Guid guid)
        {
            return new IDType
            {
                schemeID = IDTypes.UUID,
                Value = guid.ToString()
            };
        }

        public static implicit operator IDType(Guid value)
        {
            return CreateFromGuid(value);
        }

        public static explicit operator Guid(IDType value)
        {
            return Guid.Parse(value.Value);
        }

        public static IDType CreateMMSI(string mmsi)
        {
            return new IDType
            {
                schemeID = IDTypes.MMSI,
                Value = mmsi
            };
        }

        public static IDType CreateParty(string value)
        {
            return new IDType
            {
                schemeID = ListIDTypes.FLUX_GP_PARTY,
                Value = value
            };
        }

        public static IDType CreateID(string type, string value)
        {
            return new IDType
            {
                schemeID = type,
                Value = value
            };
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                IDType id2 = (IDType)obj;
                return this.schemeID == id2.schemeID && this.Value == id2.Value;
            }            
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }
    }
}
