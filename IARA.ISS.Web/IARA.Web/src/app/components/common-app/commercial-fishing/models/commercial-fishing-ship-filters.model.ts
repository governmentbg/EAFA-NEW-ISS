export class CommercialFishingShipFilters {
    public hasBlackSeaPermit: boolean | undefined;
    public hasDanubePermit: boolean | undefined;

    public hasBlackSeaPermitApplication: boolean | undefined;
    public hasDanubePermitApplication: boolean | undefined;

    public hasPoundNetPermit: boolean | undefined;
    public hasPoundNetPermitApplication: boolean | undefined;

    public isThirdCountryShip: boolean | undefined;
    public isDestroyedOrDeregistered: boolean | undefined;

    public hasActiveFishQuota: boolean | undefined;

    public isForbiddenForLicenses: boolean | undefined;

    public constructor(obj?: Partial<CommercialFishingShipFilters>) {
        Object.assign(this, obj);
    }
}