import { AdmissionLogBookPageRegisterDTO } from '@app/models/generated/dtos/AdmissionLogBookPageRegisterDTO';

export class AdmissionPageRecordChanged {
    public page!: AdmissionLogBookPageRegisterDTO;
    public viewMode: boolean = false;

    public constructor(obj?: Partial<AdmissionPageRecordChanged>) {
        Object.assign(this, obj);
    }
}