import { TransportationLogBookPageRegisterDTO } from '@app/models/generated/dtos/TransportationLogBookPageRegisterDTO';

export class TransportationPageRecordChanged {
    public page!: TransportationLogBookPageRegisterDTO;
    public viewMode: boolean = false;

    public constructor(obj?: Partial<TransportationPageRecordChanged>) {
        Object.assign(this, obj);
    }
}