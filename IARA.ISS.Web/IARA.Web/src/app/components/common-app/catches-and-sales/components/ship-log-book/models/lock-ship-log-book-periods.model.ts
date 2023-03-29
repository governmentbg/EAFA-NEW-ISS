export class LockShipLogBookPeriodsModel {
    public lockShipOver12MLogBookAfterHours!: number;
    public lockShip10M12MLogBookAfterHours!: number;
    public lockShipUnder10MLogBookAfterDays!: number;

    public constructor(obj?: Partial<LockShipLogBookPeriodsModel>) {
        Object.assign(this, obj);
    }
}