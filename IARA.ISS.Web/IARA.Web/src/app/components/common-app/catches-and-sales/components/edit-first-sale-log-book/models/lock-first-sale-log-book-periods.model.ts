export class LockFirstSaleLogBookPeriodsModel {
    public lockFirstSaleBelow200KLogBookAfterHours!: number;
    public lockFirstSaleAbove200KLogBookAfterHours!: number;

    public constructor(obj?: Partial<LockFirstSaleLogBookPeriodsModel>) {
        Object.assign(this, obj);
    }
}