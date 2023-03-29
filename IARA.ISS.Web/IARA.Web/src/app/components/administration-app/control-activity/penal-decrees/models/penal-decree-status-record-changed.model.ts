import { PenalDecreeStatusEditDTO } from '@app/models/generated/dtos/PenalDecreeStatusEditDTO';

export class PenalDecreeStatusRecordChanged {
    public status!: PenalDecreeStatusEditDTO;
    public viewMode: boolean = false;

    public constructor(obj?: Partial<PenalDecreeStatusRecordChanged>) {
        Object.assign(this, obj);
    }
}