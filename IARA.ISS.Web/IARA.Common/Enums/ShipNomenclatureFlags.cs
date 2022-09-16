namespace IARA.Common.Enums
{
    public enum ShipNomenclatureFlags : ushort
    {
        None = 0,
        ThirdPartyShip = 1 << 0,
        FishingCapacity = 1 << 1,
        Forbidden = 1 << 2,
        BlackSeaPermit = 1 << 3,
        DanubePermit = 1 << 4,
        BlackSeaPermitAppl = 1 << 5,
        DanubePermitAppl = 1 << 6,
        PoundNetPermit = 1 << 7,
        PoundNetPermitAppl = 1 << 8,
        ActiveFishQuota = 1 << 9,
        DestOrDereg = 1 << 10
    }
}
