import { AquacultureLogBookPageRegisterDTO } from '@app/models/generated/dtos/AquacultureLogBookPageRegisterDTO';

export class AquaculturePageRecordChanged {
    public page!: AquacultureLogBookPageRegisterDTO;
    public viewMode: boolean = false;

    public constructor(obj?: Partial<AquaculturePageRecordChanged>) {
        Object.assign(this, obj);
    }
}