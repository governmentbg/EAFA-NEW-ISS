import { FirstSaleLogBookPageRegisterDTO } from '@app/models/generated/dtos/FirstSaleLogBookPageRegisterDTO';

export class FirstSalePageRecordChanged {
    public page!: FirstSaleLogBookPageRegisterDTO;
    public viewMode: boolean = false;

    public constructor(obj?: Partial<FirstSalePageRecordChanged>) {
        Object.assign(this, obj);
    }
}