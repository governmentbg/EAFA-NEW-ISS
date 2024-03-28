namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    public class FaFishingGearCharacteristics : IEquatable<FaFishingGearCharacteristics>
    {
        // Код на уреда
        public string TypeCode { get; set; }

        // Описание на уреда (УСР)
        public string GD { get; set; }

        // Дължина или широчина на уреда
        public decimal? GM { get; set; }

        // Брой куки
        public decimal? GN { get; set; }

        // Височина
        public decimal? HE { get; set; }

        // Размер на окото
        public decimal? ME { get; set; }

        // Модел на трал
        public string MT { get; set; }

        // Брой линии
        public decimal? NI { get; set; }

        // Номинална дължина на мрежа
        public decimal? NL { get; set; }

        // Брой мрежи във флота
        public decimal? NN { get; set; }

        // Брой уреди
        public decimal? QG { get; set; }

        public static bool operator ==(FaFishingGearCharacteristics lhs, FaFishingGearCharacteristics rhs)
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if (lhs is null)
            {
                return false;
            }

            if (rhs is null)
            {
                return false;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(FaFishingGearCharacteristics lhs, FaFishingGearCharacteristics rhs)
        {
            return !(lhs == rhs);
        }

        public bool Equals(FaFishingGearCharacteristics rhs)
        {
            if (rhs is null)
            {
                return false;
            }

            if (ReferenceEquals(this, rhs))
            {
                return true;
            }

            return (TypeCode ?? "") == (rhs.TypeCode ?? "") &&
                   (GD ?? "") == (rhs.GD ?? "") &&
                   GM == rhs.GM &&
                   GN == rhs.GN &&
                   HE == rhs.HE &&
                   ME == rhs.ME &&
                   (MT ?? "") == (rhs.MT ?? "") &&
                   NI == rhs.NI &&
                   NL == rhs.NL &&
                   NN == rhs.NN &&
                   QG == rhs.QG;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FaFishingGearCharacteristics);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(HashCode.Combine(GD, GM, GN, HE, ME), HashCode.Combine(MT, NI, NL, NN, QG));
        }
    }
}
